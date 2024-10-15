#ifndef __LANDSCAPE_MASK_CGINC__
#define __LANDSCAPE_MASK_CGINC__

#define MODE_OVERRIDE 1
#define MODE_ALTITUDE_MASK 2
#define MODE_FOLIAGE_MASK 4
#define MODE_TREE_MASK 8
#define MODE_NOISE_MASK 16

#define DECLARE_MASK(MASK_NAME) \
struct MASK_NAME##_type \
{ \
	int priority; \
	int mode; \
	float3 position; \
	float3 scale; \
}; \
struct MASK_NAME##_CustomData_type \
{ \


#define END_MASK_DECLARATION(MASK_NAME) \
}; \
StructuredBuffer<MASK_NAME##_type> MASK_NAME; \
StructuredBuffer< MASK_NAME##_CustomData_type> MASK_NAME##_CustomData; \
int MASK_NAME##_Count = 0;

DECLARE_MASK(RectangleModifier)
float margins;
END_MASK_DECLARATION(RectangleModifier)

DECLARE_MASK(TextureModifier)
int textureId;
float zOffset;
END_MASK_DECLARATION(TextureModifier)

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
	
	return float2(lerp(from.x, from.x + to.x, uv.x), lerp(from.y, from.y + to.y, uv.y));
}

void ApplyModifierAtPosition(float2 position, inout float modifiedWeight, int mask)
{
	for (int i = 0; i < TextureModifier_Count; ++i)
	{
		const TextureModifier_type data = TextureModifier[i];
		if (!(data.mode & mask))
			continue;

		const float2 pos = data.position.xz;
		const float2 ext = data.scale.xz / 2;

		
		if (
			position.x > pos.x - ext.x &&
			position.y > pos.y - ext.y &&
			position.x < pos.x + ext.x &&
			position.y < pos.y + ext.y)
		{
			const TextureModifier_CustomData_type customData = TextureModifier_CustomData[i];

			const float2 uvPos = (position - data.position.xz) / data.scale.xz + float2(0.5, 0.5);

			const float4 color = clamp(tex2Dlod(LandscapeMaskAtlas, float4(uvToAtlasMask(uvPos, customData.textureId), 0, 0)) - customData.zOffset, 0, 1);

			modifiedWeight = data.position.y + color.r * data.scale.y + (data.mode & MODE_OVERRIDE ? 0 : modifiedWeight);
		}
	}

	for (int j = 0; j < RectangleModifier_Count; ++j)
	{
		const RectangleModifier_type data = RectangleModifier[j];
		if (!(data.mode & mask))
			continue;
		
		const float3 pos = data.position;
		const float3 ext = data.scale;

		if (
			position.x > pos.x - ext.x &&
			position.y > pos.z - ext.z &&
			position.x < pos.x + ext.x &&
			position.y < pos.z + ext.z)
		{
			const RectangleModifier_CustomData_type customData = RectangleModifier_CustomData[j];

			const float xdistance = min(abs(position.x - pos.x - ext.x), abs(position.x - pos.x + ext.x));
			const float ydistance = min(abs(position.y - pos.z - ext.z), abs(position.y - pos.z + ext.z));
			const float distance = pow(clamp(min(xdistance, ydistance) / customData.margins, 0, 1), 2.f);

			modifiedWeight = lerp(data.position.y, modifiedWeight, clamp(1 - distance, 0, 1)) + (data.mode & MODE_OVERRIDE ? 1 : modifiedWeight);
		}
	}
}
#endif