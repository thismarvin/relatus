using Microsoft.Xna.Framework;
using Mimoso.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mimoso.Graphics.Transitions
{
    class Pinhole : Transition
    {
        const int PADDING = 64;

        private readonly Circle pinhole;
        private int radius;

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
            lineWidth = Type == TransitionType.Enter ? radius : 1;
            //pinhole.ShapeData = Geometry.CreateHollowCircle(pinhole.Radius, lineWidth);
            pinhole.LineWidth = lineWidth;
            
        }

        protected override void AccommodateToCamera()
        {
            radius = (int)Camera.Bounds.Width > (int)Camera.Bounds.Height ? (int)Camera.Bounds.Width / 2 : (int)Camera.Bounds.Height / 2;
            radius += PADDING;
            pinhole.Radius = radius;
            pinhole.SetCenter(Camera.Center.X, Camera.Center.Y);
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

            //pinhole.ShapeData = Geometry.CreateHollowCircle(pinhole.Radius, lineWidth);
            pinhole.LineWidth = lineWidth;
        }

        protected override void DrawTransition()
        {
            pinhole.Draw(CameraManager.GetCamera(CameraType.Static));
        }
    }
}
