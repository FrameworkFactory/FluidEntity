namespace FWF.FluidEntity.ComponentModel.Streams
{
    public class NoOpStreamWriter : DisposableObject, IStreamWriter
    {

        public void Write(byte[] buffer, int offset, int count)
        {

        }
    }
}
