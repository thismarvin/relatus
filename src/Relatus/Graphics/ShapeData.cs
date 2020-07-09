using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Relatus.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Graphics
{
    class ShapeData : IDisposable
    {
        internal bool Managed { get; set; }
        public VertexBuffer Geometry { get; private set; }
        public IndexBuffer Indices { get; private set; }        
        public Vector3[] Vertices { get; private set; }
        public int TotalTriangles { get => Indices.IndexCount / 3; }
        public int TotalVertices { get => Geometry.VertexCount; }
        
        public ShapeData(Vector3[] vertices, short[] indices)
        {
            Geometry = new VertexBuffer(Engine.Graphics.GraphicsDevice, typeof(VertexPosition), vertices.Length, BufferUsage.WriteOnly);
            Geometry.SetData(vertices);

            Indices = new IndexBuffer(Engine.Graphics.GraphicsDevice, typeof(short), indices.Length, BufferUsage.WriteOnly);
            Indices.SetData(indices);
            
            Vertices = vertices;
        }

        internal ShapeData(Vector3[] vertices, short[] indices, bool managed) : this(vertices, indices)
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
                    Geometry.Dispose();
                    Indices.Dispose();
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
