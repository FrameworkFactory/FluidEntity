using System.Collections.Generic;

namespace FWF.FluidEntity.Data
{
    public interface IProcResult
    {
        IProc Procedure { get; }
        int ReturnCode { get; }
    }

    public interface IProcResult<out T> : IProcResult
    {
        IEnumerable<T> Results { get; }
    }
}
