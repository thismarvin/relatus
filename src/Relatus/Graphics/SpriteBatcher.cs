using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Graphics
{
    public class SpriteBatcher
    {
        private readonly List<List<BetterSprite>> sprites;
        private int index;

        private Camera camera;

        private static readonly GraphicsDevice graphicsDevice;

        static SpriteBatcher()
        {
            graphicsDevice = Engine.Graphics.GraphicsDevice;
        }

        internal SpriteBatcher()
        {
            sprites = new List<List<BetterSprite>>();
        }

        public SpriteBatcher Begin()
        {
            sprites.Clear();

            sprites.Add(new List<BetterSprite>((int)SpriteCollection.MaxBatchSize));
            index = 0;

            return this;
        }

        public SpriteBatcher AttachCamera(Camera camera)
        {
            this.camera = camera;

            return this;
        }

        public SpriteBatcher Add(BetterSprite sprite)
        {
            sprites[sprites.Count - 1].Add(sprite);
            index++;

            if (index >= SpriteCollection.MaxBatchSize)
            {
                sprites.Add(new List<BetterSprite>((int)SpriteCollection.MaxBatchSize));
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
            for (int i = 0; i < sprites.Count; i++)
            {
                if (i + 1 == sprites.Count)
                {
                    using (SpriteCollection spriteCollection = new SpriteCollection((uint)index, sprites[i]))
                    {
                        spriteCollection.Draw(camera);
                    }
                }
                else
                {
                    using (SpriteCollection spriteCollection = new SpriteCollection(SpriteCollection.MaxBatchSize, sprites[i]))
                    {
                        spriteCollection.Draw(camera);
                    }
                }
            }

            camera = null;

            return this;
        }
    }
}
