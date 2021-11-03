
struct LandscapeMask_Rectangle
{
	float2 position;
	float2 halfExtent;
	float2 margins;
	float altitude;
};

StructuredBuffer<LandscapeMask_Rectangle> LandscapeMask_Rectangle_Array;
int LandscapeMask_Rectangle_Count = 0;

sampler2D _Albedo;

void addAltitudeOverrides(float2 position, inout float altitude)
{
	for (int i = 0; i < LandscapeMask_Rectangle_Count; ++i)
	{
		float2 pos = LandscapeMask_Rectangle_Array[i].position;
		float2 ext = LandscapeMask_Rectangle_Array[i].halfExtent;
		float2 mar = LandscapeMask_Rectangle_Array[i].margins;

		if (
			position.x > pos.x - ext.x &&
			position.y > pos.y - ext.y &&
			position.x < pos.x + ext.x &&
			position.y < pos.y + ext.y) 
		{

			altitude = LandscapeMask_Rectangle_Array[i].altitude;
			

		}
	}
}