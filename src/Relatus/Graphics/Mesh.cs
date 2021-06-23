using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Relatus.Graphics
{
    public class Mesh : IDisposable
    {
        public readonly Vector3[] Vertices;
        public readonly short[] Indices;

        public readonly int TotalVertices;
        public readonly int TotalTriangles;

        public readonly VertexBuffer VertexPositionBuffer;
        public readonly IndexBuffer IndexBuffer;

        public Mesh(Vector3[] vertices, short[] indices)
        {
            Vertices = vertices;
            Indices = indices;

            TotalVertices = Vertices.Length;
            TotalTriangles = Indices.Length / 3;

            VertexPositionBuffer = new VertexBuffer(Engine.Graphics.GraphicsDevice, typeof(VertexPosition), Vertices.Length, BufferUsage.WriteOnly);
            VertexPositionBuffer.SetData(Vertices);

            IndexBuffer = new IndexBuffer(Engine.Graphics.GraphicsDevice, typeof(short), Indices.Length, BufferUsage.WriteOnly);
            IndexBuffer.SetData(Indices);
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
