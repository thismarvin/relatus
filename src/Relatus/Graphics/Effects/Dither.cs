using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Graphics.Effects
{
    public class Dither : FX
    {
        public Texture2D Texture { get; private set; }

        public Dither(Texture2D texture) : base("Dither")
        {
            Texture = texture;

            Initialize();
        }

        public void SetTexture(Texture2D texture)
        {
            Texture = texture;

            Initialize();
        }

        protected override void Initialize()
        {
            Effect.Parameters["TextureWidth"].SetValue((float)Texture.Width);
            Effect.Parameters["TextureHeight"].SetValue((float)Texture.Height);

            //Color[] Colors = new Color[] { new Color(14, 56, 15), new Color(48, 98, 48), new Color(139, 172, 15), new Color(155, 188, 15) };
            //Color[] Colors = new Color[] { new Color(8, 24, 32), new Color(48, 104, 80), new Color(136, 192, 112), new Color(224, 248, 208) };
            //Color[] Colors = new Color[] { new Color(0, 0, 0), new Color(85, 0, 0), new Color(170, 0, 0), new Color(255, 0, 0) };
            Color[] Colors = new Color[] { new Color(0, 0, 0), new Color(0, 0, 255), new Color(225, 0, 255), new Color(255, 255, 255) };
            Vector3[] colorsAsVectors = new Vector3[Colors.Length];
            for (int i = 0; i < colorsAsVectors.Length; i++)
            {
                colorsAsVectors[i] = Colors[i].ToVector3();
            }

            Effect.Parameters["Palette"].SetValue(colorsAsVectors);
        }
    }
}
