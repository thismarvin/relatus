using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Relatus.Graphics
{
    public struct VertexNormal : IVertexType
    {
        public Vector3 Normal { get; set; }

        public VertexDeclaration VertexDeclaration => vertexDeclaration;

        private static readonly VertexDeclaration vertexDeclaration;

        static VertexNormal()
        {
            vertexDeclaration = new VertexDeclaration
            (
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
            );
        }

        public VertexNormal(Vector3 normal)
        {
            Normal = normal;
        }
    }
}

