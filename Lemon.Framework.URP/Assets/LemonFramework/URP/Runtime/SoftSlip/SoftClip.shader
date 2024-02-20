Shader "Custom/SoftClipShader"
{
    Properties
    {
        //_Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
        // 裁切边界（左，下，右，上）
        _ClipRect ("Clip Rect", Vector) = (0,0,1,1)
        _Softness ("Softness", Range(0.0,0.5)) = 0.01
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
        }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            // x, y, width, height
            float4 _ClipRect;
            float _Softness;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Convert UVs to a 0-1 space based on the _ClipRect
                float2 uv = (i.uv - _ClipRect.xy) / _ClipRect.zw;
                // Calculate softness based on distance to the edge
                float4 edgeDist = float4(uv.x, 1.0-uv.x, uv.y, 1.0-uv.y);
                float alpha = saturate(min(min(edgeDist.x, edgeDist.y), min(edgeDist.z, edgeDist.w))/_Softness);
                fixed4 col = tex2D(_MainTex, i.uv);
                col.a *= alpha;
                return col;
            }
            ENDCG
        }
    }
}