using Microsoft.Xna.Framework.Graphics;
using System;

// !FIXME: This class is redundant. It does everything Mesh already does...

namespace Relatus.Graphics
{
    public class GeometryData : IDisposable
    {
        public readonly Mesh Mesh;

        public readonly VertexBuffer VertexPositionBuffer;
        public readonly IndexBuffer IndexBuffer;

        internal bool Managed { get; set; }

        public GeometryData(Mesh mesh)
        {
            Mesh = mesh;

            // !FIXME: Why are we creating a whole new VertextBuffer when Mesh's VertexBuffer will do??
            VertexPositionBuffer = new VertexBuffer(Engine.Graphics.GraphicsDevice, typeof(VertexPosition), Mesh.Vertices.Length, BufferUsage.WriteOnly);
            VertexPositionBuffer.SetData(Mesh.Vertices);

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
                VertexPositionBuffer.Dispose();
                IndexBuffer.Dispose();

                disposedValue = true;
            }
        }
        #endregion
    }
}
