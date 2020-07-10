using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Graphics
{
    public class PolygonCollection : DrawCollection<Polygon>, IDisposable
    {
        public PolygonCollection() : base(100000)
        {
        }

        protected override DrawGroup<Polygon> CreateDrawGroup(Polygon currentEntry, int capacity)
        {
            return new PolygonGroup(currentEntry.Geometry, capacity);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    for (int i = 0; i < groups.Length; i++)
                    {
                        ((IDisposable)groups[i]).Dispose();
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~PolygonCollection()
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
    }
}
