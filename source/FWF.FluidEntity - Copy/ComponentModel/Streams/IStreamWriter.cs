using System;

namespace FWF.FluidEntity.ComponentModel.Streams
{
    public interface IStreamWriter : IDisposable
    {

        void Write(byte[] buffer, int offset, int count);

    }
}
