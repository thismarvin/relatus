using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Graphics.Effects
{
    internal class BMFontShader : FX
    {
        public Color TextColor { get; private set; }
        public Color OutlineColor { get; private set; }
        public Color AAColor { get; private set; }

        public BMFontShader(Color textColor, Color outlineColor, Color aaColor) : base("BMFontShader")
        {
            TextColor = textColor;
            OutlineColor = outlineColor;
            AAColor = aaColor;

            Initialize();
        }

        public void SetTextColor(Color color)
        {
            TextColor = color;

            Initialize();
        }

        public void SetOutlineColor(Color color)
        {
            OutlineColor = color;

            Initialize();
        }

        public void SetAAColor(Color color)
        {
            AAColor = color;

            Initialize();
        }

        protected override void Initialize()
        {
            Effect.Parameters["TextColor"].SetValue(TextColor.ToVector4());
            Effect.Parameters["OutlineColor"].SetValue(OutlineColor.ToVector4());
            Effect.Parameters["AAColor"].SetValue(AAColor.ToVector4());
        }
    }
}
