using System.Collections.Generic;

namespace Relatus.Graphics
{
    public abstract class Batcher<T>
    {
        protected BatchExecution execution;
        protected uint batchSize;
        protected Camera camera;

        internal Batcher()
        {

        }

        public abstract Batcher<T> Begin();
        public abstract Batcher<T> Add(params T[] entry);
        public abstract Batcher<T> End();

        public Batcher<T> SetBatchExecution(BatchExecution execution)
        {
            this.execution = execution;

            return this;
        }

        public Batcher<T> SetBatchSize(uint batchSize)
        {
            this.batchSize = batchSize;

            return this;
        }

        public Batcher<T> AttachCamera(Camera camera)
        {
            this.camera = camera;

            return this;
        }

        public Batcher<T> AddRange(IEnumerable<T> entries)
        {
            foreach (T entry in entries)
            {
                Add(entry);
            }

            return this;
        }
    }
}
