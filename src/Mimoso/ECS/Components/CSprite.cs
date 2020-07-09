using Microsoft.Xna.Framework.Graphics;
using Mimoso.Core;
using Mimoso.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mimoso.ECS
{
    class CSprite : IComponent
    {
        public Texture2D SpriteSheet { get; set; }
        public Microsoft.Xna.Framework.Rectangle Source { get; set; }
        public SpriteEffects SpriteEffect { get; set; }

        public CSprite(string name)
        {
            SpriteData spriteData = SpriteManager.GetSpriteData(name);
            SpriteSheet = AssetManager.GetImage(spriteData.SpriteSheet);
            Source = new Microsoft.Xna.Framework.Rectangle(spriteData.X, spriteData.Y, spriteData.Width, spriteData.Height);
        }
    }
}
