using System;
using System.Collections.Generic;
using Relatus.Utilities;

namespace Relatus.Industry
{
    public class Factory
    {
        public int Capacity { get; private set; }

        private readonly Worker[] workers;
        private int workerIndex;

        private readonly Stack<Tuple<int, Behavior[]>> behaviorAddition;
        private readonly Stack<Tuple<int, Type[]>> behaviorSubtraction;
        private readonly Queue<int> vacancies;
        private readonly SparseSet entityRemoval;
        private bool dataModified;

        public Factory(int capacity)
        {
            Capacity = capacity;
            workers = new Worker[Capacity];

            behaviorAddition = new Stack<Tuple<int, Behavior[]>>(capacity);
            behaviorSubtraction = new Stack<Tuple<int, Type[]>>(capacity);
            vacancies = new Queue<int>(capacity);
            entityRemoval = new SparseSet(capacity);
        }

        public Entity RecruitWorker()
        {
            // Prioritize creating an entity in an empty slot.
            if (vacancies.Count > 0)
            {
                int index = vacancies.Dequeue();

                Worker replacement = new Worker(index);
                workers[index] = replacement;

                return replacement;
            }

            // Overwrite the last entity if the factory is at capacity.
            if (workerIndex >= Capacity)
            {
                workerIndex = Capacity - 1;
                workers[workerIndex].Dispose();
            }

            Worker worker = new Worker(workerIndex);
            workers[workerIndex] = worker;

            workerIndex++;

            return worker;
        }

        public Factory FireWorker(int ssn)
        {
            vacancies.Enqueue(ssn);
            dataModified = true;

            return this;
        }

        public Factory TrainWorker(int ssn, params Behavior[] behaviors)
        {
            behaviorAddition.Push(new Tuple<int, Behavior[]>(ssn, behaviors));

            return this;
        }

        public Factory GaslightWorker(int ssn, params Type[] behaviorsTypes)
        {
            behaviorSubtraction.Push(new Tuple<int, Type[]>(ssn, behaviorsTypes));

            return this;
        }

        public Factory ApplyChanges()
        {
            if (!dataModified)
                return this;

            while (behaviorSubtraction.Count > 0)
            {
                Tuple<int, Type[]> modification = behaviorSubtraction.Pop();
                workers[modification.Item1].RemoveBehaviors(modification.Item2);
            }

            while (behaviorAddition.Count > 0)
            {
                Tuple<int, Behavior[]> modification = behaviorAddition.Pop();
                workers[modification.Item1].AddBehavior(modification.Item2);
            }

            // Handles removing entities.
            if (entityRemoval.Count != 0)
            {
                foreach (uint entity in entityRemoval)
                {
                    int temp = (int)entity;
                    workers[temp].Dispose();
                    workers[temp] = null;
                    vacancies.Enqueue(temp);
                }
                entityRemoval.Clear();
            }

            return this;
        }

        public List<Worker> RequestWorkersWith<T>() where T : Behavior
        {
            List<Worker> result = new List<Worker>();
            Type type = typeof(T);

            for (int i = 0; i < workerIndex; i++)
            {
                if (workers[i]?.Contains(type) ?? false)
                {
                    result.Add(workers[i]);
                }
            }

            return result;
        }

        public List<Worker> RequestWorkersWithBehaviors(params Type[] behaviorsTypes)
        {
            List<Worker> result = new List<Worker>();

            for (int i = 0; i < workerIndex; i++)
            {
                if (workers[i]?.Contains(behaviorsTypes) ?? false)
                {
                    result.Add(workers[i]);
                }
            }

            return result;
        }

        public void Update()
        {
            for (int i = 0; i < workerIndex; i++)
            {
                workers[i]?.Update();
            }
        }

        public void Draw(Camera camera)
        {
            for (int i = 0; i < workerIndex; i++)
            {
                workers[i]?.Draw(camera);
            }
        }
    }
}
