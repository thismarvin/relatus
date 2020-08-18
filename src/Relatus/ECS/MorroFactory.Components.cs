using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.ECS
{
    public partial class MorroFactory
    {
        private readonly Dictionary<Type, uint> componentLookup;
        private readonly IComponent[][] componentData;
        private uint componentIndex;

        /// <summary>
        /// Returns an array of all of the data of a given <see cref="IComponent"/> type.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IComponent"/> data to be retrieved.</typeparam>
        /// <returns>An array of all of the data of a given <see cref="IComponent"/> type.</returns>
        public IComponent[] GetData<T>() where T : IComponent
        {
            Type componentType = typeof(T);
            if (!componentLookup.ContainsKey(componentType))
                return new IComponent[0];

            return componentData[componentLookup[componentType]];
        }

        /// <summary>
        /// Returns the <see cref="IComponent"/> data of a given entity.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IComponent"/> data to be retrieved.</typeparam>
        /// <param name="entity">The target entity to retrieve data from.</param>
        /// <returns>The <see cref="IComponent"/> data of a given entity.</returns>
        public T GetData<T>(uint entity) where T : IComponent
        {
            Type componentType = typeof(T);
            if (!componentLookup.ContainsKey(componentType))
                return default;

            return (T)componentData[componentLookup[componentType]][entity];
        }

        private void RegisterComponent(params IComponent[] components)
        {
            for (int i = 0; i < components.Length; i++)
            {
                Type componentType = components[i].GetType();

                if (componentLookup.ContainsKey(componentType))
                    continue;

                componentLookup.Add(componentType, componentIndex);
                componentIndex++;
            }
        }

        private bool TryGetComponentID(Type componentType, out uint id)
        {
            id = uint.MaxValue;

            if (!componentLookup.ContainsKey(componentType))
                return false;

            id = componentLookup[componentType];
            return true;
        }
    }
}
