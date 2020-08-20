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
            switch (execution)
            {
                case BatchExecution.DrawElements:
                    return new PolygonElements(batchSize, polygon.Geometry, polygon.RenderOptions);

                case BatchExecution.DrawElementsInstanced:
                    return new PolygonElementsInstanced(batchSize, polygon.Geometry, polygon.RenderOptions);

                default:
                    throw new RelatusException("Unknown batch execution type.", new ArgumentException());
            }
        }
    }
}
