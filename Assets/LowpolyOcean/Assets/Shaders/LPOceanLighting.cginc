#ifndef LPOCEAN_LIGHTING_INCLUDED
#define LPOCEAN_LIGHTING_INCLUDED

    #include "UnityCG.cginc"
    #include "AutoLight.cginc"
    #include "LPOceanHelper.cginc"
    #include "LPOceanFog.cginc"
    #include "Lighting.cginc"
    #include "UnityPBSLighting.cginc"
    #include "UnityStandardBRDF.cginc"
    #include "LPOceanClip.cginc"
    #include "LPUnderOceanMark.cginc"

    #define LPOCEAN_ANY_DEPTH_EFFECT LPOCEAN_CLIP || LPOCEAN_REFRACTION || LPOCEAN_REFRACTION_FULL || LPOCEAN_FOAM_SIMPLE_EYE_DEPTH

    //Global
    uniform sampler2D _LPOceanRefractionTexture;
    uniform sampler2D _LPOceanRefractionDepthTexture;
    uniform sampler2D _CameraDepthTexture;

    #if LPOCEAN_TRANSPARENT
        #define LPOCEAN_REFRACTION_DEPTH_TEXTURE _CameraDepthTexture
    #else
        #define LPOCEAN_REFRACTION_DEPTH_TEXTURE _LPOceanRefractionDepthTexture
    #endif

    uniform sampler2D _LPOceanReflectionTexture;

    uniform float4 _LPOceanSunLight;
    uniform half3 _LPOceanSunLightColor;


    //Artistic Lighting
    half3 _OceanArtisticColor;
    half _OceanArtisticSpecularPow;
    half3 _OceanArtisticSpecularColor;
    sampler2D _OceanArtisticFresnelTexture;
    half _OceanArtisticFresnePow;
    half3 _OceanArtisticFresnelColor;
    half _OceanArtisticShadowAttenuation;

    //Back Lighting
    half3 _OceanBackAlbedo;
    half _OceanBackSpecular;
    half _OceanBackSmoothness;
    half _OceanBackOcclusion;
    half3 _OceanBackEmission;
    half _OceanBackFresneIntensity;
    sampler2D _OceanBackFresnelTexture;
    half3 _OceanBackFresnelColor;
    half _OceanBackShadowAttenuation;


    // ------------------------------------------------------------------
    // Artistic Lighting

    //Lighting
    half3 _OceanAlbedo;
    half4 _OceanSpecular;
    half _OceanSmoothness;
    half _OceanOcclusion;
    half _OceanShadowAttenuation;
    sampler2D _OceanFresnelTexture;
    half _OceanFresnelIntensity;
    half3 _OceanFresnelColor;

    half3 OceanArtisticLighting(half3 color, half3 worldNormal, half3 worldViewDir, float shadowAtten)
    {
        half3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
        half3 lightColor = _LightColor0.rgb;

        half3 diffuse = max(0, dot(worldNormal, lightDir)) * lightColor;
        half3 ambient = saturate(ShadeSH9(half4(worldNormal, 1.0)));
        half nh = max(0, dot (worldNormal, normalize(lightDir + worldViewDir)));
        half3 specular = pow(nh, _OceanArtisticSpecularPow) * _OceanArtisticSpecularColor * lightColor;

        color.rgb *= diffuse + ambient;
        color.rgb += specular;
        color.rgb *= saturate(shadowAtten + _OceanArtisticShadowAttenuation);

        return color;
    }

    //Back Artistic Lighting
    half3 _OceanBackArtisticColor;
    half _OceanBackArtisticSpecularPow;
    half3 _OceanBackArtisticSpecularColor;
    sampler2D _OceanBackArtisticFresnelTexture;
    half _OceanBackArtisticFresneIntensity;
    half3 _OceanBackArtisticFresneColor;
    half _OceanBackArtisticShadowAttenuation;

    half3 OceanBackArtisticLighting(half3 color, half3 worldNormal, half3 worldViewDir, float shadowAtten)
    {
        half3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
        half3 lightColor = _LightColor0.rgb;

        half3 diffuse = max(0, dot(worldNormal, lightDir)) * lightColor;
        half3 ambient = saturate(ShadeSH9(half4(worldNormal, 1.0)));
        half nh = max(0, dot(lightDir, -worldViewDir));
        half3 specular = pow(nh, _OceanBackArtisticSpecularPow) * _OceanBackArtisticSpecularColor * lightColor;

        half fresnel = tex2D(_OceanBackArtisticFresnelTexture, half4(max(0, dot(worldViewDir, -worldNormal)), 0.5, 0, 0)).a;
        fresnel = saturate(fresnel * _OceanBackArtisticFresneIntensity);
        diffuse = lerp(diffuse, _OceanBackArtisticFresneColor, fresnel);

        color.rgb *= diffuse + ambient;
        color.rgb += specular;
        color.rgb *= saturate(shadowAtten + _OceanBackArtisticShadowAttenuation);

        return color;
    }


    // ------------------------------------------------------------------
    // Fresne

    half3 Fresnel(half3 albedo, half3 worldNormal, half3 worldViewDir)
    {
        half fresnel = tex2D(_OceanFresnelTexture, half4(max(0, dot(worldViewDir, worldNormal)), 0.5, 0, 0)).a;
        fresnel = saturate(fresnel * _OceanFresnelIntensity);
        albedo = lerp(albedo, _OceanFresnelColor, fresnel);
        return albedo;
    }

    half3 OceanBackFresnel(half3 albedo, half3 worldNormal, half3 worldViewDir)
    {
        half fresnel = tex2Dlod(_OceanBackFresnelTexture, half4(saturate(dot(worldViewDir, -worldNormal)), 0.5, 0, 0)).a;
        fresnel = saturate(fresnel * _OceanBackFresneIntensity);
        albedo = lerp(albedo, _OceanBackFresnelColor, fresnel);
        return albedo;
    }


    // ------------------------------------------------------------------
    // Depth Effect

    void GetOceanDepth(float4 pos, float4 screenPos, out float cameraDepth, out float oceanDepth, out float refractionDepth)
    {
        cameraDepth = pos.w;
        refractionDepth = SAMPLE_DEPTH_TEXTURE_PROJ(LPOCEAN_REFRACTION_DEPTH_TEXTURE, UNITY_PROJ_COORD(screenPos));
        refractionDepth = LinearEyeDepth(refractionDepth);
        oceanDepth = refractionDepth - cameraDepth;
    }

    // ------------------------------------------------------------------
    // Cookie

    sampler2D _OceanCookieTexture;
    half3 _OceanCookieColor;
    half _OceanCookieDepthAtten;
    half _OceanCookieDepthAttenOffset;
    half _OceanCookieLightAtten;
    half _OceanCookieIntensity;
    float4x4 _OceanWorldToCookie;

    half3 AppleRefractionCookieAddition(half3 color, float3 worldPos, half3 worldViewDir, float oceanDepth)
    {
        float3 cookieWorldPos = worldPos - worldViewDir * oceanDepth;
        half cookie = tex2Dbias(_OceanCookieTexture, float4(mul(_OceanWorldToCookie, half4(cookieWorldPos,1)).xy, 0, -8)).a;
        half3 cookieColor = cookie * _LPOceanSunLightColor * _OceanCookieColor;
        float depth = worldPos.y - cookieWorldPos.y;
        depth += _OceanCookieDepthAttenOffset;
        depth = abs(depth);
        float factor = saturate(_OceanCookieDepthAtten / depth);
        cookieColor *= _OceanCookieIntensity * factor;
        cookieColor *= pow(_LPOceanSunLight.w, _OceanCookieLightAtten);
        color += cookieColor;
        return color;
    }

    #if LPOCEAN_COOKIE
        #define LPOCEAN_COOKIE_COORD(id)
        #define LPOCEAN_COOKIE_TRANSFER(output)
        #define LPOCEAN_COOKIE_APPLY(color, worldPos, worldViewDir, oceanDepth) color = AppleRefractionCookieAddition(color, worldPos, worldViewDir, oceanDepth);
    #else
        #define LPOCEAN_COOKIE_COORD(id)
        #define LPOCEAN_COOKIE_TRANSFER(output)
        #define LPOCEAN_COOKIE_APPLY(color, worldPos, worldViewDir, oceanDepth)
    #endif


    // ------------------------------------------------------------------
    // Refraction
   
    half _OceanRefractionClarity;
    half4 _OceanRefractionDeepColor;
    half _OceanRefractionOffset;

    bool IsRefraction(float4 uv)
    {
        uv = UNITY_PROJ_COORD(uv);
        float rawDepth = SAMPLE_DEPTH_TEXTURE_PROJ(LPOCEAN_REFRACTION_DEPTH_TEXTURE, uv);
        float eyeDepth = LinearEyeDepth(rawDepth);

        rawDepth = SAMPLE_DEPTH_TEXTURE_PROJ(_LPOceanMarkDepthTexture, uv);
        float oceanDepth = LinearEyeDepth(rawDepth);

        return eyeDepth > oceanDepth;
    }

    bool IsRefractionSample(float4 screenPos)
    {
        float4 screenSize = _ScreenParams;
        screenSize.zw = (screenSize.zw  - 1) * 4;

        float4 uv = screenPos;
        uv.xy = screenPos.xy + screenSize.zw;
        if(!IsRefraction(uv))
        {
            return false;        
        }

        uv.xy = screenPos.xy - screenSize.zw;
        if(!IsRefraction(uv))
        {
            return false;        
        }

        uv.x = screenPos.x + screenSize.z;
        uv.y = screenPos.y - screenSize.w;
        if(!IsRefraction(uv))
        {
            return false;        
        }

        uv.x = screenPos.x - screenSize.z;
        uv.y = screenPos.y + screenSize.w;
        if(!IsRefraction(uv))
        {
            return false;        
        }

        return true;
    }

    half3 Refraction(half3 col, float3 worldPos, half3 worldViewDir, float4 refractionUV, float oceanDepth)
    {
        half3 refractionColor = tex2Dproj(_LPOceanRefractionTexture, refractionUV).rgb;

        LPOCEAN_COOKIE_APPLY(refractionColor, worldPos, worldViewDir, oceanDepth)

        float fogFactor = GetOceanFogFactor(_OceanRefractionClarity, oceanDepth);
        col = lerp(col, _OceanRefractionDeepColor.rgb, (1 - fogFactor) * _OceanRefractionDeepColor.a);
        col = lerp(col, refractionColor, fogFactor);

        return col;
    }

    half3 RefractionFull(half3 col, float3 worldPos, half3 worldViewDir, half4 screenPos, half3 worldNormal, float oceanDepth)
    {
        float4 refractionUV = screenPos;
        refractionUV.xy -= worldNormal.xz * _OceanRefractionOffset;

        float refractionDepth = SAMPLE_DEPTH_TEXTURE_PROJ(LPOCEAN_REFRACTION_DEPTH_TEXTURE, UNITY_PROJ_COORD(refractionUV));
        refractionDepth = LinearEyeDepth(refractionDepth);

        float depth = SAMPLE_DEPTH_TEXTURE_PROJ(_LPOceanMarkDepthTexture, UNITY_PROJ_COORD(refractionUV));
        depth = LinearEyeDepth(depth);

        if(refractionDepth > depth && IsRefractionSample(refractionUV))
        {
            oceanDepth = refractionDepth - depth;
        }
        else
        {
            refractionUV = screenPos;
        }

        return Refraction(col, worldPos, worldViewDir, refractionUV, oceanDepth);
    }

    #if LPOCEAN_REFRACTION
        #define LPOCEAN_REFRACTION_APPLY(color, data, oceanDepth) color = Refraction(color, data.worldPos, data.worldViewDir, data.screenPos, oceanDepth);
    #elif LPOCEAN_REFRACTION_FULL
        #define LPOCEAN_REFRACTION_APPLY(color, data, oceanDepth) color = RefractionFull(color, data.worldPos, data.worldViewDir, data.screenPos, data.worldNormal, oceanDepth);
    #else
        #define LPOCEAN_REFRACTION_APPLY(color, data, oceanDepth)
    #endif


    half _OceanBackRefractionClarity;
    half _OceanBackRefractionOffset;

    half3 BackRefraction(half3 col, float4 refractionUV, float oceanDepth)
    {
        half3 refractionColor = tex2Dproj(_LPOceanRefractionTexture, refractionUV).rgb;
        col = lerp(col, refractionColor, _OceanBackRefractionClarity);
        return col;
    }

    half3 BackRefractionFull(half3 col, half4 screenPos, half3 worldNormal, float oceanDepth)
    {
        float4 refractionUV = screenPos;
        refractionUV.xy -= worldNormal.xz * _OceanBackRefractionOffset;

        float rawDepth = SAMPLE_DEPTH_TEXTURE_PROJ(LPOCEAN_REFRACTION_DEPTH_TEXTURE, UNITY_PROJ_COORD(refractionUV));
        float refractionDepth = LinearEyeDepth(rawDepth);

        rawDepth = SAMPLE_DEPTH_TEXTURE_PROJ(_LPOceanMarkDepthTexture, UNITY_PROJ_COORD(refractionUV));
        float depth = LinearEyeDepth(rawDepth);

        if(refractionDepth > depth && IsRefractionSample(refractionUV))
        {
            oceanDepth = refractionDepth - depth;
        }
        else
        {
            refractionUV = screenPos;
        }

        return BackRefraction(col, refractionUV, oceanDepth);
    }

    #if LPOCEAN_BACK_REFRACTION
        #define LPOCEAN_BACK_REFRACTION_APPLY(color, data, oceanDepth) color = BackRefraction(color, data.screenPos, oceanDepth);
    #elif LPOCEAN_BACK_REFRACTION_FULL
        #define LPOCEAN_BACK_REFRACTION_APPLY(color, data, oceanDepth) color = BackRefractionFull(color, data.screenPos,  data.worldNormal, oceanDepth);
    #else
        #define LPOCEAN_BACK_REFRACTION_APPLY(color, data, oceanDepth)
    #endif


    // ------------------------------------------------------------------
    // Ocean Foam

    half4 _OceanFoamColor;
    half _OceanFoamIntensity;
    half _OceanFoamRange;
    sampler2D _OceanFoamTexture;
    half4 _OceanFoamSpeed;
    half4 _OceanFoamRect;

    half3 ApplyFoamSimpleEyeDepth(half3 color, float oceanDepth)
    {
        float factor = smoothstep(_OceanFoamRange, 0, oceanDepth) * _OceanFoamColor.a;
        factor *= _OceanFoamIntensity;

        color = lerp(color, _OceanFoamColor.rgb, factor);
        return color;
    }

    float2 GetFoamUV(float3 worldPos)
    {
        return GetUV(worldPos.xz, _OceanFoamRect);
    }

    half3 ApplyFoamEyeDepth(half3 color, float2 uv, float oceanDepth)
    {
        float factor = smoothstep(_OceanFoamRange, 0, oceanDepth) * _OceanFoamColor.a;
        factor *= tex2D(_OceanFoamTexture, float4(uv, 0, 1)).r;
        factor *= _OceanFoamIntensity;

        color = lerp(color, _OceanFoamColor.rgb, factor);
        return color;
    }


    #if LPOCEAN_FOAM_SHPERE8
        uniform float4 _OceanFoamShpereData0[8]; // xyz : postion, w : radius
        uniform float4 _OceanFoamShpereData1[2]; // intensity
    #elif LPOCEAN_FOAM_SHPERE16
        uniform float4 _OceanFoamShpereData0[16]; // xyz : postion, w : radius
        uniform float4 _OceanFoamShpereData1[4]; // intensity
    #endif

    #define LPOCEAN_FOAM_POINT_CALCULATE(index0, index1, pass) dis = distance(worldPos, _OceanFoamShpereData0[##index0].xyz);\
        foamFactor += saturate(smoothstep(_OceanFoamShpereData0[##index0].w, 0, dis) * _OceanFoamShpereData1[##index1].##pass);


    uniform sampler2D _OceanFoamAreaTexture0;
    uniform half4 _OceanFoamAreaRect0;
    uniform float4 _OceanFoamAreaPosition0; // xyz : position, w : intensity

    #define LPOCEAN_FOAM_AREA_UV(index, worldPos) GetUV(worldPos.xz, _OceanFoamAreaPosition##index.xz, _OceanFoamAreaRect##index)
    #define LPOCEAN_FOAM_AREA_CALCULATE(index, uv) foamFactor += tex2D(_OceanFoamAreaTexture##index, uv) * _OceanFoamAreaPosition##index.w * UVClamp(uv);

    half GetFoamIntensity(float3 worldPos, float2 foamUV
    #if LPOCEAN_FOAM_AREA1
        , float2 areaUV0
    #endif
    )
    {
        float foamFactor = 0;
        float dis;

        #if LPOCEAN_FOAM_SHPERE8 || LPOCEAN_FOAM_SHPERE16
            LPOCEAN_FOAM_POINT_CALCULATE(0, 0, x)
            LPOCEAN_FOAM_POINT_CALCULATE(1, 0, y)
            LPOCEAN_FOAM_POINT_CALCULATE(2, 0, z)
            LPOCEAN_FOAM_POINT_CALCULATE(3, 0, w)

            LPOCEAN_FOAM_POINT_CALCULATE(4, 1, x)
            LPOCEAN_FOAM_POINT_CALCULATE(5, 1, y)
            LPOCEAN_FOAM_POINT_CALCULATE(6, 1, z)
            LPOCEAN_FOAM_POINT_CALCULATE(7, 1, w)
        #endif

        #if LPOCEAN_FOAM_SHPERE16
            LPOCEAN_FOAM_POINT_CALCULATE(8, 2, x)
            LPOCEAN_FOAM_POINT_CALCULATE(9, 2, y)
            LPOCEAN_FOAM_POINT_CALCULATE(10, 2, z)
            LPOCEAN_FOAM_POINT_CALCULATE(11, 2, w)

            LPOCEAN_FOAM_POINT_CALCULATE(12, 3, x)
            LPOCEAN_FOAM_POINT_CALCULATE(13, 3, y)
            LPOCEAN_FOAM_POINT_CALCULATE(14, 3, z)
            LPOCEAN_FOAM_POINT_CALCULATE(15, 3, w)
        #endif

        #if LPOCEAN_FOAM_AREA1
            LPOCEAN_FOAM_AREA_CALCULATE(0, areaUV0)
        #endif

        float2 uv0 = foamUV.xy + _OceanFoamSpeed.xy * _LPOceanTime;
        float holdFactor = tex2D(_OceanFoamTexture, float4(uv0, 0, 1)).r;

        float2 uv1 = foamUV.xy + _OceanFoamSpeed.zw * _LPOceanTime;
        holdFactor *= tex2D(_OceanFoamTexture, float4(uv1, 0, 1)).r;

        holdFactor = pow(holdFactor, (1 - foamFactor) * _OceanFoamRange);
        foamFactor = saturate(foamFactor * _OceanFoamIntensity * holdFactor);

        return foamFactor;
    }

    half3 ApplyFoamColor(half3 color, float3 worldPos, float2 foamUV
    #if LPOCEAN_FOAM_AREA1
        , float2 areaUV0
    #endif
    )
    {
        float foamFactor = GetFoamIntensity(worldPos, foamUV
        #if LPOCEAN_FOAM_AREA1
        , areaUV0
        #endif
        );
        color = lerp(color, _OceanFoamColor.rgb, foamFactor);
        return color;
    }

    #if LPOCEAN_FOAM_SIMPLE_EYE_DEPTH
        #define LPOCEAN_FOAM_COORD(id0, id1)
        #define LPOCEAN_FOAM_TRANSFER(data)
        #define LPOCEAN_FOAM_APPLY(color, data, oceanDepth) color = ApplyFoamSimpleEyeDepth(color, oceanDepth);
    #elif LPOCEAN_FOAM_EYE_DEPTH
        #define LPOCEAN_FOAM_COORD(id0, id1) float2 foamUV : TEXCOORD##id0;
        #define LPOCEAN_FOAM_TRANSFER(data) data.foamUV = GetFoamUV(data.worldPos);
        #define LPOCEAN_FOAM_APPLY(color, data, oceanDepth) color = ApplyFoamEyeDepth(color, data.foamUV, oceanDepth);
    #elif LPOCEAN_FOAM_AREA1
        #define LPOCEAN_FOAM_COORD(id0, id1) float4 foamUV : TEXCOORD##id0;
        #define LPOCEAN_FOAM_TRANSFER(data) data.foamUV.xy = GetFoamUV(data.worldPos); data.foamUV.zw = LPOCEAN_FOAM_AREA_UV(0, data.worldPos);
        #define LPOCEAN_FOAM_APPLY(color, data, oceanDepth) color = ApplyFoamColor(color, data.worldPos, data.foamUV.xy, data.foamUV.zw);
    #elif LPOCEAN_FOAM_SHPERE8 || LPOCEAN_FOAM_SHPERE16
        #define LPOCEAN_FOAM_COORD(id0, id1) float2 foamUV : TEXCOORD##id0;
        #define LPOCEAN_FOAM_TRANSFER(data) data.foamUV = GetFoamUV(data.worldPos);
        #define LPOCEAN_FOAM_APPLY(color, data, oceanDepth) color = ApplyFoamColor(color, data.worldPos, data.foamUV);
    #else
        #define LPOCEAN_FOAM_COORD(id0, id1)
        #define LPOCEAN_FOAM_TRANSFER(data)
        #define LPOCEAN_FOAM_APPLY(color, data, oceanDepth)
    #endif


    // ------------------------------------------------------------------
    // Reflection

    float _OceanReflectionHeight;
    half _OceanReflectionIntensity;
    half _OceanReflectionOffset;

    half4 GetOceanReflectionUV(half4 screenPos, float heightOffset, half3 worldNormal)
    {
        half4 reflectionUV = screenPos;
        reflectionUV.y -= heightOffset + _OceanReflectionHeight;

        reflectionUV.xy -= worldNormal.xz * _OceanReflectionOffset;
        reflectionUV = UNITY_PROJ_COORD(reflectionUV);

        return reflectionUV;
    }

    half3 ApplyOceanReflection(half3 color, half4 reflectionUV)
    {
        half3 reflectionColor = tex2Dproj(_LPOceanReflectionTexture, reflectionUV).rgb;
        color = lerp(color, reflectionColor, _OceanReflectionIntensity);
        return color;
    }

    #if LPOCEAN_REFLECTION
        #define LPOCEAN_REFLECTION_COORD(id) half4 reflectionUV : TEXCOORD##id;
        #define LPOCEAN_REFLECTION_TRANSFER(output) output.reflectionUV = GetOceanReflectionUV(output.screenPos, output.worldPos.w, output.worldNormal);
        #define LPOCEAN_REFLECTION_APPLY(color, output) color = ApplyOceanReflection(color, output.reflectionUV);
    #else
        #define LPOCEAN_REFLECTION_COORD(id)
        #define LPOCEAN_REFLECTION_TRANSFER(output)
        #define LPOCEAN_REFLECTION_APPLY(color, output)
    #endif


    // ------------------------------------------------------------------
    // Point Lighting

    half _OceanPointLightIntensity;

    float3 OceanShade4PointLights0(
        float4 lightPosX, float4 lightPosY, float4 lightPosZ,
        float3 lightColor0, float3 lightColor1, float3 lightColor2, float3 lightColor3,
        float4 lightAttenSq,
        float3 pos, float3 normal)
    {
        // to light vectors
        float4 toLightX = lightPosX - pos.x;
        float4 toLightY = lightPosY - pos.y;
        float4 toLightZ = lightPosZ - pos.z;
        // squared lengths
        float4 lengthSq = 0;
        lengthSq += toLightX * toLightX;
        lengthSq += toLightY * toLightY;
        lengthSq += toLightZ * toLightZ;
        // don't produce NaNs if some vertex position overlaps with the light
        lengthSq = max(lengthSq, 0.000001);

        // NdotL
        float4 ndotl = 0;
        ndotl += toLightX * normal.x;
        ndotl += toLightY * normal.y;
        ndotl += toLightZ * normal.z;
        // correct NdotL
        float4 corr = rsqrt(lengthSq);
        ndotl = abs(ndotl);
        ndotl = max (float4(0,0,0,0), ndotl * corr);
        // attenuation
        float4 atten = 1.0 / (1.0 + lengthSq * lightAttenSq);
        float4 diff = ndotl * atten;
        // final color
        float3 col = 0;
        col += lightColor0 * diff.x;
        col += lightColor1 * diff.y;
        col += lightColor2 * diff.z;
        col += lightColor3 * diff.w;
        return col;
    }

    half3 ApplyPointLighting(half3 col, float3 worldPos, half3 worldNormal)
    {
        col += OceanShade4PointLights0(unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0, unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb, unity_4LightAtten0, worldPos, worldNormal) * _OceanPointLightIntensity;

        return col;
    }

    #if LPOCEAN_POINT_LIGHTING
        #define LPOCEAN_POINT_LIGHTING_APPLY(color, output) color = ApplyPointLighting(color, output.worldPos, output.worldNormal);
    #else
        #define LPOCEAN_POINT_LIGHTING_APPLY(color, output)
    #endif


    half _OceanBackPointLightIntensity;

    half3 AppleOceanBackPointLighting(half3 col, float3 worldPos, half3 worldNormal)
    {
        col += OceanShade4PointLights0(unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0, unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb, unity_4LightAtten0, worldPos, worldNormal) * _OceanBackPointLightIntensity;

        return col;
    }

    #if LPOCEAN_POINT_LIGHTING
        #define LPOCEAN_BACK_POINT_LIGHTING_APPLY(color, output) color = AppleOceanBackPointLighting(color, output.worldPos, output.worldNormal);
    #else
        #define LPOCEAN_BACK_POINT_LIGHTING_APPLY(color, output)
    #endif


    // ------------------------------------------------------------------
    // Clip

    bool OceanClip(half4 screenPos, float cameraDepth)
    {   
        half4 col = tex2Dproj(_LPOceanClipTexture, screenPos);
        half clipValue = col.r;

        if(clipValue > 0.1)
        {
            float clipDepth = SAMPLE_DEPTH_TEXTURE_PROJ(_LPOceanClipDepthTexture, UNITY_PROJ_COORD(screenPos));
            clipDepth = LinearEyeDepth(clipDepth);

            if(clipValue < 0.5) //if clip fron
            {
                clip(cameraDepth - clipDepth);
            }
            else
            {
                clip(clipDepth - cameraDepth);
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    #if LPOCEAN_CLIP
        #define LPOCEAN_CLIP_APPLY(output, cameraDepth) OceanClip(output.screenPos, cameraDepth);
    #else
        #define LPOCEAN_CLIP_APPLY(output, cameraDepth)
    #endif

    // ------------------------------------------------------------------
    // Light

    UnityLight MainLight()
    {
        UnityLight l;
        l.color = _LightColor0.rgb;
        l.dir = _WorldSpaceLightPos0.xyz;
        return l;
    }

    UnityLight DummyLight()
    {
        UnityLight l;
        l.color = 0;
        l.dir = half3 (0,1,0);
        return l;
    }

    UnityIndirect ZeroIndirect()
    {
        UnityIndirect ind;
        ind.diffuse = 0;
        ind.specular = 0;
        return ind;
    }


#endif