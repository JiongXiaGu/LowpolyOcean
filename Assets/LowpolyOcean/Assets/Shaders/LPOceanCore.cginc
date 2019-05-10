#ifndef LPOCEAN_CORE_INCLUDED
#define LPOCEAN_CORE_INCLUDED

    #include "UnityCG.cginc"
    #include "UnityStandardUtils.cginc"
    #include "AutoLight.cginc"
    #include "UnityGBuffer.cginc"
    #include "LPOceanHelper.cginc"
    #include "LPOceanVertex.cginc"
    #include "LPOceanLighting.cginc"
    #include "LPOceanTessellation.cginc"
    #include "LPUnderOceanLighting.cginc"

    struct VertexInput
    {
	    float4 vertex : POSITION;
    };

    struct VertexInputDX9
    {
	    float4 vertex : POSITION;
        float3 uv0 : TEXCOORD0;
        float3 uv1 : TEXCOORD1;
    };

    struct VertexOutputForwardBase
    {
        UNITY_POSITION(pos);
        float4 vertex : INTERNALTESSPOS0;
        float4 worldPos : TEXCOORD0;
        half3 worldNormal : TEXCOORD1;
        half3 worldViewDir : TEXCOORD2;
        half4 screenPos : TEXCOORD3;
        UNITY_FOG_COORDS(4)
        SHADOW_COORDS(5)
        LPOCEAN_FOAM_COORD(6, 7)
        LPOCEAN_REFLECTION_COORD(8)
    };


    #define LPOCEAN_LIGHTING_TRANSFER(vertexOutput) LPOCEAN_FOAM_TRANSFER(vertexOutput)\
        LPOCEAN_REFLECTION_TRANSFER(vertexOutput)\
        

    // ------------------------------------------------------------------
    // DX9 ForwardBase Pass

    VertexOutputForwardBase VertForwardBaseDX9(VertexInputDX9 v)
    {
        VertexOutputForwardBase o;
        UNITY_INITIALIZE_OUTPUT(VertexOutputForwardBase, o);
        float4 vertex = v.vertex;
        float4 worldPos = mul(unity_ObjectToWorld, float4(vertex.xyz, 1));
        float3 worldPos1 = mul(unity_ObjectToWorld, float4(v.uv0, 1));
        float3 worldPos2 = mul(unity_ObjectToWorld, float4(v.uv1, 1));

        GetOceanVert(worldPos, vertex);
        GetOceanVert(worldPos1);
        GetOceanVert(worldPos2);

        o.pos = UnityObjectToClipPos(vertex);
        o.vertex = v.vertex;
        o.worldPos = worldPos;
        o.worldNormal = GetNormal(worldPos, worldPos1, worldPos2);
        o.worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
        o.screenPos = ComputeScreenPos(o.pos);

        TRANSFER_SHADOW(o)
        UNITY_TRANSFER_FOG(o, o.pos);
        LPOCEAN_LIGHTING_TRANSFER(o)
        return o;
    }


    // ------------------------------------------------------------------
    // DX10 or DX11 ForwardBase Pass

    VertexOutputForwardBase VertForwardBase(VertexInput v)
    {
        VertexOutputForwardBase o;
        UNITY_INITIALIZE_OUTPUT(VertexOutputForwardBase, o);
        float4 vertex = v.vertex;
        float4 worldPos = mul(unity_ObjectToWorld, float4(vertex.xyz, 1));

        GetOceanVert(worldPos, vertex);

        o.pos = UnityObjectToClipPos(vertex);
        o.vertex = v.vertex;
        o.worldPos = worldPos;
        o.worldNormal = 0;
        o.worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
        o.screenPos = ComputeScreenPos(o.pos);

        TRANSFER_SHADOW(o)
        return o;
    }


    // ------------------------------------------------------------------
    // Tessellation

    TessellationFactors PatchConstantForwardBase(InputPatch<VertexOutputForwardBase, 3> patch) 
    {
        OCEAN_PATCH_CONSTANT
    }

    [UNITY_domain("tri")]
    [UNITY_outputcontrolpoints(3)]
    [UNITY_outputtopology("triangle_cw")]
    [UNITY_partitioning(LPOCEAN_UNITY_PARTITIONING)]
    [UNITY_patchconstantfunc("PatchConstantForwardBase")]
    VertexOutputForwardBase HullForwardBase(InputPatch<VertexOutputForwardBase, 3> patch, uint id : SV_OutputControlPointID)
    {
        return patch[id];
    }

    [UNITY_domain("tri")]
    VertexOutputForwardBase DomainForwardBase(TessellationFactors factors, OutputPatch<VertexOutputForwardBase, 3> patch, float3 barycentricCoordinates : SV_DomainLocation)
    {
        VertexInput data;

        DOMAIN_PROGRAM_INTERPOLATE(vertex)

        return VertForwardBase(data);
    }


    // ------------------------------------------------------------------
    // Geometry

    [maxvertexcount(3)]
    void GeometryForwardBase(triangle VertexOutputForwardBase input[3], inout TriangleStream<VertexOutputForwardBase> outputStream)
    {
        #define GEOMETRY_FORWARD_BASE(index) input[index].worldNormal = worldNormal;\
            UNITY_TRANSFER_FOG(input[index], input[index].pos);\
            LPOCEAN_LIGHTING_TRANSFER(input[index]);\
            outputStream.Append(input[index]);

        float3 worldNormal = GetNormal(input[0].worldPos, input[1].worldPos, input[2].worldPos);

        GEOMETRY_FORWARD_BASE(0)
        GEOMETRY_FORWARD_BASE(1)
        GEOMETRY_FORWARD_BASE(2)
    }


    // ------------------------------------------------------------------
    // Frag

    UnityGI FragmentGI(VertexOutputForwardBase oceanData, UnityLight light, half atten)
    {
        UnityGIInput input;
        input.light = light;
        input.worldPos = oceanData.worldPos;
        input.worldViewDir = oceanData.worldViewDir;
        input.atten = atten;
        input.ambient = 0;
        input.lightmapUV = 0;

        //Form "UnityStandardCore.cginc"
            input.probeHDR[0] = unity_SpecCube0_HDR;
            input.probeHDR[1] = unity_SpecCube1_HDR;
            #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
              input.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
            #endif
            #ifdef UNITY_SPECCUBE_BOX_PROJECTION
              input.boxMax[0] = unity_SpecCube0_BoxMax;
              input.probePosition[0] = unity_SpecCube0_ProbePosition;
              input.boxMax[1] = unity_SpecCube1_BoxMax;
              input.boxMin[1] = unity_SpecCube1_BoxMin;
              input.probePosition[1] = unity_SpecCube1_ProbePosition;
        #endif
        //End

        UnityGI gi;
        //LightingStandard_GI(surfaceData, input, gi);  error : output parameter 'gi' not completely initialized (-_-?)
        #if defined(UNITY_PASS_DEFERRED) && UNITY_ENABLE_REFLECTION_BUFFERS
            gi = UnityGlobalIllumination(input, _OceanOcclusion, oceanData.worldNormal);
        #else
            //Unity_GlossyEnvironmentData g = UnityGlossyEnvironmentSetup(surfaceData.Smoothness, input.worldViewDir, surfaceData.Normal, lerp(unity_ColorSpaceDielectricSpec.rgb, surfaceData.Albedo, surfaceData.Metallic));
            Unity_GlossyEnvironmentData g = UnityGlossyEnvironmentSetup(_OceanSmoothness, oceanData.worldViewDir, oceanData.worldNormal, 0);
            gi = UnityGlobalIllumination(input, _OceanOcclusion, oceanData.worldNormal, g);
        #endif

        return gi;
    }


    half4 GetOceanBackColor(VertexOutputForwardBase vertexOutput, half shadowAtten)
    {
        half3 color = _OceanBackArtisticColor;

        #if LPOCEAN_ANY_DEPTH_EFFECT
            float cameraDepth, oceanDepth, refractionDepth;
            GetOceanDepth(vertexOutput.pos, vertexOutput.screenPos, cameraDepth, oceanDepth, refractionDepth);

            LPOCEAN_CLIP_APPLY(vertexOutput, cameraDepth)
            LPOCEAN_BACK_REFRACTION_APPLY(color, vertexOutput, oceanDepth)
        #endif

        color = OceanBackArtisticLighting(color, vertexOutput.worldNormal, vertexOutput.worldViewDir, shadowAtten);
        LPOCEAN_BACK_POINT_LIGHTING_APPLY(color, vertexOutput)
        return half4(color, 1);
    }

    half3 GetOceanAlbedo(VertexOutputForwardBase vertexOutput, half3 albedo)
    {
        #if LPOCEAN_FRESNEL
            albedo = Fresnel(albedo, vertexOutput.worldNormal, vertexOutput.worldViewDir);
        #endif

        #if LPOCEAN_ANY_DEPTH_EFFECT
            float cameraDepth, oceanDepth, refractionDepth;
            GetOceanDepth(vertexOutput.pos, vertexOutput.screenPos, cameraDepth, oceanDepth, refractionDepth);

            LPOCEAN_CLIP_APPLY(vertexOutput, cameraDepth)
            LPOCEAN_REFRACTION_APPLY(albedo, vertexOutput, oceanDepth)
            LPOCEAN_FOAM_APPLY(albedo, vertexOutput, oceanDepth);
        #endif

        LPOCEAN_REFLECTION_APPLY(albedo, vertexOutput)
        LPOCEAN_POINT_LIGHTING_APPLY(albedo, vertexOutput)
        return albedo;
    }

    //UnityPBS
    half4 GetLightingForwardBaseStandard(VertexOutputForwardBase vertexOutput, UnityLight light, half shadowAtten)
    {
        shadowAtten = saturate(shadowAtten + _OceanShadowAttenuation);
        UnityGI gi = FragmentGI(vertexOutput, light, shadowAtten);

        half3 albedo = GetOceanAlbedo(vertexOutput, _OceanAlbedo);
        half3 specColor = _OceanSpecular.rgb;
        half oneMinusReflectivity = OneMinusReflectivityFromMetallic(_OceanSpecular.a);
        half4 c = UNITY_BRDF_PBS(albedo, specColor, oneMinusReflectivity, _OceanSmoothness, vertexOutput.worldNormal, vertexOutput.worldViewDir, gi.light, gi.indirect);
        return c;
    }

    half4 FragForwardBase(VertexOutputForwardBase vertexOutput
    #if LPOCEAN_BACK_LIGHTING
        , fixed facing : VFACE
    #endif
    ) : SV_Target
    {
        UnityLight light = MainLight();
        half shadowAtten = SHADOW_ATTENUATION(vertexOutput);
        half4 color;

        #if LPOCEAN_BACK_LIGHTING
            if(facing < 0)
            {
                color = GetOceanBackColor(vertexOutput, shadowAtten);
                APPLY_LPUNDER_OCEAN_EFFECT_ONLY(color.rgb, vertexOutput.pos, vertexOutput.worldPos, light.dir, light.color)
            }
            else
        #endif
        {
            color = GetLightingForwardBaseStandard(vertexOutput, light, shadowAtten);
            UNITY_APPLY_FOG(vertexOutput.fogCoord, color);
        }

        return color;
    }


    // Obsolete
    // ------------------------------------------------------------------
    // D10 or DX11 Deferred Pass

    //struct VertexOutputDeferred
    //{
    //    UNITY_POSITION(pos);
    //    float4 vertex : INTERNALTESSPOS0;
    //    float4 worldPos : TEXCOORD0;
    //    float heightOffset : TEXCOORD1;
    //    half3 worldNormal : TEXCOORD2;
    //    half3 worldViewDir : TEXCOORD3;
    //    half4 screenPos : TEXCOORD4;
    //};

    //VertexOutputDeferred VertDeferred(VertexInput v)
    //{
    //    VertexOutputDeferred o;
    //    UNITY_INITIALIZE_OUTPUT(VertexOutputDeferred, o);
    //    float4 vertex = v.vertex;
    //    float4 worldPos = mul(unity_ObjectToWorld, vertex);

    //    GetOceanVert(worldPos, vertex);

    //    o.pos = UnityObjectToClipPos(vertex);
    //    o.vertex = v.vertex;
    //    o.worldPos = worldPos;
    //    o.worldNormal = 0;
    //    o.worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
    //    o.screenPos = ComputeScreenPos(o.pos);

    //    return o;
    //}

    //TessellationFactors PatchConstantDeferred(InputPatch<VertexOutputDeferred, 3> patch) 
    //{
    //    OCEAN_PATCH_CONSTANT
    //}

    //[UNITY_domain("tri")]
    //[UNITY_outputcontrolpoints(3)]
    //[UNITY_outputtopology("triangle_cw")]
    //[UNITY_partitioning(LPOCEAN_UNITY_PARTITIONING)]
    //[UNITY_patchconstantfunc("PatchConstantDeferred")]
    //VertexOutputDeferred HullDeferred(InputPatch<VertexOutputDeferred, 3> patch, uint id : SV_OutputControlPointID)
    //{
    //    return patch[id];
    //}

    //[UNITY_domain("tri")]
    //VertexOutputDeferred DomainDeferred(TessellationFactors factors, OutputPatch<VertexOutputDeferred, 3> patch, float3 barycentricCoordinates : SV_DomainLocation)
    //{
    //    VertexInput data;

    //    DOMAIN_PROGRAM_INTERPOLATE(vertex)

    //    return VertDeferred(data);
    //}

    //[maxvertexcount(3)]
    //void GeometryDeferred(triangle VertexOutputDeferred input[3], inout TriangleStream<VertexOutputDeferred> outputStream)
    //{
    //    #define GEOMETRY_DEFERRED(index) input[index].worldNormal = worldNormal;\
    //        outputStream.Append(input[index]);

    //    float3 worldNormal = GetNormal(input[0].worldPos, input[1].worldPos, input[2].worldPos);

    //    GEOMETRY_DEFERRED(0)
    //    GEOMETRY_DEFERRED(1)
    //    GEOMETRY_DEFERRED(2)
    //}

    //void FragDeferred(VertexOutputDeferred i
    //    , out half4 outGBuffer0 : SV_Target0
    //    , out half4 outGBuffer1 : SV_Target1
    //    , out half4 outGBuffer2 : SV_Target2
    //    , out half4 outEmission : SV_Target3
    //    #if LPOCEAN_BACK_LIGHTING
    //    , fixed facing : VFACE
    //    #endif
    //    )
    //{
    //    OceanLightingData oceanData;
    //    oceanData.pos = i.pos;
    //    oceanData.worldPos = i.worldPos;
    //    oceanData.heightOffset = i.heightOffset;
    //    oceanData.worldNormal = i.worldNormal;
    //    oceanData.worldViewDir = i.worldViewDir;
    //    oceanData.screenPos = i.screenPos;
    //    oceanData.light = DummyLight();

    //    SurfaceOutputStandard data;

    //    #if LPOCEAN_BACK_LIGHTING
    //        if(facing < 0)
    //        {
    //            data = GetOceanSurfaceData(oceanData);
    //        }
    //        else
    //    #endif
    //    {
    //        data = GetOceanBackSurfaceData(oceanData);
    //    }

    //    UnityGI gi = FragmentGI(data, oceanData, oceanData.light, 1);
    //    outEmission = LightingStandard_Deferred(data, oceanData.worldViewDir, gi, outGBuffer0, outGBuffer1, outGBuffer2);

    //    #ifndef UNITY_HDR_ON
    //        outEmission.rgb = exp2(-outEmission.rgb);
    //    #endif
    //}


#endif

