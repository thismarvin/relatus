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

        /// <summary>
        /// By default all registered <see cref="MorroSystem"/>'s are ran asyncronously.
        /// If this functionality is disabled, systems will run in the order they were registered.
        /// </summary>
        public bool AsynchronousSystemsEnabled
        {
            get => !systemManager.DisableAsynchronousUpdates;
            set => systemManager.DisableAsynchronousUpdates = !value;
        }

        private readonly SystemManager systemManager;
        private readonly ComponentManager componentManager;
        private readonly EntityManager entityManager;
        private readonly EventManager eventManager;

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

            entityRemovalQueue = new SparseSet(entityCapacity);
        }

        /// <summary>
        /// Registers a given collection of <see cref="MorroSystem"/>'s to be managed by the scene.
        /// </summary>
        /// <param name="morroSystem">A collection of <see cref="MorroSystem"/>'s to be registered and managed by the scene.</param>
        public void RegisterSystems(params MorroSystem[] morroSystem)
        {
            systemManager.RegisterSystem(morroSystem);
            eventManager.LinkSystems();
        }

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
        /// <param name="entity">The entity to be removed from the scene.</param>
        public void RemoveEntity(int entity)
        {
            entityRemovalQueue.Add((uint)entity);
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
        public void AddComponents(int entity, params IComponent[] components)
        {
            entityManager.AddComponent(entity, components);
        }

        /// <summary>
        /// Removes a given collection of <see cref="IComponent"/> types from a given entity.
        /// </summary>
        /// <param name="entity">The entity that will be modified.</param>
        /// <param name="componentTypes">The collection of <see cref="IComponent"/> types that will be removed.</param>
        public void RemoveComponents(int entity, params Type[] componentTypes)
        {
            entityManager.RemoveComponent(entity, componentTypes);
        }

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
        /// Runs all registered <see cref="IUpdateableSystem"/> systems.
        /// </summary>
        public void RunUpdateableSystems()
        {
            systemManager.Update();

            if (entityRemovalQueue.Count != 0)
            {
                foreach (uint entity in entityRemovalQueue)
                {
                    entityManager.ClearEntity((int)entity);
                }
                entityRemovalQueue.Clear();
            }
        }

        /// <summary>
        /// Runs all registered <see cref="IDrawableSystem"/> systems.
        /// </summary>
        public void RunDrawableSystems(Camera camera)
        {
            systemManager.Draw(camera);
        }
    }
}
