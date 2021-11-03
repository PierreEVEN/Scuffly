#include "simplexNoiseGPU.cginc"

#include "landscapeMask.cginc"

float GetAltitudeAtLocation(float2 posX)
{
	float alt = snoise(float2(posX.x * 0.0001, posX.y * 0.0001)) * 1000;
	addAltitudeOverrides(posX, alt);
	return alt;
}