using System.Collections.Generic;

namespace FWF.FluidEntity.Data
{
    public class Proc : IProc
    {
        public string Name { get; set; }

        public IEnumerable<IProcParameter> Parameters { get; set; }
    }
}
