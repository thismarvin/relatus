using Microsoft.Xna.Framework.Graphics;

namespace Relatus.Graphics
{
    public struct VertexMaterial : IVertexType
    {
        public float Shininess { get; private set; }

        public VertexDeclaration VertexDeclaration => vertexDeclaration;

        private static readonly VertexDeclaration vertexDeclaration;

        static VertexMaterial()
        {
            vertexDeclaration = new VertexDeclaration
            (
                new VertexElement(0, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 1)
            );
        }

        public VertexMaterial(float shininess)
        {
            Shininess = shininess;
        }
    }
}

