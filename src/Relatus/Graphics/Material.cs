using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Relatus.Graphics
{
    public class Material
    {
        public Texture2D Texture { get; set; }
        public Color Tint { get; set; }
        public MaterialProperties Properties { get; set; }
    }
}
