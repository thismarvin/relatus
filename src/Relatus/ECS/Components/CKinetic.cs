using Microsoft.Xna.Framework;
using Relatus.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.ECS
{
    class CKinetic : IComponent
    {
        public Vector2 Velocity { get; set; }
        public Vector2 Acceleration { get; set; }        
        public TimeSpan LastUpdate { get; set; }
        public float Accumulator { get; set; }

        public CKinetic()
        {
            Velocity = Vector2.Zero;
            Acceleration = Vector2.Zero;

            LastUpdate = new TimeSpan(Engine.TotalGameTime.Ticks);
        }

        public CKinetic(Vector2 initialVelocity, Vector2 initialAcceleration)
        {
            Velocity = initialVelocity;
            Acceleration = initialAcceleration;

            LastUpdate = new TimeSpan(Engine.TotalGameTime.Ticks);
        }
    }
}
