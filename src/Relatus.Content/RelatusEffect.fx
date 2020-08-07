#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

#include "MatrixHelper.fxh"

struct VSInput
{
	float4 Position : POSITION0;
	float3 Translation: POSITION1;
	float3 Scale: POSITION2;
	float3 Origin : POSITION3;
	float3 Rotation: Position4;
};

struct VSOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
};

matrix WVP;

VSOutput MainVS(in VSInput input)
{
	VSOutput output = (VSOutput)0;

	// Transform = Scale * [(Translation to origin) * Rotation * (Undo Translation to origin)] * Translation;
	float4x4 transform = mul(mul(mul(CreateScale(input.Scale), CreateTranslation(-input.Origin)), CreateRotation(input.Rotation)), CreateTranslation(input.Origin + input.Translation));

	output.Position = mul(mul(input.Position, transform), WVP);
	output.Color = float4(1, 1, 1, 1);

	return output;
}

float4 MainPS(VSOutput input) : COLOR
{
	return input.Color;
}

technique PolygonVertexTextureTransformColor
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};
