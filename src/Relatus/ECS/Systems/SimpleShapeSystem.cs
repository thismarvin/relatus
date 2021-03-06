using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Relatus.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.ECS
{
    /// <summary>
    /// A type of <see cref="HybridSystem"/> that helps process, manipulate, and draw <see cref="GeometryData"/>.
    /// </summary>
    public abstract class SimpleShapeSystem : HybridSystem
    {
        protected IComponent[] positions;
        protected IComponent[] dimensions;
        protected IComponent[] transforms;
        protected IComponent[] colors;

        protected GeometryData geometry;
        protected VertexTransformColor[] vertexBuffer;
        protected DynamicVertexBuffer transformsBuffer;
        protected VertexBufferBinding[] vertexBufferBindings;

        protected static readonly GraphicsDevice graphicsDevice;
        static SimpleShapeSystem()
        {
            graphicsDevice = Engine.Graphics.GraphicsDevice;
        }

        /// <summary>
        /// Create a <see cref="HybridSystem"/> that helps process, manipulate, and draw <see cref="GeometryData"/>.
        /// By default, this system will handle any entity that includes the following components: <see cref="CPosition"/>, <see cref="CDimension"/>, <see cref="CTransform"/>, <see cref="CColor"/>,
        /// and a custom <see cref="IComponent"/> that acts as a tag specifically for this system.
        /// </summary>
        /// <param name="scene">The scene this system will exist in.</param>
        /// <param name="geometry">The shape data that this system will focus on and draw.</param>
        /// <param name="shapeTag">The type of the custom <see cref="IComponent"/> that acts as a tag specifically for this system.</param>
        /// <param name="tasks">The total amount of tasks to divide the update cycle into. Assigning more than one task allows entities to be updated asynchronously.</param>
        public SimpleShapeSystem(Scene scene, GeometryData geometry, Type shapeTag, uint tasks) : base(scene, tasks)
        {
            Require(typeof(CPosition), typeof(CDimension), typeof(CTransform), typeof(CColor), shapeTag);

            this.geometry = geometry;
            vertexBuffer = new VertexTransformColor[scene.EntityCapacity];
        }

        public override void UpdateEntity(int entity)
        {
            CPosition position = (CPosition)positions[entity];
            CDimension dimension = (CDimension)dimensions[entity];
            CTransform transform = (CTransform)transforms[entity];
            CColor color = (CColor)colors[entity];

            vertexBuffer[entity] = CreateVertexTransformColor(position, dimension, transform, color);
        }

        public override void DrawEntity(int entity, Camera camera)
        {
            throw new NotImplementedException();
        }

        private VertexTransformColor CreateVertexTransformColor(CPosition position, CDimension dimension, CTransform transform, CColor color)
        {
            Vector3 scale = new Vector3(dimension.Width * transform.Scale.X, dimension.Height * transform.Scale.Y, transform.Scale.Z);
            Vector2 rotationOffset = new Vector2(transform.RotationOffset.X, transform.RotationOffset.Y);
            Vector3 translation = new Vector3(position.X + transform.Translation.X, position.Y + transform.Translation.Y, position.Z + transform.Translation.Z);

            return new VertexTransformColor(scale, rotationOffset, transform.Rotation, translation, color.Color);
        }

        private void CreateVertexBufferBindings()
        {
            if (Entities.Count <= 0)
                return;

            transformsBuffer?.Dispose();
            transformsBuffer = new DynamicVertexBuffer(graphicsDevice, typeof(VertexTransformColor), vertexBuffer.Length, BufferUsage.WriteOnly);
            transformsBuffer.SetData(vertexBuffer);

            vertexBufferBindings = new VertexBufferBinding[]
            {
                new VertexBufferBinding(geometry.VertexBuffer),
                new VertexBufferBinding(transformsBuffer, 0, 1)
            };
        }

        public override void Update()
        {
            positions = scene.GetData<CPosition>();
            dimensions = scene.GetData<CDimension>();
            transforms = scene.GetData<CTransform>();
            colors = scene.GetData<CColor>();

            Array.Clear(vertexBuffer, 0, vertexBuffer.Length);

            base.Update();
        }

        public override void Draw(Camera camera)
        {
            if (Entities.Count <= 0)
                return;

            CreateVertexBufferBindings();

            graphicsDevice.RasterizerState = GraphicsManager.RasterizerState;
            graphicsDevice.SetVertexBuffers(vertexBufferBindings);
            graphicsDevice.Indices = geometry.IndexBuffer;

            GeometryManager.SetupPolygonShader(camera);

            foreach (EffectPass pass in GeometryManager.PolygonShader.Techniques[1].Passes)
            {
                pass.Apply();
                graphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, geometry.TotalTriangles, vertexBuffer.Length);
            }
        }
    }
}
