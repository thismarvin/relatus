using System.Collections.Generic;

namespace Relatus.Graphics
{
    internal abstract class SpriteGroup
    {
        public BatchExecution Execution { get; private set; }
        public uint BatchSize { get; protected set; }

        public SpriteGroup(BatchExecution execution, uint batchSize)
        {
            Execution = execution;
            BatchSize = batchSize;
        }

        public abstract bool Add(BetterSprite sprite);
        public abstract SpriteGroup ApplyChanges();
        public abstract SpriteGroup Draw(Camera camera);

        public SpriteGroup AddRange(IEnumerable<BetterSprite> sprites)
        {
            foreach (BetterSprite sprite in sprites)
            {
                Add(sprite);
            }

            return this;
        }
    }
}
