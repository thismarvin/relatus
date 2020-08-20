using System;
using System.Collections.Generic;

namespace Relatus.Graphics
{
    public abstract class DrawGroup<T> : IDisposable
    {
        public BatchExecution Execution { get; private set; }
        public uint BatchSize { get; protected set; }

        internal DrawGroup(BatchExecution execution, uint batchSize)
        {
            Execution = execution;
            BatchSize = batchSize;
        }

        public abstract bool Add(T entry);
        public abstract DrawGroup<T> ApplyChanges();
        public abstract DrawGroup<T> Draw(Camera camera);

        public DrawGroup<T> AddRange(IEnumerable<T> entries)
        {
            foreach (T entry in entries)
            {
                Add(entry);
            }

            return this;
        }

        #region IDisposable Support
        private bool disposedValue;

        protected virtual void OnDispose()
        {

        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    OnDispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
