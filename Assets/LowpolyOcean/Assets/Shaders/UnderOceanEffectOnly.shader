Shader "LowpolyOcean/UnderOceanEffectOnly"
{
	Properties
	{
        _Color ("Color", COLOR) = (0, 0, 0, 0)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }

		Pass
		{
			CGPROGRAM
            #include "UnityCG.cginc"
            #include "LPOceanLighting.cginc"
            #include "LPUnderOceanLighting.cginc"

            #pragma multi_compile_fog
            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
            #pragma multi_compile __ LPUNDER_OCEAN_EFFECT
		
            #pragma vertex vert
			#pragma fragment frag

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
                UNITY_POSITION(pos);
                float4 worldPos : TEXCOORD0;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				return o;
			}
			
            half4 _Color;

			fixed4 frag (v2f i) : SV_Target
			{
                half4 color = _Color;
                UnityLight light = MainLight();
                APPLY_LPUNDER_OCEAN_EFFECT_ONLY(color.rgb, i.pos, i.worldPos, light.dir, light.color)
				return color;
			}
			ENDCG
		}
	}
}
