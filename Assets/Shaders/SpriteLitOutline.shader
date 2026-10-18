Shader "Alongside You/Sprite-Lit-Outline"
{
    Properties
    {
        [Header(Lighting)]
        _MinBrightness("Min Brightness (0 = off; sprite never goes darker than this fraction of its color)", Range(0, 1)) = 0

        _MainTex("Diffuse", 2D) = "white" {}
        _MaskTex("Mask", 2D) = "white" {}
        _NormalMap("Normal Map", 2D) = "bump" {}

        // Legacy properties. They're here so that materials using this shader can gracefully fallback to the legacy sprite shader.
        [HideInInspector] _Color("Tint", Color) = (1,1,1,1)
        [HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
        [HideInInspector] _AlphaTex("External Alpha", 2D) = "white" {}
        [HideInInspector] _EnableExternalAlpha("Enable External Alpha", Float) = 0

        [Header(Outline)]
        _OutlineColor("Outline Color", Color) = (1, 0.9, 0.2, 1)
        _OutlineThickness("Outline Thickness (sprite pixels)", Range(1, 4)) = 1
        [Toggle] _OutlineInternal("Internal Outline (off = external)", Float) = 0
        _OutlineGlow("Glow (0 = affected by light, 1 = ignores light)", Range(0, 1)) = 0.4
        _OutlineLightFade("Light Fade (0 = off, 1 = outline vanishes where there is no light)", Range(0, 1)) = 1
        _OutlineLightCutoff("Light Cutoff (below this light level the outline is invisible)", Range(0, 1)) = 0.15
        _OutlineLightSoftness("Light Softness (size of the fade transition)", Range(0.01, 1)) = 0.3
        _OutlineRevealRadius("Reveal Radius (distance from the player where the outline starts to show)", Float) = 3
        _OutlineRevealSoftness("Reveal Softness (distance over which it goes from hidden to complete)", Range(0.05, 4)) = 1.5
        [Toggle] _OutlineEnabled("Outline Enabled (controlled by script)", Float) = 0
    }

    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }

        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        half4 _OutlineColor;
        float _OutlineThickness;
        float _OutlineInternal;
        half _OutlineGlow;
        half _OutlineLightFade;
        half _OutlineLightCutoff;
        half _OutlineLightSoftness;
        half _MinBrightness;
        float _OutlineRevealRadius;
        float _OutlineRevealSoftness;
        // Posicao do jogador (xy) e w = 1 se existe. Definido pelo script OutlineRevealSource.
        float4 _OutlineRevealCenter;

        half OutlineLuminance(half3 rgb)
        {
            return dot(rgb, half3(0.2126, 0.7152, 0.0722));
        }
        float _OutlineEnabled;

        // 1 nos pixels que formam o contorno, 0 no resto.
        // Externo: pixel transparente encostado num opaco.
        // Interno: pixel opaco encostado num transparente.
        // Amostra a textura direto (LOD 0), sem depender de _MainTex_TexelSize.
        half OutlineMask(TEXTURE2D_PARAM(tex, samp), float2 uv, half centerAlpha)
        {
            if (_OutlineEnabled < 0.5)
            {
                return 0;
            }

            float texWidth;
            float texHeight;
            tex.GetDimensions(texWidth, texHeight);
            float2 offset = _OutlineThickness / float2(texWidth, texHeight);

            const float2 dirs[8] =
            {
                float2(1, 0), float2(-1, 0), float2(0, 1), float2(0, -1),
                float2(1, 1), float2(1, -1), float2(-1, 1), float2(-1, -1)
            };

            half maxAlpha = 0;
            half minAlpha = 1;

            [unroll]
            for (int k = 0; k < 8; k++)
            {
                half a = SAMPLE_TEXTURE2D_LOD(tex, samp, uv + dirs[k] * offset, 0).a;
                maxAlpha = max(maxAlpha, a);
                minAlpha = min(minAlpha, a);
            }

            half outerEdge = (centerAlpha < 0.5 && maxAlpha >= 0.5) ? 1.0 : 0.0;
            half innerEdge = (centerAlpha >= 0.5 && minAlpha < 0.5) ? 1.0 : 0.0;

            return _OutlineInternal > 0.5 ? innerEdge : outerEdge;
        }
        ENDHLSL

        Pass
        {
            Tags { "LightMode" = "Universal2D" }

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"

            #pragma vertex CombinedShapeLightVertex
            #pragma fragment CombinedShapeLightFragment

			// GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_0 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_1 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_2 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_3 __
            #pragma multi_compile _ DEBUG_DISPLAY

            struct Attributes
            {
                float3 positionOS   : POSITION;
                float4 color        : COLOR;
                float2  uv          : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4  positionCS  : SV_POSITION;
                half4   color       : COLOR;
                float2  uv          : TEXCOORD0;
                half2   lightingUV  : TEXCOORD1;
                float3  positionWS  : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/LightingUtility.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_MaskTex);
            SAMPLER(sampler_MaskTex);
            half4 _MainTex_ST;
            float4 _Color;
            half4 _RendererColor;

            #if USE_SHAPE_LIGHT_TYPE_0
            SHAPE_LIGHT(0)
            #endif

            #if USE_SHAPE_LIGHT_TYPE_1
            SHAPE_LIGHT(1)
            #endif

            #if USE_SHAPE_LIGHT_TYPE_2
            SHAPE_LIGHT(2)
            #endif

            #if USE_SHAPE_LIGHT_TYPE_3
            SHAPE_LIGHT(3)
            #endif

            Varyings CombinedShapeLightVertex(Attributes v)
            {
                Varyings o = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

#ifdef UNITY_INSTANCING_ENABLED
                v.positionOS = UnityFlipSprite(v.positionOS, unity_SpriteFlip);
#endif
                o.positionCS = TransformObjectToHClip(v.positionOS);
                o.positionWS = TransformObjectToWorld(v.positionOS);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.lightingUV = half2(ComputeScreenPos(o.positionCS / o.positionCS.w).xy);

                o.color = v.color * _Color * _RendererColor;
#ifdef UNITY_INSTANCING_ENABLED
                o.color *= unity_SpriteColor;
#endif
                return o;
            }

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/CombinedShapeLightShared.hlsl"

            half4 CombinedShapeLightFragment(Varyings i) : SV_Target
            {
                const half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                half4 main = i.color * tex;

                const half outline = OutlineMask(TEXTURE2D_ARGS(_MainTex, sampler_MainTex), i.uv, tex.a);
                const half4 outlineColor = half4(_OutlineColor.rgb, _OutlineColor.a * i.color.a);
                main = lerp(main, outlineColor, outline);

                const half4 mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, i.uv);
                SurfaceData2D surfaceData;
                InputData2D inputData;

                InitializeSurfaceData(main.rgb, main.a, mask, surfaceData);
                InitializeInputData(i.uv, i.lightingUV, inputData);

                // O contorno passa pelas luzes 2D; _OutlineGlow devolve parte
                // da cor original, para ele nao sumir no escuro.
                half4 lit = CombinedShapeLightShared(surfaceData, inputData);

                // Quanta luz chega neste pixel: razao entre a cor iluminada e a
                // cor original do contorno (o max evita dividir por ~0 se a
                // cor do contorno for muito escura). Por ser por pixel, so a
                // parte do contorno que esta na luz fica visivel.
                const half baseLuminance = max(OutlineLuminance(outlineColor.rgb), 0.1);
                const half lightReceived = saturate(OutlineLuminance(lit.rgb) / baseLuminance);
                const half softness = max(_OutlineLightSoftness, 0.01);
                const half lightVisibility = smoothstep(_OutlineLightCutoff, _OutlineLightCutoff + softness, lightReceived);

                // Brilho minimo: o sprite reage a luz, mas nunca fica mais escuro que
                // esta fracao da propria cor (nao vale para os pixels do contorno).
                lit.rgb = max(lit.rgb, main.rgb * (_MinBrightness * (1.0 - outline)));

                lit.rgb = lerp(lit.rgb, outlineColor.rgb, outline * _OutlineGlow);

                // Revelacao por proximidade: so o contorno mais perto do jogador aparece;
                // conforme ele chega, o contorno vai se completando do lado dele.
                half reveal = 1;
                if (_OutlineRevealCenter.w > 0.5)
                {
                    const float revealDistance = distance(i.positionWS.xy, _OutlineRevealCenter.xy);
                    const float revealSoftness = max(_OutlineRevealSoftness, 0.01);
                    reveal = 1 - smoothstep(_OutlineRevealRadius - revealSoftness, _OutlineRevealRadius, revealDistance);
                }

                lit.a *= lerp(1, lightVisibility, outline * _OutlineLightFade) * lerp(1, reveal, outline);
                return lit;
            }
            ENDHLSL
        }

        Pass
        {
            Tags { "LightMode" = "NormalsRendering"}

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"

            #pragma vertex NormalsRenderingVertex
            #pragma fragment NormalsRenderingFragment

            // GPU Instancing
            #pragma multi_compile_instancing

            struct Attributes
            {
                float3 positionOS   : POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
                float4 tangent      : TANGENT;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4  positionCS      : SV_POSITION;
                half4   color           : COLOR;
                float2  uv              : TEXCOORD0;
                half3   normalWS        : TEXCOORD1;
                half3   tangentWS       : TEXCOORD2;
                half3   bitangentWS     : TEXCOORD3;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);
            half4 _NormalMap_ST;  // Is this the right way to do this?

            Varyings NormalsRenderingVertex(Attributes attributes)
            {
                Varyings o = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(attributes);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

#ifdef UNITY_INSTANCING_ENABLED
                attributes.positionOS = UnityFlipSprite(attributes.positionOS, unity_SpriteFlip);
#endif
                o.positionCS = TransformObjectToHClip(attributes.positionOS);
                o.uv = TRANSFORM_TEX(attributes.uv, _NormalMap);
                o.color = attributes.color;
                o.normalWS = -GetViewForwardDir();
                o.tangentWS = TransformObjectToWorldDir(attributes.tangent.xyz);
                o.bitangentWS = cross(o.normalWS, o.tangentWS) * attributes.tangent.w;
#ifdef UNITY_INSTANCING_ENABLED
                o.color *= unity_SpriteColor;
#endif
                return o;
            }

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/NormalsRenderingShared.hlsl"

            half4 NormalsRenderingFragment(Varyings i) : SV_Target
            {
                const half4 mainTex = i.color * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                const half3 normalTS = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, i.uv));

                return NormalsRenderingShared(mainTex, normalTS, i.tangentWS.xyz, i.bitangentWS.xyz, i.normalWS.xyz);
            }
            ENDHLSL
        }

        Pass
        {
            Tags { "LightMode" = "UniversalForward" "Queue"="Transparent" "RenderType"="Transparent"}

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"

            #pragma vertex UnlitVertex
            #pragma fragment UnlitFragment

            // GPU Instancing
            #pragma multi_compile_instancing

            struct Attributes
            {
                float3 positionOS   : POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4  positionCS      : SV_POSITION;
                float4  color           : COLOR;
                float2  uv              : TEXCOORD0;
                #if defined(DEBUG_DISPLAY)
                float3  positionWS  : TEXCOORD2;
                #endif
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;
            float4 _Color;
            half4 _RendererColor;

            Varyings UnlitVertex(Attributes attributes)
            {
                Varyings o = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(attributes);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

#ifdef UNITY_INSTANCING_ENABLED
                attributes.positionOS = UnityFlipSprite(attributes.positionOS, unity_SpriteFlip);
#endif
                o.positionCS = TransformObjectToHClip(attributes.positionOS);
                #if defined(DEBUG_DISPLAY)
                o.positionWS = TransformObjectToWorld(v.positionOS);
                #endif
                o.uv = TRANSFORM_TEX(attributes.uv, _MainTex);
                o.color = attributes.color * _Color * _RendererColor;
#ifdef UNITY_INSTANCING_ENABLED
                o.color *= unity_SpriteColor;
#endif
                return o;
            }

            float4 UnlitFragment(Varyings i) : SV_Target
            {
                float4 texSample = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                float4 mainTex = i.color * texSample;

                const half outline = OutlineMask(TEXTURE2D_ARGS(_MainTex, sampler_MainTex), i.uv, texSample.a);
                mainTex = lerp(mainTex, float4(_OutlineColor.rgb, _OutlineColor.a * i.color.a), outline);

                #if defined(DEBUG_DISPLAY)
                SurfaceData2D surfaceData;
                InputData2D inputData;
                half4 debugColor = 0;

                InitializeSurfaceData(mainTex.rgb, mainTex.a, surfaceData);
                InitializeInputData(i.uv, inputData);
                SETUP_DEBUG_DATA_2D(inputData, i.positionWS);

                if(CanDebugOverrideOutputColor(surfaceData, inputData, debugColor))
                {
                    return debugColor;
                }
                #endif

                return mainTex;
            }
            ENDHLSL
        }
    }

    Fallback "Sprites/Default"
}
