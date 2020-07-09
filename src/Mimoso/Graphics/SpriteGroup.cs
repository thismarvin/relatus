using Microsoft.Xna.Framework.Graphics;
using Mimoso.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mimoso.Graphics
{
    class SpriteGroup : DrawGroup<Sprite>
    {
        private readonly BlendState sharedBlendState;
        private readonly SamplerState sharedSamplerState;
        private readonly Effect sharedEffect;

        private static readonly SpriteBatch spriteBatch;

        static SpriteGroup()
        {
            spriteBatch = GraphicsManager.SpriteBatch;
        }

        public SpriteGroup(BlendState sharedBlendState, SamplerState sharedSamplerState, Effect sharedEffect, int capacity) : base(capacity)
        {
            this.sharedBlendState = sharedBlendState;
            this.sharedSamplerState = sharedSamplerState;
            this.sharedEffect = sharedEffect;
        }

        protected override bool ConditionToAdd(Sprite sprite)
        {
            return sprite.BlendState == sharedBlendState && sprite.SamplerState == sharedSamplerState && sprite.Effect == sharedEffect;
        }

        public override void Draw(Camera camera)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, sharedBlendState, sharedSamplerState, null, null, sharedEffect, camera.Transform);
            {
                for (int i = 0; i < groupIndex; i++)
                {
                    group[i]?.ManagedDraw();
                }
            }
            spriteBatch.End();
        }
    }
}
