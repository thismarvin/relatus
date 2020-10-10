using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Relatus
{
    /// <summary>
    /// Handles the entire life cycle of a set of various graphics realted objects.
    /// </summary>
    public static class GraphicsManager
    {
        public static RasterizerState DefaultRasterizerState { get; private set; }
        public static RasterizerState ScissorRasterizerState { get; private set; }
        public static RasterizerState DebugRasterizerState { get; private set; }

        public static Texture2D SimpleTexture { get; private set; }
        public static Texture2D SimpleNormalTexture { get; private set; }

        public static RasterizerState RasterizerState => DebugManager.ShowWireFrame ? DebugRasterizerState : DefaultRasterizerState;

        static GraphicsManager()
        {
            DefaultRasterizerState = new RasterizerState()
            {
                CullMode = CullMode.CullClockwiseFace
            };
            ScissorRasterizerState = new RasterizerState()
            {
                ScissorTestEnable = true,
                CullMode = CullMode.CullClockwiseFace
            };
            DebugRasterizerState = new RasterizerState
            {
                FillMode = FillMode.WireFrame,
                CullMode = CullMode.None
            };

            SimpleTexture = new Texture2D(Engine.Graphics.GraphicsDevice, 1, 1);
            SimpleTexture.SetData(new[] { Color.White });

            SimpleNormalTexture = new Texture2D(Engine.Graphics.GraphicsDevice, 1, 1);
            SimpleNormalTexture.SetData(new[] { ColorExt.CreateFromHex(0x7f7fff) });
        }

        internal static void UnloadContent()
        {
            DefaultRasterizerState.Dispose();
            ScissorRasterizerState.Dispose();
            DebugRasterizerState.Dispose();

            SimpleTexture.Dispose();
            SimpleNormalTexture.Dispose();
        }
    }
}

