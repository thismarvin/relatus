using System;
using System.Collections.Generic;

namespace Relatus.EB
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

        public Entity AddBehavior(Behavior behavior)
        {
            Type type = behavior.GetType();

            if (behaviorLookup.ContainsKey(type))
                throw new RelatusException("An entity cannot have two behaviors of the same Type.", new ArgumentException());

            BehaviorTypes.Add(type);
            Behaviors.Add(behavior);

            return this;
        }

        public bool Contains(params Type[] behaviorTypes)
        {
            for (int i = 0; i < behaviorTypes.Length; i++)
            {
                if (!BehaviorTypes.Contains(behaviorTypes[i]))
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
            for (int i = 0; i < Behaviors.Count; i++)
            {
                if (Behaviors[i] is IUpdateableBehavior behavior)
                {
                    behavior.Update();
                }
            }
        }

        public void Draw(Camera camera)
        {
            for (int i = 0; i < Behaviors.Count; i++)
            {
                if (Behaviors[i] is IDrawableBehavior behavior)
                {
                    behavior.Draw(camera);
                }
            }
        }
    }
}
