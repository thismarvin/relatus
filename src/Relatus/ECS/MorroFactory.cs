using Relatus.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.ECS
{
    public partial class MorroFactory
    {
        public int SystemCapacity { get; private set; }
        public int ComponentCapacity { get; private set; }
        public int EntityCapacity { get; private set; }

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
            EntityCapacity = entityCapacity;
            ComponentCapacity = componentCapacity;
            SystemCapacity = systemCapacity;

            componentAddition = new Stack<Tuple<int, IComponent[]>>();
            componentSubtraction = new Stack<Tuple<int, Type[]>>();
            entityRemovalQueue = new SparseSet(EntityCapacity);

            // System Setup
            systems = new MorroSystem[SystemCapacity];
            registeredSystems = new HashSet<Type>();
            systemLookup = new Dictionary<Type, int>();

            // Component Setup
            registeredComponents = new HashSet<Type>();
            componentLookup = new Dictionary<Type, int>();
            componentData = new IComponent[ComponentCapacity][];

            for (int i = 0; i < ComponentCapacity; i++)
            {
                componentData[i] = new IComponent[EntityCapacity];
            }

            // Entity Setup
            attachedComponents = new SparseSet[EntityCapacity];
            attachedSystems = new SparseSet[EntityCapacity];
            entityBuffer = new Stack<int>(EntityCapacity);

            for (int i = 0; i < EntityCapacity; i++)
            {
                attachedComponents[i] = new SparseSet(ComponentCapacity);
                attachedSystems[i] = new SparseSet(SystemCapacity);
            }
        }

        public SystemGroup CreateGroup(params MorroSystem[] systems)
        {
            return new SystemGroup(systems.Length).Add(systems);
        }

        /// <summary>
        /// Exposes all systems in a given collection of <see cref="SystemGroup"/>'s to this factory, and initializes the factory.
        /// </summary>
        /// <param name="systemGroups">The system groups to be exposed to this factory.</param>
        /// <returns></returns>
        public MorroFactory Jumpstart(params SystemGroup[] systemGroups)
        {
            for (int i = 0; i < systemGroups.Length; i++)
            {
                RegisterSystem(systemGroups[i].Systems);
            }

            LinkSystems();

            return this;
        }

        #region Entity Helper Methods
        /// <summary>
        /// Queues the creation of an entity. The entity will consist of the given collection of <see cref="IComponent"/> data.
        /// </summary>
        /// <param name="components">A collection of <see cref="IComponent"/> data that represents an entity.</param>
        /// <returns>The unique identifier of the entity that was just created.</returns>
        public int CreateEntity(params IComponent[] components)
        {
            int entity = AllocateEntity();
            componentAddition.Push(new Tuple<int, IComponent[]>(entity, components));

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
        /// Queues the addition of a given collection of <see cref="IComponent"/> data to a given entity.
        /// </summary>
        /// <param name="entity">The entity that will be modified.</param>
        /// <param name="components">The collection of <see cref="IComponent"/> data that will be added.</param>
        public MorroFactory AddComponents(int entity, params IComponent[] components)
        {
            componentAddition.Push(new Tuple<int, IComponent[]>(entity, components));

            return this;
        }

        /// <summary>
        /// Queues the removal of a given collection of <see cref="IComponent"/> types from a given entity.
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
        /// Applies any recent changes to entity data, and maintains the integrity of all of the factory's systems.
        /// </summary>
        public MorroFactory ApplyChanges()
        {
            // Handles removing components from entities.
            while (componentSubtraction.Count > 0)
            {
                Tuple<int, Type[]> modification = componentSubtraction.Pop();
                RemoveComponent(modification.Item1, modification.Item2);
            }

            // Handles adding components to entities.
            while (componentAddition.Count > 0)
            {
                Tuple<int, IComponent[]> modification = componentAddition.Pop();
                AddComponent(modification.Item1, modification.Item2);
            }

            // Handles removing entities.
            if (entityRemovalQueue.Count != 0)
            {
                foreach (uint entity in entityRemovalQueue)
                {
                    ClearEntity((int)entity);
                }
                entityRemovalQueue.Clear();
            }

            // Update all systems with the new entity data.
            UpdateSystems();

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
