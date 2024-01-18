Shader "PlaneReflection/Caster"
{
        Properties {
        _MainTex ("MainTex", 2D) = "white" {}
    }
    
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100
        
        Pass
        {
            Tags { "LightMode" = "ReflectionCaster"}
            
            // ステンシルバッファの設定
            Stencil{
                // ステンシルの番号
                Ref 1
                // Equal: ステンシルバッファの値がRefと同じであれば描画を行う
                Comp Equal
            }
            
            Cull Front
            
            CGPROGRAM
            #pragma multi_compile _ TK_DEFERRED_PASS
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            
            struct VSInput
            {
                float4 pos : POSITION;      // 頂点座標
                float2 uv : TEXCOORD0;      // UV座標
                float3 normal : NORMAL;     // 法線
                float3 tangent : TANGENT;   // 節ベクトル
            };

            struct VSOutput
            {
                float4 pos : SV_POSITION;   // 座標
                float2 uv : TEXCOORD0;      // UV座標
                float3 normal : NORMAL;     // 法線
                float3 tangent : TANGENT;   // 節ベクトル
                float biNormal : BINORMAL; // 従ベクトル
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            VSOutput vert( VSInput vsIn )
            {
                VSOutput vsOut = (VSOutput)0;
                vsOut.pos = UnityObjectToClipPos(vsIn.pos);
                vsOut.uv = vsIn.uv;
                vsOut.normal = mul(unity_ObjectToWorld, vsIn.normal);
                vsOut.tangent = mul(unity_ObjectToWorld, vsIn.tangent);;
                return vsOut;
            }
            
            float4 frag(VSOutput vsOut) : SV_Target
            {
                float4 col = tex2D(_MainTex, vsOut.uv);
                return col;
            }
            ENDCG
        }
    }
}
