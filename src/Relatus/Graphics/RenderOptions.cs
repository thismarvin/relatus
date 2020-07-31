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

        public bool Equals(RenderOptions other)
        {
            return SamplerState == other.SamplerState &&
                   BlendState == other.BlendState &&
                   DepthStencilState == other.DepthStencilState &&
                   Effect == other.Effect;
        }
    }
}
