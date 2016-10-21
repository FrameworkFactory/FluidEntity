using System;

namespace FWF.FluidEntity.ComponentModel
{
    public class StreamExceedsMemoryBufferSizeException : Exception
    {
        public StreamExceedsMemoryBufferSizeException()
        { }

        public StreamExceedsMemoryBufferSizeException(string message)
            : base(message)
        { }

        public StreamExceedsMemoryBufferSizeException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
