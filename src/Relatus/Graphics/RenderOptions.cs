using Microsoft.Xna.Framework.Graphics;

namespace Relatus.Graphics
{
    public class RenderOptions
    {
        public RasterizerState RasterizerState { get; set; }
        public BlendState BlendState { get; set; }
        public DepthStencilState DepthStencilState { get; set; }
        public Effect Effect { get; set; }

        public RenderOptions()
        {
            RasterizerState = GraphicsManager.DefaultRasterizerState;
            BlendState = BlendState.AlphaBlend;
            DepthStencilState = DepthStencilState.Default;
            Effect = null;
        }

        public bool Equals(RenderOptions other)
        {
            return
                RasterizerState == other.RasterizerState &&
                BlendState == other.BlendState &&
                DepthStencilState == other.DepthStencilState &&
                Effect == other.Effect;
        }
    }
}
