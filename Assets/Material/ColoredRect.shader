Shader "Unlit/ColoredRect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Tint("Tint", Vector) = (0.2, 0.8, 1.0, 1.0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 posW : POSITION1;
                float3 normalW : NORMAL;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float3 _Tint;
            v2f vert (appdata v)
            {
                v2f o;
                o.posW = mul(unity_ObjectToWorld, v.vertex);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normalW = mul(unity_ObjectToWorld, float4(v.normal, 0.0f));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                float mask = pow(2.0f * abs(i.uv.x - 0.5f), 5) + pow(2.0f * abs(i.uv.y - 0.5f), 5);
                fixed4 middleColor = fixed4(0.5f, 0.5f, 0.5f, 1.0f);
                fixed4 colorAnim = 0.5f * fixed4(sin(i.posW.x * 0.1f + 0.5f * _Time.y), sin(i.posW.x * 0.1f + 0.5f + 0.8f *_Time.y), sin(i.posW.x * 0.1f + 1.0f+ 1.5f *_Time.y) , 0.0f);
                fixed4 col = fixed4(0,0,0,1);
                col = (2.0f * i.posW.y + 0.1f) * (middleColor + colorAnim);
                col = lerp(col, float4(_Tint.xyz, 1.0f), 0.4f);
                return col;
            }
            ENDCG
        }
    }
}
