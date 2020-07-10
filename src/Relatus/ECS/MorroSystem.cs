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
        public HashSet<Type> Dependencies { get; private set; }
        public HashSet<Type> Subscriptions { get; private set; }
        protected internal HashSet<int> Entities { get; private set; }

        protected internal int[] EntitiesAsArray
        {
            get
            {
                if (!entityDataChanged)
                {
                    return entitiesAsArray;
                }
                else
                {
                    entityDataChanged = false;

                    int entityIndex = 0;

                    foreach (int entity in Entities)
                    {
                        entitiesAsArray[entityIndex++] = entity;
                    }

                    return entitiesAsArray;
                }
            }
        }

        protected Scene scene;

        private readonly int[] entitiesAsArray;
        private bool entityDataChanged;

        public MorroSystem(Scene scene)
        {
            RequiredComponents = new HashSet<Type>();
            BlacklistedComponents = new HashSet<Type>();
            Dependencies = new HashSet<Type>();
            Subscriptions = new HashSet<Type>();
            Entities = new HashSet<int>();
            entitiesAsArray = new int[scene.EntityCapacity];

            Enabled = true;
            this.scene = scene;
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
        /// Initialize a set of <see cref="MorroSystem"/> types this system depends on running first before this system can run.
        /// </summary>
        /// <param name="systems">The types of <see cref="MorroSystem"/> this system depends on running first.</param>
        public void Depend(params Type[] systems)
        {
            Dependencies.Clear();

            for (int i = 0; i < systems.Length; i++)
            {
                Dependencies.Add(systems[i]);
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
            if (Entities.Contains(entity))
                return;

            Entities.Add(entity);
            entityDataChanged = true;
        }

        internal void RemoveEntity(int entity)
        {
            if (!Entities.Contains(entity))
                return;

            Entities.Remove(entity);
            entityDataChanged = true;
        }
    }
}
