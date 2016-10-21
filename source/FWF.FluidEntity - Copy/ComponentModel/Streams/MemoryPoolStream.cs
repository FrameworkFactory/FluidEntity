using System.IO;

namespace FWF.FluidEntity.ComponentModel.Streams
{
    public class MemoryPoolStream : MemoryStream, IStreamReader, IStreamWriter
    {

        // TODO: Pull required buffers from a buffer pool rather than derive from MemoryStream

        public MemoryPoolStream()
        {

        }

        public MemoryPoolStream(byte[] data) : base(data)
        {

        }

        public MemoryPoolStream(int capacity) : base(capacity)
        {

        }


    }
}
