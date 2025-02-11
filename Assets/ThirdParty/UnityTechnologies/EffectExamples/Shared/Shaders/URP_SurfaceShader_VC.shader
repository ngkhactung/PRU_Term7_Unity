Shader "Custom/URP_SurfaceShader_VC"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Normal("Normal Map", 2D) = "bump" {}
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 200
        Blend One OneMinusSrcAlpha 

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
                float4 color        : COLOR;
                float3 normalOS     : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float2 uv           : TEXCOORD0;
                float4 color        : COLOR;
                float3 normalWS     : TEXCOORD1;
            };

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            TEXTURE2D(_Normal); SAMPLER(sampler_Normal);
            float4 _Color;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                VertexPositionInputs positionInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionHCS = positionInputs.positionCS;
                OUT.uv = IN.uv;
                OUT.color = IN.color;
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv) * _Color;
                half3 normalMap = UnpackNormal(SAMPLE_TEXTURE2D(_Normal, sampler_Normal, IN.uv));

                Light mainLight = GetMainLight();
                half3 normalWS = normalize(IN.normalWS + normalMap);
                half3 lightDir = normalize(mainLight.direction);
                half diff = saturate(dot(normalWS, lightDir));

                half3 finalColor = texColor.rgb * IN.color.rgb * diff * mainLight.color;
                return half4(finalColor, texColor.a * IN.color.a);
            }
            ENDHLSL
        }
    }
}
