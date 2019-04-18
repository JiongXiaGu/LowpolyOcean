Shader "LowpolyOcean/Examples/UnderOceanDiffuse"
{
	Properties
	{
        _Color ("Color", COLOR) = (1, 1, 1, 1)
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque"  "IgnoreProjector"="True" }

		Pass
		{
            Tags {"LightMode"="ForwardBase"}

			CGPROGRAM
            #pragma target 3.0
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "../LPUnderOceanLighting.cginc"

            #pragma multi_compile_fog
            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight

            //Necessary keywords
            #pragma multi_compile __ LPUNDER_OCEAN_CLIP
            #pragma multi_compile __ LPUNDER_OCEAN_LIGHTING
            #pragma multi_compile __ LPUNDER_OCEAN_COOKIE
            #pragma multi_compile __ LPUNDER_OCEAN_EFFECT

			#pragma vertex vert
			#pragma fragment frag

			struct appdata
			{
				float4 vertex : POSITION;
                float3 normal : Normal;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
            	UNITY_POSITION(pos);
				float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 worldPos : TEXCOORD2;
                half3 worldNormal : TEXCOORD3;
                half4 screenPos : TEXCOORD5;
                SHADOW_COORDS(6)
			};

            half3 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.worldNormal = normalize(UnityObjectToWorldNormal(v.normal));
                o.screenPos = ComputeScreenPos(o.pos);
				UNITY_TRANSFER_FOG(o, o.vertex);
                TRANSFER_SHADOW(o)
				return o;
			}
			
            /*
                The underwater effect is achieved by these two macros:
                    APPLY_LPUNDER_OCEAN_LIGHTING(pos, worldPos, screenPos, lightDir, lightColor, shadowAtten)
                    APPLY_LPUNDER_OCEAN_EFFECT(col, fogCoord, screenPos, pos, worldPos, lightDir, lightColor)
                From "LPUnderOceanLighting.cginc"
            */    
            
			half4 frag (v2f i) : SV_Target
			{
				half4 col = tex2D(_MainTex, i.uv);
                col.rgb *= _Color;

                half3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                half3 lightColor = _LightColor0.rgb;
                float shadowAtten = SHADOW_ATTENUATION(i);

                //Calculate light intensity, shadow attenuation and "cookies"
                APPLY_LPUNDER_OCEAN_LIGHTING(i.pos, i.worldPos, i.screenPos, lightDir, lightColor, shadowAtten)

                half diff = max(0, dot(i.worldNormal, lightDir));
                half3 ambient = saturate(ShadeSH9(half4(i.worldNormal, 1.0)));
                col.rgb *= lightColor * diff * shadowAtten + ambient;

                //Add fog effect
                APPLY_LPUNDER_OCEAN_EFFECT(col.rgb, i.fogCoord, i.screenPos, i.pos, i.worldPos, lightDir, lightColor)
				return col;
			}
			ENDCG
		}

        Pass
        {
            Tags {"LightMode"="ShadowCaster"}

            CGPROGRAM
            #include "UnityCG.cginc"

            #pragma multi_compile_shadowcaster

            #pragma vertex vert
            #pragma fragment frag

            struct v2f 
            { 
                V2F_SHADOW_CASTER;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
	}
}
