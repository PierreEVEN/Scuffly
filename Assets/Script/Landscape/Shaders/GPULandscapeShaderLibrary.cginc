
int _Subdivision;
float _Width;
float3 _Offset;



float3 getVertexLocalPosition(int vertexIndex)
{
	int quadId = vertexIndex / 6;
	int vertId = vertexIndex % 6;

	float posX = (quadId % _Subdivision) + (vertId == 2 || vertId == 4 || vertId == 5 ? 1 : 0);
	float posY = (quadId / _Subdivision) + (vertId == 1 || vertId == 2 || vertId == 4 ? 1 : 0);

	float y = 0;

	// Move seams down to avoir holes
	if (posX == 0) {
		posX += 1;
		y = -1;
	}
	if (posX == _Subdivision) {
		posX -= 1;
		y = -1;
	}
	if (posY == 0) {
		posY += 1;
		y = -1;
	}
	if (posY == _Subdivision) {
		posY -= 1;
		y = -1;
	}
	return float3(posX, y, posY);
}

float3 getVertexWorldPosition(int vertexIndex) {
	float reCenter = _Width / 2 * _Subdivision;
	return getVertexLocalPosition(vertexIndex) * _Width - float3(reCenter, 0, reCenter) + _Offset;
}

struct VertexInput
{
	uint vertex_id		: SV_VertexID;
};

struct VertexOutput {
	float4 positionCS : SV_POSITION; // Clip space position
	float3 positionWS : TEXCOORD1; // World space position
};

VertexOutput Vert(VertexInput IN)
{
	VertexOutput OUT;

	// Compute vertex position on xz axis in world space.
	float3 inputVertexWSPosition = getVertexWorldPosition(IN.vertex_id);

	// Retrieve altitude at vertex position
	float realAltitude = GetAltitudeAtLocation(inputVertexWSPosition.xz);

	// Clamp altitude to a minimum of zero
	float waterCorrectedAltitude = max(0, realAltitude);

	// Transform from world position to clip space position
	OUT.positionCS = TransformWorldToHClip(GetCameraRelativePositionWS(inputVertexWSPosition + float3(0, waterCorrectedAltitude, 0)));

	// Send world position to fragment stage
	OUT.positionWS = inputVertexWSPosition + float3(0, realAltitude, 0);

	return OUT;
}