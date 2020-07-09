using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Graphics.Effects
{
    public class Palette : FX
    {
        public Color[] Colors { get; private set; }

        public Palette() : base("Palette")
        {
            //Colors = new Color[] { new Color(8, 24, 32), new Color(48, 104, 80), new Color(136, 192, 112), new Color(224, 248, 208) };
            //Colors = new Color[] { new Color(14, 56, 15), new Color(48, 98, 48), new Color(139, 172, 15), new Color(155, 188, 15) };
            Colors = new Color[] { new Color(0, 0, 0), new Color(0, 0, 255), new Color(225, 0, 255), new Color(255, 255, 255) };

            Initialize();
        }

        protected override void Initialize()
        {
            Vector3[] colorsAsVectors = new Vector3[Colors.Length];
            for (int i = 0; i < colorsAsVectors.Length; i++)
            {
                colorsAsVectors[i] = Colors[i].ToVector3();
            }

            Effect.Parameters["Palette"].SetValue(colorsAsVectors);
        }
    }
}
