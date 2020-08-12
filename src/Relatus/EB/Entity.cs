using System;
using System.Collections.Generic;

namespace Relatus.EB
{
    public class Entity
    {
        public List<Behavior> Behaviors => new List<Behavior>(behaviorLookup.Values);
        public HashSet<Type> BehaviorTypes => new HashSet<Type>(behaviorLookup.Keys);

        private Dictionary<Type, Behavior> behaviorLookup;

        private Stack<Behavior> behaviorAddition;
        private Stack<Type> behaviorSubtraction;
        private bool modified;

        public Entity()
        {
            behaviorLookup = new Dictionary<Type, Behavior>();
            behaviorAddition = new Stack<Behavior>();
            behaviorSubtraction = new Stack<Type>();
        }

        public Entity AddBehavior(Behavior behavior)
        {
            Type type = behavior.GetType();

            if (behaviorLookup.ContainsKey(type))
                throw new RelatusException("An entity cannot have two behaviors of the same Type.", new ArgumentException());

            behaviorAddition.Push(behavior);
            modified = true;

            return this;
        }

        public Entity RemoveBehavior<T>()
        {
            Type type = typeof(T);

            if (!behaviorLookup.ContainsKey(type))
                throw new RelatusException("The current entity does not contain a Behavior of that type.", new KeyNotFoundException());

            behaviorSubtraction.Push(type);
            modified = true;

            return this;
        }

        public Entity ApplyChanges()
        {
            if (!modified)
                return this;

            while (behaviorSubtraction.Count > 0)
            {
                behaviorLookup.Remove(behaviorSubtraction.Pop());
            }

            while (behaviorAddition.Count > 0)
            {
                Behavior behavior = behaviorAddition.Pop();
                behaviorLookup.Add(behavior.GetType(), behavior);
            }

            modified = false;

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
            if (modified)
                throw new RelatusException("The entity was modified, but ApplyChanges() was never called.", new MethodExpectedException());

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
            if (modified)
                throw new RelatusException("The entity was modified, but ApplyChanges() was never called.", new MethodExpectedException());

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
