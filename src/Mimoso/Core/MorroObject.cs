using Microsoft.Xna.Framework;
using Mimoso.Maths;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Mimoso.Core
{
    abstract class MorroObject : IComparable<MorroObject>
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public int Depth { get; set; }

        public Vector2 Position { get => new Vector2(X, Y); }
        public Vector2 Center { get => new Vector2(X + Width / 2, X + Height / 2); }
        public RectangleF Bounds { get => new RectangleF(X, Y, Width, Height); }

        public MorroObject(float x, float y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Depth = 1;
        }

        public virtual void SetPosition(float x, float y)
        {
            X = x;
            Y = y;
        }

        public virtual void SetBounds(float x, float y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public int CompareTo(MorroObject morroObject)
        {
            return Depth.CompareTo(morroObject.Depth);
        }

        public override string ToString()
        {
            return base.ToString() + $": Position:(X:{X}, Y:{Y}), Dimensions:(W:{Width}, H:{Height}), Depth:{Depth}";
        }
    }
}
