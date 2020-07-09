using Relatus.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Relatus.ECS
{
    class UpdateSystemHandler
    {
        public uint TotalTasks { get; set; }
        public bool FixedUpdateEnabled { get; set; }
        public bool AsynchronousUpdateEnabled { get => TotalTasks > 1; }

        public int TargetFPS
        {
            get => targetFPS;
            set
            {
                if (value <= 0)
                    throw new RelatusException("The target framerate cannot be less than 1.", new ArgumentException());

                targetFPS = value;
                FixedUpdateEnabled = TargetFPS > 0;
                threshold = 1f / TargetFPS;
            }
        }

        private readonly MorroSystem parent;
        private readonly Action<int> onUpdate;

        private int targetFPS;
        private float threshold;
        private float accumulator;

        public UpdateSystemHandler(MorroSystem parent, Action<int> onUpdate)
        {
            this.parent = parent;
            this.onUpdate = onUpdate;
        }

        public UpdateSystemHandler(MorroSystem parent, Action<int> onUpdate, uint totalTasks, int targetFPS)
        {
            this.parent = parent;
            this.onUpdate = onUpdate;
            TotalTasks = totalTasks;
            TargetFPS = targetFPS;
            accumulator = threshold;
        }

        public void Update()
        {
            if (FixedUpdateEnabled)
            {
                if (AsynchronousUpdateEnabled)
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
                if (AsynchronousUpdateEnabled)
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
            foreach (int entity in parent.Entities)
            {
                onUpdate(entity);
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
                int end = increment;

                for (int i = 0; i < totalTasks; i++)
                {
                    if (i != totalTasks - 1)
                    {
                        result[i] = UpdateSection(start, end);
                    }
                    else
                    {
                        result[i] = UpdateSection(start, parent.Entities.Count);
                    }

                    start += increment;
                    end += increment;
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
