#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

#include "MatrixHelper.fxh"

struct VSInputTransform
{
	float4 Position : POSITION0;
	float3 Scale: POSITION1;
	float3 RotationOffset: POSITION2;
	float3 Translation: POSITION3;
	float Rotation: TEXCOORD1;
};

struct VSInputTransformColor
{
	float4 Position : POSITION0;
	float3 Scale: POSITION1;
	float3 RotationOffset: POSITION2;
	float3 Translation: POSITION3;
	float Rotation: TEXCOORD1;
	float4 Color: COLOR5;
};

struct VSOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
};

matrix WorldViewProjection;

VSOutput VSTransform(in VSInputTransform input)
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
	output.Color = float4(1, 1, 1, 1);

	return output;
}

VSOutput VSTransformColor(in VSInputTransformColor input)
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
	output.Color = input.Color;

	return output;
}

float4 MainPS(VSOutput input) : COLOR
{
	return input.Color;
}

technique PolygonVertexTransform
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL VSTransform();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};

technique PolygonVertexTransformColor
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL VSTransformColor();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};
