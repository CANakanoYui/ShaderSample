Shader "PlaneReflection/Receiver"
{
SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma multi_compile _ TK_DEFERRED_PASS
            #pragma vertex vert
            #pragma fragment frag
           
            #include "UnityCG.cginc"

            // クリップ空間からUV座標を計算する。
            // プラットフォーム間の際を吸収したUV座標を計算します。
            inline float2 CalcUVCoordFromClip(float4 coordInClipSpace)
            {
                float2 uv = coordInClipSpace.xy / coordInClipSpace.w;
                uv *= float2(0.5f, 0.5f * _ProjectionParams.x);

                uv += 0.5f;

                return uv;
            }

            struct VSInput
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct VSOutput
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 posInProj : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _ReflectionTex;
            
            VSOutput vert( VSInput vsIn )
            {
                VSOutput vsOut = (VSOutput)0;
                vsOut.pos = UnityObjectToClipPos(vsIn.pos);
                vsOut.uv = vsIn.uv;
                vsOut.posInProj = vsOut.pos;
                return vsOut;
            }
            float4 frag(VSOutput vsOut) : SV_Target
            {
                float4 col = tex2D(_MainTex, vsOut.uv);
                float2 uv = CalcUVCoordFromClip(vsOut.posInProj);
                float4 refCol = tex2D(_ReflectionTex, uv);
                float4 finalCol = lerp( col, refCol, 0.1f);
                return finalCol;
            }
            ENDCG
        }
    }
}