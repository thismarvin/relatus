using Relatus.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Graphics
{
    abstract class DrawCollection<T>
    {
        private readonly int maximumGroupCapacity;
        protected DrawGroup<T>[] groups;

        public DrawCollection(int maximumGroupCapacity)
        {
            this.maximumGroupCapacity = maximumGroupCapacity;
            groups = new DrawGroup<T>[0];
        }

        protected abstract DrawGroup<T> CreateDrawGroup(T currentEntry, int capacity);

        public DrawCollection<T> SetCollection(T[] entries)
        {
            int totalGroups = (int)Math.Ceiling((float)entries.Length / maximumGroupCapacity);
            groups = new DrawGroup<T>[totalGroups];
            int groupIndex = -1;
            int remaining = entries.Length;

            int capacity;
            for (int i = 0; i < entries.Length; i++)
            {
                if (groupIndex == -1 || !groups[groupIndex].Add(entries[i]))
                {
                    capacity = remaining / maximumGroupCapacity > 0 ? maximumGroupCapacity : remaining % maximumGroupCapacity;
                    remaining -= maximumGroupCapacity;

                    groupIndex++;
                    groups[groupIndex] = CreateDrawGroup(entries[i], capacity);
                    groups[groupIndex].Add(entries[i]);
                }
            }

            return this;
        }

        public DrawCollection<T> Clear()
        {
            for (int i = 0; i < groups.Length; i++)
            {
                groups[i].Clear();
            }

            return this;
        }

        public DrawCollection<T> Draw(Camera camera)
        {
            for (int i = 0; i < groups.Length; i++)
            {
                groups[i].Draw(camera);
            }

            return this;
        }
    }
}
