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

        public VertexDeclaration VertexDeclaration => vertexDeclaration;

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

        public VertexTransform(Vector3 scale, Vector2 rotationOffset, float rotation, Vector3 translation)
        {
            Scale = scale;
            RotationOffset = new Vector3(rotationOffset.X, rotationOffset.Y, 0);
            Rotation = rotation;
            Translation = translation;
        }
    }
}
