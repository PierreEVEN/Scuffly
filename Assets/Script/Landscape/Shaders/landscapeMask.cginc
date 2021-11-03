
/*
 * Rectangle modifiers
 */
struct LandscapeMask_Rectangle
{
	float2 position;
	float2 halfExtent;
	float margins;
	float altitude;
};
StructuredBuffer<LandscapeMask_Rectangle> RectangleModifier;
int RectangleModifier_Count = 0;


/*
 * Texture modifiers
 */
struct LandscapeMask_Texture
{
	int maskId;
	float3 position;
	float3 scale;
};
StructuredBuffer<LandscapeMask_Texture> TextureModifier;
int TextureModifier_Count = 0;

/*
 * Mask atlas
 */
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
	uv.x = clamp(uv.x, 0, 1);
	uv.y = clamp(uv.y, 0, 1);

	LandscapeTextureRefs data = TextureMasksRefs[textureID];
	float2 from = data.from;
	float2 to = data.to;
	
	return float2(lerp(from.x, to.x, uv.x), lerp(from.y, to.y, uv.y));
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

			float2 uvPos = (position - data.position.xz) / data.scale.xz + float2(0.5, 0.5);

			uvPos = 2 * uvPos - float2(0.5, 0.5);
			
			float4 color = tex2Dlod(LandscapeMaskAtlas, float4(uvToAtlasMask(uvPos, data.maskId), 0, 0));

			altitude = data.position.y + color.r * data.scale.y;
		}
	}
}