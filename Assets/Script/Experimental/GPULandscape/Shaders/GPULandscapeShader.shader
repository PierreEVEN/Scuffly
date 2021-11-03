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

			// Render State
			Cull Back
			ZTest On
			Stencil
			{
				Ref 10
				CompFront Always
				PassFront Replace
				CompBack Always
				PassBack Replace
			}

			// Debug
			// <None>

			// --------------------------------------------------
			// Pass

			HLSLPROGRAM

			// Pragmas
			#pragma instancing_options renderinglayer
			#pragma target 4.5
			#pragma vertex Vert
			#pragma fragment Frag
			#pragma only_renderers d3d11 playstation xboxone xboxseries vulkan metal switch
			#pragma multi_compile_instancing

			// Keywords
			#pragma multi_compile_fragment _ LIGHT_LAYERS
			#pragma multi_compile_raytracing _ LIGHT_LAYERS
			#pragma shader_feature _ _SURFACE_TYPE_TRANSPARENT
			#pragma shader_feature_local _BLENDMODE_OFF _BLENDMODE_ALPHA _BLENDMODE_ADD _BLENDMODE_PRE_MULTIPLY
			#pragma shader_feature_local _ _DOUBLESIDED_ON
			#pragma shader_feature_local _ _ADD_PRECOMPUTED_VELOCITY
			#pragma shader_feature_local _ _TRANSPARENT_WRITES_MOTION_VEC
			#pragma shader_feature_local_fragment _ _ENABLE_FOG_ON_TRANSPARENT
			#pragma multi_compile _ DEBUG_DISPLAY
			#pragma shader_feature_local_fragment _ _DISABLE_DECALS
			#pragma shader_feature_local_raytracing _ _DISABLE_DECALS
			#pragma shader_feature_local_fragment _ _DISABLE_SSR
			#pragma shader_feature_local_raytracing _ _DISABLE_SSR
			#pragma shader_feature_local_fragment _ _DISABLE_SSR_TRANSPARENT
			#pragma shader_feature_local_raytracing _ _DISABLE_SSR_TRANSPARENT
			#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile_fragment PROBE_VOLUMES_OFF PROBE_VOLUMES_L1 PROBE_VOLUMES_L2
			#pragma multi_compile_raytracing PROBE_VOLUMES_OFF PROBE_VOLUMES_L1 PROBE_VOLUMES_L2
			#pragma multi_compile _ DYNAMICLIGHTMAP_ON
			#pragma multi_compile_fragment _ SHADOWS_SHADOWMASK
			#pragma multi_compile_raytracing _ SHADOWS_SHADOWMASK
			#pragma multi_compile_fragment DECALS_OFF DECALS_3RT DECALS_4RT
			#pragma multi_compile_fragment _ DECAL_SURFACE_GRADIENT
			#pragma shader_feature_local _REFRACTION_OFF _REFRACTION_PLANE _REFRACTION_SPHERE _REFRACTION_THIN

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

			#include "AltitudeGenerator.cginc"


			struct VertexInput
			{
				uint vertex_id		: SV_VertexID;
			};

			int _Subdivision;
			float _Width;
			float3 _Offset;


			struct VertexOutput {
				float4 positionCS               : SV_POSITION;
				float2 uv                       : TEXCOORD0;
				float3 worldPosition			: TEXCOORD1;
				float3 normalWS					: TEXCOORD2;
			};

			VertexOutput Vert(VertexInput IN)
			{
				VertexOutput OUT;

				uint quadId = IN.vertex_id / 6;
				uint vertId = IN.vertex_id % 6;

				float posX = (quadId % _Subdivision) * _Width + (vertId == 2 || vertId == 4 || vertId == 5 ? _Width : 0);
				float posY = (quadId / _Subdivision) * _Width + (vertId == 1 || vertId == 2 || vertId == 4 ? _Width : 0);

				OUT.uv = float2(posX, posY);
				OUT.worldPosition = float3(posX - _Width / 2 * _Subdivision, 0, posY - _Width / 2 * _Subdivision) + _Offset;
				OUT.worldPosition.y = GetAltitudeAtLocation(OUT.worldPosition.xz);

				float3 finalWorldPos = OUT.worldPosition;
				if (finalWorldPos.y < 0)
					finalWorldPos.y = 0;
				OUT.positionCS = TransformWorldToHClip(GetCameraRelativePositionWS(finalWorldPos));


				float altX = GetAltitudeAtLocation(OUT.worldPosition.xz + float2(1, 0));
				float altZ = GetAltitudeAtLocation(OUT.worldPosition.xz + float2(0, 1));

				OUT.normalWS = normalize(cross(float3(-1, altX - OUT.worldPosition.y, 0), float3(0, altZ - OUT.worldPosition.y, 1)));

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


				float3 color;

				if (IN.worldPosition.y < 0)
					color = float3(0, 0.5, 1);
				else
					color = float3(0.1, 0.5, 0);


				SurfaceData surfaceData;
				ZERO_INITIALIZE(SurfaceData, surfaceData);
				surfaceData.baseColor = float4(color, 1);
				surfaceData.normalWS = IN.normalWS;
				surfaceData.geomNormalWS = T2W(input, 2);
				surfaceData.tangentWS = normalize(T2W(input, 0).xyz);
				surfaceData.tangentWS = Orthonormalize(surfaceData.tangentWS, surfaceData.normalWS);
				surfaceData.perceptualSmoothness = 0;
				surfaceData.ambientOcclusion = 1;
				surfaceData.metallic = 0;
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
				builtinData.emissiveColor = float4(0, 0, 0, 0);
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
