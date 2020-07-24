using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Utilities
{
    internal class BMFontShader : IDisposable
    {
        public Effect Effect { get; private set; }
        public Color TextColor { get; private set; }
        public Color OutlineColor { get; private set; }
        public Color AAColor { get; private set; }

        internal BMFontShader(Color textColor, Color outlineColor, Color aaColor)
        {
            Effect = AssetManager.GetEffect("Relatus_BMFontShader").Clone();

            TextColor = textColor;
            OutlineColor = outlineColor;
            AAColor = aaColor;

            Effect.Parameters["TextColor"].SetValue(TextColor.ToVector4());
            Effect.Parameters["OutlineColor"].SetValue(OutlineColor.ToVector4());
            Effect.Parameters["AAColor"].SetValue(AAColor.ToVector4());
        }

        internal BMFontShader SetTextColor(Color color)
        {
            TextColor = color;

            Effect.Parameters["TextColor"].SetValue(TextColor.ToVector4());

            return this;
        }

        internal BMFontShader SetOutlineColor(Color color)
        {
            OutlineColor = color;

            Effect.Parameters["OutlineColor"].SetValue(OutlineColor.ToVector4());

            return this;
        }

        internal BMFontShader SetAAColor(Color color)
        {
            AAColor = color;

            Effect.Parameters["AAColor"].SetValue(AAColor.ToVector4());

            return this;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Effect.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~BMFontShader()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
