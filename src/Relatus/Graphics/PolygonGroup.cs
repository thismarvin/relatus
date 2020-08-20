namespace Relatus.Graphics
{
    internal abstract class PolygonGroup : DrawGroup<Polygon>
    {
        protected PolygonGroup(BatchExecution execution, uint batchSize) : base(execution, batchSize)
        {
        }
    }
}
