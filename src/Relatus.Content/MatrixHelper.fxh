float4x4 CreateScale(float3 scale)
{
	return float4x4 (
		scale.x, 0, 0, 0,
		0, scale.y, 0, 0,
		0, 0, scale.z, 0,
		0, 0, 0, 1
	);
}

float4x4 CreateTranslation(float3 translation)
{
	return float4x4 (
		1, 0, 0, 0,
		0, 1, 0, 0,
		0, 0, 1, 0,
		translation.x, translation.y, translation.z, 1
	);
}

float4x4 CreateRotationZ(float theta)
{
	return float4x4 (
		cos(theta), -sin(theta), 0, 0,
		sin(theta), cos(theta), 0, 0,
		0, 0, 1, 0,
		0, 0, 0, 1
	);
}