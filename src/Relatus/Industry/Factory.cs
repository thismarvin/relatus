using System;
using System.Collections.Generic;
using Relatus.Utilities;

namespace Relatus.Industry
{
    public class Factory
    {
        public uint Capacity { get; private set; }

        private readonly Worker[] workers;
        private uint workerIndex;

        private readonly Stack<Tuple<uint, Trade[]>> behaviorAddition;
        private readonly Stack<Tuple<uint, Type[]>> behaviorSubtraction;
        private readonly Queue<uint> vacancies;
        private readonly SparseSet entityRemoval;
        private bool dataModified;

        public Factory(uint capacity)
        {
            Capacity = capacity;
            workers = new Worker[Capacity];

            behaviorAddition = new Stack<Tuple<uint, Trade[]>>((int)capacity);
            behaviorSubtraction = new Stack<Tuple<uint, Type[]>>((int)capacity);
            vacancies = new Queue<uint>((int)capacity);
            entityRemoval = new SparseSet((int)capacity);
        }

        public Worker RecruitWorker()
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

        public Factory RecruitWorker(params Trade[] behaviors)
        {
            Worker worker = RecruitWorker();

            for (int i = 0; i < behaviors.Length; i++)
            {
                behaviors[i].Attach(worker, this);
            }

            worker.AddBehavior(behaviors);

            return this;
        }

        public Factory FireWorker(uint ssn)
        {
            VerifySSN(ssn);

            vacancies.Enqueue(ssn);
            dataModified = true;

            return this;
        }

        public Factory TrainWorker(uint ssn, params Trade[] behaviors)
        {
            VerifySSN(ssn);

            behaviorAddition.Push(new Tuple<uint, Trade[]>(ssn, behaviors));
            dataModified = true;

            return this;
        }

        public Factory GaslightWorker(uint ssn, params Type[] behaviorsTypes)
        {
            VerifySSN(ssn);

            behaviorSubtraction.Push(new Tuple<uint, Type[]>(ssn, behaviorsTypes));
            dataModified = true;

            return this;
        }

        public Factory ApplyChanges()
        {
            if (!dataModified)
                return this;

            while (behaviorSubtraction.Count > 0)
            {
                Tuple<uint, Type[]> modification = behaviorSubtraction.Pop();
                workers[modification.Item1].RemoveBehaviors(modification.Item2);
            }

            while (behaviorAddition.Count > 0)
            {
                Tuple<uint, Trade[]> modification = behaviorAddition.Pop();

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
                T behavior = default(T);
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

        private void VerifySSN(uint ssn)
        {
            if (ssn >= workerIndex)
                throw new RelatusException("A worker with that SSN does not exist.", new ArgumentOutOfRangeException());
        }
    }
}
