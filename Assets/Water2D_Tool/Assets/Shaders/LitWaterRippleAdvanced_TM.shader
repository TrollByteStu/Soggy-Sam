Shader"Water2D/LitWaterRippleAdvanced_TM" {
    Properties {
		[HideInInspector] _MainTex ("Base (RGB)", 2D) = "grey" {}
 
        _DeepWaterColor ("Deep Water Color", Color) = (0.588, 0.745, 1.0, 1)
        _ShallowWaterColor ("Shallow Water Color", Color) = (0.874, 0.965, 1.0, 1)
        _UnderWaterOpacity ("Under Water Opacity", Range (0.0, 1.0)) = 0.4
        _CubeMapLevel ("Cube Map Reflection", Range (0.0, 1.0)) = 0.29    
        _EdgeBlend ("Edge Blend", Float) = 4.0
        _WaterDepth ("Water Depth", Float) = 0.15

        _Smoothness ("Reflection Intensity", Range(0.0, 1.0)) = 0.8
        _FresnelPower  ("Fresnel Power", Float) = 0.2
        _FresnelBias  ("Fresnel Bias", Float) = 0.2
        
        [Toggle(_SPECULAR_ON)] _SPECULAR ("Specular Reflection", Float) = 0
	    _SpecularColor ("Specular Color", COLOR)  = ( .72, .72, .72, 1)
	    _WorldLightDir ("Specular Light Direction", Vector) = (0.0, 0.1, -0.5, 0.0)
	    _Shininess ("Shininess", Range (2.0, 500.0)) = 200.0
		_Distortion("Distortion", Range (0.0, 1.5)) = 0.168
		
        [KeywordEnum(PerPixel, PerVertex)] _Normals ("Normals", Float)  = 0
        _NormalStrength ("Normal Strength", Float) = 15
		
        _BumpTiling ("Bump Tiling", Vector) = (0.04 ,0.04, 0.04, 0.08)
		_BumpDirection ("Bump Speed (Map1 x, y, Map2 z, w)", Vector) = (1.0 ,30.0, 20.0, -20.0)
        [NoScaleOffset] _Cube ("Cubemap", CUBE) = "black" {}
		[NoScaleOffset] _BumpMap ("Bump Normals", 2D) = "bump" {}
        
        [Toggle(_FOAM_ON)] _FOAM ("Enable Foam", Float) = 0
        _FoamColor("FoamColor", Color) = (1, 1, 1, 1)
	    [NoScaleOffset] _FoamTex ("Foam Texture", 2D) = "white" {}
	    [NoScaleOffset] _FoamGradient ("Foam Gradient ", 2D) = "white" {}
	    _FoamStrength ("Foam Strength", float) = 0.4

        [HideInInspector] _WaveHeightScale ("Wave Height Scale", Float) = 1.0
        [HideInInspector] _FaceCulling ("FaceCulling", Float) = 2
        [HideInInspector] _OneOrZero ("One Or Zero", Float) = 0
        [HideInInspector] _HeightOffset (" ", Float) = 0.0
        [HideInInspector] _ApplyOffset (" ", Float) = 0.0
        [HideInInspector] _BottomPos (" ", Float) = -2
	}

    Subshader {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline" = "LightweightPipeline" }
             
        Pass 
        {
            Tags { "LightMode"="LightweightForward" }
            Name"Base"
            
            Blend SrcAlpha OneMinusSrcAlpha
            ZTest LEqual
            Cull[_FaceCulling]     
            ZWrite On
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
            // Shader features     
            #pragma shader_feature _NORMALS_PERPIXEL _NORMALS_PERVERTEX
            #pragma shader_feature _ _FOAM_ON
            #pragma shader_feature _ _SPECULAR_ON

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
                float4 uv : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
                float4 grabPassPos : TEXCOORD3;
                float3 viewInterpolator : TEXCOORD4;
                float2 bumpuv0 : TEXCOORD5;
                float2 bumpuv1 : TEXCOORD6;

                #if _FOAM_ON
                    float2 foamuv       : TEXCOORD7;
                #endif

                #if _NORMALS_PERVERTEX
                    float3 vertNormal   : TEXCOORD8;
                #endif

                float4 clipPos : SV_POSITION;

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

            half3 worldSpaceVertex = mul(unity_ObjectToWorld, (v.vertex)).xyz;
            o.viewInterpolator.xyz = worldSpaceVertex - _WorldSpaceCameraPos;

            #if _NORMALS_PERVERTEX
                o.vertNormal = NormalFromHeightmap(_MainTex, _MainTex_TexelSize, float4(v.texcoord.xy, 0, 0), _NormalStrength);
            #endif

            o.pos.xyz = TransformObjectToWorld(v.vertex.xyz);
            o.uv.xy = v.texcoord;
            o.uv.zw = o.pos.xz;

            float4 screenUV = ComputeScreenPos(TransformWorldToHClip(o.pos.xyz));
            o.screenPos = screenUV;

            o.clipPos = TransformWorldToHClip(o.pos.xyz);
            o.pos = TransformWorldToHClip(o.pos.xyz);
            o.grabPassPos = ComputeGrabScreenPos(o.pos);

            float4 temp;
            float4 wpos = mul(unity_ObjectToWorld, v.vertex);
            temp.xyzw = wpos.xzxz * _BumpTiling.xyzw + _Time.xxxx * 0.1 * _BumpDirection.xyzw;
            o.bumpuv0 = temp.xy;
            o.bumpuv1 = temp.wz;
                
            #if _FOAM_ON
                o.foamuv = 7.0f * wpos.xz + 0.05 * float2(_SinTime.w, _SinTime.w);
            #endif
                
            return o;
        }

        half4 frag(v2f i) : SV_Target
        {
            UNITY_SETUP_INSTANCE_ID(i);
			             
            float3 normal = float3(0, 1, 0);
                
            #if _NORMALS_PERPIXEL
                normal = NormalFromHeightmap(_MainTex, _MainTex_TexelSize, float4(i.uv.xy, 0, 0), _NormalStrength);
            #endif 
    
            #if _NORMALS_PERVERTEX 
                normal = i.vertNormal;      
            #endif

            float4 reflectionScreenPos = float4(i.screenPos.xyz / i.screenPos.w, 1);
            float4 refractionScreenPos = float4(i.grabPassPos.xy / i.grabPassPos.w, 0, 0);
            float4 origRefractionScreenPos = refractionScreenPos;
   
            half3 bump1 = UnpackNormal(tex2D(_BumpMap, i.bumpuv0)).rgb;
            half3 bump2 = UnpackNormal(tex2D(_BumpMap, i.bumpuv1)).rgb;
            half3 bump = (bump1 + bump2) * 0.5;
                
            half2 distortOffset = half2(bump.xy * _Distortion);
            distortOffset = lerp(float2(0, 0), distortOffset, dot(normal, float3(0, 1, 0)));
            	
            half4 edgeBlendFactors = half4(1.0, 0.0, 0.0, 0.0);
            float uvOffsetLevel = 1;

            half depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_ScreenTextures_linear_clamp, reflectionScreenPos.xy);
            depth = LinearEyeDepth(depth, _ZBufferParams);
			        
            edgeBlendFactors = saturate(_EdgeBlend * (depth - i.screenPos.w));
            uvOffsetLevel = saturate(_WaterDepth * (depth - i.screenPos.w));

            reflectionScreenPos.xy += lerp(float2(0, 0), (normal.xz * _WaveHeightScale) / (i.screenPos.w + 0.00000000001) + distortOffset.xy, uvOffsetLevel);
            refractionScreenPos.xy += lerp(float2(0, 0), (normal.xz * _WaveHeightScale) / i.grabPassPos.w + distortOffset.xy, uvOffsetLevel);
                    
            float currentPixelDepth = i.screenPos.w;
            float offsetPixelDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_ScreenTextures_linear_clamp, reflectionScreenPos.xy), _ZBufferParams);

            float oneOrZero = step(offsetPixelDepth, currentPixelDepth);
            refractionScreenPos.xy = refractionScreenPos.xy * (step((oneOrZero + 1), 1)) + origRefractionScreenPos.xy * oneOrZero;

            float4 rtReflections = SAMPLE_TEXTURE2D(_ReflectionTex, sampler_ReflectionTex_linear_clamp, reflectionScreenPos.xy);
            float4 rtRefractions = SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture_linear_clamp, refractionScreenPos.xy);
                
            half3 viewVector = normalize(i.viewInterpolator.xyz);
            float4 cubeMapReflection = GetCubeMapColor(viewVector, normal, _Cube);
     
            rtReflections = lerp(rtReflections, cubeMapReflection, _CubeMapLevel);
            rtReflections = rtReflections * (step((_OneOrZero + 1), 1)) + cubeMapReflection * _OneOrZero;
                
            half refl2Refr = Fresnel(viewVector, UnityObjectToWorldNormal(float3(0, 1, 0)), _FresnelBias, _FresnelPower);
            refl2Refr = refl2Refr * (step((_OneOrZero + 1), 1)) + _UnderWaterOpacity * _OneOrZero;
                
            half depthWithOffset = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_ScreenTextures_linear_clamp, refractionScreenPos.xy);
            depthWithOffset = LinearEyeDepth(depthWithOffset, _ZBufferParams);
            depthWithOffset = depthWithOffset * (step((oneOrZero + 1), 1)) + depth * oneOrZero;

            float colorDepth = saturate(_WaterDepth / abs(depthWithOffset - i.grabPassPos.w));
            float4 colorTint = lerp(_DeepWaterColor, _ShallowWaterColor, colorDepth);
            rtReflections = lerp(colorTint, rtReflections, _Smoothness);
                
            float4 baseColor = lerp(rtRefractions, rtReflections, refl2Refr) * colorTint;
                
            #if _SPECULAR_ON
                float spec = 0;
                float3 worldNormal = UnityObjectToWorldNormal(normal);
                spec = GetSpecularLevel(viewVector, worldNormal, _WorldLightDir, _Shininess);
                baseColor = baseColor + spec * _SpecularColor;
            #endif
                       
            #if _FOAM_ON
                float objectZ = i.screenPos.w;
                float3 foam = Foam(_FoamTex, _FoamGradient, i.foamuv, objectZ, depth, _FoamStrength, bump);
                baseColor.rgb += foam;
            #endif

            baseColor.a = edgeBlendFactors.x;

            //--------------------------------------
            // Shader Debugging Options

            //#ifdef WATER_COLOR
            //    baseColor = baseColor;                            
            //#endif
                
            //#ifdef WATER_HEIGHTMAP
            //    float4 heightMapColor = tex2D(_MainTex, i.uv);
            //    heightMapColor = float4 (heightMapColor.r, heightMapColor.r, heightMapColor.r, 1);
            //    baseColor = heightMapColor;
            //#endif
                                
            //#ifdef WATER_NORMAL
            //    float3 newNorm = float3(normal.x, normal.z, normal.y);
            //    baseColor.rgb = newNorm * 0.5 + 0.5;
            //#endif
               
            return baseColor;
        }
		    ENDHLSL
        }
    }
}