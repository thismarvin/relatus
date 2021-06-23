using Microsoft.Xna.Framework;

namespace Relatus.Graphics.Bundled
{
    public class Fade : Transition
    {
        private float alpha;
        private Color defaultColor;
        private Color fadeColor;
        private readonly Quad fade;

        public Fade(TransitionType type) : this(type, 0.01f, 0.01f, Color.Black)
        {

        }

        public Fade(TransitionType type, float velocity, float acceleration, Color color) : base(type, velocity, acceleration)
        {
            defaultColor = color;
            fade = new Quad(0, 0, 1, 1) { Tint = Color.Black };
        }

        protected override void SetupTransition()
        {
            alpha = Type == TransitionType.Enter ? 1 : 0;
            fadeColor = defaultColor * alpha;

            //fade.Width = Camera.Bounds.Width * 1.4f;
            //fade.Height = Camera.Bounds.Height * 1.4f;
            fade.Tint = fadeColor;
            fade.SetCenter(Camera.Position);
        }

        protected override void UpdateLogic()
        {
            switch (Type)
            {
                case TransitionType.Exit:
                    alpha += Force;
                    if (alpha > 1)
                    {
                        alpha = 1;
                        FlagCompletion();
                    }
                    break;

                case TransitionType.Enter:
                    alpha -= Force;
                    if (alpha < 0)
                    {
                        alpha = 0;
                        FlagCompletion();
                    }
                    break;
            }
        }

        protected override void AfterUpdate()
        {
            fadeColor = defaultColor * alpha;

            //fade.Width = Camera.Bounds.Width * 1.4f;
            //fade.Height = Camera.Bounds.Height * 1.4f;
            fade.Tint = fadeColor;
            fade.SetCenter(Camera.Position);
        }

        protected override void DrawTransition()
        {
            Sketch.GeometryBatcher
                .AttachCamera(Camera)
                .Begin()
                    .Add(fade)
                .End();
        }

        protected override void OnDispose()
        {
            fade.Dispose();
        }
    }
}
