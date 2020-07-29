using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Relatus.ECS
{
    internal class UpdateSystemHandler
    {
        public bool AsynchronousUpdateEnabled { get; set; }
        public bool FixedUpdateEnabled { get; set; }

        public uint TotalTasks
        {
            get => totalTasks;
            set
            {
                if (value == 0)
                    throw new RelatusException("The total amount of tasks cannot be zero.", new ArgumentException());

                totalTasks = value;
            }
        }
        public uint UpdatesPerSecond
        {
            set
            {
                if (value == 0)
                    throw new RelatusException("The total amount of updates per second cannot be zero.", new ArgumentException());

                threshold = 1f / value;
            }
        }

        private readonly MorroSystem parent;
        private readonly Action<int> onUpdate;

        private uint totalTasks;
        private float threshold;
        private float accumulator;

        public UpdateSystemHandler(MorroSystem parent, Action<int> onUpdate)
        {
            this.parent = parent;
            this.onUpdate = onUpdate;

            totalTasks = 1;
            threshold = 1f / 60;
        }

        public void Update()
        {
            if (FixedUpdateEnabled)
            {
                if (AsynchronousUpdateEnabled && parent.Entities.Count >= TotalTasks)
                {
                    ExecuteAsFixedUpdate(ParallelUpdate);
                }
                else
                {
                    ExecuteAsFixedUpdate(NormalUpdate);
                }
            }
            else
            {
                if (AsynchronousUpdateEnabled && parent.Entities.Count >= TotalTasks)
                {
                    ParallelUpdate();
                }
                else
                {
                    NormalUpdate();
                }
            }

            void ExecuteAsFixedUpdate(Action action)
            {
                accumulator += Engine.DeltaTime;
                while (accumulator >= threshold)
                {
                    action();
                    accumulator -= threshold;
                }
            }
        }

        private void NormalUpdate()
        {
            int[] entities = parent.EntitiesAsArray;
            for (int i = 0; i < parent.Entities.Count; i++)
            {
                onUpdate(entities[i]);
            }
        }

        private void ParallelUpdate()
        {
            int[] entities = parent.EntitiesAsArray;
            Task.WaitAll(DivideUpdateIntoTasks(TotalTasks));

            Task[] DivideUpdateIntoTasks(uint totalTasks)
            {
                Task[] result = new Task[totalTasks];

                int increment = parent.Entities.Count / (int)totalTasks;
                int start = 0;

                for (int i = 0; i < totalTasks; i++)
                {
                    if (i != totalTasks - 1)
                    {
                        result[i] = UpdateSection(start, start + increment);
                    }
                    else
                    {
                        result[i] = UpdateSection(start, parent.Entities.Count);
                    }

                    start += increment;
                }

                return result;
            }

            Task UpdateSection(int startingIndex, int endingIndex)
            {
                return Task.Run(() =>
                {
                    for (int i = startingIndex; i < endingIndex; i++)
                    {
                        onUpdate(entities[i]);
                    }
                });
            }
        }
    }
}
