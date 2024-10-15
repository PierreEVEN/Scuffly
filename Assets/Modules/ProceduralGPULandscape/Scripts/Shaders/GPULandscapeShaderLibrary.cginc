
float _Width;
float3 _Offset;
sampler2D _AltitudeGet;
RWTexture2D<half4> _AltitudeSet;

struct VertexOutput {
	float4 positionCS : SV_POSITION; // Clip space position
	float3 positionWS : TEXCOORD1; // World space position
	float3 normalWS : TEXCOORD2;
};


struct VertexInput
{
	float4 vertex    : POSITION;
};

VertexOutput Vert(VertexInput IN)
{
	VertexOutput OUT;

	// Compute vertex position on xz axis in world space.
	//float3 inputVertexWSPosition = getVertexWorldPosition(IN.vertex);

	// Retrieve altitude at vertex position

	float3 worldPos = IN.vertex.xyz * _Width + _Offset;
	
	float4 data = tex2Dlod(_AltitudeGet, float4(IN.vertex.xz + float2(0.5, 0.5), 0, 0));

	// Clamp altitude to a minimum of zero
	float waterCorrectedAltitude = max(0, data.x);

	// Transform from world position to clip space position
	OUT.positionCS = TransformWorldToHClip(GetCameraRelativePositionWS(worldPos + float3(0, waterCorrectedAltitude, 0)));// TransformWorldToHClip(GetCameraRelativePositionWS(precomputedAltitude));
	//OUT.positionCS = TransformWorldToHClip(GetCameraRelativePositionWS(float3(inputVertexWSPosition.x, inputVertexWSPosition.y + waterCorrectedAltitude, inputVertexWSPosition.z)));
	
		
	// Send world position to fragment stage
	OUT.positionWS = worldPos + float3(0, data.x, 0);

	OUT.normalWS = data.yzw;
	
	return OUT;
}