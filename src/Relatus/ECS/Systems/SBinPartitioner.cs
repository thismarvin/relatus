using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.ECS
{
    /// <summary>
    /// An implementation of a <see cref="PartitioningSystem"/> that implements a <see cref="Bin{T}"/>.
    /// </summary>
    public class SBinPartitioner : PartitioningSystem
    {
        /// <summary>
        /// Creates an implementation of a <see cref="PartitioningSystem"/> that implements a <see cref="Bin{T}"/>.
        /// </summary>
        /// <param name="scene">The scene this system will exist in.</param>
        /// <param name="maximumDimension">The maximum expected size of any entity inserted into the bin.</param>
        /// <param name="targetFPS">The target framerate the system will update in.</param>
        public SBinPartitioner(Scene scene, int maximumDimension, int targetFPS) : base(scene, targetFPS)
        {            
            partitioner = new Bin<PartitionerEntry>(scene.SceneBounds, maximumDimension);
        }
    }
}
