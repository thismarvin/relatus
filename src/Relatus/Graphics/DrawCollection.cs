using System;
using System.Collections.Generic;

namespace Relatus.Graphics
{
    public abstract class DrawCollection<T> : IDisposable
    {
        protected readonly List<DrawGroup<T>> groups;
        protected readonly BatchExecution execution;
        protected readonly uint batchSize;

        internal DrawCollection(BatchExecution execution, uint batchSize)
        {
            this.execution = execution;
            this.batchSize = batchSize;

            groups = new List<DrawGroup<T>>();
        }

        internal DrawCollection(BatchExecution execution, uint batchSize, IEnumerable<T> entries) : this(execution, batchSize)
        {
            AddRange(entries);
            ApplyChanges();
        }

        protected abstract DrawGroup<T> CreateDrawGroup(T entry);

        public bool Add(T entry)
        {
            if (groups.Count == 0)
            {
                groups.Add(CreateDrawGroup(entry));
                groups[^1].Add(entry);

                return true;
            }

            if (groups[^1].Add(entry))
                return true;

            groups.Add(CreateDrawGroup(entry));
            groups[^1].Add(entry);

            return false;
        }

        public DrawCollection<T> AddRange(IEnumerable<T> entries)
        {
            foreach (T entry in entries)
            {
                Add(entry);
            }

            return this;
        }

        public DrawCollection<T> Clear()
        {
            groups.Clear();

            return this;
        }

        public DrawCollection<T> ApplyChanges()
        {
            for (int i = 0; i < groups.Count; i++)
            {
                groups[i].ApplyChanges();
            }

            return this;
        }

        public DrawCollection<T> Draw(Camera camera)
        {
            for (int i = 0; i < groups.Count; i++)
            {
                groups[i].Draw(camera);
            }

            return this;
        }

        #region IDisposable Support
        private bool disposedValue;

        protected virtual void OnDispose()
        {

        }

        public void Dispose()
        {
            if (!disposedValue)
            {
                for (int i = 0; i < groups.Count; i++)
                {
                    if (groups[i] is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }

                OnDispose();

                disposedValue = true;
            }
        }
        #endregion
    }
}
