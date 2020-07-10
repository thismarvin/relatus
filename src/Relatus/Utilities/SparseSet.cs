using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Utilities
{
    /// <summary>
    /// A data structure that stores a set of <see cref="uint"/> that all fall within a given range.
    /// </summary>
    public class SparseSet : IEnumerable<uint>
    {
        public int Count { get => (int)n; }

        private readonly uint[] dense;
        private readonly uint[] sparse;
        /// <summary>
        /// The maximum amount of elements allowed inside the sparse set AND the maximum value allowed inside the set.
        /// </summary>
        private readonly uint u;
        /// <summary>
        /// The current index of the sparse set.
        /// </summary>
        private uint n;

        /// <summary>
        /// Creates a data structure that stores a set of <see cref="uint"/> that all fall within a given range. 
        /// </summary>
        /// <param name="range">The maximum amount of elements allowed inside the sparse set AND the maximum value allowed inside the set.</param>
        public SparseSet(int range)
        {
            u = (uint)range;
            dense = new uint[u];
            sparse = new uint[u];
        }

        /// <summary>
        /// Adds a given <see cref="uint"/> to the set.
        /// </summary>
        /// <param name="k">The element to add to the set.</param>
        /// <returns>Whether or not the <see cref="uint"/> was successfully added to the set.</returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public bool Add(uint k)
        {
            if (!(0 <= k && k < u))
                throw new IndexOutOfRangeException("Index was outside the bounds of the array. A SparseSet cannot contain a value less than 0 or greater than its range.");

            if (Contains(k))
                return false;

            dense[n] = k;
            sparse[k] = n;
            n++;

            return true;
        }

        /// <summary>
        /// Adds the elements of a given collection to the set.
        /// </summary>
        /// <param name="collection">A collection of <see cref="uint"/> that will be added to the set.</param>
        public void AddRange(IEnumerable<uint> collection)
        {
            foreach (uint i in collection)
            {
                Add(i);
            }
        }

        /// <summary>
        /// Removes a given <see cref="uint"/> from the set. 
        /// </summary>
        /// <param name="k">The element to remove from the set.</param>
        public bool Remove(uint k)
        {
            if (!Contains(k))
                return false;

            n--;

            for (int i = Array.IndexOf(dense, k); i < n; i++)
            {
                dense[i] = dense[i + 1];
                sparse[dense[i + 1]] = (uint)i;
            }

            return true;
        }

        /// <summary>
        /// Returns whether or not a given <see cref="uint"/> is in the set.
        /// </summary>
        /// <param name="k">The element to find in the set.</param>
        /// <returns>Whether or not a given <see cref="uint"/> is in the set.</returns>
        public bool Contains(uint k)
        {
            return k < u && sparse[k] < n && dense[sparse[k]] == k;
        }

        /// <summary>
        /// Removes all elements from the set.
        /// </summary>
        public void Clear()
        {
            n = 0;
        }

        public uint this[uint i]
        {
            get => dense[i];
            set
            {
                dense[i] = value;
                sparse[value] = i;
            }
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("{ ");
            for (int i = 0; i < n; i++)
            {
                stringBuilder.Append(dense[i]);

                if (i != n - 1)
                {
                    stringBuilder.Append(", ");
                }
            }
            stringBuilder.Append(" }");

            return stringBuilder.ToString();
        }

        public IEnumerator<uint> GetEnumerator()
        {
            return new SparseSetEnumerator(dense, n);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new SparseSetEnumerator(dense, n);
        }

        internal sealed class SparseSetEnumerator : IEnumerator<uint>
        {
            private readonly uint[] dense;
            private readonly uint n;

            private int index;

            public uint Current { get { return dense[index]; } }

            object IEnumerator.Current { get { return dense[index]; } }

            public SparseSetEnumerator(uint[] dense, uint n)
            {
                this.dense = dense;
                this.n = n;
                index = -1;
            }

            public void Dispose()
            {

            }

            public bool MoveNext()
            {
                index++;
                return index < n;
            }

            public void Reset()
            {
                index = -1;
            }
        }
    }
}
