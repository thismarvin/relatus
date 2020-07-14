using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Graphics
{
    /// <summary>
    /// All of the basic <see cref="FX"/>'s included with Relatus.
    /// </summary>
    public enum EffectType
    {
        Blur,
        ChromaticAberration,
        Dither,
        DropShadow,
        Grayscale,
        Invert,
        Outline,
        Palette,
        Quantize
    }

    /// <summary>
    /// An abstraction of a helper class used to manage the parameters and logic of a custom registered <see cref="Microsoft.Xna.Framework.Graphics.Effect"/>.
    /// Refer to <see cref="Relatus.Graphics.EffectType"/> to see all implementations of this class by Relatus.
    /// </summary>
    public abstract class FX : IDisposable
    {
        public Effect Effect { get; private set; }
        public string EffectType { get; private set; }

        public FX(string type)
        {
            EffectType = type;
            Effect = AssetManager.GetEffect(EffectType).Clone();
        }

        protected abstract void Initialize();

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
        // ~FX()
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
