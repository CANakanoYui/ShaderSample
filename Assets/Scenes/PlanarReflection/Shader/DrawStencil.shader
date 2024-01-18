Shader "DrawStencil"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        ZWrite Off

        Pass
        {
            // ステンシルバッファの設定
            Stencil{
                // ステンシルの番号
                Ref 1
                // Always: このシェーダでレンダリングされたピクセルのステンシルバッファを「対象」とするという意味
                Comp Always
                // Replace: 「対象」としたステンシルバッファにRefの値を書き込む、という意味
                Pass Replace
            }
            
            ColorMask 0

            CGPROGRAM
           #pragma vertex vert
           #pragma fragment frag
            
           #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }
            
            float4 frag (v2f i) : SV_Target
            {
                return float4(1, 1, 1, 1);
            }
            ENDCG
        }
    }
}
