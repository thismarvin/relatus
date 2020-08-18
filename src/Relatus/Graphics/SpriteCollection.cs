using System;

namespace Relatus.Graphics
{
    public class SpriteCollection : DrawCollection<BetterSprite>, IDisposable
    {
        public SpriteCollection() : base(short.MaxValue / 6)
        {
        }

        protected override DrawGroup<BetterSprite> CreateDrawGroup(BetterSprite currentEntry, int capacity)
        {
            return new SpriteGroup(currentEntry.RenderOptions, currentEntry.Texture, capacity);
        }

        #region IDisposable Support
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    for (int i = 0; i < groups.Length; i++)
                    {
                        if (groups[i] is IDisposable disposable)
                        {
                            disposable.Dispose();
                        }
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~SpriteCollection()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
