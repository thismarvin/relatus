using Relatus.Core;
using Relatus.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Relatus.ECS
{
    /// <summary>
    /// Handles all functionality related to maintaining and managing <see cref="MorroSystem"/>'s.
    /// </summary>
    class SystemManager
    {
        public int Capacity { get; private set; }
        public int TotalSystemsRegistered { get; private set; }
        public bool DisableAsynchronousUpdates { get; set; }
        public MorroSystem[] Systems { get; private set; }

        private readonly HashSet<Type> registeredSystems;
        private readonly Dictionary<Type, int> systemLookup;

        private IUpdateableSystem[][] updateableSystemGroups;
        private IDrawableSystem[] drawableSystems;

        public SystemManager(int capacity)
        {
            Capacity = capacity;

            Systems = new MorroSystem[Capacity];
            registeredSystems = new HashSet<Type>();
            systemLookup = new Dictionary<Type, int>();
        }

        /// <summary>
        /// Registers a given collection of <see cref="MorroSystem"/>'s to be managed.
        /// </summary>
        /// <param name="morroSystem">A collection of <see cref="MorroSystem"/>'s to be registered and managed.</param>
        public void RegisterSystem(params MorroSystem[] system)
        {
            if (TotalSystemsRegistered > Capacity)
                throw new RelatusException("Too many systems have been registered. Consider raising the capacity of systems allowed.", new IndexOutOfRangeException());

            Type systemType;
            for (int i = 0; i < system.Length; i++)
            {
                systemType = system[i].GetType();

                if (registeredSystems.Contains(systemType))
                    continue;

                registeredSystems.Add(systemType);
                systemLookup.Add(systemType, TotalSystemsRegistered);
                Systems[TotalSystemsRegistered] = system[i];
                TotalSystemsRegistered++;
            }

            CreateUpdateGroups();
            CreateDrawGroups();
        }

        /// <summary>
        /// Returns a <see cref="MorroSystem"/> of a given type.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="MorroSystem"/> to retrieve.</typeparam>
        /// <returns>A <see cref="MorroSystem"/> of a given type.</returns>
        public T GetSystem<T>() where T : MorroSystem
        {
            Type systemType = typeof(T);

            if (!registeredSystems.Contains(systemType))
                return default;

            return (T)Systems[systemLookup[systemType]];
        }

        public void Update()
        {
            if (DisableAsynchronousUpdates)
            {
                SynchronousUpdate();
            }
            else
            {
                AsynchronousUpdate();
            }
        }

        public void Draw(Camera camera)
        {
            for (int i = 0; i < drawableSystems.Length; i++)
            {
                if (drawableSystems[i].Enabled)
                {
                    drawableSystems[i].Draw(camera);
                }
            }
        }

        private void CreateUpdateGroups()
        {
            int totalCanidates;
            int groupIndex = -1;
            HashSet<MorroSystem> canidates = new HashSet<MorroSystem>();
            HashSet<Type> registered = new HashSet<Type>();
            List<Type> registeredBuffer = new List<Type>();
            List<MorroSystem> removalBuffer = new List<MorroSystem>();
            List<List<MorroSystem>> groups = new List<List<MorroSystem>>(TotalSystemsRegistered);

            SetupCanidates();
            ProcessCanidates();
            HandleDependencies();
            FinalizeResults();

            void SetupCanidates()
            {
                for (int i = 0; i < TotalSystemsRegistered; i++)
                {
                    if (Systems[i] is IUpdateableSystem)
                    {
                        canidates.Add(Systems[i]);
                    }
                }

                totalCanidates = canidates.Count;
            }

            void ProcessCanidates()
            {
                CreateNewGroup();

                foreach (MorroSystem system in canidates)
                {
                    if (system.Dependencies.Count == 0)
                    {
                        ProcessSystem(system);
                    }
                }
                ClearBuffers();
            }

            void HandleDependencies()
            {
                CreateNewGroup();

                bool done = false;
                int registeredDependencies;

                while (!done)
                {
                    done = true;

                    foreach (MorroSystem system in canidates)
                    {
                        registeredDependencies = 0;

                        foreach (Type dependency in system.Dependencies)
                        {
                            if (registered.Contains(dependency))
                            {
                                registeredDependencies++;
                            }
                        }

                        if (registeredDependencies == system.Dependencies.Count)
                        {
                            ProcessSystem(system);
                            done = false;
                        }
                    }

                    ClearBuffers();

                    if (!done && canidates.Count != 0)
                    {
                        CreateNewGroup();
                    }
                }
            }

            void FinalizeResults()
            {
                updateableSystemGroups = new IUpdateableSystem[groups.Count][];
                for (int i = 0; i < groups.Count; i++)
                {
                    updateableSystemGroups[i] = new IUpdateableSystem[groups[i].Count];
                    for (int j = 0; j < groups[i].Count; j++)
                    {
                        updateableSystemGroups[i][j] = (IUpdateableSystem)groups[i][j];
                    }
                }
            }

            void CreateNewGroup()
            {
                groups.Add(new List<MorroSystem>(totalCanidates));
                groupIndex++;
            }

            void ProcessSystem(MorroSystem system)
            {
                groups[groupIndex].Add(system);
                removalBuffer.Add(system);
                registeredBuffer.Add(system.GetType());
            }

            void ClearBuffers()
            {
                for (int i = 0; i < removalBuffer.Count; i++)
                {
                    canidates.Remove(removalBuffer[i]);
                }
                removalBuffer.Clear();

                for (int i = 0; i < registeredBuffer.Count; i++)
                {
                    registered.Add(registeredBuffer[i]);
                }
                registeredBuffer.Clear();
            }
        }

        private void CreateDrawGroups()
        {
            List<IDrawableSystem> systems = new List<IDrawableSystem>();

            foreach (MorroSystem morroSystem in Systems)
            {
                if (morroSystem is IDrawableSystem)
                {
                    systems.Add((IDrawableSystem)morroSystem);
                }
            }

            systems.Sort();

            drawableSystems = systems.ToArray();
        }

        private void SynchronousUpdate()
        {
            for (int i = 0; i < updateableSystemGroups.Length; i++)
            {
                for (int j = 0; j < updateableSystemGroups[i].Length; j++)
                {
                    if (updateableSystemGroups[i][j].Enabled)
                    {
                        updateableSystemGroups[i][j].Update();
                    }
                }
            }
        }

        private void AsynchronousUpdate()
        {
            for (int i = 0; i < updateableSystemGroups.Length; i++)
            {
                Task.WaitAll(DivideSystemsIntoTasks(i));
            }

            Task[] DivideSystemsIntoTasks(int groupIndex)
            {
                int taskIndex = 0;
                int totalTasks = TotalSystemsEnabled(groupIndex);
                Task[] tasks = new Task[totalTasks];

                for (int i = 0; i < totalTasks; i++)
                {
                    if (updateableSystemGroups[groupIndex][i].Enabled)
                    {
                        tasks[taskIndex++] = CreateTask(groupIndex, i);
                    }
                }

                return tasks;
            }

            Task CreateTask(int groupIndex, int systemIndex)
            {
                return Task.Run(() =>
                {
                    updateableSystemGroups[groupIndex][systemIndex].Update();
                });
            }

            int TotalSystemsEnabled(int groupIndex)
            {
                int result = 0;
                for (int i = 0; i < updateableSystemGroups[groupIndex].Length; i++)
                {
                    result = updateableSystemGroups[groupIndex][i].Enabled ? ++result : result;
                }
                return result;
            }
        }
    }
}
