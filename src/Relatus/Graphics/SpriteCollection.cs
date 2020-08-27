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
            switch (execution)
            {
                case BatchExecution.DrawElements:
                    return new SpriteElements(batchSize, sprite.Texture, sprite.RenderOptions);

                case BatchExecution.DrawElementsInstanced:
                    return new SpriteElementsInstanced(batchSize, sprite.Texture, sprite.RenderOptions);

                default:
                    throw new RelatusException("Unknown batch execution type.", new ArgumentException());
            }
        }
    }
}
