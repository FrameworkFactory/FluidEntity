namespace FWF.FluidEntity
{
    public interface IRunnable
    {
        bool IsRunning { get; }
        void Start();
        void Stop();
    }
}
