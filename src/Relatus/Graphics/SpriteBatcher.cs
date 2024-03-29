using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Relatus.Graphics
{
    public class SpriteBatcher : Batcher<Sprite>
    {
        private readonly List<List<Sprite>> sprites;
        private int index;

        internal SpriteBatcher()
        {
            sprites = new List<List<Sprite>>();
            execution = BatchExecution.DrawElements;
            batchSize = SpriteElements.MaxBatchSize;
        }

        public override Batcher<Sprite> Begin()
        {
            sprites.Clear();

            sprites.Add(new List<Sprite>((int)batchSize));
            index = 0;

            return this;
        }

        public override Batcher<Sprite> Add(params Sprite[] sprites)
        {
            for (int i = 0; i < sprites.Length; i++)
            {
                this.sprites[^1].Add(sprites[i]);
                index++;

                if (index >= batchSize)
                {
                    this.sprites.Add(new List<Sprite>((int)batchSize));
                    index = 0;
                }
            }

            return this;
        }

        public override Batcher<Sprite> End()
        {
            if (camera == null)
                throw new RelatusException("A Camera has not been attached yet. Make sure to call AttachCamera(camera).", new ArgumentNullException());

            for (int i = 0; i < sprites.Count; i++)
            {
                if (i + 1 == sprites.Count)
                {
                    using SpriteCollection spriteCollection = new SpriteCollection(execution, (uint)index, sprites[i]);
                    spriteCollection.Draw(camera);
                }
                else
                {
                    using SpriteCollection spriteCollection = new SpriteCollection(execution, batchSize, sprites[i]);
                    spriteCollection.Draw(camera);
                }
            }

            camera = null;
            execution = BatchExecution.DrawElements;
            batchSize = SpriteElements.MaxBatchSize;

            return this;
        }
    }
}
