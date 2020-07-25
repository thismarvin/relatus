using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.ECS
{
    /// <summary>
    /// Processes a particular set of <see cref="IComponent"/> data, and performs specialized logic on said data.
    /// </summary>
    public abstract class MorroSystem : IMorroSystem
    {
        public bool Enabled { get; set; }
        public HashSet<Type> RequiredComponents { get; private set; }
        public HashSet<Type> BlacklistedComponents { get; private set; }
        public HashSet<Type> Subscriptions { get; private set; }

        // The following properties need to be internal for the UpdateHandlers to work.
        // This probably isn't the best design, but it works for now!
        protected internal HashSet<int> Entities { get; private set; }
        protected internal int[] EntitiesAsArray { get; private set; }

        protected MorroFactory factory;

        private readonly int[] entitiesAsArray;
        private readonly Stack<int> entityRemoval;
        private readonly Stack<int> entityAddition;
        private bool entityDataChanged;

        public MorroSystem(MorroFactory factory)
        {
            this.factory = factory;

            RequiredComponents = new HashSet<Type>();
            BlacklistedComponents = new HashSet<Type>();
            Subscriptions = new HashSet<Type>();
            Entities = new HashSet<int>();
            entitiesAsArray = new int[factory.EntityCapacity];
            entityRemoval = new Stack<int>(factory.EntityCapacity);
            entityAddition = new Stack<int>(factory.EntityCapacity);

            Enabled = true;
        }

        /// <summary>
        /// Initialize a set of <see cref="IComponent"/> types that all entities associated with this system must have.
        /// </summary>
        /// <param name="components">The types of <see cref="IComponent"/> this system will require before associating with an entity.</param>
        public void Require(params Type[] components)
        {
            RequiredComponents.Clear();

            for (int i = 0; i < components.Length; i++)
            {
                RequiredComponents.Add(components[i]);
            }
        }

        /// <summary>
        /// Initialize a set of <see cref="IComponent"/> types this system must avoid before associating itself with an entity.
        /// </summary>
        /// <param name="components">The types of <see cref="IComponent"/> this system will avoid during initialization.</param>
        public void Avoid(params Type[] components)
        {
            BlacklistedComponents.Clear();

            for (int i = 0; i < components.Length; i++)
            {
                BlacklistedComponents.Add(components[i]);
            }
        }

        /// <summary>
        /// Initialize a set of <see cref="IEventAnnouncer"/> types this system will subscribe to for future events.
        /// </summary>
        /// <param name="systems">The types of <see cref="IEventAnnouncer"/> this system will subscribe to.</param>
        public void Subscribe(params Type[] systems)
        {
            Subscriptions.Clear();

            for (int i = 0; i < systems.Length; i++)
            {
                Subscriptions.Add(systems[i]);
            }
        }

        internal void AddEntity(int entity)
        {
            entityAddition.Push(entity);
            entityDataChanged = true;
        }

        internal void RemoveEntity(int entity)
        {
            entityRemoval.Push(entity);
            entityDataChanged = true;
        }

        internal void ApplyChanges()
        {
            if (!entityDataChanged)
                return;

            while (entityRemoval.Count > 0)
            {
                int entity = entityRemoval.Pop();

                if (!Entities.Contains(entity))
                    continue;

                Entities.Remove(entity);
            }

            while (entityAddition.Count > 0)
            {
                Entities.Add(entityAddition.Pop());
            }

            int entityIndex = 0;
            foreach (int entity in Entities)
            {
                entitiesAsArray[entityIndex++] = entity;
            }

            entityDataChanged = false;
        }
    }
}
