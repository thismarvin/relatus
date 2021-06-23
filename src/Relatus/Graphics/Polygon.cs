using Microsoft.Xna.Framework;

namespace Relatus.Graphics
{
    public class Polygon : Geometry
    {
        public float Width
        {
            get => transform.Scale.X;
            set => SetDimensions(value, transform.Scale.Y);
        }
        public float Height
        {
            get => transform.Scale.Y;
            set => SetDimensions(transform.Scale.X, value);
        }

        public Vector3 Center
        {
            get => new Vector3(Position.X + Width * 0.5f, Position.Y - Height * 0.5f, Position.Z);
            set => SetCenter(value);
        }

        public RectangleF Bounds => new RectangleF(X, Y, Width, Height);

        public Polygon() : base()
        {
        }

        public static Polygon Create()
        {
            return new Polygon();
        }

        public virtual Renderable SetDimensions(float width, float height)
        {
            transform.Scale = new Vector3(width, height, 0);

            return this;
        }

        public virtual Renderable SetCenter(Vector3 center)
        {
            Position = new Vector3(center.X - Width * 0.5f, center.Y + Height * 0.5f, center.Z);

            return this;
        }

        public Renderable SetCenter(float x, float y, float z)
        {
            return SetCenter(new Vector3(x, y, z));
        }

        public PolygonSchema CalculatePolygonSchema()
        {
            int totalVertices = geometryData.Mesh.TotalVertices;
            Matrix polygonTransform = transform.Matrix;

            Vector2[] transformedVertices = new Vector2[totalVertices];

            for (int i = 0; i < totalVertices; i++)
            {
                transformedVertices[i] = Vector2.Transform(new Vector2(geometryData.Mesh.Vertices[i].X, geometryData.Mesh.Vertices[i].Y), polygonTransform);
            }

            LineSegment[] transformedLineSegments = new LineSegment[totalVertices];

            for (int i = 0; i < totalVertices - 1; i++)
            {
                transformedLineSegments[i] = new LineSegment(transformedVertices[i + 1].X, transformedVertices[i + 1].Y, transformedVertices[i].X, transformedVertices[i].Y);
            }

            transformedLineSegments[totalVertices - 1] = new LineSegment(transformedVertices[0].X, transformedVertices[0].Y, transformedVertices[totalVertices - 1].X, transformedVertices[totalVertices - 1].Y);

            return new PolygonSchema(transformedVertices, transformedLineSegments);
        }
    }
}
