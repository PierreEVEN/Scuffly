Shader /*ase_name*/ "Hidden/Impostors/Runtime/HighDefinition PBR" /*end*/
{
    Properties
    {
		/*ase_props*/
    }

    SubShader
    {
		/*ase_subshader_options:Name=Additional Options
			Port:GBuffer:Bent Normal
				On:SetDefine:ASE_BENT_NORMAL 1
			Port:GBuffer:Coat Mask
				On:SetDefine:ASE_LIT_CLEAR_COAT 1
			Port:GBuffer:Occlusion
				On:SetDefine:_AMBIENT_OCCLUSION 1
			Port:GBuffer:Alpha Clip Threshold
				On:SetDefine:SHADERPASS_GBUFFER_BYPASS_ALPHA_TEST
			Option:Material Type,InvertActionOnDeselection:Standard,Subsurface Scattering,Anisotropy,Iridescence,Specular Color,Translucent:Specular Color
				Standard:ShowPort:GBuffer:Metallic
				Subsurface Scattering:SetDefine:_MATERIAL_FEATURE_SUBSURFACE_SCATTERING 1
				Subsurface Scattering:SetPropertyOnPass:GBuffer:StencilReference,1
				Subsurface Scattering:SetPropertyOnPass:Forward:StencilReference,1
				Subsurface Scattering:ShowPort:GBuffer:SubsurfaceMask
				Subsurface Scattering:ShowPort:GBuffer:DiffusionProfile
				Subsurface Scattering:ShowOption:Transmission
				Anisotropy:SetDefine:_MATERIAL_FEATURE_ANISOTROPY 1
				Anisotropy:ShowPort:GBuffer:Tangent
				Anisotropy:ShowPort:GBuffer:Anisotropy
				Anisotropy:ShowPort:GBuffer:Metallic
				Iridescence:SetDefine:_MATERIAL_FEATURE_IRIDESCENCE 1
				Iridescence:ShowPort:GBuffer:IridescenceMask
				Iridescence:ShowPort:GBuffer:IridescenceThickness
				Iridescence:ShowPort:GBuffer:Metallic
				Specular Color:SetDefine:_MATERIAL_FEATURE_SPECULAR_COLOR 1
				Specular Color:ShowPort:GBuffer:Specular
				Specular Color:ShowOption:Energy Conserving Specular
				Translucent:SetDefine:_MATERIAL_FEATURE_TRANSMISSION 1
				Translucent:ShowPort:GBuffer:Thickness
				Translucent:ShowPort:GBuffer:DiffusionProfile
			Option:Energy Conserving Specular,InvertActionOnDeselection:false,true:false
				true:SetDefine:_ENERGY_CONSERVING_SPECULAR 1
			Option:Transmission,InvertActionOnDeselection:false,true:false
				true:ShowPort:GBuffer:Thickness
				true:SetDefine:_MATERIAL_FEATURE_TRANSMISSION 1
			Option:Surface Type:Opaque,Transparent:Opaque
				Opaque:SetPropertyOnSubShader:RenderType,Opaque
				Opaque:SetPropertyOnSubShader:RenderQueue,Geometry
				Opaque:HideOption:Distortion,0
				Opaque:HideOption:Blend Preserves Specular
				Opaque:HideOption:Fog
				Transparent:SetPropertyOnSubShader:RenderType,Transparent
				Transparent:SetPropertyOnSubShader:RenderQueue,Transparent
				Transparent:ShowOption:Distortion
				Transparent:ShowOption:Blend Preserves Specular
				Transparent:ShowOption:Fog
			Option:Receive Decals:false,true:true
				true:SetDefine:_DECALS 1
				false:RemoveDefine:_DECALS 1
			Option:Alpha Cutoff:false,true:false
				true:ShowPort:GBuffer:Alpha Clip Threshold
				true:SetDefine:_ALPHATEST_ON 1
				false:HidePort:GBuffer:Alpha Clip Threshold
				false:RemoveDefine:_ALPHATEST_ON 1
			Option:Receives SSR:false,true:true
				false:SetDefine:_DISABLE_SSR 1
				false:SetPropertyOnPass:GBuffer:StencilReference,34
				false:SetPropertyOnPass:GBuffer:StencilWriteMask,39
				false:SetPropertyOnPass:GBuffer:StencilComparison,Always
				false:SetPropertyOnPass:GBuffer:StencilPass,Replace
				false:SetPropertyOnPass:Forward:StencilReference,34
				false:SetPropertyOnPass:Forward:StencilWriteMask,39
				false:SetPropertyOnPass:Forward:StencilComparison,Always
				false:SetPropertyOnPass:Forward:StencilPass,Replace
				true:RemoveDefine:_DISABLE_SSR 1
				true:SetPropertyOnPass:GBuffer:StencilReference,2
				true:SetPropertyOnPass:GBuffer:StencilWriteMask,7
				true:SetPropertyOnPass:GBuffer:StencilComparison,Always
				true:SetPropertyOnPass:GBuffer:StencilPass,Replace
				true:SetPropertyOnPass:Forward:StencilReference,2
				true:SetPropertyOnPass:Forward:StencilWriteMask,7
				true:SetPropertyOnPass:Forward:StencilComparison,Always
				true:SetPropertyOnPass:Forward:StencilPass,Replace
			Option:Specular AA:false,true:false
				true:SetDefine:GBuffer:_ENABLE_GEOMETRIC_SPECULAR_AA 1
				true:SetDefine:Forward:_ENABLE_GEOMETRIC_SPECULAR_AA 1
				true:SetDefine:META:_ENABLE_GEOMETRIC_SPECULAR_AA 1
				true:ShowPort:GBuffer:SpecularAAScreenSpaceVariance
				true:ShowPort:GBuffer:SpecularAAThreshold
				false:RemoveDefine:GBuffer:_ENABLE_GEOMETRIC_SPECULAR_AA 1
				false:RemoveDefine:Forward:_ENABLE_GEOMETRIC_SPECULAR_AA 1
				false:RemoveDefine:META:_ENABLE_GEOMETRIC_SPECULAR_AA 1
				false:HidePort:GBuffer:SpecularAAScreenSpaceVariance
				false:HidePort:GBuffer:SpecularAAThreshold
			Option:Specular Occlusion Mode:Off,From AO,From AO And Bent Normal,Custom:Off
				Off:RemoveDefine:_SPECULAR_OCCLUSION_FROM_AO 1
				Off:RemoveDefine:_SPECULAR_OCCLUSION_FROM_AO_BENT_NORMAL 1
				Off:RemoveDefine:_SPECULAR_OCCLUSION_CUSTOM 1
				Off:HidePort:SpecularOcclusion
				From AO:SetDefine:_SPECULAR_OCCLUSION_FROM_AO 1
				From AO:RemoveDefine:_SPECULAR_OCCLUSION_FROM_AO_BENT_NORMAL 1
				From AO:RemoveDefine:_SPECULAR_OCCLUSION_CUSTOM 1
				From AO:HidePort:SpecularOcclusion
				From AO And Bent Normal:RemoveDefine:_SPECULAR_OCCLUSION_FROM_AO 1
				From AO And Bent Normal:SetDefine:_SPECULAR_OCCLUSION_FROM_AO_BENT_NORMAL 1
				From AO And Bent Normal:RemoveDefine:_SPECULAR_OCCLUSION_CUSTOM 1
				From AO And Bent Normal:HidePort:SpecularOcclusion
				Custom:RemoveDefine:_SPECULAR_OCCLUSION_FROM_AO 1
				Custom:RemoveDefine:_SPECULAR_OCCLUSION_FROM_AO_BENT_NORMAL 1
				Custom:SetDefine:_SPECULAR_OCCLUSION_CUSTOM 1
				Custom:ShowPort:SpecularOcclusion
			Option:Distortion:false,true:false
				true:IncludePass:Distortion
				true:ShowOption:Distortion Mode
				true:ShowOption:Distortion Depth Test
				false:ExcludePass:Distortion
				false:HideOption:Distortion Mode
				false:HideOption:Distortion Depth Test
			Option:Distortion Mode:Add,Multiply:Add
				Add:SetPropertyOnPass:Distortion:BlendRGB,One,One
				Add:SetPropertyOnPass:Distortion:BlendAlpha,One,One
				Multiply:SetPropertyOnPass:Distortion:BlendRGB,DstColor,Zero
				Multiply:SetPropertyOnPass:Distortion:BlendAlpha,DstAlpha,Zero
			Option:Distortion Depth Test:false,true:false
				true:SetPropertyOnPass:Distortion:ZTest,LEqual
				false:SetPropertyOnPass:Distortion:ZTest,Always
			Option:Blend Preserves Specular:false,true:true
				true:SetDefine:_BLENDMODE_PRESERVE_SPECULAR_LIGHTING 1
				false:RemoveDefine:_BLENDMODE_PRESERVE_SPECULAR_LIGHTING 1
			Option:Fog:false,true:true
				true:SetDefine:_ENABLE_FOG_ON_TRANSPARENT 1
				false:RemoveDefine:_ENABLE_FOG_ON_TRANSPARENT 1
			Option:Draw Before Refraction:false,true:false
				true:SetPropertyOnSubShader:RenderQueue,Transparent,-250
				true:HideOption:Refraction Model
				false:ShowOption:Refraction Model
			Option:Refraction Model:None,Box,Sphere:None
				None:HidePort:GBuffer:Thickness
				None:HidePort:GBuffer:RefractionIndex
				None:HidePort:GBuffer:RefractionColor
				None:HidePort:GBuffer:RefractionDistance
				None:RemoveDefine:_HAS_REFRACTION 1
				None:RemoveDefine:_REFRACTION_PLANE 1
				None:RemoveDefine:_REFRACTION_SPHERE 1
				Box:ShowPort:GBuffer:Thickness
				Box:ShowPort:GBuffer:RefractionIndex
				Box:ShowPort:GBuffer:RefractionColor
				Box:ShowPort:GBuffer:RefractionDistance
				Box:SetDefine:_HAS_REFRACTION 1
				Box:SetDefine:_REFRACTION_PLANE 1
				Box:RemoveDefine:_REFRACTION_SPHERE 1
				Sphere:ShowPort:GBuffer:Thickness
				Sphere:ShowPort:GBuffer:RefractionIndex
				Sphere:ShowPort:GBuffer:RefractionColor
				Sphere:ShowPort:GBuffer:RefractionDistance
				Sphere:SetDefine:_HAS_REFRACTION 1
				Sphere:RemoveDefine:_REFRACTION_PLANE 1
				Sphere:SetDefine:_REFRACTION_SPHERE 1
			Port:Baked GI
				On:SetDefine:LIGHTMAP_ON 1
				On:SetDefine:DIRLIGHTMAP_COMBINED 1
				On:SetDefine:DYNAMICLIGHTMAP_ON 1
				On:SetDefine:CUSTOM_BAKED_GI 1
		*/
        Tags
        {
            "RenderPipeline"="HDRenderPipeline"
            "RenderType"="Opaque"
            "Queue"="Geometry+0"
        }
        
		Cull Back
		Blend One Zero
		ZTest LEqual
		ZWrite On
		ZClip [_ZClip]

		HLSLINCLUDE
		#pragma target 4.5
		#pragma only_renderers d3d11 ps4 xboxone vulkan metal switch
		#pragma multi_compile_instancing
		#pragma instancing_options renderinglayer
		#pragma multi_compile _ LOD_FADE_CROSSFADE
		#define AI_RENDERPIPELINE

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
			float SpecularAAScreenSpaceVariance;
			float SpecularAAThreshold;
			float SpecularOcclusion;
			//Refraction
			float RefractionIndex;
			float3 RefractionColor;
			float RefractionDistance;
			//SSS/Translucent
			float Thickness;
			float SubsurfaceMask;
			float DiffusionProfile;
			//Anisotropy
			float Anisotropy;
			float3 Tangent;
			//Iridescent
			float IridescenceMask;
			float IridescenceThickness;
		};

		struct SurfaceOutput
		{
			half3 Albedo;
			half3 Specular;
			half Metallic;
			float3 Normal;
			half3 Emission;
			half Smoothness;
			half Occlusion;
			half Alpha;
		};

		struct AlphaSurfaceDescription
		{
			float Alpha;
			float AlphaClipThreshold;
		};

		struct SmoothSurfaceDescription
		{
			float Smoothness;
			float Alpha;
			float AlphaClipThreshold;
		};

		struct DistortionSurfaceDescription
		{
			float Alpha;
			float2 Distortion;
			float DistortionBlur;
			float AlphaClipThreshold;
		};
		ENDHLSL
		/*ase_pass*/
		Pass
        {
			/*ase_main_pass*/
            Name "GBuffer"
            Tags { "LightMode" = "GBuffer" }
           
			Stencil
			{
				WriteMask 7
				Ref  2
				Comp Always
				Pass Replace
			}
        
            HLSLPROGRAM

			#pragma vertex Vert
			#pragma fragment Frag
        
			/*ase_pragma*/

			#if defined(_MATERIAL_FEATURE_SUBSURFACE_SCATTERING) && !defined(_SURFACE_TYPE_TRANSPARENT)
			#define OUTPUT_SPLIT_LIGHTING
			#endif
        
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
			#if defined( ASE_SRP_VERSION ) && ASE_SRP_VERSION < 60900              
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Wind.hlsl"
			#endif
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
        
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TANGENT_TO_WORLD
            #define VARYINGS_NEED_TEXCOORD1
            #define VARYINGS_NEED_TEXCOORD2
        
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/Lit.hlsl"       
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/DecalUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/LitDecalData.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"
            
			#if defined( ASE_SRP_VERSION ) && ASE_SRP_VERSION < 60900
			#define T2W(var, index) var.worldToTangent[index]
			#else
			#define T2W(var, index) var.tangentToWorld[index]
			#endif

			struct AttributesMesh 
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 texcoord : TEXCOORD0;
				float4 uv1 : TEXCOORD1;
				float4 uv2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				/*ase_vdata:p=p;n=n;t=t;uv0=tc0;uv1=tc1;uv2=tc2*/
			};
        
			struct PackedVaryingsMeshToPS 
			{
				float4 positionCS : SV_Position;
				float3 interp00 : TEXCOORD0;
				float3 interp01 : TEXCOORD1;
				float4 interp02 : TEXCOORD2;
				float4 interp03 : TEXCOORD3;
				float4 interp04 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				/*ase_interp(5,):sp=sp.xyzw;rwp=tc0;wn=tc1;wt=tc2;uv1=tc3;uv2=tc4*/
			};

			/*ase_globals*/

			/*ase_funcs*/
    
			void BuildSurfaceData(FragInputs fragInputs, inout GlobalSurfaceDescription surfaceDescription, float3 V, out SurfaceData surfaceData, out float3 bentNormalWS)
			{
				ZERO_INITIALIZE(SurfaceData, surfaceData);
        
				surfaceData.baseColor =                 surfaceDescription.Albedo;
				surfaceData.perceptualSmoothness =      surfaceDescription.Smoothness;
				#ifdef _SPECULAR_OCCLUSION_CUSTOM
					surfaceData.specularOcclusion =         surfaceDescription.SpecularOcclusion;
				#endif
				surfaceData.ambientOcclusion =          surfaceDescription.Occlusion;
				surfaceData.metallic =                  surfaceDescription.Metallic;
				surfaceData.coatMask =                  surfaceDescription.CoatMask;
			
				#ifdef _MATERIAL_FEATURE_IRIDESCENCE		
					surfaceData.iridescenceMask =           surfaceDescription.IridescenceMask;
					surfaceData.iridescenceThickness =      surfaceDescription.IridescenceThickness;
				#endif
					surfaceData.materialFeatures = MATERIALFEATUREFLAGS_LIT_STANDARD;
				#ifdef _MATERIAL_FEATURE_SUBSURFACE_SCATTERING
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_SUBSURFACE_SCATTERING;
				#endif
				#ifdef _MATERIAL_FEATURE_TRANSMISSION
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_TRANSMISSION;
				#endif
				#ifdef _MATERIAL_FEATURE_ANISOTROPY
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_ANISOTROPY;
				#endif

				#ifdef ASE_LIT_CLEAR_COAT
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_CLEAR_COAT;
				#endif

				#ifdef _MATERIAL_FEATURE_IRIDESCENCE
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_IRIDESCENCE;
				#endif
				#ifdef _MATERIAL_FEATURE_SPECULAR_COLOR
					surfaceData.specularColor = surfaceDescription.Specular;
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_SPECULAR_COLOR;
				#endif
        
				#if defined (_MATERIAL_FEATURE_SPECULAR_COLOR) && defined (_ENERGY_CONSERVING_SPECULAR)
					surfaceData.baseColor *= (1.0 - Max3(surfaceData.specularColor.r, surfaceData.specularColor.g, surfaceData.specularColor.b));
				#endif
        
					GetNormalWS( fragInputs, float3( 0.0, 0.0, 1.0 ), surfaceData.normalWS, float3( 1.0, 1.0, 1.0 ) );
        
					bentNormalWS = surfaceData.normalWS;
			
				#ifdef ASE_BENT_NORMAL
					GetNormalWS(fragInputs, surfaceDescription.BentNormal, bentNormalWS, float3( 1, 1, 1 ) );
				#endif
			
				surfaceData.geomNormalWS = T2W(fragInputs, 2);

				#if defined(_HAS_REFRACTION) || defined(_MATERIAL_FEATURE_TRANSMISSION)
					surfaceData.thickness =	                 surfaceDescription.Thickness;
				#endif

				#ifdef _HAS_REFRACTION
					if (_EnableSSRefraction)
					{
						surfaceData.ior =                       surfaceDescription.RefractionIndex;
						surfaceData.transmittanceColor =        surfaceDescription.RefractionColor;
						surfaceData.atDistance =                surfaceDescription.RefractionDistance;
        
						surfaceData.transmittanceMask = (1.0 - surfaceDescription.Alpha);
						surfaceDescription.Alpha = 1.0;
					}
					else
					{
						surfaceData.ior = 1.0;
						surfaceData.transmittanceColor = float3(1.0, 1.0, 1.0);
						surfaceData.atDistance = 1.0;
						surfaceData.transmittanceMask = 0.0;
						surfaceDescription.Alpha = 1.0;
					}
				#else
					surfaceData.ior = 1.0;
					surfaceData.transmittanceColor = float3(1.0, 1.0, 1.0);
					surfaceData.atDistance = 1.0;
					surfaceData.transmittanceMask = 0.0;
				#endif

				#if defined(_HAS_REFRACTION) || defined(_MATERIAL_FEATURE_TRANSMISSION)
					surfaceData.thickness =	                 surfaceDescription.Thickness;
				#endif

				#ifdef _MATERIAL_FEATURE_SUBSURFACE_SCATTERING
					surfaceData.subsurfaceMask =            surfaceDescription.SubsurfaceMask;
					surfaceData.diffusionProfile =          surfaceDescription.DiffusionProfile;
				#endif

					surfaceData.tangentWS = normalize(T2W(fragInputs, 0).xyz);    // The tangent is not normalize in worldToTangent for mikkt. TODO: Check if it expected that we normalize with Morten. Tag: SURFACE_GRADIENT
				#ifdef _MATERIAL_FEATURE_ANISOTROPY	
					surfaceData.anisotropy = surfaceDescription.Anisotropy;
					surfaceData.tangentWS = TransformTangentToWorld(surfaceDescription.Tangent, fragInputs.worldToTangent);
				#endif
					surfaceData.tangentWS = Orthonormalize(surfaceData.tangentWS, surfaceData.normalWS);
			
				#if defined(_SPECULAR_OCCLUSION_CUSTOM)
				#elif defined(_SPECULAR_OCCLUSION_FROM_AO_BENT_NORMAL)
					surfaceData.specularOcclusion = GetSpecularOcclusionFromBentAO(V, bentNormalWS, surfaceData.normalWS, surfaceData.ambientOcclusion, PerceptualSmoothnessToPerceptualRoughness(surfaceData.perceptualSmoothness));
				#elif defined(_AMBIENT_OCCLUSION) && defined(_SPECULAR_OCCLUSION_FROM_AO)
					surfaceData.specularOcclusion = GetSpecularOcclusionFromAmbientOcclusion(ClampNdotV(dot(surfaceData.normalWS, V)), surfaceData.ambientOcclusion, PerceptualSmoothnessToRoughness(surfaceData.perceptualSmoothness));
				#else
					surfaceData.specularOcclusion = 1.0;
				#endif
				#ifdef _ENABLE_GEOMETRIC_SPECULAR_AA
					surfaceData.perceptualSmoothness = GeometricNormalFiltering(surfaceData.perceptualSmoothness, T2W(fragInputs, 2), surfaceDescription.SpecularAAScreenSpaceVariance, surfaceDescription.SpecularAAThreshold);
				#endif
			}
        
			void GetSurfaceAndBuiltinData(GlobalSurfaceDescription surfaceDescription, FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData)
			{
				#ifdef LOD_FADE_CROSSFADE
					uint3 fadeMaskSeed = asuint((int3)(V * _ScreenSize.xyx));
					LODDitheringTransition(fadeMaskSeed, unity_LODFade.x);
				#endif

				#ifdef _ALPHATEST_ON
					DoAlphaTest(surfaceDescription.Alpha, surfaceDescription.AlphaClipThreshold);
				#endif

				float3 bentNormalWS;
				BuildSurfaceData(fragInputs, surfaceDescription, V, surfaceData, bentNormalWS);
        
				#if HAVE_DECALS
				if( _EnableDecals )
				{
					DecalSurfaceData decalSurfaceData = GetDecalSurfaceData(posInput, fragInputs.tangentToWorld[2], surfaceDescription.Alpha);
					ApplyDecalToSurfaceData(decalSurfaceData, fragInputs.tangentToWorld[2], surfaceData);
				}
				#endif

				#if defined( ASE_SRP_VERSION ) && ASE_SRP_VERSION <= 50702
					InitBuiltinData( surfaceDescription.Alpha, bentNormalWS, -T2W(fragInputs, 2), fragInputs.positionRWS, fragInputs.texCoord1, fragInputs.texCoord2, builtinData );
				#else
					InitBuiltinData( posInput, surfaceDescription.Alpha, bentNormalWS, -T2W(fragInputs, 2), fragInputs.texCoord1, fragInputs.texCoord2, builtinData );
				#endif
        
				builtinData.emissiveColor = surfaceDescription.Emission;
        
				builtinData.depthOffset = 0.0;
        
				#if (SHADERPASS == SHADERPASS_DISTORTION)
					builtinData.distortion = surfaceDescription.Distortion;
					builtinData.distortionBlur = surfaceDescription.DistortionBlur;
				#else
					builtinData.distortion = float2(0.0, 0.0);
					builtinData.distortionBlur = 0.0;
				#endif
        
				PostInitBuiltinData(V, posInput, surfaceData, builtinData);
			}
        
			PackedVaryingsMeshToPS Vert(AttributesMesh inputMesh/*ase_vert_input*/ )
			{
				UNITY_SETUP_INSTANCE_ID( inputMesh );
				PackedVaryingsMeshToPS outputPackedVaryingsMeshToPS;
				UNITY_TRANSFER_INSTANCE_ID( inputMesh, outputPackedVaryingsMeshToPS );

				/*ase_vert_code:inputMesh=AttributesMesh;outputPackedVaryingsMeshToPS=PackedVaryingsMeshToPS*/
				inputMesh.vertex.xyz += /*ase_vert_out:Vertex Offset;Float3;11;-1;_VertexOffset*/ float3( 0, 0, 0 ) /*end*/;

				float3 positionRWS = TransformObjectToWorld(inputMesh.vertex.xyz );
				float3 normalWS = TransformObjectToWorldNormal(inputMesh.normal );
				float4 tangentWS = float4(TransformObjectToWorldDir(inputMesh.tangent.xyz), inputMesh.tangent.w);

				outputPackedVaryingsMeshToPS.positionCS = TransformWorldToHClip(positionRWS);
				outputPackedVaryingsMeshToPS.interp00.xyz =	positionRWS;
				outputPackedVaryingsMeshToPS.interp01.xyz =	normalWS;
				outputPackedVaryingsMeshToPS.interp02.xyzw = tangentWS;
				outputPackedVaryingsMeshToPS.interp03.xyzw = inputMesh.uv1;
				outputPackedVaryingsMeshToPS.interp04.xyzw = inputMesh.uv2;
			
				return outputPackedVaryingsMeshToPS;
			}

			void Frag(  PackedVaryingsMeshToPS packedInput,	OUTPUT_GBUFFER(outGBuffer), out float outputDepth : SV_Depth /*ase_frag_input*/ )
			{
				UNITY_SETUP_INSTANCE_ID( packedInput );
				FragInputs input;
				ZERO_INITIALIZE( FragInputs, input );
				GlobalSurfaceDescription surfaceDescription = (GlobalSurfaceDescription)0;
				SurfaceOutput o = (SurfaceOutput)0;

				o.Normal = float3( 0, 0, 1 );
				float4 clipPos = 0;
				float3 worldPos = 0;

				/*ase_frag_code:packedInput=PackedVaryingsMeshToPS*/

                surfaceDescription.Albedo = /*ase_frag_out:Albedo;Float3;0;-1;_Albedo*/float3( 0.5, 0.5, 0.5 )/*end*/;
                surfaceDescription.Normal = /*ase_frag_out:World Normal;Float3;1;-1;_WorldNormal*/float3( 0, 0, 1 )/*end*/;
                surfaceDescription.BentNormal = /*ase_frag_out:Bent Normal;Float3;2;-1;_BentNormal*/float3( 0, 0, 1 )/*end*/;
                surfaceDescription.CoatMask = /*ase_frag_out:Coat Mask;Float;3;-1;_CoatMask*/0/*end*/;
                surfaceDescription.Metallic = /*ase_frag_out:Metallic;Float;4;-1;_Metallic*/0/*end*/;
				
				#ifdef _MATERIAL_FEATURE_SPECULAR_COLOR
				surfaceDescription.Specular = /*ase_frag_out:Specular;Float3;5;-1;_Specular*/0/*end*/;
				#endif
                
				surfaceDescription.Emission = /*ase_frag_out:Emission;Float3;6;-1;_Emission*/0/*end*/;
                surfaceDescription.Smoothness = /*ase_frag_out:Smoothness;Float;7;-1;_Smoothness*/0.5/*end*/;
                surfaceDescription.Occlusion = /*ase_frag_out:Occlusion;Float;8;-1;_Occlusion*/1/*end*/;
				surfaceDescription.Alpha = /*ase_frag_out:Alpha;Float;9;-1;_Alpha*/1/*end*/;
				
				#ifdef _ALPHATEST_ON
				surfaceDescription.AlphaClipThreshold = /*ase_frag_out:Alpha Clip Threshold;Float;10;-1;_AlphaClip*/0/*end*/;
				#endif

				#ifdef _ENABLE_GEOMETRIC_SPECULAR_AA
				surfaceDescription.SpecularAAScreenSpaceVariance = /*ase_frag_out:SpecularAAScreenSpaceVariance;Float;13;-1;_SpecularAAScreenSpaceVariance*/0/*end*/;
				surfaceDescription.SpecularAAThreshold = /*ase_frag_out:SpecularAAThreshold;Float;14;-1;_SpecularAAThreshold*/0/*end*/;
				#endif

				#ifdef _SPECULAR_OCCLUSION_CUSTOM
				surfaceDescription.SpecularOcclusion = /*ase_frag_out:SpecularOcclusion;Float;15;-1;_SpecularOcclusion*/0/*end*/;
				#endif

				#if defined(_HAS_REFRACTION) || defined(_MATERIAL_FEATURE_TRANSMISSION)
				surfaceDescription.Thickness = /*ase_frag_out:Thickness;Float;16;-1;_Thickness*/1/*end*/;
				#endif

				#ifdef _HAS_REFRACTION
				surfaceDescription.RefractionIndex = /*ase_frag_out:RefractionIndex;Float;17;-1;_RefractionIndex*/1/*end*/;
                surfaceDescription.RefractionColor = /*ase_frag_out:RefractionColor;Float3;18;-1;_RefractionColor*/float3(1,1,1)/*end*/;
                surfaceDescription.RefractionDistance = /*ase_frag_out:RefractionDistance;Float;19;-1;_RefractionDistance*/0/*end*/;
				#endif

				#ifdef _MATERIAL_FEATURE_SUBSURFACE_SCATTERING
				surfaceDescription.SubsurfaceMask = /*ase_frag_out:SubsurfaceMask;Float;20;-1;_SubsurfaceMask*/1/*end*/;
                surfaceDescription.DiffusionProfile = /*ase_frag_out:DiffusionProfile;Float;21;-1;_DiffusionProfile*/0/*end*/;
				#endif

				#ifdef _MATERIAL_FEATURE_ANISOTROPY
				surfaceDescription.Anisotropy = /*ase_frag_out:Anisotropy;Float;22;-1;_Anisotropy*/1/*end*/;
				surfaceDescription.Tangent = /*ase_frag_out:Tangent;Float3;23;-1;_Tangent*/float3(1,0,0)/*end*/;
				#endif

				#ifdef _MATERIAL_FEATURE_IRIDESCENCE
				surfaceDescription.IridescenceMask = /*ase_frag_out:IridescenceMask;Float;24;-1;_IridescenceMask*/0/*end*/;
				surfaceDescription.IridescenceThickness = /*ase_frag_out:IridescenceThickness;Float;25;-1;_IridescenceThickness*/0/*end*/;
				#endif

				float4 bakedGI = /*ase_frag_out:Baked GI;Float4;12;-1;_BakedGI*/float4( 0, 0, 0, 0 )/*end*/;

				packedInput.positionCS.zw = clipPos.zw;
				float3 positionRWS = worldPos;
				float3 normalWS = o.Normal;
				float4 tangentWS = packedInput.interp02.xyzw;

				input.positionSS = packedInput.positionCS;
				input.positionRWS = positionRWS;
				#if defined( ASE_SRP_VERSION ) && ASE_SRP_VERSION < 60900
				input.worldToTangent = BuildWorldToTangent( tangentWS, normalWS );
				#else
				input.tangentToWorld = BuildTangentToWorld( tangentWS, normalWS );
				#endif
				input.texCoord1 = packedInput.interp03.xyzw;
				input.texCoord2 = packedInput.interp04.xyzw;

				PositionInputs posInput = GetPositionInput( input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS );
				SurfaceData surfaceData;
				BuiltinData builtinData;

				float3 normalizedWorldViewDir = GetWorldSpaceNormalizeViewDir( input.positionRWS );

				GetSurfaceAndBuiltinData(surfaceDescription,input, normalizedWorldViewDir, posInput, surfaceData, builtinData);

				#ifdef CUSTOM_BAKED_GI
					BSDFData bsdfData = ConvertSurfaceDataToBSDFData( posInput.positionSS, surfaceData );
					half4 decodeInstructions = half4( LIGHTMAP_HDR_MULTIPLIER, LIGHTMAP_HDR_EXPONENT, 0.0h, 0.0h );
					builtinData.bakeDiffuseLighting = UnpackLightmapRGBM( bakedGI, decodeInstructions ) * EMISSIVE_RGBM_SCALE * bsdfData.diffuseColor;
				#endif

				ENCODE_INTO_GBUFFER(surfaceData, builtinData, posInput.positionSS, outGBuffer);
				
				outputDepth = posInput.deviceDepth;
			}
     
            ENDHLSL
        }

		/*ase_pass*/
		Pass
        {
			/*ase_hide_pass*/
            Name "SceneSelectionPass"
            Tags { "LightMode" = "SceneSelectionPass" }
            ColorMask 0
        	
            HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag
        
			/*ase_pragma*/

			#if defined(_MATERIAL_FEATURE_SUBSURFACE_SCATTERING) && !defined(_SURFACE_TYPE_TRANSPARENT)
			#define OUTPUT_SPLIT_LIGHTING
			#endif
        
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
			#if defined( ASE_SRP_VERSION ) && ASE_SRP_VERSION < 60900              
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Wind.hlsl"
			#endif
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/NormalSurfaceGradient.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
        
            
            #define SHADERPASS SHADERPASS_DEPTH_ONLY
            #define SCENESELECTIONPASS
        
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/Lit.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/DecalUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/LitDecalData.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"
        
			#if defined( ASE_SRP_VERSION ) && ASE_SRP_VERSION < 60900
			#define T2W(var, index) var.worldToTangent[index]
			#else
			#define T2W(var, index) var.tangentToWorld[index]
			#endif

			int _ObjectId;
			int _PassValue;
        
			struct AttributesMesh 
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				/*ase_vdata:p=p;n=n;uv0=tc0*/
			};
        
			struct PackedVaryingsMeshToPS 
			{
				float4 positionCS : SV_Position;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				/*ase_interp(0,):sp=sp.xyzw*/
			};

			/*ase_globals*/

			/*ase_funcs*/
        
			void BuildSurfaceData(FragInputs fragInputs, inout AlphaSurfaceDescription surfaceDescription, float3 V, out SurfaceData surfaceData, out float3 bentNormalWS)
			{
				ZERO_INITIALIZE(SurfaceData, surfaceData);

				surfaceData.materialFeatures = MATERIALFEATUREFLAGS_LIT_STANDARD;
				#ifdef _MATERIAL_FEATURE_SUBSURFACE_SCATTERING
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_SUBSURFACE_SCATTERING;
				#endif
				#ifdef _MATERIAL_FEATURE_TRANSMISSION
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_TRANSMISSION;
				#endif
				#ifdef _MATERIAL_FEATURE_ANISOTROPY
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_ANISOTROPY;
				#endif
        
				#ifdef _MATERIAL_FEATURE_IRIDESCENCE
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_IRIDESCENCE;
				#endif
				#ifdef _MATERIAL_FEATURE_SPECULAR_COLOR
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_SPECULAR_COLOR;
				#endif
        
				#if defined (_MATERIAL_FEATURE_SPECULAR_COLOR) && defined (_ENERGY_CONSERVING_SPECULAR)
					surfaceData.baseColor *= (1.0 - Max3(surfaceData.specularColor.r, surfaceData.specularColor.g, surfaceData.specularColor.b));
				#endif
        
				GetNormalWS( fragInputs, float3( 0.0f, 0.0f, 1.0f ), surfaceData.normalWS, float3( 1.0, 1.0, 1.0 ) );
        
				bentNormalWS = surfaceData.normalWS;
        
				#ifdef _HAS_REFRACTION
					if (_EnableSSRefraction)
					{
        
						surfaceData.transmittanceMask = (1.0 - surfaceDescription.Alpha);
						surfaceDescription.Alpha = 1.0;
					}
					else
					{
						surfaceData.ior = 1.0;
						surfaceData.transmittanceColor = float3(1.0, 1.0, 1.0);
						surfaceData.atDistance = 1.0;
						surfaceData.transmittanceMask = 0.0;
						surfaceDescription.Alpha = 1.0;
					}
				#else
					surfaceData.ior = 1.0;
					surfaceData.transmittanceColor = float3(1.0, 1.0, 1.0);
					surfaceData.atDistance = 1.0;
					surfaceData.transmittanceMask = 0.0;
				#endif
        
				surfaceData.tangentWS = normalize(T2W(fragInputs, 0).xyz);    // The tangent is not normalize in worldToTangent for mikkt. TODO: Check if it expected that we normalize with Morten. Tag: SURFACE_GRADIENT
				surfaceData.tangentWS = Orthonormalize(surfaceData.tangentWS, surfaceData.normalWS);
        
				#if defined(_SPECULAR_OCCLUSION_CUSTOM)
				#elif defined(_SPECULAR_OCCLUSION_FROM_AO_BENT_NORMAL)
					surfaceData.specularOcclusion = GetSpecularOcclusionFromBentAO(V, bentNormalWS, surfaceData.normalWS, surfaceData.ambientOcclusion, PerceptualSmoothnessToPerceptualRoughness(surfaceData.perceptualSmoothness));
				#elif defined(_AMBIENT_OCCLUSION) && defined(_SPECULAR_OCCLUSION_FROM_AO)
					surfaceData.specularOcclusion = GetSpecularOcclusionFromAmbientOcclusion(ClampNdotV(dot(surfaceData.normalWS, V)), surfaceData.ambientOcclusion, PerceptualSmoothnessToRoughness(surfaceData.perceptualSmoothness));
				#else
					surfaceData.specularOcclusion = 1.0;
				#endif
				#ifdef _ENABLE_GEOMETRIC_SPECULAR_AA
					surfaceData.perceptualSmoothness = GeometricNormalFiltering(surfaceData.perceptualSmoothness, T2W(fragInputs, 2), surfaceDescription.SpecularAAScreenSpaceVariance, surfaceDescription.SpecularAAThreshold);
				#endif
			}
        
            void GetSurfaceAndBuiltinData(AlphaSurfaceDescription surfaceDescription, FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData)
            {
				#ifdef LOD_FADE_CROSSFADE 
					uint3 fadeMaskSeed = asuint((int3)(V * _ScreenSize.xyx));
					LODDitheringTransition(fadeMaskSeed, unity_LODFade.x);
				#endif
        
				#ifdef _ALPHATEST_ON
					DoAlphaTest ( surfaceDescription.Alpha, surfaceDescription.AlphaClipThreshold );
				#endif

                float3 bentNormalWS;
                BuildSurfaceData(fragInputs, surfaceDescription, V, surfaceData, bentNormalWS);
        
				#if HAVE_DECALS
				if( _EnableDecals )
				{
					DecalSurfaceData decalSurfaceData = GetDecalSurfaceData(posInput, fragInputs.tangentToWorld[2], surfaceDescription.Alpha);
					ApplyDecalToSurfaceData(decalSurfaceData, fragInputs.tangentToWorld[2], surfaceData);
				}
				#endif

				#if defined( ASE_SRP_VERSION ) && ASE_SRP_VERSION <= 50702
					InitBuiltinData( surfaceDescription.Alpha, bentNormalWS, -T2W(fragInputs, 2), fragInputs.positionRWS, fragInputs.texCoord1, fragInputs.texCoord2, builtinData );
				#else
					InitBuiltinData( posInput, surfaceDescription.Alpha, bentNormalWS, -T2W(fragInputs, 2), fragInputs.texCoord1, fragInputs.texCoord2, builtinData );
				#endif
        
                builtinData.depthOffset = 0.0;
        
				#if (SHADERPASS == SHADERPASS_DISTORTION)
					builtinData.distortion = surfaceDescription.Distortion;
					builtinData.distortionBlur = surfaceDescription.DistortionBlur;
				#else
					builtinData.distortion = float2(0.0, 0.0);
					builtinData.distortionBlur = 0.0;
				#endif
        
                PostInitBuiltinData(V, posInput, surfaceData, builtinData);
            }
        
			PackedVaryingsMeshToPS Vert(AttributesMesh inputMesh /*ase_vert_input*/ )
			{
				UNITY_SETUP_INSTANCE_ID( inputMesh );
				PackedVaryingsMeshToPS outputPackedVaryingsMeshToPS;
				UNITY_TRANSFER_INSTANCE_ID( inputMesh, outputPackedVaryingsMeshToPS );

				/*ase_vert_code:inputMesh=AttributesMesh;outputPackedVaryingsMeshToPS=PackedVaryingsMeshToPS*/
				inputMesh.vertex.xyz += /*ase_vert_out:Vertex Offset;Float3;2;-1;_VertexOffset*/ float3( 0, 0, 0 ) /*end*/;

				float3 positionRWS = TransformObjectToWorld(inputMesh.vertex.xyz );
				outputPackedVaryingsMeshToPS.positionCS = TransformWorldToHClip(positionRWS);
				return outputPackedVaryingsMeshToPS;
			}

			void Frag(  PackedVaryingsMeshToPS packedInput
				#ifdef WRITE_NORMAL_BUFFER
				, out float4 outNormalBuffer : SV_Target0
					#ifdef WRITE_MSAA_DEPTH
				, out float1 depthColor : SV_Target1
					#endif
				#elif defined(SCENESELECTIONPASS)
				, out float4 outColor : SV_Target0
				#endif
				, out float outputDepth : SV_Depth
				/*ase_frag_input*/
			)
			{
				UNITY_SETUP_INSTANCE_ID( packedInput );
				FragInputs input;
				ZERO_INITIALIZE(FragInputs, input);
				AlphaSurfaceDescription surfaceDescription = (AlphaSurfaceDescription)0;
				SurfaceOutput o = (SurfaceOutput)0;

				o.Normal = float3( 0, 0, 1 );
				float4 clipPos = 0;
				float3 worldPos = 0;
				/*ase_frag_code:packedInput=PackedVaryingsMeshToPS*/
				surfaceDescription.Alpha = /*ase_frag_out:Alpha;Float;0;-1;_Alpha*/1/*end*/;
				
				#ifdef _ALPHATEST_ON
				surfaceDescription.AlphaClipThreshold = /*ase_frag_out:Alpha Clip Threshold;Float;1;-1;_AlphaClip*/0/*end*/;
				#endif
				
				packedInput.positionCS.zw = clipPos.zw;
				float3 positionRWS = worldPos;

#if defined( ASE_SRP_VERSION ) && ASE_SRP_VERSION < 60900
				input.worldToTangent = k_identity3x3;
#else
				input.tangentToWorld = k_identity3x3;
#endif

				input.positionSS = packedInput.positionCS;
				input.positionRWS = positionRWS;
				PositionInputs posInput = GetPositionInput( input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS );

				float3 normalizedWorldViewDir = GetWorldSpaceNormalizeViewDir( input.positionRWS );

				SurfaceData surfaceData;
				BuiltinData builtinData;

				GetSurfaceAndBuiltinData(surfaceDescription,input, normalizedWorldViewDir, posInput, surfaceData, builtinData);

				outputDepth = posInput.deviceDepth;

				#ifdef WRITE_NORMAL_BUFFER
					EncodeIntoNormalBuffer(ConvertSurfaceDataToNormalData(surfaceData), posInput.positionSS, outNormalBuffer);
					#ifdef WRITE_MSAA_DEPTH
					depthColor = packedInput.positionCS.z;
					#endif
				#elif defined(SCENESELECTIONPASS)
					outColor = float4(_ObjectId, _PassValue, 1.0, 1.0);
				#endif
			}

            ENDHLSL
        } 

		/*ase_pass*/
		Pass
		{
			/*ase_hide_pass*/
			Name "META"
			Tags { "LightMode" = "Meta" }
			Cull Off

			HLSLPROGRAM

			#pragma vertex Vert
			#pragma fragment Frag

			/*ase_pragma*/

			#if defined(_MATERIAL_FEATURE_SUBSURFACE_SCATTERING) && !defined(_SURFACE_TYPE_TRANSPARENT)
			#define OUTPUT_SPLIT_LIGHTING
			#endif

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
			#if defined( ASE_SRP_VERSION ) && ASE_SRP_VERSION < 60900              
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Wind.hlsl"
			#endif
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/NormalSurfaceGradient.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"

			#define SHADERPASS SHADERPASS_LIGHT_TRANSPORT

			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/Lit.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/DecalUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/LitDecalData.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#if defined( ASE_SRP_VERSION ) && ASE_SRP_VERSION < 60900
			#define T2W(var, index) var.worldToTangent[index]
			#else
			#define T2W(var, index) var.tangentToWorld[index]
			#endif

			struct AttributesMesh
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 texcoord : TEXCOORD0;
				float4 uv1 : TEXCOORD1;
				float4 uv2 : TEXCOORD2;
				float4 color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				/*ase_vdata:p=p;n=n;t=t;c=c;uv0=tc0;uv1=tc1;uv2=tc2*/
			};

			struct PackedVaryingsMeshToPS
			{
				float4 positionCS : SV_Position;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				/*ase_interp(0,):sp=sp.xyzw*/
			};

			/*ase_globals*/

			/*ase_funcs*/

			void BuildSurfaceData( FragInputs fragInputs, inout GlobalSurfaceDescription surfaceDescription, float3 V, out SurfaceData surfaceData, out float3 bentNormalWS )
			{
				ZERO_INITIALIZE( SurfaceData, surfaceData );
				surfaceData.baseColor = surfaceDescription.Albedo;
				surfaceData.perceptualSmoothness = surfaceDescription.Smoothness;
				#ifdef _SPECULAR_OCCLUSION_CUSTOM
					surfaceData.specularOcclusion = surfaceDescription.SpecularOcclusion;
				#endif
					surfaceData.ambientOcclusion = surfaceDescription.Occlusion;
					surfaceData.metallic = surfaceDescription.Metallic;
					surfaceData.coatMask = surfaceDescription.CoatMask;

				#ifdef _MATERIAL_FEATURE_IRIDESCENCE		
					surfaceData.iridescenceMask = surfaceDescription.IridescenceMask;
					surfaceData.iridescenceThickness = surfaceDescription.IridescenceThickness;
				#endif
					surfaceData.materialFeatures = MATERIALFEATUREFLAGS_LIT_STANDARD;
				#ifdef _MATERIAL_FEATURE_SUBSURFACE_SCATTERING
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_SUBSURFACE_SCATTERING;
				#endif
				#ifdef _MATERIAL_FEATURE_TRANSMISSION
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_TRANSMISSION;
				#endif
				#ifdef _MATERIAL_FEATURE_ANISOTROPY
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_ANISOTROPY;
				#endif

				#ifdef ASE_LIT_CLEAR_COAT
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_CLEAR_COAT;
				#endif

				#ifdef _MATERIAL_FEATURE_IRIDESCENCE
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_IRIDESCENCE;
				#endif
				#ifdef _MATERIAL_FEATURE_SPECULAR_COLOR
					surfaceData.specularColor = surfaceDescription.Specular;
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_SPECULAR_COLOR;
				#endif

				#if defined (_MATERIAL_FEATURE_SPECULAR_COLOR) && defined (_ENERGY_CONSERVING_SPECULAR)
					surfaceData.baseColor *= ( 1.0 - Max3( surfaceData.specularColor.r, surfaceData.specularColor.g, surfaceData.specularColor.b ) );
				#endif
					//float3 normalTS = float3( 0.0f, 0.0f, 1.0f );
					//normalTS = surfaceDescription.Normal;
					GetNormalWS( fragInputs, float3( 0.0, 0.0, 1.0 ), surfaceData.normalWS, float3( 1.0, 1.0, 1.0 ) );
					bentNormalWS = surfaceData.normalWS;

				#ifdef ASE_BENT_NORMAL
					GetNormalWS( fragInputs, surfaceDescription.BentNormal, bentNormalWS, float4( 1, 1, -1, 0 ) );
				#endif
				
				surfaceData.geomNormalWS = T2W(fragInputs, 2);
				
				#ifdef _HAS_REFRACTION
					if( _EnableSSRefraction )
					{
						surfaceData.ior = surfaceDescription.RefractionIndex;
						surfaceData.transmittanceColor = surfaceDescription.RefractionColor;
						surfaceData.atDistance = surfaceDescription.RefractionDistance;

						surfaceData.transmittanceMask = ( 1.0 - surfaceDescription.Alpha );
						surfaceDescription.Alpha = 1.0;
					}
					else
					{
						surfaceData.ior = 1.0;
						surfaceData.transmittanceColor = float3( 1.0, 1.0, 1.0 );
						surfaceData.atDistance = 1.0;
						surfaceData.transmittanceMask = 0.0;
						surfaceDescription.Alpha = 1.0;
					}
				#else
					surfaceData.ior = 1.0;
					surfaceData.transmittanceColor = float3( 1.0, 1.0, 1.0 );
					surfaceData.atDistance = 1.0;
					surfaceData.transmittanceMask = 0.0;
				#endif

				#if defined(_HAS_REFRACTION) || defined(_MATERIAL_FEATURE_TRANSMISSION)
					surfaceData.thickness = surfaceDescription.Thickness;
				#endif

				#ifdef _MATERIAL_FEATURE_SUBSURFACE_SCATTERING
					surfaceData.subsurfaceMask = surfaceDescription.SubsurfaceMask;
					surfaceData.diffusionProfile = surfaceDescription.DiffusionProfile;
				#endif

					surfaceData.tangentWS = normalize( T2W(fragInputs, 0).xyz );    // The tangent is not normalize in worldToTangent for mikkt. TODO: Check if it expected that we normalize with Morten. Tag: SURFACE_GRADIENT
				#ifdef _MATERIAL_FEATURE_ANISOTROPY	
					surfaceData.anisotropy = surfaceDescription.Anisotropy;
					surfaceData.tangentWS = TransformTangentToWorld( surfaceDescription.Tangent, fragInputs.worldToTangent );
				#endif
					surfaceData.tangentWS = Orthonormalize( surfaceData.tangentWS, surfaceData.normalWS );
				#if defined(_SPECULAR_OCCLUSION_CUSTOM)
				#elif defined(_SPECULAR_OCCLUSION_FROM_AO_BENT_NORMAL)
					surfaceData.specularOcclusion = GetSpecularOcclusionFromBentAO( V, bentNormalWS, surfaceData.normalWS, surfaceData.ambientOcclusion, PerceptualSmoothnessToPerceptualRoughness( surfaceData.perceptualSmoothness ) );
				#elif defined(_AMBIENT_OCCLUSION) && defined(_SPECULAR_OCCLUSION_FROM_AO)
					surfaceData.specularOcclusion = GetSpecularOcclusionFromAmbientOcclusion( ClampNdotV( dot( surfaceData.normalWS, V ) ), surfaceData.ambientOcclusion, PerceptualSmoothnessToRoughness( surfaceData.perceptualSmoothness ) );
				#else
					surfaceData.specularOcclusion = 1.0;
				#endif
				#ifdef _ENABLE_GEOMETRIC_SPECULAR_AA
					surfaceData.perceptualSmoothness = GeometricNormalFiltering( surfaceData.perceptualSmoothness, T2W(fragInputs, 2), surfaceDescription.SpecularAAScreenSpaceVariance, surfaceDescription.SpecularAAThreshold );
				#endif

			}

			void GetSurfaceAndBuiltinData( GlobalSurfaceDescription surfaceDescription,FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData )
			{
				#ifdef LOD_FADE_CROSSFADE
					uint3 fadeMaskSeed = asuint( ( int3 )( V * _ScreenSize.xyx ) ); // Quantize V to _ScreenSize values
					LODDitheringTransition( fadeMaskSeed, unity_LODFade.x );
				#endif

				#ifdef _ALPHATEST_ON
					DoAlphaTest( surfaceDescription.Alpha, surfaceDescription.AlphaClipThreshold );
				#endif

				float3 bentNormalWS;
				BuildSurfaceData( fragInputs, surfaceDescription, V, surfaceData, bentNormalWS );

				#if HAVE_DECALS
				if( _EnableDecals )
				{
					DecalSurfaceData decalSurfaceData = GetDecalSurfaceData(posInput, fragInputs.tangentToWorld[2], surfaceDescription.Alpha);
					ApplyDecalToSurfaceData(decalSurfaceData, fragInputs.tangentToWorld[2], surfaceData);
				}
				#endif

				#if defined( ASE_SRP_VERSION ) && ASE_SRP_VERSION <= 50702
					InitBuiltinData( surfaceDescription.Alpha, bentNormalWS, -T2W(fragInputs, 2), fragInputs.positionRWS, fragInputs.texCoord1, fragInputs.texCoord2, builtinData );
				#else
					InitBuiltinData( posInput, surfaceDescription.Alpha, bentNormalWS, -T2W(fragInputs, 2), fragInputs.texCoord1, fragInputs.texCoord2, builtinData );
				#endif

				builtinData.emissiveColor = surfaceDescription.Emission;

				builtinData.depthOffset = 0.0;

				#if (SHADERPASS == SHADERPASS_DISTORTION)
					builtinData.distortion = surfaceDescription.Distortion;
					builtinData.distortionBlur = surfaceDescription.DistortionBlur;
				#else
					builtinData.distortion = float2( 0.0, 0.0 );
					builtinData.distortionBlur = 0.0;
				#endif

				PostInitBuiltinData( V, posInput, surfaceData, builtinData );
			}

			CBUFFER_START( UnityMetaPass )
			bool4 unity_MetaVertexControl;
			bool4 unity_MetaFragmentControl;
			CBUFFER_END

			float unity_OneOverOutputBoost;
			float unity_MaxOutputValue;

			PackedVaryingsMeshToPS Vert( AttributesMesh inputMesh /*ase_vert_input*/ )
			{
				UNITY_SETUP_INSTANCE_ID( inputMesh );
				PackedVaryingsMeshToPS outputPackedVaryingsMeshToPS;
				UNITY_TRANSFER_INSTANCE_ID( inputMesh, outputPackedVaryingsMeshToPS );

				/*ase_vert_code:inputMesh=AttributesMesh;outputPackedVaryingsMeshToPS=PackedVaryingsMeshToPS*/
				inputMesh.vertex.xyz += /*ase_vert_out:Vertex Offset;Float3;11;-1;_VertexOffset*/ float3( 0, 0, 0 ) /*end*/;

				float2 uv;

				if( unity_MetaVertexControl.x )
				{
					uv = inputMesh.uv1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				}
				else if( unity_MetaVertexControl.y )
				{
					uv = inputMesh.uv2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
				}

				outputPackedVaryingsMeshToPS.positionCS = float4( uv * 2.0 - 1.0, inputMesh.vertex.z > 0 ? 1.0e-4 : 0.0, 1.0 );
				return outputPackedVaryingsMeshToPS;
			}

			float4 Frag( PackedVaryingsMeshToPS packedInput /*ase_frag_input*/ ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( packedInput );
				FragInputs input;
				ZERO_INITIALIZE( FragInputs, input );
				GlobalSurfaceDescription surfaceDescription = (GlobalSurfaceDescription)0;
				SurfaceOutput o = (SurfaceOutput)0;

				o.Normal = float3( 0, 0, 1 );
				float4 clipPos = 0;
				float3 worldPos = 0;

				/*ase_frag_code:packedInput=PackedVaryingsMeshToPS*/

				surfaceDescription.Albedo = /*ase_frag_out:Albedo;Float3;0;-1;_Albedo*/float3( 0.5, 0.5, 0.5 )/*end*/;
				surfaceDescription.Normal = /*ase_frag_out:World Normal;Float3;1;-1;_WorldNormal*/float3( 0, 0, 1 )/*end*/;
				surfaceDescription.BentNormal = /*ase_frag_out:Bent Normal;Float3;2;-1;_BentNormal*/float3( 0, 0, 1 )/*end*/;
				surfaceDescription.CoatMask = /*ase_frag_out:Coat Mask;Float;3;-1;_CoatMask*/0/*end*/;
				surfaceDescription.Metallic = /*ase_frag_out:Metallic;Float;4;-1;_Metallic*/0/*end*/;

				#ifdef _MATERIAL_FEATURE_SPECULAR_COLOR
				surfaceDescription.Specular = /*ase_frag_out:Specular;Float3;5;-1;_Specular*/0/*end*/;
				#endif

				surfaceDescription.Emission = /*ase_frag_out:Emission;Float3;6;-1;_Emission*/0/*end*/;
				surfaceDescription.Smoothness = /*ase_frag_out:Smoothness;Float;7;-1;_Smoothness*/0.5/*end*/;
				surfaceDescription.Occlusion = /*ase_frag_out:Occlusion;Float;8;-1;_Occlusion*/1/*end*/;
				surfaceDescription.Alpha = /*ase_frag_out:Alpha;Float;9;-1;_Alpha*/1/*end*/;

				#ifdef _ALPHATEST_ON
				surfaceDescription.AlphaClipThreshold = /*ase_frag_out:Alpha Clip Threshold;Float;10;-1;_AlphaClip*/0/*end*/;
				#endif

				#ifdef _ENABLE_GEOMETRIC_SPECULAR_AA
				surfaceDescription.SpecularAAScreenSpaceVariance = /*ase_frag_out:SpecularAAScreenSpaceVariance;Float;13;-1;_SpecularAAScreenSpaceVariance*/0/*end*/;
				surfaceDescription.SpecularAAThreshold = /*ase_frag_out:SpecularAAThreshold;Float;14;-1;_SpecularAAThreshold*/0/*end*/;
				#endif

				#ifdef _SPECULAR_OCCLUSION_CUSTOM
				surfaceDescription.SpecularOcclusion = /*ase_frag_out:SpecularOcclusion;Float;15;-1;_SpecularOcclusion*/0/*end*/;
				#endif

				#if defined(_HAS_REFRACTION) || defined(_MATERIAL_FEATURE_TRANSMISSION)
				surfaceDescription.Thickness = /*ase_frag_out:Thickness;Float;16;-1;_Thickness*/1/*end*/;
				#endif

				#ifdef _HAS_REFRACTION
				surfaceDescription.RefractionIndex = /*ase_frag_out:RefractionIndex;Float;17;-1;_RefractionIndex*/1/*end*/;
				surfaceDescription.RefractionColor = /*ase_frag_out:RefractionColor;Float3;18;-1;_RefractionColor*/float3( 1,1,1 )/*end*/;
				surfaceDescription.RefractionDistance = /*ase_frag_out:RefractionDistance;Float;19;-1;_RefractionDistance*/0/*end*/;
				#endif

				#ifdef _MATERIAL_FEATURE_SUBSURFACE_SCATTERING
				surfaceDescription.SubsurfaceMask = /*ase_frag_out:SubsurfaceMask;Float;20;-1;_SubsurfaceMask*/1/*end*/;
				surfaceDescription.DiffusionProfile = /*ase_frag_out:DiffusionProfile;Float;21;-1;_DiffusionProfile*/0/*end*/;
				#endif

				#ifdef _MATERIAL_FEATURE_ANISOTROPY
				surfaceDescription.Anisotropy = /*ase_frag_out:Anisotropy;Float;22;-1;_Anisotropy*/1/*end*/;
				surfaceDescription.Tangent = /*ase_frag_out:Tangent;Float3;23;-1;_Tangent*/float3( 1,0,0 )/*end*/;
				#endif

				#ifdef _MATERIAL_FEATURE_IRIDESCENCE
				surfaceDescription.IridescenceMask = /*ase_frag_out:IridescenceMask;Float;24;-1;_IridescenceMask*/0/*end*/;
				surfaceDescription.IridescenceThickness = /*ase_frag_out:IridescenceThickness;Float;25;-1;_IridescenceThickness*/0/*end*/;
				#endif

				packedInput.positionCS.zw = clipPos.zw;
				float3 positionRWS = worldPos;

				input.positionSS = packedInput.positionCS;
				input.positionRWS = positionRWS;
#if defined( ASE_SRP_VERSION ) && ASE_SRP_VERSION < 60900
				input.worldToTangent = k_identity3x3;
#else
				input.tangentToWorld = k_identity3x3;
#endif

				PositionInputs posInput = GetPositionInput( input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS );

				SurfaceData surfaceData;
				BuiltinData builtinData;

				float3 normalizedWorldViewDir = GetWorldSpaceNormalizeViewDir( input.positionRWS );

				GetSurfaceAndBuiltinData( surfaceDescription,input, normalizedWorldViewDir, posInput, surfaceData, builtinData );

				BSDFData bsdfData = ConvertSurfaceDataToBSDFData( input.positionSS.xy, surfaceData );
				LightTransportData lightTransportData = GetLightTransportData( surfaceData, builtinData, bsdfData );

				float4 res = float4( 0.0, 0.0, 0.0, 1.0 );
				if( unity_MetaFragmentControl.x )
				{
					res.rgb = clamp( pow( abs( lightTransportData.diffuseColor ), saturate( unity_OneOverOutputBoost ) ), 0, unity_MaxOutputValue );
				}

				if( unity_MetaFragmentControl.y )
				{
					res.rgb = lightTransportData.emissiveColor;
				}

				return res;
			}

			ENDHLSL
		}

		/*ase_pass*/
		Pass
        {
			/*ase_hide_pass*/
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
			ColorMask 0
        
            HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag
        
			/*ase_pragma*/

			#if defined(_MATERIAL_FEATURE_SUBSURFACE_SCATTERING) && !defined(_SURFACE_TYPE_TRANSPARENT)
			#define OUTPUT_SPLIT_LIGHTING
			#endif
        
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
			#if defined( ASE_SRP_VERSION ) && ASE_SRP_VERSION < 60900              
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Wind.hlsl"
			#endif
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

			#if defined( ASE_SRP_VERSION ) && ASE_SRP_VERSION < 60900
			#define T2W(var, index) var.worldToTangent[index]
			#else
			#define T2W(var, index) var.tangentToWorld[index]
			#endif

			struct AttributesMesh 
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				/*ase_vdata:p=p;n=n*/
			};
        
			struct PackedVaryingsMeshToPS 
			{
				float4 positionCS : SV_Position;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				/*ase_interp(0,):sp=sp.xyzw*/
			};

			/*ase_globals*/
				
			/*ase_funcs*/
                
			void BuildSurfaceData(FragInputs fragInputs, inout AlphaSurfaceDescription surfaceDescription, float3 V, out SurfaceData surfaceData, out float3 bentNormalWS)
			{
				ZERO_INITIALIZE(SurfaceData, surfaceData);
        
				surfaceData.materialFeatures = MATERIALFEATUREFLAGS_LIT_STANDARD;
				#ifdef _MATERIAL_FEATURE_SUBSURFACE_SCATTERING
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_SUBSURFACE_SCATTERING;
				#endif
				#ifdef _MATERIAL_FEATURE_TRANSMISSION
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_TRANSMISSION;
				#endif
				#ifdef _MATERIAL_FEATURE_ANISOTROPY
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_ANISOTROPY;
				#endif
        
				#ifdef _MATERIAL_FEATURE_IRIDESCENCE
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_IRIDESCENCE;
				#endif
				#ifdef _MATERIAL_FEATURE_SPECULAR_COLOR
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_SPECULAR_COLOR;
				#endif
        
				#if defined (_MATERIAL_FEATURE_SPECULAR_COLOR) && defined (_ENERGY_CONSERVING_SPECULAR)
					surfaceData.baseColor *= (1.0 - Max3(surfaceData.specularColor.r, surfaceData.specularColor.g, surfaceData.specularColor.b));
				#endif
        
				GetNormalWS( fragInputs, float3( 0.0, 0.0, 1.0 ), surfaceData.normalWS, float3( 1.0, 1.0, 1.0 ) );
				bentNormalWS = surfaceData.normalWS;

				surfaceData.geomNormalWS = T2W(fragInputs, 2);
				
				#ifdef _HAS_REFRACTION
					if (_EnableSSRefraction)
					{
        
						surfaceData.transmittanceMask = (1.0 - surfaceDescription.Alpha);
						surfaceDescription.Alpha = 1.0;
					}
					else
					{
						surfaceData.ior = 1.0;
						surfaceData.transmittanceColor = float3(1.0, 1.0, 1.0);
						surfaceData.atDistance = 1.0;
						surfaceData.transmittanceMask = 0.0;
						surfaceDescription.Alpha = 1.0;
					}
				#else
					surfaceData.ior = 1.0;
					surfaceData.transmittanceColor = float3(1.0, 1.0, 1.0);
					surfaceData.atDistance = 1.0;
					surfaceData.transmittanceMask = 0.0;
				#endif
        
					surfaceData.tangentWS = normalize(T2W(fragInputs, 0).xyz);    // The tangent is not normalize in worldToTangent for mikkt. TODO: Check if it expected that we normalize with Morten. Tag: SURFACE_GRADIENT
					surfaceData.tangentWS = Orthonormalize(surfaceData.tangentWS, surfaceData.normalWS);
        
				#if defined(_SPECULAR_OCCLUSION_CUSTOM)
				#elif defined(_SPECULAR_OCCLUSION_FROM_AO_BENT_NORMAL)
					surfaceData.specularOcclusion = GetSpecularOcclusionFromBentAO(V, bentNormalWS, surfaceData.normalWS, surfaceData.ambientOcclusion, PerceptualSmoothnessToPerceptualRoughness(surfaceData.perceptualSmoothness));
				#elif defined(_AMBIENT_OCCLUSION) && defined(_SPECULAR_OCCLUSION_FROM_AO)
					surfaceData.specularOcclusion = GetSpecularOcclusionFromAmbientOcclusion(ClampNdotV(dot(surfaceData.normalWS, V)), surfaceData.ambientOcclusion, PerceptualSmoothnessToRoughness(surfaceData.perceptualSmoothness));
				#else
					surfaceData.specularOcclusion = 1.0;
				#endif
				#ifdef _ENABLE_GEOMETRIC_SPECULAR_AA
					surfaceData.perceptualSmoothness = GeometricNormalFiltering(surfaceData.perceptualSmoothness, T2W(fragInputs, 2), surfaceDescription.SpecularAAScreenSpaceVariance, surfaceDescription.SpecularAAThreshold);
				#endif
			}
        
            void GetSurfaceAndBuiltinData(AlphaSurfaceDescription surfaceDescription, FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData)
            {
				#ifdef LOD_FADE_CROSSFADE
					uint3 fadeMaskSeed = asuint((int3)(V * _ScreenSize.xyx));
					LODDitheringTransition(fadeMaskSeed, unity_LODFade.x);
				#endif
    
				#ifdef _ALPHATEST_ON
					DoAlphaTest ( surfaceDescription.Alpha, surfaceDescription.AlphaClipThreshold );
				#endif
				float3 bentNormalWS;
				BuildSurfaceData(fragInputs, surfaceDescription, V, surfaceData, bentNormalWS);
        
				#if HAVE_DECALS
				if( _EnableDecals )
				{
					DecalSurfaceData decalSurfaceData = GetDecalSurfaceData(posInput, fragInputs.tangentToWorld[2], surfaceDescription.Alpha);
					ApplyDecalToSurfaceData(decalSurfaceData, fragInputs.tangentToWorld[2], surfaceData);
				}
				#endif
        
				#if defined( ASE_SRP_VERSION ) && ASE_SRP_VERSION <= 50702
					InitBuiltinData( surfaceDescription.Alpha, bentNormalWS, -T2W(fragInputs, 2), fragInputs.positionRWS, fragInputs.texCoord1, fragInputs.texCoord2, builtinData );
				#else
					InitBuiltinData( posInput, surfaceDescription.Alpha, bentNormalWS, -T2W(fragInputs, 2), fragInputs.texCoord1, fragInputs.texCoord2, builtinData );
				#endif

				builtinData.depthOffset = 0.0;
        
				#if (SHADERPASS == SHADERPASS_DISTORTION)
					builtinData.distortion = surfaceDescription.Distortion;
					builtinData.distortionBlur = surfaceDescription.DistortionBlur;
				#else
					builtinData.distortion = float2(0.0, 0.0);
					builtinData.distortionBlur = 0.0;
				#endif
        
                PostInitBuiltinData(V, posInput, surfaceData, builtinData);
            }
        
			PackedVaryingsMeshToPS Vert(AttributesMesh inputMesh /*ase_vert_input*/ )
			{
				UNITY_SETUP_INSTANCE_ID( inputMesh );
				PackedVaryingsMeshToPS outputPackedVaryingsMeshToPS;
				UNITY_TRANSFER_INSTANCE_ID( inputMesh, outputPackedVaryingsMeshToPS );

				/*ase_vert_code:inputMesh=AttributesMesh;outputPackedVaryingsMeshToPS=PackedVaryingsMeshToPS*/
				inputMesh.vertex.xyz += /*ase_vert_out:Vertex Offset;Float3;2;-1;_VertexOffset*/ float3( 0, 0, 0 ) /*end*/;

				float3 positionRWS = TransformObjectToWorld(inputMesh.vertex.xyz);
				outputPackedVaryingsMeshToPS.positionCS = TransformWorldToHClip(positionRWS);
				return outputPackedVaryingsMeshToPS;
			}

			void Frag(  PackedVaryingsMeshToPS packedInput
				#ifdef WRITE_NORMAL_BUFFER
				, out float4 outNormalBuffer : SV_Target0
					#ifdef WRITE_MSAA_DEPTH
				, out float1 depthColor : SV_Target1
					#endif
				#endif
				, out float outputDepth : SV_Depth
				/*ase_frag_input*/
			)
			{
				UNITY_SETUP_INSTANCE_ID( packedInput );
				FragInputs input;
				ZERO_INITIALIZE(FragInputs, input);
				AlphaSurfaceDescription surfaceDescription = (AlphaSurfaceDescription)0;
				SurfaceOutput o = (SurfaceOutput)0;

				o.Normal = float3( 0, 0, 1 );
				float4 clipPos = 0;
				float3 worldPos = 0;

				/*ase_frag_code:packedInput=PackedVaryingsMeshToPS*/
				
				surfaceDescription.Alpha = /*ase_frag_out:Alpha;Float;0;-1;_Alpha*/1/*end*/;

				#ifdef _ALPHATEST_ON
				surfaceDescription.AlphaClipThreshold = /*ase_frag_out:Alpha Clip Threshold;Float;1;-1;_AlphaClip*/0/*end*/;
				#endif

				packedInput.positionCS.zw = clipPos.zw;
				float3 positionRWS = worldPos;

				input.positionSS = packedInput.positionCS;
				input.positionRWS = positionRWS;
#if defined( ASE_SRP_VERSION ) && ASE_SRP_VERSION < 60900
				input.worldToTangent = k_identity3x3;
#else
				input.tangentToWorld = k_identity3x3;
#endif

				PositionInputs posInput = GetPositionInput( input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS );

				SurfaceData surfaceData;
				BuiltinData builtinData;

				float3 normalizedWorldViewDir = GetWorldSpaceNormalizeViewDir( input.positionRWS );

				GetSurfaceAndBuiltinData(surfaceDescription, input, normalizedWorldViewDir, posInput, surfaceData, builtinData);

				outputDepth = posInput.deviceDepth;

				#ifdef WRITE_NORMAL_BUFFER
					EncodeIntoNormalBuffer(ConvertSurfaceDataToNormalData(surfaceData), posInput.positionSS, outNormalBuffer);
					#ifdef WRITE_MSAA_DEPTH
					depthColor = packedInput.positionCS.z;
					#endif
				#endif
			}

            ENDHLSL
        }

		/*ase_pass*/
        Pass
        {
			/*ase_hide_pass*/
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }
        
            HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag
        
			/*ase_pragma*/

			#if defined(_MATERIAL_FEATURE_SUBSURFACE_SCATTERING) && !defined(_SURFACE_TYPE_TRANSPARENT)
			#define OUTPUT_SPLIT_LIGHTING
			#endif
        
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
			#if defined( ASE_SRP_VERSION ) && ASE_SRP_VERSION < 60900              
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Wind.hlsl"
			#endif
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/NormalSurfaceGradient.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
        
            #define SHADERPASS SHADERPASS_DEPTH_ONLY
            #pragma multi_compile _ WRITE_NORMAL_BUFFER
            #pragma multi_compile _ WRITE_MSAA_DEPTH
        
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TANGENT_TO_WORLD
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_TEXCOORD1
            #define VARYINGS_NEED_TEXCOORD2
            #define VARYINGS_NEED_TEXCOORD3
            #define VARYINGS_NEED_COLOR
        
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/Lit.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/DecalUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/LitDecalData.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"
        
			#if defined( ASE_SRP_VERSION ) && ASE_SRP_VERSION < 60900
			#define T2W(var, index) var.worldToTangent[index]
			#else
			#define T2W(var, index) var.tangentToWorld[index]
			#endif

			struct AttributesMesh 
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 texcoord : TEXCOORD0;
				float4 uv1 : TEXCOORD1;
				float4 uv2 : TEXCOORD2;
				float4 uv3 : TEXCOORD3;
				float4 color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				/*ase_vdata:p=p;n=n;t=t;c=c;uv0=tc0;uv1=tc1;uv2=tc2;uv3=tc3*/
			};
        
			struct PackedVaryingsMeshToPS 
			{
				float4 positionCS : SV_Position;
				float3 interp00 : TEXCOORD0;
				float3 interp01 : TEXCOORD1;
				float4 interp02 : TEXCOORD2;
				float4 interp03 : TEXCOORD3;
				float4 interp04 : TEXCOORD4;
				float4 interp05 : TEXCOORD5;
				float4 interp06 : TEXCOORD6;
				float4 interp07 : TEXCOORD7;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				/*ase_interp(8,):sp=sp.xyzw;rwp=tc0;wn=tc1;wt=tc2;uv0=tc3;uv1=tc4;uv2=tc5;uv3=tc6;c=tc7*/
			};
      
			/*ase_globals*/

			/*ase_funcs*/
                    
			void BuildSurfaceData(FragInputs fragInputs, inout SmoothSurfaceDescription surfaceDescription, float3 V, out SurfaceData surfaceData, out float3 bentNormalWS)
			{
				ZERO_INITIALIZE(SurfaceData, surfaceData);
				surfaceData.perceptualSmoothness =      surfaceDescription.Smoothness;
                
				surfaceData.materialFeatures = MATERIALFEATUREFLAGS_LIT_STANDARD;
				#ifdef _MATERIAL_FEATURE_SUBSURFACE_SCATTERING
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_SUBSURFACE_SCATTERING;
				#endif
				#ifdef _MATERIAL_FEATURE_TRANSMISSION
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_TRANSMISSION;
				#endif
				#ifdef _MATERIAL_FEATURE_ANISOTROPY
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_ANISOTROPY;
				#endif
        
				#ifdef _MATERIAL_FEATURE_IRIDESCENCE
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_IRIDESCENCE;
				#endif
				#ifdef _MATERIAL_FEATURE_SPECULAR_COLOR
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_SPECULAR_COLOR;
				#endif
        
				#if defined (_MATERIAL_FEATURE_SPECULAR_COLOR) && defined (_ENERGY_CONSERVING_SPECULAR)
					surfaceData.baseColor *= (1.0 - Max3(surfaceData.specularColor.r, surfaceData.specularColor.g, surfaceData.specularColor.b));
				#endif
        
				GetNormalWS( fragInputs, float3( 0.0, 0.0, 1.0 ), surfaceData.normalWS, float3( 1.0, 1.0, 1.0 ) );
				bentNormalWS = surfaceData.normalWS;

				surfaceData.geomNormalWS = T2W(fragInputs, 2);
				
				#ifdef _HAS_REFRACTION
					surfaceData.transmittanceMask = 1.0 - surfaceDescription.Alpha;
					surfaceDescription.Alpha = 1.0;
				#endif
        
					surfaceData.tangentWS = normalize(T2W(fragInputs, 0).xyz);    // The tangent is not normalize in worldToTangent for mikkt. TODO: Check if it expected that we normalize with Morten. Tag: SURFACE_GRADIENT
					surfaceData.tangentWS = Orthonormalize(surfaceData.tangentWS, surfaceData.normalWS);
        
				#if defined(_SPECULAR_OCCLUSION_CUSTOM)
				#elif defined(_SPECULAR_OCCLUSION_FROM_AO_BENT_NORMAL)
					surfaceData.specularOcclusion = GetSpecularOcclusionFromBentAO(V, bentNormalWS, surfaceData.normalWS, surfaceData.ambientOcclusion, PerceptualSmoothnessToPerceptualRoughness(surfaceData.perceptualSmoothness));
				#elif defined(_AMBIENT_OCCLUSION) && defined(_SPECULAR_OCCLUSION_FROM_AO)
					surfaceData.specularOcclusion = GetSpecularOcclusionFromAmbientOcclusion(ClampNdotV(dot(surfaceData.normalWS, V)), surfaceData.ambientOcclusion, PerceptualSmoothnessToRoughness(surfaceData.perceptualSmoothness));
				#else
					surfaceData.specularOcclusion = 1.0;
				#endif
				#ifdef _ENABLE_GEOMETRIC_SPECULAR_AA
					surfaceData.perceptualSmoothness = GeometricNormalFiltering(surfaceData.perceptualSmoothness, T2W(fragInputs, 2), surfaceDescription.SpecularAAScreenSpaceVariance, surfaceDescription.SpecularAAThreshold);
				#endif
			}
        
            void GetSurfaceAndBuiltinData(SmoothSurfaceDescription surfaceDescription, FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData)
            {
				#ifdef LOD_FADE_CROSSFADE
					uint3 fadeMaskSeed = asuint((int3)(V * _ScreenSize.xyx)); 
					LODDitheringTransition(fadeMaskSeed, unity_LODFade.x);
				#endif
        
				#ifdef _ALPHATEST_ON
					DoAlphaTest ( surfaceDescription.Alpha, surfaceDescription.AlphaClipThreshold );
				#endif

					float3 bentNormalWS;
					BuildSurfaceData(fragInputs, surfaceDescription, V, surfaceData, bentNormalWS);
        
				#if HAVE_DECALS
				if( _EnableDecals )
				{
					DecalSurfaceData decalSurfaceData = GetDecalSurfaceData(posInput, fragInputs.tangentToWorld[2], surfaceDescription.Alpha);
					ApplyDecalToSurfaceData(decalSurfaceData, fragInputs.tangentToWorld[2], surfaceData);
				}
				#endif
        
				#if defined( ASE_SRP_VERSION ) && ASE_SRP_VERSION <= 50702
					InitBuiltinData( surfaceDescription.Alpha, bentNormalWS, -T2W(fragInputs, 2), fragInputs.positionRWS, fragInputs.texCoord1, fragInputs.texCoord2, builtinData );
				#else
					InitBuiltinData( posInput, surfaceDescription.Alpha, bentNormalWS, -T2W(fragInputs, 2), fragInputs.texCoord1, fragInputs.texCoord2, builtinData );
				#endif

				builtinData.depthOffset = 0.0;
        
				#if (SHADERPASS == SHADERPASS_DISTORTION)
					builtinData.distortion = surfaceDescription.Distortion;
					builtinData.distortionBlur = surfaceDescription.DistortionBlur;
				#else
					builtinData.distortion = float2(0.0, 0.0);
					builtinData.distortionBlur = 0.0;
				#endif
        
                PostInitBuiltinData(V, posInput, surfaceData, builtinData);
            }
        
			PackedVaryingsMeshToPS Vert(AttributesMesh inputMesh /*ase_vert_input*/)
			{
				UNITY_SETUP_INSTANCE_ID( inputMesh );
				PackedVaryingsMeshToPS outputPackedVaryingsMeshToPS;
				UNITY_TRANSFER_INSTANCE_ID( inputMesh, outputPackedVaryingsMeshToPS );
				
				/*ase_vert_code:inputMesh=AttributesMesh;outputPackedVaryingsMeshToPS=PackedVaryingsMeshToPS*/
				inputMesh.vertex.xyz += /*ase_vert_out:Vertex Offset;Float3;3;-1;_VertexOffset*/ float3( 0, 0, 0 ) /*end*/;

				float3 positionRWS = TransformObjectToWorld( inputMesh.vertex.xyz );
				float3 normalWS = TransformObjectToWorldNormal(inputMesh.normal);
				float4 tangentWS = float4(TransformObjectToWorldDir(inputMesh.tangent.xyz), inputMesh.tangent.w);

				outputPackedVaryingsMeshToPS.positionCS = TransformWorldToHClip(positionRWS);
				outputPackedVaryingsMeshToPS.interp00.xyz = positionRWS;
				outputPackedVaryingsMeshToPS.interp01.xyz = normalWS;
				outputPackedVaryingsMeshToPS.interp02.xyzw = tangentWS;
				outputPackedVaryingsMeshToPS.interp03.xyzw = inputMesh.texcoord;
				outputPackedVaryingsMeshToPS.interp04.xyzw = inputMesh.uv1;
				outputPackedVaryingsMeshToPS.interp05.xyzw = inputMesh.uv2;
				outputPackedVaryingsMeshToPS.interp06.xyzw = inputMesh.uv3;
				outputPackedVaryingsMeshToPS.interp07.xyzw = inputMesh.color;
				
				return outputPackedVaryingsMeshToPS;
			}

			void Frag(  PackedVaryingsMeshToPS packedInput
				#ifdef WRITE_NORMAL_BUFFER
				, out float4 outNormalBuffer : SV_Target0
					#ifdef WRITE_MSAA_DEPTH
				, out float1 depthColor : SV_Target1
					#endif
				#endif
				, out float outputDepth : SV_Depth
				/*ase_frag_input*/
			)
			{
				UNITY_SETUP_INSTANCE_ID( packedInput );
				FragInputs input;
				ZERO_INITIALIZE(FragInputs, input);
				SmoothSurfaceDescription surfaceDescription = (SmoothSurfaceDescription)0;
				SurfaceOutput o = (SurfaceOutput)0;

				o.Normal = float3( 0, 0, 1 );
				float4 clipPos = 0;
				float3 worldPos = 0;

				/*ase_frag_code:packedInput=PackedVaryingsMeshToPS*/
				surfaceDescription.Smoothness = /*ase_frag_out:Smoothness;Float;0;-1;_Smoothness*/1/*end*/;
				surfaceDescription.Alpha = /*ase_frag_out:Alpha;Float;1;-1;_Alpha*/1/*end*/;

				#ifdef _ALPHATEST_ON
				surfaceDescription.AlphaClipThreshold = /*ase_frag_out:Alpha Clip Threshold;Float;2;-1;_AlphaClip*/0/*end*/;
				#endif

				packedInput.positionCS.zw = clipPos.zw;
				float3 positionRWS = worldPos;
				float3 normalWS = o.Normal;
				float4 tangentWS = packedInput.interp02.xyzw;
				float4 texCoord0 = packedInput.interp03.xyzw;
				float4 texCoord1 = packedInput.interp04.xyzw;
				float4 texCoord2 = packedInput.interp05.xyzw;
				float4 texCoord3 = packedInput.interp06.xyzw;
				float4 vertexColor = packedInput.interp07.xyzw;

				input.positionSS = packedInput.positionCS;
				input.positionRWS = positionRWS;
#if defined( ASE_SRP_VERSION ) && ASE_SRP_VERSION < 60900
				input.worldToTangent = BuildWorldToTangent( tangentWS, normalWS );
#else
				input.tangentToWorld = BuildTangentToWorld( tangentWS, normalWS );
#endif
				input.texCoord0 = texCoord0;
				input.texCoord1 = texCoord1;
				input.texCoord2 = texCoord2;
				input.texCoord3 = texCoord3;
				input.color = vertexColor;

				PositionInputs posInput = GetPositionInput( input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS );

				float3 normalizedWorldViewDir = GetWorldSpaceNormalizeViewDir( input.positionRWS );

				SurfaceData surfaceData;
				BuiltinData builtinData;

				GetSurfaceAndBuiltinData(surfaceDescription, input, normalizedWorldViewDir, posInput, surfaceData, builtinData);

				outputDepth = posInput.deviceDepth;

				#ifdef WRITE_NORMAL_BUFFER
					EncodeIntoNormalBuffer(ConvertSurfaceDataToNormalData(surfaceData), posInput.positionSS, outNormalBuffer);
					#ifdef WRITE_MSAA_DEPTH
					depthColor = packedInput.positionCS.z;
					#endif
				#endif
			}
            ENDHLSL
        }

		/*ase_pass*/
		Pass
        {
			/*ase_hide_pass*/
            Name "Motion Vectors"
            Tags { "LightMode" = "MotionVectors" }
			Stencil
			{
				WriteMask 128
				Ref 128
				Comp Always
				Pass Replace
			}
        
            HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag

			/*ase_pragma*/
        
			#if defined(_MATERIAL_FEATURE_SUBSURFACE_SCATTERING) && !defined(_SURFACE_TYPE_TRANSPARENT)
			#define OUTPUT_SPLIT_LIGHTING
			#endif
        
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
			#if defined( ASE_SRP_VERSION ) && ASE_SRP_VERSION < 60900              
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Wind.hlsl"
			#endif
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/NormalSurfaceGradient.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
        
            #define SHADERPASS SHADERPASS_VELOCITY
            #pragma multi_compile _ WRITE_NORMAL_BUFFER
            #pragma multi_compile _ WRITE_MSAA_DEPTH
        
            #define VARYINGS_NEED_POSITION_WS
        
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/Lit.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/DecalUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/LitDecalData.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#if defined( ASE_SRP_VERSION ) && ASE_SRP_VERSION < 60900
			#define T2W(var, index) var.worldToTangent[index]
			#else
			#define T2W(var, index) var.tangentToWorld[index]
			#endif

			struct AttributesMesh 
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				/*ase_vdata:p=p;n=n;uv0=tc0*/
			};
        
			struct VaryingsMeshToPS 
			{
				float4 positionCS : SV_Position;
				float3 positionRWS;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct AttributesPass
			{
				float3 previousPositionOS : TEXCOORD4;
			};

			struct VaryingsPassToPS
			{
				float4 positionCS;
				float4 previousPositionCS;
			};

			#define VARYINGS_NEED_PASS
			struct VaryingsToPS
			{
				VaryingsMeshToPS vmesh;
				VaryingsPassToPS vpass;
			};

			struct PackedVaryingsToPS
			{
				float4 vmeshPositionCS : SV_Position;
				float3 vmeshInterp00 : TEXCOORD0; 
				float3 vpassInterpolators0 : TEXCOORD1;
				float3 vpassInterpolators1 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				/*ase_interp(3,):sp=sp.xyzw*/
			};

			/*ase_globals*/

			/*ase_funcs*/
        
			void BuildSurfaceData(FragInputs fragInputs, inout SmoothSurfaceDescription surfaceDescription, float3 V, out SurfaceData surfaceData, out float3 bentNormalWS)
			{
				ZERO_INITIALIZE(SurfaceData, surfaceData);
        
				surfaceData.perceptualSmoothness =      surfaceDescription.Smoothness;
                
				surfaceData.materialFeatures = MATERIALFEATUREFLAGS_LIT_STANDARD;
				#ifdef _MATERIAL_FEATURE_SUBSURFACE_SCATTERING
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_SUBSURFACE_SCATTERING;
				#endif
				#ifdef _MATERIAL_FEATURE_TRANSMISSION
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_TRANSMISSION;
				#endif
				#ifdef _MATERIAL_FEATURE_ANISOTROPY
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_ANISOTROPY;
				#endif
        
				#ifdef _MATERIAL_FEATURE_IRIDESCENCE
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_IRIDESCENCE;
				#endif
				#ifdef _MATERIAL_FEATURE_SPECULAR_COLOR
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_SPECULAR_COLOR;
				#endif
        
				#if defined (_MATERIAL_FEATURE_SPECULAR_COLOR) && defined (_ENERGY_CONSERVING_SPECULAR)
					surfaceData.baseColor *= (1.0 - Max3(surfaceData.specularColor.r, surfaceData.specularColor.g, surfaceData.specularColor.b));
				#endif
        
				GetNormalWS( fragInputs, float3( 0.0, 0.0, 1.0 ), surfaceData.normalWS, float3( 1.0, 1.0, 1.0 ) );
				bentNormalWS = surfaceData.normalWS;

				surfaceData.geomNormalWS = T2W(fragInputs, 2);

				#ifdef _HAS_REFRACTION
					if (_EnableSSRefraction)
					{
        
						surfaceData.transmittanceMask = (1.0 - surfaceDescription.Alpha);
						surfaceDescription.Alpha = 1.0;
					}
					else
					{
						surfaceData.ior = 1.0;
						surfaceData.transmittanceColor = float3(1.0, 1.0, 1.0);
						surfaceData.atDistance = 1.0;
						surfaceData.transmittanceMask = 0.0;
						surfaceDescription.Alpha = 1.0;
					}
				#else
					surfaceData.ior = 1.0;
					surfaceData.transmittanceColor = float3(1.0, 1.0, 1.0);
					surfaceData.atDistance = 1.0;
					surfaceData.transmittanceMask = 0.0;
				#endif
        
					surfaceData.tangentWS = normalize(T2W(fragInputs, 0).xyz);    // The tangent is not normalize in worldToTangent for mikkt. TODO: Check if it expected that we normalize with Morten. Tag: SURFACE_GRADIENT
					surfaceData.tangentWS = Orthonormalize(surfaceData.tangentWS, surfaceData.normalWS);
        
				#if defined(_SPECULAR_OCCLUSION_CUSTOM)
				#elif defined(_SPECULAR_OCCLUSION_FROM_AO_BENT_NORMAL)
					surfaceData.specularOcclusion = GetSpecularOcclusionFromBentAO(V, bentNormalWS, surfaceData.normalWS, surfaceData.ambientOcclusion, PerceptualSmoothnessToPerceptualRoughness(surfaceData.perceptualSmoothness));
				#elif defined(_AMBIENT_OCCLUSION) && defined(_SPECULAR_OCCLUSION_FROM_AO)
					surfaceData.specularOcclusion = GetSpecularOcclusionFromAmbientOcclusion(ClampNdotV(dot(surfaceData.normalWS, V)), surfaceData.ambientOcclusion, PerceptualSmoothnessToRoughness(surfaceData.perceptualSmoothness));
				#else
					surfaceData.specularOcclusion = 1.0;
				#endif
				#ifdef _ENABLE_GEOMETRIC_SPECULAR_AA
					surfaceData.perceptualSmoothness = GeometricNormalFiltering(surfaceData.perceptualSmoothness, T2W(fragInputs, 2), surfaceDescription.SpecularAAScreenSpaceVariance, surfaceDescription.SpecularAAThreshold);
				#endif
			}
        
			void GetSurfaceAndBuiltinData( SmoothSurfaceDescription surfaceDescription, FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData)
			{
				#ifdef LOD_FADE_CROSSFADE
					uint3 fadeMaskSeed = asuint((int3)(V * _ScreenSize.xyx));
					LODDitheringTransition(fadeMaskSeed, unity_LODFade.x);
				#endif
        
				#ifdef _ALPHATEST_ON
					DoAlphaTest ( surfaceDescription.Alpha, surfaceDescription.AlphaClipThreshold );
				#endif
					float3 bentNormalWS;
					BuildSurfaceData(fragInputs, surfaceDescription, V, surfaceData, bentNormalWS);
        
				#if HAVE_DECALS
				if( _EnableDecals )
				{
					DecalSurfaceData decalSurfaceData = GetDecalSurfaceData(posInput, fragInputs.tangentToWorld[2], surfaceDescription.Alpha);
					ApplyDecalToSurfaceData(decalSurfaceData, fragInputs.tangentToWorld[2], surfaceData);
				}
				#endif
        
				#if defined( ASE_SRP_VERSION ) && ASE_SRP_VERSION <= 50702
					InitBuiltinData( surfaceDescription.Alpha, bentNormalWS, -T2W(fragInputs, 2), fragInputs.positionRWS, fragInputs.texCoord1, fragInputs.texCoord2, builtinData );
				#else
					InitBuiltinData( posInput, surfaceDescription.Alpha, bentNormalWS, -T2W(fragInputs, 2), fragInputs.texCoord1, fragInputs.texCoord2, builtinData );
				#endif

				builtinData.depthOffset = 0.0;
        
				#if (SHADERPASS == SHADERPASS_DISTORTION)
					builtinData.distortion = surfaceDescription.Distortion;
					builtinData.distortionBlur = surfaceDescription.DistortionBlur;
				#else
					builtinData.distortion = float2(0.0, 0.0);
					builtinData.distortionBlur = 0.0;
				#endif
        
				PostInitBuiltinData(V, posInput, surfaceData, builtinData);
			}
        
			VaryingsPassToPS UnpackVaryingsPassToPS(PackedVaryingsToPS input)
			{
				VaryingsPassToPS output;
				output.positionCS = float4(input.vpassInterpolators0.xy, 0.0, input.vpassInterpolators0.z);
				output.previousPositionCS = float4(input.vpassInterpolators1.xy, 0.0, input.vpassInterpolators1.z);

				return output;
			}

#if UNITY_VERSION < 201930
			float3 TransformPreviousObjectToWorldNormal(float3 normalOS)
			{
			#ifdef UNITY_ASSUME_UNIFORM_SCALING
				return normalize(mul((float3x3)unity_MatrixPreviousM, normalOS));
			#else
				return normalize(mul(normalOS, (float3x3)unity_MatrixPreviousMI));
			#endif
			}

			float3 TransformPreviousObjectToWorld(float3 positionOS)
			{
				float4x4 previousModelMatrix = ApplyCameraTranslationToMatrix(unity_MatrixPreviousM);
				return mul(previousModelMatrix, float4(positionOS, 1.0)).xyz;
			}
#endif

			void VelocityPositionZBias(VaryingsToPS input)
			{
			#if defined(UNITY_REVERSED_Z)
				input.vmesh.positionCS.z -= unity_MotionVectorsParams.z * input.vmesh.positionCS.w;
			#else
				input.vmesh.positionCS.z += unity_MotionVectorsParams.z * input.vmesh.positionCS.w;
			#endif
			}

			PackedVaryingsToPS Vert(AttributesMesh inputMesh, AttributesPass inputPass /*ase_vert_input*/ )
			{
				UNITY_SETUP_INSTANCE_ID(inputMesh);
				PackedVaryingsToPS outputPackedVaryingsToPS;
				VaryingsToPS varyingsType;
				VaryingsMeshToPS outputVaryingsMeshToPS;

				UNITY_TRANSFER_INSTANCE_ID(inputMesh, outputVaryingsMeshToPS);

				/*ase_vert_code:inputMesh=AttributesMesh;outputPackedVaryingsToPS=PackedVaryingsToPS*/
				inputMesh.vertex.xyz += /*ase_vert_out:Vertex Offset;Float3;3;-1;_VertexOffset*/ float3( 0, 0, 0 ) /*end*/;

				float3 positionRWS = TransformObjectToWorld( inputMesh.vertex.xyz );

				outputVaryingsMeshToPS.positionRWS = positionRWS;
				outputVaryingsMeshToPS.positionCS = TransformWorldToHClip(positionRWS);
					
				varyingsType.vmesh = outputVaryingsMeshToPS;

				VelocityPositionZBias(varyingsType);
				varyingsType.vpass.positionCS = mul(_NonJitteredViewProjMatrix, float4(varyingsType.vmesh.positionRWS, 1.0));

				bool forceNoMotion = unity_MotionVectorsParams.y == 0.0;
				if (forceNoMotion)
				{
					varyingsType.vpass.previousPositionCS = float4(0.0, 0.0, 0.0, 1.0);
				}
				else
				{
					bool hasDeformation = unity_MotionVectorsParams.x > 0.0; // Skin or morph target
					float3 previousPositionRWS = TransformPreviousObjectToWorld(hasDeformation ? inputPass.previousPositionOS : inputMesh.vertex.xyz );
					varyingsType.vpass.previousPositionCS = mul(_PrevViewProjMatrix, float4(previousPositionRWS, 1.0));
				}

				outputPackedVaryingsToPS.vmeshPositionCS = varyingsType.vmesh.positionCS;
				outputPackedVaryingsToPS.vmeshInterp00.xyz = varyingsType.vmesh.positionRWS;
				#if INSTANCING_ON
				outputPackedVaryingsToPS.vmeshInstanceID = varyingsType.vmeshInstanceID;
				#endif 
					
				outputPackedVaryingsToPS.vpassInterpolators0 = float3(varyingsType.vpass.positionCS.xyw);
				outputPackedVaryingsToPS.vpassInterpolators1 = float3(varyingsType.vpass.previousPositionCS.xyw);
				return outputPackedVaryingsToPS;
			}

			void Frag(  PackedVaryingsToPS packedInput
				, out float4 outVelocity : SV_Target0
				#ifdef WRITE_NORMAL_BUFFER
				, out float4 outNormalBuffer : SV_Target1
					#ifdef WRITE_MSAA_DEPTH
					, out float1 depthColor : SV_Target2
					#endif
				#endif
				, out float outputDepth : SV_Depth
				/*ase_frag_input*/
			)
			{
				UNITY_SETUP_INSTANCE_ID( packedInput );
				FragInputs input;
				ZERO_INITIALIZE(FragInputs, input);
				SmoothSurfaceDescription surfaceDescription = (SmoothSurfaceDescription)0;
				SurfaceOutput o = (SurfaceOutput)0;

				o.Normal = float3( 0, 0, 1 );
				float4 clipPos = 0;
				float3 worldPos = 0;

                /*ase_frag_code:packedInput=PackedVaryingsToPS*/
				surfaceDescription.Smoothness = /*ase_frag_out:Smoothness;Float;0;-1;_Smoothness*/1/*end*/;
				surfaceDescription.Alpha = /*ase_frag_out:Alpha;Float;1;-1;_Alpha*/1/*end*/;
					
				#ifdef _ALPHATEST_ON
				surfaceDescription.AlphaClipThreshold = /*ase_frag_out:Alpha Clip Threshold;Float;2;-1;_AlphaClip*/0/*end*/;
                #endif
				
				packedInput.vmeshPositionCS.zw = clipPos.zw;
				float3 positionRWS = worldPos;
				float3 normalWS = o.Normal;

#if defined( ASE_SRP_VERSION ) && ASE_SRP_VERSION < 60900
				input.worldToTangent = k_identity3x3;
#else
				input.tangentToWorld = k_identity3x3;
#endif
				input.positionSS = packedInput.vmeshPositionCS; 
				input.positionRWS = positionRWS;
					
				PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS);

				float3 normalizedWorldViewDir = GetWorldSpaceNormalizeViewDir( input.positionRWS );

				SurfaceData surfaceData;
				BuiltinData builtinData;

				GetSurfaceAndBuiltinData(surfaceDescription, input, normalizedWorldViewDir, posInput, surfaceData, builtinData);

				VaryingsPassToPS inputPass = UnpackVaryingsPassToPS(packedInput);
#if defined( ASE_SRP_VERSION ) && ASE_SRP_VERSION <= 50702
				float2 velocity = CalculateVelocity(inputPass.positionCS, inputPass.previousPositionCS);
				EncodeVelocity(velocity * 0.5, outVelocity);
#else
				float2 velocity = CalculateMotionVector( inputPass.positionCS, inputPass.previousPositionCS );
				EncodeMotionVector( velocity * 0.5, outVelocity );
#endif

				bool forceNoMotion = unity_MotionVectorsParams.y == 0.0;
				if (forceNoMotion)
					outVelocity = float4(0.0, 0.0, 0.0, 0.0);

				#ifdef WRITE_NORMAL_BUFFER
					EncodeIntoNormalBuffer(ConvertSurfaceDataToNormalData(surfaceData), posInput.positionSS, outNormalBuffer);

					#ifdef WRITE_MSAA_DEPTH
					depthColor = packedInput.vmesh.positionCS.z;
					#endif
				#endif

				outputDepth = posInput.deviceDepth;
			}

            ENDHLSL
        }

		/*ase_pass*/
        Pass
        {
            Name "Distortion"
            Tags { "LightMode" = "DistortionVectors" }
        
            Blend One One, One One
			BlendOp Add, Max
        
			ZTest LEqual

            HLSLPROGRAM
        
			#pragma vertex Vert
			#pragma fragment Frag
        
			/*ase_pragma*/

			#if defined(_MATERIAL_FEATURE_SUBSURFACE_SCATTERING) && !defined(_SURFACE_TYPE_TRANSPARENT)
			#define OUTPUT_SPLIT_LIGHTING
			#endif
        
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
			#if defined( ASE_SRP_VERSION ) && ASE_SRP_VERSION < 60900              
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Wind.hlsl"
			#endif
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/NormalSurfaceGradient.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
        
            #define SHADERPASS SHADERPASS_DISTORTION
        
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/Lit.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/DecalUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/LitDecalData.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#if defined( ASE_SRP_VERSION ) && ASE_SRP_VERSION < 60900
			#define T2W(var, index) var.worldToTangent[index]
			#else
			#define T2W(var, index) var.tangentToWorld[index]
			#endif

			struct AttributesMesh 
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				/*ase_vdata:p=p;n=n;uv0=tc0*/
			};

			struct PackedVaryingsMeshToPS 
			{
				float4 positionCS : SV_Position;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				/*ase_interp(0,):sp=sp.xyzw*/
			};

			/*ase_globals*/

			/*ase_funcs*/
        
			void BuildSurfaceData(FragInputs fragInputs, inout DistortionSurfaceDescription surfaceDescription, float3 V, out SurfaceData surfaceData, out float3 bentNormalWS)
			{
				ZERO_INITIALIZE(SurfaceData, surfaceData);
				#ifdef _HAS_REFRACTION
					if (_EnableSSRefraction)
					{
						surfaceData.transmittanceMask = (1.0 - surfaceDescription.Alpha);
						surfaceDescription.Alpha = 1.0;
					}
					else
					{
						surfaceData.ior = 1.0;
						surfaceData.transmittanceColor = float3(1.0, 1.0, 1.0);
						surfaceData.atDistance = 1.0;
						surfaceData.transmittanceMask = 0.0;
						surfaceDescription.Alpha = 1.0;
					}
				#else
					surfaceData.ior = 1.0;
					surfaceData.transmittanceColor = float3(1.0, 1.0, 1.0);
					surfaceData.atDistance = 1.0;
					surfaceData.transmittanceMask = 0.0;
				#endif
                
					surfaceData.materialFeatures = MATERIALFEATUREFLAGS_LIT_STANDARD;
				#ifdef _MATERIAL_FEATURE_SUBSURFACE_SCATTERING
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_SUBSURFACE_SCATTERING;
				#endif
				#ifdef _MATERIAL_FEATURE_TRANSMISSION
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_TRANSMISSION;
				#endif
				#ifdef _MATERIAL_FEATURE_ANISOTROPY
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_ANISOTROPY;
				#endif
        
				#ifdef _MATERIAL_FEATURE_IRIDESCENCE
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_IRIDESCENCE;
				#endif
				#ifdef _MATERIAL_FEATURE_SPECULAR_COLOR
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_SPECULAR_COLOR;
				#endif
        
				#if defined (_MATERIAL_FEATURE_SPECULAR_COLOR) && defined (_ENERGY_CONSERVING_SPECULAR)
					surfaceData.baseColor *= (1.0 - Max3(surfaceData.specularColor.r, surfaceData.specularColor.g, surfaceData.specularColor.b));
				#endif
        
				GetNormalWS( fragInputs, float3( 0.0, 0.0, 1.0 ), surfaceData.normalWS, float3( 1.0, 1.0, 1.0 ) );
				bentNormalWS = surfaceData.normalWS;
        
				surfaceData.geomNormalWS = T2W(fragInputs, 2);

				surfaceData.tangentWS = normalize(T2W(fragInputs, 0).xyz);    // The tangent is not normalize in worldToTangent for mikkt. TODO: Check if it expected that we normalize with Morten. Tag: SURFACE_GRADIENT
				surfaceData.tangentWS = Orthonormalize(surfaceData.tangentWS, surfaceData.normalWS);
				#if defined(_SPECULAR_OCCLUSION_CUSTOM)
				#elif defined(_SPECULAR_OCCLUSION_FROM_AO_BENT_NORMAL)
					surfaceData.specularOcclusion = GetSpecularOcclusionFromBentAO(V, bentNormalWS, surfaceData.normalWS, surfaceData.ambientOcclusion, PerceptualSmoothnessToPerceptualRoughness(surfaceData.perceptualSmoothness));
				#elif defined(_AMBIENT_OCCLUSION) && defined(_SPECULAR_OCCLUSION_FROM_AO)
					surfaceData.specularOcclusion = GetSpecularOcclusionFromAmbientOcclusion(ClampNdotV(dot(surfaceData.normalWS, V)), surfaceData.ambientOcclusion, PerceptualSmoothnessToRoughness(surfaceData.perceptualSmoothness));
				#else
					surfaceData.specularOcclusion = 1.0;
				#endif
				#ifdef _ENABLE_GEOMETRIC_SPECULAR_AA
					surfaceData.perceptualSmoothness = GeometricNormalFiltering(surfaceData.perceptualSmoothness, T2W(fragInputs, 2), surfaceDescription.SpecularAAScreenSpaceVariance, surfaceDescription.SpecularAAThreshold);
				#endif
			}
        
			void GetSurfaceAndBuiltinData(DistortionSurfaceDescription surfaceDescription, FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData)
			{
				#ifdef LOD_FADE_CROSSFADE
					uint3 fadeMaskSeed = asuint((int3)(V * _ScreenSize.xyx));
					LODDitheringTransition(fadeMaskSeed, unity_LODFade.x);
				#endif
        
				#ifdef _ALPHATEST_ON
					DoAlphaTest(surfaceDescription.Alpha, surfaceDescription.AlphaClipThreshold);
				#endif
					float3 bentNormalWS;
					BuildSurfaceData(fragInputs, surfaceDescription, V, surfaceData, bentNormalWS);
        
				#if HAVE_DECALS
				if( _EnableDecals )
				{
					DecalSurfaceData decalSurfaceData = GetDecalSurfaceData(posInput, fragInputs.tangentToWorld[2], surfaceDescription.Alpha);
					ApplyDecalToSurfaceData(decalSurfaceData, fragInputs.tangentToWorld[2], surfaceData);
				}
				#endif
        
				#if defined( ASE_SRP_VERSION ) && ASE_SRP_VERSION <= 50702
					InitBuiltinData( surfaceDescription.Alpha, bentNormalWS, -T2W(fragInputs, 2), fragInputs.positionRWS, fragInputs.texCoord1, fragInputs.texCoord2, builtinData );
				#else
					InitBuiltinData( posInput, surfaceDescription.Alpha, bentNormalWS, -T2W(fragInputs, 2), fragInputs.texCoord1, fragInputs.texCoord2, builtinData );
				#endif

				builtinData.depthOffset = 0.0;
        
				builtinData.distortion = surfaceDescription.Distortion;
				builtinData.distortionBlur = surfaceDescription.DistortionBlur;
        
				PostInitBuiltinData(V, posInput, surfaceData, builtinData);
			}

			PackedVaryingsMeshToPS Vert(AttributesMesh inputMesh /*ase_vert_input*/ )
			{
				UNITY_SETUP_INSTANCE_ID(inputMesh);
				PackedVaryingsMeshToPS outputPackedVaryingsMeshToPS;
				UNITY_TRANSFER_INSTANCE_ID(inputMesh, outputPackedVaryingsMeshToPS);
    
				/*ase_vert_code:inputMesh=AttributesMesh;outputPackedVaryingsMeshToPS=PackedVaryingsMeshToPS*/
				inputMesh.vertex.xyz += /*ase_vert_out:Vertex Offset;Float3;4;-1;_VertexOffset*/ float3( 0, 0, 0 ) /*end*/;
					
				float3 positionRWS = TransformObjectToWorld( inputMesh.vertex.xyz );

				outputPackedVaryingsMeshToPS.positionCS = TransformWorldToHClip(positionRWS);
					
				return outputPackedVaryingsMeshToPS;
			}

			float4 Frag(PackedVaryingsMeshToPS packedInput, out float outputDepth : SV_Depth /*ase_frag_input*/ ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( packedInput );
				FragInputs input;
				ZERO_INITIALIZE(FragInputs, input);
					
				DistortionSurfaceDescription surfaceDescription = (DistortionSurfaceDescription)0;
				SurfaceOutput o = (SurfaceOutput)0;

				o.Normal = float3( 0, 0, 1 );
				float4 clipPos = 0;
				float3 worldPos = 0;

				/*ase_frag_code:packedInput=PackedVaryingsMeshToPS*/
				surfaceDescription.Alpha = /*ase_frag_out:Alpha;Float;0;-1;_Alpha*/1/*end*/;

				#ifdef _ALPHATEST_ON
				surfaceDescription.AlphaClipThreshold = /*ase_frag_out:Alpha Clip Threshold;Float;1;-1;_AlphaClip*/0/*end*/;
				#endif

                surfaceDescription.Distortion = /*ase_frag_out:Distortion;Float2;2;-1;_Distortion*/float2 (2,-1)/*end*/;
                surfaceDescription.DistortionBlur = /*ase_frag_out:Distortion Blur;Float;3;-1;_DistortionBlur*/1/*end*/;;

				packedInput.positionCS.zw = clipPos.zw;
				float3 positionRWS = worldPos;
				float3 normalWS = o.Normal;
				input.positionRWS = positionRWS;
#if defined( ASE_SRP_VERSION ) && ASE_SRP_VERSION < 60900
				input.worldToTangent = k_identity3x3;
#else
				input.tangentToWorld = k_identity3x3;
#endif
				input.positionSS = packedInput.positionCS;

				PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS);
				float3 V = float3(1.0, 1.0, 1.0); 
				SurfaceData surfaceData;
				BuiltinData builtinData;

				GetSurfaceAndBuiltinData(surfaceDescription, input, V, posInput, surfaceData, builtinData);
				float4 outBuffer;
				EncodeDistortion(builtinData.distortion, builtinData.distortionBlur, true, outBuffer);

				outputDepth = posInput.deviceDepth;

				return outBuffer;
			}
            ENDHLSL
        }

		/*ase_pass*/
		Pass
        {
			/*ase_hide_pass:SyncP*/
            Name "Forward"
            Tags { "LightMode" = "Forward" }
			Stencil
			{
			   WriteMask 7
			   Ref  2
			   Comp Always
			   Pass Replace
			}
        
            HLSLPROGRAM
        
			#pragma vertex Vert
			#pragma fragment Frag
				
			/*ase_pragma*/
        
			#if defined(_MATERIAL_FEATURE_SUBSURFACE_SCATTERING) && !defined(_SURFACE_TYPE_TRANSPARENT)
			#define OUTPUT_SPLIT_LIGHTING
			#endif
        
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
			#if defined( ASE_SRP_VERSION ) && ASE_SRP_VERSION < 60900              
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Wind.hlsl"
			#endif
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/NormalSurfaceGradient.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
        
            #define SHADERPASS SHADERPASS_FORWARD
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ DYNAMICLIGHTMAP_ON
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile DECALS_OFF DECALS_3RT DECALS_4RT
            #define LIGHTLOOP_TILE_PASS
            #pragma multi_compile USE_FPTL_LIGHTLIST USE_CLUSTERED_LIGHTLIST
            #pragma multi_compile SHADOW_LOW SHADOW_MEDIUM SHADOW_HIGH
        
            #define VARYINGS_NEED_POSITION_WS
            #define VARYINGS_NEED_TANGENT_TO_WORLD
            #define VARYINGS_NEED_TEXCOORD1
            #define VARYINGS_NEED_TEXCOORD2
        
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/Lighting.hlsl"
        
			#define HAS_LIGHTLOOP
        
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/LightLoop/LightLoopDef.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/Lit.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/LightLoop/LightLoop.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/DecalUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/LitDecalData.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#if defined( ASE_SRP_VERSION ) && ASE_SRP_VERSION < 60900
			#define T2W(var, index) var.worldToTangent[index]
			#else
			#define T2W(var, index) var.tangentToWorld[index]
			#endif

			struct AttributesMesh 
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL; 
				float4 tangent : TANGENT; 
				float4 texcoord : TEXCOORD0;
				float4 uv1 : TEXCOORD1;
				float4 uv2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				/*ase_vdata:p=p;n=n;t=t;uv0=tc0;uv1=tc1;uv2=tc2*/
			};
        
			struct PackedVaryingsMeshToPS 
			{
				float4 positionCS : SV_Position;
				float3 interp00 : TEXCOORD0;
				float3 interp01 : TEXCOORD1;
				float4 interp02 : TEXCOORD2;
				float4 interp03 : TEXCOORD3;
				float4 interp04 : TEXCOORD4;
				float4 interp05 : TEXCOORD5;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				/*ase_interp(6,):sp=sp.xyzw;rwp=tc0;wn=tc1;wt=tc2;uv0=tc3;uv1=tc4;uv2=tc5*/
			};

			/*ase_globals*/

			/*ase_funcs*/
                  
			void BuildSurfaceData(FragInputs fragInputs, inout GlobalSurfaceDescription surfaceDescription, float3 V, PositionInputs posInput, out SurfaceData surfaceData, out float3 bentNormalWS)
            {
                ZERO_INITIALIZE(SurfaceData, surfaceData);
                surfaceData.baseColor =                 surfaceDescription.Albedo;
                surfaceData.perceptualSmoothness =      surfaceDescription.Smoothness;
                surfaceData.ambientOcclusion =          surfaceDescription.Occlusion;
                surfaceData.specularColor =             surfaceDescription.Specular;
                surfaceData.coatMask =                  surfaceDescription.CoatMask;
        
				#ifdef _HAS_REFRACTION
					if (_EnableSSRefraction)
					{
						surfaceData.ior =                       surfaceDescription.RefractionIndex;
						surfaceData.transmittanceColor =        surfaceDescription.RefractionColor;
						surfaceData.atDistance =                surfaceDescription.RefractionDistance;

						surfaceData.transmittanceMask = (1.0 - surfaceDescription.Alpha);
						surfaceDescription.Alpha = 1.0;
					}
					else
					{
						surfaceData.ior = 1.0;
						surfaceData.transmittanceColor = float3(1.0, 1.0, 1.0);
						surfaceData.atDistance = 1.0;
						surfaceData.transmittanceMask = 0.0;
						surfaceDescription.Alpha = 1.0;
					}
				#else
					surfaceData.ior = 1.0;
					surfaceData.transmittanceColor = float3(1.0, 1.0, 1.0);
					surfaceData.atDistance = 1.0;
					surfaceData.transmittanceMask = 0.0;
				#endif
                
				#ifdef _MATERIAL_FEATURE_IRIDESCENCE		
					surfaceData.iridescenceMask =           surfaceDescription.IridescenceMask;
					surfaceData.iridescenceThickness =      surfaceDescription.IridescenceThickness;
				#endif

				surfaceData.materialFeatures = MATERIALFEATUREFLAGS_LIT_STANDARD;
				#ifdef _MATERIAL_FEATURE_SUBSURFACE_SCATTERING
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_SUBSURFACE_SCATTERING;
				#endif
				#ifdef _MATERIAL_FEATURE_TRANSMISSION
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_TRANSMISSION;
				#endif
				#ifdef _MATERIAL_FEATURE_ANISOTROPY
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_ANISOTROPY;
				#endif
				#ifdef ASE_LIT_CLEAR_COAT
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_CLEAR_COAT;
				#endif
				#ifdef _MATERIAL_FEATURE_IRIDESCENCE
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_IRIDESCENCE;
				#endif
				#ifdef _MATERIAL_FEATURE_SPECULAR_COLOR
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_SPECULAR_COLOR;
				#endif
        
				#if defined (_MATERIAL_FEATURE_SPECULAR_COLOR) && defined (_ENERGY_CONSERVING_SPECULAR)
					surfaceData.baseColor *= (1.0 - Max3(surfaceData.specularColor.r, surfaceData.specularColor.g, surfaceData.specularColor.b));
				#endif
        
				GetNormalWS( fragInputs, float3( 0.0, 0.0, 1.0 ), surfaceData.normalWS, float3( 1.0, 1.0, 1.0 ) );
        
				bentNormalWS = surfaceData.normalWS;
				
				#ifdef ASE_BENT_NORMAL
					GetNormalWS(fragInputs, surfaceDescription.BentNormal, bentNormalWS, float3( 1, 1, 1 ) );
				#endif

				surfaceData.geomNormalWS = T2W(fragInputs, 2);
				
				#if defined(_HAS_REFRACTION) || defined(_MATERIAL_FEATURE_TRANSMISSION)
					surfaceData.thickness =	                 surfaceDescription.Thickness;
				#endif

				#ifdef _MATERIAL_FEATURE_SUBSURFACE_SCATTERING
					surfaceData.subsurfaceMask =            surfaceDescription.SubsurfaceMask;
					surfaceData.diffusionProfile =          surfaceDescription.DiffusionProfile;
				#endif

				surfaceData.tangentWS = normalize( T2W(fragInputs, 0).xyz );
				#ifdef _MATERIAL_FEATURE_ANISOTROPY
					surfaceData.anisotropy = surfaceDescription.Anisotropy;
					surfaceData.tangentWS = TransformTangentToWorld(surfaceDescription.Tangent, fragInputs.worldToTangent);
				#endif
				surfaceData.tangentWS = Orthonormalize(surfaceData.tangentWS, surfaceData.normalWS);
        
				#if defined(_SPECULAR_OCCLUSION_CUSTOM)
					surfaceData.specularOcclusion = surfaceDescription.SpecularOcclusion;
				#elif defined(_SPECULAR_OCCLUSION_FROM_AO_BENT_NORMAL)
					surfaceData.specularOcclusion = GetSpecularOcclusionFromBentAO(V, bentNormalWS, surfaceData.normalWS, surfaceData.ambientOcclusion, PerceptualSmoothnessToPerceptualRoughness(surfaceData.perceptualSmoothness));
				#elif defined(_AMBIENT_OCCLUSION) && defined(_SPECULAR_OCCLUSION_FROM_AO)
					surfaceData.specularOcclusion = GetSpecularOcclusionFromAmbientOcclusion(ClampNdotV(dot(surfaceData.normalWS, V)), surfaceData.ambientOcclusion, PerceptualSmoothnessToRoughness(surfaceData.perceptualSmoothness));
				#else
					surfaceData.specularOcclusion = 1.0;
				#endif
        
				#ifdef _ENABLE_GEOMETRIC_SPECULAR_AA
					surfaceData.perceptualSmoothness = GeometricNormalFiltering(surfaceData.perceptualSmoothness, T2W(fragInputs, 2), surfaceDescription.SpecularAAScreenSpaceVariance, surfaceDescription.SpecularAAThreshold);
				#endif
            }
			
			void GetSurfaceAndBuiltinData(GlobalSurfaceDescription surfaceDescription,FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData)
			{
				#ifdef LOD_FADE_CROSSFADE
					uint3 fadeMaskSeed = asuint((int3)(V * _ScreenSize.xyx));
					LODDitheringTransition(fadeMaskSeed, unity_LODFade.x);
				#endif
        
				#ifdef _ALPHATEST_ON
						DoAlphaTest(surfaceDescription.Alpha, surfaceDescription.AlphaClipThreshold);
				#endif
        
				float3 bentNormalWS;
				BuildSurfaceData(fragInputs, surfaceDescription, V, posInput, surfaceData, bentNormalWS);
        
				#if HAVE_DECALS
				if( _EnableDecals )
				{
					DecalSurfaceData decalSurfaceData = GetDecalSurfaceData(posInput, fragInputs.tangentToWorld[2], surfaceDescription.Alpha);
					ApplyDecalToSurfaceData(decalSurfaceData, fragInputs.tangentToWorld[2], surfaceData);
				}
				#endif
        
				#if defined( ASE_SRP_VERSION ) && ASE_SRP_VERSION <= 50702
					InitBuiltinData( surfaceDescription.Alpha, bentNormalWS, -T2W(fragInputs, 2), fragInputs.positionRWS, fragInputs.texCoord1, fragInputs.texCoord2, builtinData );
				#else
					InitBuiltinData( posInput, surfaceDescription.Alpha, bentNormalWS, -T2W(fragInputs, 2), fragInputs.texCoord1, fragInputs.texCoord2, builtinData );
				#endif

				builtinData.emissiveColor = surfaceDescription.Emission;
        
				builtinData.depthOffset = 0.0;
        
				#if (SHADERPASS == SHADERPASS_DISTORTION)
					builtinData.distortion = surfaceDescription.Distortion;
					builtinData.distortionBlur = surfaceDescription.DistortionBlur;
				#else
					builtinData.distortion = float2(0.0, 0.0);
					builtinData.distortionBlur = 0.0;
				#endif
        
				PostInitBuiltinData(V, posInput, surfaceData, builtinData);
			}
    
			PackedVaryingsMeshToPS Vert(AttributesMesh inputMesh /*ase_vert_input*/)
			{
				UNITY_SETUP_INSTANCE_ID( inputMesh );
				PackedVaryingsMeshToPS outputPackedVaryingsMeshToPS;
				UNITY_TRANSFER_INSTANCE_ID( inputMesh, outputPackedVaryingsMeshToPS );

				/*ase_vert_code:inputMesh=AttributesMesh;outputPackedVaryingsMeshToPS=PackedVaryingsMeshToPS*/
				inputMesh.vertex.xyz += /*ase_vert_out:Vertex Offset;Float3;11;-1;_VertexOffset*/ float3( 0, 0, 0 ) /*end*/;

				float3 positionRWS = TransformObjectToWorld(inputMesh.vertex.xyz );
				float3 normalWS = TransformObjectToWorldNormal(inputMesh.normal);
				float4 tangentWS = float4(TransformObjectToWorldDir(inputMesh.tangent.xyz), inputMesh.tangent.w);

				outputPackedVaryingsMeshToPS.positionCS = TransformWorldToHClip(positionRWS);
				outputPackedVaryingsMeshToPS.interp00.xyz = positionRWS;
				outputPackedVaryingsMeshToPS.interp01.xyz = normalWS;
				outputPackedVaryingsMeshToPS.interp02.xyzw = tangentWS;
				outputPackedVaryingsMeshToPS.interp03.xyzw = inputMesh.texcoord;
				outputPackedVaryingsMeshToPS.interp04.xyzw = inputMesh.uv1;
				outputPackedVaryingsMeshToPS.interp05.xyzw = inputMesh.uv2;
				return outputPackedVaryingsMeshToPS;
			}

			void Frag(PackedVaryingsMeshToPS packedInput,
				#ifdef OUTPUT_SPLIT_LIGHTING
					out float4 outColor : SV_Target0,
					out float4 outDiffuseLighting : SV_Target1,
					OUTPUT_SSSBUFFER(outSSSBuffer)
				#else
					out float4 outColor : SV_Target0
				#endif
					, out float outputDepth : SV_Depth
				/*ase_frag_input*/
			)
			{
				UNITY_SETUP_INSTANCE_ID( packedInput );
				FragInputs input;
				ZERO_INITIALIZE(FragInputs, input);
				GlobalSurfaceDescription surfaceDescription = (GlobalSurfaceDescription)0;
				SurfaceOutput o = (SurfaceOutput)0;

				o.Normal = packedInput.interp01.xyz; //float3( 0, 0, 1 );
				float4 clipPos = packedInput.positionCS;  //0;
				float3 worldPos = packedInput.interp00.xyz; //0;

				/*ase_frag_code:packedInput=PackedVaryingsMeshToPS*/
				surfaceDescription.Albedo = /*ase_frag_out:Albedo;Float3;0;-1;_Albedo*/float3( 0.5, 0.5, 0.5 )/*end*/;
				surfaceDescription.Normal = /*ase_frag_out:World Normal;Float3;1;-1;_WorldNormal*/float3( 0.0, 0.0, 1.0 )/*end*/;
				surfaceDescription.BentNormal = /*ase_frag_out:Bent Normal;Float3;2;-1;_BentNormal*/float3( 0, 0, 1 )/*end*/;
				surfaceDescription.CoatMask = /*ase_frag_out:Coat Mask;Float;3;-1;_CoatMask*/0/*end*/;
				surfaceDescription.Metallic = /*ase_frag_out:Metallic;Float;4;-1;_Metallic*/0/*end*/;
					
				#ifdef _MATERIAL_FEATURE_SPECULAR_COLOR
				surfaceDescription.Specular = /*ase_frag_out:Specular;Float3;5;-1;_Specular*/0/*end*/;
				#endif
					
				surfaceDescription.Emission = /*ase_frag_out:Emission;Float3;6;-1;_Emission*/0/*end*/;
				surfaceDescription.Smoothness = /*ase_frag_out:Smoothness;Float;7;-1;_Smoothness*/0.5/*end*/;
				surfaceDescription.Occlusion = /*ase_frag_out:Occlusion;Float;8;-1;_Occlusion*/1/*end*/;
				surfaceDescription.Alpha = /*ase_frag_out:Alpha;Float;9;-1;_Alpha*/1/*end*/;
					
				#ifdef _ALPHATEST_ON
				surfaceDescription.AlphaClipThreshold = /*ase_frag_out:Alpha Clip Threshold;Float;10;-1;_AlphaClip*/0/*end*/;
				#endif

				#ifdef _ENABLE_GEOMETRIC_SPECULAR_AA
				surfaceDescription.SpecularAAScreenSpaceVariance = /*ase_frag_out:SpecularAAScreenSpaceVariance;Float;13;-1;_SpecularAAScreenSpaceVariance*/0/*end*/;
				surfaceDescription.SpecularAAThreshold = /*ase_frag_out:SpecularAAThreshold;Float;14;-1;_SpecularAAThreshold*/0/*end*/;
				#endif

				#ifdef _SPECULAR_OCCLUSION_CUSTOM
				surfaceDescription.SpecularOcclusion = /*ase_frag_out:SpecularOcclusion;Float;15;-1;_SpecularOcclusion*/0/*end*/;
				#endif

				#if defined(_HAS_REFRACTION) || defined(_MATERIAL_FEATURE_TRANSMISSION)
				surfaceDescription.Thickness = /*ase_frag_out:Thickness;Float;16;-1;_Thickness*/1/*end*/;
				#endif

				#ifdef _HAS_REFRACTION
				surfaceDescription.RefractionIndex = /*ase_frag_out:RefractionIndex;Float;17;-1;_RefractionIndex*/1/*end*/;
				surfaceDescription.RefractionColor = /*ase_frag_out:RefractionColor;Float3;18;-1;_RefractionColor*/float3(1,1,1)/*end*/;
				surfaceDescription.RefractionDistance = /*ase_frag_out:RefractionDistance;Float;19;-1;_RefractionDistance*/0/*end*/;
				#endif

				#ifdef _MATERIAL_FEATURE_SUBSURFACE_SCATTERING
				surfaceDescription.SubsurfaceMask = /*ase_frag_out:SubsurfaceMask;Float;20;-1;_SubsurfaceMask*/1/*end*/;
				surfaceDescription.DiffusionProfile = /*ase_frag_out:DiffusionProfile;Float;21;-1;_DiffusionProfile*/0/*end*/;
				#endif

				#ifdef _MATERIAL_FEATURE_ANISOTROPY
				surfaceDescription.Anisotropy = /*ase_frag_out:Anisotropy;Float;22;-1;_Anisotropy*/1/*end*/;
				surfaceDescription.Tangent = /*ase_frag_out:Tangent;Float3;23;-1;_Tangent*/float3(1,0,0)/*end*/;
				#endif

				#ifdef _MATERIAL_FEATURE_IRIDESCENCE
				surfaceDescription.IridescenceMask = /*ase_frag_out:IridescenceMask;Float;24;-1;_IridescenceMask*/0/*end*/;
				surfaceDescription.IridescenceThickness = /*ase_frag_out:IridescenceThickness;Float;25;-1;_IridescenceThickness*/0/*end*/;
				#endif

				float4 bakedGI = /*ase_frag_out:Baked GI;Float4;12;-1;_BakedGI*/float4( 0, 0, 0, 0 )/*end*/;

				packedInput.positionCS.zw = clipPos.zw;
				float3 positionRWS = worldPos;
				float3 normalWS = o.Normal;
				float4 tangentWS = packedInput.interp02.xyzw;

				input.positionSS = packedInput.positionCS;
				input.positionRWS = positionRWS;
#if defined( ASE_SRP_VERSION ) && ASE_SRP_VERSION < 60900
				input.worldToTangent = BuildWorldToTangent( tangentWS, normalWS );
#else
				input.tangentToWorld = BuildTangentToWorld( tangentWS, normalWS );
#endif
				input.texCoord0 = packedInput.interp03.xyzw;
				input.texCoord1 = packedInput.interp04.xyzw;
				input.texCoord2 = packedInput.interp05.xyzw;

				uint2 tileIndex = uint2( input.positionSS.xy ) / GetTileSize();
#if defined( ASE_SRP_VERSION ) && ASE_SRP_VERSION < 60900
				#if defined(UNITY_SINGLE_PASS_STEREO)
					tileIndex.x -= unity_StereoEyeIndex * _NumTileClusteredX;
				#endif
				PositionInputs posInput = GetPositionInput_Stereo( input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS.xyz, tileIndex, unity_StereoEyeIndex );
#else
				PositionInputs posInput = GetPositionInput( input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS.xyz, tileIndex );
#endif
				float3 normalizedWorldViewDir = GetWorldSpaceNormalizeViewDir( input.positionRWS );

				SurfaceData surfaceData;
				BuiltinData builtinData;
				
				GetSurfaceAndBuiltinData( surfaceDescription, input, normalizedWorldViewDir, posInput, surfaceData, builtinData);
				BSDFData bsdfData = ConvertSurfaceDataToBSDFData(input.positionSS.xy, surfaceData);
				PreLightData preLightData = GetPreLightData(normalizedWorldViewDir, posInput, bsdfData);

				#ifdef CUSTOM_BAKED_GI
					half4 decodeInstructions = half4( LIGHTMAP_HDR_MULTIPLIER, LIGHTMAP_HDR_EXPONENT, 0.0h, 0.0h );
					builtinData.bakeDiffuseLighting = UnpackLightmapRGBM( bakedGI, decodeInstructions ) * EMISSIVE_RGBM_SCALE * bsdfData.diffuseColor;
				#endif

				outColor = float4(0.0, 0.0, 0.0, 0.0);
				#ifdef _SURFACE_TYPE_TRANSPARENT
					uint featureFlags = LIGHT_FEATURE_MASK_FLAGS_TRANSPARENT;
				#else
					uint featureFlags = LIGHT_FEATURE_MASK_FLAGS_OPAQUE;
				#endif
				//float3 diffuseLighting;
				//float3 specularLighting;

				//LightLoop(normalizedWorldViewDir, posInput, preLightData, bsdfData, builtinData, featureFlags, diffuseLighting, specularLighting);
				LightLoopOutput lightLoopOutput;
				LightLoop(normalizedWorldViewDir, posInput, preLightData, bsdfData, builtinData, featureFlags, lightLoopOutput);

				// Alias
				float3 diffuseLighting = lightLoopOutput.diffuseLighting;
				float3 specularLighting = lightLoopOutput.specularLighting;

				#ifdef OUTPUT_SPLIT_LIGHTING
					if (_EnableSubsurfaceScattering != 0 && ShouldOutputSplitLighting(bsdfData))
					{
						outColor = float4(specularLighting, 1.0);
						outDiffuseLighting = float4(TagLightingForSSS(diffuseLighting), 1.0);
					}
					else
					{
						outColor = float4(diffuseLighting + specularLighting, 1.0);
						outDiffuseLighting = 0;
					}
					ENCODE_INTO_SSSBUFFER(surfaceData, posInput.positionSS, outSSSBuffer);
				#else
					outColor = ApplyBlendMode(diffuseLighting, specularLighting, builtinData.opacity);
					outColor = EvaluateAtmosphericScattering(posInput, normalizedWorldViewDir, outColor);
				#endif

				outputDepth = posInput.deviceDepth;
			}
            ENDHLSL
        }
		/*ase_pass_end*/
    }
    FallBack "Hidden/InternalErrorShader"
}
