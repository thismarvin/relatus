using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Graphics
{
    public class Sprite : RelatusObject
    {
        public float Rotation { get; set; }
        public string SpriteDataName { get; private set; }
        public bool Visible { get; set; }
        public Color Tint { get; private set; }
        public SpriteEffects SpriteEffect { get; set; }
        public Vector2 RotationOffset { get; set; }
        public Vector2 Scale { get; set; }
        public Effect Effect { get; set; }
        public BlendState BlendState { get; set; }
        public SamplerState SamplerState { get; set; }
        public Texture2D SpriteSheet { get; private set; }

        private Rectangle sourceRectangle;
        private Rectangle scissorRectangle;
        private bool customScissorRectangle;
        private int originalFrameX;
        private int originalFrameY;
        private int frameX;
        private int frameY;

        private static readonly SpriteBatch spriteBatch;

        static Sprite()
        {
            spriteBatch = GraphicsManager.SpriteBatch;
        }

        public Sprite(float x, float y, int frame, int columns, string sprite) : this(x, y, sprite)
        {
            SetFrame(frame, columns);
        }

        public Sprite(float x, float y, string spriteDataName) : base(x, y, 1, 1)
        {
            Rotation = 0;
            Visible = true;
            SpriteDataName = spriteDataName;
            RotationOffset = Vector2.Zero;
            Scale = new Vector2(1, 1);
            BlendState = BlendState.AlphaBlend;
            SamplerState = SamplerState.PointClamp;
            Tint = Color.White;

            InitializeSprite();
        }

        private void InitializeSprite()
        {
            SpriteSetup(SpriteManager.GetSpriteData(SpriteDataName));
        }

        private void SpriteSetup(SpriteData spriteData)
        {
            SpriteSheet = AssetManager.GetImage(spriteData.SpriteSheet);
            frameX = spriteData.X;
            frameY = spriteData.Y;
            originalFrameX = frameX;
            originalFrameY = frameY;

            SetBounds(X, Y, spriteData.Width, spriteData.Height);

            sourceRectangle = new Rectangle(frameX, frameY, (int)Width, (int)Height);
        }

        public void IncrementFrameX(int pixels)
        {
            frameX += pixels;
            sourceRectangle = new Rectangle(frameX, frameY, (int)Width, (int)Height);
        }

        public void IncrementFrameY(int pixels)
        {
            frameY += pixels;
            sourceRectangle = new Rectangle(frameX, frameY, (int)Width, (int)Height);
        }

        public void SetFrame(int frame, int columns)
        {
            frameX = originalFrameX + frame % columns * (int)Width;
            frameY = originalFrameY + frame / columns * (int)Height;
            sourceRectangle = new Rectangle(frameX, frameY, (int)Width, (int)Height);
        }

        public void SetSprite(string spriteDataName)
        {
            if (SpriteDataName == spriteDataName)
                return;

            SpriteDataName = spriteDataName;
            InitializeSprite();
        }

        public void SetScissorRectangle(Microsoft.Xna.Framework.Rectangle scissorRectangle)
        {
            this.scissorRectangle = scissorRectangle;
            customScissorRectangle = true;
        }

        public virtual void Update()
        {

        }

        internal virtual void ManagedDraw()
        {
           spriteBatch.Draw(SpriteSheet, Position, sourceRectangle, Tint, Rotation, RotationOffset, Scale, SpriteEffect, 0);
        }

        public void Draw(CameraType cameraType)
        {
            Draw(CameraManager.Get(cameraType));
        }

        public virtual void Draw(Camera camera)
        {
            if (!Visible)
                return;

            if (customScissorRectangle)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState, SamplerState, null, GraphicsManager.ScissorRasterizerState, Effect, camera.Transform);
                {
                    spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle;
                    spriteBatch.Draw(SpriteSheet, Position, sourceRectangle, Tint, Rotation, RotationOffset, Scale, SpriteEffect, 0);
                }
                spriteBatch.End();
            }
            else
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState, SamplerState, null, null, Effect, camera.Transform);
                {
                    spriteBatch.Draw(SpriteSheet, Position, sourceRectangle, Tint, Rotation, RotationOffset, Scale, SpriteEffect, 0);
                }
                spriteBatch.End();
            }
        }
    }
}
