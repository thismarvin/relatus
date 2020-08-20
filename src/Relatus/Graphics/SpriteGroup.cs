namespace Relatus.Graphics
{
    internal abstract class SpriteGroup : DrawGroup<BetterSprite>
    {
        protected SpriteGroup(BatchExecution execution, uint batchSize) : base(execution, batchSize)
        {
        }
    }
}

