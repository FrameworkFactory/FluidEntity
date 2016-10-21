using System.Collections.Generic;

namespace FWF.FluidEntity.Data
{
    public interface IProc
    {
        string Name { get; }

        IEnumerable<IProcParameter> Parameters { get; }
    }
}
