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
        /// <param name="factory">The scene this system will exist in.</param>
        /// <param name="boundary">The area the partitioner will cover.</param>
        /// <param name="maximumDimension">The maximum expected size of any entity inserted into the bin.</param>
        public SBinPartitioner(MorroFactory factory, RectangleF boundary, uint maximumDimension) : base(factory)
        {
            partitioner = new Bin<PartitionerEntry>(boundary, maximumDimension);
        }

        public override void EnableDivideAndConquer(uint sections)
        {
            throw new RelatusException("SBinParitioner was not designed to be compatible with the divide and conquer implementation.", new NotSupportedException());
        }
    }
}
