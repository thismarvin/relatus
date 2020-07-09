using Microsoft.Xna.Framework.Graphics;
using Mimoso.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mimoso.Graphics
{
    class PolygonGroup : DrawGroup<Polygon>, IDisposable
    {
        private readonly ShapeData sharedShapeData;
        private readonly VertexTransformColor[] transforms;

        private DynamicVertexBuffer transformBuffer;
        private VertexBufferBinding[] vertexBufferBindings;
        private bool dataChanged;

        public PolygonGroup(ShapeData sharedShapeData, int capacity) : base(capacity)
        {
            this.sharedShapeData = sharedShapeData;
            transforms = new VertexTransformColor[capacity];
            group = null;
        }

        protected override bool ConditionToAdd(Polygon polygon)
        {
            return polygon.ShapeData == sharedShapeData;
        }

        public override bool Add(Polygon polygon)
        {
            if (groupIndex >= capacity)
                return false;

            if (ConditionToAdd(polygon))
            {
                transforms[groupIndex++] = polygon.GetVertexTransformColor();
                dataChanged = true;
                return true;
            }

            return false;
        }

        private void UpdateBuffer()
        {
            transformBuffer?.Dispose();

            transformBuffer = new DynamicVertexBuffer(Engine.Graphics.GraphicsDevice, typeof(VertexTransformColor), transforms.Length, BufferUsage.WriteOnly);
            transformBuffer.SetData(transforms);

            vertexBufferBindings = new VertexBufferBinding[]
            {
                new VertexBufferBinding(sharedShapeData.Geometry),
                new VertexBufferBinding(transformBuffer, 0, 1)
            };
        }

        public override void Draw(Camera camera)
        {
            if (dataChanged)
            {
                UpdateBuffer();
                dataChanged = false;
            }

            Engine.Graphics.GraphicsDevice.RasterizerState = GraphicsManager.RasterizerState;
            Engine.Graphics.GraphicsDevice.SetVertexBuffers(vertexBufferBindings);
            Engine.Graphics.GraphicsDevice.Indices = sharedShapeData.Indices;

            GeometryManager.SetupPolygonShader(camera);

            foreach (EffectPass pass in GeometryManager.PolygonShader.Techniques[1].Passes)
            {
                pass.Apply();
                Engine.Graphics.GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, sharedShapeData.TotalTriangles, transforms.Length);
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
