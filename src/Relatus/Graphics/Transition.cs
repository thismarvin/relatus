using Relatus.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Graphics
{
    public enum TransitionType
    {
        Enter,
        Exit
    }

    public abstract class Transition
    {
        public bool Started { get; private set; }
        public bool Done { get; protected set; }
        
        protected TransitionType Type { get; private set; }
        protected Camera Camera { get; private set; }
        protected float Force { get; private set; }        

        private bool setup;
        private bool lastDraw;

        private readonly float initialVelocity;
        private float velocity;
        private readonly float acceleration;

        private readonly float deltaTime;
        private double accumulator;

        internal Transition(TransitionType type, float velocity, float acceleration)
        {
            Type = type;
            Camera = CameraManager.GetCamera(CameraType.Static);

            initialVelocity = velocity;
            this.velocity = initialVelocity;
            this.acceleration = acceleration;

            deltaTime = 1f / 240;
        }

        public virtual void Reset()
        {
            Started = false;
            Done = false;
            setup = false;
            lastDraw = false;

            velocity = initialVelocity;
            Force = 0;
        }

        public void Begin()
        {
            Started = true;
        }

        protected void FlagCompletion()
        {
            lastDraw = true;
        }

        private void CalculateForce()
        {
            velocity += acceleration * deltaTime;
            Force += velocity * deltaTime;
        }

        protected abstract void SetupTransition();

        protected abstract void AccommodateToCamera();

        protected abstract void UpdateLogic();

        protected abstract void DrawTransition();

        public void Update()
        {
            if (!setup || Done)
                return;

            accumulator += Engine.DeltaTime;

            while (accumulator >= deltaTime)
            {
                CalculateForce();
                AccommodateToCamera();
                UpdateLogic();

                accumulator -= deltaTime;
            }
        }

        public void Draw()
        {
            if (Done)
                return;

            if (!setup)
            {
                AccommodateToCamera();
                SetupTransition();
                setup = true;
            }

            DrawTransition();

            if (lastDraw)
                Done = true;
        }
    }
}
