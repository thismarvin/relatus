using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Relatus.Graphics
{
    public class SpriteBatcher
    {
        private readonly List<List<BetterSprite>> sprites;
        private int index;

        private BatchExecution execution;
        private uint batchSize;
        private Camera camera;

        private static readonly GraphicsDevice graphicsDevice;

        static SpriteBatcher()
        {
            graphicsDevice = Engine.Graphics.GraphicsDevice;
        }

        internal SpriteBatcher()
        {
            sprites = new List<List<BetterSprite>>();
            execution = BatchExecution.DrawElements;
            batchSize = SpriteElements.MaxBatchSize;
        }

        public SpriteBatcher SetBatchExecution(BatchExecution execution)
        {
            this.execution = execution;

            return this;
        }

        public SpriteBatcher SetBatchSize(uint batchSize)
        {
            this.batchSize = batchSize;

            return this;
        }

        public SpriteBatcher AttachCamera(Camera camera)
        {
            this.camera = camera;

            return this;
        }

        public SpriteBatcher Begin()
        {
            sprites.Clear();

            sprites.Add(new List<BetterSprite>((int)batchSize));
            index = 0;

            return this;
        }

        public SpriteBatcher Add(BetterSprite sprite)
        {
            sprites[sprites.Count - 1].Add(sprite);
            index++;

            if (index >= batchSize)
            {
                sprites.Add(new List<BetterSprite>((int)batchSize));
                index = 0;
            }

            return this;
        }

        public SpriteBatcher AddRange(IEnumerable<BetterSprite> sprites)
        {
            foreach (BetterSprite sprite in sprites)
            {
                Add(sprite);
            }

            return this;
        }

        public SpriteBatcher End()
        {
            if (camera == null)
                throw new RelatusException("A Camera has not been attached yet. Make sure to call AttachCamera(camera).", new ArgumentNullException());

            for (int i = 0; i < sprites.Count; i++)
            {
                if (i + 1 == sprites.Count)
                {
                    using (SpriteCollection spriteCollection = new SpriteCollection(execution, (uint)index, sprites[i]))
                    {
                        spriteCollection.Draw(camera);
                    }
                }
                else
                {
                    using (SpriteCollection spriteCollection = new SpriteCollection(execution, batchSize, sprites[i]))
                    {
                        spriteCollection.Draw(camera);
                    }
                }
            }

            camera = null;
            execution = BatchExecution.DrawElements;
            batchSize = SpriteElements.MaxBatchSize;

            return this;
        }
    }
}
