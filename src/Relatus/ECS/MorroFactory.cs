using Relatus.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.ECS
{
    public partial class MorroFactory
    {
        public uint SystemCapacity { get; private set; }
        public uint ComponentCapacity { get; private set; }
        public uint EntityCapacity { get; private set; }

        private readonly Queue<Tuple<uint, IComponent[]>> componentAddition;
        private readonly Queue<Tuple<uint, Type[]>> componentSubtraction;
        private readonly SparseSet entityRemovalQueue;
        private bool dataModified;

        /// <summary>
        /// Creates a factory that provides ECS funcitonality.
        /// </summary>
        /// <param name="entityCapacity">The total of amount of entities supported by this factory.</param>
        /// <param name="componentCapacity">The total of amount of unique <see cref="IComponent"/> types supported by this factory.</param>
        /// <param name="systemCapacity">The total of amount of unique <see cref="MorroSystem"/> types supported by this factory.</param>
        public MorroFactory(uint entityCapacity, uint componentCapacity, uint systemCapacity)
        {
            EntityCapacity = entityCapacity;
            ComponentCapacity = componentCapacity;
            SystemCapacity = systemCapacity;

            componentAddition = new Queue<Tuple<uint, IComponent[]>>((int)EntityCapacity);
            componentSubtraction = new Queue<Tuple<uint, Type[]>>((int)EntityCapacity);
            entityRemovalQueue = new SparseSet(EntityCapacity);

            // System Setup
            systems = new MorroSystem[SystemCapacity];
            systemLookup = new Dictionary<Type, uint>((int)SystemCapacity);

            // Component Setup
            componentData = new IComponent[ComponentCapacity][];
            componentLookup = new Dictionary<Type, uint>((int)ComponentCapacity);

            for (int i = 0; i < ComponentCapacity; i++)
            {
                componentData[i] = new IComponent[EntityCapacity];
            }

            // Entity Setup
            attachedComponents = new SparseSet[EntityCapacity];
            attachedSystems = new SparseSet[EntityCapacity];
            vacancies = new Queue<uint>((int)EntityCapacity);

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
        public uint CreateEntity(params IComponent[] components)
        {
            uint entity = AllocateEntity();
            componentAddition.Enqueue(new Tuple<uint, IComponent[]>(entity, components));
            dataModified = true;

            return entity;
        }

        /// <summary>
        /// Queues the deletion of a given entity.
        /// </summary>
        /// <param name="entity">The entity to be removed from the factory.</param>
        public MorroFactory RemoveEntity(uint entity)
        {
            entityRemovalQueue.Add(entity);
            dataModified = true;

            return this;
        }

        /// <summary>
        /// Queues the addition of a given collection of <see cref="IComponent"/> data to a given entity.
        /// </summary>
        /// <param name="entity">The entity that will be modified.</param>
        /// <param name="components">The collection of <see cref="IComponent"/> data that will be added.</param>
        public MorroFactory AddComponents(uint entity, params IComponent[] components)
        {
            componentAddition.Enqueue(new Tuple<uint, IComponent[]>(entity, components));
            dataModified = true;

            return this;
        }

        /// <summary>
        /// Queues the removal of a given collection of <see cref="IComponent"/> types from a given entity.
        /// </summary>
        /// <param name="entity">The entity that will be modified.</param>
        /// <param name="componentTypes">The collection of <see cref="IComponent"/> types that will be removed.</param>
        public MorroFactory RemoveComponents(uint entity, params Type[] componentTypes)
        {
            componentSubtraction.Enqueue(new Tuple<uint, Type[]>(entity, componentTypes));
            dataModified = true;

            return this;
        }
        #endregion

        ///<summary>
        /// Clears all entities of any attached components and systems.
        ///</summary>
        public MorroFactory ClearEntities()
        {
            // Remove any attached components and systems from each entity.
            for (int i = 0; i < EntityCapacity; i++)
            {
                attachedComponents[i].Clear();
                attachedSystems[i].Clear();
            }

            // Make sure the component for each entity is empty.
            for (int i = 0; i < componentIndex; i++)
            {
                for (int j = 0; j < EntityCapacity; j++)
                {
                    // ! Might want to have Dispose() here.
                    componentData[i][j] = null;
                }
            }

            // Remove all references to entities from each system.
            for (int i = 0; i < systemIndex; i++)
            {
                systems[i].ClearEntities();
            }

            componentAddition.Clear();
            componentSubtraction.Clear();
            entityRemovalQueue.Clear();
            vacancies.Clear();

            nextEntity = 0;
            totalEntitiesCreated = 0;
            dataModified = false;

            return this;
        }

        /// <summary>
        /// Applies any recent changes to entity data, and maintains the integrity of all of the factory's systems.
        /// </summary>
        public MorroFactory ApplyChanges()
        {
            if (!dataModified)
                return this;

            // Handles removing components from entities.
            while (componentSubtraction.Count > 0)
            {
                Tuple<uint, Type[]> modification = componentSubtraction.Dequeue();
                RemoveComponent(modification.Item1, modification.Item2);
            }

            // Handles adding components to entities.
            while (componentAddition.Count > 0)
            {
                Tuple<uint, IComponent[]> modification = componentAddition.Dequeue();
                AddComponent(modification.Item1, modification.Item2);
            }

            // Handles removing entities.
            if (entityRemovalQueue.Count != 0)
            {
                foreach (uint entity in entityRemovalQueue)
                {
                    ClearEntity(entity);
                }
                entityRemovalQueue.Clear();
            }

            // Update all systems with the new entity data.
            UpdateSystems();

            dataModified = false;

            return this;
        }

        /// <summary>
        /// Runs all registered <see cref="IUpdateableSystem"/> systems in a given group.
        /// </summary>
        public MorroFactory RunUpdateableSystems(params SystemGroup[] groups)
        {
            if (dataModified)
                throw new RelatusException("The Factory's data was modified, but ApplyChanges() was never called.", new MethodExpectedException());

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
            if (dataModified)
                throw new RelatusException("The Factory's data was modified, but ApplyChanges() was never called.", new MethodExpectedException());

            for (int i = 0; i < groups.Length; i++)
            {
                groups[i].RunDrawableSystems(camera);
            }

            return this;
        }
    }
}
