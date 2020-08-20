using System;
using System.Collections.Generic;

namespace Relatus.Graphics
{
    public class SpriteCollection : IDisposable
    {
        public const uint MaxSpriteElementsBatchSize = SpriteElements.MaxBatchSize;

        private readonly List<SpriteGroup> spriteGroups;
        private readonly BatchExecution execution;
        private readonly uint batchSize;

        public SpriteCollection(BatchExecution execution, uint batchSize)
        {
            this.execution = execution;

            if (this.execution == BatchExecution.DrawElements && batchSize > MaxSpriteElementsBatchSize)
                throw new RelatusException($"DrawElements does not support support a batch size greater than {MaxSpriteElementsBatchSize}.", new ArgumentOutOfRangeException());

            this.batchSize = batchSize;

            spriteGroups = new List<SpriteGroup>();
        }

        public SpriteCollection(BatchExecution execution, uint batchSize, IEnumerable<BetterSprite> sprites) : this(execution, batchSize)
        {
            AddRange(sprites);
            ApplyChanges();
        }

        public bool Add(BetterSprite sprite)
        {
            if (spriteGroups.Count == 0)
            {
                spriteGroups.Add(CreateSpriteGroup(sprite));
                spriteGroups[spriteGroups.Count - 1].Add(sprite);

                return true;
            }

            if (spriteGroups[spriteGroups.Count - 1].Add(sprite))
                return true;

            spriteGroups.Add(CreateSpriteGroup(sprite));
            spriteGroups[spriteGroups.Count - 1].Add(sprite);

            return false;
        }

        public SpriteCollection AddRange(IEnumerable<BetterSprite> sprites)
        {
            foreach (BetterSprite sprite in sprites)
            {
                Add(sprite);
            }

            return this;
        }

        public SpriteCollection Clear()
        {
            spriteGroups.Clear();

            return this;
        }

        public SpriteCollection ApplyChanges()
        {
            for (int i = 0; i < spriteGroups.Count; i++)
            {
                spriteGroups[i].ApplyChanges();
            }

            return this;
        }

        private SpriteGroup CreateSpriteGroup(BetterSprite sprite)
        {
            switch (execution)
            {
                case BatchExecution.DrawElements:
                    return new SpriteElements(batchSize, sprite.Texture, sprite.RenderOptions);

                case BatchExecution.DrawElementsInstanced:
                    return new SpriteElementsInstanced(batchSize, sprite.Texture, sprite.RenderOptions);

                default:
                    throw new RelatusException("Unknown collection type.", new ArgumentException());
            }
        }

        public void Draw(Camera camera)
        {
            for (int i = 0; i < spriteGroups.Count; i++)
            {
                spriteGroups[i].Draw(camera);
            }
        }

        #region IDisposable Support
        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    for (int i = 0; i < spriteGroups.Count; i++)
                    {
                        if (spriteGroups[i] is IDisposable disposable)
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
        // ~BetterSpriteCollection()
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
