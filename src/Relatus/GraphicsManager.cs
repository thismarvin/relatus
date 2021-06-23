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
        public static RasterizerState DebugRasterizerState { get; private set; }
        public static Texture2D SimpleTexture { get; private set; }
        public static Texture2D SimpleNormalTexture { get; private set; }

        private static readonly GraphicsDevice graphicsDevice;

        static GraphicsManager()
        {
            graphicsDevice = Engine.Graphics.GraphicsDevice;

            DefaultRasterizerState = new RasterizerState()
            {
                CullMode = CullMode.CullClockwiseFace
            };
            DebugRasterizerState = new RasterizerState
            {
                FillMode = FillMode.WireFrame,
                CullMode = CullMode.None
            };

            SimpleTexture = new Texture2D(graphicsDevice, 1, 1);
            SimpleTexture.SetData(new[] { Color.White });

            SimpleNormalTexture = new Texture2D(graphicsDevice, 1, 1);
            SimpleNormalTexture.SetData(new[] { ColorExt.CreateFromHex(0x7f7fff) });
        }

        internal static void UnloadContent()
        {
            DefaultRasterizerState.Dispose();
            DebugRasterizerState.Dispose();

            SimpleTexture.Dispose();
            SimpleNormalTexture.Dispose();
        }
    }
}

