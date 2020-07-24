using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Relatus.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.ECS
{
    public class SSprite : DrawSystem
    {
        private IComponent[] sprites;
        private IComponent[] positions;
        private IComponent[] transforms;

        private VertexColorTexture[] vertexData;
        private VertexBufferBinding[] vertexBufferBindings;
        private DynamicVertexBuffer transformsBuffer;

        private static readonly SpriteBatch spriteBatch;
        private static readonly GraphicsDevice graphicsDevice;
        private static readonly Effect spriteShader;
        private static readonly GeometryData geometry;

        static SSprite()
        {
            spriteBatch = GraphicsManager.SpriteBatch;
            graphicsDevice = Engine.Graphics.GraphicsDevice;
            spriteShader = AssetManager.GetEffect("Relatus_SpriteShader");
            geometry = GeometryManager.GetShapeData(ShapeType.Square);
        }

        public SSprite(Scene scene) : base(scene)
        {
            Require(typeof(CSprite), typeof(CPosition), typeof(CTransform));
        }

        public override void DrawEntity(int entity, Camera camera)
        {
            throw new NotImplementedException();
        }

        public override void Draw(Camera camera)
        {
            sprites = scene.GetData<CSprite>();
            positions = scene.GetData<CPosition>();
            transforms = scene.GetData<CTransform>();

            graphicsDevice.RasterizerState = GraphicsManager.RasterizerState;
            graphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            graphicsDevice.Indices = geometry.IndexBuffer;

            spriteShader.Parameters["WorldViewProjection"].SetValue(camera.WVP);

            foreach (int entity in Entities)
            {
                CSprite sprite = (CSprite)sprites[entity];
                graphicsDevice.SetVertexBuffers(vertexBufferBindings);
                graphicsDevice.Textures[0] = sprite.Texture;
                spriteShader.Parameters["SpriteTexture"].SetValue(sprite.Texture);

                foreach (EffectPass pass in spriteShader.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    graphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, geometry.TotalTriangles, 1);
                }
            }


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
