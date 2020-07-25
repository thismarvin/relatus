using Relatus.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.ECS
{
    public class MorroFactory
    {
        public int SystemCapacity => systemManager.Capacity;
        public int ComponentCapacity => componentManager.Capacity;
        public int EntityCapacity => entityManager.Capacity;
        public int EntityCount => entityManager.EntityCount;

        private readonly SystemManager systemManager;
        private readonly ComponentManager componentManager;
        private readonly EntityManager entityManager;
        private readonly EventManager eventManager;

        private readonly Stack<Tuple<int, IComponent[]>> componentAddition;
        private readonly Stack<Tuple<int, Type[]>> componentSubtraction;
        private readonly SparseSet entityRemovalQueue;

        /// <summary>
        /// Creates a factory that provides ECS funcitonality.
        /// </summary> 
        /// <param name="entityCapacity">The total of amount of entities supported by this factory.</param>
        /// <param name="componentCapacity">The total of amount of unique <see cref="IComponent"/> types supported by this factory.</param>
        /// <param name="systemCapacity">The total of amount of unique <see cref="MorroSystem"/> types supported by this factory.</param>
        public MorroFactory(int entityCapacity, int componentCapacity, int systemCapacity)
        {
            systemManager = new SystemManager(systemCapacity);
            componentManager = new ComponentManager(componentCapacity, entityCapacity);
            entityManager = new EntityManager(entityCapacity, systemManager, componentManager);
            eventManager = new EventManager(systemManager);

            componentAddition = new Stack<Tuple<int, IComponent[]>>();
            componentSubtraction = new Stack<Tuple<int, Type[]>>();
            entityRemovalQueue = new SparseSet(entityCapacity);
        }

        public SystemGroup CreateGroup(params MorroSystem[] systems)
        {
            return new SystemGroup(systems.Length).Add(systems);
        }

        public MorroFactory Jumpstart(params SystemGroup[] systemGroups)
        {
            Register(systemGroups);
            eventManager.LinkSystems();

            return this;
        }

        public MorroFactory Register(params SystemGroup[] groups)
        {
            for (int i = 0; i < groups.Length; i++)
            {
                systemManager.RegisterSystem(groups[i].Systems);
            }

            return this;
        }

        #region Entity Helper Methods
        /// <summary>
        /// Creates an entity from a given collection of <see cref="IComponent"/> data.
        /// </summary>
        /// <param name="components">A collection of <see cref="IComponent"/> data that represents an entity.</param>
        /// <returns>The unique identifier of the entity that was just created.</returns>
        public int CreateEntity(params IComponent[] components)
        {
            int entity = entityManager.AllocateEntity();
            entityManager.AddComponent(entity, components);

            return entity;
        }

        /// <summary>
        /// Queues the deletion of a given entity.
        /// </summary>
        /// <param name="entity">The entity to be removed from the factory.</param>
        public MorroFactory RemoveEntity(int entity)
        {
            entityRemovalQueue.Add((uint)entity);

            return this;
        }

        /// <summary>
        /// Returns whether or not a given entity contains a given collection of <see cref="IComponent"/> types.
        /// </summary>
        /// <param name="entity">The entity that will be checked.</param>
        /// <param name="components">The collection of <see cref="IComponent"/> types that will be checked for.</param>
        /// <returns>Whether or not a given entity contains a given collection of <see cref="IComponent"/> types.</returns>
        public bool EntityContains(int entity, params Type[] components)
        {
            return entityManager.EntityContains(entity, components);
        }

        /// <summary>
        /// Adds a given collection of <see cref="IComponent"/> data to a given entity.
        /// </summary>
        /// <param name="entity">The entity that will be modified.</param>
        /// <param name="components">The collection of <see cref="IComponent"/> data that will be added.</param>
        public MorroFactory AddComponents(int entity, params IComponent[] components)
        {
            componentAddition.Push(new Tuple<int, IComponent[]>(entity, components));

            return this;
        }

        /// <summary>
        /// Removes a given collection of <see cref="IComponent"/> types from a given entity.
        /// </summary>
        /// <param name="entity">The entity that will be modified.</param>
        /// <param name="componentTypes">The collection of <see cref="IComponent"/> types that will be removed.</param>
        public MorroFactory RemoveComponents(int entity, params Type[] componentTypes)
        {
            componentSubtraction.Push(new Tuple<int, Type[]>(entity, componentTypes));

            return this;
        }
        #endregion

        /// <summary>
        /// Returns an array of all of the data of a given <see cref="IComponent"/> type.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IComponent"/> data to be retrieved.</typeparam>
        /// <returns>An array of all of the data of a given <see cref="IComponent"/> type.</returns>
        public IComponent[] GetData<T>() where T : IComponent
        {
            return componentManager.GetData<T>();
        }

        /// <summary>
        /// Returns the <see cref="IComponent"/> data of a given entity.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IComponent"/> data to be retrieved.</typeparam>
        /// <param name="entity">The target entity to retrieve data from.</param>
        /// <returns>The <see cref="IComponent"/> data of a given entity.</returns>
        public T GetData<T>(int entity) where T : IComponent
        {
            return componentManager.GetData<T>(entity);
        }

        /// <summary>
        /// Returns a <see cref="MorroSystem"/> of a given type.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="MorroSystem"/> to retrieve.</typeparam>
        /// <returns>A <see cref="MorroSystem"/> of a given type.</returns>
        public T GetSystem<T>() where T : MorroSystem
        {
            return systemManager.GetSystem<T>();
        }

        /// <summary>
        /// Applies any recent changes to entity data, and maintains the integrity of all of the factory's systems.
        /// </summary>
        public MorroFactory ApplyChanges()
        {
            // Handles removing components from entities.
            while (componentSubtraction.Count > 0)
            {
                Tuple<int, Type[]> modification = componentSubtraction.Pop();
                entityManager.RemoveComponent(modification.Item1, modification.Item2);
            }

            // Handles adding components to entities.
            while (componentAddition.Count > 0)
            {
                Tuple<int, IComponent[]> modification = componentAddition.Pop();
                entityManager.AddComponent(modification.Item1, modification.Item2);
            }

            // Handles removing entities.
            if (entityRemovalQueue.Count != 0)
            {
                foreach (uint entity in entityRemovalQueue)
                {
                    entityManager.ClearEntity((int)entity);
                }
                entityRemovalQueue.Clear();
            }

            // Update all systems with the new entity data.
            systemManager.ApplyChanges();

            return this;
        }

        /// <summary>
        /// Runs all registered <see cref="IUpdateableSystem"/> systems in a given group.
        /// </summary>
        public MorroFactory RunUpdateableSystems(params SystemGroup[] groups)
        {
            for (int i = 0; i < groups.Length; i++)
            {
                groups[i].RunUpdateableSystems();
            }

            return this;
        }

        /// <summary>
        /// Runs all <see cref="IDrawableSystem"/> systems in a given group.
        /// </summary>
        public MorroFactory RunDrawableSystems(Camera camera, params SystemGroup[] groups)
        {
            for (int i = 0; i < groups.Length; i++)
            {
                groups[i].RunDrawableSystems(camera);
            }

            return this;
        }
    }
}
