using System;
using System.Collections.Generic;

namespace Relatus.Graphics
{
    public class PolygonCollection : DrawCollection<Polygon>
    {
        public PolygonCollection(BatchExecution execution, uint batchSize) : base(execution, batchSize)
        {
        }

        public PolygonCollection(BatchExecution execution, uint batchSize, IEnumerable<Polygon> entries) : base(execution, batchSize, entries)
        {
        }

        protected override DrawGroup<Polygon> CreateDrawGroup(Polygon polygon)
        {
            return execution switch
            {
                BatchExecution.DrawElements => new PolygonElements(batchSize, polygon.GeometryData, polygon.RenderOptions),
                BatchExecution.DrawElementsInstanced => new PolygonElementsInstanced(batchSize, polygon.GeometryData, polygon.RenderOptions),
                _ => throw new RelatusException("Unknown batch execution type.", new ArgumentException()),
            };
        }
    }
}
