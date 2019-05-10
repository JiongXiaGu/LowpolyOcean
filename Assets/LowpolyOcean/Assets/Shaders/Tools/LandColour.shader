Shader "Hidden/LowpolyOcean/LandColour"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
        _SeaLevelDepth ("SeaLevelDepth", float) = 0.5
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
            	float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
            float _SeaLevelDepth;

			half4 frag (v2f i) : SV_Target
			{
                float rawDpth = SAMPLE_DEPTH_TEXTURE(_MainTex, i.uv);
                float value = rawDpth > _SeaLevelDepth ? 1 : 0;
				return half4(value, value, value, 1);
			}
			ENDCG
		}
	}
}
