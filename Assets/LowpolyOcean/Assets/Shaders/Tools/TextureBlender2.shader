Shader "Hidden/LowpolyOcean/Tools/TextureBlender2"
{
	Properties
	{
        TextureRG ("TextureRG", 2D) = "black" {}
        TextureBA ("TextureBA", 2D) = "black" {}
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
            #include "../LPOceanHelper.cginc"

			#pragma vertex vert
			#pragma fragment frag
			
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D TextureRG;
            sampler2D TextureBA;

			half4 frag (v2f i) : SV_Target
			{
                half4 col = 0;
                
                float v = tex2D(TextureRG, i.uv).r;
                col.rg = OceanEncodeFloatRG(v);

                v = tex2D(TextureBA, i.uv).r;
                col.ba = OceanEncodeFloatRG(v);

                return col;
			}
			ENDCG
		}
	}
}
