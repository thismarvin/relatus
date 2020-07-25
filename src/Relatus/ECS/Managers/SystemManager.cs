using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Relatus.ECS
{
    /// <summary>
    /// Handles all functionality related to maintaining and managing <see cref="MorroSystem"/>'s.
    /// </summary>
    internal class SystemManager
    {
        public int Capacity { get; private set; }
        public int TotalSystemsRegistered { get; private set; }
        public MorroSystem[] Systems { get; private set; }

        private readonly HashSet<Type> registeredSystems;
        private readonly Dictionary<Type, int> systemLookup;

        public SystemManager(int capacity)
        {
            Capacity = capacity;

            Systems = new MorroSystem[Capacity];
            registeredSystems = new HashSet<Type>();
            systemLookup = new Dictionary<Type, int>();
        }

        public void RegisterSystem(params MorroSystem[] systems)
        {
            if (TotalSystemsRegistered > Capacity)
                throw new RelatusException("Too many systems have been registered. Consider raising the capacity of systems allowed.", new IndexOutOfRangeException());

            for (int i = 0; i < systems.Length; i++)
            {
                Type systemType = systems[i].GetType();

                // This should really throw an error.
                if (registeredSystems.Contains(systemType))
                    continue;

                registeredSystems.Add(systemType);
                systemLookup.Add(systemType, TotalSystemsRegistered);
                Systems[TotalSystemsRegistered] = systems[i];
                TotalSystemsRegistered++;
            }
        }

        public void ApplyChanges()
        {
            for (int i = 0; i < TotalSystemsRegistered; i++)
            {
                Systems[i].ApplyChanges();
            }
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
    }
}
