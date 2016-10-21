using System;

namespace FWF.FluidEntity.Logging
{
    public interface ILogFactory : IRunnable
    {
        ILog CreateForType<T>();
        ILog CreateForType(Type type);
        ILog CreateForType(object instance);
        ILog Create(string logFullName);
    }
}
