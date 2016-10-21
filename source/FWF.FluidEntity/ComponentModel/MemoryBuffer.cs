using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FWF.FluidEntity.ComponentModel
{
    /// <summary>
    /// Wrapper around a shared memory buffer obtained from BufferPool
    /// </summary>
    public class MemoryBuffer : DisposableObject
    {
        private readonly BufferPool _bufferPool;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);
        private static readonly TimeSpan SlimLockTimeout = TimeSpan.FromSeconds(1);

        private byte[] _buffer;
        
        public MemoryBuffer()
        {
            _bufferPool = BufferPool.Create();
        }

        public MemoryBuffer(BufferPool bufferPool)
        {
            if (bufferPool == null)
            {
                throw new ArgumentNullException();
            }

            _bufferPool = bufferPool;
        }

        /// <summary>
        /// Returns the size of the actual data in the memory buffer,
        /// as opposed to the size of the entire buffer
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// Returns the entire allocated memory buffer
        /// </summary>
        /// <returns></returns>
        public byte[] GetBuffer()
        {
            return _buffer;
        }

        public static implicit operator byte[](MemoryBuffer mb)
        {
            return mb.GetBuffer();
        }

        /// <summary>
        /// Reads stream data into a shared memory buffer
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public async Task ReadAsync(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException();
            }

            if (!stream.CanRead)
            {
                throw new InvalidOperationException("Stream is not readable");
            }

            try
            {
                if (await _semaphore.WaitAsync(SlimLockTimeout))
                {
                    Length = 0;

                    if (_buffer == null)
                    {
                        _buffer = _bufferPool.GetBuffer();
                    }

                    if (stream.CanSeek)
                    {
                        if (stream.Length > _buffer.Length)
                        {
                            throw new StreamExceedsMemoryBufferSizeException();
                        }
                    }

                    var index = 0;
                    var bytesRead = 0;
                    
                    do
                    {
                        bytesRead = await stream.ReadAsync(_buffer, index, (_buffer.Length - index));
                        index += bytesRead;

                        if (index > _buffer.Length)
                        {
                            throw new StreamExceedsMemoryBufferSizeException();
                        }

                    } while (bytesRead > 0);

                    Length = index;
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Writes buffer data to a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public async Task WriteAsync(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException();
            }

            if (_buffer == null)
            {
                throw new InvalidOperationException("No data to write");
            }

            if (!stream.CanWrite)
            {
                throw new InvalidOperationException("Stream is not writeable");
            }

            try
            {
                if (await _semaphore.WaitAsync(SlimLockTimeout))
                {
                    await stream.WriteAsync(_buffer, 0, Length);
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Releases the memory buffer back to the pool for re-use
        /// </summary>
        public void ReleaseBuffer()
        {
            if (_buffer == null)
            {
                return;
            }

            try
            {
                if (_semaphore.Wait(SlimLockTimeout))
                {
                    _bufferPool.ReleaseBufferToPool(ref _buffer);
                    _buffer = null;
                    Length = 0;
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public override void Dispose(bool disposing)
        {
            try
            {
                ReleaseBuffer();

                if (disposing && !IsDisposed)
                {
                    _semaphore.Dispose();
                }
            }
            catch
            {
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}
