Shader "Water2D/UnlitWaterRippleBasic_TM" {
    Properties {
		[HideInInspector] _MainTex ("Base (RGB)", 2D) = "grey" {}
        _DeepWaterColor ("Deep Water Color", Color) = (0.73, 0.92, 0.99, 1)
        _CubeMapLevel ("Cube Map Reflection", Range (0.0, 1.0)) = 0.2
        [KeywordEnum(PerPixel, PerVertex)] 
		_Normals("Normals", Float)  = 0
        _NormalStrength ("Normal Strength", Float) = 15
        _Cube ("Cubemap", CUBE) = "black" {}

        [HideInInspector] _WaveHeightScale ("Wave Height Scale", Float) = 1.0
        [HideInInspector] _FaceCulling ("", Float) = 2
        [HideInInspector] _HeightOffset (" ", Float) = 0.0
        [HideInInspector] _ApplyOffset (" ", Float) = 0.0
        [HideInInspector] _BottomPos (" ", Float) = -2
	}

    Subshader 
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline" = "LightweightPipeline" }
        
        Cull [_FaceCulling]
        ZTest LEqual
        Blend SrcAlpha OneMinusSrcAlpha

        Pass 
        {
            Tags { "LightMode"="LightweightForward" }
            Name"Base"

		    HLSLPROGRAM
			#pragma prefer_hlslcc gles
			#pragma multi_compile _ USE_STRUCTURED_BUFFER
			
			// -------------------------------------
            // Lightweight Pipeline keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT

			//--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile_fog

            #define _MAIN_LIGHT_SHADOWS_CASCADE 1
            #define SHADOWS_SCREEN 0

            #pragma shader_feature _NORMALS_PERPIXEL _NORMALS_PERVERTEX

		    #pragma vertex vert 
		    #pragma fragment frag

            // Lighting include is needed because of GI
            #include "Water2DInclude.cginc"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
  
            struct WaterVertexInput
            {
                float4 vertex   : POSITION; // vertex positions
                float2 texcoord : TEXCOORD0; // local UVs

	            UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos              : TEXCOORD0;
                float2 uv               : TEXCOORD1;
                float3 viewInterpolator : TEXCOORD3;
                
                #if _NORMALS_PERVERTEX
                    float3 vertNormal   : TEXCOORD3;
                #endif

                float4 clipPos          : SV_POSITION;

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };
              
            float4 _MainTex_ST;
       
            v2f vert(WaterVertexInput v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                float offset = GetAverage(_MainTex, _MainTex_TexelSize, float4(v.texcoord.xy, 0, 0));
                
                float oneOrZero = step(_BottomPos + 0.001, v.vertex.y) * _ApplyOffset;
                v.vertex.y += (offset * _WaveHeightScale * oneOrZero) + (_HeightOffset * oneOrZero);

                #if _NORMALS_PERVERTEX
                    o.vertNormal = NormalFromHeightmap(_MainTex, _MainTex_TexelSize, float4(v.texcoord.xy, 0, 0), _NormalStrength);
                #endif

                half3 worldSpaceVertex = mul(unity_ObjectToWorld, (v.vertex)).xyz;
                o.viewInterpolator.xyz = worldSpaceVertex - _WorldSpaceCameraPos;
                
                o.pos.xyz = TransformObjectToWorld(v.vertex.xyz);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

                o.clipPos = TransformWorldToHClip(o.pos.xyz);
                o.pos = TransformWorldToHClip(o.pos.xyz);

                return o;
            }


            float4 frag(v2f i) : SV_Target
            {
    
                float3 normal = float3(0, 1, 0);
                
                #ifdef _NORMALS_PERPIXEL
                    normal = NormalFromHeightmap(_MainTex, _MainTex_TexelSize, float4(i.uv, 0, 0), _NormalStrength);
                #endif 
    
                #ifdef _NORMALS_PERVERTEX 
                    normal = i.vertNormal;      
                #endif

                half3 viewVector = normalize(i.viewInterpolator.xyz);
                float4x4 modelMatrixInverse = unity_WorldToObject;
                float3 normalDir = normalize(mul(float4(normal, 0.0), modelMatrixInverse).xyz);
                float3 reflectedDir = reflect(viewVector, normalize(normalDir));           

                float4 baseColor = lerp(_DeepWaterColor, texCUBE(_Cube, reflectedDir), _CubeMapLevel);
                baseColor.a = _DeepWaterColor.a;
                       
                return baseColor;
            }
		    ENDHLSL
        }
    }
}
