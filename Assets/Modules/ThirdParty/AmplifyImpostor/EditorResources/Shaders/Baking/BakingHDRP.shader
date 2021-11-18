// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Hidden/Baking HDRP"
{
	Properties
    {
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		_BaseColorMap("_BaseColorMap", 2D) = "white" {}
		_EmissiveColorMap("_EmissiveColorMap", 2D) = "white" {}
		_SpecularColorMap("_SpecularColorMap", 2D) = "white" {}
		_NormalMap("_NormalMap", 2D) = "bump" {}
		_AlphaCutoff("_AlphaCutoff", Float) = 0.5
		_NormalScale("_NormalScale", Float) = 1
		_EmissiveColor("_EmissiveColor", Color) = (0,0,0,0)
		_BaseColor("_BaseColor", Color) = (1,1,1,1)
		_DetailMap("_DetailMap", 2D) = "white" {}
		_AlbedoAffectEmissive("_AlbedoAffectEmissive", Float) = 0
		_MaskMap("_MaskMap", 2D) = "white" {}
		_DetailNormalScale("_DetailNormalScale", Float) = 1
		_DetailSmoothnessScale("_DetailSmoothnessScale", Float) = 1
		_DetailAlbedoScale("_DetailAlbedoScale", Float) = 1
		_SmoothnessRemapMin("_SmoothnessRemapMin", Float) = 0
		_SmoothnessRemapMax("_SmoothnessRemapMax", Float) = 1
		_AORemapMin("_AORemapMin", Float) = 0
		_SpecularColor("_SpecularColor", Color) = (1,1,1,1)
		_Smoothness("_Smoothness", Float) = 0.5
		_EnergyConservingSpecularColor("_EnergyConservingSpecularColor", Float) = 1
		_Metallic("_Metallic", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

    }

    SubShader
    {
		LOD 0

		

        Tags { "RenderPipeline"="HDRenderPipeline" "RenderType"="Opaque" "Queue"="Geometry" }
        Cull Back
		HLSLINCLUDE
		#pragma target 4.5
		ENDHLSL

		
		Pass
        {
            Name "ForwardOnly"
            Tags { "LightMode"="ForwardOnly" }

			Blend One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA

			Stencil
			{
				Ref 2
				WriteMask 7
				Comp Always
				Pass Replace
				Fail Keep
				ZFail Keep
			}

        
            HLSLPROGRAM
			#define AI_ALPHATEST_ON 1
			#define ASE_SRP_VERSION 100202

			#pragma only_renderers d3d11 ps4 xboxone vulkan metal switch
			#pragma multi_compile_instancing
        
			#pragma vertex Vert
			#pragma fragment Frag
				
			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_VERT_POSITION
			#pragma shader_feature _EMISSIVE_COLOR_MAP
			#pragma shader_feature _ALPHATEST_ON
			#pragma shader_feature _MASKMAP
			#pragma shader_feature _DETAIL_MAP
			#pragma shader_feature _SPECULARCOLORMAP
			#pragma shader_feature _THREAD_MAP
			#pragma shader_feature _MATERIAL_FEATURE_SPECULAR_COLOR

			#ifndef SHADERCONFIG_CS_HLSL
			#define SHADERCONFIG_CS_HLSL
			
			#define PROBEVOLUMESEVALUATIONMODES_DISABLED (0)
			#define PROBEVOLUMESEVALUATIONMODES_LIGHT_LOOP (1)
			#define PROBEVOLUMESEVALUATIONMODES_MATERIAL_PASS (2)

			#define SHADEROPTIONS_CAMERA_RELATIVE_RENDERING 0

			#endif
        
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
        
            #define SHADERPASS SHADERPASS_FORWARD
			#pragma multi_compile USE_FPTL_LIGHTLIST USE_CLUSTERED_LIGHTLIST
			#pragma multi_compile SHADOW_LOW SHADOW_MEDIUM SHADOW_HIGH
			//#define USE_LEGACY_UNITY_MATRIX_VARIABLES

			// newer HDRP versions need legacy matrices to render with command buffers properly???
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariablesMatrixDefsHDCamera.hlsl"
			
			#include "../Runtime/AmplifyImpostorsConfig.cginc"

			#if AI_HDRP_VERSION >= 60900
				float4x4 glstate_matrix_projection;
				float4x4 unity_MatrixV;
				float4x4 unity_MatrixInvV;
				float4x4 unity_MatrixVP;
			#endif
			#undef UNITY_MATRIX_V	
			#define UNITY_MATRIX_V     unity_MatrixV
			#undef UNITY_MATRIX_I_V	
			#define UNITY_MATRIX_I_V   unity_MatrixInvV
			#undef UNITY_MATRIX_P	
			#define UNITY_MATRIX_P     OptimizeProjectionMatrix(glstate_matrix_projection)
			#undef UNITY_MATRIX_VP
			#define UNITY_MATRIX_VP    unity_MatrixVP
        
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/Lighting.hlsl"
        
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/Lit.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"
        
			sampler2D _MaskMap;
			sampler2D _BaseColorMap;
			sampler2D _DetailMap;
			sampler2D _SpecularColorMap;
			sampler2D _NormalMap;
			sampler2D _EmissiveColorMap;
			CBUFFER_START( UnityPerMaterial )
			float4 _BaseColorMap_ST;
			float4 _BaseColor;
			float4 _DetailMap_ST;
			float4 _EmissiveColor;
			float4 _SpecularColor;
			float _Metallic;
			float _AlbedoAffectEmissive;
			float _DetailNormalScale;
			float _NormalScale;
			float _DetailSmoothnessScale;
			float _DetailAlbedoScale;
			float _SmoothnessRemapMax;
			float _SmoothnessRemapMin;
			float _AORemapMin;
			float _Smoothness;
			float _EnergyConservingSpecularColor;
			float _AlphaCutoff;
			CBUFFER_END


			struct GraphVertexInput
			{
				float4 vertex : POSITION;
				float4 normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_tangent : TANGENT;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct GraphVertexOutput
			{
				float4 position : POSITION;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			float3 conservativeSpecular309( float3 baseColor, float3 specColor, float conserve )
			{
				return baseColor.rgb * ( conserve > 0.0 ? ( 1.0 - max( specColor.r, max( specColor.g, specColor.b ) ) ) : 1.0 );
			}
			

			GraphVertexOutput Vert( GraphVertexInput v )
			{
				UNITY_SETUP_INSTANCE_ID( v );
				GraphVertexOutput o;
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				float3 ase_worldTangent = TransformObjectToWorldDir(v.ase_tangent.xyz);
				o.ase_texcoord1.xyz = ase_worldTangent;
				float3 ase_worldNormal = TransformObjectToWorldNormal(v.normal.xyz);
				o.ase_texcoord2.xyz = ase_worldNormal;
				float ase_vertexTangentSign = v.ase_tangent.w * unity_WorldTransformParams.w;
				float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * ase_vertexTangentSign;
				o.ase_texcoord3.xyz = ase_worldBitangent;
				float3 objectToViewPos = TransformWorldToView(TransformObjectToWorld(v.vertex.xyz));
				float eyeDepth = -objectToViewPos.z;
				o.ase_texcoord.z = eyeDepth;
				
				o.ase_texcoord.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord.w = 0;
				o.ase_texcoord1.w = 0;
				o.ase_texcoord2.w = 0;
				o.ase_texcoord3.w = 0;
				v.vertex.xyz +=  float3( 0, 0, 0 ) ;
				o.position = TransformObjectToHClip( v.vertex.xyz );
				return o;
			}

			void Frag( GraphVertexOutput IN ,
				out half4 outGBuffer0 : SV_Target0,
				out half4 outGBuffer1 : SV_Target1,
				out half4 outGBuffer2 : SV_Target2,
				out half4 outGBuffer3 : SV_Target3,
				out half4 outGBuffer4 : SV_Target4,
				out half4 outGBuffer5 : SV_Target5,
				out half4 outGBuffer6 : SV_Target6,
				out half4 outGBuffer7 : SV_Target7,
				out float outDepth : SV_Depth
			)
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				float4 appendResult305 = (float4(_Metallic , 1.0 , 1.0 , _Smoothness));
				float2 uv_BaseColorMap = IN.ase_texcoord.xy * _BaseColorMap_ST.xy + _BaseColorMap_ST.zw;
				float2 baseUvs352 = uv_BaseColorMap;
				float4 tex2DNode241 = tex2D( _MaskMap, baseUvs352 );
				float lerpResult298 = lerp( _AORemapMin , 1.0 , tex2DNode241.g);
				float lerpResult297 = lerp( _SmoothnessRemapMin , _SmoothnessRemapMax , tex2DNode241.a);
				float4 appendResult350 = (float4(tex2DNode241.r , lerpResult298 , tex2DNode241.b , lerpResult297));
				#ifdef _MASKMAP
				float4 staticSwitch240 = appendResult350;
				#else
				float4 staticSwitch240 = appendResult305;
				#endif
				float metallic328 = (staticSwitch240).x;
				float4 temp_output_206_0 = ( _BaseColor * tex2D( _BaseColorMap, baseUvs352 ) );
				float3 simpleAlbedo334 = (temp_output_206_0).rgb;
				float2 uv_DetailMap = IN.ase_texcoord.xy * _DetailMap_ST.xy + _DetailMap_ST.zw;
				float4 tex2DNode246 = tex2D( _DetailMap, uv_DetailMap );
				float temp_output_294_0 = (tex2DNode246.r*2.0 + -1.0);
				float3 _Vector1 = float3(1,1,1);
				float3 temp_cast_0 = (0.0).xxx;
				float3 ifLocalVar278 = 0;
				if( temp_output_294_0 >= 0.0 )
				ifLocalVar278 = _Vector1;
				else
				ifLocalVar278 = temp_cast_0;
				float temp_output_275_0 = saturate( ( abs( temp_output_294_0 ) * _DetailAlbedoScale ) );
				float3 lerpResult279 = lerp( sqrt( simpleAlbedo334 ) , ifLocalVar278 , ( temp_output_275_0 * temp_output_275_0 ));
				float detailMask258 = (staticSwitch240).z;
				float3 lerpResult284 = lerp( simpleAlbedo334 , saturate( ( lerpResult279 * lerpResult279 ) ) , detailMask258);
				#ifdef _DETAIL_MAP
				float3 staticSwitch358 = lerpResult284;
				#else
				float3 staticSwitch358 = simpleAlbedo334;
				#endif
				float3 baseColor309 = staticSwitch358;
				float3 specular345 = (( tex2D( _SpecularColorMap, baseUvs352 ) * _SpecularColor )).rgb;
				float3 specColor309 = specular345;
				float conserve309 = _EnergyConservingSpecularColor;
				float3 localconservativeSpecular309 = conservativeSpecular309( baseColor309 , specColor309 , conserve309 );
				#ifdef _MATERIAL_FEATURE_SPECULAR_COLOR
				float3 staticSwitch316 = localconservativeSpecular309;
				#else
				float3 staticSwitch316 = ( ( 1.0 - metallic328 ) * staticSwitch358 );
				#endif
				float3 albedo325 = staticSwitch316;
				float4 appendResult188 = (float4(albedo325 , 1.0));
				
				float3 temp_cast_1 = (0.04).xxx;
				float3 lerpResult320 = lerp( temp_cast_1 , staticSwitch358 , metallic328);
				float temp_output_243_0 = (staticSwitch240).w;
				float temp_output_293_0 = (tex2DNode246.b*2.0 + -1.0);
				float ifLocalVar269 = 0;
				if( temp_output_293_0 >= 0.0 )
				ifLocalVar269 = 1.0;
				else
				ifLocalVar269 = 0.0;
				float lerpResult267 = lerp( temp_output_243_0 , ifLocalVar269 , saturate( ( abs( temp_output_293_0 ) * _DetailSmoothnessScale ) ));
				float lerpResult261 = lerp( temp_output_243_0 , saturate( lerpResult267 ) , detailMask258);
				#ifdef _DETAIL_MAP
				float staticSwitch351 = lerpResult261;
				#else
				float staticSwitch351 = temp_output_243_0;
				#endif
				float smoothness343 = staticSwitch351;
				float4 appendResult312 = (float4(lerpResult320 , smoothness343));
				float4 appendResult244 = (float4(specular345 , smoothness343));
				#ifdef _MATERIAL_FEATURE_SPECULAR_COLOR
				float4 staticSwitch311 = appendResult244;
				#else
				float4 staticSwitch311 = appendResult312;
				#endif
				
				float3 unpack180 = UnpackNormalScale( tex2D( _NormalMap, baseUvs352 ), _NormalScale );
				unpack180.z = lerp( 1, unpack180.z, saturate(_NormalScale) );
				float3 tex2DNode180 = unpack180;
				float4 appendResult256 = (float4(1.0 , tex2DNode246.g , 0.0 , tex2DNode246.a));
				float3 unpack255 = UnpackNormalScale( appendResult256, _DetailNormalScale );
				unpack255.z = lerp( 1, unpack255.z, saturate(_DetailNormalScale) );
				float3 lerpResult257 = lerp( tex2DNode180 , BlendNormal( tex2DNode180 , unpack255 ) , detailMask258);
				#ifdef _DETAIL_MAP
				float3 staticSwitch251 = lerpResult257;
				#else
				float3 staticSwitch251 = tex2DNode180;
				#endif
				float3 normal341 = staticSwitch251;
				float3 ase_worldTangent = IN.ase_texcoord1.xyz;
				float3 ase_worldNormal = IN.ase_texcoord2.xyz;
				float3 ase_worldBitangent = IN.ase_texcoord3.xyz;
				float3 tanToWorld0 = float3( ase_worldTangent.x, ase_worldBitangent.x, ase_worldNormal.x );
				float3 tanToWorld1 = float3( ase_worldTangent.y, ase_worldBitangent.y, ase_worldNormal.y );
				float3 tanToWorld2 = float3( ase_worldTangent.z, ase_worldBitangent.z, ase_worldNormal.z );
				float3 tanNormal8_g3 = normal341;
				float3 worldNormal8_g3 = float3(dot(tanToWorld0,tanNormal8_g3), dot(tanToWorld1,tanNormal8_g3), dot(tanToWorld2,tanNormal8_g3));
				float eyeDepth = IN.ase_texcoord.z;
				float temp_output_4_0_g3 = ( -1.0 / UNITY_MATRIX_P[2].z );
				float temp_output_7_0_g3 = ( ( eyeDepth + temp_output_4_0_g3 ) / temp_output_4_0_g3 );
				float4 appendResult11_g3 = (float4((worldNormal8_g3*0.5 + 0.5) , temp_output_7_0_g3));
				
				#ifdef _EMISSIVE_COLOR_MAP
				float4 staticSwitch195 = tex2D( _EmissiveColorMap, baseUvs352 );
				#else
				float4 staticSwitch195 = float4( 0,0,0,0 );
				#endif
				float3 temp_output_330_0 = (( staticSwitch195 * _EmissiveColor )).rgb;
				float3 ifLocalVar237 = 0;
				if( _AlbedoAffectEmissive == 1.0 )
				ifLocalVar237 = ( simpleAlbedo334 * temp_output_330_0 );
				else
				ifLocalVar237 = temp_output_330_0;
				float4 appendResult193 = (float4(ifLocalVar237 , (staticSwitch240).y));
				
				float albedoAlpha339 = (temp_output_206_0).a;
				#ifdef _ALPHATEST_ON
				float staticSwitch222 = ( albedoAlpha339 - _AlphaCutoff );
				#else
				float staticSwitch222 = 1.0;
				#endif
				float alphaClip331 = staticSwitch222;
				

				outGBuffer0 = appendResult188;
				outGBuffer1 = staticSwitch311;
				outGBuffer2 = appendResult11_g3;
				outGBuffer3 = appendResult193;
				outGBuffer4 = 0;
				outGBuffer5 = 0;
				outGBuffer6 = 0;
				outGBuffer7 = 0;
				float alpha = alphaClip331;
				#if AI_ALPHATEST_ON
					clip( alpha );
				#endif
				outDepth = IN.position.z;
			}
            ENDHLSL
        }
		

	}
	
	CustomEditor "ASEMaterialInspector"
	
}
/*ASEBEGIN
Version=18804
-2236;79;1160;875;315.8082;3321.652;1.129272;True;False
Node;AmplifyShaderEditor.TextureCoordinatesNode;287;-1696,-1088;Inherit;False;0;179;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;352;-1390.983,-1070.208;Float;False;baseUvs;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;353;-1797.114,-1859.336;Inherit;False;352;baseUvs;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;246;-1696,-480;Inherit;True;Property;_DetailMap;_DetailMap;8;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;207;-1523.042,-2086.425;Float;False;Property;_BaseColor;_BaseColor;7;0;Create;True;0;0;0;False;0;False;1,1,1,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;179;-1603.042,-1910.426;Inherit;True;Property;_BaseColorMap;_BaseColorMap;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;356;-626.8651,968.0226;Inherit;False;352;baseUvs;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;206;-1283.042,-1974.426;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;294;-32,-2144;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;2;False;2;FLOAT;-1;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;274;208,-1920;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;289;-1107.042,-1974.426;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;272;112,-1840;Float;False;Property;_DetailAlbedoScale;_DetailAlbedoScale;13;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;241;-432,912;Inherit;True;Property;_MaskMap;_MaskMap;10;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;296;-192,1328;Float;False;Property;_SmoothnessRemapMax;_SmoothnessRemapMax;15;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;300;-48,1136;Float;False;Constant;_AORemapMax;_AORemapMax;17;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;299;-48,1056;Float;False;Property;_AORemapMin;_AORemapMin;16;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;295;-192,1248;Float;False;Property;_SmoothnessRemapMin;_SmoothnessRemapMin;14;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;334;-867.042,-1974.426;Float;False;simpleAlbedo;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;297;160,1280;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;298;160,1136;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;307;200.7639,798.241;Float;False;Property;_Smoothness;_Smoothness;18;0;Create;True;0;0;0;False;0;False;0.5;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;318;235.4955,710.2387;Float;False;Property;_Metallic;_Metallic;20;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;273;352,-1920;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;288;272,-2080;Float;False;Constant;_Vector1;Vector 1;14;0;Create;True;0;0;0;False;0;False;1,1,1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;276;464,-2016;Float;False;Constant;_Float2;Float 2;13;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;350;464,944;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;305;439.5981,721.3659;Inherit;False;FLOAT4;4;0;FLOAT;1;False;1;FLOAT;1;False;2;FLOAT;1;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;335;432,-2224;Inherit;False;334;simpleAlbedo;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;293;784,1040;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;2;False;2;FLOAT;-1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;275;512,-1920;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;264;1040,1232;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;240;688,752;Float;False;Property;_MASKMAP;_MASKMAP;15;0;Fetch;True;0;0;0;False;0;False;0;0;0;False;_MASKMAP;Toggle;2;Key0;Key1;Fetch;False;True;9;1;FLOAT4;0,0,0,0;False;0;FLOAT4;0,0,0,0;False;2;FLOAT4;0,0,0,0;False;3;FLOAT4;0,0,0,0;False;4;FLOAT4;0,0,0,0;False;5;FLOAT4;0,0,0,0;False;6;FLOAT4;0,0,0,0;False;7;FLOAT4;0,0,0,0;False;8;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;354;-537.6382,-1274.169;Inherit;False;352;baseUvs;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SqrtOpNode;281;688,-2224;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;280;672,-1920;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;278;656,-2144;Inherit;False;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;266;912,1312;Float;False;Property;_DetailSmoothnessScale;_DetailSmoothnessScale;12;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;142;-345.1643,-1342.892;Inherit;True;Property;_SpecularColorMap;_SpecularColorMap;2;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;301;-249.1643,-1150.892;Float;False;Property;_SpecularColor;_SpecularColor;17;0;Create;True;0;0;0;False;0;False;1,1,1,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;279;912,-2176;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;270;1200,1200;Float;False;Constant;_Float0;Float 0;13;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;271;1200,1120;Float;False;Constant;_Float1;Float 1;13;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;263;1296,1280;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;245;976,720;Inherit;False;False;False;True;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;310;976,528;Inherit;False;True;False;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;258;1200,720;Float;False;detailMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;265;1472,1280;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;269;1456,1056;Inherit;False;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;282;1104,-2176;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;243;976,816;Inherit;False;False;False;False;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;302;-9.164213,-1214.892;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;355;-512.0123,-689.4347;Inherit;False;352;baseUvs;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;256;-352,-464;Inherit;False;FLOAT4;4;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;336;1232,-2272;Inherit;False;334;simpleAlbedo;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;283;1264,-2160;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;357;660.5489,184.3424;Inherit;False;352;baseUvs;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ComponentMaskNode;346;134.8361,-1214.892;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;253;-432,-320;Float;False;Property;_DetailNormalScale;_DetailNormalScale;11;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;285;1232,-2064;Inherit;False;258;detailMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;328;1216,528;Float;False;metallic;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;194;-432,-608;Float;False;Property;_NormalScale;_NormalScale;5;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;267;1696,1024;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;345;342.8362,-1214.892;Float;False;specular;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;359;1888,-2608;Inherit;False;328;metallic;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;208;-1105.704,-1865.439;Inherit;False;False;False;False;True;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.UnpackScaleNormalNode;255;-176,-464;Inherit;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.LerpOp;284;1472,-2192;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;286;1872,1120;Inherit;False;258;detailMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;140;912,128;Inherit;True;Property;_EmissiveColorMap;_EmissiveColorMap;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;180;-224,-672;Inherit;True;Property;_NormalMap;_NormalMap;3;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;268;1872,1024;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;308;1785.3,-2025.755;Float;False;Property;_EnergyConservingSpecularColor;_EnergyConservingSpecularColor;19;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;196;1280,224;Float;False;Property;_EmissiveColor;_EmissiveColor;6;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;323;2096,-2608;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;339;-865.7037,-1865.439;Float;False;albedoAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;358;1664,-2272;Float;False;Property;_Keyword7;Keyword 7;7;0;Fetch;False;0;0;0;False;0;False;0;0;0;False;_DETAIL_MAP;Toggle;2;Key0;Key1;Fetch;False;True;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BlendNormalsNode;250;96,-512;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StaticSwitch;195;1216,128;Float;False;Property;_EMISSION;_EMISSIVE_COLOR_MAP;7;0;Fetch;False;0;0;0;False;0;False;0;0;0;False;_EMISSIVE_COLOR_MAP;Toggle;2;Key0;Key1;Fetch;False;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;260;96,-416;Inherit;False;258;detailMask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;348;1882.136,-2118.645;Inherit;False;345;specular;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;261;2144,912;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;324;2288,-2608;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;197;1520,192;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;351;2320,832;Float;False;Property;_Keyword3;Keyword 3;7;0;Fetch;False;0;0;0;False;0;False;0;0;0;False;_DETAIL_MAP;Toggle;2;Key0;Key1;Fetch;False;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CustomExpressionNode;309;2112,-2192;Float;False;return baseColor.rgb * ( conserve > 0.0 ? ( 1.0 - max( specColor.r, max( specColor.g, specColor.b ) ) ) : 1.0 )@;3;False;3;True;baseColor;FLOAT3;0,0,0;In;;Float;False;True;specColor;FLOAT3;0,0,0;In;;Float;False;True;conserve;FLOAT;0;In;;Float;False;conservativeSpecular;True;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;257;352,-576;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;191;160,-2720;Float;False;Property;_AlphaCutoff;_AlphaCutoff;4;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;340;128,-2816;Inherit;False;339;albedoAlpha;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;337;1664,96;Inherit;False;334;simpleAlbedo;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;321;2240,-640;Float;False;Constant;_Float3;Float 3;22;0;Create;True;0;0;0;False;0;False;0.04;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;329;2240,-544;Inherit;False;328;metallic;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;189;352,-2896;Float;False;Constant;_Alpha1;Alpha1;5;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;251;512,-672;Float;False;Property;_Keyword2;_DETAIL_MAP;7;0;Fetch;False;0;0;0;False;0;False;0;0;0;False;_DETAIL_MAP;Toggle;2;Key0;Key1;Fetch;False;True;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;190;352,-2784;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;343;2528,832;Float;False;smoothness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;316;2432,-2608;Float;False;Property;_Keyword5;_MATERIAL_FEATURE_SPECULAR_COLOR;7;0;Fetch;False;0;0;0;False;0;False;0;0;0;False;_MATERIAL_FEATURE_SPECULAR_COLOR;Toggle;2;Key0;Key1;Fetch;False;True;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;330;1664,192;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;320;2464,-640;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;347;2400,-352;Inherit;False;345;specular;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StaticSwitch;222;512,-2848;Float;False;Property;_Keyword1;_ALPHATEST_ON;7;0;Fetch;False;0;0;0;False;0;False;0;0;0;False;_ALPHATEST_ON;Toggle;2;Key0;Key1;Fetch;False;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;239;1888,128;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;344;2400,-432;Inherit;False;343;smoothness;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;341;736,-672;Float;False;normal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;238;1856,48;Float;False;Property;_AlbedoAffectEmissive;_AlbedoAffectEmissive;9;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;325;2752,-2608;Float;False;albedo;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;327;3695.894,-388.7268;Float;False;Constant;_Float4;Float 4;22;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;312;2688,-512;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;342;3414.48,-208.4201;Inherit;False;341;normal;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;331;752,-2848;Float;False;alphaClip;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;237;2176,96;Inherit;False;False;5;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;244;2688,-384;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ComponentMaskNode;242;976,624;Inherit;False;False;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;326;3662.296,-487.3999;Inherit;False;325;albedo;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;193;2416,208;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StaticSwitch;311;2896,-481;Float;False;Property;_MATERIAL_FEATURE_SPECULAR_COLOR;_MATERIAL_FEATURE_SPECULAR_COLOR;15;0;Fetch;True;0;0;0;False;0;False;0;0;0;False;_MATERIAL_FEATURE_SPECULAR_COLOR;Toggle;2;Key0;Key1;Fetch;False;True;9;1;FLOAT4;0,0,0,0;False;0;FLOAT4;0,0,0,0;False;2;FLOAT4;0,0,0,0;False;3;FLOAT4;0,0,0,0;False;4;FLOAT4;0,0,0,0;False;5;FLOAT4;0,0,0,0;False;6;FLOAT4;0,0,0,0;False;7;FLOAT4;0,0,0,0;False;8;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;333;3850.11,-20.57428;Inherit;False;331;alphaClip;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;187;3606.48,-208.4201;Inherit;False;Pack Normal Depth;-1;;3;8e386dbec347c9f44befea8ff816d188;0;1;12;FLOAT3;0,0,0;False;3;FLOAT4;0;FLOAT3;14;FLOAT;15
Node;AmplifyShaderEditor.DynamicAppendNode;188;3878.715,-445.6821;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;362;4086.555,-245.3209;Float;False;True;-1;2;ASEMaterialInspector;0;11;Hidden/Baking HDRP;5b7fbe5f8e132bd40b11a10c99044f79;True;ForwardOnly;0;0;ForwardOnly;10;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;False;False;False;False;False;False;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;0;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;-1;False;False;False;True;True;2;False;-1;255;False;-1;7;False;-1;7;False;-1;3;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=ForwardOnly;False;8;Include;;False;;Native;Pragma;shader_feature _EMISSIVE_COLOR_MAP;False;;Custom;Pragma;shader_feature _ALPHATEST_ON;False;;Custom;Pragma;shader_feature _MASKMAP;False;;Custom;Pragma;shader_feature _DETAIL_MAP;False;;Custom;Pragma;shader_feature _SPECULARCOLORMAP;False;;Custom;Pragma;shader_feature _THREAD_MAP;False;;Custom;Pragma;shader_feature _MATERIAL_FEATURE_SPECULAR_COLOR;False;;Custom;;0;0;Standard;0;0;1;True;False;;False;0
WireConnection;352;0;287;0
WireConnection;179;1;353;0
WireConnection;206;0;207;0
WireConnection;206;1;179;0
WireConnection;294;0;246;1
WireConnection;274;0;294;0
WireConnection;289;0;206;0
WireConnection;241;1;356;0
WireConnection;334;0;289;0
WireConnection;297;0;295;0
WireConnection;297;1;296;0
WireConnection;297;2;241;4
WireConnection;298;0;299;0
WireConnection;298;1;300;0
WireConnection;298;2;241;2
WireConnection;273;0;274;0
WireConnection;273;1;272;0
WireConnection;350;0;241;1
WireConnection;350;1;298;0
WireConnection;350;2;241;3
WireConnection;350;3;297;0
WireConnection;305;0;318;0
WireConnection;305;3;307;0
WireConnection;293;0;246;3
WireConnection;275;0;273;0
WireConnection;264;0;293;0
WireConnection;240;1;305;0
WireConnection;240;0;350;0
WireConnection;281;0;335;0
WireConnection;280;0;275;0
WireConnection;280;1;275;0
WireConnection;278;0;294;0
WireConnection;278;2;288;0
WireConnection;278;3;288;0
WireConnection;278;4;276;0
WireConnection;142;1;354;0
WireConnection;279;0;281;0
WireConnection;279;1;278;0
WireConnection;279;2;280;0
WireConnection;263;0;264;0
WireConnection;263;1;266;0
WireConnection;245;0;240;0
WireConnection;310;0;240;0
WireConnection;258;0;245;0
WireConnection;265;0;263;0
WireConnection;269;0;293;0
WireConnection;269;2;271;0
WireConnection;269;3;271;0
WireConnection;269;4;270;0
WireConnection;282;0;279;0
WireConnection;282;1;279;0
WireConnection;243;0;240;0
WireConnection;302;0;142;0
WireConnection;302;1;301;0
WireConnection;256;1;246;2
WireConnection;256;3;246;4
WireConnection;283;0;282;0
WireConnection;346;0;302;0
WireConnection;328;0;310;0
WireConnection;267;0;243;0
WireConnection;267;1;269;0
WireConnection;267;2;265;0
WireConnection;345;0;346;0
WireConnection;208;0;206;0
WireConnection;255;0;256;0
WireConnection;255;1;253;0
WireConnection;284;0;336;0
WireConnection;284;1;283;0
WireConnection;284;2;285;0
WireConnection;140;1;357;0
WireConnection;180;1;355;0
WireConnection;180;5;194;0
WireConnection;268;0;267;0
WireConnection;323;0;359;0
WireConnection;339;0;208;0
WireConnection;358;1;336;0
WireConnection;358;0;284;0
WireConnection;250;0;180;0
WireConnection;250;1;255;0
WireConnection;195;0;140;0
WireConnection;261;0;243;0
WireConnection;261;1;268;0
WireConnection;261;2;286;0
WireConnection;324;0;323;0
WireConnection;324;1;358;0
WireConnection;197;0;195;0
WireConnection;197;1;196;0
WireConnection;351;1;243;0
WireConnection;351;0;261;0
WireConnection;309;0;358;0
WireConnection;309;1;348;0
WireConnection;309;2;308;0
WireConnection;257;0;180;0
WireConnection;257;1;250;0
WireConnection;257;2;260;0
WireConnection;251;1;180;0
WireConnection;251;0;257;0
WireConnection;190;0;340;0
WireConnection;190;1;191;0
WireConnection;343;0;351;0
WireConnection;316;1;324;0
WireConnection;316;0;309;0
WireConnection;330;0;197;0
WireConnection;320;0;321;0
WireConnection;320;1;358;0
WireConnection;320;2;329;0
WireConnection;222;1;189;0
WireConnection;222;0;190;0
WireConnection;239;0;337;0
WireConnection;239;1;330;0
WireConnection;341;0;251;0
WireConnection;325;0;316;0
WireConnection;312;0;320;0
WireConnection;312;3;344;0
WireConnection;331;0;222;0
WireConnection;237;0;238;0
WireConnection;237;2;330;0
WireConnection;237;3;239;0
WireConnection;237;4;330;0
WireConnection;244;0;347;0
WireConnection;244;3;344;0
WireConnection;242;0;240;0
WireConnection;193;0;237;0
WireConnection;193;3;242;0
WireConnection;311;1;312;0
WireConnection;311;0;244;0
WireConnection;187;12;342;0
WireConnection;188;0;326;0
WireConnection;188;3;327;0
WireConnection;362;0;188;0
WireConnection;362;1;311;0
WireConnection;362;2;187;0
WireConnection;362;3;193;0
WireConnection;362;8;333;0
ASEEND*/
//CHKSM=0FA054C11F020ADA746398F7ADDE5316F484D252