namespace FWF.FluidEntity.Data
{
    public interface IWriteDataSet<in T> : IPendingChanges where T : class
    {
        void Add(T item);
        void Update(T item);
        void Remove(T item);
    }
}
