using Microsoft.Xna.Framework.Graphics;

namespace Relatus.Graphics
{
    public static class GraphicsHelper
    {
        private static readonly GraphicsDevice graphicsDevice;

        static GraphicsHelper()
        {
            graphicsDevice = Engine.Graphics.GraphicsDevice;
        }

        public static void ApplyRenderStates(RenderStates renderStates)
        {
            graphicsDevice.RasterizerState = renderStates.RasterizerState;
            graphicsDevice.BlendState = renderStates.BlendState;
            graphicsDevice.DepthStencilState = renderStates.DepthStencilState;
        }

        public static void BindTexture(params (uint, Texture, SamplerState)[] textureData)
        {
            for (int i = 0; i < textureData.Length; i++)
            {
                int index = (int)textureData[i].Item1;
                graphicsDevice.Textures[index] = textureData[i].Item2;
                graphicsDevice.SamplerStates[index] = textureData[i].Item3;
            }
        }
    }
}
