using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.ECS.Bundled
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

        private IComponent[] positions;
        private IComponent[] kinetics;

        public SPhysics(MorroFactory factory, uint targetFramerate, Integrator integrator) : base(factory)
        {
            Require(typeof(CPosition), typeof(CKinetic));

            target = 1f / targetFramerate;
            this.integrator = integrator;
        }

        public override void EnableFixedUpdate(uint updatesPerSecond)
        {
            throw new RelatusException("SPhysics is not compatible with the default fixed update functionality (it implements its own).", new NotSupportedException());
        }

        public override void UpdateEntity(uint entity)
        {
            Simulate(entity);
        }

        public override void Update()
        {
            positions ??= factory.GetData<CPosition>();
            kinetics ??= factory.GetData<CKinetic>();

            base.Update();
        }

        private void Simulate(uint entity)
        {
            CPosition position = (CPosition)positions[entity];
            CKinetic kinetic = (CKinetic)kinetics[entity];

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
                    kinetic.Velocity += kinetic.Acceleration * deltaTime;

                    position.X += kinetic.Velocity.X * deltaTime;
                    position.Y += kinetic.Velocity.Y * deltaTime;
                    break;

                case Integrator.VelocityVerlet:
                    position.X += kinetic.Velocity.X * deltaTime + 0.5f * kinetic.Acceleration.X * deltaTime * deltaTime;
                    position.Y += kinetic.Velocity.Y * deltaTime + 0.5f * kinetic.Acceleration.Y * deltaTime * deltaTime;

                    kinetic.Velocity += kinetic.Acceleration * deltaTime;
                    break;
            }
        }
    }
}
