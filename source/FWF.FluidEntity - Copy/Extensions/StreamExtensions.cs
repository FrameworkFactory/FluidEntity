using System;
using System.IO;
using FWF.FluidEntity.ComponentModel.Streams;

namespace FWF.FluidEntity
{
    public static class StreamExtensions
    {

        public static void CopyTo(this IStreamReader streamReader, IStreamWriter streamWriter, int bufferSize = 8192)
        {
            byte[] buffer = new byte[bufferSize];
            int count;

            while ((count = streamReader.Read(buffer, 0, buffer.Length)) != 0)
            {
                streamWriter.Write(buffer, 0, count);
            }
        }

        public static IStreamReaderWriter Wrap(this Stream stream)
        {
            return new WrappedStream(stream);
        }

    }
}
