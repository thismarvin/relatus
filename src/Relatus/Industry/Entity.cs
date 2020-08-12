using System;
using System.Collections.Generic;

namespace Relatus.Industry
{
    public class Entity
    {
        public List<Behavior> Behaviors => new List<Behavior>(behaviorLookup.Values);
        public HashSet<Type> BehaviorTypes => new HashSet<Type>(behaviorLookup.Keys);

        private Dictionary<Type, Behavior> behaviorLookup;

        public Entity()
        {
            behaviorLookup = new Dictionary<Type, Behavior>();
        }

        public Entity AddBehavior(params Behavior[] behaviors)
        {
            for (int i = 0; i < behaviors.Length; i++)
            {
                Type type = behaviors[i].GetType();

                if (behaviorLookup.ContainsKey(type))
                    throw new RelatusException("An entity cannot have two behaviors of the same Type.", new ArgumentException());

                behaviorLookup.Add(type, behaviors[i]);
            }

            return this;
        }

        public Entity RemoveBehavior<T>()
        {
            Type type = typeof(T);

            if (!behaviorLookup.ContainsKey(type))
                return this;

            behaviorLookup.Remove(type);

            return this;
        }

        public Entity RemoveBehaviors(params Type[] behaviorTypes)
        {
            for (int i = 0; i < behaviorTypes.Length; i++)
            {
                if (!behaviorLookup.ContainsKey(behaviorTypes[i]))
                    continue;

                behaviorLookup.Remove(behaviorTypes[i]);

            }
            return this;
        }

        public bool Contains(params Type[] behaviorTypes)
        {
            for (int i = 0; i < behaviorTypes.Length; i++)
            {
                if (!behaviorLookup.ContainsKey(behaviorTypes[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public Behavior GetBehavior<T>() where T : Behavior
        {
            Type type = typeof(T);

            if (!Contains(type))
                throw new RelatusException("The current entity does not contain a Behavior of that type.", new KeyNotFoundException());

            return behaviorLookup[type];
        }

        public void Update()
        {
            foreach (Behavior b in behaviorLookup.Values)
            {
                if (b is IUpdateableBehavior behavior)
                {
                    behavior.Update();
                }
            }
        }

        public void Draw(Camera camera)
        {
            foreach (Behavior b in behaviorLookup.Values)
            {
                if (b is IDrawableBehavior behavior)
                {
                    behavior.Draw(camera);
                }
            }
        }
    }
}
