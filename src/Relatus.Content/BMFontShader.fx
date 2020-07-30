#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 TextColor;
float4 OutlineColor;
float4 AAColor;

float4 MainPS(VertexShaderOutput input) : COLOR0
{
	float4 color = tex2D(SpriteTextureSampler, input.TextureCoordinates);

	if (color.r >= 1) return TextColor;
	if (color.r >= 127.0 / 256.0) return OutlineColor;
	if (color.r >= 63.0 / 256.0) return AAColor;

    return float4(0, 0, 0, 0);
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};
