using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.ECS.Bundled
{
    /// <summary>
    /// An implementation of a <see cref="PartitioningSystem"/> that implements a <see cref="Quadtree{T}"/>.
    /// </summary>
    public class SQuadtreePartitioner : PartitioningSystem
    {
        /// <summary>
        /// Creates an implementation of a <see cref="PartitioningSystem"/> that implements a <see cref="Quadtree{T}"/>.
        /// </summary>
        /// <param name="factory">The scene this system will exist in.</param>
        /// <param name="boundary">The area the partitioner the will cover.</param>
        /// <param name="nodeCapacity">The total amount of entities that exist in a node before overflowing into a new tree.</param>
        public SQuadtreePartitioner(MorroFactory factory, RectangleF boundary, uint nodeCapacity) : base(factory)
        {
            partitioner = new Quadtree<PartitionerEntry>(boundary, nodeCapacity);
        }

        public override void EnableDivideAndConquer(uint sections)
        {
            throw new RelatusException("SQuadtreePartitioner was not designed to be compatible with the divide and conquer implementation.", new NotSupportedException());
        }
    }
}
