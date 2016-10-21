
namespace FWF.FluidEntity.Data
{
    public class NoOpDataContextTransaction : IDataContextTransaction
    {
        public void RollBack()
        {
        }

        public void Commit()
        {
        }

        public void Dispose()
        {
        }
    }
}
