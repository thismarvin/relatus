using Microsoft.Xna.Framework.Graphics;

namespace Relatus.Graphics
{
    public class Material
    {
        public Texture2D DiffuseMap { get; set; }
        public Texture2D NormalMap { get; set; }
        public Texture2D SpecularMap { get; set; }

        public Material()
        {
            DiffuseMap = GraphicsManager.SimpleTexture;
            NormalMap = GraphicsManager.SimpleNormalTexture;
            SpecularMap = GraphicsManager.SimpleTexture;
        }
    }
}
