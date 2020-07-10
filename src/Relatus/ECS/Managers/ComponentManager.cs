using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.ECS
{
    /// <summary>
    /// Handles all functionality related to managing <see cref="IComponent"/> data.
    /// </summary>
    public class ComponentManager
    {
        public int Capacity { get; private set; }
        public int TotalComponentsRegistered { get; private set; }
        public IComponent[][] Data { get; private set; }

        private readonly HashSet<Type> registeredComponents;
        private readonly Dictionary<Type, int> componentLookup;

        public ComponentManager(int capacity, int entityCapacity)
        {
            Capacity = capacity;

            registeredComponents = new HashSet<Type>();
            componentLookup = new Dictionary<Type, int>();

            Data = new IComponent[capacity][];
            for (int i = 0; i < capacity; i++)
            {
                Data[i] = new IComponent[entityCapacity];
            }
        }

        public void RegisterComponent(params IComponent[] components)
        {
            Type componentType;
            for (int i = 0; i < components.Length; i++)
            {
                componentType = components[i].GetType();

                if (registeredComponents.Contains(componentType))
                    continue;

                registeredComponents.Add(componentType);
                componentLookup.Add(componentType, TotalComponentsRegistered);
                TotalComponentsRegistered++;
            }
        }

        public int GetComponentID(IComponent component)
        {
            return GetComponentID(component.GetType());
        }

        public int GetComponentID(Type componentType)
        {
            if (!componentLookup.ContainsKey(componentType))
                return -1;

            return componentLookup[componentType];
        }

        /// <summary>
        /// Returns an array of all of the data of a given <see cref="IComponent"/> type.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IComponent"/> data to be retrieved.</typeparam>
        /// <returns>An array of all of the data of a given <see cref="IComponent"/> type.</returns>
        public IComponent[] GetData<T>() where T : IComponent
        {
            Type componentType = typeof(T);
            if (!registeredComponents.Contains(componentType))
                return new IComponent[0];

            return Data[componentLookup[componentType]];
        }

        /// <summary>
        /// Returns the <see cref="IComponent"/> data of a given entity.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IComponent"/> data to be retrieved.</typeparam>
        /// <param name="entity">The target entity to retrieve data from.</param>
        /// <returns>The <see cref="IComponent"/> data of a given entity.</returns>
        public T GetData<T>(int entity) where T : IComponent
        {
            Type componentType = typeof(T);
            if (!componentLookup.ContainsKey(componentType))
                return default;

            return (T)Data[componentLookup[componentType]][entity];
        }
    }
}
