using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus
{
    /// <summary>
    /// Provides functionality to be processed by any given <see cref="Partitioner{T}"/>.
    /// </summary>
    public interface IPartitionable
    {
        int Identifier { get; set; }
        RectangleF Span { get; set; }
    }
}
