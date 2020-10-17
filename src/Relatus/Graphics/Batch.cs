using Microsoft.Xna.Framework.Graphics;
using System;

namespace Relatus.Graphics
{
    public class Batch : IDisposable
    {
        public VertexBufferBinding[] VertexBufferBindings { get; set; }
        public IndexBuffer IndexBuffer { get; set; }
        public IDisposable[] Disposables { get; set; }
        public BatchExecution BatchExecution { get; set; }
        public PrimitiveType PrimitiveType { get; set; }
        public int TotalPrimitives { get; set; }
        public int Instances { get; set; }

        private static readonly GraphicsDevice graphicsDevice;

        static Batch()
        {
            graphicsDevice = Engine.Graphics.GraphicsDevice;
        }

        public void Draw()
        {
            graphicsDevice.SetVertexBuffers(VertexBufferBindings);
            graphicsDevice.Indices = IndexBuffer;

            switch (BatchExecution)
            {
                case BatchExecution.DrawElements:
                    graphicsDevice.DrawIndexedPrimitives(PrimitiveType, 0, 0, TotalPrimitives);
                    break;
                case BatchExecution.DrawElementsInstanced:
                    graphicsDevice.DrawInstancedPrimitives(PrimitiveType, 0, 0, TotalPrimitives, Instances);
                    break;
            }
        }

        #region IDisposable Support
        private bool disposedValue;

        public void Dispose()
        {
            if (!disposedValue)
            {
                for (int i = 0; i < Disposables.Length; i++)
                {
                    Disposables[i].Dispose();
                }

                disposedValue = true;
            }
        }
        #endregion
    }
}
