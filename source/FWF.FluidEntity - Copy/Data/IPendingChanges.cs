using System.Collections.Generic;

namespace FWF.FluidEntity.Data
{
    public interface IPendingChanges
    {
        void Clear();
        IEnumerable<DataSetItem> PendingChanges { get; }
    }
}
