#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

#include "MatrixHelper.fxh"

struct VSInputTextureTransform
{
	float4 Position : POSITION0;
	float4 Color: COLOR0;
	float2 TextureCoordinates: TEXCOORD0;
	float3 Scale: POSITION1;
	float3 RotationOffset: POSITION2;
	float3 Translation: POSITION3;
	float Rotation: TEXCOORD1;
};

struct VSOutput
{
	float4 Position : SV_POSITION;
	float2 TextureCoordinates : TEXCOORD0;
	float4 Color : COLOR0;
};

Texture2D SpriteTexture;
sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

matrix WorldViewProjection;

VSOutput VSTextureTransformColor(in VSInputTextureTransform input)
{
	VSOutput output = (VSOutput)0;

	// We must set the z-value of the rotation offset to zero to avoid an "internal error: blob content mismatch".
	// I am not 100% sure, but I think the error is because at one point in the matrix multiplication the operation
	// "input.RotationOffset.z - input.RotationOffset.z" occurs. Considering the inaccuracies involved with floating point arithmetic,
	// the "blob content mismatch" error makes sense.
	input.RotationOffset.z = 0;

	// Transform = Scale * [(Translation to center) * Rotation * (Undo Translation to center)] * Translation;
	float4x4 transform = mul(mul(mul(CreateScale(input.Scale), CreateTranslation(-input.RotationOffset)), CreateRotationZ(input.Rotation)), CreateTranslation(input.RotationOffset + input.Translation));
	output.Position = mul(mul(input.Position, transform), WorldViewProjection);
	output.TextureCoordinates = input.TextureCoordinates;
	output.Color = input.Color;

	return output;
}

float4 MainPS(VSOutput input) : COLOR
{
	return tex2D(SpriteTextureSampler, input.TextureCoordinates) * input.Color;
}

technique PolygonVertexTextureTransformColor
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL VSTextureTransformColor();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};
