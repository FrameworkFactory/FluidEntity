using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace FWF.FluidEntity.ComponentModel.Streams
{
    public class AggregateStreamReader : DisposableObject, IStreamReader
    {

        private readonly List<IStreamReader> _streamReaders = new List<IStreamReader>();
        private int _index;

        private readonly byte[] _readBuffer = new byte[8192];
        private bool _isEndOfStream;

        public override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var streamReader in _streamReaders)
                {
                    streamReader.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        public void Add(IStreamReader streamReader)
        {
            _streamReaders.Add(streamReader);
        }

        public int Read([In, Out] byte[] buffer, int offset, int count)
        {
            if (_isEndOfStream)
            {
                return 0;
            }
            if (count > _readBuffer.Length)
            {
                throw new InvalidOperationException("Read count is too large");
            }

            int returnCount;

            while (true)
            {
                var readerItem = _streamReaders[_index];

                var bytesRead = readerItem.Read(_readBuffer, 0, count);

                if (bytesRead > 0)
                {
                    Buffer.BlockCopy(_readBuffer, 0, buffer, offset, bytesRead);

                    returnCount = bytesRead;
                    break;
                }

                if (bytesRead == 0)
                {
                    _index++;
                }

                if (_index > _streamReaders.Count - 1)
                {
                    _isEndOfStream = true;
                    returnCount = 0;
                    break;
                }
            }

            return returnCount;
        }
    }
}
