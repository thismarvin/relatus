using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Graphics
{
    public struct VertexTextureTransform : IVertexType
    {
        public Color Color { get; private set; }
        public Vector3 Scale { get; private set; }
        public Vector3 RotationOffset { get; private set; }
        public Vector3 Translation { get; private set; }
        public float Rotation { get; private set; }

        public VertexDeclaration VertexDeclaration => vertexDeclaration;

        private static readonly VertexDeclaration vertexDeclaration;
        static VertexTextureTransform()
        {
            vertexDeclaration = new VertexDeclaration
            (
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 1),
                new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 2),
                new VertexElement(20, VertexElementFormat.Color, VertexElementUsage.Color, 3),
                new VertexElement(36, VertexElementFormat.Vector3, VertexElementUsage.Position, 4),
                new VertexElement(48, VertexElementFormat.Vector3, VertexElementUsage.Position, 5),
                new VertexElement(60, VertexElementFormat.Single, VertexElementUsage.Position, 6)
            );
        }

        public VertexTextureTransform(Color color, Vector3 scale, Vector2 rotationOffset, float rotation, Vector3 translation)
        {
            Color = color;
            Scale = scale;
            RotationOffset = new Vector3(rotationOffset.X, rotationOffset.Y, 0);
            Rotation = rotation;
            Translation = translation;
        }
    }
}
