using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Graphics
{
    public class BetterSprite
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        private readonly Texture2D texture;
        private int xFrame;
        private int yFrame;

        private DynamicVertexBuffer modelBuffer;
        private VertexBufferBinding[] vertexBufferBindings;
        VertexBuffer test;

        private static readonly GraphicsDevice graphicsDevice;
        private static readonly GeometryData geometry;
        private static readonly Effect spriteShader;
        static BetterSprite()
        {
            graphicsDevice = Engine.Graphics.GraphicsDevice;
            geometry = GeometryManager.GetShapeData(ShapeType.Square);
            spriteShader = AssetManager.GetEffect("Relatus_SpriteShader");            
        }

        public BetterSprite(Texture2D texture)
        {
            this.texture = texture;
            Width = texture.Width;
            Height = texture.Height;
            ApplyChanges();                        
        }

        public BetterSprite ApplyChanges()
        {
            test = new VertexBuffer(graphicsDevice, typeof(VertexPositionColorTexture), 4, BufferUsage.WriteOnly);
            test.SetData(new VertexPositionColorTexture[] {
                new VertexPositionColorTexture(new Vector3(0, 0, 0), Color.White, new Vector2(0, 0)),
                new VertexPositionColorTexture(new Vector3(0, -1, 0), Color.White, new Vector2(0, 1)),
                new VertexPositionColorTexture(new Vector3(1, -1, 0), Color.White, new Vector2(1, 1)),
                new VertexPositionColorTexture(new Vector3(1, 0, 0), Color.White, new Vector2(1, 0)),
            });

            modelBuffer?.Dispose();
            modelBuffer = new DynamicVertexBuffer(graphicsDevice, typeof(VertexTransform), 1, BufferUsage.WriteOnly);
            modelBuffer.SetData(new VertexTransform[] { GetVertexTransformColor() });

            vertexBufferBindings = new VertexBufferBinding[]
            {
                new VertexBufferBinding(test),
                new VertexBufferBinding(modelBuffer, 0, 1)
            };
            
            return this;
        }

        internal VertexTransform GetVertexTransformColor()
        {
            Vector3 scale = new Vector3(Width, Height, 1);
            Vector3 translation = new Vector3(X, Y, 0);

            return new VertexTransform(scale, Vector2.Zero, 0, translation);
        }

        public void Draw(Camera camera)
        {            
            graphicsDevice.RasterizerState = GraphicsManager.RasterizerState;
            graphicsDevice.SetVertexBuffers(vertexBufferBindings);
            graphicsDevice.Indices = geometry.IndexBuffer;
            graphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            graphicsDevice.Textures[0] = texture;

            spriteShader.Parameters["SpriteTexture"].SetValue(texture);
            spriteShader.Parameters["WorldViewProjection"].SetValue(camera.WVP);

            foreach (EffectPass pass in spriteShader.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, geometry.TotalTriangles, 1);
            }

        }
    }
}
