using Relatus.Maths;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.ECS
{
    /// <summary>
    /// Provides functionality to be processed by any given <see cref="Partitioner{T}"/>.
    /// </summary>
    public interface IPartitionable
    {
        int Identifier { get; set; }
        RectangleF Bounds { get; set; }
    }
}
