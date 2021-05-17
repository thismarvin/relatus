#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

struct VSInput 
{
	float4 Position : POSITION0;
	float2 TextureCoordinates : TEXCOORD0;
	float4 Color : COLOR0;
	float4 M1 : POSITION1;
	float4 M2 : POSITION2;
	float4 M3 : POSITION3;
	float4 M4 : POSITION4;
};

struct VSOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

matrix WVP;
Texture2D Texture;

sampler2D TextureSampler = sampler_state
{
	Texture = <Texture>;
};

VSOutput VSMain(in VSInput input)
{
	VSOutput output = (VSOutput)0;

	float4x4 model = float4x4 (
		input.M1.x, input.M1.y, input.M1.z, input.M1.w,
		input.M2.x, input.M2.y, input.M2.z, input.M2.w,
		input.M3.x, input.M3.y, input.M3.z, input.M3.w,
		input.M4.x, input.M4.y, input.M4.z, input.M4.w
	);

	output.Position = mul(mul(input.Position, model), WVP);
	output.Color = input.Color;
	output.TextureCoordinates = input.TextureCoordinates;

	return output;
}

float4 PSMain(VSOutput input) : COLOR
{
	return tex2D(TextureSampler, input.TextureCoordinates) * input.Color;
}

technique GeometryBasic
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL VSMain();
		PixelShader = compile PS_SHADERMODEL PSMain();
	}
};
