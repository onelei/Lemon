Shader "Custom/MatrixRainShader"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _RainDropTex ("Alpha (A)", 2D) = "white" {}
        _Speed ("Speed", Float) = 0.1
        _Intensity ("Intensity", Float) = 1
        _Color ("Color", Color) = (0,1,0,1)
    }
    SubShader
    {
        Tags 
        { 
            "Queue" = "Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
        }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _RainDropTex;
            float _Speed;
            float _Intensity;
            float4 _Color;

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv; // 直接使用原始uv坐标，不进行额外转换
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                uv.y = uv.y + _Time.y * _Speed;

                // 使用RainDropTex的r值作为alphamap来实现alpha渐变效果
                //float alpha = tex2D(_RainDropTex, uv * _Intensity).r;
                float alpha = uv.y * 0.5 + sin(_Time.y) * 0.5;
                float4 color = tex2D(_MainTex, uv);
                
                // 应用预定的颜色并乘以alpha渐变，保证始终是绿色
                color.rgb = _Color.rgb;
                return float4(color.rgb, alpha);
            }
            ENDCG
        }
    } 
}