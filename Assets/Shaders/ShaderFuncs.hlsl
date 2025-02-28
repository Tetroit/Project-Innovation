#ifndef SHADERFUNCS
#define SHADERFUNCS

void RotateVector2_vector(float2 v, float angle, out float2 res)
{
	float s = sin(angle);
	float c = cos(angle);
	res = float2(v.x * c - v.y * s, v.x * s + v.y * c);
}

void MakePolar_float(float r, float angle, out float2 res)
{
	float s = sin(angle);
	float c = cos(angle);
	res = float2(r * c, r * s);
}

void VignetteMask_float(float2 uv, float2 center, float radius, float softness, out float res)
{
	float dist = distance(uv, center);
	res = saturate((radius - dist) / softness);
}
void DistanceToBorder_float(float2 uv, float2 minV, float2 maxV, out float res)
{
	float2 d = min(maxV - uv, uv - minV);
	res = min(d.x, d.y);
}

#endif