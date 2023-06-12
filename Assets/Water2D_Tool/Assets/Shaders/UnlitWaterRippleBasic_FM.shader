Shader "Water2D/UnlitWaterRippleBasic_FM"
{
	Properties
	{
		[HideInInspector] _MainTex ("Texture", 2D) = "grey" {}
		_BaseColor ("Base Color", COLOR)  = ( .17, .25, .2, 0.5)
        _WaterLineColor ("Water Line Color", COLOR)  = ( 1, 1, 1, 1)
        _WaterLineHeight ("Water Line Height", Float) = 0.1

        [HideInInspector] _WaveHeightScale (" ", Float) = 1.0
        [HideInInspector] _WaterHeight (" ", Float) = 10
        [HideInInspector] _HeightOffset (" ", Float) = 0
        [HideInInspector] _ApplyOffset (" ", Float) = 0.0
        [HideInInspector] _BottomPos (" ", Float) = -2
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent+10" "RenderPipeline" = "LightweightPipeline" }
        
        Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
            Tags { "LightMode"="LightweightForward" }
            Name "Base"

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

		    #pragma vertex vert 
		    #pragma fragment frag

            // Lighting include is needed because of GI
            #include "Water2DInclude.cginc"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"


			struct appdata
			{
				float4 vertex   : POSITION;
				float2 uv       : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

			struct v2f
			{
				float2 uv       : TEXCOORD0;
                float4 vertex   : TEXCOORD1;

                float4 clipPos  : SV_POSITION;

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
			};

			float4  _MainTex_ST;
            float4  _BaseColor;
            float   _WaterLineHeight;
			
			v2f vert (appdata v)
			{
				v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				
                float offset = GetAverage(_MainTex, _MainTex_TexelSize, float4(v.uv.xy, 0, 0));
                float oneOrZero = step(_BottomPos + 0.001, v.vertex.y) * _ApplyOffset;
                v.vertex.y += (offset * _WaveHeightScale * oneOrZero) + (_HeightOffset * oneOrZero);

                o.vertex.xyz = TransformObjectToWorld(v.vertex.xyz);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                o.clipPos = TransformWorldToHClip(o.vertex.xyz);
                o.vertex = TransformWorldToHClip(o.vertex.xyz);
				
				return o;
			}
			
			float4 frag (v2f i) : SV_Target
			{
                float4 baseColor = _BaseColor;

                float uvFilter = 1.0 - (1.0 / _WaterHeight) * _WaterLineHeight;
                float oneOrZero = step(uvFilter, i.uv.y);
                baseColor = baseColor * (step((oneOrZero + 1), 1)) + _WaterLineColor * oneOrZero;
               
                return baseColor;
            }
			ENDHLSL
		}
	}
}
