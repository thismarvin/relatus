using System;

namespace Relatus.Industry
{
    public class Worker : Entity, IDisposable
    {
        public uint SSN { get; private set; }

        public Worker(uint ssn)
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
