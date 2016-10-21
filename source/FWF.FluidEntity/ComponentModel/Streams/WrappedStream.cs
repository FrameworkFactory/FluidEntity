using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace FWF.FluidEntity.ComponentModel.Streams
{
    internal class WrappedStream : DisposableObject, IStreamReaderWriter
    {
        private readonly Stream _stream;

        public WrappedStream(Stream stream)
        {
            _stream = stream;
        }

        public override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _stream.Dispose();
            }

            base.Dispose(disposing);
        }

        public Stream BaseStream
        {
            get { return _stream; }
        }

        [DebuggerStepThrough]
        public int Read([In, Out] byte[] buffer, int offset, int count)
        {
            return _stream.Read(buffer, offset, count);
        }

        [DebuggerStepThrough]
        public void Write(byte[] buffer, int offset, int count)
        {
            _stream.Write(buffer, offset, count);
        }

    }
}
