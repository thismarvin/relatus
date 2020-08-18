using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Graphics
{
    internal class PolygonGroup : DrawGroup<Polygon>, IDisposable
    {
        private readonly GeometryData sharedGeometry;
        private readonly RenderOptions sharedRenderOptions;

        private readonly VertexTransform[] transforms;
        private readonly VertexColor[] colors;

        private DynamicVertexBuffer transformBuffer;
        private DynamicVertexBuffer colorBuffer;
        private VertexBufferBinding[] vertexBufferBindings;
        private bool dataChanged;

        private static readonly GraphicsDevice graphicsDevice;
        private static readonly Effect polygonShader;
        private static readonly EffectPass polygonPass;

        static PolygonGroup()
        {
            graphicsDevice = Engine.Graphics.GraphicsDevice;
            polygonShader = AssetManager.GetEffect("Relatus_RelatusEffect");
            polygonPass = polygonShader.Techniques[1].Passes[0];
        }

        public PolygonGroup(GeometryData sharedGeometry, RenderOptions sharedRenderOptions, int capacity) : base(capacity)
        {
            this.sharedGeometry = sharedGeometry;
            this.sharedRenderOptions = sharedRenderOptions;

            transforms = new VertexTransform[capacity];
            colors = new VertexColor[capacity];
            group = null;
        }

        protected override bool ConditionToAdd(Polygon polygon)
        {
            return polygon.Geometry.Equals(sharedGeometry) && polygon.RenderOptions.Equals(sharedRenderOptions);
        }

        public override bool Add(Polygon polygon)
        {
            if (groupIndex >= capacity)
                return false;

            if (ConditionToAdd(polygon))
            {
                transforms[groupIndex] = polygon.GetVertexTransform();
                colors[groupIndex] = new VertexColor(polygon.Color);
                groupIndex++;
                dataChanged = true;

                return true;
            }

            return false;
        }

        private void UpdateBuffer()
        {
            transformBuffer?.Dispose();
            transformBuffer = new DynamicVertexBuffer(graphicsDevice, typeof(VertexTransform), transforms.Length, BufferUsage.WriteOnly);
            transformBuffer.SetData(transforms);

            colorBuffer?.Dispose();
            colorBuffer = new DynamicVertexBuffer(graphicsDevice, typeof(VertexColor), colors.Length, BufferUsage.WriteOnly);
            colorBuffer.SetData(colors);

            vertexBufferBindings = new VertexBufferBinding[]
            {
                new VertexBufferBinding(sharedGeometry.VertexBuffer),
                new VertexBufferBinding(transformBuffer, 0, 1),
                new VertexBufferBinding(colorBuffer, 0, 1)
            };
        }

        public override void Draw(Camera camera)
        {
            if (dataChanged)
            {
                UpdateBuffer();
                dataChanged = false;
            }

            graphicsDevice.RasterizerState = GraphicsManager.RasterizerState;
            graphicsDevice.SamplerStates[0] = sharedRenderOptions.SamplerState;
            graphicsDevice.BlendState = sharedRenderOptions.BlendState;
            graphicsDevice.DepthStencilState = sharedRenderOptions.DepthStencilState;
            graphicsDevice.SetVertexBuffers(vertexBufferBindings);
            graphicsDevice.Indices = sharedGeometry.IndexBuffer;

            polygonShader.Parameters["WVP"].SetValue(camera.WVP);

            polygonPass.Apply();

            if (sharedRenderOptions.Effect == null)
            {
                graphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, sharedGeometry.TotalTriangles, capacity);
            }
            else
            {
                foreach (EffectPass pass in sharedRenderOptions.Effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    graphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, sharedGeometry.TotalTriangles, capacity);
                }
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    transformBuffer.Dispose();
                    colorBuffer.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~PolygonGroup()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
