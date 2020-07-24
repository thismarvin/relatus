using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Graphics
{
    public struct VertexColorTexture : IVertexType
    {
        public Color Color { get; set; }
        public Vector2 TextureCoordinates { get; set; }

        public VertexDeclaration VertexDeclaration => vertexDeclaration;

        private static readonly VertexDeclaration vertexDeclaration;
        static VertexColorTexture()
        {
            vertexDeclaration = new VertexDeclaration
            (
                new VertexElement(0, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                new VertexElement(4, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
            );
        }

        public VertexColorTexture(Color color, Vector2 textureCoordinates)
        {
            Color = color;
            TextureCoordinates = textureCoordinates;
        }
    }
}
