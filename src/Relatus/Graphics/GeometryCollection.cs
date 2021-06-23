using System;
using System.Collections.Generic;

namespace Relatus.Graphics
{
    public class GeometryCollection : DrawCollection<Geometry>
    {
        public GeometryCollection(BatchExecution execution, uint batchSize) : base(execution, batchSize)
        {
        }

        public GeometryCollection(BatchExecution execution, uint batchSize, IEnumerable<Geometry> entries) : base(execution, batchSize, entries)
        {
        }

        protected override DrawGroup<Geometry> CreateDrawGroup(Geometry geometry)
        {
            return execution switch
            {
                BatchExecution.DrawElements => new GeometryElements(batchSize, geometry.GeometryData, geometry.RenderOptions),
                BatchExecution.DrawElementsInstanced => new GeometryElementsInstanced(batchSize, geometry.GeometryData, geometry.RenderOptions),
                _ => throw new RelatusException("Unknown batch execution type.", new ArgumentException()),
            };
        }
    }
}
