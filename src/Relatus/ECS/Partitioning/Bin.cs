using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.ECS
{
    /// <summary>
    /// An implementation of a <see cref="Partitioner{T}"/> that uses a hashing algorithm to store and retrieve <see cref="IPartitionable"/> entries.
    /// </summary>
    /// <typeparam name="T">The element stored within the partitioner.</typeparam>
    public class Bin<T> : Partitioner<T> where T : IPartitionable
    {
        private RectangleF validatedBoundary;
        private HashSet<T>[] buckets;
        private readonly int powerOfTwo;
        private readonly int bucketSize;
        private int columns;
        private int rows;

        /// <summary>
        /// Creates an implementation of a <see cref="Partitioner{T}"/> that uses a hashing algorithm to store and retrieve <see cref="IPartitionable"/> entries.
        /// </summary>
        /// <param name="boundary">The area that the partitioner will cover.</param>
        /// <param name="maximumDimension">The maximum expected size of any <see cref="IPartitionable"/> entry inserted into the bin.</param>
        public Bin(RectangleF boundary, int maximumDimension) : base(boundary)
        {
            powerOfTwo = (int)Math.Ceiling(Math.Log(maximumDimension, 2));
            bucketSize = 1 << powerOfTwo;

            Initialize();
        }

        protected override void Initialize()
        {
            validatedBoundary = new RectangleF
            (
                (float)MathExt.RemapRange
                (
                    Boundary.X,
                    Boundary.Left,
                    Boundary.Right,
                    0,
                    Boundary.Width
                ),
                (float)MathExt.RemapRange
                (
                    Boundary.Y,
                    Boundary.Bottom,
                    Boundary.Top,
                    0,
                    Boundary.Height
                ),
                Boundary.Width,
                Boundary.Height
            );

            columns = (int)Math.Ceiling(Boundary.Width / bucketSize);
            rows = (int)Math.Ceiling(Boundary.Height / bucketSize);

            buckets = new HashSet<T>[rows * columns];

            for (int i = 0; i < buckets.Length; i++)
            {
                buckets[i] = new HashSet<T>();
            }
        }

        public override List<int> Query(RectangleF bounds)
        {
            List<int> result = new List<int>();

            if (!bounds.Intersects(Boundary))
                return result;

            if (Boundary.CompletelyWithin(bounds))
            {
                for (int i = 0; i < buckets.Length; i++)
                {
                    foreach (T entry in buckets[i])
                    {
                        result.Add(entry.Identifier);
                    }
                }

                return result;
            }

            HashSet<int> unique = new HashSet<int>();
            HashSet<int> ids = GetHashIDs(bounds);

            foreach (int id in ids)
            {
                if (id < 0)
                    continue;

                foreach (T entry in buckets[id])
                {
                    if (unique.Add(entry.Identifier))
                    {
                        result.Add(entry.Identifier);
                    }
                }
            }

            return result;
        }

        public override bool Insert(T entry)
        {
            if (!entry.Bounds.Intersects(Boundary))
                return false;

            HashSet<int> ids = GetHashIDs(entry.Bounds);

            foreach (int id in ids)
            {
                if (id < 0)
                    continue;

                buckets[id].Add(entry);
            }

            return ids.Count > 0;
        }

        public override void Clear()
        {
            if (buckets == null)
                return;

            for (int i = 0; i < buckets.Length; i++)
            {
                buckets[i].Clear();
            }
        }

        private HashSet<int> GetHashIDs(RectangleF bounds)
        {
            RectangleF validatedBoundary = new RectangleF
            (
                (float)MathExt.RemapRange
                (
                    bounds.X,
                    Boundary.Left,
                    Boundary.Right,
                    0,
                    Boundary.Width
                ),
                (float)MathExt.RemapRange
                (
                    bounds.Y,
                    Boundary.Bottom,
                    Boundary.Top,
                    0,
                    Boundary.Height
                ),
                bounds.Width,
                bounds.Height
            );

            RectangleF constrainedBounds = MathExt.ConstrainRectangle(validatedBoundary, this.validatedBoundary);

            List<int> hashes = new List<int>()
            {
                GetHash(constrainedBounds.Left, constrainedBounds.Top),
                GetHash(constrainedBounds.Right, constrainedBounds.Top),
                GetHash(constrainedBounds.Right, constrainedBounds.Bottom),
                GetHash(constrainedBounds.Left, constrainedBounds.Bottom)
            };

            if (constrainedBounds.Width > bucketSize || constrainedBounds.Height > bucketSize)
            {
                int totalRows = (int)Math.Ceiling(constrainedBounds.Height / bucketSize);
                int totalColumns = (int)Math.Ceiling(constrainedBounds.Width / bucketSize);

                for (int y = 0; y <= totalRows; y++)
                {
                    for (int x = 0; x <= totalColumns; x++)
                    {
                        hashes.Add(GetHash(constrainedBounds.X + x * bucketSize, constrainedBounds.Y - y * bucketSize));
                    }
                }
            }

            return new HashSet<int>(hashes);

            int GetHash(float x, float y)
            {
                Vector2 constrainedPosition = MathExt.ConstrainPoint(x, y, this.validatedBoundary);

                int column = (int)constrainedPosition.X >> powerOfTwo;
                int row = (int)constrainedPosition.Y >> powerOfTwo;

                if (row < 0 || row >= rows || column < 0 || column >= columns)
                    return -1;

                return columns * row + column;
            }
        }
    }
}
