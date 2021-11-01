Shader /*ase_name*/ "Hidden/Impostors/Bake/HighDefinition"/*end*/
{
	Properties
    {
		/*ase_props*/
    }

    SubShader
    {
		/*ase_subshader_options:Name=Additional Options
			Port:ForwardOnly:Clip
				On:SetDefine:AI_ALPHATEST_ON 1
		*/

        Tags{ "RenderPipeline" = "HDRenderPipeline" "RenderType"="Opaque" "Queue"="Geometry"}
        Cull Back
		HLSLINCLUDE
		#pragma target 4.5
		ENDHLSL

		/*ase_pass*/
		Pass
        {
            Name "ForwardOnly"
            Tags { "LightMode" = "ForwardOnly" }

			Blend One Zero
			ZWrite On
			ZTest LEqual
			Offset 0,0
			ColorMask RGBA

			Stencil
			{
			   WriteMask 7
			   Ref  2
			   Comp Always
			   Pass Replace
			}
        
            HLSLPROGRAM
			#pragma only_renderers d3d11 ps4 xboxone vulkan metal switch
			#pragma multi_compile_instancing
        
			#pragma vertex Vert
			#pragma fragment Frag
				
			/*ase_pragma*/
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
        
			/*ase_globals*/

			struct GraphVertexInput
			{
				float4 vertex : POSITION;
				float4 normal : NORMAL;
				/*ase_vdata:p=p;n=n*/
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct GraphVertexOutput
			{
				float4 position : POSITION;
				/*ase_interp(0,):sp=sp*/
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			/*ase_funcs*/

			GraphVertexOutput Vert( GraphVertexInput v /*ase_vert_input*/)
			{
				UNITY_SETUP_INSTANCE_ID( v );
				GraphVertexOutput o;
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				/*ase_vert_code:v=GraphVertexInput;o=GraphVertexOutput*/
				v.vertex.xyz += /*ase_vert_out:Vertex Offset;Float3*/ float3( 0, 0, 0 ) /*end*/;
				o.position = TransformObjectToHClip( v.vertex.xyz );
				return o;
			}

			void Frag( GraphVertexOutput IN /*ase_frag_input*/,
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
				/*ase_frag_code:IN=GraphVertexOutput*/

				outGBuffer0 = /*ase_frag_out:Output RT 0;Float4*/0/*end*/;
				outGBuffer1 = /*ase_frag_out:Output RT 1;Float4*/0/*end*/;
				outGBuffer2 = /*ase_frag_out:Output RT 2;Float4*/0/*end*/;
				outGBuffer3 = /*ase_frag_out:Output RT 3;Float4*/0/*end*/;
				outGBuffer4 = /*ase_frag_out:Output RT 4;Float4*/0/*end*/;
				outGBuffer5 = /*ase_frag_out:Output RT 5;Float4*/0/*end*/;
				outGBuffer6 = /*ase_frag_out:Output RT 6;Float4*/0/*end*/;
				outGBuffer7 = /*ase_frag_out:Output RT 7;Float4*/0/*end*/;
				float alpha = /*ase_frag_out:Clip;Float*/1/*end*/;
				#if AI_ALPHATEST_ON
					clip( alpha );
				#endif
				outDepth = IN.position.z;
			}
            ENDHLSL
        }
		/*ase_pass_end*/

	}
}
