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

            CGPROGRAM
            #pragma surface surf Lambert vertex:vert

            sampler2D _MainTex;
            float _Speed;
            float _Amplitude;

            struct Input
            {
                float2 uv_MainTex;
            };

            void vert(inout appdata_full v)
            {
                float offset = sin(_Time.y * _Speed) * _Amplitude;
                v.vertex.y += offset;
            }

            void surf(Input IN, inout SurfaceOutput o)
            {
                half4 c = tex2D(_MainTex, IN.uv_MainTex);
                o.Albedo = c.rgb;
                o.Alpha = c.a;
            }
            ENDCG
        }
            FallBack "Diffuse"
}
