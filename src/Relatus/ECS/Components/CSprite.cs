using Microsoft.Xna.Framework.Graphics;
using Relatus.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.ECS
{
    public class CSprite : IComponent
    {
        public Texture2D Texture { get; set; }
        public RectangleF SampleRegion { get; set; }

        public CSprite(Texture2D texture, RectangleF sampleRegion)
        {
            Texture = texture;
            SampleRegion = new RectangleF((int)sampleRegion.X, (int)sampleRegion.Y, (int)sampleRegion.Width, (int)sampleRegion.Height);
        }
    }
}
