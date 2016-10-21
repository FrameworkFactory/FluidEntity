using System;

namespace FWF.FluidEntity.Data
{
    public interface IDataContextTransaction : IDisposable
    {

        void RollBack();

        void Commit();
    }
}
