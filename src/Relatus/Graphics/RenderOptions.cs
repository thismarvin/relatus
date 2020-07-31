using Microsoft.Xna.Framework.Graphics;

namespace Relatus.Graphics
{
    public class RenderOptions
    {
        public SamplerState SamplerState { get; set; }
        public BlendState BlendState { get; set; }
        public DepthStencilState DepthStencilState { get; set; }
        public Effect Effect { get; set; }

        public RenderOptions()
        {
            SamplerState = SamplerState.PointClamp;
            BlendState = BlendState.AlphaBlend;
            DepthStencilState = DepthStencilState.None;
            Effect = null;
        }
    }
}
