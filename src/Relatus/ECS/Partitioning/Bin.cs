using Microsoft.Xna.Framework;
using Relatus.Core;
using Relatus.Maths;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.ECS
{
    /// <summary>
    /// An implementation of a <see cref="Partitioner{T}"/> that uses a hashing algorithm to store and retrieve <see cref="IPartitionable"/> entries.
    /// </summary>
    /// <typeparam name="T">The element stored within the partitioner.</typeparam>
    class Bin<T> : Partitioner<T> where T : IPartitionable
    {
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
            columns = (int)Math.Ceiling(Boundary.Width / Math.Pow(2, powerOfTwo));
            rows = (int)Math.Ceiling(Boundary.Height / Math.Pow(2, powerOfTwo));

            buckets = new HashSet<T>[rows * columns];

            for (int i = 0; i < buckets.Length; i++)
            {
                buckets[i] = new HashSet<T>();
            }
        }

        public override List<int> Query(RectangleF bounds)
        {
            List<int> result = new List<int>();
            HashSet<int> unique = new HashSet<int>();
            HashSet<int> ids = GetHashIDs(bounds);

            foreach (int id in ids)
            {
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

            foreach (int i in ids)
            {
                buckets[i].Add(entry);
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
            HashSet<int> result = new HashSet<int>();

            // Make sure that the query's bounds are within the partitioner's bounds.
            RectangleF validatedBounds = ValidateBounds(bounds);

            // Hash all corners of the validated query's bounds.
            HashPosition(validatedBounds.Left, validatedBounds.Top);
            HashPosition(validatedBounds.Right, validatedBounds.Top);
            HashPosition(validatedBounds.Right, validatedBounds.Bottom);
            HashPosition(validatedBounds.Left, validatedBounds.Bottom);

            /// Ideally the dimensions of the validated query's bounds will be less than the partitioner's bucket size.
            /// However, this is not always the case. In order to handle all dimensions, we have to carefully divide the query bounds into smaller
            /// subsections. Each subsection needs to be the same size as the partitioner's bucket size for optimal guaranteed coverage.
            if (validatedBounds.Width > bucketSize || validatedBounds.Height > bucketSize)
            {
                int totalRows = (int)Math.Ceiling(validatedBounds.Height / bucketSize);
                int totalColumns = (int)Math.Ceiling(validatedBounds.Width / bucketSize);

                for (int y = 0; y <= totalRows; y++)
                {
                    for (int x = 0; x <= totalColumns; x++)
                    {
                        HashPosition(bounds.X + x * bucketSize, bounds.Y + y * bucketSize);
                    }
                }
            }

            return result;

            Point ValidatePosition(float _x, float _y)
            {
                int xValidated = (int)_x;
                int yValidated = (int)_y;

                xValidated = Math.Max(0, xValidated);
                xValidated = Math.Min((int)Boundary.Right, xValidated);

                yValidated = Math.Max(0, yValidated);
                yValidated = Math.Min((int)Boundary.Bottom, yValidated);

                return new Point(xValidated, yValidated);
            }

            RectangleF ValidateBounds(RectangleF _bounds)
            {
                Point validatedPosition = ValidatePosition((int)_bounds.X, (int)_bounds.Y);

                int xValidated = validatedPosition.X;
                int yValidated = validatedPosition.Y;

                int widthValidated = (int)Math.Ceiling(_bounds.Width);
                int heightValidated = (int)Math.Ceiling(_bounds.Height);

                widthValidated = Math.Min((int)Math.Ceiling(Boundary.Right) - xValidated, widthValidated);
                heightValidated = Math.Min((int)Math.Ceiling(Boundary.Bottom) - yValidated, heightValidated);

                return new RectangleF(xValidated, yValidated, widthValidated, heightValidated);
            }

            void HashPosition(float _x, float _y)
            {
                Point validatedPosition = ValidatePosition(_x, _y);

                int xValidated = validatedPosition.X;
                int yValidated = validatedPosition.Y;

                int row = xValidated >> powerOfTwo;
                int column = yValidated >> powerOfTwo;

                if (column < 0 || column >= columns || row < 0 || row >= rows)
                    return;

                result.Add(columns * column + row);
            }
        }
    }
}
