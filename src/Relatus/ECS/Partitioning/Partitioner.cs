using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.ECS
{
    /// <summary>
    /// An abstraction of a class that implements spatial partitioning capabilities.
    /// </summary>
    /// <typeparam name="T">The element stored within the partitioner.</typeparam>
    public abstract class Partitioner<T> where T : IPartitionable
    {
        public RectangleF Boundary
        {
            get => boundary;
            set
            {
                boundary = value;
                Clear();
                Initialize();
            }
        }

        private RectangleF boundary;

        /// <summary>
        /// Creates an abstraction of a class that implements spatial partitioning capabilities.
        /// </summary>
        /// <param name="boundary">The area that the partitioner will cover.</param>
        internal Partitioner(RectangleF boundary)
        {
            this.boundary = boundary;
        }

        /// <summary>
        /// Includes all logic for creating and setting up a new partitioner.
        /// </summary>
        protected abstract void Initialize();

        /// <summary>
        /// Returns a list of the identifiers of all of the <see cref="IPartitionable"/> entries that are within a given boundary.
        /// </summary>
        /// <param name="bounds">The area to check for <see cref="IPartitionable"/> entries.</param>
        /// <returns>Any <see cref="IPartitionable"/> items found within the given bounds of this partitioner.</returns>
        public abstract List<int> Query(RectangleF bounds);

        /// <summary>
        /// Adds a given <see cref="IPartitionable"/> item to the partitioner.
        /// </summary>
        /// <param name="entry">The <see cref="IPartitionable"/> item to add to the partitioner.</param>
        /// <returns>Whether or not the given item was successfully added to the partitioner.</returns>
        public abstract bool Add(T entry);

        /// <summary>
        /// Removes all items that were inside the partitioner.
        /// </summary>
        public abstract void Clear();

        public void AddRange(IEnumerable<T> entries)
        {
            foreach (T entry in entries)
            {
                Add(entry);
            }
        }
    }
}
