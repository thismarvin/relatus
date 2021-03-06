using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Graphics.Transitions
{
    public class Fade : Transition
    {
        const int PADDING = 32;

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
            fade = new Quad(0, 0, 1, 1) { Color = Color.Black };
            fade.ApplyChanges();
        }

        protected override void AccommodateToCamera()
        {
            fade.Width = Camera.Bounds.Width + PADDING * 2;
            fade.Height = Camera.Bounds.Height + PADDING * 2;

            fade.SetCenter(Camera.Position.X, Camera.Position.Y);
            fade.ApplyChanges();
        }

        protected override void SetupTransition()
        {
            alpha = Type == TransitionType.Enter ? 1 : 0;
            fadeColor = defaultColor * alpha;

            fade.Color = fadeColor;
            fade.ApplyChanges();
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
            fade.Color = fadeColor;
            fade.ApplyChanges();
        }

        protected override void DrawTransition()
        {
            fade.Draw(CameraManager.Get(CameraType.Static));
        }

        protected override void OnDispose()
        {
            fade.Dispose();
        }
    }
}
