using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Relatus.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.ECS.Bundled
{
    public class SSprite : DrawSystem
    {
        private IComponent[] sprites;
        private IComponent[] positions;
        private IComponent[] transforms;

        //private static readonly GraphicsDevice graphicsDevice;
        //private static readonly Effect spriteShader;
        //private static readonly GeometryData geometry;

        static SSprite()
        {
            //graphicsDevice = Engine.Graphics.GraphicsDevice;
            //spriteShader = AssetManager.GetEffect("Relatus_SpriteShader");
            //geometry = GeometryManager.GetShapeData(ShapeType.Square);
        }

        public SSprite(MorroFactory factory) : base(factory)
        {
            Require(typeof(CSprite), typeof(CPosition), typeof(CTransform));
        }

        //public override void EnableFixedUpdate(uint updatesPerSecond)
        //{
        //    throw new RelatusException("SSprite was not designed to run using a fixed update.", new NotSupportedException());
        //}

        public override void DrawEntity(uint entity, Camera camera)
        {
            throw new NotImplementedException();
        }

        public override void Draw(Camera camera)
        {
            sprites = sprites ?? factory.GetData<CSprite>();
            positions = positions ?? factory.GetData<CPosition>();
            transforms = transforms ?? factory.GetData<CTransform>();

            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.SpriteTransform);
            //{
            //    CSprite sprite;
            //    CPosition position;
            //    CTransform transform;

            //    foreach (int entity in Entities)
            //    {
            //        sprite = (CSprite)sprites[entity];
            //        position = (CPosition)positions[entity];
            //        transform = (CTransform)transforms[entity];

            //        spriteBatch.Draw(sprite.Texture, new Vector2(position.X, position.Y), sprite.SampleRegion, Color.White, transform.Rotation, transform.RotationOffset, new Vector2(transform.Scale.X, transform.Scale.Y), sprite.SpriteEffect, 0);
            //    }
            //}
            //spriteBatch.End();
        }
    }
}
