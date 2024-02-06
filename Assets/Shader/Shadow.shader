Shader "Custom/Shadow"
{
    Properties
    {
        [Header(Main)][Space]
        // メインのテクスチャ
        [NoScaleOffset]_MainTex("MainTexture", 2D) = "white" {}
        _ShadowColor("Shadow Color", Color) = (0, 0, 0, 0)
        _MainTexScale("MainTexture Tiling", Float) = 1
        [Space]
        _MultiplyStrength("Multiply Strength", Range(0.0, 1.0)) = 1
        [Space]
        //[Toggle(UV_SCROLL)]
        //_UseUVScroll("Use UVScroll", Float) = 0
        [Toggle(DRAW_CIRCLE)]
        _UseDrawCircle("Use DrawCircle", Float) = 0
        [Space(10)]
        // UVスクロール
        [Header(UVScroll)][Space]
        // 影のテクスチャ
        [NoScaleOffset]_ShadowTex("ShadowTex", 2D) = "white" {}
        _ShadowTexScale("ShadowTexture Tiling", Float) = 1
        [Space]
        // 加速値
        _ScrollSpeedX("Scroll Speed X", Range(-1.0, 1.0)) = 0.25
        _ScrollSpeedY("Scroll Speed Y", Range(-1.0, 1.0)) = 0.25
        [Space]
        // 移動方向
        _ScrollShiftX("Scroll Shift X", Range(-1.0, 1.0)) = 0.25
        _ScrollShiftY("Scroll Shift Y", Range(-1.0, 1.0)) = 0.25
        [Space(10)]
        // 円を描画する
        [Header(DrawCircle)][Space]
        _Frequency("Frequency", Float) = 50
        _Width("Width", Range(-1.0, 1.0)) = 0.9
        [Space]
        _CenterPositionX("Center Position X", Range(0.0, 1.0)) = 0.5
        _CenterPositionY("Center Position Y", Range(0.0, 1.0)) = 0.5
    }

    SubShader
    {
        Tags {  "RenderType" = "Opaque"
                "RenderPipeline" = "UniversalPipeline"
                "UniversalMaterialType" = "SimpleLit" 
                "IgnoreProjector" = "True" "ShaderModel" = "4.5"
        }
        LOD 300

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }


            HLSLPROGRAM
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _RECEIVE_SHADOWS_OFF

            // -------------------------------------
            // Universal Pipeline keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            //--------------------------------------

            #pragma vertex LitPassVertexSimple
            #pragma fragment LitPassFragmentSimple

            #pragma shader_feature UV_SCROLL
            #pragma shader_feature DRAW_CIRCLE

            #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS    : POSITION;
                float2 texcoord      : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv                       : TEXCOORD0;
                float3 positionWS               : TEXCOORD2;
                float4 positionCS               : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _ShadowTex;

            half4 _ShadowColor;
            float _ShadowMode;
            float _ScrollSpeedX;
            float _ScrollSpeedY;
            float _ScrollShiftX;
            float _ScrollShiftY;
            float _MainTexScale;
            float _ShadowTexScale;
            float _MultiplyStrength;
            float _ShadowAlpha;
            float _CenterPositionX;
            float _CenterPositionY;
            float _Frequency;
            float _Width;

            Varyings LitPassVertexSimple(Attributes input)
            {
                Varyings output = (Varyings)0;
                // 座標を変換
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionWS.xyz = vertexInput.positionWS;
                output.positionCS = vertexInput.positionCS;
                // テクスチャのタイリング
                output.uv = input.texcoord * _MainTexScale;

                return output;
            }

            Light MyGetMainLight(float4 shadowCoord)
            {
                Light light = GetMainLight();

                half4 shadowParams = GetMainLightShadowParams();
                half shadowStrength = shadowParams.x;
                ShadowSamplingData shadowSamplingData = GetMainLightShadowSamplingData();
                half attenuation;
                attenuation = SAMPLE_TEXTURE2D_SHADOW(_MainLightShadowmapTexture, sampler_MainLightShadowmapTexture, shadowCoord.xyz);
                attenuation = SampleShadowmapFiltered(TEXTURE2D_SHADOW_ARGS(_MainLightShadowmapTexture,
                    sampler_MainLightShadowmapTexture), shadowCoord, shadowSamplingData);
                attenuation = LerpWhiteTo(attenuation, shadowStrength);
                half shadowAttenuation = BEYOND_SHADOW_FAR(shadowCoord) ? 1.0 : attenuation;
                light.shadowAttenuation = shadowAttenuation;
                return light;
            }

            // UVスクロール
            half4 UVScroll(float2 uv)
            {
                float2 uvShadow;
                uvShadow.x = uv.x + (_ScrollSpeedX * _ScrollShiftX * _Time.x);
                uvShadow.y = uv.y + (_ScrollSpeedY * _ScrollShiftY * _Time.y);
                uvShadow *= _ShadowTexScale;
                // テクスチャをサンプリング
                return tex2D(_ShadowTex, uvShadow);
            }

            // 円を描画する
            half4 DrawCircle(float2 uv)
            {
                half len = distance(uv, half2(_CenterPositionX, _CenterPositionY)) - _Time;
                half circle = step(_Width, sin(len * _Frequency));
                return mul(circle, _ShadowColor);
            }

            // 影を描画する
            half4 LitPassFragmentSimple(Varyings input) : SV_Target
            {
                half4 positon = (0, 0, 0, 0);
                half4 shadowCoord = TransformWorldToShadowCoord(input.positionWS);
                Light mainLight = MyGetMainLight(shadowCoord);
                // テクスチャをサンプリング
                half4 mainTexColor = tex2D(_MainTex, input.uv);
                half4 shadowColor;
                #ifdef UV_SCROLL
                shadowColor = UVScroll(input.uv);
                #elif DRAW_CIRCLE
                shadowColor = DrawCircle(input.uv);
                #else
                shadowColor = _ShadowColor;
                #endif
                // 最終カラーを計算
                shadowColor *= 1.0f - (1.0f - mainTexColor) * _MultiplyStrength;
                half3 finalColor = lerp(shadowColor.rgb, mainTexColor.rgb, mainLight.shadowAttenuation);
                return half4(finalColor, 1.0f);
            }
            ENDHLSL
        }
    }
}