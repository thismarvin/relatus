namespace Relatus.Graphics
{
    internal abstract class GeometryGroup : DrawGroup<Geometry>
    {
        protected GeometryGroup(BatchExecution execution, uint batchSize) : base(execution, batchSize)
        {
        }
    }
}
