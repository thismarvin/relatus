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

float4x4 CreateRotation(float3 rotation)
{
	return float4x4 (
		cos(rotation.z) * cos(rotation.y), cos(rotation.z) * sin(rotation.y) * sin(rotation.x) - sin(rotation.z) * cos(rotation.x), cos(rotation.z) * sin(rotation.y) * cos(rotation.x) + sin(rotation.z) * sin(rotation.x), 0,
		sin(rotation.z) * cos(rotation.y), sin(rotation.z) * sin(rotation.y) * sin(rotation.x) + cos(rotation.z) * cos(rotation.x), sin(rotation.z) * sin(rotation.y) * cos(rotation.x) - cos(rotation.z) * sin(rotation.x), 0,
		-sin(rotation.y), cos(rotation.y) * sin(rotation.x), cos(rotation.y) * cos(rotation.x), 0,
		0, 0, 0, 1
	);
}

float4x4 CalculateTransform(float3 translation, float3 scale, float3 origin, float3 rotation)
{
	// Transform = Scale * [(Translation to origin) * Rotation * (Undo Translation to origin)] * Translation;
	return mul(mul(mul(CreateScale(scale), CreateTranslation(-origin)), CreateRotation(rotation)), CreateTranslation(origin + translation));
}

