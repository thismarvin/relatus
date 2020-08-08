using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Graphics
{
    public struct VertexTransform : IVertexType
    {
        public Vector3 Translation { get; private set; }
        public Vector3 Scale { get; private set; }
        public Vector3 Origin { get; private set; }
        public Vector3 Rotation { get; private set; }
        
        public VertexDeclaration VertexDeclaration => vertexDeclaration;

        private static readonly VertexDeclaration vertexDeclaration;

        static VertexTransform()
        {
            vertexDeclaration = new VertexDeclaration
            (
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 1),
                new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Position, 2),
                new VertexElement(24, VertexElementFormat.Vector3, VertexElementUsage.Position, 3),
                new VertexElement(36, VertexElementFormat.Vector3, VertexElementUsage.Position, 4)
            );
        }

        public VertexTransform(Vector3 translation, Vector3 scale, Vector3 origin, Vector3 rotation)
        {
            Translation = translation;
            Scale = scale;
            Origin = origin;
            Rotation = rotation;
        }
    }
}

