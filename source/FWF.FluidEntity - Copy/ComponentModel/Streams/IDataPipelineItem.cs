namespace FWF.FluidEntity.ComponentModel.Streams
{
    public interface IDataPipelineItem
    {

        void Handle(IStreamReader inputReader, IStreamWriter outputWriter);

    }
}
