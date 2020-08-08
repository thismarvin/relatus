using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Graphics
{
    public struct VertexColor : IVertexType
    {
        public Color Color { get; set; }

        public VertexDeclaration VertexDeclaration => vertexDeclaration;

        private static readonly VertexDeclaration vertexDeclaration;

        static VertexColor()
        {
            vertexDeclaration = new VertexDeclaration
            (
                new VertexElement(0, VertexElementFormat.Color, VertexElementUsage.Color, 0)
            );
        }

        public VertexColor(Color color)
        {
            Color = color;
        }
    }
}

