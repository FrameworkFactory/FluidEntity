using System;

namespace FWF.FluidEntity.ComponentModel
{
    public interface IEntityHistory : IEntityId, IEntityVersion
    {
        string LastModifiedBy { get; set; }
        DateTime LastModified { get; set; }
    }
}
