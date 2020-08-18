using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.ECS
{
    public partial class MorroFactory
    {
        private readonly Dictionary<Type, int> systemLookup;
        private readonly MorroSystem[] systems;
        private int systemIndex;

        /// <summary>
        /// Returns a <see cref="MorroSystem"/> of a given type.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="MorroSystem"/> to retrieve.</typeparam>
        /// <returns>A <see cref="MorroSystem"/> of a given type.</returns>
        public T GetSystem<T>() where T : MorroSystem
        {
            Type systemType = typeof(T);

            if (!systemLookup.ContainsKey(systemType))
                return default;

            return (T)systems[systemLookup[systemType]];
        }

        private void RegisterSystem(params MorroSystem[] systems)
        {
            if (systemIndex > SystemCapacity)
                throw new RelatusException("Too many systems have been registered. Consider raising the capacity of systems allowed.", new IndexOutOfRangeException());

            for (int i = 0; i < systems.Length; i++)
            {
                Type systemType = systems[i].GetType();

                if (systemLookup.ContainsKey(systemType))
                    throw new RelatusException("A factory cannot have multiple systems of the same type.", new ArgumentException());

                systemLookup.Add(systemType, systemIndex);
                this.systems[systemIndex] = systems[i];
                systemIndex++;
            }
        }

        private void UpdateSystems()
        {
            for (int i = 0; i < systemIndex; i++)
            {
                systems[i].ApplyChanges();
            }
        }
    }
}
