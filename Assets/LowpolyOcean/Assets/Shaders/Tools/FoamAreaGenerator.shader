Shader "Hidden/LowpolyOcean/FoamAreaGenerator"
{
	Properties
	{
		_LandTexture ("LandTexture", 2D) = "black" {}
        _FoamTexture ("FoamTexture", 2D) = "black" {}
	}

    CGINCLUDE

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

    v2f Vert (appdata v)
    {
	    v2f o;
	    o.pos = UnityObjectToClipPos(v.vertex);
	    o.uv = v.uv;
	    return o;
    }
			
    sampler2D _LandTexture;
    sampler2D _FoamTexture;
    float4 _OffsetArray[32]; // xy : offset, z : factor

    void CalculateOffset(inout float result, float2 uv, int index)
    {
        float3 value = _OffsetArray[index];
        uv += value.xy;
        half isLand = tex2D(_LandTexture, uv).r;
        result = max(result, value.z * isLand);
    }

    float4 Frag (v2f i) : SV_Target
    {
        float result = tex2D(_FoamTexture, i.uv).r;

        CalculateOffset(result, i.uv, 0);
        CalculateOffset(result, i.uv, 1);
        CalculateOffset(result, i.uv, 2);
        CalculateOffset(result, i.uv, 3);
        CalculateOffset(result, i.uv, 4);
        CalculateOffset(result, i.uv, 5);
        CalculateOffset(result, i.uv, 6);
        CalculateOffset(result, i.uv, 7);

        CalculateOffset(result, i.uv, 8);
        CalculateOffset(result, i.uv, 9);
        CalculateOffset(result, i.uv, 10);
        CalculateOffset(result, i.uv, 11);
        CalculateOffset(result, i.uv, 12);
        CalculateOffset(result, i.uv, 13);
        CalculateOffset(result, i.uv, 14);
        CalculateOffset(result, i.uv, 15);

        CalculateOffset(result, i.uv, 16);
        CalculateOffset(result, i.uv, 17);
        CalculateOffset(result, i.uv, 18);
        CalculateOffset(result, i.uv, 19);
        CalculateOffset(result, i.uv, 20);
        CalculateOffset(result, i.uv, 21);
        CalculateOffset(result, i.uv, 22);
        CalculateOffset(result, i.uv, 23);

        CalculateOffset(result, i.uv, 24);
        CalculateOffset(result, i.uv, 25);
        CalculateOffset(result, i.uv, 26);
        CalculateOffset(result, i.uv, 27);
        CalculateOffset(result, i.uv, 28);
        CalculateOffset(result, i.uv, 29);
        CalculateOffset(result, i.uv, 30);
        CalculateOffset(result, i.uv, 31);

	    return float4(result, result, result, 1);
    }

    ENDCG

	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag
			ENDCG
		}
	}
}
