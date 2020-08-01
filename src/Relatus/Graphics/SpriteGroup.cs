using Microsoft.Xna.Framework.Graphics;
using System;

namespace Relatus.Graphics
{
    public class SpriteGroup : DrawGroup<BetterSprite>, IDisposable
    {
        private readonly RenderOptions sharedRenderOptions;
        private readonly Texture2D sharedTexture;
        private readonly VertexColorTexture[] vertexColorTextures;
        private readonly VertexTransform[] vertexTransforms;

        private bool dataChanged;

        private static readonly GraphicsDevice graphicsDevice;
        static SpriteGroup() {
            graphicsDevice = Engine.Graphics.GraphicsDevice;
        }

        public SpriteGroup(RenderOptions sharedRenderOptions, Texture2D sharedTexture, int capacity) : base(capacity)
        {
            this.sharedRenderOptions = sharedRenderOptions;
            this.sharedTexture = sharedTexture;

            vertexColorTextures = new VertexColorTexture[capacity];
            vertexTransforms = new VertexTransform[capacity];
        }

        protected override bool ConditionToAdd(BetterSprite entry)
        {
            return entry.RenderOptions.Equals(sharedRenderOptions) && entry.Texture == sharedTexture;
        }

        public override bool Add(BetterSprite entry){
            if (groupIndex >= capacity)
                return false;

            if (ConditionToAdd(entry))
            {
                //vertexTransforms[groupIndex++] = entry.GetVertexTransform();
                dataChanged = true;
                return true;
            }

            return false;
        }

        public override void Draw(Camera camera)
        {
        }

        #region IDisposable Support
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~SpriteGroup()
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
