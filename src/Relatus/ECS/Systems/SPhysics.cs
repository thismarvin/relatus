using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.ECS
{
    /// <summary>
    /// The types of integrators available to the built in physics system.
    /// </summary>
    public enum Integrator
    {
        SemiImplictEuler,
        VelocityVerlet
    }

    public class SPhysics : UpdateSystem
    {
        private readonly float target;
        private readonly Integrator integrator;

        private CPosition[] positions;
        private CKinetic[] kinetics;

        public SPhysics(MorroFactory factory, Integrator integrator, uint tasks, int targetFPS) : base(factory, tasks)
        {
            Require(typeof(CPosition), typeof(CKinetic));

            target = 1f / targetFPS;
            this.integrator = integrator;
        }

        public override void UpdateEntity(int entity)
        {
            Simultate(entity);
        }

        public override void Update()
        {
            positions = positions ?? factory.GetData<CPosition>();
            kinetics = kinetics ?? factory.GetData<CKinetic>();

            base.Update();
        }

        private void Simultate(int entity)
        {
            CPosition position = positions[entity];
            CKinetic kinetic = kinetics[entity];

            kinetic.Accumulator += (float)(Engine.TotalGameTime - kinetic.LastUpdate).TotalSeconds;
            kinetic.LastUpdate = new TimeSpan(Engine.TotalGameTime.Ticks);

            while (kinetic.Accumulator >= target)
            {
                Integrate(position, kinetic, integrator, target);
                kinetic.Accumulator -= target;
            }
        }

        private void Integrate(CPosition position, CKinetic kinetic, Integrator integrator, float deltaTime)
        {
            switch (integrator)
            {
                case Integrator.SemiImplictEuler:
                    SemiImplictEulerIntegration();
                    break;

                case Integrator.VelocityVerlet:
                    VelocityVerletIntegration();
                    break;
            }

            void SemiImplictEulerIntegration()
            {
                kinetic.Velocity += kinetic.Acceleration * deltaTime;

                position.X += kinetic.Velocity.X * deltaTime;
                position.Y += kinetic.Velocity.Y * deltaTime;
            }

            void VelocityVerletIntegration()
            {
                position.X += kinetic.Velocity.X * deltaTime + 0.5f * kinetic.Acceleration.X * deltaTime * deltaTime;
                position.Y += kinetic.Velocity.Y * deltaTime + 0.5f * kinetic.Acceleration.Y * deltaTime * deltaTime;

                kinetic.Velocity += kinetic.Acceleration * deltaTime;
            }
        }
    }
}
