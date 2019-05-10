#ifndef LPUNDER_OCEAN_LIGHTING_INCLUDED
#define LPUNDER_OCEAN_LIGHTING_INCLUDED

    #include "UnityCG.cginc"
    #include "LPUnderOceanMark.cginc"
    #include "LPOceanClip.cginc"
    #include "LPOceanFog.cginc"

    #define UNITY_ANY_FOG defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)

    uniform float3 _LPUnderOceanPosition;

    uniform float _LPOceanUnderLightAtten;
    uniform float2 _LPOceanUnderLightAttenClamp;
    uniform float _LPOceanUnderShadowAtten;
    uniform float2 _LPOceanUnderShadowAttenClamp;
    uniform float _LPOceanUnderShadowIntensity;

    uniform float4x4 _LPOceanUnderWorldToCookie;
    uniform sampler2D _LPOceanUnderCookieTexture;
    uniform float _LPOceanUnderCookieAtten;
    uniform float _LPOceanUnderCookieIntensity;
    uniform float _LPOceanUnderCookieInShadowIntensity;

    uniform half3 _LPOceanUnderFogColor;
    uniform half _LPOceanUnderFogClarity;
    uniform float2 _LPOceanUnderFogLightAttenClamp;
    uniform half _LPOceanUnderFogDepthAtten;


    bool UnderOceanClip(half4 screenPos, inout float outCameraDepth)
    {
        half4 col = tex2Dproj(_LPOceanClipTexture, screenPos);
        half clipValue = col.g;

        if(clipValue > 0.1)
        {
            float clipDepth = SAMPLE_DEPTH_TEXTURE_PROJ(_LPOceanClipDepthTexture, UNITY_PROJ_COORD(screenPos));
            clipDepth = LinearEyeDepth(clipDepth);

            if(clipValue < 0.5) //if clip fron
            {
                if(outCameraDepth < clipDepth)
                {
                    outCameraDepth -= clipDepth;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return outCameraDepth > clipDepth;
            }
        }
        else
        {
            return false;
        }
    }

    float GetUnderOceanLightDepth(float oceanHeight, float3 worldPos, half3 lightDir)
    {
        float oceanDepth = oceanHeight - worldPos.y;
        oceanDepth /= dot(float3(0, 1, 0), lightDir);
        return oceanDepth > 0 ? oceanDepth : 0.001;
    }

    float GetUnderOceanLightDepth(float3 worldPos, half3 lightDir)
    {
        return GetUnderOceanLightDepth(_LPUnderOceanPosition.y, worldPos, lightDir);
    }

    void CalculateUnderOceanDirectionalLight(float3 worldPos, half3 lightDir, inout half3 lightColor, inout float shadowAtten)
    {
        float oceanDepth = GetUnderOceanLightDepth(worldPos, lightDir);

        half lightAtten = saturate(_LPOceanUnderLightAtten / oceanDepth);
        lightAtten = clamp(lightAtten, _LPOceanUnderLightAttenClamp.x, _LPOceanUnderLightAttenClamp.y);
        lightColor *= lightAtten;

        shadowAtten = saturate(shadowAtten + (oceanDepth / _LPOceanUnderShadowAtten) + _LPOceanUnderShadowIntensity);
        shadowAtten = clamp(shadowAtten, _LPOceanUnderShadowAttenClamp.x, _LPOceanUnderShadowAttenClamp.y);

        #if LPUNDER_OCEAN_COOKIE
            half cookie = tex2Dbias(_LPOceanUnderCookieTexture, float4(mul(_LPOceanUnderWorldToCookie, half4(worldPos,1)).xy, 0, -8)).a;
            half cookieAtten = saturate(_LPOceanUnderCookieAtten / oceanDepth);
            cookieAtten *= cookieAtten * _LPOceanUnderCookieIntensity;
            cookie *= cookieAtten * pow(shadowAtten, _LPOceanUnderCookieInShadowIntensity);
            lightColor += cookie * lightColor;
        #endif
    }

    void CalculateUnderOceanDirectionalLight(float3 worldPos, float4 screenPos, half3 lightDir, inout half3 lightColor, inout float shadowAtten, float eyeDepth, half4 mark, float markEyeDepth
    #if LPUNDER_OCEAN_CLIP
        , bool ignoreUnderOceanEffect
    #endif
    )
    {
        #if LPUNDER_OCEAN_CLIP
            if(!ignoreUnderOceanEffect)
        #endif
            if(IsUnderOcean(mark))
            {
                CalculateUnderOceanDirectionalLight(worldPos, lightDir, lightColor, shadowAtten);
            }
            else if(IsOceanFron(mark) && (eyeDepth > markEyeDepth))
            {
                CalculateUnderOceanDirectionalLight(worldPos, lightDir, lightColor, shadowAtten);
            }
            else if (IsOceanBack(mark) && (eyeDepth < markEyeDepth))
            {
                CalculateUnderOceanDirectionalLight(worldPos, lightDir, lightColor, shadowAtten);
            }
    }

    half3 GetUnderOceanFog(half3 color, float eyeDepth, half3 fogColor)
    {
        float fogFactor = GetOceanFogFactor(_LPOceanUnderFogClarity, eyeDepth);
        color = lerp(fogColor, color, saturate(fogFactor));
        return color;
    }

    void GetUnderOceanFogForwardBase(inout half3 color, float3 worldPos, float eyeDepth, half3 lightColor, half3 lightDir)
    {
        float cameraLightDepth = GetUnderOceanLightDepth(_WorldSpaceCameraPos, lightDir);
        float lightAttenuation = saturate(_LPOceanUnderFogDepthAtten / cameraLightDepth);
        lightAttenuation = clamp(lightAttenuation, _LPOceanUnderFogLightAttenClamp.x, _LPOceanUnderFogLightAttenClamp.y);

        half3 fogColor = _LPOceanUnderFogColor * lightColor * lightAttenuation.xxx;
        float fogFactor = GetOceanFogFactor(_LPOceanUnderFogClarity, eyeDepth);
        color = lerp(fogColor, color, saturate(fogFactor));
    }

    void CalculateUnderOceanEffectForwardBase(inout half3 color, float3 worldPos, half4 screenPos, float eyeDepth, half3 lightDir, half3 lightColor, half4 mark, float markEyeDepth
    #if UNITY_ANY_FOG
        , float fogCoord
    #endif
    #if LPUNDER_OCEAN_CLIP
        , bool ignoreUnderOceanEffect
    #endif
    )
    {
        #if !LPUNDER_OCEAN_CLIP
            if (IsOceanBack(mark))
            {
                GetUnderOceanFogForwardBase(color, worldPos, min(eyeDepth, markEyeDepth), lightColor, lightDir);
            }
            else if(IsUnderOcean(mark))
            {
                GetUnderOceanFogForwardBase(color, worldPos, eyeDepth, lightColor, lightDir);
            }
        #else
            if(ignoreUnderOceanEffect)
                return;

            if(IsUnderOcean(mark))
            {
                GetUnderOceanFogForwardBase(color, worldPos, eyeDepth, lightColor, lightDir);
            }
            else if(IsOceanBack(mark))
            {
                GetUnderOceanFogForwardBase(color, worldPos, min(eyeDepth, markEyeDepth), lightColor, lightDir);
            }
            else if (IsOceanFron(mark) && markEyeDepth < eyeDepth)
            {
                GetUnderOceanFogForwardBase(color, worldPos, eyeDepth, lightColor, lightDir);
            }
        #endif

        #if UNITY_ANY_FOG
            else
            {
                UNITY_APPLY_FOG(fogCoord, color);
            }
        #endif
    }

    void GetUnderOceanEffectOnlyForwardBase(inout half3 color, float4 pos, float3 worldPos, half3 lightDir, half3 lightColor)
    {
        GetUnderOceanFogForwardBase(color, worldPos, pos.w, lightColor, lightDir);
    }


    #if LPUNDER_OCEAN_CLIP
        #define LPUNDER_OCEAN_CLIP_COORD(pos, screenPos) float eyeDepth = pos.w;\
            bool ignoreUnderOceanEffect = UnderOceanClip(screenPos, eyeDepth);
    #else
        #define LPUNDER_OCEAN_CLIP_COORD(pos, screenPos) float eyeDepth = pos.w;
    #endif


    #define LPUNDER_OCEAN_MARK_COORD(screenPos) half4 mark = tex2Dproj(_LPOceanMarkTexture, screenPos);\
        float markDepth = SAMPLE_DEPTH_TEXTURE_PROJ(_LPOceanMarkDepthTexture, UNITY_PROJ_COORD(screenPos));\
        float markEyeDepth = LinearEyeDepth(markDepth);


    //Calculate light intensity, shadow attenuation and "cookies"
    #if LPUNDER_OCEAN_LIGHTING && LPUNDER_OCEAN_CLIP
        #define APPLY_LPUNDER_OCEAN_LIGHTING(pos, worldPos, screenPos, lightDir, lightColor, shadowAtten) LPUNDER_OCEAN_MARK_COORD(screenPos)\
        LPUNDER_OCEAN_CLIP_COORD(pos, screenPos)\
        CalculateUnderOceanDirectionalLight(worldPos, screenPos, lightDir, lightColor, shadowAtten, eyeDepth, mark, markEyeDepth, ignoreUnderOceanEffect);
    #elif LPUNDER_OCEAN_LIGHTING
        #define APPLY_LPUNDER_OCEAN_LIGHTING(pos, worldPos, screenPos, lightDir, lightColor, shadowAtten) LPUNDER_OCEAN_MARK_COORD(screenPos)\
        LPUNDER_OCEAN_CLIP_COORD(pos, screenPos)\
        CalculateUnderOceanDirectionalLight(worldPos, screenPos, lightDir, lightColor, shadowAtten, eyeDepth, mark, markEyeDepth);
    #else
        #define APPLY_LPUNDER_OCEAN_LIGHTING(pos, worldPos, screenPos, lightDir, lightColor, shadowAtten)
    #endif


    //Add fog effect
    #if UNITY_ANY_FOG && LPUNDER_OCEAN_EFFECT && LPUNDER_OCEAN_LIGHTING && LPUNDER_OCEAN_CLIP
        #define APPLY_LPUNDER_OCEAN_EFFECT(col, fogCoord, screenPos, pos, worldPos, lightDir, lightColor) CalculateUnderOceanEffectForwardBase(col, worldPos, screenPos, eyeDepth, lightDir, lightColor, mark, markEyeDepth, fogCoord, ignoreUnderOceanEffect);
    #elif UNITY_ANY_FOG && LPUNDER_OCEAN_EFFECT && LPUNDER_OCEAN_LIGHTING
        #define APPLY_LPUNDER_OCEAN_EFFECT(col, fogCoord, screenPos, pos, worldPos, lightDir, lightColor) CalculateUnderOceanEffectForwardBase(col, worldPos, screenPos, eyeDepth, lightDir, lightColor, mark, markEyeDepth, fogCoord);
    #elif UNITY_ANY_FOG && LPUNDER_OCEAN_EFFECT && LPUNDER_OCEAN_CLIP
        #define APPLY_LPUNDER_OCEAN_EFFECT(col, fogCoord, screenPos, pos, worldPos, lightDir, lightColor) LPUNDER_OCEAN_MARK_COORD(screenPos)\
          LPUNDER_OCEAN_CLIP_COORD(pos, screenPos)\
          CalculateUnderOceanEffectForwardBase(col, worldPos, screenPos, eyeDepth, lightDir, lightColor, mark, markEyeDepth, fogCoord, ignoreUnderOceanEffect);
    #elif UNITY_ANY_FOG && LPUNDER_OCEAN_EFFECT
        #define APPLY_LPUNDER_OCEAN_EFFECT(col, fogCoord, screenPos, pos, worldPos, lightDir, lightColor) LPUNDER_OCEAN_MARK_COORD(screenPos)\
          LPUNDER_OCEAN_CLIP_COORD(pos, screenPos)\
          CalculateUnderOceanEffectForwardBase(col, worldPos, screenPos, eyeDepth, lightDir, lightColor, mark, markEyeDepth, fogCoord);
    #elif UNITY_ANY_FOG
        #define APPLY_LPUNDER_OCEAN_EFFECT(col, fogCoord, screenPos, pos, worldPos, lightDir, lightColor) UNITY_APPLY_FOG(fogCoord, col);
    #elif LPUNDER_OCEAN_EFFECT && LPUNDER_OCEAN_LIGHTING && LPUNDER_OCEAN_CLIP
        #define APPLY_LPUNDER_OCEAN_EFFECT(col, fogCoord, screenPos, pos, worldPos, lightDir, lightColor) CalculateUnderOceanEffectForwardBase(col, worldPos, screenPos, eyeDepth, lightDir, lightColor, mark, markEyeDepth, ignoreUnderOceanEffect);
    #elif LPUNDER_OCEAN_EFFECT && LPUNDER_OCEAN_LIGHTING
        #define APPLY_LPUNDER_OCEAN_EFFECT(col, fogCoord, screenPos, pos, worldPos, lightDir, lightColor) CalculateUnderOceanEffectForwardBase(col, worldPos, screenPos, eyeDepth, lightDir, lightColor, mark, markEyeDepth);
    #elif LPUNDER_OCEAN_EFFECT && LPUNDER_OCEAN_CLIP
        #define APPLY_LPUNDER_OCEAN_EFFECT(col, fogCoord, screenPos, pos, worldPos, lightDir, lightColor) LPUNDER_OCEAN_MARK_COORD(screenPos)\
            LPUNDER_OCEAN_CLIP_COORD(pos, screenPos)\
            CalculateUnderOceanEffectForwardBase(col, worldPos, screenPos, eyeDepth, lightDir, lightColor, mark, markEyeDepth, ignoreUnderOceanEffect);
    #elif LPUNDER_OCEAN_EFFECT
        #define APPLY_LPUNDER_OCEAN_EFFECT(col, fogCoord, screenPos, pos, worldPos, lightDir, lightColor) LPUNDER_OCEAN_MARK_COORD(screenPos)\
            LPUNDER_OCEAN_CLIP_COORD(pos, screenPos)\
            CalculateUnderOceanEffectForwardBase(col, worldPos, screenPos, eyeDepth, lightDir, lightColor, mark, markEyeDepth);
    #else
        #define APPLY_LPUNDER_OCEAN_EFFECT(col, fogCoord, screenPos, pos, worldPos, lightDir, lightColor)
    #endif


    //Add fog effect only
    #if LPUNDER_OCEAN_EFFECT
        #define APPLY_LPUNDER_OCEAN_EFFECT_ONLY(col, pos, worldPos, lightDir, lightColor) GetUnderOceanFogForwardBase(col, worldPos, pos.w, lightColor, lightDir);
    #else
        #define APPLY_LPUNDER_OCEAN_EFFECT_ONLY(col, pos, worldPos, lightDir, lightColor)
    #endif

#endif