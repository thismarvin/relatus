using System;
using System.Collections.Generic;
using Relatus.Utilities;

namespace Relatus.Industry
{
    public class Factory
    {
        public uint Capacity { get; private set; }

        public uint WorkerCount => workerIndex - (uint)vacancies.Count;

        private readonly Worker[] workers;
        private uint workerIndex;

        private readonly Queue<Tuple<uint, Trade[]>> behaviorAddition;
        private readonly Queue<Tuple<uint, Type[]>> behaviorSubtraction;
        private readonly Queue<uint> vacancies;
        private readonly SparseSet entityRemoval;
        private bool dataModified;

        public Factory(uint capacity)
        {
            Capacity = capacity;
            workers = new Worker[Capacity];

            behaviorAddition = new Queue<Tuple<uint, Trade[]>>((int)capacity);
            behaviorSubtraction = new Queue<Tuple<uint, Type[]>>((int)capacity);
            vacancies = new Queue<uint>((int)capacity);
            entityRemoval = new SparseSet(capacity);
        }

        // My implementation is somewhat jank, but having the ability to send a pseudo event once all
        // workers have been initialized seems valueable.
        public Factory Jumpstart()
        {
            for (int i = 0; i < workerIndex; i++)
            {
                List<IBehavior> behaviors = workers[i].Behaviors;
                for (int j = 0; j < behaviors.Count; j++)
                {
                    if (behaviors[j] is Trade trade)
                    {
                        trade.OnJumpstart();
                    }
                }
            }

            return this;
        }

        public Factory RecruitWorker(params Trade[] trades)
        {
            Worker worker = AllocateWorker();

            if (trades.Length == 0)
                return this;

            behaviorAddition.Enqueue(new Tuple<uint, Trade[]>(worker.SSN, trades));
            dataModified = true;

            return this;
        }

        public Worker AdoptWorker(params Trade[] trades)
        {
            Worker worker = AllocateWorker();

            if (trades.Length == 0)
                return worker;

            behaviorAddition.Enqueue(new Tuple<uint, Trade[]>(worker.SSN, trades));
            dataModified = true;

            return worker;
        }

        public Factory FireWorker(uint ssn)
        {
            VerifySSN(ssn);

            entityRemoval.Add(ssn);
            dataModified = true;

            return this;
        }

        public Factory TrainWorker(uint ssn, params Trade[] behaviors)
        {
            VerifySSN(ssn);

            if (behaviors.Length == 0)
                return this;

            behaviorAddition.Enqueue(new Tuple<uint, Trade[]>(ssn, behaviors));
            dataModified = true;

            return this;
        }

        public Factory GaslightWorker(uint ssn, params Type[] behaviorsTypes)
        {
            VerifySSN(ssn);

            if (behaviorsTypes.Length == 0)
                return this;

            behaviorSubtraction.Enqueue(new Tuple<uint, Type[]>(ssn, behaviorsTypes));
            dataModified = true;

            return this;
        }

        public Factory Clear()
        {
            // Clean up all workers.
            for (int i = 0; i < Capacity; i++)
            {
                workers[i]?.Dispose();
                workers[i]?.ClearBehaviors();
                // ? Not entirely sure if this is necessary, but I am going to keep it.
                workers[i] = null;
            }

            // Make sure to dispose of any behaviors that were about to be added.
            foreach (Tuple<uint, Trade[]> entry in behaviorAddition)
            {
                for (int i = 0; i < entry.Item2.Length; i++)
                {
                    if (entry.Item2[i] is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
            }

            behaviorAddition.Clear();
            behaviorSubtraction.Clear();
            entityRemoval.Clear();
            vacancies.Clear();

            workerIndex = 0;
            dataModified = false;

            return this;
        }

        public Factory ApplyChanges()
        {
            if (!dataModified)
                return this;

            while (behaviorSubtraction.Count > 0)
            {
                Tuple<uint, Type[]> modification = behaviorSubtraction.Dequeue();
                workers[modification.Item1].RemoveBehaviors(modification.Item2);
            }

            while (behaviorAddition.Count > 0)
            {
                Tuple<uint, Trade[]> modification = behaviorAddition.Dequeue();

                for (int i = 0; i < modification.Item2.Length; i++)
                {
                    modification.Item2[i].Attach(workers[modification.Item1], this);
                }

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
                    vacancies.Enqueue(entity);
                }
                entityRemoval.Clear();
            }

            dataModified = false;

            return this;
        }

        public Worker GetWorker(uint ssn)
        {
            VerifySSN(ssn);

            return workers[ssn];
        }

        public bool TryGetWorker(uint ssn, out Worker worker)
        {
            worker = null;

            if (ssn >= workerIndex)
                return false;

            worker = workers[ssn];

            if (worker == null)
                return false;

            return true;
        }

        public List<Worker> RequestWorkersWithBehavior<T>() where T : IBehavior
        {
            List<Worker> result = new List<Worker>((int)Capacity);

            for (int i = 0; i < workerIndex; i++)
            {
                if (workers[i]?.ContainsBehavior<T>() ?? false)
                {
                    result.Add(workers[i]);
                }
            }

            return result;
        }

        public List<Worker> RequestWorkersWithBehaviors(params Type[] behaviorsTypes)
        {
            List<Worker> result = new List<Worker>((int)Capacity);

            if (behaviorsTypes.Length == 0)
                return result;

            for (int i = 0; i < workerIndex; i++)
            {
                if (workers[i]?.ContainsBehaviors(behaviorsTypes) ?? false)
                {
                    result.Add(workers[i]);
                }
            }

            return result;
        }

        public List<T> RequestBehavior<T>() where T : IBehavior
        {
            List<T> result = new List<T>((int)Capacity);

            for (int i = 0; i < workerIndex; i++)
            {
                T behavior = default;
                if (workers[i]?.TryGetBehavior<T>(out behavior) ?? false)
                {
                    result.Add(behavior);
                }
            }

            return result;
        }

        public Factory RunUpdateableBehaviors()
        {
            if (dataModified)
                throw new RelatusException("The Factory's data was modified, but ApplyChanges() was never called.", new MethodExpectedException());

            for (int i = 0; i < workerIndex; i++)
            {
                workers[i]?.Update();
            }

            return this;
        }

        public Factory RunDrawableBehaviors(Camera camera)
        {
            if (dataModified)
                throw new RelatusException("The Factory's data was modified, but ApplyChanges() was never called.", new MethodExpectedException());

            for (int i = 0; i < workerIndex; i++)
            {
                workers[i]?.Draw(camera);
            }

            return this;
        }

        private Worker AllocateWorker()
        {
            // Prioritize creating an entity in an empty slot.
            if (vacancies.Count > 0)
            {
                uint index = vacancies.Dequeue();

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

        private void VerifySSN(uint ssn)
        {
            if (ssn >= workerIndex)
                throw new RelatusException("A worker with that SSN does not exist.", new ArgumentOutOfRangeException());
        }
    }
}
