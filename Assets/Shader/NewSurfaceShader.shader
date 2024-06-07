Shader "Custom/VerticalMovement"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Speed("Speed", Range(0, 10)) = 1.0
        _Amplitude("Amplitude", Range(0, 1)) = 0.1
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 200

            ZWrite On
            ZTest LEqual

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                sampler2D _MainTex;
                float _Speed;
                float _Amplitude;

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

                v2f vert(appdata v)
                {
                    v2f o;
                    float offset = sin(_Time.y * _Speed) * _Amplitude;
                    v.vertex.z += offset;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    return fixed4(1.0, 0.0, 0.0, 1.0);
                }
                ENDCG
            }
        }
            FallBack "Diffuse"
}