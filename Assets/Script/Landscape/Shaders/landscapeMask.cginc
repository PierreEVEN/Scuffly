
struct LandscapeMask_Rectangle
{
	float2 position;
	float2 halfExtent;
	float margins;
	float altitude;
};
StructuredBuffer<LandscapeMask_Rectangle> RectangleModifier;
int RectangleModifier_Count = 0;


struct LandscapeMask_Texture
{
	int maskId;
	float3 position;
	float3 scale;
};
StructuredBuffer<LandscapeMask_Texture> TextureModifier;
int TextureModifier_Count = 0;


struct LandscapeTextureRefs
{
	float2 from;
	float2 to;
};
sampler2D LandscapeMaskAtlas;
StructuredBuffer<LandscapeTextureRefs> TextureMasksRefs;
int TextureMasksRefs_Count = 0;

float2 uvToAtlasMask(float2 uv, int textureID)
{
	LandscapeTextureRefs data = TextureMasksRefs[textureID];
	return float2(lerp(data.from.x, data.to.x, uv.x), lerp(data.from.y, data.to.y, uv.y));
}


void addAltitudeOverrides(float2 position, inout float altitude)
{
	for (int i = 0; i < RectangleModifier_Count; ++i)
	{
		float2 pos = RectangleModifier[i].position;
		float2 ext = RectangleModifier[i].halfExtent;
		float mar = RectangleModifier[i].margins;

		if (
			position.x > pos.x - ext.x &&
			position.y > pos.y - ext.y &&
			position.x < pos.x + ext.x &&
			position.y < pos.y + ext.y)
		{
			float marginScale =
				min(
					min(pos.x + ext.x - position.x, pos.y + ext.y - position.y),
					min(position.x - (pos.x - ext.x), position.y - (pos.y - ext.y))
				);

			altitude = lerp(RectangleModifier[i].altitude, altitude, clamp(0, 0, 1));
		}
	}

	for (int i = 0; i < TextureModifier_Count; ++i)
	{
		LandscapeMask_Texture data = TextureModifier[i];
		float2 pos = data.position.xz;
		float2 ext = data.scale.xz / 2;

		if (
			position.x > pos.x - ext.x &&
			position.y > pos.y - ext.y &&
			position.x < pos.x + ext.x &&
			position.y < pos.y + ext.y)
		{

			float2 uvPos = (position - data.position.xz) / data.scale.xz;

			float4 color = tex2Dlod(LandscapeMaskAtlas, float4(uvToAtlasMask(uvPos, data.maskId), 0, 0));

			altitude = data.position.y + color.r * data.scale.y;
		}
	}
}