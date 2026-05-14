Shader "Custom/S_Waveform"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        _BackgroundColor("Background Color", Color) = (0, 0, 0, 0)
        _Scale("Scale", float) = 1
        _Thickness("Thinkness", float) = 0.02
        _Glow("Glow", float) = 1
    }

    SubShader
    {
        Tags { "RenderType" = "Translucent" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            StructuredBuffer<float> _Buffer;

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                half4 _BackgroundColor;

                float _Scale;
                float _Thickness;
                float _Glow;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                uint index = (uint)(IN.uv.x * 255.0);
                float s = _Buffer[index] * _Scale;
                
                // recenter uv.y
                float y = IN.uv.y * 2.0 - 1.0;
                float height = abs(y - s);
                
                float step = 1.0 - smoothstep(0.0, _Thickness, height);
                float glow = 1.0 - smoothstep(0.0, _Glow, height);
                glow *= glow;
 
                float l = max(step, glow * 0.4);
                return lerp(_BackgroundColor, _BaseColor, l);
            }
            ENDHLSL
        }
    }
}
