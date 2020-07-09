using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mimoso.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mimoso.Graphics
{
    /// <summary>
    /// Creates a convex polygon on the fly!
    /// </summary>
    class PolygonBuilder
    {
        public int TotalPrimitives { get { return rawVertices.Count - 2; } }

        private readonly List<VertexPositionColor> rawVertices;
        private VertexPositionColor[] vertices;

        public PolygonBuilder()
        {
            rawVertices = new List<VertexPositionColor>();
        }

        public void Reset()
        {
            rawVertices.Clear();
        }

        public void AddVertex(float x, float y)
        {
            AddVertex(x, y, Color.Black);
        }

        public void AddVertex(float x, float y, Color color)
        {
            rawVertices.Add(new VertexPositionColor(new Vector3(x, y, 0), color));

            if (TotalPrimitives <= 0)
                return;

            vertices = new VertexPositionColor[rawVertices.Count];

            for (int i = 0; i < vertices.Length; i++)
            {
                if (i == 0)
                {
                    vertices[i] = rawVertices[0];
                }
                else if (i % 2 == 0)
                {
                    vertices[i] = rawVertices[vertices.Length - i / 2];
                }
                else
                {
                    vertices[i] = rawVertices[1 + i / 2];
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            if (TotalPrimitives <= 0)
                return;

            GraphicsManager.BasicEffect.World = camera.World;
            GraphicsManager.BasicEffect.View = camera.View;
            GraphicsManager.BasicEffect.Projection = camera.Projection;

            foreach (EffectPass pass in GraphicsManager.BasicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                spriteBatch.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices, 0, TotalPrimitives);
            }
        }
    }
}
