using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Relatus.Core
{
    /// <summary>
    /// Handles the entire life cycle of a set of various graphics realted objects.
    /// </summary>
    static class GraphicsManager
    {
        public static RasterizerState DefaultRasterizerState { get; private set; }
        public static RasterizerState ScissorRasterizerState { get; private set; }
        public static RasterizerState DebugRasterizerState { get; private set; }
        public static SpriteBatch SpriteBatch { get; private set; }
        public static Texture2D SimpleTexture { get; private set; }
        public static BasicEffect BasicEffect { get; private set; }

        public static RasterizerState RasterizerState { get => DebugManager.ShowWireFrame ? DebugRasterizerState : DefaultRasterizerState; }

        static GraphicsManager()
        {
            DefaultRasterizerState = new RasterizerState();
            ScissorRasterizerState = new RasterizerState() { ScissorTestEnable = true };
            DebugRasterizerState = new RasterizerState
            {
                FillMode = FillMode.WireFrame,
                CullMode = CullMode.None
            };

            SpriteBatch = new SpriteBatch(Engine.Graphics.GraphicsDevice);

            SimpleTexture = new Texture2D(Engine.Graphics.GraphicsDevice, 1, 1);
            SimpleTexture.SetData(new[] { Color.White });

            BasicEffect = new BasicEffect(Engine.Graphics.GraphicsDevice)
            {
                VertexColorEnabled = true,
            };
        }

        internal static void UnloadContent()
        {
            DefaultRasterizerState.Dispose();
            ScissorRasterizerState.Dispose();
            DebugRasterizerState.Dispose();
            SpriteBatch.Dispose();
            SimpleTexture.Dispose();
            BasicEffect.Dispose();
        }
    }
}

