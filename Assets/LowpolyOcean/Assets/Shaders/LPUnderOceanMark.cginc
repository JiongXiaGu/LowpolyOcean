#ifndef LPUNDER_OCEAN_MARK_INCLUDED
#define LPUNDER_OCEAN_MARK_INCLUDED

    #include "LPOceanHelper.cginc"

    sampler2D _LPOceanMarkTexture;
    float4 _LPOceanMarkTexture_TexelSize;
    sampler2D _LPOceanMarkDepthTexture;
    float4 _LPOceanMarkDepthTexture_TexelSize;
    float _LPUnderOceanFornMark;
    float _LPOceanFornMark;
    float _LPOceanBackMark;

    bool IsUnderOcean(half4 mark)
    {
        return IsMarked(_LPUnderOceanFornMark, mark.r);
    }

    bool IsOceanFron(half4 mark)
    {
        return IsMarked(_LPOceanFornMark, mark.r);
    }

    bool IsOceanBack(half4 mark)
    {
        return IsMarked(_LPOceanBackMark, mark.r);
    }

    bool IsSkyBox(half4 mark)
    {
        return IsMarked(0, mark.r);
    }

#endif