#ifndef LPOCEAN_TESSELLATION_INCLUDED
#define LPOCEAN_TESSELLATION_INCLUDED

#include "Tessellation.cginc"

#define DOMAIN_PROGRAM_INTERPOLATE(fieldName) data.fieldName = \
	patch[0].fieldName * barycentricCoordinates.x + \
	patch[1].fieldName * barycentricCoordinates.y + \
	patch[2].fieldName * barycentricCoordinates.z;

#define TessellationBy(FragDataName, v0, v1, v2) UnityDistanceBasedTess(v0.vertex, v1.vertex, v2.vertex, _OceanTessMinDistance, _OceanTessMaxDistance, _OceanTessellation);

#define LPOCEAN_UNITY_PARTITIONING "integer"
//fractional_odd

#define OCEAN_PATCH_CONSTANT TessellationFactors f;\
    float4 v = TessellationBy(LPWVertData, patch[0], patch[1], patch[2])\
    f.edge[0] = v.x;\
    f.edge[1] = v.y;\
    f.edge[2] = v.z;\
	f.inside = v.w;\
	return f;\

float _OceanTessMinDistance;
float _OceanTessMaxDistance;
float _OceanTessellation;

struct TessellationFactors 
{
    float edge[3] : SV_TessFactor;
    float inside : SV_InsideTessFactor;
};

#endif