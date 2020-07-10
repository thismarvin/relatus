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
        /// <param name="scene">The scene this system will exist in.</param>
        /// <param name="nodeCapacity">The total amount of entities that exist in a node before overflowing into a new tree.</param>
        /// <param name="targetFPS">The target framerate the system will update in.</param>
        public SQuadtreePartitioner(Scene scene, int nodeCapacity, int targetFPS) : base(scene, targetFPS)
        {
            partitioner = new Quadtree<PartitionerEntry>(scene.SceneBounds, nodeCapacity);
        }
    }
}
