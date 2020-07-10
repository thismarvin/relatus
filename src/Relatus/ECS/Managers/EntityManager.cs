using Relatus.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.ECS
{
    /// <summary>
    /// Handles all functionality related to creating and modifying entities.
    /// </summary>
    public class EntityManager
    {
        public int Capacity { get; private set; }

        public int EntityCount { get => totalEntitiesCreated - entityBuffer.Count; }

        private readonly SparseSet[] attachedComponents;
        private readonly SparseSet[] attachedSystems;
        private readonly Stack<int> entityBuffer;
        private int nextEntity;
        private int totalEntitiesCreated;

        private readonly SystemManager systemManager;
        private readonly ComponentManager componentManager;

        public EntityManager(int capacity, SystemManager systemManager, ComponentManager componentManager)
        {
            Capacity = capacity;

            attachedComponents = new SparseSet[Capacity];
            attachedSystems = new SparseSet[Capacity];
            entityBuffer = new Stack<int>(Capacity);

            for (int i = 0; i < Capacity; i++)
            {
                attachedComponents[i] = new SparseSet(componentManager.Capacity);
                attachedSystems[i] = new SparseSet(systemManager.Capacity);
            }

            this.systemManager = systemManager;
            this.componentManager = componentManager;
        }

        /// <summary>
        /// Returns the next available entity.
        /// </summary>
        /// <returns>The next available entity.</returns>
        public int AllocateEntity()
        {
            int entity = nextEntity;

            if (entityBuffer.Count > 0)
            {
                entity = entityBuffer.Pop();
            }
            else
            {
                nextEntity = ++nextEntity >= Capacity ? Capacity - 1 : nextEntity;
                totalEntitiesCreated = ++totalEntitiesCreated > Capacity ? Capacity : totalEntitiesCreated;
            }

            attachedComponents[entity].Clear();
            attachedSystems[entity].Clear();

            return entity;
        }

        /// <summary>
        /// Clears an entity of all attached components and systems.
        /// </summary>
        /// <param name="entity">The target entity to clear.</param>
        public void ClearEntity(int entity)
        {
            if (attachedComponents[entity].Count == 0 && attachedSystems[entity].Count == 0)
                return;

            foreach (uint i in attachedComponents[entity])
            {
                componentManager.Data[i][entity] = null;
            }

            foreach (uint i in attachedSystems[entity])
            {
                systemManager.Systems[i].RemoveEntity(entity);
            }

            attachedComponents[entity].Clear();
            attachedSystems[entity].Clear();

            entityBuffer.Push(entity);
        }

        /// <summary>
        /// Adds a given collection of <see cref="IComponent"/> data to a given entity.
        /// </summary>
        /// <param name="entity">The entity that will be modified.</param>
        /// <param name="components">The collection of <see cref="IComponent"/> data that will be added.</param>
        public void AddComponent(int entity, params IComponent[] components)
        {
            componentManager.RegisterComponent(components);

            AssignComponents(entity, components);
            AssignSystems(entity);
        }

        /// <summary>
        /// Removes a given collection of <see cref="IComponent"/> types from a given entity.
        /// </summary>
        /// <param name="entity">The entity that will be modified.</param>
        /// <param name="componentTypes">The collection of <see cref="IComponent"/> types that will be removed.</param>
        public void RemoveComponent(int entity, params Type[] componentTypes)
        {
            int componentID;
            for (int i = 0; i < componentTypes.Length; i++)
            {
                componentID = componentManager.GetComponentID(componentTypes[i]);

                if (componentID == -1)
                    continue;

                componentManager.Data[componentID][entity] = null;
                attachedComponents[entity].Remove((uint)componentID);

                foreach (uint j in attachedSystems[entity])
                {
                    if (systemManager.Systems[j].RequiredComponents.Contains(componentTypes[i]))
                    {
                        systemManager.Systems[j].RemoveEntity(entity);
                    }
                }
            }
        }

        /// <summary>
        /// Returns whether or not a given entity contains a given collection of <see cref="IComponent"/> types.
        /// </summary>
        /// <param name="entity">The entity that will be checked.</param>
        /// <param name="components">The set of <see cref="IComponent"/> types that will be checked for.</param>
        /// <returns>Whether or not a given entity contains a given collection of <see cref="IComponent"/> types.</returns>
        public bool EntityContains(int entity, HashSet<Type> components)
        {
            if (components.Count == 0)
                return false;

            int componentID;
            foreach (Type component in components)
            {
                componentID = componentManager.GetComponentID(component);

                if (componentID == -1 || !attachedComponents[entity].Contains((uint)componentID))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns whether or not a given entity contains a given collection of <see cref="IComponent"/> types.
        /// </summary>
        /// <param name="entity">The entity that will be checked.</param>
        /// <param name="components">The collection of <see cref="IComponent"/> types that will be checked for.</param>
        /// <returns>Whether or not a given entity contains a given collection of <see cref="IComponent"/> types.</returns>
        public bool EntityContains(int entity, params Type[] components)
        {
            if (components.Length == 0)
                return false;

            int componentID;
            for (int i = 0; i < components.Length; i++)
            {
                componentID = componentManager.GetComponentID(components[i]);

                if (componentID == -1 || !attachedComponents[entity].Contains((uint)componentID))
                    return false;
            }

            return true;
        }

        private void AssignComponents(int entity, params IComponent[] components)
        {
            int componentID;
            for (int i = 0; i < components.Length; i++)
            {
                componentID = componentManager.GetComponentID(components[i]);

                if (componentID == -1)
                    continue;

                componentManager.Data[componentID][entity] = components[i];
                attachedComponents[entity].Add((uint)componentID);
            }
        }

        private void AssignSystems(int entity)
        {
            for (int i = 0; i < systemManager.TotalSystemsRegistered; i++)
            {
                systemManager.Systems[i].RemoveEntity(entity);

                if (EntityContains(entity, systemManager.Systems[i].RequiredComponents) && !EntityContains(entity, systemManager.Systems[i].BlacklistedComponents))
                {
                    systemManager.Systems[i].AddEntity(entity);
                    attachedSystems[entity].Add((uint)i);
                }
            }
        }
    }
}
