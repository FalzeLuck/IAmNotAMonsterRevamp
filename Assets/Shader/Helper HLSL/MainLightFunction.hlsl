#ifndef GETMAINLIGHTDIRECTION_INCLUDED
#define GETMAINLIGHTDIRECTION_INCLUDED

void GetMainLight_float(out float3 Direction, out float3 Color, out float Attenuation)
{
#if defined(SHADERGRAPH_PREVIEW)
    Direction = float3(0.5, 0.5, 0);
    Color = float3(1, 1, 1);
    Attenuation = 1;
#else
    // HDRP specific light retrieval
    Direction = _DirectionalLightDatas[0].forward;
    Color = _DirectionalLightDatas[0].color;
    Attenuation = 1; // Simplified for directional, need shadow masking for full calculation
#endif
}

#endif