#ifndef LPOCEAN_VERTEX_INCLUDED
#define LPOCEAN_VERTEX_INCLUDED

    #include "LPOceanHelper.cginc"

    // ------------------------------------------------------------------
    // Wave

    sampler2D _OceanWaveTexture;
    half4 _OceanWaveRect0;
    half4 _OceanWaveRect1;
    half4 _OceanWaveRect2;
    half4 _OceanWaveRect3;
    float4 _OceanWaveRadian;
    float _OceanWaveUniformRadian;
    half4 _OceanWaveHeightPow;
    half4 _OceanWaveHeightScale;
    half4 _OceanWaveSpeedZ;

    float2 GetWorldWaveUV(float2 pos, half radian, half4 textureRect)
    {
        pos = Rotate(pos, radian);
        return GetUV(pos, textureRect);
    }

    #define LPOCEAN_ANY_WAVE LPOCEAN_WAVE1 || LPOCEAN_WAVE2 || LPOCEAN_WAVE3

    #define LPOCEAN_WAVE_FROM1(colorPass) tex2Dlod(_OceanWaveTexture, float4(uv, 0, 0)).##colorPass

    #define LPOCEAN_WAVE_FROM2(colorPass0, colorPass1) OceanDecodeFloatRG(float2(LPOCEAN_WAVE_FROM1(colorPass0), LPOCEAN_WAVE_FROM1(colorPass1)))

    #define LPOCEAN_WAVE_HORIZONTAL_OFFSET(pass, index, colorFrom) uv = GetWorldWaveUV(worldPos.xz, _OceanWaveRadian.##pass + _OceanWaveUniformRadian, _OceanWaveRect##index);\
        uv.y -= _OceanWaveSpeedZ.##pass * _LPOceanTime;\
        tempValue = colorFrom;\
        worldPos.xz += float2(_OceanWaveHeightPow.##pass, _OceanWaveHeightScale.##pass) * tempValue;

    #define LPOCEAN_WAVE_HEIGHT(pass, index, colorFrom) uv = GetWorldWaveUV(worldPos.xz, _OceanWaveRadian.##pass + _OceanWaveUniformRadian, _OceanWaveRect##index);\
        uv.y -= _OceanWaveSpeedZ.##pass * _LPOceanTime;\
        tempValue = colorFrom;\
        tempValue = pow(tempValue, _OceanWaveHeightPow.##pass) * _OceanWaveHeightScale.##pass;\
        height += tempValue;
    
    float3 GetOceanWaveOffset(float3 worldPos, inout float height)
    {
        float2 uv;
        float tempValue;

        #if LPOCEAN_WAVE1
            LPOCEAN_WAVE_HORIZONTAL_OFFSET(a, 3, LPOCEAN_WAVE_FROM2(b, a))
            LPOCEAN_WAVE_HEIGHT(b, 2, LPOCEAN_WAVE_FROM2(r, g))
        #elif LPOCEAN_WAVE2
            LPOCEAN_WAVE_HEIGHT(r, 0, LPOCEAN_WAVE_FROM2(r, g))
            LPOCEAN_WAVE_HEIGHT(g, 1, LPOCEAN_WAVE_FROM2(b, a))
        #elif LPOCEAN_WAVE3
            LPOCEAN_WAVE_HORIZONTAL_OFFSET(a, 3, LPOCEAN_WAVE_FROM1(a))
            LPOCEAN_WAVE_HEIGHT(r, 0, LPOCEAN_WAVE_FROM1(r))
            LPOCEAN_WAVE_HEIGHT(g, 1, LPOCEAN_WAVE_FROM1(g))
            LPOCEAN_WAVE_HEIGHT(b, 2, LPOCEAN_WAVE_FROM1(b))
        #endif

        worldPos.y += height;
        return worldPos;
    }

    // ------------------------------------------------------------------
    // Ripple

    #define RIPPLE_COUNT 4

    uniform sampler2D _OceanRippleTexture0;
    uniform sampler2D _OceanRippleTexture1;
    uniform sampler2D _OceanRippleTexture2;
    uniform sampler2D _OceanRippleTexture3;
    uniform half4 _OceanRippleRect[RIPPLE_COUNT];
    uniform float4 _OceanRipplePosition[RIPPLE_COUNT];
    uniform half4 _OceanRippleHeightScale[RIPPLE_COUNT];

    float2 GetWorldRippleUV(float2 pos, float2 origin, half radian, half4 textureRect)
    {
        pos = Rotate(pos, origin, radian);
        return GetUV(pos, origin, textureRect);
    }

    half GetRippleHeight(half4 color, half4 heightScale)
    {
        half height = 0;

        #define RIPPLE_HEIGHT_COLOR(pass) height += (color.##pass - 0.5) * heightScale.##pass;

        RIPPLE_HEIGHT_COLOR(r)
        RIPPLE_HEIGHT_COLOR(g)
        RIPPLE_HEIGHT_COLOR(b)
        RIPPLE_HEIGHT_COLOR(a)

        return height;
    }

    half GetOceanRippleHeight(float3 worldPos)
    {
        half4 color;
        float2 uv;
        half height = 0;
        half4 heightScale;

        #define RIPPLE_HEIGHT(pass, index) uv = GetWorldRippleUV(worldPos.xz, _OceanRipplePosition[##index].xz, _OceanRipplePosition[##index].a, _OceanRippleRect[##index]);\
            color = tex2Dlod(_OceanRippleTexture##index, float4(uv, 0, 0));\
            heightScale = _OceanRippleHeightScale[##index];\
            heightScale *= UVClamp(uv);\
            height += GetRippleHeight(color, heightScale);

        RIPPLE_HEIGHT(r, 0)
        RIPPLE_HEIGHT(g, 1)
        RIPPLE_HEIGHT(b, 2)
        RIPPLE_HEIGHT(a, 3)

        return height;
    }

    // ------------------------------------------------------------------
    // Core

    void GetOceanVert(inout float3 worldPos)
    {
        #if LPOCEAN_ANY_WAVE || LPOCEAN_RIPPLE

            #if LPOCEAN_ANY_WAVE
                float height = 0;
                worldPos = GetOceanWaveOffset(worldPos, height);
            #endif

            #if LPOCEAN_RIPPLE
                worldPos.y += GetOceanRippleHeight(worldPos.xyz);
            #endif

        #endif
    }

    void GetOceanVert(inout float4 worldPos, inout float4 vertex)
    {
        #if LPOCEAN_ANY_WAVE || LPOCEAN_RIPPLE

            #if LPOCEAN_ANY_WAVE
                worldPos.w = 0;
                worldPos.xyz = GetOceanWaveOffset(worldPos.xyz, worldPos.w);
            #endif

            #if LPOCEAN_RIPPLE
                worldPos.y += GetOceanRippleHeight(worldPos.xyz);
            #endif

            vertex = mul(unity_WorldToObject, float4(worldPos.xyz, 1));
        #endif
    }

#endif