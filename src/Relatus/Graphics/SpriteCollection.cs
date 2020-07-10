using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Graphics
{
    public class SpriteCollection : DrawCollection<Sprite>
    {
        public SpriteCollection() : base(2048)
        {
        }

        protected override DrawGroup<Sprite> CreateDrawGroup(Sprite currentEntry, int capacity)
        {
            return new SpriteGroup(currentEntry.BlendState, currentEntry.SamplerState, currentEntry.Effect, capacity);
        }
    }
}
