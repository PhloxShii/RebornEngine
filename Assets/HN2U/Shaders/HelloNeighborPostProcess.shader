Shader "Custom/HelloNeighborPostProcess"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Saturation ("Saturation", Range(0, 2)) = 1.2
        _Contrast ("Contrast", Range(0, 2)) = 1.1
        _Warmth ("Warmth", Range(0, 1)) = 0.3
        _VignetteIntensity ("Vignette Intensity", Range(0, 1)) = 0.3
        _VignetteRoundness ("Vignette Roundness", Range(0, 1)) = 0.5
        _ChromaticAberration ("Chromatic Aberration", Range(0, 0.01)) = 0.002
        _BlurAmount ("Blur Amount", Range(0, 0.01)) = 0.001
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

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
            float _Saturation;
            float _Contrast;
            float _Warmth;
            float _VignetteIntensity;
            float _VignetteRoundness;
            float _ChromaticAberration;
            float _BlurAmount;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float3 AdjustSaturation(float3 color, float saturation)
            {
                float grey = dot(color, float3(0.2126, 0.7152, 0.0722));
                return lerp(grey.xxx, color, saturation);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Chromatic aberration
                float2 direction = (i.uv - 0.5) * 2;
                float3 color;
                color.r = tex2D(_MainTex, i.uv - direction * _ChromaticAberration).r;
                color.g = tex2D(_MainTex, i.uv).g;
                color.b = tex2D(_MainTex, i.uv + direction * _ChromaticAberration).b;

                // Simple blur
                float2 blur = _BlurAmount * direction;
                color += tex2D(_MainTex, i.uv + blur).rgb;
                color += tex2D(_MainTex, i.uv - blur).rgb;
                color *= 0.33333;

                // Contrast
                color = (color - 0.5) * _Contrast + 0.5;

                // Saturation
                color = AdjustSaturation(color, _Saturation);

                // Warmth
                float3 warmtint = float3(1.1, 0.9, 0.75);
                color = lerp(color, color * warmtint, _Warmth);

                // Vignette
                float2 uv = i.uv * (1 - i.uv.yx);
                float vig = uv.x * uv.y * 15;
                vig = pow(vig, _VignetteRoundness);
                color *= 1 - (_VignetteIntensity * (1 - vig));

                return float4(color, 1);
            }
            ENDCG
        }
    }
}
