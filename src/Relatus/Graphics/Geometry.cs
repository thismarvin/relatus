using System;
using Microsoft.Xna.Framework;

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

        internal VertexTransform GetVertexTransform()
        {
            Vector3 rotation = new Vector3(transform.EulerAngles.Pitch, transform.EulerAngles.Yaw, transform.EulerAngles.Roll);
            return new VertexTransform(transform.Translation, transform.Scale, transform.Origin, rotation);
        }

        internal VertexColor GetVertexColor()
        {
            return new VertexColor(tint);
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
