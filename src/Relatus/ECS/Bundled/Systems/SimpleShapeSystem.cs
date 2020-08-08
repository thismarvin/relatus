using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Relatus.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.ECS.Bundled
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
        protected RenderOptions renderOptions;

        protected VertexBetterTransform[] transformData;
        protected VertexColor[] colorData;

        protected DynamicVertexBuffer transformBuffer;
        protected DynamicVertexBuffer colorBuffer;
        protected VertexBufferBinding[] vertexBufferBindings;

        protected static readonly GraphicsDevice graphicsDevice;
        protected static readonly Effect polygonShader;
        protected static readonly EffectPass polygonPass;

        static SimpleShapeSystem()
        {
            graphicsDevice = Engine.Graphics.GraphicsDevice;
            polygonShader = AssetManager.GetEffect("Relatus_RelatusEffect");
            polygonPass = polygonShader.Techniques[1].Passes[0];
        }

        /// <summary>
        /// Create a <see cref="HybridSystem"/> that helps process, manipulate, and draw <see cref="GeometryData"/>.
        /// By default, this system will handle any entity that includes the following components: <see cref="CPosition"/>, <see cref="CDimension"/>, <see cref="CTransform"/>, <see cref="CColor"/>,
        /// and a custom <see cref="IComponent"/> that acts as a tag specifically for this system.
        /// </summary>
        /// <param name="factory">The scene this system will exist in.</param>
        /// <param name="geometry">The shape data that this system will focus on and draw.</param>
        /// <param name="renderOptions">The render options that should be used to draw the geometry.</param>
        /// <param name="shapeTag">The type of a custom <see cref="IComponent"/> that acts as a tag specifically for this system.</param>
        public SimpleShapeSystem(MorroFactory factory, GeometryData geometry, RenderOptions renderOptions, Type shapeTag) : base(factory)
        {
            Require(typeof(CPosition), typeof(CDimension), typeof(CTransform), typeof(CColor), shapeTag);

            this.geometry = geometry;
            this.renderOptions = renderOptions;

            transformData = new VertexBetterTransform[factory.EntityCapacity];
            colorData = new VertexColor[factory.EntityCapacity];
        }

        public override void EnableFixedUpdate(uint updatesPerSecond)
        {
            throw new RelatusException("SimpleShapeSystem was not designed to run using a fixed update.", new NotSupportedException());
        }

        public override void UpdateEntity(int entity)
        {
            CPosition position = (CPosition)positions[entity];
            CDimension dimension = (CDimension)dimensions[entity];
            CTransform transform = (CTransform)transforms[entity];
            CColor color = (CColor)colors[entity];

            transformData[entity] = CreateVertexTransform(position, dimension, transform);
            colorData[entity] = new VertexColor(color.Color);
        }

        public override void DrawEntity(int entity, Camera camera)
        {
            throw new NotImplementedException();
        }

        private VertexBetterTransform CreateVertexTransform(CPosition position, CDimension dimension, CTransform transform)
        {
            Vector3 translation = new Vector3(position.X + transform.Translation.X, position.Y + transform.Translation.Y, position.Z + transform.Translation.Z);
            Vector3 scale = new Vector3(dimension.Width * transform.Scale.X, dimension.Height * transform.Scale.Y, transform.Scale.Z);

            return new VertexBetterTransform(translation, scale, transform.Origin, transform.Rotation);
        }

        private void CreateVertexBufferBindings()
        {
            if (Entities.Count <= 0)
                return;

            transformBuffer?.Dispose();
            transformBuffer = new DynamicVertexBuffer(graphicsDevice, typeof(VertexBetterTransform), transformData.Length, BufferUsage.WriteOnly);
            transformBuffer.SetData(transformData);

            colorBuffer?.Dispose();
            colorBuffer = new DynamicVertexBuffer(graphicsDevice, typeof(VertexColor), colorData.Length, BufferUsage.WriteOnly);
            colorBuffer.SetData(colorData);

            vertexBufferBindings = new VertexBufferBinding[]
            {
                new VertexBufferBinding(geometry.VertexBuffer),
                new VertexBufferBinding(transformBuffer, 0, 1),
                new VertexBufferBinding(colorBuffer, 0, 1)
            };
        }

        public override void Update()
        {
            positions = positions ?? factory.GetData<CPosition>();
            dimensions = dimensions ?? factory.GetData<CDimension>();
            transforms = transforms ?? factory.GetData<CTransform>();
            colors = colors ?? factory.GetData<CColor>();

            Array.Clear(transformData, 0, transformData.Length);
            Array.Clear(colorData, 0, colorData.Length);

            base.Update();
        }

        public override void Draw(Camera camera)
        {
            if (Entities.Count <= 0)
                return;

            CreateVertexBufferBindings();

            graphicsDevice.RasterizerState = GraphicsManager.RasterizerState;
            graphicsDevice.SamplerStates[0] = renderOptions.SamplerState;
            graphicsDevice.BlendState = renderOptions.BlendState;
            graphicsDevice.DepthStencilState = renderOptions.DepthStencilState;
            graphicsDevice.SetVertexBuffers(vertexBufferBindings);
            graphicsDevice.Indices = geometry.IndexBuffer;

            polygonShader.Parameters["WVP"].SetValue(camera.WVP);

            polygonPass.Apply();

            if (renderOptions.Effect == null)
            {
                graphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, geometry.TotalTriangles, factory.EntityCapacity);
            }
            else
            {
                foreach (EffectPass pass in renderOptions.Effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    graphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, geometry.TotalTriangles, factory.EntityCapacity);
                }
            }
        }
    }
}
