Shader "Custom/HelloNeighborStyle"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _ShadowColor ("Shadow Color", Color) = (0.5,0.3,0.2,1)
        _Steps ("Shading Steps", Range(2,4)) = 3
        _Smoothness ("Edge Smoothness", Range(0,0.1)) = 0.03
        _WarmthIntensity ("Warmth", Range(0,1)) = 0.4
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : NORMAL;
                float3 viewDir : TEXCOORD1;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _ShadowColor;
            float _Steps;
            float _Smoothness;
            float _WarmthIntensity;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = WorldSpaceViewDir(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 normal = normalize(i.worldNormal);
                float3 viewDir = normalize(i.viewDir);
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                
                // Improved lighting calculation
                float NdotL = dot(normal, lightDir);
                float halfLambert = NdotL * 0.5 + 0.5; // Softer lighting transition
                
                // View-dependent shading
                float fresnel = pow(1.0 - saturate(dot(normal, viewDir)), 2.0);
                
                // Adjusted cel shading
                float cel = smoothstep(-_Smoothness, _Smoothness, halfLambert);
                cel = floor(cel * _Steps) / _Steps;
                
                // Mix in view-dependent lighting
                cel = lerp(cel, halfLambert, fresnel * 0.3);
                
                // Base color with texture
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                
                // Add warmth with view-dependent adjustment
                fixed3 warmTint = fixed3(1.1, 0.9, 0.75);
                float warmthMask = lerp(1, 0.7, fresnel);
                col.rgb *= lerp(1, warmTint, _WarmthIntensity * warmthMask);
                
                // Improved shadow mixing
                fixed4 finalColor = lerp(_ShadowColor, col, cel);
                
                // Enhanced rim light
                float rim = pow(1.0 - saturate(dot(viewDir, normal)), 3.0);
                finalColor.rgb += rim * _WarmthIntensity * 0.2 * col.rgb;
                
                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
