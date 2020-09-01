namespace Relatus.Graphics
{
    internal abstract class SpriteGroup : DrawGroup<Sprite>
    {
        protected SpriteGroup(BatchExecution execution, uint batchSize) : base(execution, batchSize)
        {
        }
    }
}

