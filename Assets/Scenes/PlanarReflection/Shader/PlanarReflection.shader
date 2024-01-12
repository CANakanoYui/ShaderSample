Shader "PlaneReflection/Reflection"
{
    Properties
    {
        _MainTex ("_MainTex", 2D) = "white" {}
        _ReflectionTex ("_ReflectionTex", 2D) = "black" {}
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque" "Renderpipeline"="UniversalPipeline"
        }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct VSInput
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct VSOutput
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 screenPos : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);
            
            TEXTURE2D(_ReflectionTex);
            SAMPLER(ss_LinearRepeat);

            VSOutput vert(VSInput In)
            {
                VSOutput Out;
                Out.vertex = TransformObjectToHClip(In.pos);
                Out.uv = In.uv;
                Out.screenPos = ComputeScreenPos(In.pos);
                return Out;
            }

            float4 frag(VSOutput In) : SV_Target
            {
                float2 uv = In.vertex.xy / _ScreenParams.xy;
                float4 albedoCol = SAMPLE_TEXTURE2D(_MainTex,ss_LinearRepeat,In.uv);
                float4 reflectionCol = SAMPLE_TEXTURE2D(_ReflectionTex,ss_LinearRepeat, uv * float2(1, -1) + float2(0, 1));
                float smoothness = 0.5f;
                float4 finalColor = lerp(albedoCol,reflectionCol,smoothness);
                return float4(finalColor);
            }
            ENDHLSL
        }
    }
}