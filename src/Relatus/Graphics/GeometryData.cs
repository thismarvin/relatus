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

        public bool Equals(GeometryData other)
        {
            return Mesh == other.Mesh;
        }

        #region IDisposable Support
        private bool disposedValue;

        public void Dispose()
        {
            if (!disposedValue)
            {
                VertexBuffer.Dispose();
                IndexBuffer.Dispose();

                disposedValue = true;
            }
        }
        #endregion
    }
}
