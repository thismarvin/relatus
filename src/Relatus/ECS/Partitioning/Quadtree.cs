using Relatus.Maths;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.ECS
{
    /// <summary>
    /// An implementation of a <see cref="Partitioner{T}"/> that uses a recursive tree structure to store and retrieve <see cref="IPartitionable"/> entries.
    /// </summary>
    /// <typeparam name="T">The element stored within the partitioner.</typeparam>
    public class Quadtree<T> : Partitioner<T> where T : IPartitionable
    {
        private readonly int capacity;
        private bool divided;
        private int insertionIndex;
        private T[] objects;
        private Quadtree<T> topLeft;
        private Quadtree<T> topRight;
        private Quadtree<T> bottomRight;
        private Quadtree<T> bottomLeft;

        /// <summary>
        /// Creates an implementation of a <see cref="Partitioner{T}"/> that uses a recursive tree structure to store and retrieve <see cref="IPartitionable"/> entries.
        /// </summary>
        /// <param name="boundary">The area that the partitioner will cover.</param>
        /// <param name="capacity">The total amount of entries that exist in a node before overflowing into a new tree.</param>
        public Quadtree(RectangleF boundary, int capacity) : base(boundary)
        {
            this.capacity = capacity;

            Initialize();
        }

        protected override void Initialize()
        {
            objects = new T[capacity];
        }

        public override List<int> Query(RectangleF bounds)
        {
            List<int> result = new List<int>();

            if (Boundary.CompletelyWithin(bounds))
            {
                for (int i = 0; i < objects.Length; i++)
                {
                    if (objects[i] == null)
                        continue;

                    result.Add(objects[i].Identifier);
                }
            }
            else
            {
                for (int i = 0; i < objects.Length; i++)
                {
                    if (objects[i] == null)
                        continue;

                    if (bounds.Intersects(objects[i].Bounds))
                    {
                        result.Add(objects[i].Identifier);
                    }
                }
            }

            if (!divided)
                return result;

            result.AddRange(topLeft.Query(bounds));
            result.AddRange(topRight.Query(bounds));
            result.AddRange(bottomRight.Query(bounds));
            result.AddRange(bottomLeft.Query(bounds));

            return result;
        }

        public override bool Insert(T entry)
        {
            if (!entry.Bounds.Intersects(Boundary))
                return false;

            if (insertionIndex < capacity)
            {
                objects[insertionIndex++] = entry;
                return true;
            }
            else
            {
                if (!divided)
                    Subdivide();

                /// Note that short-circuiting is intended here! We basically keep trying to add an entry to a tree and if we succeed then we should abort. Doing so prevents adding duplicate entries!
                if (topLeft.Insert(entry) || topRight.Insert(entry) || bottomRight.Insert(entry) || bottomLeft.Insert(entry))
                    return true;
            }

            return false;
        }

        public override void Clear()
        {
            if (objects == null)
                return;

            if (divided)
            {
                topLeft.Clear();
                topRight.Clear();
                bottomRight.Clear();
                bottomLeft.Clear();

                topLeft = null;
                topRight = null;
                bottomRight = null;
                bottomLeft = null;
            }

            divided = false;
            insertionIndex = 0;

            Array.Clear(objects, 0, objects.Length);
        }

        private void Subdivide()
        {
            float width = Boundary.Width * 0.5f;
            float height = Boundary.Height * 0.5f;

            topLeft = new Quadtree<T>(new RectangleF(Boundary.X, Boundary.Y, width, height), capacity);
            topRight = new Quadtree<T>(new RectangleF(Boundary.X + width, Boundary.Y, width, height), capacity);
            bottomRight = new Quadtree<T>(new RectangleF(Boundary.X + width, Boundary.Y + height, width, height), capacity);
            bottomLeft = new Quadtree<T>(new RectangleF(Boundary.X, Boundary.Y + height, width, height), capacity);

            divided = true;
        }
    }
}
