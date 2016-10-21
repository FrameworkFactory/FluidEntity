using System;

namespace FWF.FluidEntity.ComponentModel
{
    public interface IEntityWithHistory : IEntityId
    {
        string Name { get; }
        DateTime CreateDate { get; set; }
    }
}
