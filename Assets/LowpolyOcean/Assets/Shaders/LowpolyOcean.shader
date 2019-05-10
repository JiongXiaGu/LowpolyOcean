Shader "LowpolyOcean/LowpolyOcean"
{
	Properties
	{
        // ------------------------------------------------------------------
        // Vertex
        
        //Tessellation
        _OceanTessellation ("Tessellation", Range(1, 32)) = 4
        _OceanTessMinDistance ("TessMinDistance", float) = 100
        _OceanTessMaxDistance ("TessMaxDistance", float) = 200

        //Wave
        [NoScaleOffset] _OceanWaveTexture ("WaveTexture", 2D) = "black" {}
        _OceanWaveRect0 ("Wave0 Rect", Vector) = (0.5, 0.5, 30, 30)
        _OceanWaveRect1 ("Wave1 Rect", Vector) = (0.5, 0.5, 30, 30)
        _OceanWaveRect2 ("Wave2 Rect", Vector) = (0.5, 0.5, 30, 30)
        _OceanWaveRect3 ("Wave3 Rect", Vector) = (0.5, 0.5, 30, 30)
        _OceanWaveRadian ("Wave Radian", Vector) = (0, 0, 0, 0)
        _OceanWaveUniformRadian ("Wave Uniform Radian", float) = 0
        _OceanWaveHeightPow ("Wave Height Pow", Vector) = (1, 1, 1, 1)
        _OceanWaveHeightScale ("Wave Height Scale", Vector) = (0.5, 0.5, 0.5, 0.5)
        _OceanWaveSpeedZ ("Speed z", Vector) = (0.1, 0.1, 0.1, 0.1)

        // ------------------------------------------------------------------
        // Fron Side Lighting

        //Lighting
        _OceanAlbedo ("Albedo", COLOR) = (0.27, 0.66, 0.77, 1)
        _OceanSpecular ("Specular", COLOR) = (1, 1, 1, 1)
        _OceanSmoothness ("Smoothness", Range(0, 1)) = 0.6
        _OceanOcclusion ("Occlusion", Range(0, 1)) = 1
        _OceanFresnelIntensity ("FresnelIntensity", Range(0, 5)) = 1
        [NoScaleOffset] _OceanFresnelTexture ("FresnelTexture", 2D) = "black" {}
        _OceanFresnelColor ("FresnelColor", COLOR) = (0.43, 0.73, 0.89, 1)
        _OceanShadowAttenuation ("ShadowAttenuation", Range(0, 1)) = 1

        //Refraction
        _OceanRefractionClarity ("Clarity", Range(0, 1)) = 0.08
        _OceanRefractionDeepColor ("DeepColor", COLOR) = (0.27, 0.66, 0.77, 1)
        _OceanRefractionOffset ("Offset", Range(-3, 3)) = 0.5

        //Reflection
        _OceanReflectionIntensity ("Intensity", Range(0, 1)) = 0.4
        _OceanReflectionOffset ("Offset", float) = 0.3
        _OceanReflectionHeight ("Height", float) = 0

        //Foam
        _OceanFoamColor ("FoamColor", COLOR) = (1, 1, 1, 1)
        _OceanFoamIntensity ("FoamIntensity", Range(0, 2)) = 1
        _OceanFoamRange ("FoamRange", float) = 1
        [NoScaleOffset] _OceanFoamTexture ("FoamTexture", 2D) = "white" {}
        _OceanFoamSpeed ("FoamSpeed", Vector) = (0.5, 0.5, 0.5, 0.5)
        _OceanFoamRect ("FoamRect", Vector) = (0.5, 0.5, 10, 10)

        //Cookie
        _OceanCookieTexture ("CookieTexture", 2D) = "black" {}
        _OceanCookieColor ("CookieColor", COLOR) = (1, 1, 1, 1)
        _OceanCookieDepthAtten ("CookieDepthAtten", float) = 2
        _OceanCookieDepthAttenOffset ("CookieDepthAttenOffset", float) = -10
        _OceanCookieLightAtten ("CookieLightAtten", float) = 1
        _OceanCookieIntensity ("CookieIntensity", float) = 1

        // ------------------------------------------------------------------
        // Back Side Lighting

        //Lighting
        _OceanBackArtisticColor ("Color", COLOR) = (1, 1, 1, 1)
        _OceanBackArtisticSpecularPow  ("SpecularPow", float) = 0.3
        _OceanBackArtisticSpecularColor ("SpecularColor", COLOR) = (1, 1, 1, 1)
        [NoScaleOffset] _OceanBackArtisticFresnelTexture ("FresnelTexture", 2D) = "black" {}
        _OceanBackArtisticFresneIntensity ("FresneIntensity", Range(0, 5)) = 1
        _OceanBackArtisticFresneColor ("FresnelColor", COLOR) = (0.43, 0.73, 0.89, 1)
        _OceanBackArtisticShadowAttenuation ("ShadowAttenuation", Range(0, 1)) = 1

        //Refraction
        _OceanBackRefractionClarity ("UnderOceanClarity", Range(0, 1)) = 0.3
        _OceanBackRefractionOffset ("Offset", Range(-5, 5)) = 1

        // ------------------------------------------------------------------
        // Point Lighting

        _OceanPointLightIntensity ("Intensity", Range(0, 1)) = 1
        _OceanBackPointLightIntensity ("BackIntensity", Range(0, 1)) = 1

        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", float) = 2
	}

    //dx11
    SubShader
	{
        Tags { "RenderType"="Transparent" "IgnoreProjector"="True" "Queue"="Transparent" }
        LOD 200
        Cull [_Cull]

		Pass
		{
            Name "ForwardBase"
            Tags { "LightMode" = "ForwardBase" }

			CGPROGRAM
            #pragma target 4.6
            #include "LPOceanCore.cginc"

            #pragma multi_compile_fog
			#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight

            #pragma multi_compile __ LPOCEAN_TRANSPARENT

            #pragma shader_feature __ LPOCEAN_WAVE1 LPOCEAN_WAVE2 LPOCEAN_WAVE3
            #pragma shader_feature __ LPOCEAN_RIPPLE

            #pragma shader_feature __ LPOCEAN_FRESNEL
            #pragma shader_feature __ LPOCEAN_REFRACTION LPOCEAN_REFRACTION_FULL
            #pragma shader_feature __ LPOCEAN_FOAM_SIMPLE_EYE_DEPTH LPOCEAN_FOAM_EYE_DEPTH LPOCEAN_FOAM_SHPERE8 LPOCEAN_FOAM_SHPERE16
            #pragma shader_feature __ LPOCEAN_FOAM_AREA1
            #pragma shader_feature __ LPOCEAN_COOKIE
            #pragma shader_feature __ LPOCEAN_REFLECTION
            #pragma shader_feature __ LPOCEAN_BACK_LIGHTING
            #pragma shader_feature __ LPOCEAN_BACK_REFRACTION LPOCEAN_BACK_REFRACTION_FULL
            #pragma shader_feature __ LPOCEAN_POINT_LIGHTING
            #pragma shader_feature __ LPOCEAN_CLIP
            #pragma multi_compile __ LPUNDER_OCEAN_EFFECT

			#pragma vertex VertForwardBase
            #pragma hull HullForwardBase
            #pragma domain DomainForwardBase
            #pragma geometry GeometryForwardBase
			#pragma fragment FragForwardBase
			ENDCG
		}

        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode"="ShadowCaster" }

            CGPROGRAM
            #pragma target 4.6
            #include "LPOceanShadow.cginc"

            #pragma multi_compile_shadowcaster

            #pragma shader_feature __ LPOCEAN_WAVE1 LPOCEAN_WAVE2 LPOCEAN_WAVE3
            #pragma shader_feature __ LPOCEAN_RIPPLE
            #pragma shader_feature __ LPOCEAN_CLIP

			#pragma vertex VertShadow
            #pragma hull HullShadow
            #pragma domain DomainShadow
			#pragma fragment FragShadow
            ENDCG
        }
	}

    //dx10
    SubShader
	{
        Tags { "RenderType"="Transparent" "IgnoreProjector"="True" "Queue"="Transparent" }
        LOD 200
        Cull [_Cull]

		Pass
		{
            Name "ForwardBase"
            Tags { "LightMode" = "ForwardBase" }

			CGPROGRAM
            #pragma target 4.0
            #include "LPOceanCore.cginc"

            #pragma multi_compile_fog
			#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight

            #pragma multi_compile __ LPOCEAN_TRANSPARENT

            #pragma shader_feature __ LPOCEAN_WAVE1 LPOCEAN_WAVE2 LPOCEAN_WAVE3
            #pragma shader_feature __ LPOCEAN_RIPPLE

            #pragma shader_feature __ LPOCEAN_FRESNEL
            #pragma shader_feature __ LPOCEAN_REFRACTION LPOCEAN_REFRACTION_FULL
            #pragma shader_feature __ LPOCEAN_FOAM_SIMPLE_EYE_DEPTH LPOCEAN_FOAM_EYE_DEPTH LPOCEAN_FOAM_SHPERE8 LPOCEAN_FOAM_SHPERE16
            #pragma shader_feature __ LPOCEAN_FOAM_AREA1
            #pragma shader_feature __ LPOCEAN_COOKIE
            #pragma shader_feature __ LPOCEAN_REFLECTION
            #pragma shader_feature __ LPOCEAN_BACK_LIGHTING
            #pragma shader_feature __ LPOCEAN_BACK_REFRACTION LPOCEAN_BACK_REFRACTION_FULL
            #pragma shader_feature __ LPOCEAN_POINT_LIGHTING
            #pragma shader_feature __ LPOCEAN_CLIP
            #pragma multi_compile __ LPUNDER_OCEAN_EFFECT

			#pragma vertex VertForwardBase
            #pragma geometry GeometryForwardBase
			#pragma fragment FragForwardBase
			ENDCG
		}

        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode"="ShadowCaster" }

            CGPROGRAM
            #include "LPOceanShadow.cginc"

            #pragma multi_compile_shadowcaster

            #pragma shader_feature __ LPOCEAN_WAVE1 LPOCEAN_WAVE2 LPOCEAN_WAVE3
            #pragma shader_feature __ LPOCEAN_RIPPLE
            #pragma shader_feature __ LPOCEAN_CLIP

			#pragma vertex VertShadow
			#pragma fragment FragShadow
            ENDCG
        }
	}

    //dx9
    SubShader
	{
        Tags { "RenderType"="Transparent" "IgnoreProjector"="True" "Queue"="Transparent" }
        LOD 200
        Cull [_Cull]

		Pass
		{
            Name "ForwardBase"
            Tags { "LightMode" = "ForwardBase" }

			CGPROGRAM
            #pragma target 3.0
            #include "LPOceanCore.cginc"

            #pragma multi_compile_fog
			#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight

            #pragma multi_compile __ LPOCEAN_TRANSPARENT

            #pragma shader_feature __ LPOCEAN_WAVE1 LPOCEAN_WAVE2 LPOCEAN_WAVE3
            #pragma shader_feature __ LPOCEAN_RIPPLE

            #pragma shader_feature __ LPOCEAN_FRESNEL
            #pragma shader_feature __ LPOCEAN_REFRACTION LPOCEAN_REFRACTION_FULL
            #pragma shader_feature __ LPOCEAN_FOAM_SIMPLE_EYE_DEPTH LPOCEAN_FOAM_EYE_DEPTH LPOCEAN_FOAM_SHPERE8 LPOCEAN_FOAM_SHPERE16
            #pragma shader_feature __ LPOCEAN_FOAM_AREA1
            #pragma shader_feature __ LPOCEAN_COOKIE
            #pragma shader_feature __ LPOCEAN_REFLECTION
            #pragma shader_feature __ LPOCEAN_BACK_LIGHTING
            #pragma shader_feature __ LPOCEAN_BACK_REFRACTION LPOCEAN_BACK_REFRACTION_FULL
            #pragma shader_feature __ LPOCEAN_POINT_LIGHTING
            #pragma shader_feature __ LPOCEAN_CLIP
            #pragma multi_compile __ LPUNDER_OCEAN_EFFECT

			#pragma vertex VertForwardBaseDX9
			#pragma fragment FragForwardBase
			ENDCG
		}

        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode"="ShadowCaster" }

            CGPROGRAM
            #include "LPOceanShadow.cginc"

            #pragma multi_compile_shadowcaster

            #pragma shader_feature __ LPOCEAN_WAVE1 LPOCEAN_WAVE2 LPOCEAN_WAVE3
            #pragma shader_feature __ LPOCEAN_RIPPLE
            #pragma shader_feature __ LPOCEAN_CLIP

			#pragma vertex VertShadow
			#pragma fragment FragShadow
            ENDCG
        }
	}
    CustomEditor "JiongXiaGu.LowpolyOcean.OceanShaderDrawer"
}


   //Obsolete
   //     Pass
   //     {
   //         Name "Deferred"
   //         Tags { "LightMode"="Deferred" }

   //         CGPROGRAM
   //         #pragma target 5.0
   //         #include "LPOceanCore.cginc"

   //         #pragma exclude_renderers nomrt
   //         #pragma multi_compile __ UNITY_HDR_ON

   //         #pragma shader_feature __ LPOCEAN_WAVE_RG
   //         #pragma shader_feature __ LPOCEAN_WAVE_BA
   //         #pragma shader_feature __ LPOCEAN_RIPPLE

   //         #pragma shader_feature __ LPOCEAN_REFRACTION LPOCEAN_REFRACTION_FULL
   //         #pragma shader_feature __ LPOCEAN_FOAM_SIMPLE_EYE_DEPTH
   //         #pragma shader_feature __ LPOCEAN_REFLECTION
   //         #pragma shader_feature __ LPOCEAN_BACK_LIGHTING
   //         #pragma shader_feature __ LPOCEAN_BACK_REFRACTION LPOCEAN_BACK_REFRACTION_FULL
   //         #pragma shader_feature __ LPOCEAN_CLIP

   //         #pragma vertex VertDeferred
   //         #pragma hull HullDeferred
   //         #pragma domain DomainDeferred
   //         #pragma geometry GeometryDeferred
			//#pragma fragment FragDeferred
            
   //         ENDCG
   //     }