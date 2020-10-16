using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Relatus.Graphics
{
    public static class BenevolentBatcher
    {
        private static readonly GraphicsDevice graphicsDevice;

        private static readonly VertexDeclaration standardSpriteSchema;
        private static readonly VertexDeclaration instancedSpriteSchema0;
        private static readonly VertexDeclaration instancedSpriteSchema1;

        static BenevolentBatcher()
        {
            graphicsDevice = Engine.Graphics.GraphicsDevice;

            standardSpriteSchema = new VertexDeclaration
            (
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                new VertexElement(20, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                new VertexElement(36, VertexElementFormat.Vector4, VertexElementUsage.Position, 1),
                new VertexElement(52, VertexElementFormat.Vector4, VertexElementUsage.Position, 2),
                new VertexElement(68, VertexElementFormat.Vector4, VertexElementUsage.Position, 3),
                new VertexElement(84, VertexElementFormat.Vector4, VertexElementUsage.Position, 4)
            );

            instancedSpriteSchema0 = new VertexDeclaration
            (
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
            );
            instancedSpriteSchema1 = new VertexDeclaration
            (
                new VertexElement(0, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.Position, 1),
                new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.Position, 2),
                new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.Position, 3),
                new VertexElement(64, VertexElementFormat.Vector4, VertexElementUsage.Position, 4)
            );
        }

        public static Batch CreateInstancedSpriteBatch(params BetterSprite[] data)
        {
            return CreateInstancedSpriteBatch(data as IEnumerable<BetterSprite>);
        }

        public static Batch CreateInstancedSpriteBatch(IEnumerable<BetterSprite> data)
        {
            Texture2D texture = graphicsDevice.Textures[0] as Texture2D ?? throw new RelatusException("", new ArgumentException());

            int batchSize = data.Count();
            int totalVertices = batchSize * 4;

            Vector3[] positions = new Vector3[totalVertices];
            Vector2[] textureCoords = new Vector2[totalVertices];

            Color[] colors = new Color[batchSize];
            Vector4[] modelR0 = new Vector4[batchSize];
            Vector4[] modelR1 = new Vector4[batchSize];
            Vector4[] modelR2 = new Vector4[batchSize];
            Vector4[] modelR3 = new Vector4[batchSize];

            int index = 0;
            int iindex = 0;

            foreach (var entry in data)
            {
                positions[index + 0] = new Vector3(0, 0, 0);
                positions[index + 1] = new Vector3(0, -1, 0);
                positions[index + 2] = new Vector3(1, -1, 0);
                positions[index + 3] = new Vector3(1, 0, 0);

                Vector2[] _textureCoords = CreateTextureCoordsFromImageRegion(texture, entry.ImageRegion, entry.SpriteMirroring);
                textureCoords[index + 0] = _textureCoords[0];
                textureCoords[index + 1] = _textureCoords[1];
                textureCoords[index + 2] = _textureCoords[2];
                textureCoords[index + 3] = _textureCoords[3];

                Color color = entry.Tint;
                colors[iindex] = color;

                Vector4[] model = CreateModelFromMatrix(entry.Transform.Matrix);
                modelR0[iindex] = model[0];
                modelR1[iindex] = model[1];
                modelR2[iindex] = model[2];
                modelR3[iindex] = model[3];

                index += 4;
                iindex++;
            }

            int vertexStride = instancedSpriteSchema0.VertexStride;
            VertexBuffer vertexBuffer = new VertexBuffer(graphicsDevice, instancedSpriteSchema0, totalVertices, BufferUsage.WriteOnly);

            vertexBuffer.SetData(0, positions, 0, totalVertices, vertexStride);
            vertexBuffer.SetData(12, textureCoords, 0, totalVertices, vertexStride);

            int vertexStride2 = instancedSpriteSchema1.VertexStride;
            VertexBuffer colorBuffer = new VertexBuffer(graphicsDevice, instancedSpriteSchema1, batchSize, BufferUsage.WriteOnly);
            colorBuffer.SetData(0, colors, 0, batchSize, vertexStride2);
            colorBuffer.SetData(16, modelR0, 0, batchSize, vertexStride2);
            colorBuffer.SetData(32, modelR1, 0, batchSize, vertexStride2);
            colorBuffer.SetData(48, modelR2, 0, batchSize, vertexStride2);
            colorBuffer.SetData(64, modelR3, 0, batchSize, vertexStride2);

            short[] indices = new short[6]
            {
                0, 1, 2,
                0, 2, 3
            };

            IndexBuffer indexBuffer = new IndexBuffer(graphicsDevice, typeof(short), indices.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData(indices);

            return new Batch()
            {
                VertexBufferBindings = new VertexBufferBinding[] { new VertexBufferBinding(vertexBuffer), new VertexBufferBinding(colorBuffer, 0, 1) },
                IndexBuffer = indexBuffer,
                BatchExecution = BatchExecution.DrawElementsInstanced,
                PrimitiveType = PrimitiveType.TriangleList,
                TotalPrimitives = batchSize * 2,
                Instances = batchSize
            };
        }

        public static Batch CreateSpriteBatch(params BetterSprite[] data)
        {
            return CreateSpriteBatch(data as IEnumerable<BetterSprite>);
        }

        public static Batch CreateSpriteBatch(IEnumerable<BetterSprite> data)
        {
            Texture2D texture = graphicsDevice.Textures[0] as Texture2D ?? throw new RelatusException("", new ArgumentException());

            int batchSize = data.Count();
            int totalVertices = batchSize * 4;

            Vector3[] positions = new Vector3[totalVertices];
            Vector2[] textureCoords = new Vector2[totalVertices];
            Color[] colors = new Color[totalVertices];
            Vector4[] modelR0 = new Vector4[totalVertices];
            Vector4[] modelR1 = new Vector4[totalVertices];
            Vector4[] modelR2 = new Vector4[totalVertices];
            Vector4[] modelR3 = new Vector4[totalVertices];

            int index = 0;

            foreach (var entry in data)
            {
                positions[index + 0] = new Vector3(0, 0, 0);
                positions[index + 1] = new Vector3(0, -1, 0);
                positions[index + 2] = new Vector3(1, -1, 0);
                positions[index + 3] = new Vector3(1, 0, 0);

                Vector2[] _textureCoords = CreateTextureCoordsFromImageRegion(texture, entry.ImageRegion, entry.SpriteMirroring);
                textureCoords[index + 0] = _textureCoords[0];
                textureCoords[index + 1] = _textureCoords[1];
                textureCoords[index + 2] = _textureCoords[2];
                textureCoords[index + 3] = _textureCoords[3];

                Color color = entry.Tint;
                colors[index + 0] = color;
                colors[index + 1] = color;
                colors[index + 2] = color;
                colors[index + 3] = color;

                Vector4[] model = CreateModelFromMatrix(entry.Transform.Matrix);
                modelR0[index + 0] = model[0];
                modelR0[index + 1] = model[0];
                modelR0[index + 2] = model[0];
                modelR0[index + 3] = model[0];

                modelR1[index + 0] = model[1];
                modelR1[index + 1] = model[1];
                modelR1[index + 2] = model[1];
                modelR1[index + 3] = model[1];

                modelR2[index + 0] = model[2];
                modelR2[index + 1] = model[2];
                modelR2[index + 2] = model[2];
                modelR2[index + 3] = model[2];

                modelR3[index + 0] = model[3];
                modelR3[index + 1] = model[3];
                modelR3[index + 2] = model[3];
                modelR3[index + 3] = model[3];

                index += 4;
            }

            int vertexStride = standardSpriteSchema.VertexStride;
            VertexBuffer vertexBuffer = new VertexBuffer(graphicsDevice, standardSpriteSchema, totalVertices, BufferUsage.WriteOnly);

            vertexBuffer.SetData(0, positions, 0, totalVertices, vertexStride);
            vertexBuffer.SetData(12, textureCoords, 0, totalVertices, vertexStride);
            vertexBuffer.SetData(20, colors, 0, totalVertices, vertexStride);
            vertexBuffer.SetData(36, modelR0, 0, totalVertices, vertexStride);
            vertexBuffer.SetData(52, modelR1, 0, totalVertices, vertexStride);
            vertexBuffer.SetData(68, modelR2, 0, totalVertices, vertexStride);
            vertexBuffer.SetData(84, modelR3, 0, totalVertices, vertexStride);

            short[] indices = new short[batchSize * 6];
            for (int i = 0; i < batchSize; i++)
            {
                int start = 6 * i;
                int buffer = 4 * i;
                indices[start + 0] = (short)(buffer + 0);
                indices[start + 1] = (short)(buffer + 1);
                indices[start + 2] = (short)(buffer + 2);
                indices[start + 3] = (short)(buffer + 0);
                indices[start + 4] = (short)(buffer + 2);
                indices[start + 5] = (short)(buffer + 3);
            }

            IndexBuffer indexBuffer = new IndexBuffer(graphicsDevice, typeof(short), indices.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData(indices);

            return new Batch()
            {
                VertexBufferBindings = new VertexBufferBinding[] { new VertexBufferBinding(vertexBuffer) },
                IndexBuffer = indexBuffer,
                BatchExecution = BatchExecution.DrawElements,
                PrimitiveType = PrimitiveType.TriangleList,
                TotalPrimitives = batchSize * 2,
                Instances = batchSize
            };
        }

        private static Vector2[] CreateTextureCoordsFromImageRegion(Texture2D texture, ImageRegion imageRegion, SpriteMirroringType spriteMirroring)
        {
            float texelWidth = 1f / texture.Width;
            float texelHeight = 1f / texture.Height;

            Vector2 topLeft = new Vector2((float)MathExt.RemapRange(imageRegion.X, 0, texture.Width, 0, 1), (float)MathExt.RemapRange(imageRegion.Y, 0, texture.Height, 0, 1));
            Vector2 topRight = topLeft + new Vector2(texelWidth * imageRegion.Width, 0);
            Vector2 bottomRight = topLeft + new Vector2(texelWidth * imageRegion.Width, texelHeight * imageRegion.Height);
            Vector2 bottomLeft = topLeft + new Vector2(0, texelHeight * imageRegion.Height);

            Vector2[] corners = new Vector2[] { topLeft, bottomLeft, bottomRight, topRight };

            if ((spriteMirroring & SpriteMirroringType.FlipHorizontally) != SpriteMirroringType.None)
            {
                (corners[0], corners[3]) = (corners[3], corners[0]);
                (corners[1], corners[2]) = (corners[2], corners[1]);
            }

            if ((spriteMirroring & SpriteMirroringType.FlipVertically) != SpriteMirroringType.None)
            {
                (corners[0], corners[1]) = (corners[1], corners[0]);
                (corners[2], corners[3]) = (corners[3], corners[2]);
            }

            return corners;
        }

        private static Vector4[] CreateModelFromMatrix(Matrix matrix) =>
            new Vector4[]
            {
                new Vector4(matrix.M11, matrix.M12, matrix.M13, matrix.M14),
                new Vector4(matrix.M21, matrix.M22, matrix.M23, matrix.M24),
                new Vector4(matrix.M31, matrix.M32, matrix.M33, matrix.M34),
                new Vector4(matrix.M41, matrix.M42, matrix.M43, matrix.M44)
            };
    }
}
