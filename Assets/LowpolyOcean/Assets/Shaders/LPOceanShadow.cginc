#ifndef LPOCEAN_CORE_INCLUDED
#define LPOCEAN_CORE_INCLUDED

    #include "UnityCG.cginc"
    #include "AutoLight.cginc"
    #include "LPOceanHelper.cginc"
    #include "LPOceanVertex.cginc"
    #include "LPOceanTessellation.cginc"
    #include "LPOceanLighting.cginc"

    struct VertexInput
    {
	    float4 vertex : POSITION;
    };

    struct VertexOutputShadow
    {
        UNITY_POSITION(pos);
        float4 vertex : INTERNALTESSPOS0;
        float4 screenPos : TEXCOORD0;
    };

    VertexOutputShadow VertShadow(VertexInput v)
    {
        VertexOutputShadow o;
        UNITY_INITIALIZE_OUTPUT(VertexOutputShadow, o);
        float4 vertex = v.vertex;
        float4 worldPos = mul(unity_ObjectToWorld, vertex);

        GetOceanVert(worldPos, vertex);

        o.pos = UnityObjectToClipPos(vertex);
        o.vertex = v.vertex;
        o.screenPos = ComputeScreenPos(o.pos);
        return o;
    }

    TessellationFactors PatchConstantShadow(InputPatch<VertexOutputShadow, 3> patch) 
    {
        OCEAN_PATCH_CONSTANT
    }

    [UNITY_domain("tri")]
    [UNITY_outputcontrolpoints(3)]
    [UNITY_outputtopology("triangle_cw")]
    [UNITY_partitioning(LPOCEAN_UNITY_PARTITIONING)]
    [UNITY_patchconstantfunc("PatchConstantShadow")]
    VertexOutputShadow HullShadow(InputPatch<VertexOutputShadow, 3> patch, uint id : SV_OutputControlPointID)
    {
        return patch[id];
    }

    [UNITY_domain("tri")]
    VertexOutputShadow DomainShadow(TessellationFactors factors, OutputPatch<VertexOutputShadow, 3> patch, float3 barycentricCoordinates : SV_DomainLocation)
    {
        VertexInput data;

        DOMAIN_PROGRAM_INTERPOLATE(vertex)

        return VertShadow(data);
    }

    half4 FragShadow(VertexOutputShadow i) : SV_Target
    {
        #if LPOCEAN_ANY_DEPTH_EFFECT
            half cameraDepth, oceanDepth, refractionDepth;
            GetOceanDepth(i.pos.w, i.screenPos, cameraDepth, oceanDepth, refractionDepth);

            LPOCEAN_CLIP_APPLY(i, cameraDepth)
        #endif

        SHADOW_CASTER_FRAGMENT(i)
    }

#endif

