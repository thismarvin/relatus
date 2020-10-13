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
	float3 Normal : NORMAL0;
	float3 Tangent : TANGENT0;
	float2 TextureCoordinates: TEXCOORD0;
};

struct VSOutput
{
	float4 Position : SV_POSITION;
	float2 TextureCoordinates : TEXCOORD0;

	float3 TangentPosition : TEXCOORD1;
	float3 TangentCameraPosition : TEXCOORD2;
	float3 TangentLightOneDirection : TEXCOORD3;
	float3 TangentLightTwoDirection: TEXCOORD4;
};

matrix WVP;
matrix Model;
matrix Normal;

float4 Tint;
float Shininess;

float3 CameraPosition;

float3 EnvironmentColor;

Texture2D DiffuseMap;
Texture2D NormalMap;
Texture2D SpecularMap;

float3 LightOneDirection;
float3 LightOneDiffuse;
float3 LightOneSpecular;

float3 LightTwoDirection;
float3 LightTwoDiffuse;
float3 LightTwoSpecular;

sampler2D DiffuseTextureSampler = sampler_state
{
	Texture = <DiffuseMap>;
};
sampler2D NormalTextureSampler = sampler_state
{
	Texture = <NormalMap>;
};
sampler2D SpecularTextureSampler = sampler_state
{
	Texture = <SpecularMap>;
};

struct Material
{
	float4 DiffuseColor;
	float4 SpecularColor;
	float Shininess;
};

struct DirectionalLight
{
	float3 Direction;
	float4 DiffuseColor;
	float4 SpecularColor;
};

float4 BlinnPhongIlluminationDirectional(float3 viewDirection, float3 normal, DirectionalLight light, Material material)
{
	float3 halfwayDirection = normalize(light.Direction + viewDirection);

	float diffuseIntensity = max(dot(light.Direction, normal), 0.0);
	float4 diffuse = light.DiffuseColor * material.DiffuseColor;
	diffuse = float4(diffuse.rgb * diffuseIntensity, diffuse.a);

	float specularIntensity = pow(max(dot(normal, halfwayDirection), 0.0), material.Shininess);
	float4 specular = light.SpecularColor * material.SpecularColor;
	specular = float4(specular.rgb * specularIntensity, specular.a);

	return diffuse + specular;
}

VSOutput MainVS(in VSInput input)
{
	VSOutput output = (VSOutput)0;

	float4 position = mul(input.Position, Model);

	output.Position = mul(position, WVP);
	output.TextureCoordinates = input.TextureCoordinates;

	float3 normal = normalize(mul(float4(input.Normal, 0), Normal)).xyz;
	float3 tangent = normalize(mul(float4(input.Tangent, 0), Normal)).xyz;
	float3 binormal = cross(normal, tangent);
	float3x3 tbn = transpose(float3x3(tangent, binormal, normal));

	output.TangentPosition = mul(position.xyz, tbn);
	output.TangentCameraPosition = mul(CameraPosition, tbn);
	output.TangentLightOneDirection = mul(LightOneDirection, tbn);
	output.TangentLightTwoDirection = mul(LightTwoDirection, tbn);

	return output;
}

float4 MainPS(VSOutput input) : COLOR
{
	float4 baseDiffuseColor = tex2D(DiffuseTextureSampler, input.TextureCoordinates) * Tint;
	float4 baseSpecularColor = tex2D(SpecularTextureSampler, input.TextureCoordinates);

	Material material;
	material.DiffuseColor = baseDiffuseColor;
	material.SpecularColor = baseSpecularColor;
	material.Shininess = Shininess;

	float3 viewDirection = normalize(input.TangentCameraPosition - input.TangentPosition);
	float3 normal = normalize(tex2D(NormalTextureSampler, input.TextureCoordinates).xyz * 2.0 - 1.0);

	DirectionalLight lightOne;
	lightOne.Direction = normalize(-input.TangentLightOneDirection);
	lightOne.DiffuseColor = float4(LightOneDiffuse, 1);
	lightOne.SpecularColor = float4(LightOneSpecular, 1);

	DirectionalLight lightTwo;
	lightTwo.Direction = normalize(-input.TangentLightTwoDirection);
	lightTwo.DiffuseColor = float4(LightTwoDiffuse, 1);
	lightTwo.SpecularColor = float4(LightTwoSpecular, 1);

	float4 ambientColor = float4(EnvironmentColor, 1) * baseDiffuseColor;

	float4 illuminationOne = BlinnPhongIlluminationDirectional(viewDirection, normal, lightOne, material);
	float4 illuminationTwo = BlinnPhongIlluminationDirectional(viewDirection, normal, lightTwo, material);

	return ambientColor + illuminationOne + illuminationTwo;
}

technique TextureBasic
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};
