using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Relatus.Graphics
{
    public struct VertexMaterial : IVertexType
    {
        public Vector3 Ambient { get; private set; }
        public Vector3 Diffuse { get; private set; }
        //public Vector3 Specular { get; private set; }
        //public float Shininess { get; private set; }

        public VertexDeclaration VertexDeclaration => vertexDeclaration;

        private static readonly VertexDeclaration vertexDeclaration;

        static VertexMaterial()
        {
            vertexDeclaration = new VertexDeclaration
            (
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 5),
                new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Position, 6)//,
                //new VertexElement(24, VertexElementFormat.Vector3, VertexElementUsage.Position, 7),
                //new VertexElement(36, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 1)
            );
        }

        public VertexMaterial(Color ambient, Color diffuse)
        {
            Ambient = ambient.ToVector3();
            Diffuse = ambient.ToVector3();
        }
    }
}

