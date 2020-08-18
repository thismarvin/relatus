using Relatus.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.ECS
{
    public partial class MorroFactory
    {
        public int EntityCount => (int)totalEntitiesCreated - vacancies.Count;

        private readonly SparseSet[] attachedComponents;
        private readonly SparseSet[] attachedSystems;
        private readonly Queue<uint> vacancies;
        private uint nextEntity;
        private uint totalEntitiesCreated;

        /// <summary>
        /// Clears an entity of all attached components and systems.
        /// </summary>
        /// <param name="entity">The target entity to clear.</param>
        public void ClearEntity(uint entity)
        {
            if (attachedComponents[entity].Count == 0 && attachedSystems[entity].Count == 0)
                return;

            foreach (uint i in attachedComponents[entity])
            {
                componentData[i][entity] = null;
            }

            foreach (uint i in attachedSystems[entity])
            {
                systems[i].RemoveEntity(entity);
            }

            attachedComponents[entity].Clear();
            attachedSystems[entity].Clear();

            vacancies.Enqueue(entity);
        }

        /// <summary>
        /// Returns whether or not a given entity contains a given collection of <see cref="IComponent"/> types.
        /// </summary>
        /// <param name="entity">The entity that will be checked.</param>
        /// <param name="components">The set of <see cref="IComponent"/> types that will be checked for.</param>
        /// <returns>Whether or not a given entity contains a given collection of <see cref="IComponent"/> types.</returns>
        public bool EntityContains(uint entity, HashSet<Type> components)
        {
            if (components.Count == 0)
                return false;

            foreach (Type component in components)
            {
                if (TryGetComponentID(component, out uint id))
                {
                    if (!attachedComponents[entity].Contains(id))
                        return false;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns whether or not a given entity contains a given collection of <see cref="IComponent"/> types.
        /// </summary>
        /// <param name="entity">The entity that will be checked.</param>
        /// <param name="components">The collection of <see cref="IComponent"/> types that will be checked for.</param>
        /// <returns>Whether or not a given entity contains a given collection of <see cref="IComponent"/> types.</returns>
        public bool EntityContains(uint entity, params Type[] components)
        {
            if (components.Length == 0)
                return false;

            for (int i = 0; i < components.Length; i++)
            {
                if (TryGetComponentID(components[i].GetType(), out uint id))
                {
                    if (!attachedComponents[entity].Contains(id))
                        return false;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns the next available entity.
        /// </summary>
        /// <returns>The next available entity.</returns>
        private uint AllocateEntity()
        {
            uint entity = nextEntity;

            if (vacancies.Count > 0)
            {
                entity = vacancies.Dequeue();
            }
            else
            {
                nextEntity = ++nextEntity >= EntityCapacity ? EntityCapacity - 1 : nextEntity;
                totalEntitiesCreated = ++totalEntitiesCreated > EntityCapacity ? EntityCapacity : totalEntitiesCreated;
            }

            attachedComponents[entity].Clear();
            attachedSystems[entity].Clear();

            return entity;
        }

        private void AddComponent(uint entity, params IComponent[] components)
        {
            RegisterComponent(components);

            AssignComponents(entity, components);
            AssignSystems(entity);
        }

        private void RemoveComponent(uint entity, params Type[] componentTypes)
        {
            for (int i = 0; i < componentTypes.Length; i++)
            {
                if (TryGetComponentID(componentTypes[i], out uint id))
                {
                    componentData[id][entity] = null;
                    attachedComponents[entity].Remove(id);

                    foreach (uint j in attachedSystems[entity])
                    {
                        if (systems[j].RequiredComponents.Contains(componentTypes[i]))
                        {
                            systems[j].RemoveEntity(entity);
                        }
                    }
                }
            }
        }

        private void AssignComponents(uint entity, params IComponent[] components)
        {
            for (int i = 0; i < components.Length; i++)
            {
                if (TryGetComponentID(components[i].GetType(), out uint id))
                {
                    componentData[id][entity] = components[i];
                    attachedComponents[entity].Add(id);
                }
            }
        }

        private void AssignSystems(uint entity)
        {
            for (int i = 0; i < systemIndex; i++)
            {
                systems[i].RemoveEntity(entity);

                if (EntityContains(entity, systems[i].RequiredComponents) && !EntityContains(entity, systems[i].BlacklistedComponents))
                {
                    systems[i].AddEntity(entity);
                    attachedSystems[entity].Add((uint)i);
                }
            }
        }
    }
}
