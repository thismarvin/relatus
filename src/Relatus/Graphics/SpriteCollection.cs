using System;
using System.Collections.Generic;

namespace Relatus.Graphics
{
    public class SpriteCollection : DrawCollection<Sprite>
    {
        public const uint MaxSpriteElementsBatchSize = SpriteElements.MaxBatchSize;

        public SpriteCollection(BatchExecution execution, uint batchSize) : base(execution, batchSize)
        {
            if (this.execution == BatchExecution.DrawElements && this.batchSize > MaxSpriteElementsBatchSize)
                throw new RelatusException($"DrawElements does not support support a batch size greater than {MaxSpriteElementsBatchSize}.", new ArgumentOutOfRangeException());
        }

        public SpriteCollection(BatchExecution execution, uint batchSize, IEnumerable<Sprite> entries) : base(execution, batchSize, entries)
        {
        }

        protected override DrawGroup<Sprite> CreateDrawGroup(Sprite sprite)
        {
            return execution switch
            {
                BatchExecution.DrawElements => new SpriteElements(batchSize, sprite.Texture, sprite.RenderOptions),
                BatchExecution.DrawElementsInstanced => new SpriteElementsInstanced(batchSize, sprite.Texture, sprite.RenderOptions),
                _ => throw new RelatusException("Unknown batch execution type.", new ArgumentException()),
            };
        }
    }
}
