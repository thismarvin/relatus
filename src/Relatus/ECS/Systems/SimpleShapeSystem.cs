using Microsoft.Xna.Framework.Graphics;
using Relatus.Core;
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

        /// <summary>
        /// Create a <see cref="HybridSystem"/> that helps process, manipulate, and draw <see cref="GeometryData"/>.
        /// By default, this system will handle any entity that includes the following components: <see cref="CPosition"/>, <see cref="CDimension"/>, <see cref="CTransform"/>, <see cref="CColor"/>,
        /// and a custom <see cref="IComponent"/> that acts as a tag specifically for this system.
        /// </summary>
        /// <param name="scene">The scene this system will exist in.</param>
        /// <param name="geometry">The shape data that this system will focus on and draw.</param>
        /// <param name="shapeTag">The type of the custom <see cref="IComponent"/> that acts as a tag specifically for this system.</param>
        /// <param name="tasks">The total amount of tasks to divide the update cycle into. Assigning more than one task allows entities to be updated asynchronously.</param>
        internal SimpleShapeSystem(Scene scene, GeometryData geometry, Type shapeTag, uint tasks) : base(scene, tasks)
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

            vertexBuffer[entity] = new VertexTransformColor(position, dimension, transform, color);
        }

        public override void DrawEntity(int entity, Camera camera)
        {
            throw new NotImplementedException();
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

            using (DynamicVertexBuffer transformsBuffer = new DynamicVertexBuffer(Engine.Graphics.GraphicsDevice, typeof(VertexTransformColor), vertexBuffer.Length, BufferUsage.WriteOnly))
            {
                transformsBuffer.SetData(vertexBuffer);

                Engine.Graphics.GraphicsDevice.RasterizerState = GraphicsManager.RasterizerState;
                Engine.Graphics.GraphicsDevice.SetVertexBuffers(new VertexBufferBinding(geometry.Geometry), new VertexBufferBinding(transformsBuffer, 0, 1));
                Engine.Graphics.GraphicsDevice.Indices = geometry.Indices;

                GeometryManager.SetupPolygonShader(camera);

                foreach (EffectPass pass in GeometryManager.PolygonShader.Techniques[1].Passes)
                {
                    pass.Apply();
                    Engine.Graphics.GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, geometry.TotalTriangles, vertexBuffer.Length);
                }
            }
        }
    }
}
