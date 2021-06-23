#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

#include "MatrixHelper.fxh"

struct VSGeometryBasicInput
{
	float4 Position : POSITION0;
	float3 Translation: POSITION1;
	float3 Scale: POSITION2;
	float3 Origin : POSITION3;
	float3 Rotation: POSITION4;
};

struct VSGeometryColorInput
{
	float4 Position : POSITION0;
	float3 Translation: POSITION1;
	float3 Scale: POSITION2;
	float3 Origin : POSITION3;
	float3 Rotation: POSITION4;
	float4 Color: COLOR0;
};

struct VSTextureBasicInput
{
	float4 Position : POSITION0;
	float3 Translation: POSITION1;
	float3 Scale: POSITION2;
	float3 Origin : POSITION3;
	float3 Rotation: POSITION4;
	float4 Color: COLOR0;
	float2 TextureCoordinates: TEXCOORD0;
};

struct VSBasicOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
};

struct VSTextureOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

// Uniforms
matrix WVP;
Texture2D SpriteTexture;
sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

VSBasicOutput VSGeometryBasic(in VSGeometryBasicInput input)
{
	VSBasicOutput output = (VSBasicOutput)0;

	float4x4 transform = CalculateTransform(input.Translation, input.Scale, input.Origin, input.Rotation);

	output.Position = mul(mul(input.Position, transform), WVP);
	output.Color = float4(1, 1, 1, 1);

	return output;
}

VSBasicOutput VSGeometryColor(in VSGeometryColorInput input)
{
	VSBasicOutput output = (VSBasicOutput)0;

	float4x4 transform = CalculateTransform(input.Translation, input.Scale, input.Origin, input.Rotation);

	output.Position = mul(mul(input.Position, transform), WVP);
	output.Color = input.Color;

	return output;
}

VSTextureOutput VSTextureBasic(in VSTextureBasicInput input)
{
	VSTextureOutput output = (VSTextureOutput)0;

	float4x4 transform = CalculateTransform(input.Translation, input.Scale, input.Origin, input.Rotation);

	output.Position = mul(mul(input.Position, transform), WVP);
	output.Color = input.Color;
	output.TextureCoordinates = input.TextureCoordinates;

	return output;
}

float4 PSBasic(VSBasicOutput input) : COLOR
{
	return input.Color;
}

float4 PSTexture(VSTextureOutput input) : COLOR
{
	return tex2D(SpriteTextureSampler, input.TextureCoordinates) * input.Color;
}

technique GeometryBasic
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL VSGeometryBasic();
		PixelShader = compile PS_SHADERMODEL PSBasic();
	}
};

technique GeometryColor
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL VSGeometryColor();
		PixelShader = compile PS_SHADERMODEL PSBasic();
	}
};

technique TextureBasic
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL VSTextureBasic();
		PixelShader = compile PS_SHADERMODEL PSTexture();
	}
};

