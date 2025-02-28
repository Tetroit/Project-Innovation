#ifndef SHADERFUNCS
#define SHADERFUNCS


void Clamp_float(float val, float min, float max, out float res)
{
	if (val > max)
	{
		res = max;
	}
	else if (val < min)
	{
		res = min;
	}
	else
	{
		res = val;
	}
}
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

void RNG11_float (float seed, out float res)
{
	res = frac(sin(dot(seed, 78.233)) * 43758.5453);
}
void RNG21_float (float2 seed, out float res)
{
	res = frac(sin(dot(seed.x, frac(cos(seed.y) * 1905.73510 + 183.5173))) * 43758.5453);
}
void RNG22_float (float2 seed, out float2 res)
{
	float r1, r2;
	RNG21_float(seed*seed + float2(29.51587, 9175.59739),r1);
	RNG21_float(seed + float2(91.59371, 7.9357),r2);
	res = float2(r1,r2);
}

float2 getPos (float2 uv, float2 off)
{
	float2 displ;
	RNG22_float(floor(uv + off),displ);
	return floor(uv + off) + displ;
}

void Voronoi2_float(float2 uv, float2 scale, float2 off, float power, bool edge, bool clamp, out float fac)
{
	float2 scaleInverse = 1/scale;
	uv = uv * scale + off;
	//RNG1_float(uv.x+time, col.x);
	float2 displ;
	float dist = 999999.;
	float dist2 = 999999.;
	float2 dir = 0;
	for (int i=-1; i<=1; i++)
	{
		for (int j=-1; j<=1; j++)
		{
			float2 pos = getPos(uv, float2(i, j));
			float current = distance(pos, uv);
			if (dist > current)
			{
				dist2 = dist;
				dist = current;
				dir = pos - uv;
			}
			else if (dist2 > current)
			{
				dist2 = current;
			}
		}
	}
	if (edge)
	{
		dist = dist2 - dist;
	}
	else
	{
		//dist *= scale;
	}
	float res = pow(dist, power);
	if (clamp)
	{
		Clamp_float(res, 0, 1, res);
	}
	fac = res;
}
#endif