Shader "Custom/URP/TransparentMainColor"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MainColor ("Main Color", Color) = (1, 1, 1, 1)
        _Alpha ("Alpha Multiplier", Range(0.0, 1.0)) = 1.0
    }

    SubShader
    {
        // --- Tags URP ---
        Tags
        {
            "RenderType"      = "Transparent"
            "Queue"           = "Transparent"
            "RenderPipeline"  = "UniversalPipeline"
            "IgnoreProjector" = "True"
        }

        Pass
        {
            Name "UnlitTransparentMainColor"

            // Blending alpha standard (src alpha, one minus src alpha)
            Blend SrcAlpha OneMinusSrcAlpha

            // Désactiver l'écriture dans le depth buffer (important pour la transparence)
            ZWrite Off

            // Culling des faces arrière (changer en Off pour double face)
            Cull Back

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag

            // Inclure les headers Unity URP
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // --------------- Structures ---------------

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            // --------------- Déclarations des propriétés ---------------

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            // Constant Buffer (CBUFFER) — requis par le SRP Batcher
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;   // Tiling & Offset de la texture
                half4  _MainColor;    // Couleur principale RGBA
                half   _Alpha;        // Multiplicateur d'opacité global
            CBUFFER_END

            // --------------- Vertex Shader ---------------

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                // Transformation Object → Clip Space via macro URP
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);

                // Appliquer Tiling et Offset aux UVs
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);

                return OUT;
            }

            // --------------- Fragment Shader ---------------

            half4 frag(Varyings IN) : SV_Target
            {
                // Échantillonner la texture
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);

                // Multiplier par la couleur principale
                half4 finalColor = texColor * _MainColor;

                // Appliquer le multiplicateur d'alpha global
                finalColor.a *= _Alpha;

                // Discard les pixels quasi-invisibles (optimisation)
                clip(finalColor.a - 0.001);

                return finalColor;
            }

            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Unlit"
}
