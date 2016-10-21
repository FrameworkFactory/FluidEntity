using System.Collections.Generic;

namespace FWF.FluidEntity
{
    public interface IRandom
    {
        int Any();
        int Any(int maxValue);
        int Any(int minValue, int maxValue);
        long Any(long minValue, long maxValue);

        T Any<T>(IList<T> items);

        string AnyString(int size);
        string AnyString(int size, bool alphaOnly);

        bool AnyBoolean();

        byte[] AnyBytes(int length);
    }
}
