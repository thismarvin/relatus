using Microsoft.Xna.Framework.Graphics;

namespace Relatus.Graphics
{
    public class RenderStates
    {
        public RasterizerState RasterizerState { get; set; }
        public BlendState BlendState { get; set; }
        public DepthStencilState DepthStencilState { get; set; }

        public static readonly RenderStates Default;

        static RenderStates()
        {
            Default = new RenderStates();
        }

        public RenderStates()
        {
            RasterizerState = GraphicsManager.DefaultRasterizerState;
            BlendState = BlendState.AlphaBlend;
            DepthStencilState = DepthStencilState.Default;
        }

        public bool Equals(RenderStates other)
        {
            return
                RasterizerState == other.RasterizerState &&
                BlendState == other.BlendState &&
                DepthStencilState == other.DepthStencilState;
        }
    }
}
