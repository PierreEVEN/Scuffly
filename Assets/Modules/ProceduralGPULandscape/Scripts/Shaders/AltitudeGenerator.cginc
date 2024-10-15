#ifndef __ALTITUDE_GENERATOR_CGINC__
#define __ALTITUDE_GENERATOR_CGINC__

#include "simplexNoiseGPU.cginc"

#include "landscapeMask.cginc"

float GetAltitudeAtLocation(float2 posX)
{
	float mountain = snoise(float2(posX.x * 0.00001, posX.y * 0.00001));

	
	float alt = mountain * 4000 + pow(snoise(float2(posX.x * 0.0001, posX.y * 0.0001)), 3) * 1000 + 1 - abs(snoise(float2(posX.x * 0.0001, posX.y * 0.0001)) * 4000) * (clamp((mountain - 0.5) * 2, 0, 1));
	ApplyModifierAtPosition(posX, alt, MODE_NOISE_MASK);
	
	
	ApplyModifierAtPosition(posX, alt, MODE_ALTITUDE_MASK);
	return alt;
}
#endif