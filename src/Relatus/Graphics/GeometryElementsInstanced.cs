using Microsoft.Xna.Framework.Graphics;

namespace Relatus.Graphics
{
    internal class GeometryElementsInstanced : GeometryGroup
    {
        private readonly GeometryData sharedGeometry;
        private readonly RenderOptions sharedRenderOptions;

        private readonly VertexTransform[] transforms;
        private readonly VertexColor[] colors;
        private bool dataModified;

        private DynamicVertexBuffer transformBuffer;
        private DynamicVertexBuffer colorBuffer;
        private VertexBufferBinding[] vertexBufferBindings;

        private uint count;

        private static readonly GraphicsDevice graphicsDevice;
        private static readonly Effect polygonShader;
        private static readonly EffectPass polygonPass;

        static GeometryElementsInstanced()
        {
            graphicsDevice = Engine.Graphics.GraphicsDevice;
            polygonShader = AssetManager.GetEffect("Relatus_RelatusEffect");
            polygonPass = polygonShader.Techniques[1].Passes[0];
        }

        public GeometryElementsInstanced(uint batchSize, GeometryData sharedGeometry, RenderOptions sharedRenderOptions) : base(BatchExecution.DrawElementsInstanced, batchSize)
        {
            this.sharedGeometry = sharedGeometry;
            this.sharedRenderOptions = sharedRenderOptions;

            transforms = new VertexTransform[batchSize];
            colors = new VertexColor[batchSize];
        }

        public override bool Add(Geometry geometry)
        {
            if (count >= BatchSize)
                return false;

            if (!geometry.GeometryData.Equals(sharedGeometry) || !geometry.RenderOptions.Equals(sharedRenderOptions))
                return false;

            VertexTransform transform = geometry.GetVertexTransform();
            VertexColor color = geometry.GetVertexColor();

            transforms[count] = transform;
            colors[count] = color;

            dataModified = true;

            count++;

            return true;
        }

        public override DrawGroup<Geometry> ApplyChanges()
        {
            if (!dataModified)
                return this;

            transformBuffer?.Dispose();
            transformBuffer = new DynamicVertexBuffer(graphicsDevice, typeof(VertexTransform), (int)count, BufferUsage.WriteOnly);
            transformBuffer.SetData(transforms, 0, (int)count);

            colorBuffer?.Dispose();
            colorBuffer = new DynamicVertexBuffer(graphicsDevice, typeof(VertexColor), (int)count, BufferUsage.WriteOnly);
            colorBuffer.SetData(colors, 0, (int)count);

            vertexBufferBindings = new VertexBufferBinding[]
            {
                new VertexBufferBinding(sharedGeometry.VertexPositionBuffer),
                new VertexBufferBinding(transformBuffer, 0, 1),
                new VertexBufferBinding(colorBuffer, 0, 1),
            };

            dataModified = false;

            return this;
        }

        public override DrawGroup<Geometry> Draw(Camera camera)
        {
            if (dataModified)
                throw new RelatusException("The polygon group was modified, but ApplyChanges() was never called.", new MethodExpectedException());

            graphicsDevice.RasterizerState = sharedRenderOptions.RasterizerState;
            graphicsDevice.BlendState = sharedRenderOptions.BlendState;
            graphicsDevice.DepthStencilState = sharedRenderOptions.DepthStencilState;
            graphicsDevice.SetVertexBuffers(vertexBufferBindings);
            graphicsDevice.Indices = sharedGeometry.IndexBuffer;

            polygonShader.Parameters["WVP"].SetValue(camera.WVP);

            polygonPass.Apply();

            if (sharedRenderOptions.Effect == null)
            {
                graphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, sharedGeometry.Mesh.TotalTriangles, (int)count);
            }
            else
            {
                foreach (EffectPass pass in sharedRenderOptions.Effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    graphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, sharedGeometry.Mesh.TotalTriangles, (int)count);
                }
            }

            return this;
        }
    }
}
