Shader "Unlit/OutlineShader"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineSize ("Outline Size", Range(0, 8)) = 2
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            fixed4 _Color;
            fixed4 _OutlineColor;
            float _OutlineSize;

            v2f vert(appdata_t v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.texcoord) * i.color;
                float2 offset = _MainTex_TexelSize.xy * _OutlineSize;
                
                float totalAlpha = 0;
                totalAlpha += tex2D(_MainTex, i.texcoord + float2(offset.x, 0)).a;
                totalAlpha += tex2D(_MainTex, i.texcoord - float2(offset.x, 0)).a;
                totalAlpha += tex2D(_MainTex, i.texcoord + float2(0, offset.y)).a;
                totalAlpha += tex2D(_MainTex, i.texcoord - float2(0, offset.y)).a;
                totalAlpha += tex2D(_MainTex, i.texcoord + float2(offset.x, offset.y) * 0.7).a;
                totalAlpha += tex2D(_MainTex, i.texcoord - float2(offset.x, offset.y) * 0.7).a;
                totalAlpha += tex2D(_MainTex, i.texcoord + float2(-offset.x, offset.y) * 0.7).a;
                totalAlpha += tex2D(_MainTex, i.texcoord + float2(offset.x, -offset.y) * 0.7).a;

                float outlineAlpha = saturate(totalAlpha) * _OutlineColor.a;
                
                fixed4 finalColor = lerp(_OutlineColor, col, col.a);
                finalColor.a = max(col.a, outlineAlpha);
                
                return finalColor;
            }
            ENDCG
        }
    }
}