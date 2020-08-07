using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Graphics
{
    public struct VertexTexture : IVertexType
    {
        public Vector2 TextureCoordinates { get; set; }

        public VertexDeclaration VertexDeclaration => vertexDeclaration;

        private static readonly VertexDeclaration vertexDeclaration;

        static VertexTexture()
        {
            vertexDeclaration = new VertexDeclaration
            (
                new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
            );
        }

        public VertexTexture(Vector2 textureCoordinates)
        {
            TextureCoordinates = textureCoordinates;
        }
    }
}

