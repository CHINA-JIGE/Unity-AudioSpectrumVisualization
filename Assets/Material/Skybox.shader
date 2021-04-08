Shader "Unlit/Skybox"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "black" {}
        _Tint("Tint", Vector) = (0.2, 0.8, 1.0, 1.0)
        _Intensity("Intensity", Float)=0.3
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 posW : POSITION1;
                float4 vertex : SV_POSITION;
            };

            #include "PerlinWorleyNoiseGenerator3D.cginc"
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float3 _Tint;
            float _Intensity;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.posW = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                float timer = 0.02f * _Time.x;
                float3 offset = float3(50.0f * sin(timer), 0.0f, 0.0f);

                float perlinNoise1 = PerlinNoise3D_FBM6(123, offset + normalize(i.posW), float2(0.5f, 0.5f));

                float value1 = 1.0f - pow(abs(perlinNoise1), 0.1f);
                fixed4 col = _Intensity * fixed4(_Tint.xyz, 1.0f) * fixed4(value1, value1, value1, 1.0f);
                return col;
            }
            ENDCG
        }
    }
}
