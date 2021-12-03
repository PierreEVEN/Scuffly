
float _Width;
float3 _Offset;
RWTexture2D<float> _Altitude;

struct VertexOutput {
	float4 positionCS : SV_POSITION; // Clip space position
	float3 positionWS : TEXCOORD1; // World space position
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
	
	float realAltitude = GetAltitudeAtLocation(worldPos.xz);

	realAltitude = tex2Dlod(LandscapeMaskAtlas, float4(IN.vertex.xz + float2(0.5, 0.5), 0, 0)).x;

	// Clamp altitude to a minimum of zero
	float waterCorrectedAltitude = max(0,  realAltitude);

	// Transform from world position to clip space position
	OUT.positionCS = TransformWorldToHClip(GetCameraRelativePositionWS(worldPos + float3(0, waterCorrectedAltitude, 0)));// TransformWorldToHClip(GetCameraRelativePositionWS(precomputedAltitude));
	//OUT.positionCS = TransformWorldToHClip(GetCameraRelativePositionWS(float3(inputVertexWSPosition.x, inputVertexWSPosition.y + waterCorrectedAltitude, inputVertexWSPosition.z)));
	
		
	// Send world position to fragment stage
	OUT.positionWS = worldPos + float3(0, realAltitude, 0);

	return OUT;
}