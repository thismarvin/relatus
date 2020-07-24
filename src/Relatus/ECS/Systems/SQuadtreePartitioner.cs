using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.ECS
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
        /// <param name="targetFPS">The target framerate the system will update in.</param>
        public SQuadtreePartitioner(MorroFactory factory, RectangleF boundary, int nodeCapacity, int targetFPS) : base(factory, targetFPS)
        {
            partitioner = new Quadtree<PartitionerEntry>(boundary, nodeCapacity);
        }
    }
}
