using Microsoft.Xna.Framework;
using Mimoso.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mimoso.Graphics.Transitions
{
    class Fade : Transition
    {
        const int PADDING = 8;

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
            fade = new Quad(-PADDING, -PADDING, 1, 1) { Color = Color.Black };
        }

        protected override void AccommodateToCamera()
        {
            fade.Width = Camera.Bounds.Width + PADDING * 2;
            fade.Height = Camera.Bounds.Height + PADDING * 2;

            if (WindowManager.WideScreenSupported)
            {
                fade.X = -WindowManager.PillarBox - PADDING;
                fade.Y = -WindowManager.LetterBox - PADDING;
            }
            else
            {
                fade.X = -PADDING;
                fade.Y = -PADDING;
            }
        }

        protected override void SetupTransition()
        {
            alpha = Type == TransitionType.Enter ? 1 : 0;
            fadeColor = defaultColor * alpha;
            fade.Color = fadeColor;
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

            fadeColor = defaultColor * alpha;
            fade.Color = fadeColor;
        }

        protected override void DrawTransition()
        {
            fade.Draw(CameraManager.GetCamera(CameraType.Static));
        }
    }
}
