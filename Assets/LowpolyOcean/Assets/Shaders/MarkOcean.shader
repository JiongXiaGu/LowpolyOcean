Shader "LowpolyOcean/MarkOcean"
{
	Properties
	{
        [Header(Tessellation)]
        _OceanTessellation ("Tessellation", Range(1, 32)) = 4
        _OceanTessMinDistance ("TessMinDistance", float) = 100
        _OceanTessMaxDistance ("TessMaxDistance", float) = 200

        [Header(Wave)]
        [NoScaleOffset] _OceanWaveTexture ("WaveTexture", 2D) = "black" {}
        _OceanWaveRect0 ("Wave0 Rect", Vector) = (0.5, 0.5, 30, 30)
        _OceanWaveRect1 ("Wave1 Rect", Vector) = (0.5, 0.5, 30, 30)
        _OceanWaveRect2 ("Wave2 Rect", Vector) = (0.5, 0.5, 30, 30)
        _OceanWaveRect3 ("Wave3 Rect", Vector) = (0.5, 0.5, 30, 30)
        _OceanWaveRadian ("Wave Radian", Vector) = (0, 0, 0, 0)
        _OceanWaveHeightPow ("Wave Height Pow", Vector) = (1, 1, 1, 1)
        _OceanWaveHeightScale ("Wave Height Scale", Vector) = (0.5, 0.5, 0.5, 0.5)
        _OceanWaveSpeedZ ("Speed z", Vector) = (0.1, 0.1, 0.1, 0.1)
	}


    CGINCLUDE
    #include "UnityCG.cginc"
    #include "LPOceanVertex.cginc"
    #include "LPOceanTessellation.cginc"
    #include "LPUnderOceanMark.cginc"

    struct VertData
    {
        float4 vertex : POSITION;
    };

    struct FragData
    {
        float4 vertex : INTERNALTESSPOS0;
	    UNITY_POSITION(pos);
        half3 worldViewDir : TEXCOORD1;
    };

    FragData vert (VertData v)
    {
	    FragData o;
        float4 vertex = v.vertex;
        float4 worldPos = mul(unity_ObjectToWorld, vertex);

        GetOceanVert(worldPos, vertex);

        o.vertex = v.vertex;
	    o.pos = UnityObjectToClipPos(vertex);
        o.worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));

	    return o;
    }
			

    TessellationFactors LPWPatchConstant (InputPatch<FragData, 3> patch) 
    {
        OCEAN_PATCH_CONSTANT
    }

    [UNITY_domain("tri")]
    [UNITY_outputcontrolpoints(3)]
    [UNITY_outputtopology("triangle_cw")]
    [UNITY_partitioning(LPOCEAN_UNITY_PARTITIONING)]
    [UNITY_patchconstantfunc("LPWPatchConstant")]
    FragData LPWHull(InputPatch<FragData, 3> patch, uint id : SV_OutputControlPointID)
    {
        return patch[id];
    }

    [UNITY_domain("tri")]
    FragData LPWDomain(TessellationFactors factors, OutputPatch<FragData, 3> patch, float3 barycentricCoordinates : SV_DomainLocation)
    {
        VertData data;

        DOMAIN_PROGRAM_INTERPOLATE(vertex)

        return vert(data);
    }

    half4 frag (FragData vertexOutput, fixed facing : VFACE) : SV_Target
    {
        half4 color;

        if(facing > 0)
            color = half4(_LPOceanFornMark, vertexOutput.worldViewDir);
        else
            color = half4(_LPOceanBackMark, vertexOutput.worldViewDir);

        return color;
    }

    ENDCG

    //DX11
	SubShader
	{
		Tags { "RenderType"="Opaque" }
        Cull Off

        Pass
        {
			CGPROGRAM
            #pragma target 4.6

            #pragma shader_feature __ LPOCEAN_WAVE1 LPOCEAN_WAVE2 LPOCEAN_WAVE3
            #pragma shader_feature __ LPOCEAN_RIPPLE
            #pragma shader_feature __ LPOCEAN_CLIP

            #pragma vertex vert
            #pragma hull LPWHull
            #pragma domain LPWDomain
			#pragma fragment frag
			ENDCG
        }
	}

    //DX10 & DX9
	SubShader
	{
		Tags { "RenderType"="Opaque" }
        Cull Off

        Pass
        {
			CGPROGRAM
            #pragma shader_feature __ LPOCEAN_WAVE1 LPOCEAN_WAVE2 LPOCEAN_WAVE3
            #pragma shader_feature __ LPOCEAN_RIPPLE
            #pragma shader_feature __ LPOCEAN_CLIP

            #pragma vertex vert
			#pragma fragment frag
			ENDCG
        }
	}
}
