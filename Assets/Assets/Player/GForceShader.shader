Shader "Hidden/Shader/GForceShader"
{
    Properties
    {
        // This property is necessary to make the CommandBuffer.Blit bind the source texture to _MainTex
        _MainTex("Main Texture", 2DArray) = "grey" {}
    }

    HLSLINCLUDE

    #pragma target 4.5
    #pragma only_renderers d3d11 playstation xboxone xboxseries vulkan metal switch

    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/FXAA.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/RTUpscale.hlsl"

    #include "../../Modules/ProceduralGPULandscape/Scripts/Shaders/simplexNoiseGPU.cginc"

    struct Attributes
    {
        uint vertexID : SV_VertexID;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float2 texcoord   : TEXCOORD0;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    Varyings Vert(Attributes input)
    {
        Varyings output;
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
        output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
        output.texcoord = GetFullScreenTriangleTexCoord(input.vertexID);
        return output;
    }

    // List of properties to control your post process effect
    float _Intensity;
    TEXTURE2D_X(_MainTex);

    float4 CustomPostProcess(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

        // Note that if HDUtils.DrawFullScreen is used to render the post process, use ClampAndScaleUVForBilinearPostProcessTexture(input.texcoord.xy) to get the correct UVs

        float3 sourceColor = SAMPLE_TEXTURE2D_X(_MainTex, s_linear_clamp_sampler, input.texcoord).xyz;

        // Apply greyscale effect
        float borderDistance = (pow(sqrt((input.texcoord.x - 0.5) * (input.texcoord.x - 0.5) + (input.texcoord.y - 0.5) * (input.texcoord.y - 0.5)), 4));
        float noise = clamp((snoise(input.texcoord * 4 + float2(_Time.x * 2.8, -_Time.x)) + snoise(input.texcoord * 2.8 + float2(_Time.x, _Time.x) * 3)) * 1, 0, 1);

        float overG = pow(clamp(_Intensity / 10, 0, 1), 0.25);

        float3 color = sourceColor * (1 - (overG * ((overG + borderDistance * (noise + 1)))));

        float underGX = pow(clamp(-_Intensity / 10, 0, 1), 0.25);
        float underGY = pow(clamp(-_Intensity / 10, 0, 1), 0.1);
        float underGZ = pow(clamp(-_Intensity / 10, 0, 1), 0.1);

        float underX = (1 - (underGX * ((underGX + borderDistance * (noise + 1)))));
        float underY = (1 - (underGY * ((underGY + borderDistance * (noise + 1)))));
        float underZ = (1 - (underGZ * ((underGZ + borderDistance * (noise + 1)))));

        color = color * float3(underX, underY, underZ);

        return float4(color, 1);
    }

    ENDHLSL

    SubShader
    {
        Tags{ "RenderPipeline" = "HDRenderPipeline" }
        Pass
        {
            Name "GForceShader"

            ZWrite Off
            ZTest Always
            Blend Off
            Cull Off

            HLSLPROGRAM
                #pragma fragment CustomPostProcess
                #pragma vertex Vert
            ENDHLSL
        }
    }
    Fallback Off
}
