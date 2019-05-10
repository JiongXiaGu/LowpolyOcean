#ifndef LPOCEAN_HELPER_INCLUDED
#define LPOCEAN_HELPER_INCLUDED

    #include "UnityCG.cginc"
    #include "UnityShaderUtilities.cginc"

    #define PI            3.14159265358979323846
    #define RadianToAngle 57.2957795130823208768
    #define AngleToRadian 0.01745329251994329576

    uniform float _LPOceanTime;



    bool IsMarked(half target, half mark)
    {
        #define INTERVAL 0.01
        float min = target - INTERVAL;
        float max = target + INTERVAL;
        return min < mark.x && max > mark.x;
    }

    float3 GetNormal(float3 a, float3 b, float3 c)
    {
        float3 normal = cross(b - a, c - a);
        normal = normalize(normal);
        return normal;
    }


    float2 Rotate(float2 localPos, float radian)
    {
        radian = atan2(localPos.x, localPos.y) - radian;
        float dis = sqrt(localPos.x * localPos.x + localPos.y * localPos.y);

        localPos.x = dis * sin(radian);
        localPos.y = dis * cos(radian);

        return localPos;
    }

    float2 Rotate(float2 pos, float2 origin, float radian)
    {
        float2 localPos = pos - origin;
        return Rotate(localPos, radian) + origin;
    }

    float2 GetUV(float2 localPos, float4 textureRect)
    {
        localPos += textureRect.zw * textureRect.xy;
        float2 uv = localPos / textureRect.zw;
        return uv;
    }

    float2 GetUV(float2 pos, float2 origin, float4 textureRect)
    {
        float2 localPos = pos - origin;
        return GetUV(localPos, textureRect);
    }

    fixed UVClamp(float2 uv)
    {
        return step(uv.x, 1) * step(0, uv.x) * step(uv.y, 1) * step(0, uv.y);
    }

    float2 GetWorldUV(float2 pos, half radian, half4 textureRect)
    {
        pos = Rotate(pos, radian);
        return GetUV(pos, textureRect);
    }

    float2 GetWorldUV(float2 pos, float2 origin, half radian, half4 textureRect)
    {
        pos = Rotate(pos, origin, radian);
        return GetUV(pos, origin, textureRect);
    }


    //Copy from "UnityCG.cginc"
    // Encoding/decoding [0..1) floats into 8 bit/channel RG. Note that 1.0 will not be encoded properly.
    float2 OceanEncodeFloatRG(float v)
    {
        float2 kEncodeMul = float2(1.0, 255.0);
        float kEncodeBit = 1.0 / 255.0;
        float2 enc = kEncodeMul * v;
        enc = frac (enc);
        enc.x -= enc.y * kEncodeBit;
        return enc;
    }

    float OceanDecodeFloatRG(float2 enc)
    {
        float2 kDecodeDot = float2(1.0, 1.0 / 255.0);
        return dot(enc, kDecodeDot);
    }


    //Change From "UnityShaderUtilities.cginc"
    inline float4 WorldPosToClipPosODS(float3 worldPos)
    {
        float4 clipPos;
        #if defined(STEREO_CUBEMAP_RENDER_ON)
            float3 offset = ODSOffset(worldPos, unity_HalfStereoSeparation.x);
            clipPos = mul(UNITY_MATRIX_VP, float4(worldPos + offset, 1.0));
        #else
            clipPos = mul(UNITY_MATRIX_VP, float4(worldPos, 1.0));
        #endif
        return clipPos;
    }

    // Tranforms position from object to homogenous space
    inline float4 WorldPosToClipPos(in float3 worldPos)
    {
    #if defined(STEREO_CUBEMAP_RENDER_ON)
        return WorldPosToClipPosODS(worldPos);
    #else
        // More efficient than computing M*VP matrix product
        return mul(UNITY_MATRIX_VP, mul(unity_ObjectToWorld, float4(worldPos, 1.0)));
    #endif
    }
    inline float4 WorldPosToClipPos(float4 worldPos) // overload for float4; avoids "implicit truncation" warning for existing shaders
    {
        return WorldPosToClipPos(worldPos.xyz);
    }


#endif