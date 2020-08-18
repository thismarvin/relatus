using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Graphics
{
    public abstract class DrawCollection<T>
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
            int groupIndex = -1;
            int remaining = entries.Length;

            groups = new DrawGroup<T>[totalGroups];

            for (int i = 0; i < entries.Length; i++)
            {
                if (entries[i] == null)
                    continue;

                if (groupIndex == -1 || !groups[groupIndex].Add(entries[i]))
                {
                    int capacity = remaining / maximumGroupCapacity > 0 ? maximumGroupCapacity : remaining % maximumGroupCapacity;
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
