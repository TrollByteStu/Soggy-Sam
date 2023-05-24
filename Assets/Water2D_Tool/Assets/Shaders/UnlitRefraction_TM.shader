Shader"Water2D/UnlitRefraction_TM" {
	Properties {
		_BumpMap ("Normals ", 2D) = "bump" {}
		_BumpWaves("Bump waves", Float) = 1.3
		_Distortion("Distortion", Float) = 0.17
		_BumpTiling ("Bump Tiling", Vector) = (0.04 ,0.04, 0.04, 0.08)
		_BumpDirection ("Bump Direction & Speed", Vector) = (1.0 ,30.0, 20.0, -20.0)
		_BaseColor ("Base Color", COLOR)  = ( .17, .25, .2, 0.5)
	}

	Subshader {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline" = "LightweightPipeline" }
             
        Pass 
        {
            Tags { "LightMode"="LightweightForward" }
            Name"Base"
            
            Blend SrcAlpha OneMinusSrcAlpha
            ZTest LEqual
            Cull Off    
            ZWrite Off
            ColorMask RGBA
            
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

            //--------------------------------------
            // Shader Debugging Options
            //#pragma multi_compile WATER_HEIGHTMAP WATER_NORMAL WATER_COLOR       

            #define _MAIN_LIGHT_SHADOWS_CASCADE 1
            #define SHADOWS_SCREEN 0

		    #pragma vertex vert 
		    #pragma fragment frag

            // Lighting include is needed because of GI
            #include "Water2DInclude.cginc"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    
            struct WaterVertexInput
            {
                float4 vertex : POSITION; // vertex positions
                float2 texcoord : TEXCOORD0; // local UVs

	            UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : TEXCOORD0;
                float4 texcoord : TEXCOORD1;
                float4 bumpCoords : TEXCOORD2;
                float4 screenPos : TEXCOORD3;
                float4 grabPassPos : TEXCOORD4;

                float4 clipPos : SV_POSITION;

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };
     
            float4 _MainTex_ST;

            //sampler2D _BumpMap;
            float4 _BaseColor;

            //float _BumpWaves;
            //float _Distortion;
            //float4 _BumpTiling;
            //float4 _BumpDirection;
	

            v2f vert(WaterVertexInput v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                half3 worldSpaceVertex = mul(unity_ObjectToWorld, (v.vertex)).xyz;
                half2 tileableUv = worldSpaceVertex.xz;
	
                o.bumpCoords.xyzw = (tileableUv.xyxy + _Time.xxxx * _BumpDirection.xyzw) * _BumpTiling.xyzw;

                o.pos.xyz = TransformObjectToWorld(v.vertex.xyz);
                o.texcoord.xy = v.texcoord;
                o.texcoord.zw = o.pos.xz;

                float4 screenUV = ComputeScreenPos(TransformWorldToHClip(o.pos.xyz));
                o.screenPos = screenUV;

                o.clipPos = TransformWorldToHClip(o.pos.xyz);
                o.pos = TransformWorldToHClip(o.pos.xyz);
                o.grabPassPos = ComputeGrabScreenPos(o.pos);
                
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                //UNITY_SETUP_INSTANCE_ID(i);
			      
                half3 bump = (UnpackNormal(tex2D(_BumpMap, i.bumpCoords.xy)) + UnpackNormal(tex2D(_BumpMap, i.bumpCoords.zw))) * 0.5;
                half3 worldN = half3(0, 1, 0) + bump.xxy * _BumpWaves * half3(1, 0, 1);
                half3 worldNormal = normalize(worldN);

                half4 distortOffset = half4(worldNormal.xz * _Distortion * 10.0, 0, 0);
                float4 grabPassPos = float4(i.grabPassPos.xy / i.grabPassPos.w, 0, 0);
                half4 grabWithOffset = grabPassPos + distortOffset;
                float4 rtRefractions = SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture_linear_clamp, grabWithOffset.xy);
		
                half4 baseColor = _BaseColor;

                baseColor = lerp(lerp(rtRefractions, baseColor, baseColor.a), half4(1.0, 1.0, 1.0, 1.0), half(0.0));
                baseColor.a = 1;
                    
                return baseColor;
            }
		    ENDHLSL
        }
    }
	Fallback "Transparent/Diffuse"
}
