#ifndef WATER_CG_INCLUDED
#define WATER_CG_INCLUDED

#include "../FXLab.cginc"

#define COLOR_EXTINCTION(viewDepthColor, heightColor, maxDepth, maxHeight, color) lerp(lerp(color, viewDepthColor, saturate(depth / maxDepth)), heightColor, saturate(height / maxHeight));

#endif