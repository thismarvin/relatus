using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Relatus.ECS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Graphics
{
    public struct VertexTransformColor : IVertexType
    {
        public Vector3 Scale { get; private set; }
        public Vector3 RotationOffset { get; private set; }
        public Vector3 Translation { get; private set; }
        public float Rotation { get; private set; }
        public Color Color { get; private set; }

        public VertexTransformColor(Vector3 scale, Vector2 rotationOffset, float rotation, Vector3 translation, Color color)
        {
            Scale = scale;
            RotationOffset = new Vector3(rotationOffset.X, rotationOffset.Y, 0);
            Rotation = rotation;
            Translation = translation;
            Color = color;
        }

        public VertexTransformColor(CPosition position, CDimension dimension, CTransform transform, CColor color)
        {
            Scale = new Vector3(dimension.Width * transform.Scale.X, dimension.Height * transform.Scale.Y, transform.Scale.Z);
            RotationOffset = new Vector3(transform.RotationOffset.X, transform.RotationOffset.Y, 0);
            Rotation = transform.Rotation;
            Translation = new Vector3(position.X + transform.Translation.X, position.Y + transform.Translation.Y, position.Z + transform.Translation.Z);
            Color = color.Color;
        }

        public VertexDeclaration VertexDeclaration { get { return vertexDeclaration; } }
        private static readonly VertexDeclaration vertexDeclaration;

        static VertexTransformColor()
        {
            vertexDeclaration = new VertexDeclaration
            (
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 1),
                new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Position, 2),
                new VertexElement(24, VertexElementFormat.Vector3, VertexElementUsage.Position, 3),
                new VertexElement(36, VertexElementFormat.Single, VertexElementUsage.Position, 4),
                new VertexElement(40, VertexElementFormat.Color, VertexElementUsage.Color, 5)
            );
        }
    }
}
