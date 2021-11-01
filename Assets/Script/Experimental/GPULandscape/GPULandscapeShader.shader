Shader "HDRP/GpuLandscapeShader"
{
	Properties
	{
	}

	SubShader
	{
		Tags {
			"RenderPipeline" = "HDRenderPipeline"
			"RenderType" = "Opaque"
			"Queue" = "Geometry+255"
			"DisableBatching" = "True"
		}

		Pass
		{
			Name "GBuffer"
			Tags { "LightMode" = "GBuffer" }

			Cull Back

			HLSLPROGRAM
			#pragma target 4.5
			#pragma vertex Vert
			#pragma fragment Frag

			#define SHADERPASS SHADERPASS_GBUFFER
			#define AI_HD_RENDERPIPELINE
			#define T2W(var, index) var.tangentToWorld[index]

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/NormalSurfaceGradient.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/Lit.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"

			struct VertexInput
			{
				uint vertex_id		: SV_VertexID;
			};

			struct VertexOutput {
				float4 positionCS               : SV_POSITION;
				float2 uv                       : TEXCOORD0;
				float3 worldPosition			: TEXCOORD1;
			};

			VertexOutput Vert(VertexInput IN)
			{
				VertexOutput OUT;
				int s = 10;
				float width = 1;

				uint quadId = IN.vertex_id / 6;
				uint vertId = IN.vertex_id % 6;

				float posX = (quadId % s) * width + (vertId == 2 || vertId == 4 || vertId == 5 ? width : 0);
				float posY = (quadId / s) * width + (vertId == 1 || vertId == 2 || vertId == 4 ? width : 0);

				OUT.uv = float2(posX, posY);
				OUT.worldPosition = float3(posX, 4, posY);
				OUT.positionCS = TransformWorldToHClip(GetCameraRelativePositionWS(OUT.worldPosition));
				return OUT;
			}

			void Frag(VertexOutput IN, OUTPUT_GBUFFER(outGBuffer))
			{
				FragInputs input;
				ZERO_INITIALIZE(FragInputs, input);
				input.positionSS = IN.positionCS;
				input.positionRWS = IN.worldPosition;
				input.tangentToWorld = k_identity3x3;

				PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS.xyz);

				float alpha = 1.0;


				SurfaceData surfaceData;
				ZERO_INITIALIZE(SurfaceData, surfaceData);
				surfaceData.baseColor = float4(1, 1, 0, 1);
				surfaceData.normalWS = float3(0, 0, 1);
				surfaceData.geomNormalWS = T2W(input, 2);
				surfaceData.tangentWS = normalize(T2W(input, 0).xyz);
				surfaceData.tangentWS = Orthonormalize(surfaceData.tangentWS, surfaceData.normalWS);
				surfaceData.perceptualSmoothness = 1;
				surfaceData.ambientOcclusion = 1;
				surfaceData.metallic = 1;
				surfaceData.coatMask = 0;
				surfaceData.specularOcclusion = 1;
				surfaceData.specularColor = 1;
				surfaceData.thickness = 1;
				surfaceData.iridescenceMask = 0;
				surfaceData.iridescenceThickness = 1;
				surfaceData.subsurfaceMask = 1;
				surfaceData.diffusionProfileHash = 1;
				surfaceData.anisotropy = 0;
				surfaceData.ior = 1.0;
				surfaceData.transmittanceColor = float3(1.0, 1.0, 1.0);
				surfaceData.atDistance = 1.0;
				surfaceData.transmittanceMask = 0.0;
				//surfaceData.materialFeatures = MATERIALFEATUREFLAGS_LIT_SPECULAR_COLOR;

				BuiltinData builtinData;
				InitBuiltinData(posInput, alpha, surfaceData.normalWS, -T2W(input, 2), input.texCoord1, input.texCoord2, builtinData);
				builtinData.emissiveColor = float4(1, 1, 0, 1);
				builtinData.depthOffset = 0.0;
				builtinData.distortion = float2(0.0, 0.0);
				builtinData.distortionBlur = 0.0;
				builtinData.opacity = 1;
				PostInitBuiltinData(GetWorldSpaceNormalizeViewDir(input.positionRWS), posInput, surfaceData, builtinData);

				ENCODE_INTO_GBUFFER(surfaceData, builtinData, posInput.positionSS, outGBuffer);
			}
			ENDHLSL
		}
	}
}
