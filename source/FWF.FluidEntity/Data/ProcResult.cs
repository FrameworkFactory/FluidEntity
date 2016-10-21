using System.Collections.Generic;

namespace FWF.FluidEntity.Data
{
    public class ProcResult : IProcResult
    {
        public IProc Procedure { get; set; }

        public int ReturnCode { get; set; }
    }

    public class ProcResult<T> : ProcResult, IProcResult<T>
    {
        public IEnumerable<T> Results { get; set; }
    }
}
