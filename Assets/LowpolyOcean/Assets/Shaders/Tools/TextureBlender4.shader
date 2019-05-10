Shader "Hidden/LowpolyOcean/Tools/TextureBlender4"
{
	Properties
	{
        Texture0 ("Texture0", 2D) = "black" {}
        Texture1 ("Texture1", 2D) = "black" {}
        Texture2 ("Texture2", 2D) = "black" {}
        Texture3 ("Texture3", 2D) = "black" {}
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
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
			
			sampler2D Texture0;
            sampler2D Texture1;
            sampler2D Texture2;
            sampler2D Texture3;

			half4 frag (v2f i) : SV_Target
			{
                half4 col = 0;
                
                col.x = tex2D(Texture0, i.uv).r;
                col.y = tex2D(Texture1, i.uv).r;
                col.z = tex2D(Texture2, i.uv).r;
                col.w = tex2D(Texture3, i.uv).r;

                return col;
			}
			ENDCG
		}
	}
}
