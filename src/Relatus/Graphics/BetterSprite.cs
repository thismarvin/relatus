using Microsoft.Xna.Framework;

namespace Relatus.Graphics
{
    public class BetterSprite
    {
        #region Base
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
        #endregion

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
        public Color Tint
        {
            get => tint;
            set => SetTint(value);
        }
        public Vector2 Scale
        {
            get => scale;
            set => SetScale(value);
        }

        public float Width
        {
            get => ImageRegion.Width * Scale.X;
        }
        public float Height
        {
            get => ImageRegion.Width * Scale.Y;
        }
        public (float, float) Dimensions
        {
            get => (ImageRegion.Width * Transform.Scale.X, ImageRegion.Height * Transform.Scale.Y);
        }

        private ImageRegion imageRegion;
        private SpriteMirroringType spriteMirroring;
        private Color tint;
        private Vector2 scale;

        public BetterSprite()
        {
            Transform = new Transform();
            imageRegion = new ImageRegion(0, 0, 1, 1);
            tint = Color.White;
            scale = Vector2.One;
        }

        public static BetterSprite Create()
        {
            return new BetterSprite();
        }

        #region Sprite Specific Data
        public virtual BetterSprite SetImageRegion(ImageRegion imageRegion)
        {
            this.imageRegion = imageRegion;

            Transform.Scale = new Vector3(Width, Height, 0);

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

        public virtual BetterSprite SetTint(Color tint)
        {
            this.tint = tint;

            return this;
        }

        public BetterSprite SetTint(int r, int g, int b, float a = 1)
        {
            return SetTint(new Color(r, g, b) * a);
        }

        public virtual BetterSprite SetScale(Vector2 scale)
        {
            this.scale = scale;

            Transform.Scale = new Vector3(Width, Height, 0);

            return this;
        }

        public BetterSprite SetScale(float x, float y)
        {
            return SetScale(new Vector2(x, y));
        }
        #endregion

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
    }
}
