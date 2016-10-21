using System;
using System.Linq;
using FWF.FluidEntity.Logging;

namespace FWF.FluidEntity.ComponentModel.Streams
{
    public class DataPipeline : DisposableObject
    {
        private readonly DataPipelineBuilder _builder;

        private readonly MemoryPoolStream _chainPipelineInputStream;
        private readonly MemoryPoolStream _chainPipelineOutputStream;

        private readonly ILog _log;

        public DataPipeline(ILog log)
        {
            _log = log;

            _builder = new DataPipelineBuilder(this);

            _chainPipelineInputStream = new MemoryPoolStream(8192);
            _chainPipelineOutputStream = new MemoryPoolStream(8192);
        }

        public override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_builder.DisposeInputOnComplete)
                {
                    if (!ReferenceEquals(_builder.Input, null))
                    {
                        _builder.Input.Dispose();
                    }
                }
                if (_builder.DisposeOutputOnComplete)
                {
                    if (!ReferenceEquals(_builder.Output, null))
                    {
                        _builder.Output.Dispose();
                    }
                }

                _chainPipelineInputStream.Dispose();
                _chainPipelineOutputStream.Dispose();
            }
            base.Dispose(disposing);
        }

        public DataPipelineBuilder Build()
        {
            return _builder;
        }

        internal void Execute(DataPipelineBuilder builder)
        {
            if (ReferenceEquals(builder, null))
            {
                throw new ArgumentNullException("builder");
            }
            if (ReferenceEquals(builder.Input, null))
            {
                throw new InvalidOperationException("Missing pipeline input");
            }
            if (ReferenceEquals(builder.Output, null))
            {
                throw new InvalidOperationException("Missing pipeline output");
            }

            var itemList = builder.Items.ToList();
            var lastItem = itemList.Count - 1;

            if (itemList.Count == 0)
            {
                _log.Warn("Pipeline contains no items - copying source to destination.");

                builder.Input.CopyTo(builder.Output);

                return;
            }

            if (itemList.Count == 1)
            {
                var onlyItem = itemList.First();

                onlyItem.Handle(builder.Input, builder.Output);

                return;
            }

            for (int i = 0; i < itemList.Count; i++)
            {
                var pipelineItem = itemList[i];

                /*
                First Item == Source -> Pipeline Input
                Middle ITem == Pipeline Input -> Pipeline Output
                Last Item == Pipeline Input -> Output 
                */

                if (i == 0)
                {
                    pipelineItem.Handle(builder.Input, _chainPipelineInputStream);

                    _chainPipelineInputStream.Position = 0;
                }
                else if (i == lastItem)
                {
                    pipelineItem.Handle(_chainPipelineInputStream, builder.Output);
                }
                else
                {
                    pipelineItem.Handle(_chainPipelineInputStream, _chainPipelineOutputStream);

                    // Copy output to input
                    _chainPipelineInputStream.Position = 0;
                    _chainPipelineOutputStream.Position = 0;
                    _chainPipelineOutputStream.CopyTo(_chainPipelineInputStream);
                    _chainPipelineInputStream.Position = 0;
                    _chainPipelineOutputStream.Position = 0;
                }

            }

        }



    }
}
