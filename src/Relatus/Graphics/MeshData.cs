using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Relatus.Graphics
{
    public class MeshData : IDisposable
    {
        public readonly Vector3[] Vertices;
        public readonly Vector3[] Normals;
        public readonly Vector3[] Tangents;
        public readonly Vector2[] TextureCoordinates;

        public readonly short[] Indices;

        public readonly int TotalVertices;
        public readonly int TotalIndices;
        public readonly int TotalTriangles;

        public readonly VertexBuffer VertexBuffer;
        public readonly IndexBuffer IndexBuffer;

        private static readonly GraphicsDevice graphicsDevice;
        private static readonly VertexDeclaration vertexDeclaration;

        static MeshData()
        {
            graphicsDevice = Engine.Graphics.GraphicsDevice;

            vertexDeclaration = new VertexDeclaration
            (
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                new VertexElement(32, VertexElementFormat.Vector3, VertexElementUsage.Tangent, 0)
            );
        }

        public MeshData(Vector3[] vertices, Vector3[] normals, Vector2[] textureCoordinates, short[] indices)
        {
            Vertices = vertices;
            Normals = normals;
            TextureCoordinates = textureCoordinates;
            Indices = indices;

            Tangents = CalculateTangents(Vertices, Normals, TextureCoordinates, indices);

            TotalVertices = vertices.Length;
            TotalIndices = indices.Length;
            TotalTriangles = TotalIndices / 3;

            VertexBuffer = new VertexBuffer(Engine.Graphics.GraphicsDevice, vertexDeclaration, TotalVertices, BufferUsage.WriteOnly);
            VertexBuffer.SetData(0, Vertices, 0, Vertices.Length, vertexDeclaration.VertexStride);
            VertexBuffer.SetData(12, Normals, 0, Normals.Length, vertexDeclaration.VertexStride);
            VertexBuffer.SetData(24, TextureCoordinates, 0, TextureCoordinates.Length, vertexDeclaration.VertexStride);
            VertexBuffer.SetData(32, Tangents, 0, Tangents.Length, vertexDeclaration.VertexStride);

            IndexBuffer = new IndexBuffer(graphicsDevice, typeof(short), Indices.Length, BufferUsage.WriteOnly);
            IndexBuffer.SetData(Indices);
        }

        public static MeshData CreateFromModelMeshPart(ModelMeshPart mesh, bool swapWindingOrder = true)
        {
            int totalVertices = mesh.NumVertices;

            VertexPositionNormalTexture[] data = new VertexPositionNormalTexture[totalVertices];
            mesh.VertexBuffer.GetData(data);

            Vector3[] vertices = new Vector3[totalVertices];
            Vector3[] normals = new Vector3[totalVertices];
            Vector2[] textureCoordinates = new Vector2[totalVertices];

            for (int i = 0; i < totalVertices; i++)
            {
                vertices[i] = data[i].Position;
                normals[i] = data[i].Normal;
                textureCoordinates[i] = data[i].TextureCoordinate;
            }

            short[] indices = new short[mesh.IndexBuffer.IndexCount];
            mesh.IndexBuffer.GetData(indices);

            if (swapWindingOrder)
            {
                for (int i = 0; i < indices.Length; i += 3)
                {
                    short first = indices[i];
                    short last = indices[i + 2];

                    indices[i] = last;
                    indices[i + 2] = first;
                }
            }

            return new MeshData(vertices, normals, textureCoordinates, indices);
        }

        // The following code was taken from MonoGame's MeshHelper class (https://github.com/MonoGame/MonoGame/blob/661de69369ecf0f4e078551b81836586bf6c2c67/MonoGame.Framework.Content.Pipeline/Graphics/MeshHelper.cs#L168).
        private static Vector3[] CalculateTangents(Vector3[] vertices, Vector3[] normals, Vector2[] textureCoordinates, short[] indices)
        {
            Vector3[] tan1 = new Vector3[vertices.Length];

            for (int index = 0; index < indices.Length; index += 3)
            {
                short i1 = indices[index + 0];
                short i2 = indices[index + 1];
                short i3 = indices[index + 2];

                Vector2 w1 = textureCoordinates[i1];
                Vector2 w2 = textureCoordinates[i2];
                Vector2 w3 = textureCoordinates[i3];

                float s1 = w2.X - w1.X;
                float s2 = w3.X - w1.X;
                float t1 = w2.Y - w1.Y;
                float t2 = w3.Y - w1.Y;

                float denom = s1 * t2 - s2 * t1;

                if (Math.Abs(denom) < float.Epsilon)
                {
                    // The triangle UVs are zero sized one dimension.
                    //
                    // So we cannot calculate the vertex tangents for this
                    // one trangle, but maybe it can with other trangles.
                    continue;
                }

                float r = 1.0f / denom;

                Vector3 v1 = vertices[i1];
                Vector3 v2 = vertices[i2];
                Vector3 v3 = vertices[i3];

                float x1 = v2.X - v1.X;
                float x2 = v3.X - v1.X;
                float y1 = v2.Y - v1.Y;
                float y2 = v3.Y - v1.Y;
                float z1 = v2.Z - v1.Z;
                float z2 = v3.Z - v1.Z;

                Vector3 sdir = new Vector3()
                {
                    X = (t2 * x1 - t1 * x2) * r,
                    Y = (t2 * y1 - t1 * y2) * r,
                    Z = (t2 * z1 - t1 * z2) * r,
                };

                tan1[i1] += sdir;
                tan1[i2] += sdir;
                tan1[i3] += sdir;
            }

            Vector3[] tangents = new Vector3[vertices.Length];

            // At this point we have all the vectors accumulated, but we need to average
            // them all out. So we loop through all the final verts and do a Gram-Schmidt
            // orthonormalize, then make sure they're all unit length.
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 n = normals[i];
                Vector3 t = tan1[i];

                if (t.LengthSquared() < float.Epsilon)
                {
                    // We couldn't find a good tanget for this vertex.
                    //
                    // Rather than set them to zero which could produce
                    // errors in other parts of the pipeline, we just take
                    // a guess at something that may look ok.

                    t = Vector3.Cross(n, Vector3.UnitX);

                    if (t.LengthSquared() < float.Epsilon)
                    {
                        t = Vector3.Cross(n, Vector3.UnitY);
                    }

                    tangents[i] = Vector3.Normalize(t);
                    continue;
                }

                // Gram-Schmidt orthogonalize
                // TODO: This can be zero can cause NaNs on
                // normalize... how do we fix this?
                Vector3 tangent = t - n * Vector3.Dot(n, t);
                tangent = Vector3.Normalize(tangent);

                tangents[i] = tangent;
            }

            return tangents;
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
