using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Relatus
{
    public abstract class RelatusObject : IComparable<RelatusObject>
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public int Depth { get; set; }

        public Vector2 Position
        {
            get => new Vector2(X, Y);
            set => SetPosition(value.X, value.Y);
        }

        public Vector2 Center
        {
            get => new Vector2(X + Width / 2, Y - Height / 2);
            set => SetCenter(value.X, value.Y);
        }

        public RectangleF Bounds
        {
            get => new RectangleF(X, Y, Width, Height);
            set => SetBounds(value.X, value.Y, value.Width, value.Height);
        }

        public RelatusObject(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Depth = 1;
        }

        public virtual RelatusObject SetPosition(float x, float y)
        {
            X = x;
            Y = y;

            return this;
        }

        public virtual RelatusObject SetCenter(float x, float y)
        {
            X = x - Width / 2;
            Y = y + Height / 2;

            return this;
        }

        public virtual RelatusObject SetBounds(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;

            return this;
        }

        public int CompareTo(RelatusObject relatusObject)
        {
            return Depth.CompareTo(relatusObject.Depth);
        }

        public override string ToString()
        {
            return base.ToString() + $": Position:(X:{X}, Y:{Y}), Dimensions:(W:{Width}, H:{Height}), Depth:{Depth}";
        }
    }
}
