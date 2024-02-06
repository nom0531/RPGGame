Shader "Custom/Character"
{
    Properties
    {
        [MainTexture] _BaseMap("Texture", 2D) = "white" {}
        [MainColor] _BaseColor("Color", Color) = (1, 1, 1, 1)
        _Cutoff("AlphaCutout", Range(0.0, 1.0)) = 0.5

        // ディソルブ表現
        [Toggle(DISSOLVE)]
        _UseDisolve("Use Dissolve", Float) = 0
        [NoScaleOffset] _DissolveTex("DissolveTexture", 2D) = "white" {}
        _EdgeColor("Dissolve Color", Color) = (0, 0, 0, 0)
        [PowerSlider(0.5)]_EdgeWidth("Dissolve Margin Width", Range(0.0, 1.0)) = 0.5
        [PowerSlider(0.0)]_AlphaClipThreshold("Alpha Clip Threshold", Range(0.0, 1.0)) = 0.0

            // BlendMode
            _Surface("__surface", Float) = 0.0
            _Blend("__mode", Float) = 0.0
            _Cull("__cull", Float) = 2.0
            [ToggleUI] _AlphaClip("__clip", Float) = 0.0
            [HideInInspector] _BlendOp("__blendop", Float) = 0.0
            [HideInInspector] _SrcBlend("__src", Float) = 1.0
            [HideInInspector] _DstBlend("__dst", Float) = 0.0
            [HideInInspector] _ZWrite("__zw", Float) = 1.0

            // Editmode props
            _QueueOffset("Queue offset", Float) = 0.0

            // ObsoleteProperties
            [HideInInspector] _MainTex("BaseMap", 2D) = "white" {}
            [HideInInspector] _Color("Base Color", Color) = (0.5, 0.5, 0.5, 1)
            [HideInInspector] _SampleGI("SampleGI", float) = 0.0 // needed from bakedlit
    }

        SubShader
            {
                Tags {
                    "RenderType" = "Opaque"
                    "IgnoreProjector" = "True"
                    "RenderPipeline" = "UniversalPipeline"
                    "ShaderModel" = "4.5"
                }
                LOD 100

                Blend[_SrcBlend][_DstBlend]
                ZWrite[_ZWrite]
                Cull[_Cull]

                Pass
                {
                    Name "Unlit"

                    HLSLPROGRAM
                    #pragma exclude_renderers gles gles3 glcore
                    #pragma target 4.5

                    #pragma shader_feature_local_fragment _SURFACE_TYPE_TRANSPARENT
                    #pragma shader_feature_local_fragment _ALPHATEST_ON
                    #pragma shader_feature_local_fragment _ALPHAPREMULTIPLY_ON

                    // -------------------------------------
                    // Unity defined keywords
                    #pragma multi_compile_fog
                    #pragma multi_compile_instancing
                    #pragma multi_compile _ DOTS_INSTANCING_ON
                    #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
                    #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
                    #pragma multi_compile _ DEBUG_DISPLAY

                    #pragma vertex UnlitPassVertex
                    #pragma fragment UnlitPassFragment

                    #pragma shader_feature DISSOLVE

                    struct appData
                    {
                        float4 vartex : POSITION;
                        float2 uv : TEXCOORD0;
                    };

                    struct v2f
                    {
                        float2 uv : TEXCOORD0;
                        float4 vartex : SV_POSITION;
                    };

                    sampler2D _DissolveTex;
                    half4 _EdgeColor;
                    half _AlphaClopThreshold;
                    half _EdgeWidth;
                    float4 _MainTex_ST;
                    float4 _DissolveTex_ST;

                    #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitInput.hlsl"
                    #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitForwardPass.hlsl"

                    #ifdef DISSOLVE
                    // ディソルブ表現
                    v2f vart(appdata input) 
                    {
                        v2f output;
                        output.vartex = UnityObjectToClipPos(input.vartex);
                        output.uv = TANCEFORM_TEX(input.uv, _MainTex);
                        return output;
                    }

                    half4 frag(v2f input) : SV_Target
                    {
                        half4 edgeColor = half4(1.0f, 1.0f, 1.0f, 1.0f);
                        // テクスチャからα値を取得
                        half4 dissolve = tex2D(_DissolveTex, input.uv);
                        float alpha = dissolve.r * 0.2f + dissolve.g * 0.7f + dissolve.b * 0.1f;

                        // 段階的な色変化で実現する
                        if (alpha < _AlphaClipThreshold + _EdgeWidth && _AlphaClipThreshold > 0) 
                        {
                            edgeColor = _EdgeColor;
                        }
                        if (alpha < _AlphaClipThreshold) 
                        {
                            discard;
                        }
                        return tex2D(_BaseMap, input.uv) * _BaseColor * edgeColor;
                    }
                    #endif
                ENDHLSL
                }
                Pass
                {
                    Name "ShadowCaster"
                    Tags{"LightMode" = "ShadowCaster"}

                    ZWrite On
                    ZTest LEqual
                    ColorMask 0
                    Cull[_Cull]

                    HLSLPROGRAM
                    #pragma exclude_renderers gles gles3 glcore
                    #pragma target 4.5

                    // -------------------------------------
                    // Material Keywords
                    #pragma shader_feature_local_fragment _ALPHATEST_ON
                    #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

                    //--------------------------------------
                    // GPU Instancing
                    #pragma multi_compile_instancing
                    #pragma multi_compile _ DOTS_INSTANCING_ON

                    // -------------------------------------
                    // Universal Pipeline keywords

                    // This is used during shadow map generation to differentiate between directional and punctual light shadows, as they use different formulas to apply Normal Bias
                    #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

                    #pragma vertex ShadowPassVertex
                    #pragma fragment ShadowPassFragment

                    #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
                    #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
                    //#include "Assets/HLSL/ShadowCasterPass.hlsl"
                    ENDHLSL
                }
                Pass
                {
                    Name "DepthOnly"
                    Tags{"LightMode" = "DepthOnly"}

                    ZWrite On
                    ColorMask 0

                    HLSLPROGRAM
                    #pragma exclude_renderers gles gles3 glcore
                    #pragma target 4.5

                    #pragma vertex DepthOnlyVertex
                    #pragma fragment DepthOnlyFragment

                    // -------------------------------------
                    // Material Keywords
                    #pragma shader_feature_local_fragment _ALPHATEST_ON

                    //--------------------------------------
                    // GPU Instancing
                    #pragma multi_compile_instancing
                    #pragma multi_compile _ DOTS_INSTANCING_ON

                    #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitInput.hlsl"
                    #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
                    ENDHLSL
                }

                Pass
                {
                    Name "DepthNormalsOnly"
                    Tags{"LightMode" = "DepthNormalsOnly"}

                    ZWrite On

                    HLSLPROGRAM
                    #pragma exclude_renderers gles gles3 glcore
                    #pragma target 4.5

                    #pragma vertex DepthNormalsVertex
                    #pragma fragment DepthNormalsFragment

                    // -------------------------------------
                    // Material Keywords
                    #pragma shader_feature_local_fragment _ALPHATEST_ON

                    // -------------------------------------
                    // Unity defined keywords
                    #pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT // forward-only variant

                    //--------------------------------------
                    // GPU Instancing
                    #pragma multi_compile_instancing
                    #pragma multi_compile _ DOTS_INSTANCING_ON

                    #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitInput.hlsl"
                    #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitDepthNormalsPass.hlsl"
                    ENDHLSL
                }

                // This pass it not used during regular rendering, only for lightmap baking.
                Pass
                {
                    Name "Meta"
                    Tags{"LightMode" = "Meta"}

                    Cull Off

                    HLSLPROGRAM
                    #pragma exclude_renderers gles gles3 glcore
                    #pragma target 4.5

                    #pragma vertex UniversalVertexMeta
                    #pragma fragment UniversalFragmentMetaUnlit
                    #pragma shader_feature EDITOR_VISUALIZATION

                    #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitInput.hlsl"
                    #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitMetaPass.hlsl"
                    ENDHLSL
                }
            }

                SubShader
            {
                Tags {"RenderType" = "Opaque" "IgnoreProjector" = "True" "RenderPipeline" = "UniversalPipeline" "ShaderModel" = "2.0"}
                LOD 100

                Blend[_SrcBlend][_DstBlend]
                ZWrite[_ZWrite]
                Cull[_Cull]

                Pass
                {
                    Name "Unlit"

                    HLSLPROGRAM
                    #pragma only_renderers gles gles3 glcore d3d11
                    #pragma target 2.0

                    #pragma shader_feature_local_fragment _SURFACE_TYPE_TRANSPARENT
                    #pragma shader_feature_local_fragment _ALPHATEST_ON
                    #pragma shader_feature_local_fragment _ALPHAPREMULTIPLY_ON

                // -------------------------------------
                // Unity defined keywords
                #pragma multi_compile_fog
                #pragma multi_compile_instancing
                #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
                #pragma multi_compile _ DEBUG_DISPLAY

                #pragma vertex UnlitPassVertex
                #pragma fragment UnlitPassFragment

                #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitInput.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitForwardPass.hlsl"
                ENDHLSL
            }

            Pass
            {
                Name "DepthOnly"
                Tags{"LightMode" = "DepthOnly"}

                ZWrite On
                ColorMask 0

                HLSLPROGRAM
                #pragma only_renderers gles gles3 glcore d3d11
                #pragma target 2.0

                #pragma vertex DepthOnlyVertex
                #pragma fragment DepthOnlyFragment

                // -------------------------------------
                // Material Keywords
                #pragma shader_feature_local_fragment _ALPHATEST_ON

                //--------------------------------------
                // GPU Instancing
                #pragma multi_compile_instancing

                #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitInput.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
                ENDHLSL
            }

            Pass
            {
                Name "DepthNormalsOnly"
                Tags{"LightMode" = "DepthNormalsOnly"}

                ZWrite On

                HLSLPROGRAM
                #pragma only_renderers gles gles3 glcore
                #pragma target 2.0

                #pragma vertex DepthNormalsVertex
                #pragma fragment DepthNormalsFragment

                // -------------------------------------
                // Material Keywords
                #pragma shader_feature_local_fragment _ALPHATEST_ON

                // -------------------------------------
                // Unity defined keywords
                #pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT // forward-only variant

                //--------------------------------------
                // GPU Instancing
                #pragma multi_compile_instancing
                #pragma multi_compile _ DOTS_INSTANCING_ON

                #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitInput.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitDepthNormalsPass.hlsl"
                ENDHLSL
            }

                // This pass it not used during regular rendering, only for lightmap baking.
                Pass
                {
                    Name "Meta"
                    Tags{"LightMode" = "Meta"}

                    Cull Off

                    HLSLPROGRAM
                    #pragma only_renderers gles gles3 glcore d3d11
                    #pragma target 2.0

                    #pragma vertex UniversalVertexMeta
                    #pragma fragment UniversalFragmentMetaUnlit
                    #pragma shader_feature EDITOR_VISUALIZATION

                    #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitInput.hlsl"
                    #include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitMetaPass.hlsl"

                    ENDHLSL
                }
            }
                FallBack "Hidden/Universal Render Pipeline/FallbackError"
                CustomEditor "UnityEditor.Rendering.Universal.ShaderGUI.UnlitShader"
}
