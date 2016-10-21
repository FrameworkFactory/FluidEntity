using System;
using System.Threading;

namespace FWF.FluidEntity.ComponentModel
{
    /// <summary>
    /// Manages a fixed number of shared memory buffer of fixed size
    /// </summary>
    public class BufferPool
    {
        private readonly object[] _pool;

        public static BufferPool Create()
        {
            return Create(20, Bytes.MB);
        }

        public static BufferPool Create(int poolSize, long bufferLength)
        {
            return new BufferPool(poolSize, bufferLength);
        }

        private BufferPool(int poolSize, long bufferLength)
        {
            PoolSize = poolSize;
            BufferLength = bufferLength;
            _pool = new object[PoolSize];
        }

        internal int PoolSize { get; private set; }
        internal long BufferLength { get; private set; }

        /// <summary>
        /// Returns the first unused memory buffer
        /// </summary>
        /// <returns></returns>
        internal byte[] GetBuffer()
        {
            object tmp;

            for (var i = 0; i < _pool.Length; i++)
            {
                if ((tmp = Interlocked.Exchange(ref _pool[i], null)) != null)
                {
                    return (byte[])tmp;
                }
            }

            return new byte[BufferLength];
        }

        /// <summary>
        /// Resizes a memory buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="toFitAtLeastBytes"></param>
        /// <param name="copyFromIndex"></param>
        /// <param name="copyBytes"></param>
        internal void ResizeAndFlushLeft(ref byte[] buffer, int toFitAtLeastBytes, int copyFromIndex, int copyBytes)
        {
            // try doubling, else match
            int newLength = buffer.Length * 2;
            if (newLength < toFitAtLeastBytes)
            {
                newLength = toFitAtLeastBytes;
            }

            var newBuffer = new byte[newLength];
            if (copyBytes > 0)
            {
                Buffer.BlockCopy(buffer, copyFromIndex, newBuffer, 0, copyBytes);
            }
            if (buffer.Length == BufferLength)
            {
                ReleaseBufferToPool(ref buffer);
            }
            buffer = newBuffer;
        }

        /// <summary>
        /// Marks a memory buffer to be re-used again
        /// </summary>
        /// <param name="buffer"></param>
        internal void ReleaseBufferToPool(ref byte[] buffer)
        {
            if (buffer == null)
            {
                return;
            }
            if (buffer.Length == BufferLength)
            {
                for (var i = 0; i < _pool.Length; i++)
                {
                    if (Interlocked.CompareExchange(ref _pool[i], buffer, null) == null)
                    {
                        break; // found a null - swapped it in
                    }
                }
            }

            buffer = null; // if no space, just drop it on the floor
        }

        /// <summary>
        /// Deletes all memory buffers from pool
        /// </summary>
        internal void Flush()
        {
            for (var i = 0; i < _pool.Length; i++)
            {
                Interlocked.Exchange(ref _pool[i], null); // and drop the old value on the floor
            }
        }
    }
}
