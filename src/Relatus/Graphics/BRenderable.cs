using Microsoft.Xna.Framework;

namespace Relatus.Graphics
{
    public class BRenderable
    {
        public Transform Transform => transform;

        public Vector3 Position
        {
            get => transform.Translation;
            set => SetPosition(value);
        }
        public float X
        {
            get => transform.Translation.X;
            set => SetPosition(value, transform.Translation.Y, transform.Translation.Z);
        }
        public float Y
        {
            get => transform.Translation.Y;
            set => SetPosition(transform.Translation.X, value, transform.Translation.Z);
        }
        public float Z
        {
            get => transform.Translation.Z;
            set => SetPosition(transform.Translation.X, transform.Translation.Y, value);
        }
        public Vector3 Origin
        {
            get => transform.Origin;
            set => SetOrigin(value);
        }
        public Quaternion Rotation
        {
            get => transform.Rotation;
            set => SetRotation(value);
        }
        public Color Tint
        {
            get => tint;
            set => SetTint(value);
        }

        protected Transform transform;
        protected Color tint;

        public BRenderable()
        {
            transform = new Transform();
            tint = Color.White;
        }

        public virtual BRenderable SetTint(Color tint)
        {
            this.tint = tint;

            return this;
        }

        public BRenderable SetTint(int r, int g, int b, float a = 1)
        {
            return SetTint(new Color(r, g, b) * a);
        }

        public virtual BRenderable SetPosition(Vector3 position)
        {
            transform.Translation = position;

            return this;
        }

        public BRenderable SetPosition(float x, float y, float z)
        {
            return SetPosition(new Vector3(x, y, z));
        }

        public virtual BRenderable SetOrigin(Vector3 origin)
        {
            transform.Origin = origin;

            return this;
        }

        public BRenderable SetOrigin(float x, float y, float z)
        {
            return SetOrigin(new Vector3(x, y, z));
        }

        public virtual BRenderable SetRotation(Quaternion rotation)
        {
            transform.Rotation = rotation;

            return this;
        }
    }
}
