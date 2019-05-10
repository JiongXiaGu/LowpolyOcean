Shader "LowpolyOcean/MarkUnderOcean"
{
	Properties
	{
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", float) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "IgnoreProjector"="True" }
        Cull [_Cull]

		Pass
		{
			CGPROGRAM
            #pragma target 3.0
            #include "UnityStandardUtils.cginc"

			#pragma vertex vert
			#pragma fragment frag

			struct VertInput
			{
				float4 vertex : POSITION;
			};

			struct VertOutput
			{
				UNITY_POSITION(pos);
                half3 worldViewDir : TEXCOORD1;
			};

			VertOutput vert (VertInput v)
			{
				VertOutput o;
				o.pos = UnityObjectToClipPos(v.vertex);
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
				return o;
			}
			
            half _LPUnderOceanFornMark;
            half _LPUnderOceanBackMark;

			half4 frag (VertOutput vertOutput, fixed facing : VFACE) : SV_Target
			{
                half4 color;

                if(facing > 0)
                    color = half4(_LPUnderOceanFornMark, vertOutput.worldViewDir);
                else
                    color = half4(_LPUnderOceanBackMark, vertOutput.worldViewDir);

                return color;
			}
			ENDCG
		}
	}
}
