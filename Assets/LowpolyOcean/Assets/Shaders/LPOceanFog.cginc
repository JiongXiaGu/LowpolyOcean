#ifndef LPOCEAN_FOG_INCLUDED
#define LPOCEAN_FOG_INCLUDED

    
    float GetOceanFogFactor(float density, float eyeDepth)
    {
        float fogFactor = density * eyeDepth;
        fogFactor = exp2(-fogFactor);
        return fogFactor;
    }

#endif