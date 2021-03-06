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

    public abstract class Transition: IDisposable
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
            Camera = CameraManager.Get(CameraType.Static);

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

        protected virtual void AfterUpdate()
        {

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
            if (Done)
                return;

            AccommodateToCamera();

            if (!setup)
            {                
                SetupTransition();
                setup = true;
            }

            accumulator += Engine.DeltaTime;

            while (accumulator >= deltaTime)
            {
                CalculateForce();
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
        protected bool disposedValue = false; // To detect redundant calls

        protected virtual void OnDispose()
        {

        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    OnDispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Transition()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
