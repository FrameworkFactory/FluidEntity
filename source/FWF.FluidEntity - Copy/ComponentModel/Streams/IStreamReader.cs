using System;
using System.Runtime.InteropServices;

namespace FWF.FluidEntity.ComponentModel.Streams
{
    public interface IStreamReader : IDisposable
    {

        int Read([In, Out] byte[] buffer, int offset, int count);

    }
}
