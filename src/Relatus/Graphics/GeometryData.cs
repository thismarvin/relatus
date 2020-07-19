using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Graphics
{
    public class GeometryData : IDisposable
    {
        internal bool Managed { get; set; }

        public Mesh Mesh { get; private set; }
        public VertexBuffer VertexBuffer { get; private set; }
        public IndexBuffer IndexBuffer { get; private set; }

        public int TotalTriangles => IndexBuffer.IndexCount / 3;
        public int TotalVertices => VertexBuffer.VertexCount;

        public GeometryData(Mesh mesh)
        {
            Mesh = mesh;

            VertexBuffer = new VertexBuffer(Engine.Graphics.GraphicsDevice, typeof(VertexPosition), Mesh.Vertices.Length, BufferUsage.WriteOnly);
            VertexBuffer.SetData(Mesh.Vertices);

            IndexBuffer = new IndexBuffer(Engine.Graphics.GraphicsDevice, typeof(short), Mesh.Indices.Length, BufferUsage.WriteOnly);
            IndexBuffer.SetData(Mesh.Indices);
        }

        internal GeometryData(Mesh mesh, bool managed) : this(mesh)
        {
            Managed = managed;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    VertexBuffer.Dispose();
                    IndexBuffer.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ShapeData()
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
