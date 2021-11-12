Shader "HDRP/GpuLandscapeShader"
{
	Properties
	{
		[NoScaleOffset] _GrassAlbedo("Grass_Albedo", 2D) = "white" {}
		[NoScaleOffset] _Grass2Albedo("Grass2_Albedo", 2D) = "white" {}
		[NoScaleOffset] _RockAlbedo("Rock_Albedo", 2D) = "white" {}
		[NoScaleOffset] _SnowAlbedo("Snow_Albedo", 2D) = "white" {}
		[NoScaleOffset] _SandAlbedo("Sand_Albedo", 2D) = "white" {}
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

				// Per vertex normal
				// float altX = max(0, GetAltitudeAtLocation(OUT.worldPosition.xz + float2(1, 0)));
				// float altZ = max(0, GetAltitudeAtLocation(OUT.worldPosition.xz + float2(0, 1)));
				// OUT.normalWS = normalize(cross(float3(-1, altX - OUT.worldPosition.y, 0), float3(0, altZ - OUT.worldPosition.y, 1)));

				float3 finalWorldPos = OUT.worldPosition;
				if (finalWorldPos.y < 0) finalWorldPos.y = 0;

				OUT.positionCS = TransformWorldToHClip(GetCameraRelativePositionWS(finalWorldPos));

				return OUT;
			}

			sampler2D _GrassAlbedo;
			sampler2D _Grass2Albedo;
			sampler2D _RockAlbedo;
			sampler2D _SnowAlbedo;
			sampler2D _SandAlbedo;









#define DRAG_MULT 0.048
#define ITERATIONS_RAYMARCH 13
#define ITERATIONS_NORMAL 48
#define Mouse float2(0, 0)
#define WaterScale 0.05


			// Ocean shader from shadertoy : https://www.shadertoy.com/view/MdXyzX?fbclid=IwAR3qcC_lm5XNVqPD0omLxhQ-cJIRnH-Dh2l2OTdC2SiJwvJ18PPFA-DeFmY
			float2 wavedx(float2 position, float2 direction, float speed, float frequency, float timeshift) {
				float x = dot(direction, position) * frequency + timeshift * speed;
				float wave = exp(sin(x) - 1.0);
				float dx = wave * cos(x);
				return float2(wave, -dx);
			}

			float getwaves(float2 position, int iterations) {
				float iter = 0.0;
				float phase = 6.0;
				float speed = 2.0;
				float weight = 1.0;
				float w = 0.0;
				float ws = 0.0;
				for (int i = 0; i < iterations; i++) {
					float2 p = float2(sin(iter), cos(iter));
					float2 res = wavedx(position, p, speed, phase, _Time * 10);
					position += normalize(p) * res.y * weight * DRAG_MULT;
					w += res.x * weight;
					iter += 12.0;
					ws += weight;
					weight = lerp(weight, 0.0, 0.2);
					phase *= 1.18;
					speed *= 1.07;
				}
				return w / ws;
			}

			float raymarchwater(float3 camera, float3 start, float3 end, float depth) {
				float3 pos = start;
				float h = 0.0;
				float hupper = depth;
				float hlower = 0.0;
				float2 zer = float2(0.0, 0);
				float3 dir = normalize(end - start);
				for (int i = 0; i < 318; i++) {
					h = getwaves(pos.xz * WaterScale, ITERATIONS_RAYMARCH) * depth - depth;
					if (h + 0.01 > pos.y) {
						return distance(pos, camera);
					}
					pos += dir * (pos.y - h);
				}
				return -1.0;
			}

			float H = 0.0;
			float3 getNormal(float2 pos, float e, float depth) {
				float2 ex = float2(e, 0);
				H = getwaves(pos.xy * WaterScale, ITERATIONS_NORMAL) * depth;
				float3 a = float3(pos.x, H, pos.y);
				return normalize(cross(normalize(a - float3(pos.x - e, getwaves(pos.xy * WaterScale - ex.xy * WaterScale, ITERATIONS_NORMAL) * depth, pos.y)),
					normalize(a - float3(pos.x, getwaves(pos.xy * WaterScale + ex.yx * WaterScale, ITERATIONS_NORMAL) * depth, pos.y + e))));
			}
			float3x3 rotmat(float3 axis, float angle)
			{
				axis = normalize(axis);
				float s = sin(angle);
				float c = cos(angle);
				float oc = 1.0 - c;
				return float3x3(oc * axis.x * axis.x + c, oc * axis.x * axis.y - axis.z * s, oc * axis.z * axis.x + axis.y * s,
					oc * axis.x * axis.y + axis.z * s, oc * axis.y * axis.y + c, oc * axis.y * axis.z - axis.x * s,
					oc * axis.z * axis.x - axis.y * s, oc * axis.y * axis.z + axis.x * s, oc * axis.z * axis.z + c);
			}

			float3 getRay(float2 uv) {
				uv = (uv * 2.0 - 1.0) * float2(_ScreenParams.x / _ScreenParams.y, 1.0);
				float3 proj = normalize(float3(uv.x, uv.y, 1.0) + float3(uv.x, uv.y, -1.0) * pow(length(uv), 2.0) * 0.05);
				if (_ScreenParams.x < 400.0) return proj;
				float3 ray = mul(rotmat(float3(0.0, -1.0, 0.0), 3.0 * (Mouse.x * 2.0 - 1.0)), mul(rotmat(float3(1.0, 0.0, 0.0), 1.5 * (Mouse.y * 2.0 - 1.0)), proj));
				return ray;
			}

			float intersectPlane(float3 origin, float3 direction, float3 ppoint, float3 normal)
			{
				return clamp(dot(ppoint - origin, normal) / dot(direction, normal), -1.0, 9991999.0);
			}

			float3 extra_cheap_atmosphere(float3 raydir, float3 sundir) {
				sundir.y = max(sundir.y, -0.07);
				float special_trick = 1.0 / (raydir.y * 1.0 + 0.1);
				float special_trick2 = 1.0 / (sundir.y * 11.0 + 1.0);
				float raysundt = pow(abs(dot(sundir, raydir)), 2.0);
				float sundt = pow(max(0.0, dot(sundir, raydir)), 8.0);
				float mymie = sundt * special_trick * 0.2;
				float3 suncolor = lerp(float3(1.0, 1, 1), max(float3(0.0, 0, 0), float3(1.0, 1, 1) - float3(5.5, 13.0, 22.4) / 22.4), special_trick2);
				float3 bluesky = float3(5.5, 13.0, 22.4) / 22.4 * suncolor;
				float3 bluesky2 = max(float3(0.0, 0, 0), bluesky - float3(5.5, 13.0, 22.4) * 0.002 * (special_trick + -6.0 * sundir.y * sundir.y));
				bluesky2 *= special_trick * (0.24 + raysundt * 0.24);
				return bluesky2 * (1.0 + 1.0 * pow(1.0 - raydir.y, 3.0)) + mymie * suncolor;
			}
			float3 getatm(float3 ray) {
				return extra_cheap_atmosphere(ray, normalize(float3(1.0, 1, 1))) * 0.5;

			}

			float3 aces_tonemap(float3 color) {
				float3x3 m1 = float3x3(
					0.59719, 0.07600, 0.02840,
					0.35458, 0.90834, 0.13383,
					0.04823, 0.01566, 0.83777
					);
				float3x3 m2 = float3x3(
					1.60475, -0.10208, -0.00327,
					-0.53108, 1.10813, -0.07276,
					-0.07367, -0.00605, 1.07602
					);
				float3 v = mul(m1, color);
				float3 a = v * (v + 0.0245786) - 0.000090537;
				float3 b = v * (0.983729 * v + 0.4329510) + 0.238081;
				return pow(clamp(mul(m2, (a / b)), 0.0, 1.0), float3(1.0 / 2.2, 1.0 / 2.2, 1.0 / 2.2));
			}
			void mainImage(out float4 fragColor, float2 uv)
			{
				float waterdepth = 2.1;
				float3 wfloor = float3(0.0, -waterdepth, 0.0);
				float3 wceil = float3(0.0, 0.0, 0.0);
				float3 orig = float3(0.0, 2.0, 0.0);
				float3 ray = getRay(uv);
				float hihit = intersectPlane(orig, ray, wceil, float3(0.0, 1.0, 0.0));
				if (ray.y >= -0.01) {
					float3 C = float3(getatm(ray)) * 2.0;
					//tonemapping
					C = aces_tonemap(C);
					fragColor = float4(C, 1.0);
					return;
				}
				float lohit = intersectPlane(orig, ray, wfloor, float3(0.0, 1.0, 0.0));
				float3 hipos = orig + ray * hihit;
				float3 lopos = orig + ray * lohit;
				float dist = raymarchwater(orig, hipos, lopos, waterdepth);
				float3 pos = orig + ray * dist;

				float3 N = getNormal(pos.xz, 0.001, waterdepth);
				float2 velocity = N.xz * (1.0 - N.y);
				N = lerp(float3(0.0, 1.0, 0.0), N, 1.0 / (dist * dist * 0.01 + 1.0));
				float3 R = reflect(ray, N);
				float fresnel = (0.04 + (1.0 - 0.04) * (pow(1.0 - max(0.0, dot(-N, ray)), 5.0)));

				float3 C = fresnel * getatm(R) * 2.0;
				//tonemapping
				C = aces_tonemap(C);

				fragColor = float4(C, 1.0);
			}






































			void Frag(VertexOutput IN, OUTPUT_GBUFFER(outGBuffer))
			{
				float cameraDistance = length(_WorldSpaceCameraPos - IN.worldPosition);

				float normalFetchDistance = clamp(cameraDistance / 100, 1, 200);

				// Per pixel normal
				float altX = max(0, GetAltitudeAtLocation(IN.worldPosition.xz + float2(normalFetchDistance, 0)));
				float altZero = max(0, GetAltitudeAtLocation(IN.worldPosition.xz));
				float altZ = max(0, GetAltitudeAtLocation(IN.worldPosition.xz + float2(0, normalFetchDistance)));
				IN.normalWS = normalize(cross(float3(-normalFetchDistance, altX - altZero, 0), float3(0, altZ - altZero, normalFetchDistance)));

				FragInputs input;
				ZERO_INITIALIZE(FragInputs, input);
				input.positionSS = IN.positionCS;
				input.positionRWS = IN.worldPosition;
				input.tangentToWorld = k_identity3x3;

				PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS.xyz);

				float alpha = 1.0;
				float smoothness = 0;
				float metalness = 0;


				float3 color = tex2D(_GrassAlbedo, IN.worldPosition.xz * 0.1);
				color = lerp(tex2D(_Grass2Albedo, IN.worldPosition.xz * 0.1), color, (snoise(IN.worldPosition * 0.003) * snoise(IN.worldPosition * 0.001)) * 0.5 + 0.5);
				// add snow
				color = lerp(color, tex2D(_SnowAlbedo, IN.worldPosition.xz * 0.1), clamp((IN.worldPosition.y - 1000 + snoise(IN.worldPosition.xz * 0.001) * 100) * 0.001, 0, 1));

				// add rock
				color = lerp(tex2D(_RockAlbedo, IN.worldPosition.xz * 0.1), color, max(pow(dot(IN.normalWS, float3(0, 1, 0)), 15), 0));;

				// add beach
				color = lerp(tex2D(_SandAlbedo, IN.worldPosition.xz * 0.1), color, clamp(IN.worldPosition.y * 0.2 - 2, 0, 1));

				float3 normal = IN.normalWS;

				// Darker wide pos
				color = pow(color, max(1, min(2, pow(cameraDistance / 5000, 0.5))));


				if (IN.worldPosition.y < 0) {

					float currentWaterDepth = 0.4;

					float3 forward = normalize(IN.worldPosition - _WorldSpaceCameraPos);

					float hihit = intersectPlane(_WorldSpaceCameraPos, forward, float3(0, currentWaterDepth, 0), float3(0.0, 1.0, 0.0));
					float lohit = intersectPlane(_WorldSpaceCameraPos, forward, float3(0, 0, 0), float3(0.0, 1.0, 0.0));
					float3 hipos = _WorldSpaceCameraPos + forward * hihit;
					float3 lopos = _WorldSpaceCameraPos + forward * lohit;
					float dist = raymarchwater(_WorldSpaceCameraPos, hipos, lopos, currentWaterDepth);
					float3 N = getNormal(IN.worldPosition.xz, 0.001, currentWaterDepth);
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
				surfaceData.baseColor = float3(color);
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
				//surfaceData.materialFeatures = MATERIALFEATUREFLAGS_LIT_SPECULAR_COLOR;

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

			// Render State
				Cull Back
				ZWrite On
			//ZClip [_ZClip]
			ColorMask 0
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


					#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
					#define SHADERPASS SHADERPASS_SHADOWS
					#define USE_LEGACY_UNITY_MATRIX_VARIABLES

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

				return OUT;
			}

			void Frag(VertexOutput IN, out float outputDepth : SV_Depth)
			{
				FragInputs input;
				ZERO_INITIALIZE(FragInputs, input);
				input.positionSS = IN.positionCS;
				input.positionRWS = IN.worldPosition;
				input.tangentToWorld = k_identity3x3;

				PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS.xyz);

				outputDepth = posInput.deviceDepth;
			}
			ENDHLSL
		}
	}
}
