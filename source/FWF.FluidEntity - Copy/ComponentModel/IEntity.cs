using System;

namespace FWF.FluidEntity.ComponentModel
{
    public interface IEntity : IEntityId, IEntityVersion
    {
        string Name { get; }

        DateTime Created { get; set; }

        Guid CreatedBy { get; set; }

        DateTime LastModified { get; set; }

        Guid LastModifiedBy { get; set; }

        Guid LastSourceSystemId { get; set; }

        bool IsDeleted { get; set; }
    }
}
