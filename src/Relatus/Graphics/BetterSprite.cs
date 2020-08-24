using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Relatus.Utilities;

namespace Relatus.Graphics
{
    public enum SpriteMirroringType
    {
        None = 0,
        /// <summary>
        /// Reverse the x-axis of the sprite.
        /// </summary>
        FlipHorizontally = 1,
        /// <summary>
        /// Reverse the y-axis of the sprite.
        /// </summary>
        FlipVertically = 2
    }

    public class BetterSprite
    {
        #region Properties
        public Texture2D Texture
        {
            get => texture;
            set => SetTexture(value);
        }
        public Vector3 Position
        {
            get => position;
            set => SetPosition(value);
        }
        public Vector3 Translation
        {
            get => translation;
            set => SetTranslation(value);
        }
        public Vector3 Scale
        {
            get => scale;
            set => SetScale(value);
        }
        public Vector3 Origin
        {
            get => origin;
            set => SetOrigin(value);
        }
        public Vector3 Rotation
        {
            get => rotation;
            set => SetRotation(value.X, value.Y, value.Z);
        }
        public Color Tint
        {
            get => tint;
            set => SetTint(value);
        }
        public ImageRegion SampleRegion
        {
            get => sampleRegion;
            set => SetSampleRegion(value.X, value.Y, value.Width, value.Height);
        }
        public SpriteMirroringType SpriteMirroring
        {
            get => spriteMirroring;
            set => SetSpriteMirroring(value);
        }
        public RenderOptions RenderOptions
        {
            get => renderOptions;
            set => SetRenderOptions(value);
        }
        #endregion

        public int Width => SampleRegion.Width;
        public int Height => SampleRegion.Height;

        public Vector3 Center
        {
            get => new Vector3(position.X + Width * 0.5f, position.Y - Height * 0.5f, position.Z);
            set => SetCenter(value.X, value.Y);
        }
        public RectangleF Bounds
        {
            get => new RectangleF(position.X, position.Y, Width, Height);
        }
        public float X
        {
            get => position.X;
            set => SetPosition(value, position.Y, position.Z);
        }
        public float Y
        {
            get => position.Y;
            set => SetPosition(position.X, value, position.Z);
        }
        public float Z
        {
            get => position.Z;
            set => SetPosition(position.X, position.Y, value);
        }

        private Texture2D texture;
        private float texelWidth;
        private float texelHeight;
        private Vector3 position;
        private Vector3 translation;
        private Vector3 scale;
        private Vector3 origin;
        private Vector3 rotation;
        private Color tint;
        private ImageRegion sampleRegion;
        private SpriteMirroringType spriteMirroring;
        private RenderOptions renderOptions;

        public BetterSprite()
        {
            tint = Color.White;
            scale = Vector3.One;
            renderOptions = new RenderOptions();
        }

        public static BetterSprite Create()
        {
            return new BetterSprite();
        }

        public static BetterSprite CreateFromAtlas(SpriteAtlas spriteAtlas, string name)
        {
            SpriteAtlasEntry entry = spriteAtlas.GetEntry(name);

            return
                new BetterSprite()
                    .SetTexture(spriteAtlas.GetPage(entry.Page))
                    .SetSampleRegion(entry.ImageRegion);
        }

        public virtual BetterSprite SetTexture(Texture2D texture)
        {
            this.texture = texture;

            texelWidth = 1f / texture.Width;
            texelHeight = 1f / texture.Height;

            if (sampleRegion.X == 0 && sampleRegion.Y == 0 && sampleRegion.Width == 0 && sampleRegion.Height == 0)
            {
                sampleRegion = new ImageRegion(0, 0, texture.Width, texture.Height);
            }

            return this;
        }


        public virtual BetterSprite SetPosition(Vector3 position)
        {
            this.position = position;

            return this;
        }

        public BetterSprite SetPosition(float x, float y, float z)
        {
            return SetPosition(new Vector3(x, y, z));
        }

        public virtual BetterSprite SetTranslation(Vector3 translation)
        {
            this.translation = translation;

            return this;
        }

        public BetterSprite SetTranslation(float x, float y, float z)
        {
            return SetTranslation(new Vector3(x, y, z));
        }

        public virtual BetterSprite SetScale(Vector3 scale)
        {
            this.scale = scale;

            return this;
        }

        public BetterSprite SetScale(float x, float y, float z)
        {
            return SetScale(new Vector3(x, y, z));
        }

        public virtual BetterSprite SetOrigin(Vector3 origin)
        {
            this.origin = origin;

            return this;
        }

        public BetterSprite SetOrigin(float x, float y, float z)
        {
            return SetOrigin(new Vector3(x, y, z));
        }

        public virtual BetterSprite SetRotation(Vector3 rotation)
        {
            this.rotation = rotation;

            return this;
        }

        public BetterSprite SetRotation(float roll, float pitch, float yaw)
        {
            return SetRotation(new Vector3(roll, pitch, yaw));
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

        public virtual BetterSprite SetSampleRegion(ImageRegion sampleRegion)
        {
            this.sampleRegion = sampleRegion;

            return this;
        }

        public BetterSprite SetSampleRegion(int x, int y, int width, int height)
        {
            return SetSampleRegion(new ImageRegion(x, y, width, height));
        }

        public virtual BetterSprite SetSpriteMirroring(SpriteMirroringType mirroringType)
        {
            spriteMirroring = mirroringType;

            return this;
        }

        public virtual BetterSprite SetRenderOptions(RenderOptions options)
        {
            renderOptions = options;

            return this;
        }

        public BetterSprite SetCenter(float x, float y)
        {
            position = new Vector3(x - Width * 0.5f, y + Height * 0.5f, position.Z);

            return this;
        }

        internal VertexTransform GetVertexTransform()
        {
            Vector3 scale = new Vector3(Width * this.scale.X, Height * this.scale.Y, this.scale.Z);

            return new VertexTransform(position + translation, scale, origin, rotation);
        }

        internal VertexColor GetVertexColor()
        {
            return new VertexColor(tint);
        }

        internal VertexTexture[] GetTextureCoords()
        {
            Vector2 topLeft = new Vector2((float)MathExt.RemapRange(sampleRegion.X, 0, texture.Width, 0, 1), (float)MathExt.RemapRange(sampleRegion.Y, 0, texture.Height, 0, 1));
            Vector2 topRight = topLeft + new Vector2(texelWidth * sampleRegion.Width, 0);
            Vector2 bottomRight = topLeft + new Vector2(texelWidth * sampleRegion.Width, texelHeight * sampleRegion.Height);
            Vector2 bottomLeft = topLeft + new Vector2(0, texelHeight * sampleRegion.Height);

            Vector2[] corners = new Vector2[] { topLeft, bottomLeft, bottomRight, topRight };

            if ((spriteMirroring & SpriteMirroringType.FlipHorizontally) != SpriteMirroringType.None)
            {
                Vector2 temp = corners[0];
                corners[0] = corners[3];
                corners[3] = temp;

                temp = corners[1];
                corners[1] = corners[2];
                corners[2] = temp;
            }

            if ((spriteMirroring & SpriteMirroringType.FlipVertically) != SpriteMirroringType.None)
            {
                Vector2 temp = corners[0];
                corners[0] = corners[1];
                corners[1] = temp;

                temp = corners[3];
                corners[3] = corners[2];
                corners[2] = temp;
            }

            return new VertexTexture[]
            {
                new VertexTexture(corners[0]),
                new VertexTexture(corners[1]),
                new VertexTexture(corners[2]),
                new VertexTexture(corners[3])
            };
        }
    }
}
