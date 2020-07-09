using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Mimoso.Maths
{
    struct RectangleF
    {
        public float X { get; private set; }
        public float Y { get; private set; }
        public float Width { get; private set; }
        public float Height { get; private set; }

        public float Top { get => Y; }
        public float Bottom { get => Y + Height; }
        public float Left { get => X; }
        public float Right { get => X + Width; }

        public RectangleF(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public bool Intersects(RectangleF rectangle)
        {
            return (Left < rectangle.Right && Right > rectangle.Left && Top < rectangle.Bottom && Bottom > rectangle.Top);
        }

        public bool CompletelyWithin(RectangleF rectangle)
        {
            return (Left >= rectangle.Left && Right <= rectangle.Right && Top >= rectangle.Top && Bottom <= rectangle.Bottom);
        }

        public Vector2 GetResolution(Rectangle rectangle)
        {
            Vector2[] aVertices = new Vector2[4]{
                new Vector2(Left, Top),
                new Vector2(Right, Top),
                new Vector2(Right, Bottom),
                new Vector2(Left, Bottom),
            };

            LineSegment[] aEdges = new LineSegment[2]{
                new LineSegment(
                    aVertices[0].X,
                    aVertices[0].Y,
                    aVertices[1].X,
                    aVertices[1].Y
                ),
                new LineSegment(
                    aVertices[1].X,
                    aVertices[1].Y,
                    aVertices[2].X,
                    aVertices[2].Y
                )
            };

            Vector2[] bVertices = new Vector2[4]{
                new Vector2(rectangle.Left, rectangle.Top),
                new Vector2(rectangle.Right, rectangle.Top),
                new Vector2(rectangle.Right, rectangle.Bottom),
                new Vector2(rectangle.Left, rectangle.Bottom)
            };

            LineSegment[] bEdges = new LineSegment[2]{
                new LineSegment(
                    bVertices[0].X,
                    bVertices[0].Y,
                    bVertices[1].X,
                    bVertices[1].Y
                ),
                new LineSegment(
                    bVertices[1].X,
                    bVertices[1].Y,
                    bVertices[2].X,
                    bVertices[2].Y
                )
            };

            return CollisionHelper.GetResolution(new ShapeSchema(aVertices, aEdges), new ShapeSchema(bVertices, bEdges));
        }

        public override string ToString()
        {
            return base.ToString() + " " + $": Position:(X:{X}, Y:{Y}), Dimensions:(W:{Width}, H:{Height})";
        }
    }
}
