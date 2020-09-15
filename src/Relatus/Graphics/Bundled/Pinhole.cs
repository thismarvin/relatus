using Microsoft.Xna.Framework;

namespace Relatus.Graphics.Bundled
{
    public class Pinhole : Transition
    {
        private readonly Circle pinhole;
        private float radius;
        private float lineWidth;

        public Pinhole(TransitionType type) : this(type, 0, 4)
        {

        }

        public Pinhole(TransitionType type, float speed, float acceleration) : base(type, speed, acceleration)
        {
            pinhole = new Circle(0, 0, 1) { Color = Color.Black };
        }

        protected override void SetupTransition()
        {
            radius = Camera.Bounds.Width > Camera.Bounds.Height ? Camera.Bounds.Width * 0.5f : Camera.Bounds.Height * 0.5f;
            radius *= 1.2f;
            lineWidth = Type == TransitionType.Enter ? radius : 1;

            pinhole.Radius = radius;
            pinhole.LineWidth = lineWidth;
            pinhole.SetCenter(Camera.Position.X, Camera.Position.Y);
        }

        protected override void UpdateLogic()
        {
            switch (Type)
            {
                case TransitionType.Enter:
                    lineWidth -= Force;
                    if (lineWidth <= 1)
                    {
                        lineWidth = 1;
                        FlagCompletion();
                    }
                    break;

                case TransitionType.Exit:
                    lineWidth += Force;
                    if (lineWidth >= radius)
                    {
                        lineWidth = radius;
                        FlagCompletion();
                    }
                    break;
            }
        }

        protected override void AfterUpdate()
        {
            pinhole.Radius = radius;
            pinhole.LineWidth = lineWidth;
            pinhole.SetCenter(Camera.Position.X, Camera.Position.Y);
        }

        protected override void DrawTransition()
        {
            Sketch.GeometryBatcher
                .AttachCamera(Camera)
                .Begin()
                    .Add(pinhole)
                .End();
        }

        protected override void OnDispose()
        {
            pinhole.Dispose();
        }
    }
}
