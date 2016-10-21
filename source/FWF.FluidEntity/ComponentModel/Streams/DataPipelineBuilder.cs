using System.Collections.Generic;

namespace FWF.FluidEntity.ComponentModel.Streams
{
    public class DataPipelineBuilder
    {

        private readonly DataPipeline _dataPipeline;
        private readonly List<IDataPipelineItem> _items = new List<IDataPipelineItem>();

        private IStreamReader _inputReader;
        private bool _disposeInputOnComplete;
        private IStreamWriter _outputWriter;
        private bool _disposeOutputOnComplete;

        public DataPipelineBuilder(DataPipeline dataPipeline)
        {
            _dataPipeline = dataPipeline;
        }

        public DataPipelineBuilder SetInput(IStreamReader sourceReader, bool disposeOnComplete = false)
        {
            _inputReader = sourceReader;
            _disposeInputOnComplete = disposeOnComplete;

            return this;
        }

        public DataPipelineBuilder SetOutput(IStreamWriter outputWriter, bool disposeOnComplete = false)
        {
            _outputWriter = outputWriter;
            _disposeOutputOnComplete = disposeOnComplete;

            return this;
        }

        public DataPipelineBuilder Attach(IDataPipelineItem dataPipelineItem)
        {
            _items.Add(dataPipelineItem);

            return this;
        }

        public DataPipelineBuilder Attach(DataPipeline dataPipeline)
        {
            var builder = dataPipeline.Build();

            if (!ReferenceEquals(builder.Input, null))
            {
                SetInput(builder.Input);
            }
            if (!ReferenceEquals(builder.Output, null))
            {
                SetOutput(builder.Output);
            }

            foreach (var dataPipelineItem in builder.Items)
            {
                Attach(dataPipelineItem);
            }

            return this;
        }

        public IStreamReader Input
        {
            get { return _inputReader; }
        }

        public bool DisposeInputOnComplete
        {
            get { return _disposeInputOnComplete; }
        }

        public IStreamWriter Output
        {
            get { return _outputWriter; }
        }

        public bool DisposeOutputOnComplete
        {
            get { return _disposeOutputOnComplete; }
        }

        public IEnumerable<IDataPipelineItem> Items
        {
            get { return _items; }
        }

        public void Go()
        {
            _dataPipeline.Execute(this);
        }

    }
}
