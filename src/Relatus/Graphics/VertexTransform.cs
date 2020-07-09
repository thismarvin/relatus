using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Relatus.ECS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Graphics
{
    public struct VertexTransform : IVertexType
    {
        public Vector3 Scale { get; private set; }
        public Vector3 RotationOffset { get; private set; }
        public Vector3 Translation { get; private set; }
        public float Rotation { get; private set; }

        public VertexTransform(Vector3 scale, Vector2 rotationOffset, float rotation, Vector3 translation)
        {
            Scale = scale;
            RotationOffset = new Vector3(rotationOffset.X, rotationOffset.Y, 0);
            Rotation = rotation;
            Translation = translation;
        }

        public VertexTransform(CPosition position, CDimension dimension, CTransform transform)
        {
            Scale = new Vector3(dimension.Width * transform.Scale.X, dimension.Height * transform.Scale.Y, transform.Scale.Z);
            RotationOffset = new Vector3(transform.RotationOffset.X, transform.RotationOffset.Y, 0);
            Rotation = transform.Rotation;
            Translation = new Vector3(position.X + transform.Translation.X, position.Y + transform.Translation.Y, position.Z + transform.Translation.Z);
        }

        public VertexDeclaration VertexDeclaration { get { return vertexDeclaration; } }
        private static readonly VertexDeclaration vertexDeclaration;

        static VertexTransform()
        {
            vertexDeclaration = new VertexDeclaration
            (
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 1),
                new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Position, 2),
                new VertexElement(24, VertexElementFormat.Vector3, VertexElementUsage.Position, 3),
                new VertexElement(36, VertexElementFormat.Single, VertexElementUsage.Position, 4)
            );
        }
    }
}
