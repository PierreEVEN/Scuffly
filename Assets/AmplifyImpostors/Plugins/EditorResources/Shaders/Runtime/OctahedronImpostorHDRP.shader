// Amplify Impostors
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

Shader "Hidden/Amplify Impostors/Octahedron Impostor HDRP"
{
	Properties
	{
		[NoScaleOffset] _Albedo("Albedo & Alpha", 2D) = "white" {}
		[NoScaleOffset]_Normals("Normals & Depth", 2D) = "white" {}
		[NoScaleOffset]_Specular("Specular & Smoothness", 2D) = "black" {}
		[NoScaleOffset]_Emission("Emission & Occlusion", 2D) = "black" {}
		[NoScaleOffset]_Features("Diffusion & Features", 2D) = "black" {}
		[HideInInspector]_Frames("Frames", Float) = 16
		[HideInInspector]_ImpostorSize("Impostor Size", Float) = 1
		[HideInInspector]_Offset("Offset", Vector) = (0,0,0,0)
		[HideInInspector]_AI_SizeOffset("Size & Offset", Vector) = (0,0,0,0)
		_TextureBias("Texture Bias", Float) = -1
		_Parallax("Parallax", Range(-1 , 1)) = 1
		[HideInInspector]_DepthSize("DepthSize", Float) = 1
		_ClipMask("Clip", Range(0 , 1)) = 0.5
		_AI_ShadowBias("Shadow Bias", Range(0 , 2)) = 0.25
		_AI_ShadowView("Shadow View", Range(0 , 1)) = 1
		[Toggle(_HEMI_ON)] _Hemi("Hemi", Float) = 0
		[Toggle(EFFECT_HUE_VARIATION)] _Hue("Use SpeedTree Hue", Float) = 0
		_HueVariation("Hue Variation", Color) = (0,0,0,0)
		[ToggleUI] _EnergyConservingSpecularColor("Energy Conserving Specular", Float) = 1.0
		[Toggle] _AI_AlphaToCoverage("Alpha To Coverage", Float) = 0.0
		[Toggle(AI_CLIP_NEIGHBOURS_FRAMES)] AI_CLIP_NEIGHBOURS_FRAMES("Clip Neighbours Frames", Float) = 0
		[HideInInspector]_StencilForwardRef("Forward Ref", Int) = 1
		[HideInInspector]_StencilForwardMask("Forward Mask", Int) = 51
		[HideInInspector]_StencilMotionRef("Motion Ref", Int) = 128
		[HideInInspector]_StencilMotionMask("Motion Mask", Int) = 176
		[HideInInspector]_StencilDepthRef("Depth Ref", Int) = 0
		[HideInInspector]_StencilDepthMask("Depth Mask", Int) = 48
		[HideInInspector]_StencilGBufferRef("GBuffer Ref", Int) = 1
		[HideInInspector]_StencilGBufferMask("GBuffer Mask", Int) = 51
	}

		SubShader
		{
			Tags {
				"RenderPipeline" = "HDRenderPipeline"
				"RenderType" = "Opaque"
				"Queue" = "Geometry+1" // +1 so they render together and have a higher change of instancing
				"DisableBatching" = "True"
			}

			Blend One Zero
			Cull Back
			ZTest LEqual
			ZWrite On
			AlphaToMask[_AI_AlphaToCoverage]

			HLSLINCLUDE
			#pragma target 4.5
			#pragma only_renderers d3d11 ps4 xboxone vulkan metal switch
			#pragma multi_compile_instancing
			#pragma instancing_options procedural:setup

			#pragma multi_compile _ LOD_FADE_CROSSFADE

			#pragma shader_feature _HEMI_ON
			#pragma shader_feature EFFECT_HUE_VARIATION
			#pragma shader_feature AI_CLIP_NEIGHBOURS_FRAMES

			struct SurfaceOutputStandardSpecular
			{
				half3 Albedo;
				half3 Specular;
				float3 Normal;
				half3 Emission;
				half Smoothness;
				half Occlusion;
				half Alpha;
				float3 Diffusion;
				float Features;
				float MetalTangent;
			};

			struct Features
			{
				uint pixelFeatureFlags;
				bool pixelHasSubsurface;
				bool pixelHasTransmission;
				bool pixelHasAnisotropy;
				bool pixelHasIridescence;
				bool pixelHasClearCoat;
				bool pixelSpecularColor;
			};

			struct GlobalSurfaceDescription
			{
				float3 Albedo;
				float3 Normal;
				float3 BentNormal;
				float3 Specular;
				float CoatMask;
				float Metallic;
				float3 Emission;
				float Smoothness;
				float Occlusion;
				float Alpha;
				float AlphaClipThreshold;
				float AlphaClipThresholdShadow;
				float AlphaClipThresholdDepthPrepass;
				float AlphaClipThresholdDepthPostpass;
				float SpecularAAScreenSpaceVariance;
				float SpecularAAThreshold;
				float SpecularOcclusion;
				float DepthOffset;
				float RefractionIndex;
				float3 RefractionColor;
				float RefractionDistance;
				float Thickness;
				float SubsurfaceMask;
				float DiffusionProfile;
				float Anisotropy;
				float3 Tangent;
				float IridescenceMask;
				float IridescenceThickness;
				float3 BakedGI;
				float3 BakedBackGI;
			};

			ENDHLSL

			Pass
			{
				Name "GBuffer"
				Tags { "LightMode" = "GBuffer" }

				Cull Back
				ZTest LEqual

				Stencil
				{
				   Ref[_StencilGBufferRef]
				   WriteMask[_StencilGBufferMask]
				   Comp Always
				   Pass Replace
				}

				HLSLPROGRAM

				#pragma vertex Vert
				#pragma fragment Frag

				#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
				#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/NormalSurfaceGradient.hlsl"
				#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
				#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"

				#define SHADERPASS SHADERPASS_GBUFFER
				#pragma multi_compile _ LIGHTMAP_ON
				#pragma multi_compile _ DIRLIGHTMAP_COMBINED
				#pragma multi_compile _ DYNAMICLIGHTMAP_ON
				#pragma multi_compile _ SHADOWS_SHADOWMASK
				#pragma multi_compile DECALS_OFF DECALS_3RT DECALS_4RT
				#pragma multi_compile _ LIGHT_LAYERS

				#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
				#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
				#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/Lit.hlsl"

				#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
				#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
				#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/DecalUtilities.hlsl"
				#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/LitDecalData.hlsl"

				#define AI_HD_RENDERPIPELINE

				#include "AmplifyImpostors.cginc"

				#define T2W(var, index) var.tangentToWorld[index]

				struct VertexInput
				{
					float4 vertex		: POSITION;
					float3 normal		: NORMAL;
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};

				struct VertexOutput
				{
					float4 clipPos		: SV_Position;
					#ifdef LOD_FADE_CROSSFADE
					float3 worldPos		: TEXCOORD0;
					#endif
					float4 uvsFrame1	: TEXCOORD5;
					float4 uvsFrame2	: TEXCOORD6;
					float4 uvsFrame3	: TEXCOORD7;
					float4 octaFrame	: TEXCOORD8;
					float4 viewPos		: TEXCOORD9;
					UNITY_VERTEX_INPUT_INSTANCE_ID
					UNITY_VERTEX_OUTPUT_STEREO
				};

				void BuildSurfaceData(Features features, FragInputs fragInputs, inout GlobalSurfaceDescription surfaceDescription, float3 V, out SurfaceData surfaceData, out float3 bentNormalWS)
				{
					ZERO_INITIALIZE(SurfaceData, surfaceData);

					surfaceData.baseColor = surfaceDescription.Albedo;
					surfaceData.perceptualSmoothness = surfaceDescription.Smoothness;
					surfaceData.ambientOcclusion = surfaceDescription.Occlusion;
					surfaceData.metallic = surfaceDescription.Metallic;
					surfaceData.coatMask = surfaceDescription.CoatMask;
					surfaceData.specularOcclusion = surfaceDescription.SpecularOcclusion;
					surfaceData.specularColor = surfaceDescription.Specular;
					surfaceData.thickness = surfaceDescription.Thickness;
					surfaceData.iridescenceMask = surfaceDescription.IridescenceMask;
					surfaceData.iridescenceThickness = surfaceDescription.IridescenceThickness;
					surfaceData.subsurfaceMask = surfaceDescription.SubsurfaceMask;
					surfaceData.diffusionProfileHash = asuint(surfaceDescription.DiffusionProfile);
					surfaceData.anisotropy = surfaceDescription.Anisotropy;

					surfaceData.ior = 1.0;
					surfaceData.transmittanceColor = float3(1.0, 1.0, 1.0);
					surfaceData.atDistance = 1.0;
					surfaceData.transmittanceMask = 0.0;

					surfaceData.materialFeatures = features.pixelFeatureFlags;

					if (HasFlag(features.pixelFeatureFlags, MATERIALFEATUREFLAGS_LIT_SPECULAR_COLOR))
						surfaceData.baseColor *= _EnergyConservingSpecularColor > 0.0 ? (1.0 - Max3(surfaceData.specularColor.r, surfaceData.specularColor.g, surfaceData.specularColor.b)) : 1.0;

					surfaceData.normalWS = surfaceDescription.Normal;

					bentNormalWS = surfaceData.normalWS;
					surfaceData.geomNormalWS = T2W(fragInputs, 2);

					surfaceData.tangentWS = normalize(T2W(fragInputs, 0).xyz);

					if (HasFlag(features.pixelFeatureFlags, MATERIALFEATUREFLAGS_LIT_ANISOTROPY))
						surfaceData.tangentWS = surfaceDescription.Tangent;

					surfaceData.tangentWS = Orthonormalize(surfaceData.tangentWS, surfaceData.normalWS);
				}

				void GetSurfaceAndBuiltinData(Features features, GlobalSurfaceDescription surfaceDescription, FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData)
				{
					float3 bentNormalWS;
					BuildSurfaceData(features, fragInputs, surfaceDescription, V, surfaceData, bentNormalWS);

					#if HAVE_DECALS
					if (_EnableDecals)
					{
						DecalSurfaceData decalSurfaceData = GetDecalSurfaceData(posInput, fragInputs.tangentToWorld[2], surfaceDescription.Alpha);
						ApplyDecalToSurfaceData(decalSurfaceData, fragInputs.tangentToWorld[2], surfaceData);
					}
					#endif

					InitBuiltinData(posInput, surfaceDescription.Alpha, surfaceData.normalWS, -T2W(fragInputs, 2), fragInputs.texCoord1, fragInputs.texCoord2, builtinData);

					builtinData.emissiveColor = surfaceDescription.Emission;
					builtinData.depthOffset = 0.0;
					builtinData.distortion = float2(0.0, 0.0);
					builtinData.distortionBlur = 0.0;

					PostInitBuiltinData(V, posInput, surfaceData, builtinData);
				}

				#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
					StructuredBuffer<float4x4> matrixBuffer;
				#endif


				void setup() {
					#define unity_ObjectToWorld unity_ObjectToWorld
					#define unity_WorldToObject unity_WorldToObject

					#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
						float4x4 data = matrixBuffer[unity_InstanceID];

						unity_ObjectToWorld = data;

						unity_WorldToObject = unity_ObjectToWorld;
						unity_WorldToObject._14_24_34 *= -1;
						unity_WorldToObject._11_22_33 = 1.0f / unity_WorldToObject._11_22_33;

					#endif
				}

				VertexOutput Vert(VertexInput v)
				{
					VertexOutput o;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_TRANSFER_INSTANCE_ID(v, o);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

					OctaImpostorVertex(v.vertex, v.normal, o.uvsFrame1, o.uvsFrame2, o.uvsFrame3, o.octaFrame, o.viewPos);

					float3 positionRWS = TransformObjectToWorld(v.vertex.xyz);
					float4 positionCS = TransformWorldToHClip(positionRWS);

					o.clipPos = positionCS;
					#ifdef LOD_FADE_CROSSFADE
						o.worldPos = positionRWS;
					#endif
					return o;
				}

				void Frag(VertexOutput IN, OUTPUT_GBUFFER(outGBuffer), out float outputDepth : SV_Depth)
				{
					UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
					UNITY_SETUP_INSTANCE_ID(IN);
					FragInputs input;
					ZERO_INITIALIZE(FragInputs, input);

					#ifdef LOD_FADE_CROSSFADE
						float3 VC = GetWorldSpaceNormalizeViewDir(IN.worldPos);
						LODDitheringTransition(ComputeFadeMaskSeed(VC, IN.clipPos.xy), unity_LODFade.x);
					#endif

					SurfaceOutputStandardSpecular o = (SurfaceOutputStandardSpecular)0;
					o.Normal = float3(0, 0, 1);
					float4 clipPos = 0;
					float3 worldPos = 0;

					OctaImpostorFragment(o, clipPos, worldPos, IN.uvsFrame1, IN.uvsFrame2, IN.uvsFrame3, IN.octaFrame, IN.viewPos);
					IN.clipPos.zw = clipPos.zw;
					clipPos = IN.clipPos;

					float3 normalWS = o.Normal;

					input.positionSS = IN.clipPos;
					input.positionRWS = worldPos;
					input.tangentToWorld = k_identity3x3;

					PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS.xyz);

					GlobalSurfaceDescription surfaceDescription = (GlobalSurfaceDescription)0;
					surfaceDescription.Albedo = o.Albedo;
					surfaceDescription.Normal = o.Normal;
					surfaceDescription.BentNormal = float3(0, 0, 1);
					surfaceDescription.Specular = SRGBToLinear(o.Specular); //marked as linear due to other kinds of data being present
					surfaceDescription.CoatMask = 0;
					surfaceDescription.Metallic = 0;

					surfaceDescription.Emission = o.Emission;
					surfaceDescription.Emission *= GetInverseCurrentExposureMultiplier();

					surfaceDescription.Smoothness = o.Smoothness;
					surfaceDescription.Occlusion = o.Occlusion;
					surfaceDescription.SpecularOcclusion = o.Occlusion;
					surfaceDescription.Alpha = o.Alpha;

					surfaceDescription.Thickness = 1;
					surfaceDescription.SubsurfaceMask = 1;

					surfaceDescription.Anisotropy = 1;
					surfaceDescription.Tangent = float3(1, 0, 0);

					float coatMask;
					uint materialFeatureId;
					uint tileFeatureFlags = UINT_MAX;
					tileFeatureFlags &= MATERIAL_FEATURE_MASK_FLAGS;
					UnpackFloatInt8bit(o.Features, 8, coatMask, materialFeatureId);

					surfaceDescription.CoatMask = coatMask;

					uint pixelFeatureFlags = MATERIALFEATUREFLAGS_LIT_STANDARD;
					bool pixelHasSubsurface = materialFeatureId == GBUFFER_LIT_TRANSMISSION_SSS || materialFeatureId == GBUFFER_LIT_SSS;
					bool pixelHasTransmission = materialFeatureId == GBUFFER_LIT_TRANSMISSION_SSS || materialFeatureId == GBUFFER_LIT_TRANSMISSION;
					bool pixelHasAnisotropy = materialFeatureId == GBUFFER_LIT_ANISOTROPIC;
					bool pixelHasIridescence = materialFeatureId == GBUFFER_LIT_IRIDESCENCE;
					bool pixelHasClearCoat = coatMask > 0.0;
					bool pixelSpecularColor = !(pixelHasSubsurface || pixelHasAnisotropy || pixelHasIridescence || pixelHasTransmission);
					bool pixelHasMetallic = (pixelHasAnisotropy || pixelHasIridescence);

					pixelFeatureFlags |= tileFeatureFlags & (pixelSpecularColor ? MATERIALFEATUREFLAGS_LIT_SPECULAR_COLOR : 0);
					pixelFeatureFlags |= tileFeatureFlags & (pixelHasSubsurface ? MATERIALFEATUREFLAGS_LIT_SUBSURFACE_SCATTERING : 0);
					pixelFeatureFlags |= tileFeatureFlags & (pixelHasTransmission ? MATERIALFEATUREFLAGS_LIT_TRANSMISSION : 0);
					pixelFeatureFlags |= tileFeatureFlags & (pixelHasAnisotropy ? MATERIALFEATUREFLAGS_LIT_ANISOTROPY : 0);
					pixelFeatureFlags |= tileFeatureFlags & (pixelHasIridescence ? MATERIALFEATUREFLAGS_LIT_IRIDESCENCE : 0);
					pixelFeatureFlags |= tileFeatureFlags & (pixelHasClearCoat ? MATERIALFEATUREFLAGS_LIT_CLEAR_COAT : 0);

					Features features = (Features)0;
					features.pixelFeatureFlags = pixelFeatureFlags;

					if (HasFlag(pixelFeatureFlags, MATERIALFEATUREFLAGS_LIT_TRANSMISSION | MATERIALFEATUREFLAGS_LIT_SUBSURFACE_SCATTERING))
					{
						uint r = UnpackInt(o.Diffusion.r, 8) << 16;
						uint g = UnpackInt(o.Diffusion.g, 8) << 8;
						uint b = UnpackInt(o.Diffusion.b, 8);
						uint hash = (0x40 << 24) | r | g | b; // 0x0100 0000 + mantissa

						float subsurfaceMask;
						uint diffusionProfileIndex;
						UnpackFloatInt8bit(o.MetalTangent, 16, subsurfaceMask, diffusionProfileIndex);
						surfaceDescription.DiffusionProfile = asfloat(hash);
						surfaceDescription.SubsurfaceMask = subsurfaceMask;

						surfaceDescription.SpecularOcclusion = o.Specular.r;
						surfaceDescription.Occlusion = o.Specular.r;
					}

					if (HasFlag(pixelFeatureFlags, MATERIALFEATUREFLAGS_LIT_IRIDESCENCE | MATERIALFEATUREFLAGS_LIT_ANISOTROPY))
					{
						float3x3 frame = GetLocalFrame(normalWS);
						surfaceDescription.Anisotropy = o.Specular.r * 2.0 - 1.0;

						float metallic;
						uint tangentFlags;
						UnpackFloatInt8bit(o.MetalTangent, 8, metallic, tangentFlags);

						uint  quadrant = tangentFlags;
						uint  storeSin = tangentFlags & 4;
						float sinOrCos = o.Specular.g * rsqrt(2);
						float cosOrSin = sqrt(1 - sinOrCos * sinOrCos);
						float sinFrame = storeSin ? sinOrCos : cosOrSin;
						float cosFrame = storeSin ? cosOrSin : sinOrCos;
							  sinFrame = (quadrant & 1) ? -sinFrame : sinFrame;
							  cosFrame = (quadrant & 2) ? -cosFrame : cosFrame;

						frame[0] = sinFrame * frame[1] + cosFrame * frame[0];
						frame[1] = cross(frame[2], frame[0]);
						surfaceDescription.Tangent = frame[0];
						surfaceDescription.Metallic = metallic;
					}

					if (HasFlag(pixelFeatureFlags, MATERIALFEATUREFLAGS_LIT_IRIDESCENCE))
					{
						surfaceDescription.IridescenceMask = o.Specular.r;
						surfaceDescription.IridescenceThickness = o.Specular.g;
					}

					if (HasFlag(pixelFeatureFlags, MATERIALFEATUREFLAGS_LIT_IRIDESCENCE | MATERIALFEATUREFLAGS_LIT_TRANSMISSION | MATERIALFEATUREFLAGS_LIT_SUBSURFACE_SCATTERING))
					{
						surfaceDescription.Thickness = o.Specular.g;
					}

					float3 V = GetWorldSpaceNormalizeViewDir(input.positionRWS);

					SurfaceData surfaceData;
					BuiltinData builtinData;

					GetSurfaceAndBuiltinData(features, surfaceDescription, input, V, posInput, surfaceData, builtinData);
					ENCODE_INTO_GBUFFER(surfaceData, builtinData, posInput.positionSS, outGBuffer);

					outputDepth = posInput.deviceDepth;
				}
				ENDHLSL
			}

			Pass
			{
				Name "ShadowCaster"
				Tags { "LightMode" = "ShadowCaster" }
				Cull Back
				ZWrite On
					//ZClip [_ZClip]
					ColorMask 0

					HLSLPROGRAM
					#pragma vertex Vert
					#pragma fragment Frag

					#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
					#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/NormalSurfaceGradient.hlsl"
					#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
					#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"

					#define SHADERPASS SHADERPASS_SHADOWS
					#define USE_LEGACY_UNITY_MATRIX_VARIABLES

					#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
					#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
					#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/Lit.hlsl"

					#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
					#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
					#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/DecalUtilities.hlsl"
					#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/LitDecalData.hlsl"
					#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"

					#define AI_HD_RENDERPIPELINE

					#include "AmplifyImpostors.cginc"

					#define T2W(var, index) var.tangentToWorld[index]

					#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
					StructuredBuffer<float4x4> matrixBuffer;
					#endif


					void setup() {
						#define unity_ObjectToWorld unity_ObjectToWorld
						#define unity_WorldToObject unity_WorldToObject

						#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
							float4x4 data = matrixBuffer[unity_InstanceID];

							unity_ObjectToWorld = data;

							unity_WorldToObject = unity_ObjectToWorld;
							unity_WorldToObject._14_24_34 *= -1;
							unity_WorldToObject._11_22_33 = 1.0f / unity_WorldToObject._11_22_33;

						#endif
					}

					struct VertexInput
					{
						float4 vertex		: POSITION;
						float3 normal		: NORMAL;
						UNITY_VERTEX_INPUT_INSTANCE_ID
					};

					struct VertexOutput
					{
						float4 clipPos		: SV_Position;
						#ifdef LOD_FADE_CROSSFADE
						float3 worldPos		: TEXCOORD0;
						#endif
						float4 uvsFrame1	: TEXCOORD5;
						float4 uvsFrame2	: TEXCOORD6;
						float4 uvsFrame3	: TEXCOORD7;
						float4 octaFrame	: TEXCOORD8;
						float4 viewPos		: TEXCOORD9;
						UNITY_VERTEX_INPUT_INSTANCE_ID
						UNITY_VERTEX_OUTPUT_STEREO
					};

					VertexOutput Vert(VertexInput v)
					{
						VertexOutput o;
						UNITY_SETUP_INSTANCE_ID(v);
						UNITY_TRANSFER_INSTANCE_ID(v, o);
						UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

						OctaImpostorVertex(v.vertex, v.normal, o.uvsFrame1, o.uvsFrame2, o.uvsFrame3, o.octaFrame, o.viewPos);

						float3 positionRWS = TransformObjectToWorld(v.vertex.xyz);
						float4 positionCS = TransformWorldToHClip(positionRWS);

						o.clipPos = positionCS;
						#ifdef LOD_FADE_CROSSFADE
							o.worldPos = positionRWS;
						#endif
						return o;
					}

					void Frag(VertexOutput IN, out float outputDepth : SV_Depth)
					{
						UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
						UNITY_SETUP_INSTANCE_ID(IN);
						FragInputs input;
						ZERO_INITIALIZE(FragInputs, input);

						#ifdef LOD_FADE_CROSSFADE
							float3 VC = GetWorldSpaceNormalizeViewDir(IN.worldPos);
							LODDitheringTransition(ComputeFadeMaskSeed(VC, IN.clipPos.xy), unity_LODFade.x);
						#endif

						SurfaceOutputStandardSpecular o = (SurfaceOutputStandardSpecular)0;
						o.Normal = float3(0, 0, 1);
						float4 clipPos = 0;
						float3 worldPos = 0;

						OctaImpostorFragment(o, clipPos, worldPos, IN.uvsFrame1, IN.uvsFrame2, IN.uvsFrame3, IN.octaFrame, IN.viewPos);
						IN.clipPos.zw = clipPos.zw;
						clipPos = IN.clipPos;

						float3 normalWS = o.Normal;

						input.positionSS = IN.clipPos;
						input.positionRWS = worldPos;
						input.tangentToWorld = k_identity3x3;

						PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS.xyz);

						outputDepth = posInput.deviceDepth;
					}
					ENDHLSL
				}


		}
}
