using Microsoft.Xna.Framework.Graphics;
using System;

namespace Relatus.Graphics
{
    internal class PolygonElements : PolygonGroup
    {
        private readonly GeometryData sharedGeometry;
        private readonly RenderOptions sharedRenderOptions;

        private readonly uint maxBatchSize;

        private readonly VertexPosition[] vertexPositions;
        private readonly VertexTransform[] transforms;
        private readonly VertexColor[] colors;
        private bool dataModified;

        private readonly short[] indices;
        private readonly IndexBuffer indexBuffer;

        private DynamicVertexBuffer vertexPositionBuffer;
        private DynamicVertexBuffer transformBuffer;
        private DynamicVertexBuffer colorBuffer;
        private VertexBufferBinding[] vertexBufferBindings;

        private uint index;
        private uint count;
        private uint totalPrimitives;

        private int TotalVertices => sharedGeometry.TotalVertices;
        private int TotalIndices => sharedGeometry.Mesh.Indices.Length;
        private int TotalTriangles => sharedGeometry.TotalTriangles;

        private static readonly GraphicsDevice graphicsDevice;
        private static readonly Effect polygonShader;
        private static readonly EffectPass polygonPass;

        static PolygonElements()
        {
            graphicsDevice = Engine.Graphics.GraphicsDevice;
            polygonShader = AssetManager.GetEffect("Relatus_RelatusEffect");
            polygonPass = polygonShader.Techniques[1].Passes[0];
        }

        public PolygonElements(uint batchSize, GeometryData sharedGeometry, RenderOptions sharedRenderOptions) : base(BatchExecution.DrawElements, batchSize)
        {
            this.sharedGeometry = sharedGeometry;
            this.sharedRenderOptions = sharedRenderOptions;

            maxBatchSize = (uint)(short.MaxValue / TotalIndices);

            if (BatchSize > maxBatchSize)
                throw new RelatusException($"Given the shared geometry, PolygonElements does not support support a batch size greater than {maxBatchSize}.", new ArgumentOutOfRangeException());

            vertexPositions = new VertexPosition[batchSize * TotalVertices];
            transforms = new VertexTransform[batchSize * TotalVertices];
            colors = new VertexColor[batchSize * TotalVertices];

            indices = new short[batchSize * TotalIndices];

            for (int i = 0; i < batchSize; i++)
            {
                int start = TotalIndices * i;
                int buffer = TotalVertices * i;

                for (int j = 0; j < TotalIndices; j++)
                {
                    indices[start + j] = (short)(buffer + this.sharedGeometry.Mesh.Indices[j]);
                }
            }

            indexBuffer = new IndexBuffer(graphicsDevice, typeof(short), indices.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData(indices);
        }

        public override bool Add(Polygon polygon)
        {
            if (count >= BatchSize)
                return false;

            if (!polygon.GeometryData.Equals(sharedGeometry) || !polygon.RenderOptions.Equals(sharedRenderOptions))
                return false;

            VertexTransform transform = polygon.GetVertexTransform();
            VertexColor color = polygon.GetVertexColor();

            for (int i = 0; i < TotalVertices; i++)
            {
                vertexPositions[index + i] = new VertexPosition(sharedGeometry.Mesh.Vertices[i]);
                transforms[index + i] = transform;
                colors[index + i] = color;
            }

            dataModified = true;

            index += (uint)TotalVertices;
            count++;
            totalPrimitives += (uint)TotalTriangles;

            return true;
        }

        public override DrawGroup<Polygon> ApplyChanges()
        {
            if (!dataModified)
                return this;

            int vertexCount = (int)count * TotalVertices;

            vertexPositionBuffer?.Dispose();
            vertexPositionBuffer = new DynamicVertexBuffer(graphicsDevice, typeof(VertexPosition), vertexCount, BufferUsage.WriteOnly);
            vertexPositionBuffer.SetData(vertexPositions, 0, vertexCount);

            transformBuffer?.Dispose();
            transformBuffer = new DynamicVertexBuffer(graphicsDevice, typeof(VertexTransform), vertexCount, BufferUsage.WriteOnly);
            transformBuffer.SetData(transforms, 0, vertexCount);

            colorBuffer?.Dispose();
            colorBuffer = new DynamicVertexBuffer(graphicsDevice, typeof(VertexColor), vertexCount, BufferUsage.WriteOnly);
            colorBuffer.SetData(colors, 0, vertexCount);

            vertexBufferBindings = new VertexBufferBinding[]
            {
                new VertexBufferBinding(vertexPositionBuffer),
                new VertexBufferBinding(transformBuffer),
                new VertexBufferBinding(colorBuffer),
            };

            dataModified = false;

            return this;
        }

        public override DrawGroup<Polygon> Draw(Camera camera)
        {
            if (dataModified)
                throw new RelatusException("The polygon group was modified, but ApplyChanges() was never called.", new MethodExpectedException());

            graphicsDevice.RasterizerState = GraphicsManager.RasterizerState;
            graphicsDevice.SamplerStates[0] = sharedRenderOptions.SamplerState;
            graphicsDevice.BlendState = sharedRenderOptions.BlendState;
            graphicsDevice.DepthStencilState = sharedRenderOptions.DepthStencilState;
            graphicsDevice.SetVertexBuffers(vertexBufferBindings);
            graphicsDevice.Indices = indexBuffer;

            polygonShader.Parameters["WVP"].SetValue(camera.WVP);

            polygonPass.Apply();

            if (sharedRenderOptions.Effect == null)
            {
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, (int)totalPrimitives);
            }
            else
            {
                foreach (EffectPass pass in sharedRenderOptions.Effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, (int)totalPrimitives);
                }
            }

            return this;
        }

        protected override void OnDispose()
        {
            vertexPositionBuffer.Dispose();
            transformBuffer.Dispose();
            colorBuffer.Dispose();
        }
    }
}
