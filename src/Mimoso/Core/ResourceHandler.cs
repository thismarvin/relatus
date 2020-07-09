using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Mimoso.Core
{
    class ResourceHandler<T> : IDisposable, IEnumerable<T>
    {
        public int Count { get => data.Count; }

        private readonly Dictionary<string, T> data;

        public ResourceHandler()
        {
            data = new Dictionary<string, T>();
        }

        public void Register(string name, T entry)
        {
            string formatedName = FormatName(name);

            if (data.ContainsKey(formatedName))
                throw new MimosoException("An entry with that name already exists; try a different name.", new ArgumentException("An item with the same key has already been added."));

            data.Add(formatedName, entry);
        }

        public T Get(string name)
        {
            string formatedName = FormatName(name);
            VerifyEntry(formatedName);

            return data[formatedName];
        }

        public void Remove(string name)
        {
            string formatedName = FormatName(name);
            VerifyEntry(formatedName);

            if (typeof(T) is IDisposable)
            {
                ((IDisposable)data[formatedName]).Dispose();
            }

            data.Remove(formatedName);
        }

        private string FormatName(string name)
        {
            return name.ToLowerInvariant();
        }

        private void VerifyEntry(string name)
        {
            if (!data.ContainsKey(FormatName(name)))
                throw new MimosoException("An Entry with that name has not been registered.", new KeyNotFoundException());
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (KeyValuePair<string, T> entry in data)
                    {
                        if (entry.Value is IDisposable)
                        {
                            ((IDisposable)entry.Value).Dispose();
                        }
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ResourceHandler()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

        #region IEnumerable Support

        // I am not sure if what I am doing is a big no no, but no one can stop me.

        public IEnumerator<T> GetEnumerator()
        {
            return new ResourceHandlerEnumerator(data);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new ResourceHandlerEnumerator(data);
        }

        internal sealed class ResourceHandlerEnumerator : IEnumerator<T>
        {
            private readonly T[] data;
            private int index;

            public T Current { get { return data[index]; } }

            object IEnumerator.Current { get { return data[index]; } }

            public ResourceHandlerEnumerator(Dictionary<string, T> data)
            {
                this.data = new T[data.Count];

                int i = 0;
                foreach (KeyValuePair<string, T> entry in data)
                {
                    this.data[i++] = entry.Value;
                }

                index = -1;
            }

            public void Dispose()
            {

            }

            public bool MoveNext()
            {
                index++;
                return index < data.Length;
            }

            public void Reset()
            {
                index = -1;
            }
        }
        #endregion
    }
}
