using Microsoft.Xna.Framework;

namespace Relatus.Graphics
{
    public class Renderable
    {
        public RenderOptions RenderOptions
        {
            get => renderOptions;
            set => SetRenderOptions(value);
        }
        public Color Tint
        {
            get => tint;
            set => SetTint(value);
        }

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

        protected Transform transform;
        protected RenderOptions renderOptions;
        protected Color tint;

        public Renderable()
        {
            transform = new Transform();
            renderOptions = new RenderOptions();
            tint = Color.White;
        }

        public virtual Renderable SetRenderOptions(RenderOptions renderOptions)
        {
            this.renderOptions = renderOptions;

            return this;
        }

        public virtual Renderable SetTint(Color tint)
        {
            this.tint = tint;

            return this;
        }

        public Renderable SetTint(int r, int g, int b, float a = 1)
        {
            return SetTint(new Color(r, g, b) * a);
        }

        public virtual Renderable SetPosition(Vector3 position)
        {
            transform.Translation = position;

            return this;
        }

        public Renderable SetPosition(float x, float y, float z)
        {
            return SetPosition(new Vector3(x, y, z));
        }

        public virtual Renderable SetOrigin(Vector3 origin)
        {
            transform.Origin = origin;

            return this;
        }

        public Renderable SetOrigin(float x, float y, float z)
        {
            return SetOrigin(new Vector3(x, y, z));
        }

        public virtual Renderable SetRotation(Quaternion rotation)
        {
            transform.Rotation = rotation;

            return this;
        }
    }
}
