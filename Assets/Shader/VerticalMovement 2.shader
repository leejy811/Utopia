Shader "Custom/VerticalMovement"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Speed("Speed", Range(0, 10)) = 1.0
        _Amplitude("Amplitude", Range(0, 1)) = 0.1
        _Color("Color", Color) = (1,1,1,1)
        _LineColor("Color", Color) = (1,1,1,1)
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 200

            ZWrite On
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha 

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                sampler2D _MainTex;
                float _Speed;
                float _Amplitude;
                fixed4 _LineColor;

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                    float offset : TEXCOORD1;
                };

                v2f vert(appdata v)
                {
                    v2f o;
                    float period = 3.14159265359;
                    float t = _Time.y * _Speed;

                    float t_mod = frac((t + period / 3) / period) * period;
                    float offset = frac(t_mod / _Amplitude) * _Amplitude;

                    v.vertex.z += offset;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    o.offset = offset;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    float alpha = 1.0 - clamp(i.offset / _Amplitude, 0.0, 1.0);

                    return fixed4(_LineColor.rgb, alpha);
                }
                ENDCG
            }
        }
            FallBack "Diffuse"
}
