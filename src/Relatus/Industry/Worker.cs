using System;

namespace Relatus.Industry
{
    public class Worker : Entity, IDisposable
    {
        public int SSN { get; private set; }

        public Worker(int ssn)
        {
            SSN = ssn;
        }

        protected virtual void OnDispose()
        {
        }

        #region IDisposable Support
        private bool disposedValue;

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
