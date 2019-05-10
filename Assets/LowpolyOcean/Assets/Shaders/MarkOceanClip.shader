Shader "LowpolyOcean/MarkOceanClip"
{
	Properties
	{
        [Header(Ocean)]
        [Enum(Off, 0, ClipFron, 0.4, ClipBack, 0.8)] _FronClipOcean ("FronSide", float) = 0
        [Enum(Off, 0, ClipFron, 0.4, ClipBack, 0.8)] _BackClipOcean ("BackSide", float) = 0

        [Header(UnderOcean)]
        [Enum(Off, 0, ClipFron, 0.4, ClipBack, 0.8)] _FornClipUnderOcean ("FronSide", float) = 0
        [Enum(Off, 0, ClipFron, 0.4, ClipBack, 0.8)] _BackClipUnderOcean ("BackSide", float) = 0

        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", float) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
        Cull [_Cull]

		Pass
		{
			CGPROGRAM
            #pragma target 3.0
            #include "LPOceanClip.cginc"
            #include "UnityStandardUtils.cginc"

			#pragma vertex vert
			#pragma fragment frag
			
			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				UNITY_POSITION(pos);
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			}

            half _FronClipOcean;
            half _BackClipOcean;
            half _FornClipUnderOcean;
            half _BackClipUnderOcean;

			fixed4 frag (fixed facing : VFACE) : SV_Target
			{
                fixed4 color;

                if(facing > 0)
                    color = half4(_FronClipOcean, _FornClipUnderOcean, 0, 1);
                else
                    color = half4(_BackClipOcean, _BackClipUnderOcean, 0, 1);

				return color;
			}
			ENDCG
		}
	}
}
