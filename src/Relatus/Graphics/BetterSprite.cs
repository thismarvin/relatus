using Microsoft.Xna.Framework;

namespace Relatus.Graphics
{
    public class BetterSprite
    {
        public Transform Transform { get; }

        public Vector3 Position
        {
            get => Transform.Translation;
            set => SetPosition(value);
        }
        public float X
        {
            get => Transform.Translation.X;
            set => SetPosition(value, Transform.Translation.Y, Transform.Translation.Z);
        }
        public float Y
        {
            get => Transform.Translation.Y;
            set => SetPosition(Transform.Translation.X, value, Transform.Translation.Z);
        }
        public float Z
        {
            get => Transform.Translation.Z;
            set => SetPosition(Transform.Translation.X, Transform.Translation.Y, value);
        }
        public Vector3 Origin
        {
            get => Transform.Origin;
            set => SetOrigin(value);
        }
        public Quaternion Rotation
        {
            get => Transform.Rotation;
            set => SetRotation(value);
        }
        public Color Tint
        {
            get => tint;
            set => SetTint(value);
        }

        private Color tint;

        public float Width
        {
            get => Transform.Scale.X;
            set => SetDimensions(value, Transform.Scale.Y);
        }
        public float Height
        {
            get => Transform.Scale.Y;
            set => SetDimensions(Transform.Scale.X, value);
        }

        public ImageRegion ImageRegion
        {
            get => imageRegion;
            set => SetImageRegion(value.X, value.Y, value.Width, value.Height);
        }
        public SpriteMirroringType SpriteMirroring
        {
            get => spriteMirroring;
            set => SetSpriteMirroring(value);
        }

        private ImageRegion imageRegion;
        private SpriteMirroringType spriteMirroring;

        public BetterSprite()
        {
            Transform = new Transform();
            tint = Color.White;
        }

        public static BetterSprite Create()
        {
            return new BetterSprite();
        }

        public virtual BetterSprite SetTint(Color tint)
        {
            this.tint = tint;

            return this;
        }

        public BetterSprite SetTint(int r, int g, int b, float a = 1)
        {
            return SetTint(new Color(r, g, b) * a);
        }

        public virtual BetterSprite SetPosition(Vector3 position)
        {
            Transform.Translation = position;

            return this;
        }

        public BetterSprite SetPosition(float x, float y, float z)
        {
            return SetPosition(new Vector3(x, y, z));
        }

        public virtual BetterSprite SetOrigin(Vector3 origin)
        {
            Transform.Origin = origin;

            return this;
        }

        public BetterSprite SetOrigin(float x, float y, float z)
        {
            return SetOrigin(new Vector3(x, y, z));
        }

        public virtual BetterSprite SetRotation(Quaternion rotation)
        {
            Transform.Rotation = rotation;

            return this;
        }

        public virtual BetterSprite SetDimensions(float width, float height)
        {
            Transform.Scale = new Vector3(width, height, Transform.Scale.Z);

            return this;
        }

        public virtual BetterSprite SetImageRegion(ImageRegion imageRegion)
        {
            this.imageRegion = imageRegion;

            return this;
        }

        public BetterSprite SetImageRegion(int x, int y, int width, int height)
        {
            return SetImageRegion(new ImageRegion(x, y, width, height));
        }

        public virtual BetterSprite SetSpriteMirroring(SpriteMirroringType mirroringType)
        {
            spriteMirroring = mirroringType;

            return this;
        }
    }
}
