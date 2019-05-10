Shader "LowpolyOcean/MarkOceanObstruct"
{
	Properties
	{
	}


    CGINCLUDE
    #include "UnityCG.cginc"
    #include "LPUnderOceanMark.cginc"

    struct VertData
    {
        float4 vertex : POSITION;
    };

    struct FragData
    {
        float4 vertex : INTERNALTESSPOS0;
	    UNITY_POSITION(pos);
    };

    FragData vert (VertData v)
    {
	    FragData o;

        o.vertex = v.vertex;
	    o.pos = UnityObjectToClipPos(v.vertex);

	    return o;
    }
			
    half4 frag (FragData vertexOutput, fixed facing : VFACE) : SV_Target
    {
        half4 color = _LPOceanFornMark;
        return color;
    }

    ENDCG

    //DX11
	SubShader
	{
		Tags { "RenderType"="Opaque" }

        Pass
        {
			CGPROGRAM
            #pragma target 3.0

            #pragma vertex vert
			#pragma fragment frag
			ENDCG
        }
	}
}
