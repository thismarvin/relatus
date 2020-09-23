using System;

namespace Relatus.Graphics
{
    public abstract class Geometry : Renderable, IDisposable
    {
        public GeometryData GeometryData
        {
            get => geometryData;
            set => AttachGeometry(value);
        }

        protected GeometryData geometryData;

        public Geometry() : base()
        {
        }

        public virtual Renderable AttachGeometry(GeometryData geometryData)
        {
            // Dispose of GeometryData on attach if the curent geometry isn't managed.
            if (!this.geometryData?.Managed ?? false)
            {
                this.geometryData.Dispose();
            }

            this.geometryData = geometryData;

            return this;
        }

        protected virtual void OnDispose()
        {

        }

        #region IDisposable Support
        private bool disposedValue;

        public void Dispose()
        {
            if (!disposedValue)
            {
                if (!geometryData.Managed)
                {
                    geometryData.Dispose();
                }

                OnDispose();

                disposedValue = true;
            }
        }
        #endregion
    }
}
