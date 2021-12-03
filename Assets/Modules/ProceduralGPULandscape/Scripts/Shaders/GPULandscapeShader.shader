Shader "HDRP/GpuLandscapeShader"
{
	Properties
	{
		[NoScaleOffset] _GrassAlbedo("Grass_Albedo", 2D) = "white" {}
		[NoScaleOffset] _Grass2Albedo("Grass2_Albedo", 2D) = "white" {}
		[NoScaleOffset] _RockAlbedo("Rock_Albedo", 2D) = "white" {}
		[NoScaleOffset] _SnowAlbedo("Snow_Albedo", 2D) = "white" {}
		[NoScaleOffset] _SandAlbedo("Sand_Albedo", 2D) = "white" {}
		[NoScaleOffset] _GroundIntensity("Ground Intensity", float) = 1

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
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/Lit.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/DecalUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/LitDecalData.hlsl"

			#include "AltitudeGenerator.cginc"
			#include "GPULandscapeShaderLibrary.cginc"
			#include "GPULandscapeOceanShader.cginc"

			sampler2D _GrassAlbedo;
			sampler2D _Grass2Albedo;
			sampler2D _RockAlbedo;
			sampler2D _SnowAlbedo;
			sampler2D _SandAlbedo;

			float _GroundIntensity;



			void Frag(VertexOutput IN, OUTPUT_GBUFFER(outGBuffer))
			{
				float cameraDistance = length(_WorldSpaceCameraPos - IN.positionWS);

				float normalFetchDistance = clamp(cameraDistance / 100, 1, 200);

				// Per pixel normal
				/*
				float altX = max(0, GetAltitudeAtLocation(IN.positionWS.xz + float2(normalFetchDistance, 0)));
				float altZero = max(0, GetAltitudeAtLocation(IN.positionWS.xz));
				float altZ = max(0, GetAltitudeAtLocation(IN.positionWS.xz + float2(0, normalFetchDistance)));
				*/
				float3 normalWS = float3(0, 1, 0);// normalize(cross(float3(-normalFetchDistance, altX - altZero, 0), float3(0, altZ - altZero, normalFetchDistance)));

				FragInputs input;
				ZERO_INITIALIZE(FragInputs, input);
				input.positionSS = IN.positionCS;
				input.positionRWS = IN.positionWS;
				input.tangentToWorld = k_identity3x3;

				PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS.xyz);

				float alpha = 1.0;
				float smoothness = 0;
				float metalness = 0;


				float3 color = tex2D(_GrassAlbedo, IN.positionWS.xz * 0.1);
				color = lerp(tex2D(_Grass2Albedo, IN.positionWS.xz * 0.1), color, (snoise(IN.positionWS * 0.003) * snoise(IN.positionWS * 0.001)) * 0.5 + 0.5);
				// add snow
				color = lerp(color, tex2D(_SnowAlbedo, IN.positionWS.xz * 0.1), clamp((IN.positionWS.y - 1000 + snoise(IN.positionWS.xz * 0.001) * 100) * 0.001, 0, 1));

				// add rock
				color = lerp(tex2D(_RockAlbedo, IN.positionWS.xz * 0.1), color, max(pow(dot(normalWS, float3(0, 1, 0)), 15), 0));;

				// add beach
				color = lerp(tex2D(_SandAlbedo, IN.positionWS.xz * 0.1), color, clamp(IN.positionWS.y * 0.2 - 2, 0, 1));

				float3 normal = normalWS;

				// Darker wide pos
				color = pow(color, max(1, min(2, pow(cameraDistance / 5000, 0.5))));


				if (IN.positionWS.y < 0) {

					float currentWaterDepth = 0.4;

					float3 forward = normalize(IN.positionWS - _WorldSpaceCameraPos);

					float hihit = intersectPlane(_WorldSpaceCameraPos, forward, float3(0, currentWaterDepth, 0), float3(0.0, 1.0, 0.0));
					float lohit = intersectPlane(_WorldSpaceCameraPos, forward, float3(0, 0, 0), float3(0.0, 1.0, 0.0));
					float3 hipos = _WorldSpaceCameraPos + forward * hihit;
					float3 lopos = _WorldSpaceCameraPos + forward * lohit;
					float dist = raymarchwater(_WorldSpaceCameraPos, hipos, lopos, currentWaterDepth);
					float3 N = getNormal(IN.positionWS.xz, 0.001, currentWaterDepth);
					normal = N;
					float2 velocity = N.xz * (1.0 - N.y);
					N = lerp(float3(0.0, 1.0, 0.0), N, 1.0 / (dist * dist * 0.01 + 1.0));
					float3 R = reflect(forward, N);
					float fresnel = (0.04 + (1.0 - 0.04) * (pow(1.0 - max(0.0, dot(-N, forward)), 5.0)));

					float3 C = fresnel * getatm(R) * 2.0;
					//tonemapping
					C = aces_tonemap(C);

					color = C;

					smoothness = 1;
					metalness = 0.5;
				}

				SurfaceData surfaceData;
				ZERO_INITIALIZE(SurfaceData, surfaceData);
				surfaceData.baseColor = float3(color) *_GroundIntensity;
				surfaceData.normalWS = normal;
				surfaceData.geomNormalWS = T2W(input, 2);
				surfaceData.tangentWS = normalize(T2W(input, 0).xyz);
				surfaceData.tangentWS = Orthonormalize(surfaceData.tangentWS, surfaceData.normalWS);
				surfaceData.perceptualSmoothness = smoothness;
				surfaceData.ambientOcclusion = 1;
				surfaceData.metallic = metalness;
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

#if HAVE_DECALS
				if (_EnableDecals)
				{
					DecalSurfaceData decalSurfaceData = GetDecalSurfaceData(posInput, input.tangentToWorld[2], alpha);
					ApplyDecalToSurfaceData(decalSurfaceData, input.tangentToWorld[2], surfaceData);
				}
#endif

				BuiltinData builtinData;
				InitBuiltinData(posInput, alpha, surfaceData.normalWS, -T2W(input, 2), input.texCoord1, input.texCoord2, builtinData);
				builtinData.emissiveColor = float3(0, 0, 0);
				builtinData.depthOffset = 0.0;
				builtinData.distortion = float2(0.0, 0.0);
				builtinData.distortionBlur = 0.0;
				builtinData.opacity = 1;
				PostInitBuiltinData(GetWorldSpaceNormalizeViewDir(input.positionRWS), posInput, surfaceData, builtinData);

				ENCODE_INTO_GBUFFER(surfaceData, builtinData, posInput.positionSS, outGBuffer);
			}
			ENDHLSL
		}

		Pass
		{
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }

			Cull[_CullMode]
			ZWrite On
			ColorMask 0
			ZClip[_ZClip]

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
			#pragma shader_feature _ _SURFACE_TYPE_TRANSPARENT
			#pragma shader_feature_local _BLENDMODE_OFF _BLENDMODE_ALPHA _BLENDMODE_ADD _BLENDMODE_PRE_MULTIPLY
			#pragma shader_feature_local _ _DOUBLESIDED_ON
			#pragma shader_feature_local _ _ADD_PRECOMPUTED_VELOCITY
			#pragma shader_feature_local _ _TRANSPARENT_WRITES_MOTION_VEC
			#pragma shader_feature_local_fragment _ _ENABLE_FOG_ON_TRANSPARENT
			#pragma shader_feature_local_fragment _ _DISABLE_DECALS
			#pragma shader_feature_local_raytracing _ _DISABLE_DECALS
			#pragma shader_feature_local_fragment _ _DISABLE_SSR
			#pragma shader_feature_local_raytracing _ _DISABLE_SSR
			#pragma shader_feature_local_fragment _ _DISABLE_SSR_TRANSPARENT
			#pragma shader_feature_local_raytracing _ _DISABLE_SSR_TRANSPARENT
			#pragma shader_feature_local _REFRACTION_OFF _REFRACTION_PLANE _REFRACTION_SPHERE _REFRACTION_THIN


			#define SHADERPASS SHADERPASS_SHADOWS
			#define AI_HD_RENDERPIPELINE

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"

			#include "AltitudeGenerator.cginc"
			#include "GPULandscapeShaderLibrary.cginc"

			void Frag(VertexOutput IN, out float outputDepth : SV_Depth)
			{
				FragInputs input;
				ZERO_INITIALIZE(FragInputs, input);
				input.positionSS = IN.positionCS;
				input.positionRWS = IN.positionWS;
				input.tangentToWorld = k_identity3x3;

				PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS.xyz);

				outputDepth = posInput.deviceDepth;
			}
			ENDHLSL
		}

		Pass
		{
			Name "DepthOnly"
			Tags { "LightMode" = "DepthOnly" }

			// Render State
			Cull[_CullMode]
			ZWrite On
			Stencil
			{
				WriteMask[_StencilWriteMaskDepth]
				Ref[_StencilRefDepth]
				CompFront Always
				PassFront Replace
				CompBack Always
				PassBack Replace
			}

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
			#pragma multi_compile _ WRITE_NORMAL_BUFFER
			#pragma multi_compile_fragment _ WRITE_MSAA_DEPTH
			#pragma shader_feature _ _SURFACE_TYPE_TRANSPARENT
			#pragma shader_feature_local _BLENDMODE_OFF _BLENDMODE_ALPHA _BLENDMODE_ADD _BLENDMODE_PRE_MULTIPLY
			#pragma shader_feature_local _ _DOUBLESIDED_ON
			#pragma shader_feature_local _ _ADD_PRECOMPUTED_VELOCITY
			#pragma shader_feature_local _ _TRANSPARENT_WRITES_MOTION_VEC
			#pragma shader_feature_local_fragment _ _ENABLE_FOG_ON_TRANSPARENT
			#pragma shader_feature_local_fragment _ _DISABLE_DECALS
			#pragma shader_feature_local_raytracing _ _DISABLE_DECALS
			#pragma shader_feature_local_fragment _ _DISABLE_SSR
			#pragma shader_feature_local_raytracing _ _DISABLE_SSR
			#pragma shader_feature_local_fragment _ _DISABLE_SSR_TRANSPARENT
			#pragma shader_feature_local_raytracing _ _DISABLE_SSR_TRANSPARENT
			#pragma multi_compile _ WRITE_DECAL_BUFFER
			#pragma shader_feature_local _REFRACTION_OFF _REFRACTION_PLANE _REFRACTION_SPHERE _REFRACTION_THIN

			#define SHADERPASS SHADERPASS_DEPTH_ONLY
			#define AI_HD_RENDERPIPELINE

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"

			#include "AltitudeGenerator.cginc"
			#include "GPULandscapeShaderLibrary.cginc"

			void Frag(VertexOutput IN, out float outputDepth : SV_Depth)
			{
				FragInputs input;
				ZERO_INITIALIZE(FragInputs, input);
				input.positionSS = IN.positionCS;
				input.positionRWS = IN.positionWS;
				input.tangentToWorld = k_identity3x3;

				PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS.xyz);

				outputDepth = posInput.deviceDepth;
			}
			ENDHLSL
		}
	}
}
