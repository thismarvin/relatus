using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Relatus.Utilities
{
    public class BMFontShader : IDisposable
    {
        public Effect Effect { get; private set; }

        public Color TextColor
        {
            get => textColor;
            set => SetTextColor(value);
        }
        public Color OutlineColor
        {
            get => outlineColor;
            set => SetTextColor(value);
        }
        public Color AntiAliasingColor
        {
            get => aaColor;
            set => SetTextColor(value);
        }

        private Color textColor;
        private Color outlineColor;
        private Color aaColor;

        public BMFontShader()
        {
            Effect = AssetManager.GetEffect("Relatus_BMFontShader").Clone();

            textColor = Color.White;
            outlineColor = Color.Black;
            aaColor = Color.Transparent;

            Effect.Parameters["TextColor"].SetValue(textColor.ToVector4());
            Effect.Parameters["OutlineColor"].SetValue(outlineColor.ToVector4());
            Effect.Parameters["AAColor"].SetValue(aaColor.ToVector4());
        }

        public BMFontShader Create()
        {
            return new BMFontShader();
        }

        public BMFontShader SetTextColor(Color color)
        {
            textColor = color;

            Effect.Parameters["TextColor"].SetValue(TextColor.ToVector4());

            return this;
        }

        public BMFontShader SetOutlineColor(Color color)
        {
            outlineColor = color;

            Effect.Parameters["OutlineColor"].SetValue(OutlineColor.ToVector4());

            return this;
        }

        public BMFontShader SetAAColor(Color color)
        {
            aaColor = color;

            Effect.Parameters["AAColor"].SetValue(AntiAliasingColor.ToVector4());

            return this;
        }

        #region IDisposable Support
        private bool disposedValue;

        public void Dispose()
        {
            if (!disposedValue)
            {
                Effect.Dispose();

                disposedValue = true;
            }
        }
        #endregion
    }
}
