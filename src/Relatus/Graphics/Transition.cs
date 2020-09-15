using System;

namespace Relatus.Graphics
{
    public enum TransitionType
    {
        Enter,
        Exit
    }

    public abstract class Transition : IDisposable
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
            Camera = CameraManager.Static;

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
            if (Started)
                return;

            if (!setup)
            {
                SetupTransition();
                setup = true;
            }

            Started = true;
        }

        protected void FlagCompletion()
        {
            lastDraw = true;
        }

        protected virtual void AfterUpdate()
        {

        }

        protected abstract void SetupTransition();

        protected abstract void UpdateLogic();

        protected abstract void DrawTransition();

        public void Update()
        {
            if (Done)
                return;

            accumulator += Engine.DeltaTime;

            while (accumulator >= deltaTime)
            {
                velocity += acceleration * deltaTime;
                Force += velocity * deltaTime;

                UpdateLogic();

                accumulator -= deltaTime;
            }

            AfterUpdate();
        }

        public void Draw()
        {
            if (Done)
                return;

            DrawTransition();

            if (lastDraw)
                Done = true;
        }

        #region IDisposable Support
        protected bool disposedValue;

        protected virtual void OnDispose()
        {

        }

        public void Dispose()
        {
            if (!disposedValue)
            {
                OnDispose();

                disposedValue = true;
            }
        }
        #endregion
    }
}
