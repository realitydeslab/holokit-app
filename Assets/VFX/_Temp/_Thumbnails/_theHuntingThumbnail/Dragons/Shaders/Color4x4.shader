// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Malbers/Color4x4"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[Header(Albedo (A Gradient))]_Color1("Color 1", Color) = (1,0.1544118,0.1544118,0.291)
		_Color2("Color 2", Color) = (1,0.1544118,0.8017241,0.253)
		_Color3("Color 3", Color) = (0.2535501,0.1544118,1,0.541)
		_Color4("Color 4", Color) = (0.1544118,0.5451319,1,0.253)
		[Space(10)]_Color5("Color 5", Color) = (0.9533468,1,0.1544118,0.553)
		_Color6("Color 6", Color) = (0.2720588,0.1294625,0,0.097)
		_Color7("Color 7", Color) = (0.1544118,0.6151115,1,0.178)
		_Color8("Color 8", Color) = (0.4849697,0.5008695,0.5073529,0.078)
		[Space(10)]_Color9("Color 9", Color) = (0.3164301,0,0.7058823,0.134)
		_Color10("Color 10", Color) = (0.362069,0.4411765,0,0.759)
		_Color11("Color 11", Color) = (0.6691177,0.6691177,0.6691177,0.647)
		_Color12("Color 12", Color) = (0.5073529,0.1574544,0,0.128)
		[Space(10)]_Color13("Color 13", Color) = (1,0.5586207,0,0.272)
		_Color14("Color 14", Color) = (0,0.8025862,0.875,0.047)
		_Color15("Color 15", Color) = (1,0,0,0.391)
		_Color16("Color 16", Color) = (0.4080882,0.75,0.4811866,0.134)
		[Header(Metallic(R) Rough(G) Emmission(B))]_MRE1("MRE 1", Color) = (0,1,0,0)
		_MRE2("MRE 2", Color) = (0,1,0,0)
		_MRE3("MRE 3", Color) = (0,1,0,0)
		_MRE4("MRE 4", Color) = (0,1,0,0)
		[Space(10)]_MRE5("MRE 5", Color) = (0,1,0,0)
		_MRE6("MRE 6", Color) = (0,1,0,0)
		_MRE7("MRE 7", Color) = (0,1,0,0)
		_MRE8("MRE 8", Color) = (0,1,0,0)
		[Space(10)]_MRE9("MRE 9", Color) = (0,1,0,0)
		_MRE10("MRE 10", Color) = (0,1,0,0)
		_MRE11("MRE 11", Color) = (0,1,0,0)
		_MRE12("MRE 12", Color) = (0,1,0,0)
		[Space(10)]_MRE13("MRE 13", Color) = (0,1,0,0)
		_MRE14("MRE 14", Color) = (0,1,0,0)
		_MRE15("MRE 15", Color) = (0,1,0,0)
		_MRE16("MRE 16", Color) = (0,1,0,0)
		[Header(Emmision)]_EmissionPower1("Emission Power", Float) = 1
		[SingleLineTexture][Header(Gradient)]_Gradient("Gradient", 2D) = "white" {}
		_GradientIntensity("Gradient Intensity", Range( 0 , 1)) = 1
		_GradientColor("Gradient Color", Color) = (0,0,0,0)
		_GradientScale("Gradient Scale", Float) = 1
		_GradientOffset("Gradient Offset", Float) = 0
		_GradientPower("Gradient Power", Float) = 1

		[HideInInspector]_QueueOffset("_QueueOffset", Float) = 0
        [HideInInspector]_QueueControl("_QueueControl", Float) = -1
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
		//_TransmissionShadow( "Transmission Shadow", Range( 0, 1 ) ) = 0.5
		//_TransStrength( "Trans Strength", Range( 0, 50 ) ) = 1
		//_TransNormal( "Trans Normal Distortion", Range( 0, 1 ) ) = 0.5
		//_TransScattering( "Trans Scattering", Range( 1, 50 ) ) = 2
		//_TransDirect( "Trans Direct", Range( 0, 1 ) ) = 0.9
		//_TransAmbient( "Trans Ambient", Range( 0, 1 ) ) = 0.1
		//_TransShadow( "Trans Shadow", Range( 0, 1 ) ) = 0.5
		//_TessPhongStrength( "Tess Phong Strength", Range( 0, 1 ) ) = 0.5
		//_TessValue( "Tess Max Tessellation", Range( 1, 32 ) ) = 16
		//_TessMin( "Tess Min Distance", Float ) = 10
		//_TessMax( "Tess Max Distance", Float ) = 25
		//_TessEdgeLength ( "Tess Edge length", Range( 2, 50 ) ) = 16
		//_TessMaxDisp( "Tess Max Displacement", Float ) = 25
	}

	SubShader
	{
		LOD 0

		
		
		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry" }
		Cull Off
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		AlphaToMask Off
		
		HLSLINCLUDE
		#pragma target 3.0

		#pragma prefer_hlslcc gles
		#pragma exclude_renderers d3d11_9x 

		#ifndef ASE_TESS_FUNCS
		#define ASE_TESS_FUNCS
		float4 FixedTess( float tessValue )
		{
			return tessValue;
		}
		
		float CalcDistanceTessFactor (float4 vertex, float minDist, float maxDist, float tess, float4x4 o2w, float3 cameraPos )
		{
			float3 wpos = mul(o2w,vertex).xyz;
			float dist = distance (wpos, cameraPos);
			float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0) * tess;
			return f;
		}

		float4 CalcTriEdgeTessFactors (float3 triVertexFactors)
		{
			float4 tess;
			tess.x = 0.5 * (triVertexFactors.y + triVertexFactors.z);
			tess.y = 0.5 * (triVertexFactors.x + triVertexFactors.z);
			tess.z = 0.5 * (triVertexFactors.x + triVertexFactors.y);
			tess.w = (triVertexFactors.x + triVertexFactors.y + triVertexFactors.z) / 3.0f;
			return tess;
		}

		float CalcEdgeTessFactor (float3 wpos0, float3 wpos1, float edgeLen, float3 cameraPos, float4 scParams )
		{
			float dist = distance (0.5 * (wpos0+wpos1), cameraPos);
			float len = distance(wpos0, wpos1);
			float f = max(len * scParams.y / (edgeLen * dist), 1.0);
			return f;
		}

		float DistanceFromPlane (float3 pos, float4 plane)
		{
			float d = dot (float4(pos,1.0f), plane);
			return d;
		}

		bool WorldViewFrustumCull (float3 wpos0, float3 wpos1, float3 wpos2, float cullEps, float4 planes[6] )
		{
			float4 planeTest;
			planeTest.x = (( DistanceFromPlane(wpos0, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[0]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.y = (( DistanceFromPlane(wpos0, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[1]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.z = (( DistanceFromPlane(wpos0, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[2]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.w = (( DistanceFromPlane(wpos0, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[3]) > -cullEps) ? 1.0f : 0.0f );
			return !all (planeTest);
		}

		float4 DistanceBasedTess( float4 v0, float4 v1, float4 v2, float tess, float minDist, float maxDist, float4x4 o2w, float3 cameraPos )
		{
			float3 f;
			f.x = CalcDistanceTessFactor (v0,minDist,maxDist,tess,o2w,cameraPos);
			f.y = CalcDistanceTessFactor (v1,minDist,maxDist,tess,o2w,cameraPos);
			f.z = CalcDistanceTessFactor (v2,minDist,maxDist,tess,o2w,cameraPos);

			return CalcTriEdgeTessFactors (f);
		}

		float4 EdgeLengthBasedTess( float4 v0, float4 v1, float4 v2, float edgeLength, float4x4 o2w, float3 cameraPos, float4 scParams )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;
			tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
			tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
			tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
			tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			return tess;
		}

		float4 EdgeLengthBasedTessCull( float4 v0, float4 v1, float4 v2, float edgeLength, float maxDisplacement, float4x4 o2w, float3 cameraPos, float4 scParams, float4 planes[6] )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;

			if (WorldViewFrustumCull(pos0, pos1, pos2, maxDisplacement, planes))
			{
				tess = 0.0f;
			}
			else
			{
				tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
				tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
				tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
				tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			}
			return tess;
		}
		#endif //ASE_TESS_FUNCS

		ENDHLSL

		
		Pass
		{
			
			Name "Forward"
			Tags { "LightMode"="UniversalForward" }
			
			Blend One Zero, One Zero
			ColorMask RGBA
			

			HLSLPROGRAM

			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _EMISSION
			#define ASE_SRP_VERSION 999999


			#pragma multi_compile _ _SCREEN_SPACE_OCCLUSION
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS _ADDITIONAL_OFF
			#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile _ _SHADOWS_SOFT
			#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
			
			#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
			#pragma multi_compile _ SHADOWS_SHADOWMASK

			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile _ DYNAMICLIGHTMAP_ON

			#pragma multi_compile _ _REFLECTION_PROBE_BLENDING
			#pragma multi_compile _ _REFLECTION_PROBE_BOX_PROJECTION
			#pragma multi_compile _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
			#pragma multi_compile _ _LIGHT_LAYERS
			
			#pragma multi_compile _ _LIGHT_COOKIES
			#pragma multi_compile _ _CLUSTERED_RENDERING

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS SHADERPASS_FORWARD

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#if defined(UNITY_INSTANCING_ENABLED) && defined(_TERRAIN_INSTANCED_PERPIXEL_NORMAL)
			    #define ENABLE_TERRAIN_PERPIXEL_NORMAL
			#endif

			

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 lightmapUVOrVertexSH : TEXCOORD0;
				half4 fogFactorAndVertexLight : TEXCOORD1;
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				float4 shadowCoord : TEXCOORD2;
				#endif
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				float4 screenPos : TEXCOORD6;
				#endif
				#if defined(DYNAMICLIGHTMAP_ON)
				float2 dynamicLightmapUV : TEXCOORD7;
				#endif
				float4 ase_texcoord8 : TEXCOORD8;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _GradientColor;
			float4 _Color16;
			float4 _MRE1;
			float4 _MRE2;
			float4 _MRE3;
			float4 _MRE4;
			float4 _MRE5;
			float4 _MRE6;
			float4 _MRE7;
			float4 _MRE8;
			float4 _MRE9;
			float4 _MRE10;
			float4 _MRE11;
			float4 _MRE12;
			float4 _MRE13;
			float4 _MRE14;
			float4 _Color15;
			float4 _MRE15;
			float4 _Color14;
			float4 _Color12;
			float4 _Color1;
			float4 _Color2;
			float4 _Color3;
			float4 _Color13;
			float4 _Color5;
			float4 _Color6;
			float4 _Color4;
			float4 _Color8;
			float4 _Color9;
			float4 _Color10;
			float4 _Color11;
			float4 _Color7;
			float4 _MRE16;
			float _EmissionPower1;
			float _GradientPower;
			float _GradientOffset;
			float _GradientScale;
			float _GradientIntensity;
			#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			sampler2D _Gradient;


			
			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.ase_texcoord8.xy = v.texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord8.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float3 positionVS = TransformWorldToView( positionWS );
				float4 positionCS = TransformWorldToHClip( positionWS );

				VertexNormalInputs normalInput = GetVertexNormalInputs( v.ase_normal, v.ase_tangent );

				o.tSpace0 = float4( normalInput.normalWS, positionWS.x);
				o.tSpace1 = float4( normalInput.tangentWS, positionWS.y);
				o.tSpace2 = float4( normalInput.bitangentWS, positionWS.z);

				#if defined(LIGHTMAP_ON)
				OUTPUT_LIGHTMAP_UV( v.texcoord1, unity_LightmapST, o.lightmapUVOrVertexSH.xy );
				#endif

				#if defined(DYNAMICLIGHTMAP_ON)
				o.dynamicLightmapUV.xy = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
				#endif

				#if !defined(LIGHTMAP_ON)
				OUTPUT_SH( normalInput.normalWS.xyz, o.lightmapUVOrVertexSH.xyz );
				#endif

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					o.lightmapUVOrVertexSH.zw = v.texcoord;
					o.lightmapUVOrVertexSH.xy = v.texcoord * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif

				half3 vertexLight = VertexLighting( positionWS, normalInput.normalWS );
				#ifdef ASE_FOG
					half fogFactor = ComputeFogFactor( positionCS.z );
				#else
					half fogFactor = 0;
				#endif
				o.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
				
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				VertexPositionInputs vertexInput = (VertexPositionInputs)0;
				vertexInput.positionWS = positionWS;
				vertexInput.positionCS = positionCS;
				o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				
				o.clipPos = positionCS;
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				o.screenPos = ComputeScreenPos(positionCS);
				#endif
				return o;
			}
			
			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_tangent = v.ase_tangent;
				o.texcoord = v.texcoord;
				o.texcoord1 = v.texcoord1;
				o.texcoord2 = v.texcoord2;
				
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;
				o.texcoord = patch[0].texcoord * bary.x + patch[1].texcoord * bary.y + patch[2].texcoord * bary.z;
				o.texcoord1 = patch[0].texcoord1 * bary.x + patch[1].texcoord1 * bary.y + patch[2].texcoord1 * bary.z;
				o.texcoord2 = patch[0].texcoord2 * bary.x + patch[1].texcoord2 * bary.y + patch[2].texcoord2 * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)
				#define ASE_SV_DEPTH SV_DepthLessEqual  
			#else
				#define ASE_SV_DEPTH SV_Depth
			#endif

			half4 frag ( VertexOutput IN 
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float2 sampleCoords = (IN.lightmapUVOrVertexSH.zw / _TerrainHeightmapRecipSize.zw + 0.5f) * _TerrainHeightmapRecipSize.xy;
					float3 WorldNormal = TransformObjectToWorldNormal(normalize(SAMPLE_TEXTURE2D(_TerrainNormalmapTexture, sampler_TerrainNormalmapTexture, sampleCoords).rgb * 2 - 1));
					float3 WorldTangent = -cross(GetObjectToWorldMatrix()._13_23_33, WorldNormal);
					float3 WorldBiTangent = cross(WorldNormal, -WorldTangent);
				#else
					float3 WorldNormal = normalize( IN.tSpace0.xyz );
					float3 WorldTangent = IN.tSpace1.xyz;
					float3 WorldBiTangent = IN.tSpace2.xyz;
				#endif
				float3 WorldPosition = float3(IN.tSpace0.w,IN.tSpace1.w,IN.tSpace2.w);
				float3 WorldViewDirection = _WorldSpaceCameraPos.xyz  - WorldPosition;
				float4 ShadowCoords = float4( 0, 0, 0, 0 );
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				float4 ScreenPos = IN.screenPos;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					ShadowCoords = IN.shadowCoord;
				#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
					ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
				#endif
	
				WorldViewDirection = SafeNormalize( WorldViewDirection );

				float2 texCoord256 = IN.ase_texcoord8.xy * float2( 1,4 ) + float2( 0,0 );
				float4 clampResult328 = clamp( ( ( tex2D( _Gradient, texCoord256 ) + _GradientColor ) + ( 1.0 - _GradientIntensity ) ) , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
				float4 saferPower342 = abs( (clampResult328*_GradientScale + _GradientOffset) );
				float4 temp_cast_0 = (_GradientPower).xxxx;
				float2 texCoord2_g760 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g760 = 1.0;
				float temp_output_7_0_g760 = 4.0;
				float temp_output_9_0_g760 = 4.0;
				float temp_output_8_0_g760 = 4.0;
				float2 texCoord2_g754 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g754 = 2.0;
				float temp_output_7_0_g754 = 4.0;
				float temp_output_9_0_g754 = 4.0;
				float temp_output_8_0_g754 = 4.0;
				float2 texCoord2_g755 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g755 = 3.0;
				float temp_output_7_0_g755 = 4.0;
				float temp_output_9_0_g755 = 4.0;
				float temp_output_8_0_g755 = 4.0;
				float2 texCoord2_g757 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g757 = 4.0;
				float temp_output_7_0_g757 = 4.0;
				float temp_output_9_0_g757 = 4.0;
				float temp_output_8_0_g757 = 4.0;
				float2 texCoord2_g750 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g750 = 1.0;
				float temp_output_7_0_g750 = 4.0;
				float temp_output_9_0_g750 = 3.0;
				float temp_output_8_0_g750 = 4.0;
				float2 texCoord2_g745 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g745 = 2.0;
				float temp_output_7_0_g745 = 4.0;
				float temp_output_9_0_g745 = 3.0;
				float temp_output_8_0_g745 = 4.0;
				float2 texCoord2_g752 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g752 = 3.0;
				float temp_output_7_0_g752 = 4.0;
				float temp_output_9_0_g752 = 3.0;
				float temp_output_8_0_g752 = 4.0;
				float2 texCoord2_g747 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g747 = 4.0;
				float temp_output_7_0_g747 = 4.0;
				float temp_output_9_0_g747 = 3.0;
				float temp_output_8_0_g747 = 4.0;
				float2 texCoord2_g746 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g746 = 1.0;
				float temp_output_7_0_g746 = 4.0;
				float temp_output_9_0_g746 = 2.0;
				float temp_output_8_0_g746 = 4.0;
				float2 texCoord2_g756 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g756 = 2.0;
				float temp_output_7_0_g756 = 4.0;
				float temp_output_9_0_g756 = 2.0;
				float temp_output_8_0_g756 = 4.0;
				float2 texCoord2_g748 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g748 = 3.0;
				float temp_output_7_0_g748 = 4.0;
				float temp_output_9_0_g748 = 2.0;
				float temp_output_8_0_g748 = 4.0;
				float2 texCoord2_g759 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g759 = 4.0;
				float temp_output_7_0_g759 = 4.0;
				float temp_output_9_0_g759 = 2.0;
				float temp_output_8_0_g759 = 4.0;
				float2 texCoord2_g758 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g758 = 1.0;
				float temp_output_7_0_g758 = 4.0;
				float temp_output_9_0_g758 = 1.0;
				float temp_output_8_0_g758 = 4.0;
				float2 texCoord2_g751 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g751 = 2.0;
				float temp_output_7_0_g751 = 4.0;
				float temp_output_9_0_g751 = 1.0;
				float temp_output_8_0_g751 = 4.0;
				float2 texCoord2_g753 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g753 = 3.0;
				float temp_output_7_0_g753 = 4.0;
				float temp_output_9_0_g753 = 1.0;
				float temp_output_8_0_g753 = 4.0;
				float2 texCoord2_g749 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g749 = 4.0;
				float temp_output_7_0_g749 = 4.0;
				float temp_output_9_0_g749 = 1.0;
				float temp_output_8_0_g749 = 4.0;
				float4 temp_output_329_0 = ( ( ( _Color1 * ( ( ( 1.0 - step( texCoord2_g760.x , ( ( temp_output_3_0_g760 - 1.0 ) / temp_output_7_0_g760 ) ) ) * ( step( texCoord2_g760.x , ( temp_output_3_0_g760 / temp_output_7_0_g760 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g760.y , ( ( temp_output_9_0_g760 - 1.0 ) / temp_output_8_0_g760 ) ) ) * ( step( texCoord2_g760.y , ( temp_output_9_0_g760 / temp_output_8_0_g760 ) ) * 1.0 ) ) ) ) + ( _Color2 * ( ( ( 1.0 - step( texCoord2_g754.x , ( ( temp_output_3_0_g754 - 1.0 ) / temp_output_7_0_g754 ) ) ) * ( step( texCoord2_g754.x , ( temp_output_3_0_g754 / temp_output_7_0_g754 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g754.y , ( ( temp_output_9_0_g754 - 1.0 ) / temp_output_8_0_g754 ) ) ) * ( step( texCoord2_g754.y , ( temp_output_9_0_g754 / temp_output_8_0_g754 ) ) * 1.0 ) ) ) ) + ( _Color3 * ( ( ( 1.0 - step( texCoord2_g755.x , ( ( temp_output_3_0_g755 - 1.0 ) / temp_output_7_0_g755 ) ) ) * ( step( texCoord2_g755.x , ( temp_output_3_0_g755 / temp_output_7_0_g755 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g755.y , ( ( temp_output_9_0_g755 - 1.0 ) / temp_output_8_0_g755 ) ) ) * ( step( texCoord2_g755.y , ( temp_output_9_0_g755 / temp_output_8_0_g755 ) ) * 1.0 ) ) ) ) + ( _Color4 * ( ( ( 1.0 - step( texCoord2_g757.x , ( ( temp_output_3_0_g757 - 1.0 ) / temp_output_7_0_g757 ) ) ) * ( step( texCoord2_g757.x , ( temp_output_3_0_g757 / temp_output_7_0_g757 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g757.y , ( ( temp_output_9_0_g757 - 1.0 ) / temp_output_8_0_g757 ) ) ) * ( step( texCoord2_g757.y , ( temp_output_9_0_g757 / temp_output_8_0_g757 ) ) * 1.0 ) ) ) ) ) + ( ( _Color5 * ( ( ( 1.0 - step( texCoord2_g750.x , ( ( temp_output_3_0_g750 - 1.0 ) / temp_output_7_0_g750 ) ) ) * ( step( texCoord2_g750.x , ( temp_output_3_0_g750 / temp_output_7_0_g750 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g750.y , ( ( temp_output_9_0_g750 - 1.0 ) / temp_output_8_0_g750 ) ) ) * ( step( texCoord2_g750.y , ( temp_output_9_0_g750 / temp_output_8_0_g750 ) ) * 1.0 ) ) ) ) + ( _Color6 * ( ( ( 1.0 - step( texCoord2_g745.x , ( ( temp_output_3_0_g745 - 1.0 ) / temp_output_7_0_g745 ) ) ) * ( step( texCoord2_g745.x , ( temp_output_3_0_g745 / temp_output_7_0_g745 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g745.y , ( ( temp_output_9_0_g745 - 1.0 ) / temp_output_8_0_g745 ) ) ) * ( step( texCoord2_g745.y , ( temp_output_9_0_g745 / temp_output_8_0_g745 ) ) * 1.0 ) ) ) ) + ( _Color7 * ( ( ( 1.0 - step( texCoord2_g752.x , ( ( temp_output_3_0_g752 - 1.0 ) / temp_output_7_0_g752 ) ) ) * ( step( texCoord2_g752.x , ( temp_output_3_0_g752 / temp_output_7_0_g752 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g752.y , ( ( temp_output_9_0_g752 - 1.0 ) / temp_output_8_0_g752 ) ) ) * ( step( texCoord2_g752.y , ( temp_output_9_0_g752 / temp_output_8_0_g752 ) ) * 1.0 ) ) ) ) + ( _Color8 * ( ( ( 1.0 - step( texCoord2_g747.x , ( ( temp_output_3_0_g747 - 1.0 ) / temp_output_7_0_g747 ) ) ) * ( step( texCoord2_g747.x , ( temp_output_3_0_g747 / temp_output_7_0_g747 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g747.y , ( ( temp_output_9_0_g747 - 1.0 ) / temp_output_8_0_g747 ) ) ) * ( step( texCoord2_g747.y , ( temp_output_9_0_g747 / temp_output_8_0_g747 ) ) * 1.0 ) ) ) ) ) + ( ( _Color9 * ( ( ( 1.0 - step( texCoord2_g746.x , ( ( temp_output_3_0_g746 - 1.0 ) / temp_output_7_0_g746 ) ) ) * ( step( texCoord2_g746.x , ( temp_output_3_0_g746 / temp_output_7_0_g746 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g746.y , ( ( temp_output_9_0_g746 - 1.0 ) / temp_output_8_0_g746 ) ) ) * ( step( texCoord2_g746.y , ( temp_output_9_0_g746 / temp_output_8_0_g746 ) ) * 1.0 ) ) ) ) + ( _Color10 * ( ( ( 1.0 - step( texCoord2_g756.x , ( ( temp_output_3_0_g756 - 1.0 ) / temp_output_7_0_g756 ) ) ) * ( step( texCoord2_g756.x , ( temp_output_3_0_g756 / temp_output_7_0_g756 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g756.y , ( ( temp_output_9_0_g756 - 1.0 ) / temp_output_8_0_g756 ) ) ) * ( step( texCoord2_g756.y , ( temp_output_9_0_g756 / temp_output_8_0_g756 ) ) * 1.0 ) ) ) ) + ( _Color11 * ( ( ( 1.0 - step( texCoord2_g748.x , ( ( temp_output_3_0_g748 - 1.0 ) / temp_output_7_0_g748 ) ) ) * ( step( texCoord2_g748.x , ( temp_output_3_0_g748 / temp_output_7_0_g748 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g748.y , ( ( temp_output_9_0_g748 - 1.0 ) / temp_output_8_0_g748 ) ) ) * ( step( texCoord2_g748.y , ( temp_output_9_0_g748 / temp_output_8_0_g748 ) ) * 1.0 ) ) ) ) + ( _Color12 * ( ( ( 1.0 - step( texCoord2_g759.x , ( ( temp_output_3_0_g759 - 1.0 ) / temp_output_7_0_g759 ) ) ) * ( step( texCoord2_g759.x , ( temp_output_3_0_g759 / temp_output_7_0_g759 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g759.y , ( ( temp_output_9_0_g759 - 1.0 ) / temp_output_8_0_g759 ) ) ) * ( step( texCoord2_g759.y , ( temp_output_9_0_g759 / temp_output_8_0_g759 ) ) * 1.0 ) ) ) ) ) + ( ( _Color13 * ( ( ( 1.0 - step( texCoord2_g758.x , ( ( temp_output_3_0_g758 - 1.0 ) / temp_output_7_0_g758 ) ) ) * ( step( texCoord2_g758.x , ( temp_output_3_0_g758 / temp_output_7_0_g758 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g758.y , ( ( temp_output_9_0_g758 - 1.0 ) / temp_output_8_0_g758 ) ) ) * ( step( texCoord2_g758.y , ( temp_output_9_0_g758 / temp_output_8_0_g758 ) ) * 1.0 ) ) ) ) + ( _Color14 * ( ( ( 1.0 - step( texCoord2_g751.x , ( ( temp_output_3_0_g751 - 1.0 ) / temp_output_7_0_g751 ) ) ) * ( step( texCoord2_g751.x , ( temp_output_3_0_g751 / temp_output_7_0_g751 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g751.y , ( ( temp_output_9_0_g751 - 1.0 ) / temp_output_8_0_g751 ) ) ) * ( step( texCoord2_g751.y , ( temp_output_9_0_g751 / temp_output_8_0_g751 ) ) * 1.0 ) ) ) ) + ( _Color15 * ( ( ( 1.0 - step( texCoord2_g753.x , ( ( temp_output_3_0_g753 - 1.0 ) / temp_output_7_0_g753 ) ) ) * ( step( texCoord2_g753.x , ( temp_output_3_0_g753 / temp_output_7_0_g753 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g753.y , ( ( temp_output_9_0_g753 - 1.0 ) / temp_output_8_0_g753 ) ) ) * ( step( texCoord2_g753.y , ( temp_output_9_0_g753 / temp_output_8_0_g753 ) ) * 1.0 ) ) ) ) + ( _Color16 * ( ( ( 1.0 - step( texCoord2_g749.x , ( ( temp_output_3_0_g749 - 1.0 ) / temp_output_7_0_g749 ) ) ) * ( step( texCoord2_g749.x , ( temp_output_3_0_g749 / temp_output_7_0_g749 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g749.y , ( ( temp_output_9_0_g749 - 1.0 ) / temp_output_8_0_g749 ) ) ) * ( step( texCoord2_g749.y , ( temp_output_9_0_g749 / temp_output_8_0_g749 ) ) * 1.0 ) ) ) ) ) );
				float4 clampResult348 = clamp( ( pow( saferPower342 , temp_cast_0 ) + ( 1.0 - (temp_output_329_0).a ) ) , float4( 0,0,0,0 ) , float4( 1,1,1,1 ) );
				
				float2 texCoord2_g731 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g731 = 1.0;
				float temp_output_7_0_g731 = 4.0;
				float temp_output_9_0_g731 = 4.0;
				float temp_output_8_0_g731 = 4.0;
				float2 texCoord2_g744 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g744 = 2.0;
				float temp_output_7_0_g744 = 4.0;
				float temp_output_9_0_g744 = 4.0;
				float temp_output_8_0_g744 = 4.0;
				float2 texCoord2_g732 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g732 = 3.0;
				float temp_output_7_0_g732 = 4.0;
				float temp_output_9_0_g732 = 4.0;
				float temp_output_8_0_g732 = 4.0;
				float2 texCoord2_g741 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g741 = 4.0;
				float temp_output_7_0_g741 = 4.0;
				float temp_output_9_0_g741 = 4.0;
				float temp_output_8_0_g741 = 4.0;
				float2 texCoord2_g739 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g739 = 1.0;
				float temp_output_7_0_g739 = 4.0;
				float temp_output_9_0_g739 = 3.0;
				float temp_output_8_0_g739 = 4.0;
				float2 texCoord2_g733 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g733 = 2.0;
				float temp_output_7_0_g733 = 4.0;
				float temp_output_9_0_g733 = 3.0;
				float temp_output_8_0_g733 = 4.0;
				float2 texCoord2_g743 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g743 = 3.0;
				float temp_output_7_0_g743 = 4.0;
				float temp_output_9_0_g743 = 3.0;
				float temp_output_8_0_g743 = 4.0;
				float2 texCoord2_g736 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g736 = 4.0;
				float temp_output_7_0_g736 = 4.0;
				float temp_output_9_0_g736 = 3.0;
				float temp_output_8_0_g736 = 4.0;
				float2 texCoord2_g742 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g742 = 1.0;
				float temp_output_7_0_g742 = 4.0;
				float temp_output_9_0_g742 = 2.0;
				float temp_output_8_0_g742 = 4.0;
				float2 texCoord2_g735 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g735 = 2.0;
				float temp_output_7_0_g735 = 4.0;
				float temp_output_9_0_g735 = 2.0;
				float temp_output_8_0_g735 = 4.0;
				float2 texCoord2_g729 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g729 = 3.0;
				float temp_output_7_0_g729 = 4.0;
				float temp_output_9_0_g729 = 2.0;
				float temp_output_8_0_g729 = 4.0;
				float2 texCoord2_g730 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g730 = 4.0;
				float temp_output_7_0_g730 = 4.0;
				float temp_output_9_0_g730 = 2.0;
				float temp_output_8_0_g730 = 4.0;
				float2 texCoord2_g738 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g738 = 1.0;
				float temp_output_7_0_g738 = 4.0;
				float temp_output_9_0_g738 = 1.0;
				float temp_output_8_0_g738 = 4.0;
				float2 texCoord2_g737 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g737 = 2.0;
				float temp_output_7_0_g737 = 4.0;
				float temp_output_9_0_g737 = 1.0;
				float temp_output_8_0_g737 = 4.0;
				float2 texCoord2_g734 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g734 = 3.0;
				float temp_output_7_0_g734 = 4.0;
				float temp_output_9_0_g734 = 1.0;
				float temp_output_8_0_g734 = 4.0;
				float2 texCoord2_g740 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g740 = 4.0;
				float temp_output_7_0_g740 = 4.0;
				float temp_output_9_0_g740 = 1.0;
				float temp_output_8_0_g740 = 4.0;
				float4 temp_output_344_0 = ( ( ( _MRE1 * ( ( ( 1.0 - step( texCoord2_g731.x , ( ( temp_output_3_0_g731 - 1.0 ) / temp_output_7_0_g731 ) ) ) * ( step( texCoord2_g731.x , ( temp_output_3_0_g731 / temp_output_7_0_g731 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g731.y , ( ( temp_output_9_0_g731 - 1.0 ) / temp_output_8_0_g731 ) ) ) * ( step( texCoord2_g731.y , ( temp_output_9_0_g731 / temp_output_8_0_g731 ) ) * 1.0 ) ) ) ) + ( _MRE2 * ( ( ( 1.0 - step( texCoord2_g744.x , ( ( temp_output_3_0_g744 - 1.0 ) / temp_output_7_0_g744 ) ) ) * ( step( texCoord2_g744.x , ( temp_output_3_0_g744 / temp_output_7_0_g744 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g744.y , ( ( temp_output_9_0_g744 - 1.0 ) / temp_output_8_0_g744 ) ) ) * ( step( texCoord2_g744.y , ( temp_output_9_0_g744 / temp_output_8_0_g744 ) ) * 1.0 ) ) ) ) + ( _MRE3 * ( ( ( 1.0 - step( texCoord2_g732.x , ( ( temp_output_3_0_g732 - 1.0 ) / temp_output_7_0_g732 ) ) ) * ( step( texCoord2_g732.x , ( temp_output_3_0_g732 / temp_output_7_0_g732 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g732.y , ( ( temp_output_9_0_g732 - 1.0 ) / temp_output_8_0_g732 ) ) ) * ( step( texCoord2_g732.y , ( temp_output_9_0_g732 / temp_output_8_0_g732 ) ) * 1.0 ) ) ) ) + ( _MRE4 * ( ( ( 1.0 - step( texCoord2_g741.x , ( ( temp_output_3_0_g741 - 1.0 ) / temp_output_7_0_g741 ) ) ) * ( step( texCoord2_g741.x , ( temp_output_3_0_g741 / temp_output_7_0_g741 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g741.y , ( ( temp_output_9_0_g741 - 1.0 ) / temp_output_8_0_g741 ) ) ) * ( step( texCoord2_g741.y , ( temp_output_9_0_g741 / temp_output_8_0_g741 ) ) * 1.0 ) ) ) ) ) + ( ( _MRE5 * ( ( ( 1.0 - step( texCoord2_g739.x , ( ( temp_output_3_0_g739 - 1.0 ) / temp_output_7_0_g739 ) ) ) * ( step( texCoord2_g739.x , ( temp_output_3_0_g739 / temp_output_7_0_g739 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g739.y , ( ( temp_output_9_0_g739 - 1.0 ) / temp_output_8_0_g739 ) ) ) * ( step( texCoord2_g739.y , ( temp_output_9_0_g739 / temp_output_8_0_g739 ) ) * 1.0 ) ) ) ) + ( _MRE6 * ( ( ( 1.0 - step( texCoord2_g733.x , ( ( temp_output_3_0_g733 - 1.0 ) / temp_output_7_0_g733 ) ) ) * ( step( texCoord2_g733.x , ( temp_output_3_0_g733 / temp_output_7_0_g733 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g733.y , ( ( temp_output_9_0_g733 - 1.0 ) / temp_output_8_0_g733 ) ) ) * ( step( texCoord2_g733.y , ( temp_output_9_0_g733 / temp_output_8_0_g733 ) ) * 1.0 ) ) ) ) + ( _MRE7 * ( ( ( 1.0 - step( texCoord2_g743.x , ( ( temp_output_3_0_g743 - 1.0 ) / temp_output_7_0_g743 ) ) ) * ( step( texCoord2_g743.x , ( temp_output_3_0_g743 / temp_output_7_0_g743 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g743.y , ( ( temp_output_9_0_g743 - 1.0 ) / temp_output_8_0_g743 ) ) ) * ( step( texCoord2_g743.y , ( temp_output_9_0_g743 / temp_output_8_0_g743 ) ) * 1.0 ) ) ) ) + ( _MRE8 * ( ( ( 1.0 - step( texCoord2_g736.x , ( ( temp_output_3_0_g736 - 1.0 ) / temp_output_7_0_g736 ) ) ) * ( step( texCoord2_g736.x , ( temp_output_3_0_g736 / temp_output_7_0_g736 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g736.y , ( ( temp_output_9_0_g736 - 1.0 ) / temp_output_8_0_g736 ) ) ) * ( step( texCoord2_g736.y , ( temp_output_9_0_g736 / temp_output_8_0_g736 ) ) * 1.0 ) ) ) ) ) + ( ( _MRE9 * ( ( ( 1.0 - step( texCoord2_g742.x , ( ( temp_output_3_0_g742 - 1.0 ) / temp_output_7_0_g742 ) ) ) * ( step( texCoord2_g742.x , ( temp_output_3_0_g742 / temp_output_7_0_g742 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g742.y , ( ( temp_output_9_0_g742 - 1.0 ) / temp_output_8_0_g742 ) ) ) * ( step( texCoord2_g742.y , ( temp_output_9_0_g742 / temp_output_8_0_g742 ) ) * 1.0 ) ) ) ) + ( _MRE10 * ( ( ( 1.0 - step( texCoord2_g735.x , ( ( temp_output_3_0_g735 - 1.0 ) / temp_output_7_0_g735 ) ) ) * ( step( texCoord2_g735.x , ( temp_output_3_0_g735 / temp_output_7_0_g735 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g735.y , ( ( temp_output_9_0_g735 - 1.0 ) / temp_output_8_0_g735 ) ) ) * ( step( texCoord2_g735.y , ( temp_output_9_0_g735 / temp_output_8_0_g735 ) ) * 1.0 ) ) ) ) + ( _MRE11 * ( ( ( 1.0 - step( texCoord2_g729.x , ( ( temp_output_3_0_g729 - 1.0 ) / temp_output_7_0_g729 ) ) ) * ( step( texCoord2_g729.x , ( temp_output_3_0_g729 / temp_output_7_0_g729 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g729.y , ( ( temp_output_9_0_g729 - 1.0 ) / temp_output_8_0_g729 ) ) ) * ( step( texCoord2_g729.y , ( temp_output_9_0_g729 / temp_output_8_0_g729 ) ) * 1.0 ) ) ) ) + ( _MRE12 * ( ( ( 1.0 - step( texCoord2_g730.x , ( ( temp_output_3_0_g730 - 1.0 ) / temp_output_7_0_g730 ) ) ) * ( step( texCoord2_g730.x , ( temp_output_3_0_g730 / temp_output_7_0_g730 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g730.y , ( ( temp_output_9_0_g730 - 1.0 ) / temp_output_8_0_g730 ) ) ) * ( step( texCoord2_g730.y , ( temp_output_9_0_g730 / temp_output_8_0_g730 ) ) * 1.0 ) ) ) ) ) + ( ( _MRE13 * ( ( ( 1.0 - step( texCoord2_g738.x , ( ( temp_output_3_0_g738 - 1.0 ) / temp_output_7_0_g738 ) ) ) * ( step( texCoord2_g738.x , ( temp_output_3_0_g738 / temp_output_7_0_g738 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g738.y , ( ( temp_output_9_0_g738 - 1.0 ) / temp_output_8_0_g738 ) ) ) * ( step( texCoord2_g738.y , ( temp_output_9_0_g738 / temp_output_8_0_g738 ) ) * 1.0 ) ) ) ) + ( _MRE14 * ( ( ( 1.0 - step( texCoord2_g737.x , ( ( temp_output_3_0_g737 - 1.0 ) / temp_output_7_0_g737 ) ) ) * ( step( texCoord2_g737.x , ( temp_output_3_0_g737 / temp_output_7_0_g737 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g737.y , ( ( temp_output_9_0_g737 - 1.0 ) / temp_output_8_0_g737 ) ) ) * ( step( texCoord2_g737.y , ( temp_output_9_0_g737 / temp_output_8_0_g737 ) ) * 1.0 ) ) ) ) + ( _MRE15 * ( ( ( 1.0 - step( texCoord2_g734.x , ( ( temp_output_3_0_g734 - 1.0 ) / temp_output_7_0_g734 ) ) ) * ( step( texCoord2_g734.x , ( temp_output_3_0_g734 / temp_output_7_0_g734 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g734.y , ( ( temp_output_9_0_g734 - 1.0 ) / temp_output_8_0_g734 ) ) ) * ( step( texCoord2_g734.y , ( temp_output_9_0_g734 / temp_output_8_0_g734 ) ) * 1.0 ) ) ) ) + ( _MRE16 * ( ( ( 1.0 - step( texCoord2_g740.x , ( ( temp_output_3_0_g740 - 1.0 ) / temp_output_7_0_g740 ) ) ) * ( step( texCoord2_g740.x , ( temp_output_3_0_g740 / temp_output_7_0_g740 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g740.y , ( ( temp_output_9_0_g740 - 1.0 ) / temp_output_8_0_g740 ) ) ) * ( step( texCoord2_g740.y , ( temp_output_9_0_g740 / temp_output_8_0_g740 ) ) * 1.0 ) ) ) ) ) );
				
				float3 Albedo = ( clampResult348 * temp_output_329_0 ).rgb;
				float3 Normal = float3(0, 0, 1);
				float3 Emission = ( temp_output_329_0 * ( _EmissionPower1 * (temp_output_344_0).b ) ).rgb;
				float3 Specular = 0.5;
				float Metallic = (temp_output_344_0).r;
				float Smoothness = ( 1.0 - (temp_output_344_0).g );
				float Occlusion = 1;
				float Alpha = 1;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;
				float3 BakedGI = 0;
				float3 RefractionColor = 1;
				float RefractionIndex = 1;
				float3 Transmission = 1;
				float3 Translucency = 1;
				#ifdef ASE_DEPTH_WRITE_ON
				float DepthValue = 0;
				#endif
				
				#ifdef _CLEARCOAT
				float CoatMask = 0;
				float CoatSmoothness = 0;
				#endif


				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				InputData inputData = (InputData)0;
				inputData.positionWS = WorldPosition;
				inputData.viewDirectionWS = WorldViewDirection;
				

				#ifdef _NORMALMAP
					#if _NORMAL_DROPOFF_TS
					inputData.normalWS = TransformTangentToWorld(Normal, half3x3( WorldTangent, WorldBiTangent, WorldNormal ));
					#elif _NORMAL_DROPOFF_OS
					inputData.normalWS = TransformObjectToWorldNormal(Normal);
					#elif _NORMAL_DROPOFF_WS
					inputData.normalWS = Normal;
					#endif
					inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
				#else
					inputData.normalWS = WorldNormal;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					inputData.shadowCoord = ShadowCoords;
				#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
					inputData.shadowCoord = TransformWorldToShadowCoord(inputData.positionWS);
				#else
					inputData.shadowCoord = float4(0, 0, 0, 0);
				#endif


				#ifdef ASE_FOG
					inputData.fogCoord = IN.fogFactorAndVertexLight.x;
				#endif

				inputData.vertexLighting = IN.fogFactorAndVertexLight.yzw;
				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float3 SH = SampleSH(inputData.normalWS.xyz);
				#else
					float3 SH = IN.lightmapUVOrVertexSH.xyz;
				#endif

				#if defined(DYNAMICLIGHTMAP_ON)
				inputData.bakedGI = SAMPLE_GI(IN.lightmapUVOrVertexSH.xy, IN.dynamicLightmapUV.xy, SH, inputData.normalWS);
				#else
				inputData.bakedGI = SAMPLE_GI( IN.lightmapUVOrVertexSH.xy, SH, inputData.normalWS );
				#endif

				#ifdef _ASE_BAKEDGI
					inputData.bakedGI = BakedGI;
				#endif
				
				inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(IN.clipPos);
				inputData.shadowMask = SAMPLE_SHADOWMASK(IN.lightmapUVOrVertexSH.xy);

				#if defined(DEBUG_DISPLAY)
					#if defined(DYNAMICLIGHTMAP_ON)
						inputData.dynamicLightmapUV = IN.dynamicLightmapUV.xy;
					#endif

					#if defined(LIGHTMAP_ON)
						inputData.staticLightmapUV = IN.lightmapUVOrVertexSH.xy;
					#else
						inputData.vertexSH = SH;
					#endif
				#endif

				SurfaceData surfaceData;
				surfaceData.albedo              = Albedo;
				surfaceData.metallic            = saturate(Metallic);
				surfaceData.specular            = Specular;
				surfaceData.smoothness          = saturate(Smoothness),
				surfaceData.occlusion           = Occlusion,
				surfaceData.emission            = Emission,
				surfaceData.alpha               = saturate(Alpha);
				surfaceData.normalTS            = Normal;
				surfaceData.clearCoatMask       = 0;
				surfaceData.clearCoatSmoothness = 1;


				#ifdef _CLEARCOAT
					surfaceData.clearCoatMask       = saturate(CoatMask);
					surfaceData.clearCoatSmoothness = saturate(CoatSmoothness);
				#endif

				#ifdef _DBUFFER
					ApplyDecalToSurfaceData(IN.clipPos, surfaceData, inputData);
				#endif

				half4 color = UniversalFragmentPBR( inputData, surfaceData);

				#ifdef _TRANSMISSION_ASE
				{
					float shadow = _TransmissionShadow;

					Light mainLight = GetMainLight( inputData.shadowCoord );
					float3 mainAtten = mainLight.color * mainLight.distanceAttenuation;
					mainAtten = lerp( mainAtten, mainAtten * mainLight.shadowAttenuation, shadow );
					half3 mainTransmission = max(0 , -dot(inputData.normalWS, mainLight.direction)) * mainAtten * Transmission;
					color.rgb += Albedo * mainTransmission;

					#ifdef _ADDITIONAL_LIGHTS
						int transPixelLightCount = GetAdditionalLightsCount();
						for (int i = 0; i < transPixelLightCount; ++i)
						{
							Light light = GetAdditionalLight(i, inputData.positionWS);
							float3 atten = light.color * light.distanceAttenuation;
							atten = lerp( atten, atten * light.shadowAttenuation, shadow );

							half3 transmission = max(0 , -dot(inputData.normalWS, light.direction)) * atten * Transmission;
							color.rgb += Albedo * transmission;
						}
					#endif
				}
				#endif

				#ifdef _TRANSLUCENCY_ASE
				{
					float shadow = _TransShadow;
					float normal = _TransNormal;
					float scattering = _TransScattering;
					float direct = _TransDirect;
					float ambient = _TransAmbient;
					float strength = _TransStrength;

					Light mainLight = GetMainLight( inputData.shadowCoord );
					float3 mainAtten = mainLight.color * mainLight.distanceAttenuation;
					mainAtten = lerp( mainAtten, mainAtten * mainLight.shadowAttenuation, shadow );

					half3 mainLightDir = mainLight.direction + inputData.normalWS * normal;
					half mainVdotL = pow( saturate( dot( inputData.viewDirectionWS, -mainLightDir ) ), scattering );
					half3 mainTranslucency = mainAtten * ( mainVdotL * direct + inputData.bakedGI * ambient ) * Translucency;
					color.rgb += Albedo * mainTranslucency * strength;

					#ifdef _ADDITIONAL_LIGHTS
						int transPixelLightCount = GetAdditionalLightsCount();
						for (int i = 0; i < transPixelLightCount; ++i)
						{
							Light light = GetAdditionalLight(i, inputData.positionWS);
							float3 atten = light.color * light.distanceAttenuation;
							atten = lerp( atten, atten * light.shadowAttenuation, shadow );

							half3 lightDir = light.direction + inputData.normalWS * normal;
							half VdotL = pow( saturate( dot( inputData.viewDirectionWS, -lightDir ) ), scattering );
							half3 translucency = atten * ( VdotL * direct + inputData.bakedGI * ambient ) * Translucency;
							color.rgb += Albedo * translucency * strength;
						}
					#endif
				}
				#endif

				#ifdef _REFRACTION_ASE
					float4 projScreenPos = ScreenPos / ScreenPos.w;
					float3 refractionOffset = ( RefractionIndex - 1.0 ) * mul( UNITY_MATRIX_V, float4( WorldNormal,0 ) ).xyz * ( 1.0 - dot( WorldNormal, WorldViewDirection ) );
					projScreenPos.xy += refractionOffset.xy;
					float3 refraction = SHADERGRAPH_SAMPLE_SCENE_COLOR( projScreenPos.xy ) * RefractionColor;
					color.rgb = lerp( refraction, color.rgb, color.a );
					color.a = 1;
				#endif

				#ifdef ASE_FINAL_COLOR_ALPHA_MULTIPLY
					color.rgb *= color.a;
				#endif

				#ifdef ASE_FOG
					#ifdef TERRAIN_SPLAT_ADDPASS
						color.rgb = MixFogColor(color.rgb, half3( 0, 0, 0 ), IN.fogFactorAndVertexLight.x );
					#else
						color.rgb = MixFog(color.rgb, IN.fogFactorAndVertexLight.x);
					#endif
				#endif

				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif

				return color;
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "ShadowCaster"
			Tags { "LightMode"="ShadowCaster" }

			ZWrite On
			ZTest LEqual
			AlphaToMask Off
			ColorMask 0

			HLSLPROGRAM
			
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _EMISSION
			#define ASE_SRP_VERSION 999999

			
			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile _ _CASTING_PUNCTUAL_LIGHT_SHADOW

			#define SHADERPASS SHADERPASS_SHADOWCASTER

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _GradientColor;
			float4 _Color16;
			float4 _MRE1;
			float4 _MRE2;
			float4 _MRE3;
			float4 _MRE4;
			float4 _MRE5;
			float4 _MRE6;
			float4 _MRE7;
			float4 _MRE8;
			float4 _MRE9;
			float4 _MRE10;
			float4 _MRE11;
			float4 _MRE12;
			float4 _MRE13;
			float4 _MRE14;
			float4 _Color15;
			float4 _MRE15;
			float4 _Color14;
			float4 _Color12;
			float4 _Color1;
			float4 _Color2;
			float4 _Color3;
			float4 _Color13;
			float4 _Color5;
			float4 _Color6;
			float4 _Color4;
			float4 _Color8;
			float4 _Color9;
			float4 _Color10;
			float4 _Color11;
			float4 _Color7;
			float4 _MRE16;
			float _EmissionPower1;
			float _GradientPower;
			float _GradientOffset;
			float _GradientScale;
			float _GradientIntensity;
			#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			

			
			float3 _LightDirection;
			float3 _LightPosition;

			VertexOutput VertexFunction( VertexInput v )
			{
				VertexOutput o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif
				float3 normalWS = TransformObjectToWorldDir(v.ase_normal);


			#if _CASTING_PUNCTUAL_LIGHT_SHADOW
				float3 lightDirectionWS = normalize(_LightPosition - positionWS);
			#else
				float3 lightDirectionWS = _LightDirection;
			#endif

				float4 clipPos = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));
			
			#if UNITY_REVERSED_Z
				clipPos.z = min(clipPos.z, UNITY_NEAR_CLIP_VALUE);
			#else
				clipPos.z = max(clipPos.z, UNITY_NEAR_CLIP_VALUE);
			#endif


				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = clipPos;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				o.clipPos = clipPos;
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)
				#define ASE_SV_DEPTH SV_DepthLessEqual  
			#else
				#define ASE_SV_DEPTH SV_Depth
			#endif

			half4 frag(	VertexOutput IN 
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );
				
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				
				float Alpha = 1;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;
				#ifdef ASE_DEPTH_WRITE_ON
				float DepthValue = 0;
				#endif

				#ifdef _ALPHATEST_ON
					#ifdef _ALPHATEST_SHADOW_ON
						clip(Alpha - AlphaClipThresholdShadow);
					#else
						clip(Alpha - AlphaClipThreshold);
					#endif
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif

				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif
				return 0;
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }

			ZWrite On
			ColorMask 0
			AlphaToMask Off

			HLSLPROGRAM
			
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _EMISSION
			#define ASE_SRP_VERSION 999999

			
			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS SHADERPASS_DEPTHONLY
        
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _GradientColor;
			float4 _Color16;
			float4 _MRE1;
			float4 _MRE2;
			float4 _MRE3;
			float4 _MRE4;
			float4 _MRE5;
			float4 _MRE6;
			float4 _MRE7;
			float4 _MRE8;
			float4 _MRE9;
			float4 _MRE10;
			float4 _MRE11;
			float4 _MRE12;
			float4 _MRE13;
			float4 _MRE14;
			float4 _Color15;
			float4 _MRE15;
			float4 _Color14;
			float4 _Color12;
			float4 _Color1;
			float4 _Color2;
			float4 _Color3;
			float4 _Color13;
			float4 _Color5;
			float4 _Color6;
			float4 _Color4;
			float4 _Color8;
			float4 _Color9;
			float4 _Color10;
			float4 _Color11;
			float4 _Color7;
			float4 _MRE16;
			float _EmissionPower1;
			float _GradientPower;
			float _GradientOffset;
			float _GradientScale;
			float _GradientIntensity;
			#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			

			
			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;
				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				o.clipPos = positionCS;
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)
				#define ASE_SV_DEPTH SV_DepthLessEqual  
			#else
				#define ASE_SV_DEPTH SV_Depth
			#endif
			half4 frag(	VertexOutput IN 
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				
				float Alpha = 1;
				float AlphaClipThreshold = 0.5;
				#ifdef ASE_DEPTH_WRITE_ON
				float DepthValue = 0;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif
				#ifdef ASE_DEPTH_WRITE_ON
				outputDepth = DepthValue;
				#endif

				return 0;
			}
			ENDHLSL
		}
		
		
		Pass
		{
			
			Name "Meta"
			Tags { "LightMode"="Meta" }

			Cull Off

			HLSLPROGRAM
			
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _EMISSION
			#define ASE_SRP_VERSION 999999

			
			#pragma vertex vert
			#pragma fragment frag

			#pragma shader_feature _ EDITOR_VISUALIZATION

			#define SHADERPASS SHADERPASS_META

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 texcoord0 : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				#ifdef EDITOR_VISUALIZATION
				float4 VizUV : TEXCOORD2;
				float4 LightCoord : TEXCOORD3;
				#endif
				float4 ase_texcoord4 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _GradientColor;
			float4 _Color16;
			float4 _MRE1;
			float4 _MRE2;
			float4 _MRE3;
			float4 _MRE4;
			float4 _MRE5;
			float4 _MRE6;
			float4 _MRE7;
			float4 _MRE8;
			float4 _MRE9;
			float4 _MRE10;
			float4 _MRE11;
			float4 _MRE12;
			float4 _MRE13;
			float4 _MRE14;
			float4 _Color15;
			float4 _MRE15;
			float4 _Color14;
			float4 _Color12;
			float4 _Color1;
			float4 _Color2;
			float4 _Color3;
			float4 _Color13;
			float4 _Color5;
			float4 _Color6;
			float4 _Color4;
			float4 _Color8;
			float4 _Color9;
			float4 _Color10;
			float4 _Color11;
			float4 _Color7;
			float4 _MRE16;
			float _EmissionPower1;
			float _GradientPower;
			float _GradientOffset;
			float _GradientScale;
			float _GradientIntensity;
			#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			sampler2D _Gradient;


			
			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.ase_texcoord4.xy = v.texcoord0.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord4.zw = 0;
				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				o.clipPos = MetaVertexPosition( v.vertex, v.texcoord1.xy, v.texcoord1.xy, unity_LightmapST, unity_DynamicLightmapST );

			#ifdef EDITOR_VISUALIZATION
				float2 VizUV = 0;
				float4 LightCoord = 0;
				UnityEditorVizData(v.vertex.xyz, v.texcoord0.xy, v.texcoord1.xy, v.texcoord2.xy, VizUV, LightCoord);
				o.VizUV = float4(VizUV, 0, 0);
				o.LightCoord = LightCoord;
			#endif

			#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				VertexPositionInputs vertexInput = (VertexPositionInputs)0;
				vertexInput.positionWS = positionWS;
				vertexInput.positionCS = o.clipPos;
				o.shadowCoord = GetShadowCoord( vertexInput );
			#endif
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 texcoord0 : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.texcoord0 = v.texcoord0;
				o.texcoord1 = v.texcoord1;
				o.texcoord2 = v.texcoord2;
				
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.texcoord0 = patch[0].texcoord0 * bary.x + patch[1].texcoord0 * bary.y + patch[2].texcoord0 * bary.z;
				o.texcoord1 = patch[0].texcoord1 * bary.x + patch[1].texcoord1 * bary.y + patch[2].texcoord1 * bary.z;
				o.texcoord2 = patch[0].texcoord2 * bary.x + patch[1].texcoord2 * bary.y + patch[2].texcoord2 * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 texCoord256 = IN.ase_texcoord4.xy * float2( 1,4 ) + float2( 0,0 );
				float4 clampResult328 = clamp( ( ( tex2D( _Gradient, texCoord256 ) + _GradientColor ) + ( 1.0 - _GradientIntensity ) ) , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
				float4 saferPower342 = abs( (clampResult328*_GradientScale + _GradientOffset) );
				float4 temp_cast_0 = (_GradientPower).xxxx;
				float2 texCoord2_g760 = IN.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g760 = 1.0;
				float temp_output_7_0_g760 = 4.0;
				float temp_output_9_0_g760 = 4.0;
				float temp_output_8_0_g760 = 4.0;
				float2 texCoord2_g754 = IN.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g754 = 2.0;
				float temp_output_7_0_g754 = 4.0;
				float temp_output_9_0_g754 = 4.0;
				float temp_output_8_0_g754 = 4.0;
				float2 texCoord2_g755 = IN.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g755 = 3.0;
				float temp_output_7_0_g755 = 4.0;
				float temp_output_9_0_g755 = 4.0;
				float temp_output_8_0_g755 = 4.0;
				float2 texCoord2_g757 = IN.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g757 = 4.0;
				float temp_output_7_0_g757 = 4.0;
				float temp_output_9_0_g757 = 4.0;
				float temp_output_8_0_g757 = 4.0;
				float2 texCoord2_g750 = IN.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g750 = 1.0;
				float temp_output_7_0_g750 = 4.0;
				float temp_output_9_0_g750 = 3.0;
				float temp_output_8_0_g750 = 4.0;
				float2 texCoord2_g745 = IN.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g745 = 2.0;
				float temp_output_7_0_g745 = 4.0;
				float temp_output_9_0_g745 = 3.0;
				float temp_output_8_0_g745 = 4.0;
				float2 texCoord2_g752 = IN.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g752 = 3.0;
				float temp_output_7_0_g752 = 4.0;
				float temp_output_9_0_g752 = 3.0;
				float temp_output_8_0_g752 = 4.0;
				float2 texCoord2_g747 = IN.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g747 = 4.0;
				float temp_output_7_0_g747 = 4.0;
				float temp_output_9_0_g747 = 3.0;
				float temp_output_8_0_g747 = 4.0;
				float2 texCoord2_g746 = IN.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g746 = 1.0;
				float temp_output_7_0_g746 = 4.0;
				float temp_output_9_0_g746 = 2.0;
				float temp_output_8_0_g746 = 4.0;
				float2 texCoord2_g756 = IN.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g756 = 2.0;
				float temp_output_7_0_g756 = 4.0;
				float temp_output_9_0_g756 = 2.0;
				float temp_output_8_0_g756 = 4.0;
				float2 texCoord2_g748 = IN.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g748 = 3.0;
				float temp_output_7_0_g748 = 4.0;
				float temp_output_9_0_g748 = 2.0;
				float temp_output_8_0_g748 = 4.0;
				float2 texCoord2_g759 = IN.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g759 = 4.0;
				float temp_output_7_0_g759 = 4.0;
				float temp_output_9_0_g759 = 2.0;
				float temp_output_8_0_g759 = 4.0;
				float2 texCoord2_g758 = IN.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g758 = 1.0;
				float temp_output_7_0_g758 = 4.0;
				float temp_output_9_0_g758 = 1.0;
				float temp_output_8_0_g758 = 4.0;
				float2 texCoord2_g751 = IN.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g751 = 2.0;
				float temp_output_7_0_g751 = 4.0;
				float temp_output_9_0_g751 = 1.0;
				float temp_output_8_0_g751 = 4.0;
				float2 texCoord2_g753 = IN.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g753 = 3.0;
				float temp_output_7_0_g753 = 4.0;
				float temp_output_9_0_g753 = 1.0;
				float temp_output_8_0_g753 = 4.0;
				float2 texCoord2_g749 = IN.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g749 = 4.0;
				float temp_output_7_0_g749 = 4.0;
				float temp_output_9_0_g749 = 1.0;
				float temp_output_8_0_g749 = 4.0;
				float4 temp_output_329_0 = ( ( ( _Color1 * ( ( ( 1.0 - step( texCoord2_g760.x , ( ( temp_output_3_0_g760 - 1.0 ) / temp_output_7_0_g760 ) ) ) * ( step( texCoord2_g760.x , ( temp_output_3_0_g760 / temp_output_7_0_g760 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g760.y , ( ( temp_output_9_0_g760 - 1.0 ) / temp_output_8_0_g760 ) ) ) * ( step( texCoord2_g760.y , ( temp_output_9_0_g760 / temp_output_8_0_g760 ) ) * 1.0 ) ) ) ) + ( _Color2 * ( ( ( 1.0 - step( texCoord2_g754.x , ( ( temp_output_3_0_g754 - 1.0 ) / temp_output_7_0_g754 ) ) ) * ( step( texCoord2_g754.x , ( temp_output_3_0_g754 / temp_output_7_0_g754 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g754.y , ( ( temp_output_9_0_g754 - 1.0 ) / temp_output_8_0_g754 ) ) ) * ( step( texCoord2_g754.y , ( temp_output_9_0_g754 / temp_output_8_0_g754 ) ) * 1.0 ) ) ) ) + ( _Color3 * ( ( ( 1.0 - step( texCoord2_g755.x , ( ( temp_output_3_0_g755 - 1.0 ) / temp_output_7_0_g755 ) ) ) * ( step( texCoord2_g755.x , ( temp_output_3_0_g755 / temp_output_7_0_g755 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g755.y , ( ( temp_output_9_0_g755 - 1.0 ) / temp_output_8_0_g755 ) ) ) * ( step( texCoord2_g755.y , ( temp_output_9_0_g755 / temp_output_8_0_g755 ) ) * 1.0 ) ) ) ) + ( _Color4 * ( ( ( 1.0 - step( texCoord2_g757.x , ( ( temp_output_3_0_g757 - 1.0 ) / temp_output_7_0_g757 ) ) ) * ( step( texCoord2_g757.x , ( temp_output_3_0_g757 / temp_output_7_0_g757 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g757.y , ( ( temp_output_9_0_g757 - 1.0 ) / temp_output_8_0_g757 ) ) ) * ( step( texCoord2_g757.y , ( temp_output_9_0_g757 / temp_output_8_0_g757 ) ) * 1.0 ) ) ) ) ) + ( ( _Color5 * ( ( ( 1.0 - step( texCoord2_g750.x , ( ( temp_output_3_0_g750 - 1.0 ) / temp_output_7_0_g750 ) ) ) * ( step( texCoord2_g750.x , ( temp_output_3_0_g750 / temp_output_7_0_g750 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g750.y , ( ( temp_output_9_0_g750 - 1.0 ) / temp_output_8_0_g750 ) ) ) * ( step( texCoord2_g750.y , ( temp_output_9_0_g750 / temp_output_8_0_g750 ) ) * 1.0 ) ) ) ) + ( _Color6 * ( ( ( 1.0 - step( texCoord2_g745.x , ( ( temp_output_3_0_g745 - 1.0 ) / temp_output_7_0_g745 ) ) ) * ( step( texCoord2_g745.x , ( temp_output_3_0_g745 / temp_output_7_0_g745 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g745.y , ( ( temp_output_9_0_g745 - 1.0 ) / temp_output_8_0_g745 ) ) ) * ( step( texCoord2_g745.y , ( temp_output_9_0_g745 / temp_output_8_0_g745 ) ) * 1.0 ) ) ) ) + ( _Color7 * ( ( ( 1.0 - step( texCoord2_g752.x , ( ( temp_output_3_0_g752 - 1.0 ) / temp_output_7_0_g752 ) ) ) * ( step( texCoord2_g752.x , ( temp_output_3_0_g752 / temp_output_7_0_g752 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g752.y , ( ( temp_output_9_0_g752 - 1.0 ) / temp_output_8_0_g752 ) ) ) * ( step( texCoord2_g752.y , ( temp_output_9_0_g752 / temp_output_8_0_g752 ) ) * 1.0 ) ) ) ) + ( _Color8 * ( ( ( 1.0 - step( texCoord2_g747.x , ( ( temp_output_3_0_g747 - 1.0 ) / temp_output_7_0_g747 ) ) ) * ( step( texCoord2_g747.x , ( temp_output_3_0_g747 / temp_output_7_0_g747 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g747.y , ( ( temp_output_9_0_g747 - 1.0 ) / temp_output_8_0_g747 ) ) ) * ( step( texCoord2_g747.y , ( temp_output_9_0_g747 / temp_output_8_0_g747 ) ) * 1.0 ) ) ) ) ) + ( ( _Color9 * ( ( ( 1.0 - step( texCoord2_g746.x , ( ( temp_output_3_0_g746 - 1.0 ) / temp_output_7_0_g746 ) ) ) * ( step( texCoord2_g746.x , ( temp_output_3_0_g746 / temp_output_7_0_g746 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g746.y , ( ( temp_output_9_0_g746 - 1.0 ) / temp_output_8_0_g746 ) ) ) * ( step( texCoord2_g746.y , ( temp_output_9_0_g746 / temp_output_8_0_g746 ) ) * 1.0 ) ) ) ) + ( _Color10 * ( ( ( 1.0 - step( texCoord2_g756.x , ( ( temp_output_3_0_g756 - 1.0 ) / temp_output_7_0_g756 ) ) ) * ( step( texCoord2_g756.x , ( temp_output_3_0_g756 / temp_output_7_0_g756 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g756.y , ( ( temp_output_9_0_g756 - 1.0 ) / temp_output_8_0_g756 ) ) ) * ( step( texCoord2_g756.y , ( temp_output_9_0_g756 / temp_output_8_0_g756 ) ) * 1.0 ) ) ) ) + ( _Color11 * ( ( ( 1.0 - step( texCoord2_g748.x , ( ( temp_output_3_0_g748 - 1.0 ) / temp_output_7_0_g748 ) ) ) * ( step( texCoord2_g748.x , ( temp_output_3_0_g748 / temp_output_7_0_g748 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g748.y , ( ( temp_output_9_0_g748 - 1.0 ) / temp_output_8_0_g748 ) ) ) * ( step( texCoord2_g748.y , ( temp_output_9_0_g748 / temp_output_8_0_g748 ) ) * 1.0 ) ) ) ) + ( _Color12 * ( ( ( 1.0 - step( texCoord2_g759.x , ( ( temp_output_3_0_g759 - 1.0 ) / temp_output_7_0_g759 ) ) ) * ( step( texCoord2_g759.x , ( temp_output_3_0_g759 / temp_output_7_0_g759 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g759.y , ( ( temp_output_9_0_g759 - 1.0 ) / temp_output_8_0_g759 ) ) ) * ( step( texCoord2_g759.y , ( temp_output_9_0_g759 / temp_output_8_0_g759 ) ) * 1.0 ) ) ) ) ) + ( ( _Color13 * ( ( ( 1.0 - step( texCoord2_g758.x , ( ( temp_output_3_0_g758 - 1.0 ) / temp_output_7_0_g758 ) ) ) * ( step( texCoord2_g758.x , ( temp_output_3_0_g758 / temp_output_7_0_g758 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g758.y , ( ( temp_output_9_0_g758 - 1.0 ) / temp_output_8_0_g758 ) ) ) * ( step( texCoord2_g758.y , ( temp_output_9_0_g758 / temp_output_8_0_g758 ) ) * 1.0 ) ) ) ) + ( _Color14 * ( ( ( 1.0 - step( texCoord2_g751.x , ( ( temp_output_3_0_g751 - 1.0 ) / temp_output_7_0_g751 ) ) ) * ( step( texCoord2_g751.x , ( temp_output_3_0_g751 / temp_output_7_0_g751 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g751.y , ( ( temp_output_9_0_g751 - 1.0 ) / temp_output_8_0_g751 ) ) ) * ( step( texCoord2_g751.y , ( temp_output_9_0_g751 / temp_output_8_0_g751 ) ) * 1.0 ) ) ) ) + ( _Color15 * ( ( ( 1.0 - step( texCoord2_g753.x , ( ( temp_output_3_0_g753 - 1.0 ) / temp_output_7_0_g753 ) ) ) * ( step( texCoord2_g753.x , ( temp_output_3_0_g753 / temp_output_7_0_g753 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g753.y , ( ( temp_output_9_0_g753 - 1.0 ) / temp_output_8_0_g753 ) ) ) * ( step( texCoord2_g753.y , ( temp_output_9_0_g753 / temp_output_8_0_g753 ) ) * 1.0 ) ) ) ) + ( _Color16 * ( ( ( 1.0 - step( texCoord2_g749.x , ( ( temp_output_3_0_g749 - 1.0 ) / temp_output_7_0_g749 ) ) ) * ( step( texCoord2_g749.x , ( temp_output_3_0_g749 / temp_output_7_0_g749 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g749.y , ( ( temp_output_9_0_g749 - 1.0 ) / temp_output_8_0_g749 ) ) ) * ( step( texCoord2_g749.y , ( temp_output_9_0_g749 / temp_output_8_0_g749 ) ) * 1.0 ) ) ) ) ) );
				float4 clampResult348 = clamp( ( pow( saferPower342 , temp_cast_0 ) + ( 1.0 - (temp_output_329_0).a ) ) , float4( 0,0,0,0 ) , float4( 1,1,1,1 ) );
				
				float2 texCoord2_g731 = IN.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g731 = 1.0;
				float temp_output_7_0_g731 = 4.0;
				float temp_output_9_0_g731 = 4.0;
				float temp_output_8_0_g731 = 4.0;
				float2 texCoord2_g744 = IN.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g744 = 2.0;
				float temp_output_7_0_g744 = 4.0;
				float temp_output_9_0_g744 = 4.0;
				float temp_output_8_0_g744 = 4.0;
				float2 texCoord2_g732 = IN.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g732 = 3.0;
				float temp_output_7_0_g732 = 4.0;
				float temp_output_9_0_g732 = 4.0;
				float temp_output_8_0_g732 = 4.0;
				float2 texCoord2_g741 = IN.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g741 = 4.0;
				float temp_output_7_0_g741 = 4.0;
				float temp_output_9_0_g741 = 4.0;
				float temp_output_8_0_g741 = 4.0;
				float2 texCoord2_g739 = IN.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g739 = 1.0;
				float temp_output_7_0_g739 = 4.0;
				float temp_output_9_0_g739 = 3.0;
				float temp_output_8_0_g739 = 4.0;
				float2 texCoord2_g733 = IN.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g733 = 2.0;
				float temp_output_7_0_g733 = 4.0;
				float temp_output_9_0_g733 = 3.0;
				float temp_output_8_0_g733 = 4.0;
				float2 texCoord2_g743 = IN.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g743 = 3.0;
				float temp_output_7_0_g743 = 4.0;
				float temp_output_9_0_g743 = 3.0;
				float temp_output_8_0_g743 = 4.0;
				float2 texCoord2_g736 = IN.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g736 = 4.0;
				float temp_output_7_0_g736 = 4.0;
				float temp_output_9_0_g736 = 3.0;
				float temp_output_8_0_g736 = 4.0;
				float2 texCoord2_g742 = IN.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g742 = 1.0;
				float temp_output_7_0_g742 = 4.0;
				float temp_output_9_0_g742 = 2.0;
				float temp_output_8_0_g742 = 4.0;
				float2 texCoord2_g735 = IN.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g735 = 2.0;
				float temp_output_7_0_g735 = 4.0;
				float temp_output_9_0_g735 = 2.0;
				float temp_output_8_0_g735 = 4.0;
				float2 texCoord2_g729 = IN.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g729 = 3.0;
				float temp_output_7_0_g729 = 4.0;
				float temp_output_9_0_g729 = 2.0;
				float temp_output_8_0_g729 = 4.0;
				float2 texCoord2_g730 = IN.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g730 = 4.0;
				float temp_output_7_0_g730 = 4.0;
				float temp_output_9_0_g730 = 2.0;
				float temp_output_8_0_g730 = 4.0;
				float2 texCoord2_g738 = IN.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g738 = 1.0;
				float temp_output_7_0_g738 = 4.0;
				float temp_output_9_0_g738 = 1.0;
				float temp_output_8_0_g738 = 4.0;
				float2 texCoord2_g737 = IN.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g737 = 2.0;
				float temp_output_7_0_g737 = 4.0;
				float temp_output_9_0_g737 = 1.0;
				float temp_output_8_0_g737 = 4.0;
				float2 texCoord2_g734 = IN.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g734 = 3.0;
				float temp_output_7_0_g734 = 4.0;
				float temp_output_9_0_g734 = 1.0;
				float temp_output_8_0_g734 = 4.0;
				float2 texCoord2_g740 = IN.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g740 = 4.0;
				float temp_output_7_0_g740 = 4.0;
				float temp_output_9_0_g740 = 1.0;
				float temp_output_8_0_g740 = 4.0;
				float4 temp_output_344_0 = ( ( ( _MRE1 * ( ( ( 1.0 - step( texCoord2_g731.x , ( ( temp_output_3_0_g731 - 1.0 ) / temp_output_7_0_g731 ) ) ) * ( step( texCoord2_g731.x , ( temp_output_3_0_g731 / temp_output_7_0_g731 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g731.y , ( ( temp_output_9_0_g731 - 1.0 ) / temp_output_8_0_g731 ) ) ) * ( step( texCoord2_g731.y , ( temp_output_9_0_g731 / temp_output_8_0_g731 ) ) * 1.0 ) ) ) ) + ( _MRE2 * ( ( ( 1.0 - step( texCoord2_g744.x , ( ( temp_output_3_0_g744 - 1.0 ) / temp_output_7_0_g744 ) ) ) * ( step( texCoord2_g744.x , ( temp_output_3_0_g744 / temp_output_7_0_g744 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g744.y , ( ( temp_output_9_0_g744 - 1.0 ) / temp_output_8_0_g744 ) ) ) * ( step( texCoord2_g744.y , ( temp_output_9_0_g744 / temp_output_8_0_g744 ) ) * 1.0 ) ) ) ) + ( _MRE3 * ( ( ( 1.0 - step( texCoord2_g732.x , ( ( temp_output_3_0_g732 - 1.0 ) / temp_output_7_0_g732 ) ) ) * ( step( texCoord2_g732.x , ( temp_output_3_0_g732 / temp_output_7_0_g732 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g732.y , ( ( temp_output_9_0_g732 - 1.0 ) / temp_output_8_0_g732 ) ) ) * ( step( texCoord2_g732.y , ( temp_output_9_0_g732 / temp_output_8_0_g732 ) ) * 1.0 ) ) ) ) + ( _MRE4 * ( ( ( 1.0 - step( texCoord2_g741.x , ( ( temp_output_3_0_g741 - 1.0 ) / temp_output_7_0_g741 ) ) ) * ( step( texCoord2_g741.x , ( temp_output_3_0_g741 / temp_output_7_0_g741 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g741.y , ( ( temp_output_9_0_g741 - 1.0 ) / temp_output_8_0_g741 ) ) ) * ( step( texCoord2_g741.y , ( temp_output_9_0_g741 / temp_output_8_0_g741 ) ) * 1.0 ) ) ) ) ) + ( ( _MRE5 * ( ( ( 1.0 - step( texCoord2_g739.x , ( ( temp_output_3_0_g739 - 1.0 ) / temp_output_7_0_g739 ) ) ) * ( step( texCoord2_g739.x , ( temp_output_3_0_g739 / temp_output_7_0_g739 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g739.y , ( ( temp_output_9_0_g739 - 1.0 ) / temp_output_8_0_g739 ) ) ) * ( step( texCoord2_g739.y , ( temp_output_9_0_g739 / temp_output_8_0_g739 ) ) * 1.0 ) ) ) ) + ( _MRE6 * ( ( ( 1.0 - step( texCoord2_g733.x , ( ( temp_output_3_0_g733 - 1.0 ) / temp_output_7_0_g733 ) ) ) * ( step( texCoord2_g733.x , ( temp_output_3_0_g733 / temp_output_7_0_g733 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g733.y , ( ( temp_output_9_0_g733 - 1.0 ) / temp_output_8_0_g733 ) ) ) * ( step( texCoord2_g733.y , ( temp_output_9_0_g733 / temp_output_8_0_g733 ) ) * 1.0 ) ) ) ) + ( _MRE7 * ( ( ( 1.0 - step( texCoord2_g743.x , ( ( temp_output_3_0_g743 - 1.0 ) / temp_output_7_0_g743 ) ) ) * ( step( texCoord2_g743.x , ( temp_output_3_0_g743 / temp_output_7_0_g743 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g743.y , ( ( temp_output_9_0_g743 - 1.0 ) / temp_output_8_0_g743 ) ) ) * ( step( texCoord2_g743.y , ( temp_output_9_0_g743 / temp_output_8_0_g743 ) ) * 1.0 ) ) ) ) + ( _MRE8 * ( ( ( 1.0 - step( texCoord2_g736.x , ( ( temp_output_3_0_g736 - 1.0 ) / temp_output_7_0_g736 ) ) ) * ( step( texCoord2_g736.x , ( temp_output_3_0_g736 / temp_output_7_0_g736 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g736.y , ( ( temp_output_9_0_g736 - 1.0 ) / temp_output_8_0_g736 ) ) ) * ( step( texCoord2_g736.y , ( temp_output_9_0_g736 / temp_output_8_0_g736 ) ) * 1.0 ) ) ) ) ) + ( ( _MRE9 * ( ( ( 1.0 - step( texCoord2_g742.x , ( ( temp_output_3_0_g742 - 1.0 ) / temp_output_7_0_g742 ) ) ) * ( step( texCoord2_g742.x , ( temp_output_3_0_g742 / temp_output_7_0_g742 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g742.y , ( ( temp_output_9_0_g742 - 1.0 ) / temp_output_8_0_g742 ) ) ) * ( step( texCoord2_g742.y , ( temp_output_9_0_g742 / temp_output_8_0_g742 ) ) * 1.0 ) ) ) ) + ( _MRE10 * ( ( ( 1.0 - step( texCoord2_g735.x , ( ( temp_output_3_0_g735 - 1.0 ) / temp_output_7_0_g735 ) ) ) * ( step( texCoord2_g735.x , ( temp_output_3_0_g735 / temp_output_7_0_g735 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g735.y , ( ( temp_output_9_0_g735 - 1.0 ) / temp_output_8_0_g735 ) ) ) * ( step( texCoord2_g735.y , ( temp_output_9_0_g735 / temp_output_8_0_g735 ) ) * 1.0 ) ) ) ) + ( _MRE11 * ( ( ( 1.0 - step( texCoord2_g729.x , ( ( temp_output_3_0_g729 - 1.0 ) / temp_output_7_0_g729 ) ) ) * ( step( texCoord2_g729.x , ( temp_output_3_0_g729 / temp_output_7_0_g729 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g729.y , ( ( temp_output_9_0_g729 - 1.0 ) / temp_output_8_0_g729 ) ) ) * ( step( texCoord2_g729.y , ( temp_output_9_0_g729 / temp_output_8_0_g729 ) ) * 1.0 ) ) ) ) + ( _MRE12 * ( ( ( 1.0 - step( texCoord2_g730.x , ( ( temp_output_3_0_g730 - 1.0 ) / temp_output_7_0_g730 ) ) ) * ( step( texCoord2_g730.x , ( temp_output_3_0_g730 / temp_output_7_0_g730 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g730.y , ( ( temp_output_9_0_g730 - 1.0 ) / temp_output_8_0_g730 ) ) ) * ( step( texCoord2_g730.y , ( temp_output_9_0_g730 / temp_output_8_0_g730 ) ) * 1.0 ) ) ) ) ) + ( ( _MRE13 * ( ( ( 1.0 - step( texCoord2_g738.x , ( ( temp_output_3_0_g738 - 1.0 ) / temp_output_7_0_g738 ) ) ) * ( step( texCoord2_g738.x , ( temp_output_3_0_g738 / temp_output_7_0_g738 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g738.y , ( ( temp_output_9_0_g738 - 1.0 ) / temp_output_8_0_g738 ) ) ) * ( step( texCoord2_g738.y , ( temp_output_9_0_g738 / temp_output_8_0_g738 ) ) * 1.0 ) ) ) ) + ( _MRE14 * ( ( ( 1.0 - step( texCoord2_g737.x , ( ( temp_output_3_0_g737 - 1.0 ) / temp_output_7_0_g737 ) ) ) * ( step( texCoord2_g737.x , ( temp_output_3_0_g737 / temp_output_7_0_g737 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g737.y , ( ( temp_output_9_0_g737 - 1.0 ) / temp_output_8_0_g737 ) ) ) * ( step( texCoord2_g737.y , ( temp_output_9_0_g737 / temp_output_8_0_g737 ) ) * 1.0 ) ) ) ) + ( _MRE15 * ( ( ( 1.0 - step( texCoord2_g734.x , ( ( temp_output_3_0_g734 - 1.0 ) / temp_output_7_0_g734 ) ) ) * ( step( texCoord2_g734.x , ( temp_output_3_0_g734 / temp_output_7_0_g734 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g734.y , ( ( temp_output_9_0_g734 - 1.0 ) / temp_output_8_0_g734 ) ) ) * ( step( texCoord2_g734.y , ( temp_output_9_0_g734 / temp_output_8_0_g734 ) ) * 1.0 ) ) ) ) + ( _MRE16 * ( ( ( 1.0 - step( texCoord2_g740.x , ( ( temp_output_3_0_g740 - 1.0 ) / temp_output_7_0_g740 ) ) ) * ( step( texCoord2_g740.x , ( temp_output_3_0_g740 / temp_output_7_0_g740 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g740.y , ( ( temp_output_9_0_g740 - 1.0 ) / temp_output_8_0_g740 ) ) ) * ( step( texCoord2_g740.y , ( temp_output_9_0_g740 / temp_output_8_0_g740 ) ) * 1.0 ) ) ) ) ) );
				
				
				float3 Albedo = ( clampResult348 * temp_output_329_0 ).rgb;
				float3 Emission = ( temp_output_329_0 * ( _EmissionPower1 * (temp_output_344_0).b ) ).rgb;
				float Alpha = 1;
				float AlphaClipThreshold = 0.5;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				MetaInput metaInput = (MetaInput)0;
				metaInput.Albedo = Albedo;
				metaInput.Emission = Emission;
			#ifdef EDITOR_VISUALIZATION
				metaInput.VizUV = IN.VizUV.xy;
				metaInput.LightCoord = IN.LightCoord;
			#endif
				
				return MetaFragment(metaInput);
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "Universal2D"
			Tags { "LightMode"="Universal2D" }

			Blend One Zero, One Zero
			ColorMask RGBA

			HLSLPROGRAM
			
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _EMISSION
			#define ASE_SRP_VERSION 999999

			
			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS SHADERPASS_2D
        
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
			
			

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _GradientColor;
			float4 _Color16;
			float4 _MRE1;
			float4 _MRE2;
			float4 _MRE3;
			float4 _MRE4;
			float4 _MRE5;
			float4 _MRE6;
			float4 _MRE7;
			float4 _MRE8;
			float4 _MRE9;
			float4 _MRE10;
			float4 _MRE11;
			float4 _MRE12;
			float4 _MRE13;
			float4 _MRE14;
			float4 _Color15;
			float4 _MRE15;
			float4 _Color14;
			float4 _Color12;
			float4 _Color1;
			float4 _Color2;
			float4 _Color3;
			float4 _Color13;
			float4 _Color5;
			float4 _Color6;
			float4 _Color4;
			float4 _Color8;
			float4 _Color9;
			float4 _Color10;
			float4 _Color11;
			float4 _Color7;
			float4 _MRE16;
			float _EmissionPower1;
			float _GradientPower;
			float _GradientOffset;
			float _GradientScale;
			float _GradientIntensity;
			#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			sampler2D _Gradient;


			
			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				o.ase_texcoord2.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.zw = 0;
				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				o.clipPos = positionCS;
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_texcoord = v.ase_texcoord;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 texCoord256 = IN.ase_texcoord2.xy * float2( 1,4 ) + float2( 0,0 );
				float4 clampResult328 = clamp( ( ( tex2D( _Gradient, texCoord256 ) + _GradientColor ) + ( 1.0 - _GradientIntensity ) ) , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
				float4 saferPower342 = abs( (clampResult328*_GradientScale + _GradientOffset) );
				float4 temp_cast_0 = (_GradientPower).xxxx;
				float2 texCoord2_g760 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g760 = 1.0;
				float temp_output_7_0_g760 = 4.0;
				float temp_output_9_0_g760 = 4.0;
				float temp_output_8_0_g760 = 4.0;
				float2 texCoord2_g754 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g754 = 2.0;
				float temp_output_7_0_g754 = 4.0;
				float temp_output_9_0_g754 = 4.0;
				float temp_output_8_0_g754 = 4.0;
				float2 texCoord2_g755 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g755 = 3.0;
				float temp_output_7_0_g755 = 4.0;
				float temp_output_9_0_g755 = 4.0;
				float temp_output_8_0_g755 = 4.0;
				float2 texCoord2_g757 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g757 = 4.0;
				float temp_output_7_0_g757 = 4.0;
				float temp_output_9_0_g757 = 4.0;
				float temp_output_8_0_g757 = 4.0;
				float2 texCoord2_g750 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g750 = 1.0;
				float temp_output_7_0_g750 = 4.0;
				float temp_output_9_0_g750 = 3.0;
				float temp_output_8_0_g750 = 4.0;
				float2 texCoord2_g745 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g745 = 2.0;
				float temp_output_7_0_g745 = 4.0;
				float temp_output_9_0_g745 = 3.0;
				float temp_output_8_0_g745 = 4.0;
				float2 texCoord2_g752 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g752 = 3.0;
				float temp_output_7_0_g752 = 4.0;
				float temp_output_9_0_g752 = 3.0;
				float temp_output_8_0_g752 = 4.0;
				float2 texCoord2_g747 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g747 = 4.0;
				float temp_output_7_0_g747 = 4.0;
				float temp_output_9_0_g747 = 3.0;
				float temp_output_8_0_g747 = 4.0;
				float2 texCoord2_g746 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g746 = 1.0;
				float temp_output_7_0_g746 = 4.0;
				float temp_output_9_0_g746 = 2.0;
				float temp_output_8_0_g746 = 4.0;
				float2 texCoord2_g756 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g756 = 2.0;
				float temp_output_7_0_g756 = 4.0;
				float temp_output_9_0_g756 = 2.0;
				float temp_output_8_0_g756 = 4.0;
				float2 texCoord2_g748 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g748 = 3.0;
				float temp_output_7_0_g748 = 4.0;
				float temp_output_9_0_g748 = 2.0;
				float temp_output_8_0_g748 = 4.0;
				float2 texCoord2_g759 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g759 = 4.0;
				float temp_output_7_0_g759 = 4.0;
				float temp_output_9_0_g759 = 2.0;
				float temp_output_8_0_g759 = 4.0;
				float2 texCoord2_g758 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g758 = 1.0;
				float temp_output_7_0_g758 = 4.0;
				float temp_output_9_0_g758 = 1.0;
				float temp_output_8_0_g758 = 4.0;
				float2 texCoord2_g751 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g751 = 2.0;
				float temp_output_7_0_g751 = 4.0;
				float temp_output_9_0_g751 = 1.0;
				float temp_output_8_0_g751 = 4.0;
				float2 texCoord2_g753 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g753 = 3.0;
				float temp_output_7_0_g753 = 4.0;
				float temp_output_9_0_g753 = 1.0;
				float temp_output_8_0_g753 = 4.0;
				float2 texCoord2_g749 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g749 = 4.0;
				float temp_output_7_0_g749 = 4.0;
				float temp_output_9_0_g749 = 1.0;
				float temp_output_8_0_g749 = 4.0;
				float4 temp_output_329_0 = ( ( ( _Color1 * ( ( ( 1.0 - step( texCoord2_g760.x , ( ( temp_output_3_0_g760 - 1.0 ) / temp_output_7_0_g760 ) ) ) * ( step( texCoord2_g760.x , ( temp_output_3_0_g760 / temp_output_7_0_g760 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g760.y , ( ( temp_output_9_0_g760 - 1.0 ) / temp_output_8_0_g760 ) ) ) * ( step( texCoord2_g760.y , ( temp_output_9_0_g760 / temp_output_8_0_g760 ) ) * 1.0 ) ) ) ) + ( _Color2 * ( ( ( 1.0 - step( texCoord2_g754.x , ( ( temp_output_3_0_g754 - 1.0 ) / temp_output_7_0_g754 ) ) ) * ( step( texCoord2_g754.x , ( temp_output_3_0_g754 / temp_output_7_0_g754 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g754.y , ( ( temp_output_9_0_g754 - 1.0 ) / temp_output_8_0_g754 ) ) ) * ( step( texCoord2_g754.y , ( temp_output_9_0_g754 / temp_output_8_0_g754 ) ) * 1.0 ) ) ) ) + ( _Color3 * ( ( ( 1.0 - step( texCoord2_g755.x , ( ( temp_output_3_0_g755 - 1.0 ) / temp_output_7_0_g755 ) ) ) * ( step( texCoord2_g755.x , ( temp_output_3_0_g755 / temp_output_7_0_g755 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g755.y , ( ( temp_output_9_0_g755 - 1.0 ) / temp_output_8_0_g755 ) ) ) * ( step( texCoord2_g755.y , ( temp_output_9_0_g755 / temp_output_8_0_g755 ) ) * 1.0 ) ) ) ) + ( _Color4 * ( ( ( 1.0 - step( texCoord2_g757.x , ( ( temp_output_3_0_g757 - 1.0 ) / temp_output_7_0_g757 ) ) ) * ( step( texCoord2_g757.x , ( temp_output_3_0_g757 / temp_output_7_0_g757 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g757.y , ( ( temp_output_9_0_g757 - 1.0 ) / temp_output_8_0_g757 ) ) ) * ( step( texCoord2_g757.y , ( temp_output_9_0_g757 / temp_output_8_0_g757 ) ) * 1.0 ) ) ) ) ) + ( ( _Color5 * ( ( ( 1.0 - step( texCoord2_g750.x , ( ( temp_output_3_0_g750 - 1.0 ) / temp_output_7_0_g750 ) ) ) * ( step( texCoord2_g750.x , ( temp_output_3_0_g750 / temp_output_7_0_g750 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g750.y , ( ( temp_output_9_0_g750 - 1.0 ) / temp_output_8_0_g750 ) ) ) * ( step( texCoord2_g750.y , ( temp_output_9_0_g750 / temp_output_8_0_g750 ) ) * 1.0 ) ) ) ) + ( _Color6 * ( ( ( 1.0 - step( texCoord2_g745.x , ( ( temp_output_3_0_g745 - 1.0 ) / temp_output_7_0_g745 ) ) ) * ( step( texCoord2_g745.x , ( temp_output_3_0_g745 / temp_output_7_0_g745 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g745.y , ( ( temp_output_9_0_g745 - 1.0 ) / temp_output_8_0_g745 ) ) ) * ( step( texCoord2_g745.y , ( temp_output_9_0_g745 / temp_output_8_0_g745 ) ) * 1.0 ) ) ) ) + ( _Color7 * ( ( ( 1.0 - step( texCoord2_g752.x , ( ( temp_output_3_0_g752 - 1.0 ) / temp_output_7_0_g752 ) ) ) * ( step( texCoord2_g752.x , ( temp_output_3_0_g752 / temp_output_7_0_g752 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g752.y , ( ( temp_output_9_0_g752 - 1.0 ) / temp_output_8_0_g752 ) ) ) * ( step( texCoord2_g752.y , ( temp_output_9_0_g752 / temp_output_8_0_g752 ) ) * 1.0 ) ) ) ) + ( _Color8 * ( ( ( 1.0 - step( texCoord2_g747.x , ( ( temp_output_3_0_g747 - 1.0 ) / temp_output_7_0_g747 ) ) ) * ( step( texCoord2_g747.x , ( temp_output_3_0_g747 / temp_output_7_0_g747 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g747.y , ( ( temp_output_9_0_g747 - 1.0 ) / temp_output_8_0_g747 ) ) ) * ( step( texCoord2_g747.y , ( temp_output_9_0_g747 / temp_output_8_0_g747 ) ) * 1.0 ) ) ) ) ) + ( ( _Color9 * ( ( ( 1.0 - step( texCoord2_g746.x , ( ( temp_output_3_0_g746 - 1.0 ) / temp_output_7_0_g746 ) ) ) * ( step( texCoord2_g746.x , ( temp_output_3_0_g746 / temp_output_7_0_g746 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g746.y , ( ( temp_output_9_0_g746 - 1.0 ) / temp_output_8_0_g746 ) ) ) * ( step( texCoord2_g746.y , ( temp_output_9_0_g746 / temp_output_8_0_g746 ) ) * 1.0 ) ) ) ) + ( _Color10 * ( ( ( 1.0 - step( texCoord2_g756.x , ( ( temp_output_3_0_g756 - 1.0 ) / temp_output_7_0_g756 ) ) ) * ( step( texCoord2_g756.x , ( temp_output_3_0_g756 / temp_output_7_0_g756 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g756.y , ( ( temp_output_9_0_g756 - 1.0 ) / temp_output_8_0_g756 ) ) ) * ( step( texCoord2_g756.y , ( temp_output_9_0_g756 / temp_output_8_0_g756 ) ) * 1.0 ) ) ) ) + ( _Color11 * ( ( ( 1.0 - step( texCoord2_g748.x , ( ( temp_output_3_0_g748 - 1.0 ) / temp_output_7_0_g748 ) ) ) * ( step( texCoord2_g748.x , ( temp_output_3_0_g748 / temp_output_7_0_g748 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g748.y , ( ( temp_output_9_0_g748 - 1.0 ) / temp_output_8_0_g748 ) ) ) * ( step( texCoord2_g748.y , ( temp_output_9_0_g748 / temp_output_8_0_g748 ) ) * 1.0 ) ) ) ) + ( _Color12 * ( ( ( 1.0 - step( texCoord2_g759.x , ( ( temp_output_3_0_g759 - 1.0 ) / temp_output_7_0_g759 ) ) ) * ( step( texCoord2_g759.x , ( temp_output_3_0_g759 / temp_output_7_0_g759 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g759.y , ( ( temp_output_9_0_g759 - 1.0 ) / temp_output_8_0_g759 ) ) ) * ( step( texCoord2_g759.y , ( temp_output_9_0_g759 / temp_output_8_0_g759 ) ) * 1.0 ) ) ) ) ) + ( ( _Color13 * ( ( ( 1.0 - step( texCoord2_g758.x , ( ( temp_output_3_0_g758 - 1.0 ) / temp_output_7_0_g758 ) ) ) * ( step( texCoord2_g758.x , ( temp_output_3_0_g758 / temp_output_7_0_g758 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g758.y , ( ( temp_output_9_0_g758 - 1.0 ) / temp_output_8_0_g758 ) ) ) * ( step( texCoord2_g758.y , ( temp_output_9_0_g758 / temp_output_8_0_g758 ) ) * 1.0 ) ) ) ) + ( _Color14 * ( ( ( 1.0 - step( texCoord2_g751.x , ( ( temp_output_3_0_g751 - 1.0 ) / temp_output_7_0_g751 ) ) ) * ( step( texCoord2_g751.x , ( temp_output_3_0_g751 / temp_output_7_0_g751 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g751.y , ( ( temp_output_9_0_g751 - 1.0 ) / temp_output_8_0_g751 ) ) ) * ( step( texCoord2_g751.y , ( temp_output_9_0_g751 / temp_output_8_0_g751 ) ) * 1.0 ) ) ) ) + ( _Color15 * ( ( ( 1.0 - step( texCoord2_g753.x , ( ( temp_output_3_0_g753 - 1.0 ) / temp_output_7_0_g753 ) ) ) * ( step( texCoord2_g753.x , ( temp_output_3_0_g753 / temp_output_7_0_g753 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g753.y , ( ( temp_output_9_0_g753 - 1.0 ) / temp_output_8_0_g753 ) ) ) * ( step( texCoord2_g753.y , ( temp_output_9_0_g753 / temp_output_8_0_g753 ) ) * 1.0 ) ) ) ) + ( _Color16 * ( ( ( 1.0 - step( texCoord2_g749.x , ( ( temp_output_3_0_g749 - 1.0 ) / temp_output_7_0_g749 ) ) ) * ( step( texCoord2_g749.x , ( temp_output_3_0_g749 / temp_output_7_0_g749 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g749.y , ( ( temp_output_9_0_g749 - 1.0 ) / temp_output_8_0_g749 ) ) ) * ( step( texCoord2_g749.y , ( temp_output_9_0_g749 / temp_output_8_0_g749 ) ) * 1.0 ) ) ) ) ) );
				float4 clampResult348 = clamp( ( pow( saferPower342 , temp_cast_0 ) + ( 1.0 - (temp_output_329_0).a ) ) , float4( 0,0,0,0 ) , float4( 1,1,1,1 ) );
				
				
				float3 Albedo = ( clampResult348 * temp_output_329_0 ).rgb;
				float Alpha = 1;
				float AlphaClipThreshold = 0.5;

				half4 color = half4( Albedo, Alpha );

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				return color;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthNormals"
			Tags { "LightMode"="DepthNormals" }

			ZWrite On
			Blend One Zero
            ZTest LEqual
            ZWrite On

			HLSLPROGRAM
			
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _EMISSION
			#define ASE_SRP_VERSION 999999

			
			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS SHADERPASS_DEPTHNORMALSONLY

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float3 worldNormal : TEXCOORD2;
				float4 worldTangent : TEXCOORD3;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _GradientColor;
			float4 _Color16;
			float4 _MRE1;
			float4 _MRE2;
			float4 _MRE3;
			float4 _MRE4;
			float4 _MRE5;
			float4 _MRE6;
			float4 _MRE7;
			float4 _MRE8;
			float4 _MRE9;
			float4 _MRE10;
			float4 _MRE11;
			float4 _MRE12;
			float4 _MRE13;
			float4 _MRE14;
			float4 _Color15;
			float4 _MRE15;
			float4 _Color14;
			float4 _Color12;
			float4 _Color1;
			float4 _Color2;
			float4 _Color3;
			float4 _Color13;
			float4 _Color5;
			float4 _Color6;
			float4 _Color4;
			float4 _Color8;
			float4 _Color9;
			float4 _Color10;
			float4 _Color11;
			float4 _Color7;
			float4 _MRE16;
			float _EmissionPower1;
			float _GradientPower;
			float _GradientOffset;
			float _GradientScale;
			float _GradientIntensity;
			#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			

			
			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;
				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float3 normalWS = TransformObjectToWorldNormal( v.ase_normal );
				float4 tangentWS = float4(TransformObjectToWorldDir( v.ase_tangent.xyz), v.ase_tangent.w);
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				o.worldNormal = normalWS;
				o.worldTangent = tangentWS;

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				o.clipPos = positionCS;
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_tangent = v.ase_tangent;
				
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)
				#define ASE_SV_DEPTH SV_DepthLessEqual  
			#else
				#define ASE_SV_DEPTH SV_Depth
			#endif
			half4 frag(	VertexOutput IN 
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );
				
				float3 WorldNormal = IN.worldNormal;
				float4 WorldTangent = IN.worldTangent;

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				
				float3 Normal = float3(0, 0, 1);
				float Alpha = 1;
				float AlphaClipThreshold = 0.5;
				#ifdef ASE_DEPTH_WRITE_ON
				float DepthValue = 0;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif
				
				#ifdef ASE_DEPTH_WRITE_ON
				outputDepth = DepthValue;
				#endif
				
				#if defined(_GBUFFER_NORMALS_OCT)
					float2 octNormalWS = PackNormalOctQuadEncode(WorldNormal);
					float2 remappedOctNormalWS = saturate(octNormalWS * 0.5 + 0.5);
					half3 packedNormalWS = PackFloat2To888(remappedOctNormalWS);
					return half4(packedNormalWS, 0.0);
				#else
					
					#if defined(_NORMALMAP)
						#if _NORMAL_DROPOFF_TS
							float crossSign = (WorldTangent.w > 0.0 ? 1.0 : -1.0) * GetOddNegativeScale();
							float3 bitangent = crossSign * cross(WorldNormal.xyz, WorldTangent.xyz);
							float3 normalWS = TransformTangentToWorld(Normal, half3x3(WorldTangent.xyz, bitangent, WorldNormal.xyz));
						#elif _NORMAL_DROPOFF_OS
							float3 normalWS = TransformObjectToWorldNormal(Normal);
						#elif _NORMAL_DROPOFF_WS
							float3 normalWS = Normal;
						#endif
					#else
						float3 normalWS = WorldNormal;
					#endif

					return half4(NormalizeNormalPerPixel(normalWS), 0.0);
				#endif
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "GBuffer"
			Tags { "LightMode"="UniversalGBuffer" }
			
			Blend One Zero, One Zero
			ColorMask RGBA
			

			HLSLPROGRAM
			
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _EMISSION
			#define ASE_SRP_VERSION 999999

			
			#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile _ DYNAMICLIGHTMAP_ON
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
			
			#pragma multi_compile _ _REFLECTION_PROBE_BLENDING
			#pragma multi_compile _ _REFLECTION_PROBE_BOX_PROJECTION

			#pragma multi_compile _ _SHADOWS_SOFT
			#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
			#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
			#pragma multi_compile _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
			#pragma multi_compile _ _GBUFFER_NORMALS_OCT
			#pragma multi_compile _ _LIGHT_LAYERS
			#pragma multi_compile _ _RENDER_PASS_ENABLED

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS SHADERPASS_GBUFFER

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityGBuffer.hlsl"


			#if defined(UNITY_INSTANCING_ENABLED) && defined(_TERRAIN_INSTANCED_PERPIXEL_NORMAL)
			    #define ENABLE_TERRAIN_PERPIXEL_NORMAL
			#endif

			

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 lightmapUVOrVertexSH : TEXCOORD0;
				half4 fogFactorAndVertexLight : TEXCOORD1;
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				float4 shadowCoord : TEXCOORD2;
				#endif
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				float4 screenPos : TEXCOORD6;
				#endif
				#if defined(DYNAMICLIGHTMAP_ON)
				float2 dynamicLightmapUV : TEXCOORD7;
				#endif
				float4 ase_texcoord8 : TEXCOORD8;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _GradientColor;
			float4 _Color16;
			float4 _MRE1;
			float4 _MRE2;
			float4 _MRE3;
			float4 _MRE4;
			float4 _MRE5;
			float4 _MRE6;
			float4 _MRE7;
			float4 _MRE8;
			float4 _MRE9;
			float4 _MRE10;
			float4 _MRE11;
			float4 _MRE12;
			float4 _MRE13;
			float4 _MRE14;
			float4 _Color15;
			float4 _MRE15;
			float4 _Color14;
			float4 _Color12;
			float4 _Color1;
			float4 _Color2;
			float4 _Color3;
			float4 _Color13;
			float4 _Color5;
			float4 _Color6;
			float4 _Color4;
			float4 _Color8;
			float4 _Color9;
			float4 _Color10;
			float4 _Color11;
			float4 _Color7;
			float4 _MRE16;
			float _EmissionPower1;
			float _GradientPower;
			float _GradientOffset;
			float _GradientScale;
			float _GradientIntensity;
			#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			sampler2D _Gradient;


			
			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.ase_texcoord8.xy = v.texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord8.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float3 positionVS = TransformWorldToView( positionWS );
				float4 positionCS = TransformWorldToHClip( positionWS );

				VertexNormalInputs normalInput = GetVertexNormalInputs( v.ase_normal, v.ase_tangent );

				o.tSpace0 = float4( normalInput.normalWS, positionWS.x);
				o.tSpace1 = float4( normalInput.tangentWS, positionWS.y);
				o.tSpace2 = float4( normalInput.bitangentWS, positionWS.z);

				OUTPUT_LIGHTMAP_UV( v.texcoord1, unity_LightmapST, o.lightmapUVOrVertexSH.xy );
				#if defined(DYNAMICLIGHTMAP_ON)
				o.dynamicLightmapUV.xy = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
				#endif

				OUTPUT_SH( normalInput.normalWS.xyz, o.lightmapUVOrVertexSH.xyz );

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					o.lightmapUVOrVertexSH.zw = v.texcoord;
					o.lightmapUVOrVertexSH.xy = v.texcoord * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif

				half3 vertexLight = VertexLighting( positionWS, normalInput.normalWS );
				#ifdef ASE_FOG
					half fogFactor = ComputeFogFactor( positionCS.z );
				#else
					half fogFactor = 0;
				#endif
				o.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
				
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				VertexPositionInputs vertexInput = (VertexPositionInputs)0;
				vertexInput.positionWS = positionWS;
				vertexInput.positionCS = positionCS;
				o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				
				o.clipPos = positionCS;
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				o.screenPos = ComputeScreenPos(positionCS);
				#endif
				return o;
			}
			
			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_tangent = v.ase_tangent;
				o.texcoord = v.texcoord;
				o.texcoord1 = v.texcoord1;
				o.texcoord2 = v.texcoord2;
				
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;
				o.texcoord = patch[0].texcoord * bary.x + patch[1].texcoord * bary.y + patch[2].texcoord * bary.z;
				o.texcoord1 = patch[0].texcoord1 * bary.x + patch[1].texcoord1 * bary.y + patch[2].texcoord1 * bary.z;
				o.texcoord2 = patch[0].texcoord2 * bary.x + patch[1].texcoord2 * bary.y + patch[2].texcoord2 * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)
				#define ASE_SV_DEPTH SV_DepthLessEqual  
			#else
				#define ASE_SV_DEPTH SV_Depth
			#endif
			FragmentOutput frag ( VertexOutput IN 
								#ifdef ASE_DEPTH_WRITE_ON
								,out float outputDepth : ASE_SV_DEPTH
								#endif
								 )
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float2 sampleCoords = (IN.lightmapUVOrVertexSH.zw / _TerrainHeightmapRecipSize.zw + 0.5f) * _TerrainHeightmapRecipSize.xy;
					float3 WorldNormal = TransformObjectToWorldNormal(normalize(SAMPLE_TEXTURE2D(_TerrainNormalmapTexture, sampler_TerrainNormalmapTexture, sampleCoords).rgb * 2 - 1));
					float3 WorldTangent = -cross(GetObjectToWorldMatrix()._13_23_33, WorldNormal);
					float3 WorldBiTangent = cross(WorldNormal, -WorldTangent);
				#else
					float3 WorldNormal = normalize( IN.tSpace0.xyz );
					float3 WorldTangent = IN.tSpace1.xyz;
					float3 WorldBiTangent = IN.tSpace2.xyz;
				#endif
				float3 WorldPosition = float3(IN.tSpace0.w,IN.tSpace1.w,IN.tSpace2.w);
				float3 WorldViewDirection = _WorldSpaceCameraPos.xyz  - WorldPosition;
				float4 ShadowCoords = float4( 0, 0, 0, 0 );
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				float4 ScreenPos = IN.screenPos;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					ShadowCoords = IN.shadowCoord;
				#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
					ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
				#else
					ShadowCoords = float4(0, 0, 0, 0);
				#endif


	
				WorldViewDirection = SafeNormalize( WorldViewDirection );

				float2 texCoord256 = IN.ase_texcoord8.xy * float2( 1,4 ) + float2( 0,0 );
				float4 clampResult328 = clamp( ( ( tex2D( _Gradient, texCoord256 ) + _GradientColor ) + ( 1.0 - _GradientIntensity ) ) , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
				float4 saferPower342 = abs( (clampResult328*_GradientScale + _GradientOffset) );
				float4 temp_cast_0 = (_GradientPower).xxxx;
				float2 texCoord2_g760 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g760 = 1.0;
				float temp_output_7_0_g760 = 4.0;
				float temp_output_9_0_g760 = 4.0;
				float temp_output_8_0_g760 = 4.0;
				float2 texCoord2_g754 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g754 = 2.0;
				float temp_output_7_0_g754 = 4.0;
				float temp_output_9_0_g754 = 4.0;
				float temp_output_8_0_g754 = 4.0;
				float2 texCoord2_g755 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g755 = 3.0;
				float temp_output_7_0_g755 = 4.0;
				float temp_output_9_0_g755 = 4.0;
				float temp_output_8_0_g755 = 4.0;
				float2 texCoord2_g757 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g757 = 4.0;
				float temp_output_7_0_g757 = 4.0;
				float temp_output_9_0_g757 = 4.0;
				float temp_output_8_0_g757 = 4.0;
				float2 texCoord2_g750 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g750 = 1.0;
				float temp_output_7_0_g750 = 4.0;
				float temp_output_9_0_g750 = 3.0;
				float temp_output_8_0_g750 = 4.0;
				float2 texCoord2_g745 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g745 = 2.0;
				float temp_output_7_0_g745 = 4.0;
				float temp_output_9_0_g745 = 3.0;
				float temp_output_8_0_g745 = 4.0;
				float2 texCoord2_g752 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g752 = 3.0;
				float temp_output_7_0_g752 = 4.0;
				float temp_output_9_0_g752 = 3.0;
				float temp_output_8_0_g752 = 4.0;
				float2 texCoord2_g747 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g747 = 4.0;
				float temp_output_7_0_g747 = 4.0;
				float temp_output_9_0_g747 = 3.0;
				float temp_output_8_0_g747 = 4.0;
				float2 texCoord2_g746 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g746 = 1.0;
				float temp_output_7_0_g746 = 4.0;
				float temp_output_9_0_g746 = 2.0;
				float temp_output_8_0_g746 = 4.0;
				float2 texCoord2_g756 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g756 = 2.0;
				float temp_output_7_0_g756 = 4.0;
				float temp_output_9_0_g756 = 2.0;
				float temp_output_8_0_g756 = 4.0;
				float2 texCoord2_g748 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g748 = 3.0;
				float temp_output_7_0_g748 = 4.0;
				float temp_output_9_0_g748 = 2.0;
				float temp_output_8_0_g748 = 4.0;
				float2 texCoord2_g759 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g759 = 4.0;
				float temp_output_7_0_g759 = 4.0;
				float temp_output_9_0_g759 = 2.0;
				float temp_output_8_0_g759 = 4.0;
				float2 texCoord2_g758 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g758 = 1.0;
				float temp_output_7_0_g758 = 4.0;
				float temp_output_9_0_g758 = 1.0;
				float temp_output_8_0_g758 = 4.0;
				float2 texCoord2_g751 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g751 = 2.0;
				float temp_output_7_0_g751 = 4.0;
				float temp_output_9_0_g751 = 1.0;
				float temp_output_8_0_g751 = 4.0;
				float2 texCoord2_g753 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g753 = 3.0;
				float temp_output_7_0_g753 = 4.0;
				float temp_output_9_0_g753 = 1.0;
				float temp_output_8_0_g753 = 4.0;
				float2 texCoord2_g749 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g749 = 4.0;
				float temp_output_7_0_g749 = 4.0;
				float temp_output_9_0_g749 = 1.0;
				float temp_output_8_0_g749 = 4.0;
				float4 temp_output_329_0 = ( ( ( _Color1 * ( ( ( 1.0 - step( texCoord2_g760.x , ( ( temp_output_3_0_g760 - 1.0 ) / temp_output_7_0_g760 ) ) ) * ( step( texCoord2_g760.x , ( temp_output_3_0_g760 / temp_output_7_0_g760 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g760.y , ( ( temp_output_9_0_g760 - 1.0 ) / temp_output_8_0_g760 ) ) ) * ( step( texCoord2_g760.y , ( temp_output_9_0_g760 / temp_output_8_0_g760 ) ) * 1.0 ) ) ) ) + ( _Color2 * ( ( ( 1.0 - step( texCoord2_g754.x , ( ( temp_output_3_0_g754 - 1.0 ) / temp_output_7_0_g754 ) ) ) * ( step( texCoord2_g754.x , ( temp_output_3_0_g754 / temp_output_7_0_g754 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g754.y , ( ( temp_output_9_0_g754 - 1.0 ) / temp_output_8_0_g754 ) ) ) * ( step( texCoord2_g754.y , ( temp_output_9_0_g754 / temp_output_8_0_g754 ) ) * 1.0 ) ) ) ) + ( _Color3 * ( ( ( 1.0 - step( texCoord2_g755.x , ( ( temp_output_3_0_g755 - 1.0 ) / temp_output_7_0_g755 ) ) ) * ( step( texCoord2_g755.x , ( temp_output_3_0_g755 / temp_output_7_0_g755 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g755.y , ( ( temp_output_9_0_g755 - 1.0 ) / temp_output_8_0_g755 ) ) ) * ( step( texCoord2_g755.y , ( temp_output_9_0_g755 / temp_output_8_0_g755 ) ) * 1.0 ) ) ) ) + ( _Color4 * ( ( ( 1.0 - step( texCoord2_g757.x , ( ( temp_output_3_0_g757 - 1.0 ) / temp_output_7_0_g757 ) ) ) * ( step( texCoord2_g757.x , ( temp_output_3_0_g757 / temp_output_7_0_g757 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g757.y , ( ( temp_output_9_0_g757 - 1.0 ) / temp_output_8_0_g757 ) ) ) * ( step( texCoord2_g757.y , ( temp_output_9_0_g757 / temp_output_8_0_g757 ) ) * 1.0 ) ) ) ) ) + ( ( _Color5 * ( ( ( 1.0 - step( texCoord2_g750.x , ( ( temp_output_3_0_g750 - 1.0 ) / temp_output_7_0_g750 ) ) ) * ( step( texCoord2_g750.x , ( temp_output_3_0_g750 / temp_output_7_0_g750 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g750.y , ( ( temp_output_9_0_g750 - 1.0 ) / temp_output_8_0_g750 ) ) ) * ( step( texCoord2_g750.y , ( temp_output_9_0_g750 / temp_output_8_0_g750 ) ) * 1.0 ) ) ) ) + ( _Color6 * ( ( ( 1.0 - step( texCoord2_g745.x , ( ( temp_output_3_0_g745 - 1.0 ) / temp_output_7_0_g745 ) ) ) * ( step( texCoord2_g745.x , ( temp_output_3_0_g745 / temp_output_7_0_g745 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g745.y , ( ( temp_output_9_0_g745 - 1.0 ) / temp_output_8_0_g745 ) ) ) * ( step( texCoord2_g745.y , ( temp_output_9_0_g745 / temp_output_8_0_g745 ) ) * 1.0 ) ) ) ) + ( _Color7 * ( ( ( 1.0 - step( texCoord2_g752.x , ( ( temp_output_3_0_g752 - 1.0 ) / temp_output_7_0_g752 ) ) ) * ( step( texCoord2_g752.x , ( temp_output_3_0_g752 / temp_output_7_0_g752 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g752.y , ( ( temp_output_9_0_g752 - 1.0 ) / temp_output_8_0_g752 ) ) ) * ( step( texCoord2_g752.y , ( temp_output_9_0_g752 / temp_output_8_0_g752 ) ) * 1.0 ) ) ) ) + ( _Color8 * ( ( ( 1.0 - step( texCoord2_g747.x , ( ( temp_output_3_0_g747 - 1.0 ) / temp_output_7_0_g747 ) ) ) * ( step( texCoord2_g747.x , ( temp_output_3_0_g747 / temp_output_7_0_g747 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g747.y , ( ( temp_output_9_0_g747 - 1.0 ) / temp_output_8_0_g747 ) ) ) * ( step( texCoord2_g747.y , ( temp_output_9_0_g747 / temp_output_8_0_g747 ) ) * 1.0 ) ) ) ) ) + ( ( _Color9 * ( ( ( 1.0 - step( texCoord2_g746.x , ( ( temp_output_3_0_g746 - 1.0 ) / temp_output_7_0_g746 ) ) ) * ( step( texCoord2_g746.x , ( temp_output_3_0_g746 / temp_output_7_0_g746 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g746.y , ( ( temp_output_9_0_g746 - 1.0 ) / temp_output_8_0_g746 ) ) ) * ( step( texCoord2_g746.y , ( temp_output_9_0_g746 / temp_output_8_0_g746 ) ) * 1.0 ) ) ) ) + ( _Color10 * ( ( ( 1.0 - step( texCoord2_g756.x , ( ( temp_output_3_0_g756 - 1.0 ) / temp_output_7_0_g756 ) ) ) * ( step( texCoord2_g756.x , ( temp_output_3_0_g756 / temp_output_7_0_g756 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g756.y , ( ( temp_output_9_0_g756 - 1.0 ) / temp_output_8_0_g756 ) ) ) * ( step( texCoord2_g756.y , ( temp_output_9_0_g756 / temp_output_8_0_g756 ) ) * 1.0 ) ) ) ) + ( _Color11 * ( ( ( 1.0 - step( texCoord2_g748.x , ( ( temp_output_3_0_g748 - 1.0 ) / temp_output_7_0_g748 ) ) ) * ( step( texCoord2_g748.x , ( temp_output_3_0_g748 / temp_output_7_0_g748 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g748.y , ( ( temp_output_9_0_g748 - 1.0 ) / temp_output_8_0_g748 ) ) ) * ( step( texCoord2_g748.y , ( temp_output_9_0_g748 / temp_output_8_0_g748 ) ) * 1.0 ) ) ) ) + ( _Color12 * ( ( ( 1.0 - step( texCoord2_g759.x , ( ( temp_output_3_0_g759 - 1.0 ) / temp_output_7_0_g759 ) ) ) * ( step( texCoord2_g759.x , ( temp_output_3_0_g759 / temp_output_7_0_g759 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g759.y , ( ( temp_output_9_0_g759 - 1.0 ) / temp_output_8_0_g759 ) ) ) * ( step( texCoord2_g759.y , ( temp_output_9_0_g759 / temp_output_8_0_g759 ) ) * 1.0 ) ) ) ) ) + ( ( _Color13 * ( ( ( 1.0 - step( texCoord2_g758.x , ( ( temp_output_3_0_g758 - 1.0 ) / temp_output_7_0_g758 ) ) ) * ( step( texCoord2_g758.x , ( temp_output_3_0_g758 / temp_output_7_0_g758 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g758.y , ( ( temp_output_9_0_g758 - 1.0 ) / temp_output_8_0_g758 ) ) ) * ( step( texCoord2_g758.y , ( temp_output_9_0_g758 / temp_output_8_0_g758 ) ) * 1.0 ) ) ) ) + ( _Color14 * ( ( ( 1.0 - step( texCoord2_g751.x , ( ( temp_output_3_0_g751 - 1.0 ) / temp_output_7_0_g751 ) ) ) * ( step( texCoord2_g751.x , ( temp_output_3_0_g751 / temp_output_7_0_g751 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g751.y , ( ( temp_output_9_0_g751 - 1.0 ) / temp_output_8_0_g751 ) ) ) * ( step( texCoord2_g751.y , ( temp_output_9_0_g751 / temp_output_8_0_g751 ) ) * 1.0 ) ) ) ) + ( _Color15 * ( ( ( 1.0 - step( texCoord2_g753.x , ( ( temp_output_3_0_g753 - 1.0 ) / temp_output_7_0_g753 ) ) ) * ( step( texCoord2_g753.x , ( temp_output_3_0_g753 / temp_output_7_0_g753 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g753.y , ( ( temp_output_9_0_g753 - 1.0 ) / temp_output_8_0_g753 ) ) ) * ( step( texCoord2_g753.y , ( temp_output_9_0_g753 / temp_output_8_0_g753 ) ) * 1.0 ) ) ) ) + ( _Color16 * ( ( ( 1.0 - step( texCoord2_g749.x , ( ( temp_output_3_0_g749 - 1.0 ) / temp_output_7_0_g749 ) ) ) * ( step( texCoord2_g749.x , ( temp_output_3_0_g749 / temp_output_7_0_g749 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g749.y , ( ( temp_output_9_0_g749 - 1.0 ) / temp_output_8_0_g749 ) ) ) * ( step( texCoord2_g749.y , ( temp_output_9_0_g749 / temp_output_8_0_g749 ) ) * 1.0 ) ) ) ) ) );
				float4 clampResult348 = clamp( ( pow( saferPower342 , temp_cast_0 ) + ( 1.0 - (temp_output_329_0).a ) ) , float4( 0,0,0,0 ) , float4( 1,1,1,1 ) );
				
				float2 texCoord2_g731 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g731 = 1.0;
				float temp_output_7_0_g731 = 4.0;
				float temp_output_9_0_g731 = 4.0;
				float temp_output_8_0_g731 = 4.0;
				float2 texCoord2_g744 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g744 = 2.0;
				float temp_output_7_0_g744 = 4.0;
				float temp_output_9_0_g744 = 4.0;
				float temp_output_8_0_g744 = 4.0;
				float2 texCoord2_g732 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g732 = 3.0;
				float temp_output_7_0_g732 = 4.0;
				float temp_output_9_0_g732 = 4.0;
				float temp_output_8_0_g732 = 4.0;
				float2 texCoord2_g741 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g741 = 4.0;
				float temp_output_7_0_g741 = 4.0;
				float temp_output_9_0_g741 = 4.0;
				float temp_output_8_0_g741 = 4.0;
				float2 texCoord2_g739 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g739 = 1.0;
				float temp_output_7_0_g739 = 4.0;
				float temp_output_9_0_g739 = 3.0;
				float temp_output_8_0_g739 = 4.0;
				float2 texCoord2_g733 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g733 = 2.0;
				float temp_output_7_0_g733 = 4.0;
				float temp_output_9_0_g733 = 3.0;
				float temp_output_8_0_g733 = 4.0;
				float2 texCoord2_g743 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g743 = 3.0;
				float temp_output_7_0_g743 = 4.0;
				float temp_output_9_0_g743 = 3.0;
				float temp_output_8_0_g743 = 4.0;
				float2 texCoord2_g736 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g736 = 4.0;
				float temp_output_7_0_g736 = 4.0;
				float temp_output_9_0_g736 = 3.0;
				float temp_output_8_0_g736 = 4.0;
				float2 texCoord2_g742 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g742 = 1.0;
				float temp_output_7_0_g742 = 4.0;
				float temp_output_9_0_g742 = 2.0;
				float temp_output_8_0_g742 = 4.0;
				float2 texCoord2_g735 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g735 = 2.0;
				float temp_output_7_0_g735 = 4.0;
				float temp_output_9_0_g735 = 2.0;
				float temp_output_8_0_g735 = 4.0;
				float2 texCoord2_g729 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g729 = 3.0;
				float temp_output_7_0_g729 = 4.0;
				float temp_output_9_0_g729 = 2.0;
				float temp_output_8_0_g729 = 4.0;
				float2 texCoord2_g730 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g730 = 4.0;
				float temp_output_7_0_g730 = 4.0;
				float temp_output_9_0_g730 = 2.0;
				float temp_output_8_0_g730 = 4.0;
				float2 texCoord2_g738 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g738 = 1.0;
				float temp_output_7_0_g738 = 4.0;
				float temp_output_9_0_g738 = 1.0;
				float temp_output_8_0_g738 = 4.0;
				float2 texCoord2_g737 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g737 = 2.0;
				float temp_output_7_0_g737 = 4.0;
				float temp_output_9_0_g737 = 1.0;
				float temp_output_8_0_g737 = 4.0;
				float2 texCoord2_g734 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g734 = 3.0;
				float temp_output_7_0_g734 = 4.0;
				float temp_output_9_0_g734 = 1.0;
				float temp_output_8_0_g734 = 4.0;
				float2 texCoord2_g740 = IN.ase_texcoord8.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g740 = 4.0;
				float temp_output_7_0_g740 = 4.0;
				float temp_output_9_0_g740 = 1.0;
				float temp_output_8_0_g740 = 4.0;
				float4 temp_output_344_0 = ( ( ( _MRE1 * ( ( ( 1.0 - step( texCoord2_g731.x , ( ( temp_output_3_0_g731 - 1.0 ) / temp_output_7_0_g731 ) ) ) * ( step( texCoord2_g731.x , ( temp_output_3_0_g731 / temp_output_7_0_g731 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g731.y , ( ( temp_output_9_0_g731 - 1.0 ) / temp_output_8_0_g731 ) ) ) * ( step( texCoord2_g731.y , ( temp_output_9_0_g731 / temp_output_8_0_g731 ) ) * 1.0 ) ) ) ) + ( _MRE2 * ( ( ( 1.0 - step( texCoord2_g744.x , ( ( temp_output_3_0_g744 - 1.0 ) / temp_output_7_0_g744 ) ) ) * ( step( texCoord2_g744.x , ( temp_output_3_0_g744 / temp_output_7_0_g744 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g744.y , ( ( temp_output_9_0_g744 - 1.0 ) / temp_output_8_0_g744 ) ) ) * ( step( texCoord2_g744.y , ( temp_output_9_0_g744 / temp_output_8_0_g744 ) ) * 1.0 ) ) ) ) + ( _MRE3 * ( ( ( 1.0 - step( texCoord2_g732.x , ( ( temp_output_3_0_g732 - 1.0 ) / temp_output_7_0_g732 ) ) ) * ( step( texCoord2_g732.x , ( temp_output_3_0_g732 / temp_output_7_0_g732 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g732.y , ( ( temp_output_9_0_g732 - 1.0 ) / temp_output_8_0_g732 ) ) ) * ( step( texCoord2_g732.y , ( temp_output_9_0_g732 / temp_output_8_0_g732 ) ) * 1.0 ) ) ) ) + ( _MRE4 * ( ( ( 1.0 - step( texCoord2_g741.x , ( ( temp_output_3_0_g741 - 1.0 ) / temp_output_7_0_g741 ) ) ) * ( step( texCoord2_g741.x , ( temp_output_3_0_g741 / temp_output_7_0_g741 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g741.y , ( ( temp_output_9_0_g741 - 1.0 ) / temp_output_8_0_g741 ) ) ) * ( step( texCoord2_g741.y , ( temp_output_9_0_g741 / temp_output_8_0_g741 ) ) * 1.0 ) ) ) ) ) + ( ( _MRE5 * ( ( ( 1.0 - step( texCoord2_g739.x , ( ( temp_output_3_0_g739 - 1.0 ) / temp_output_7_0_g739 ) ) ) * ( step( texCoord2_g739.x , ( temp_output_3_0_g739 / temp_output_7_0_g739 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g739.y , ( ( temp_output_9_0_g739 - 1.0 ) / temp_output_8_0_g739 ) ) ) * ( step( texCoord2_g739.y , ( temp_output_9_0_g739 / temp_output_8_0_g739 ) ) * 1.0 ) ) ) ) + ( _MRE6 * ( ( ( 1.0 - step( texCoord2_g733.x , ( ( temp_output_3_0_g733 - 1.0 ) / temp_output_7_0_g733 ) ) ) * ( step( texCoord2_g733.x , ( temp_output_3_0_g733 / temp_output_7_0_g733 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g733.y , ( ( temp_output_9_0_g733 - 1.0 ) / temp_output_8_0_g733 ) ) ) * ( step( texCoord2_g733.y , ( temp_output_9_0_g733 / temp_output_8_0_g733 ) ) * 1.0 ) ) ) ) + ( _MRE7 * ( ( ( 1.0 - step( texCoord2_g743.x , ( ( temp_output_3_0_g743 - 1.0 ) / temp_output_7_0_g743 ) ) ) * ( step( texCoord2_g743.x , ( temp_output_3_0_g743 / temp_output_7_0_g743 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g743.y , ( ( temp_output_9_0_g743 - 1.0 ) / temp_output_8_0_g743 ) ) ) * ( step( texCoord2_g743.y , ( temp_output_9_0_g743 / temp_output_8_0_g743 ) ) * 1.0 ) ) ) ) + ( _MRE8 * ( ( ( 1.0 - step( texCoord2_g736.x , ( ( temp_output_3_0_g736 - 1.0 ) / temp_output_7_0_g736 ) ) ) * ( step( texCoord2_g736.x , ( temp_output_3_0_g736 / temp_output_7_0_g736 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g736.y , ( ( temp_output_9_0_g736 - 1.0 ) / temp_output_8_0_g736 ) ) ) * ( step( texCoord2_g736.y , ( temp_output_9_0_g736 / temp_output_8_0_g736 ) ) * 1.0 ) ) ) ) ) + ( ( _MRE9 * ( ( ( 1.0 - step( texCoord2_g742.x , ( ( temp_output_3_0_g742 - 1.0 ) / temp_output_7_0_g742 ) ) ) * ( step( texCoord2_g742.x , ( temp_output_3_0_g742 / temp_output_7_0_g742 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g742.y , ( ( temp_output_9_0_g742 - 1.0 ) / temp_output_8_0_g742 ) ) ) * ( step( texCoord2_g742.y , ( temp_output_9_0_g742 / temp_output_8_0_g742 ) ) * 1.0 ) ) ) ) + ( _MRE10 * ( ( ( 1.0 - step( texCoord2_g735.x , ( ( temp_output_3_0_g735 - 1.0 ) / temp_output_7_0_g735 ) ) ) * ( step( texCoord2_g735.x , ( temp_output_3_0_g735 / temp_output_7_0_g735 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g735.y , ( ( temp_output_9_0_g735 - 1.0 ) / temp_output_8_0_g735 ) ) ) * ( step( texCoord2_g735.y , ( temp_output_9_0_g735 / temp_output_8_0_g735 ) ) * 1.0 ) ) ) ) + ( _MRE11 * ( ( ( 1.0 - step( texCoord2_g729.x , ( ( temp_output_3_0_g729 - 1.0 ) / temp_output_7_0_g729 ) ) ) * ( step( texCoord2_g729.x , ( temp_output_3_0_g729 / temp_output_7_0_g729 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g729.y , ( ( temp_output_9_0_g729 - 1.0 ) / temp_output_8_0_g729 ) ) ) * ( step( texCoord2_g729.y , ( temp_output_9_0_g729 / temp_output_8_0_g729 ) ) * 1.0 ) ) ) ) + ( _MRE12 * ( ( ( 1.0 - step( texCoord2_g730.x , ( ( temp_output_3_0_g730 - 1.0 ) / temp_output_7_0_g730 ) ) ) * ( step( texCoord2_g730.x , ( temp_output_3_0_g730 / temp_output_7_0_g730 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g730.y , ( ( temp_output_9_0_g730 - 1.0 ) / temp_output_8_0_g730 ) ) ) * ( step( texCoord2_g730.y , ( temp_output_9_0_g730 / temp_output_8_0_g730 ) ) * 1.0 ) ) ) ) ) + ( ( _MRE13 * ( ( ( 1.0 - step( texCoord2_g738.x , ( ( temp_output_3_0_g738 - 1.0 ) / temp_output_7_0_g738 ) ) ) * ( step( texCoord2_g738.x , ( temp_output_3_0_g738 / temp_output_7_0_g738 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g738.y , ( ( temp_output_9_0_g738 - 1.0 ) / temp_output_8_0_g738 ) ) ) * ( step( texCoord2_g738.y , ( temp_output_9_0_g738 / temp_output_8_0_g738 ) ) * 1.0 ) ) ) ) + ( _MRE14 * ( ( ( 1.0 - step( texCoord2_g737.x , ( ( temp_output_3_0_g737 - 1.0 ) / temp_output_7_0_g737 ) ) ) * ( step( texCoord2_g737.x , ( temp_output_3_0_g737 / temp_output_7_0_g737 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g737.y , ( ( temp_output_9_0_g737 - 1.0 ) / temp_output_8_0_g737 ) ) ) * ( step( texCoord2_g737.y , ( temp_output_9_0_g737 / temp_output_8_0_g737 ) ) * 1.0 ) ) ) ) + ( _MRE15 * ( ( ( 1.0 - step( texCoord2_g734.x , ( ( temp_output_3_0_g734 - 1.0 ) / temp_output_7_0_g734 ) ) ) * ( step( texCoord2_g734.x , ( temp_output_3_0_g734 / temp_output_7_0_g734 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g734.y , ( ( temp_output_9_0_g734 - 1.0 ) / temp_output_8_0_g734 ) ) ) * ( step( texCoord2_g734.y , ( temp_output_9_0_g734 / temp_output_8_0_g734 ) ) * 1.0 ) ) ) ) + ( _MRE16 * ( ( ( 1.0 - step( texCoord2_g740.x , ( ( temp_output_3_0_g740 - 1.0 ) / temp_output_7_0_g740 ) ) ) * ( step( texCoord2_g740.x , ( temp_output_3_0_g740 / temp_output_7_0_g740 ) ) * 1.0 ) ) * ( ( 1.0 - step( texCoord2_g740.y , ( ( temp_output_9_0_g740 - 1.0 ) / temp_output_8_0_g740 ) ) ) * ( step( texCoord2_g740.y , ( temp_output_9_0_g740 / temp_output_8_0_g740 ) ) * 1.0 ) ) ) ) ) );
				
				float3 Albedo = ( clampResult348 * temp_output_329_0 ).rgb;
				float3 Normal = float3(0, 0, 1);
				float3 Emission = ( temp_output_329_0 * ( _EmissionPower1 * (temp_output_344_0).b ) ).rgb;
				float3 Specular = 0.5;
				float Metallic = (temp_output_344_0).r;
				float Smoothness = ( 1.0 - (temp_output_344_0).g );
				float Occlusion = 1;
				float Alpha = 1;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;
				float3 BakedGI = 0;
				float3 RefractionColor = 1;
				float RefractionIndex = 1;
				float3 Transmission = 1;
				float3 Translucency = 1;
				#ifdef ASE_DEPTH_WRITE_ON
				float DepthValue = 0;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				InputData inputData = (InputData)0;
				inputData.positionWS = WorldPosition;
				inputData.positionCS = IN.clipPos;
				inputData.shadowCoord = ShadowCoords;



				#ifdef _NORMALMAP
					#if _NORMAL_DROPOFF_TS
					inputData.normalWS = TransformTangentToWorld(Normal, half3x3( WorldTangent, WorldBiTangent, WorldNormal ));
					#elif _NORMAL_DROPOFF_OS
					inputData.normalWS = TransformObjectToWorldNormal(Normal);
					#elif _NORMAL_DROPOFF_WS
					inputData.normalWS = Normal;
					#endif
				#else
					inputData.normalWS = WorldNormal;
				#endif
					
				inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
				inputData.viewDirectionWS = SafeNormalize( WorldViewDirection );



				#ifdef ASE_FOG
					inputData.fogCoord = InitializeInputDataFog(float4(WorldPosition, 1.0),  IN.fogFactorAndVertexLight.x);
				#endif

				inputData.vertexLighting = IN.fogFactorAndVertexLight.yzw;

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float3 SH = SampleSH(inputData.normalWS.xyz);
				#else
					float3 SH = IN.lightmapUVOrVertexSH.xyz;
				#endif

				

				#ifdef _ASE_BAKEDGI
					inputData.bakedGI = BakedGI;
				#else
					#if defined(DYNAMICLIGHTMAP_ON)
						inputData.bakedGI = SAMPLE_GI( IN.lightmapUVOrVertexSH.xy, IN.dynamicLightmapUV.xy, SH, inputData.normalWS);
					#else
						inputData.bakedGI = SAMPLE_GI( IN.lightmapUVOrVertexSH.xy, SH, inputData.normalWS );
					#endif
				#endif

				inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(IN.clipPos);
				inputData.shadowMask = SAMPLE_SHADOWMASK(IN.lightmapUVOrVertexSH.xy);

				#if defined(DEBUG_DISPLAY)
					#if defined(DYNAMICLIGHTMAP_ON)
						inputData.dynamicLightmapUV = IN.dynamicLightmapUV.xy;
						#endif
					#if defined(LIGHTMAP_ON)
						inputData.staticLightmapUV = IN.lightmapUVOrVertexSH.xy;
					#else
						inputData.vertexSH = SH;
					#endif
				#endif

				#ifdef _DBUFFER
					ApplyDecal(IN.clipPos,
						Albedo,
						Specular,
						inputData.normalWS,
						Metallic,
						Occlusion,
						Smoothness);
				#endif

				BRDFData brdfData;
				InitializeBRDFData
				(Albedo, Metallic, Specular, Smoothness, Alpha, brdfData);

				Light mainLight = GetMainLight(inputData.shadowCoord, inputData.positionWS, inputData.shadowMask);
				half4 color;
				MixRealtimeAndBakedGI(mainLight, inputData.normalWS, inputData.bakedGI, inputData.shadowMask);
				color.rgb = GlobalIllumination(brdfData, inputData.bakedGI, Occlusion, inputData.positionWS, inputData.normalWS, inputData.viewDirectionWS);
				color.a = Alpha;
				
				#ifdef ASE_FINAL_COLOR_ALPHA_MULTIPLY
					color.rgb *= color.a;
				#endif
				
				#ifdef ASE_FOG
					#ifdef TERRAIN_SPLAT_ADDPASS
						color.rgb = MixFogColor(color.rgb, half3( 0, 0, 0 ), IN.fogFactorAndVertexLight.x );
					#else
						color.rgb = MixFog(color.rgb, IN.fogFactorAndVertexLight.x);
					#endif
				#endif
				
				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif
				
				return BRDFDataToGbuffer(brdfData, inputData, Smoothness, Emission + color.rgb);
			}

			ENDHLSL
		}

		
        Pass
        {
			
            Name "SceneSelectionPass"
            Tags { "LightMode"="SceneSelectionPass" }
        
			Cull Off

			HLSLPROGRAM
        
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _EMISSION
			#define ASE_SRP_VERSION 999999

        
			#pragma only_renderers d3d11 glcore gles gles3 
			#pragma vertex vert
			#pragma fragment frag

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define SHADERPASS SHADERPASS_DEPTHONLY

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
			

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
        
			CBUFFER_START(UnityPerMaterial)
			float4 _GradientColor;
			float4 _Color16;
			float4 _MRE1;
			float4 _MRE2;
			float4 _MRE3;
			float4 _MRE4;
			float4 _MRE5;
			float4 _MRE6;
			float4 _MRE7;
			float4 _MRE8;
			float4 _MRE9;
			float4 _MRE10;
			float4 _MRE11;
			float4 _MRE12;
			float4 _MRE13;
			float4 _MRE14;
			float4 _Color15;
			float4 _MRE15;
			float4 _Color14;
			float4 _Color12;
			float4 _Color1;
			float4 _Color2;
			float4 _Color3;
			float4 _Color13;
			float4 _Color5;
			float4 _Color6;
			float4 _Color4;
			float4 _Color8;
			float4 _Color9;
			float4 _Color10;
			float4 _Color11;
			float4 _Color7;
			float4 _MRE16;
			float _EmissionPower1;
			float _GradientPower;
			float _GradientOffset;
			float _GradientScale;
			float _GradientIntensity;
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			

			
			int _ObjectId;
			int _PassValue;

			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};
        
			VertexOutput VertexFunction(VertexInput v  )
			{
				VertexOutput o;
				ZERO_INITIALIZE(VertexOutput, o);

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);


				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				o.clipPos = TransformWorldToHClip(positionWS);
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif
			
			half4 frag(VertexOutput IN ) : SV_TARGET
			{
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;
				
				surfaceDescription.Alpha = 1;
				surfaceDescription.AlphaClipThreshold = 0.5;


				#if _ALPHATEST_ON
					float alphaClipThreshold = 0.01f;
					#if ALPHA_CLIP_THRESHOLD
						alphaClipThreshold = surfaceDescription.AlphaClipThreshold;
					#endif
					clip(surfaceDescription.Alpha - alphaClipThreshold);
				#endif

				half4 outColor = half4(_ObjectId, _PassValue, 1.0, 1.0);
				return outColor;
			}

			ENDHLSL
        }

		
        Pass
        {
			
            Name "ScenePickingPass"
            Tags { "LightMode"="Picking" }
        
			HLSLPROGRAM

			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _EMISSION
			#define ASE_SRP_VERSION 999999


			#pragma only_renderers d3d11 glcore gles gles3 
			#pragma vertex vert
			#pragma fragment frag

        
			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define SHADERPASS SHADERPASS_DEPTHONLY
			

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
			

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
        
			CBUFFER_START(UnityPerMaterial)
			float4 _GradientColor;
			float4 _Color16;
			float4 _MRE1;
			float4 _MRE2;
			float4 _MRE3;
			float4 _MRE4;
			float4 _MRE5;
			float4 _MRE6;
			float4 _MRE7;
			float4 _MRE8;
			float4 _MRE9;
			float4 _MRE10;
			float4 _MRE11;
			float4 _MRE12;
			float4 _MRE13;
			float4 _MRE14;
			float4 _Color15;
			float4 _MRE15;
			float4 _Color14;
			float4 _Color12;
			float4 _Color1;
			float4 _Color2;
			float4 _Color3;
			float4 _Color13;
			float4 _Color5;
			float4 _Color6;
			float4 _Color4;
			float4 _Color8;
			float4 _Color9;
			float4 _Color10;
			float4 _Color11;
			float4 _Color7;
			float4 _MRE16;
			float _EmissionPower1;
			float _GradientPower;
			float _GradientOffset;
			float _GradientScale;
			float _GradientIntensity;
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			

			
        
			float4 _SelectionID;

        
			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};
        
			VertexOutput VertexFunction(VertexInput v  )
			{
				VertexOutput o;
				ZERO_INITIALIZE(VertexOutput, o);

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);


				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				o.clipPos = TransformWorldToHClip(positionWS);
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN ) : SV_TARGET
			{
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;
				
				surfaceDescription.Alpha = 1;
				surfaceDescription.AlphaClipThreshold = 0.5;


				#if _ALPHATEST_ON
					float alphaClipThreshold = 0.01f;
					#if ALPHA_CLIP_THRESHOLD
						alphaClipThreshold = surfaceDescription.AlphaClipThreshold;
					#endif
					clip(surfaceDescription.Alpha - alphaClipThreshold);
				#endif

				half4 outColor = 0;
				outColor = _SelectionID;
				
				return outColor;
			}
        
			ENDHLSL
        }
		
	}
	
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=18935
157;99;1399;633;-1380.816;76.75456;1.246358;True;True
Node;AmplifyShaderEditor.TextureCoordinatesNode;256;288.1596,87.91077;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,4;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;318;855.679,4178.713;Inherit;True;ColorShartSlot;-1;;736;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;4;False;9;FLOAT;3;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;327;862.4109,4930.181;Inherit;True;ColorShartSlot;-1;;729;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;3;False;9;FLOAT;2;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;328;1325.247,67.20997;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;332;853.4119,5152.679;Inherit;True;ColorShartSlot;-1;;730;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;4;False;9;FLOAT;2;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;329;689.7629,1490.179;Inherit;True;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;325;1321.758,324.4118;Float;False;Property;_GradientScale;Gradient Scale;36;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;324;877.594,2625.656;Inherit;True;ColorShartSlot;-1;;731;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;1;False;9;FLOAT;4;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;323;873.4548,3051.996;Inherit;True;ColorShartSlot;-1;;732;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;3;False;9;FLOAT;4;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;319;860.446,3741.301;Inherit;True;ColorShartSlot;-1;;733;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;2;False;9;FLOAT;3;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;316;867.7078,6001.553;Inherit;True;ColorShartSlot;-1;;734;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;3;False;9;FLOAT;1;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;320;860.7371,4715.267;Inherit;True;ColorShartSlot;-1;;735;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;2;False;9;FLOAT;2;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;311;556.5591,3968.35;Float;False;Property;_MRE7;MRE 7;22;0;Create;True;0;0;0;False;0;False;0,1,0,0;0,1,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;310;554.5208,4730.661;Float;False;Property;_MRE10;MRE 10;25;0;Create;True;0;0;0;False;0;False;0,1,0,0;0,1,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;317;866.0349,5786.639;Inherit;True;ColorShartSlot;-1;;737;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;2;False;9;FLOAT;1;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;315;851.7717,5565.973;Inherit;True;ColorShartSlot;-1;;738;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;1;False;9;FLOAT;1;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;322;862.8147,3533.231;Inherit;True;ColorShartSlot;-1;;739;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;1;False;9;FLOAT;3;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;294;561.9038,6013.687;Float;False;Property;_MRE15;MRE 15;30;0;Create;True;0;0;0;False;0;False;0,1,0,0;0,1,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;313;593.4414,2625.924;Float;False;Property;_MRE1;MRE 1;16;0;Create;True;0;0;0;False;1;Header(Metallic(R) Rough(G) Emmission(B));False;0,1,0,0;0,1,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;312;554.5156,4184.477;Float;False;Property;_MRE8;MRE 8;23;0;Create;True;0;0;0;False;0;False;0,1,0,0;0,1,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;314;600.7974,3259.348;Float;False;Property;_MRE4;MRE 4;19;0;Create;True;0;0;0;False;0;False;0,1,0,0;0,1,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;330;858.7087,6224.051;Inherit;True;ColorShartSlot;-1;;740;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;4;False;9;FLOAT;1;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;331;846.4747,4494.602;Inherit;True;ColorShartSlot;-1;;742;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;1;False;9;FLOAT;2;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;321;873.4221,3262.493;Inherit;True;ColorShartSlot;-1;;741;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;4;False;9;FLOAT;4;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;308;597.8821,3050.848;Float;False;Property;_MRE3;MRE 3;18;0;Create;True;0;0;0;False;0;False;0,1,0,0;0,1,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;309;560.7014,3530.267;Float;False;Property;_MRE5;MRE 5;20;0;Create;True;0;0;0;False;1;Space(10);False;0,1,0,0;0,1,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;354;2243.338,1191.311;Inherit;True;True;False;False;False;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;343;1955.269,393.4111;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;353;3107.01,689.065;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;351;2787.695,1612.243;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ComponentMaskNode;349;2249.315,1389.258;Inherit;True;False;True;False;False;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;345;2197.731,1690.528;Inherit;False;Property;_EmissionPower1;Emission Power;32;0;Create;True;0;0;0;False;1;Header(Emmision);False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;350;2501.627,1677.187;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;347;2203.216,104.3341;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ComponentMaskNode;346;2187.081,1825.551;Inherit;True;False;False;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;338;1333.067,513.5909;Float;False;Property;_GradientPower;Gradient Power;38;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;344;1633.28,3281.224;Inherit;True;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;335;1245.005,5733.872;Inherit;True;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ComponentMaskNode;340;1733.138,385.1413;Inherit;False;False;False;False;True;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;339;1226.362,3693.714;Inherit;True;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;341;1603.028,106.3281;Inherit;True;3;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;326;1328.576,417.155;Float;False;Property;_GradientOffset;Gradient Offset;37;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;355;2617.921,1385.25;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;337;1242.845,2960.129;Inherit;True;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;336;1239.708,4662.5;Inherit;True;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;334;862.1199,3956.216;Inherit;True;ColorShartSlot;-1;;743;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;3;False;9;FLOAT;3;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;333;875.3811,2841.581;Inherit;True;ColorShartSlot;-1;;744;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;1,1,1,1;False;3;FLOAT;2;False;9;FLOAT;4;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;348;2469.75,101.3337;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,1;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;272;-522.8309,1407.066;Float;False;Property;_Color6;Color 6;5;0;Create;True;0;0;0;False;0;False;0.2720588,0.1294625,0,0.097;1,0.4519259,0.152941,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;304;561.4189,3756.696;Float;False;Property;_MRE6;MRE 6;21;0;Create;True;0;0;0;False;0;False;0,1,0,0;0,1,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;280;-207.1133,1411.907;Inherit;True;ColorShartSlot;-1;;745;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;2;False;9;FLOAT;3;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;266;-556.0259,3635.911;Float;False;Property;_Color15;Color 15;14;0;Create;True;0;0;0;False;0;False;1,0,0,0.391;1,0,0,0.391;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;306;549.3809,4496.889;Float;False;Property;_MRE9;MRE 9;24;0;Create;True;0;0;0;False;1;Space(10);False;0,1,0,0;0,1,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;274;-551.3253,2140.742;Float;False;Property;_Color9;Color 9;8;0;Create;True;0;0;0;False;1;Space(10);False;0.3164301,0,0.7058823,0.134;0.152941,0.9929401,1,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;271;-522.5457,870.9286;Float;False;Property;_Color4;Color 4;3;0;Create;True;0;0;0;False;0;False;0.1544118,0.5451319,1,0.253;0.9533468,1,0.1544116,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;275;-534.6641,392.7433;Float;False;Property;_Color2;Color 2;1;0;Create;True;0;0;0;False;0;False;1,0.1544118,0.8017241,0.253;1,0.1544116,0.8017241,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;269;-547.1475,2877.121;Float;False;Property;_Color12;Color 12;11;0;Create;True;0;0;0;False;0;False;0.5073529,0.1574544,0,0.128;0.02270761,0.1632712,0.2205881,0.484;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;268;-559.2659,2400.1;Float;False;Property;_Color10;Color 10;9;0;Create;True;0;0;0;False;0;False;0.362069,0.4411765,0,0.759;0.1544117,0.1602433,1,0.341;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;284;-193.0711,1888.928;Inherit;True;ColorShartSlot;-1;;747;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;4;False;9;FLOAT;3;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;259;532.3035,61.52296;Inherit;True;Property;_Gradient;Gradient;33;1;[SingleLineTexture];Create;True;0;0;0;False;1;Header(Gradient);False;-1;0f424a347039ef447a763d3d4b4782b0;0f424a347039ef447a763d3d4b4782b0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;262;-510.7125,1884.087;Float;False;Property;_Color8;Color 8;7;0;Create;True;0;0;0;False;0;False;0.4849697,0.5008695,0.5073529,0.078;0.1544116,0.1602432,1,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;279;-235.6077,2145.583;Inherit;True;ColorShartSlot;-1;;746;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;1;False;9;FLOAT;2;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;273;-591.2968,3879.068;Float;False;Property;_Color16;Color 16;15;0;Create;True;0;0;0;False;0;False;0.4080882,0.75,0.4811866,0.134;0.4080881,0.75,0.4811865,0.134;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;270;518.8171,472.1493;Float;False;Property;_GradientIntensity;Gradient Intensity;34;0;Create;True;0;0;0;False;0;False;1;0.75;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;263;-545.7383,2633.965;Float;False;Property;_Color11;Color 11;10;0;Create;True;0;0;0;False;0;False;0.6691177,0.6691177,0.6691177,0.647;1,0.1544117,0.3818459,0.316;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;257;-514.8902,1147.708;Float;False;Property;_Color5;Color 5;4;0;Create;True;0;0;0;False;1;Space(10);False;0.9533468,1,0.1544118,0.553;0.2669382,0.3207545,0.0226949,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;261;-521.1363,626.6081;Float;False;Property;_Color3;Color 3;2;0;Create;True;0;0;0;False;0;False;0.2535501,0.1544118,1,0.541;0.2535499,0.1544116,1,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;258;-561.6131,3142.689;Float;False;Property;_Color13;Color 13;12;0;Create;True;0;0;0;False;1;Space(10);False;1,0.5586207,0,0.272;1,0.5586207,0,0.272;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;265;-526.7234,133.3854;Float;False;Property;_Color1;Color 1;0;0;Create;True;0;0;0;False;1;Header(Albedo (A Gradient));False;1,0.1544118,0.1544118,0.291;1,0.1544116,0.1544116,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;260;-569.5535,3402.046;Float;False;Property;_Color14;Color 14;13;0;Create;True;0;0;0;False;0;False;0,0.8025862,0.875,0.047;0,0.8025862,0.875,0.047;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;267;-509.303,1640.931;Float;False;Property;_Color7;Color 7;6;0;Create;True;0;0;0;False;0;False;0.1544118,0.6151115,1,0.178;0.9099331,0.9264706,0.6267301,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;276;-230.0205,2638.806;Inherit;True;ColorShartSlot;-1;;748;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;3;False;9;FLOAT;2;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;283;-239.794,3883.909;Inherit;True;ColorShartSlot;-1;;749;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;4;False;9;FLOAT;1;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;264;547.1333,288.8422;Float;False;Property;_GradientColor;Gradient Color;35;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;307;554.6787,5568.261;Float;False;Property;_MRE13;MRE 13;28;0;Create;True;0;0;0;False;1;Space(10);False;0,1,0,0;0,1,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;301;554.292,4942.316;Float;False;Property;_MRE11;MRE 11;26;0;Create;True;0;0;0;False;0;False;0,1,0,0;0,1,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;342;1910.574,105.6353;Inherit;True;True;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;298;552.2485,5158.444;Float;False;Property;_MRE12;MRE 12;27;0;Create;True;0;0;0;False;0;False;0,1,0,0;0,1,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;300;557.5464,6229.815;Float;False;Property;_MRE16;MRE 16;31;0;Create;True;0;0;0;False;0;False;0,1,0,0;0,1,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;299;359.8215,1720.239;Inherit;True;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;278;-199.1724,1152.549;Inherit;True;ColorShartSlot;-1;;750;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;1;False;9;FLOAT;3;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;282;-253.836,3404.727;Inherit;True;ColorShartSlot;-1;;751;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;2;False;9;FLOAT;1;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;297;559.8184,5802.033;Float;False;Property;_MRE14;MRE 14;29;0;Create;True;0;0;0;False;0;False;0,1,0,0;0,1,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;295;1040.498,75.77015;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;302;361.3183,1187.068;Inherit;True;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;305;601.9778,2839.576;Float;False;Property;_MRE2;MRE 2;17;0;Create;True;0;0;0;False;0;False;0,1,0,0;0,1,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;303;362.0073,1453.096;Inherit;True;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;296;356.4282,1984.446;Inherit;True;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;287;-240.3081,3640.752;Inherit;True;ColorShartSlot;-1;;753;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;3;False;9;FLOAT;1;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;291;-193.5855,1645.772;Inherit;True;ColorShartSlot;-1;;752;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;3;False;9;FLOAT;3;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;277;-229.5064,2881.962;Inherit;True;ColorShartSlot;-1;;759;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;4;False;9;FLOAT;2;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;293;-245.8953,3147.53;Inherit;True;ColorShartSlot;-1;;758;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;1;False;9;FLOAT;1;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;288;-204.9043,874.6049;Inherit;True;ColorShartSlot;-1;;757;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;4;False;9;FLOAT;4;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;290;-211.0059,138.2261;Inherit;True;ColorShartSlot;-1;;760;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;1;False;9;FLOAT;4;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;289;855.199,86.99012;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;292;829.6819,391.2721;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;286;-205.4187,631.4487;Inherit;True;ColorShartSlot;-1;;755;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;3;False;9;FLOAT;4;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;285;-218.9466,397.584;Inherit;True;ColorShartSlot;-1;;754;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;2;False;9;FLOAT;4;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;281;-243.5486,2404.941;Inherit;True;ColorShartSlot;-1;;756;231fe18505db4a84b9c478d379c9247d;0;5;38;COLOR;0.7843138,0.3137255,0,0;False;3;FLOAT;2;False;9;FLOAT;2;False;7;FLOAT;4;False;8;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;360;3722.291,1294.892;Float;False;False;-1;2;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Meta;0;4;Meta;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;2;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Meta;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;361;3722.291,1294.892;Float;False;False;-1;2;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Universal2D;0;5;Universal2D;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;2;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;True;1;1;False;-1;0;False;-1;1;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Universal2D;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;365;3722.291,1294.892;Float;False;False;-1;2;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ScenePickingPass;0;9;ScenePickingPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;2;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Picking;False;True;4;d3d11;glcore;gles;gles3;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;363;3722.291,1294.892;Float;False;False;-1;2;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;GBuffer;0;7;GBuffer;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;2;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;True;1;1;False;-1;0;False;-1;1;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;False;True;1;LightMode=UniversalGBuffer;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;364;3722.291,1294.892;Float;False;False;-1;2;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;SceneSelectionPass;0;8;SceneSelectionPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;2;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=SceneSelectionPass;False;True;4;d3d11;glcore;gles;gles3;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;356;3722.291,1294.892;Float;False;False;-1;2;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ExtraPrePass;0;0;ExtraPrePass;5;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;2;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;0;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;357;3722.291,1294.892;Float;False;True;-1;2;ASEMaterialInspector;0;2;Malbers/Color4x4;94348b07e5e8bab40bd6c8a1e3df54cd;True;Forward;0;1;Forward;19;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;2;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;2;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;True;1;1;False;-1;0;False;-1;1;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;False;False;True;1;LightMode=UniversalForward;False;False;0;;0;0;Standard;40;Workflow;1;0;Surface;0;0;  Refraction Model;0;0;  Blend;0;0;Two Sided;0;637780419590991176;Fragment Normal Space,InvertActionOnDeselection;0;0;Transmission;0;0;  Transmission Shadow;0.5,False,-1;0;Translucency;0;0;  Translucency Strength;1,False,-1;0;  Normal Distortion;0.5,False,-1;0;  Scattering;2,False,-1;0;  Direct;0.9,False,-1;0;  Ambient;0.1,False,-1;0;  Shadow;0.5,False,-1;0;Cast Shadows;1;0;  Use Shadow Threshold;0;0;Receive Shadows;1;0;GPU Instancing;1;0;LOD CrossFade;1;0;Built-in Fog;1;0;_FinalColorxAlpha;0;0;Meta Pass;1;0;Override Baked GI;0;0;Extra Pre Pass;0;0;DOTS Instancing;0;0;Tessellation;0;0;  Phong;0;0;  Strength;0.5,False,-1;0;  Type;0;0;  Tess;16,False,-1;0;  Min;10,False,-1;0;  Max;25,False,-1;0;  Edge Length;16,False,-1;0;  Max Displacement;25,False,-1;0;Write Depth;0;0;  Early Z;0;0;Vertex Position,InvertActionOnDeselection;1;0;Debug Display;0;0;Clear Coat;0;0;0;10;False;True;True;True;True;True;True;True;True;True;False;;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;358;3722.291,1294.892;Float;False;False;-1;2;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;2;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;False;False;True;False;False;False;False;0;False;-1;False;False;False;False;False;False;False;False;False;True;1;False;-1;True;3;False;-1;False;True;1;LightMode=ShadowCaster;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;359;3722.291,1294.892;Float;False;False;-1;2;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;DepthOnly;0;3;DepthOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;2;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;False;False;True;False;False;False;False;0;False;-1;False;False;False;False;False;False;False;False;False;True;1;False;-1;False;False;True;1;LightMode=DepthOnly;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;362;3722.291,1294.892;Float;False;False;-1;2;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;DepthNormals;0;6;DepthNormals;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;2;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;-1;True;3;False;-1;False;True;1;LightMode=DepthNormals;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
WireConnection;318;38;312;0
WireConnection;327;38;301;0
WireConnection;328;0;295;0
WireConnection;332;38;298;0
WireConnection;329;0;302;0
WireConnection;329;1;303;0
WireConnection;329;2;299;0
WireConnection;329;3;296;0
WireConnection;324;38;313;0
WireConnection;323;38;308;0
WireConnection;319;38;304;0
WireConnection;316;38;294;0
WireConnection;320;38;310;0
WireConnection;317;38;297;0
WireConnection;315;38;307;0
WireConnection;322;38;309;0
WireConnection;330;38;300;0
WireConnection;331;38;306;0
WireConnection;321;38;314;0
WireConnection;354;0;344;0
WireConnection;343;0;340;0
WireConnection;353;0;348;0
WireConnection;353;1;329;0
WireConnection;351;0;329;0
WireConnection;351;1;350;0
WireConnection;349;0;344;0
WireConnection;350;0;345;0
WireConnection;350;1;346;0
WireConnection;347;0;342;0
WireConnection;347;1;343;0
WireConnection;346;0;344;0
WireConnection;344;0;337;0
WireConnection;344;1;339;0
WireConnection;344;2;336;0
WireConnection;344;3;335;0
WireConnection;335;0;315;0
WireConnection;335;1;317;0
WireConnection;335;2;316;0
WireConnection;335;3;330;0
WireConnection;340;0;329;0
WireConnection;339;0;322;0
WireConnection;339;1;319;0
WireConnection;339;2;334;0
WireConnection;339;3;318;0
WireConnection;341;0;328;0
WireConnection;341;1;325;0
WireConnection;341;2;326;0
WireConnection;355;0;349;0
WireConnection;337;0;324;0
WireConnection;337;1;333;0
WireConnection;337;2;323;0
WireConnection;337;3;321;0
WireConnection;336;0;331;0
WireConnection;336;1;320;0
WireConnection;336;2;327;0
WireConnection;336;3;332;0
WireConnection;334;38;311;0
WireConnection;333;38;305;0
WireConnection;348;0;347;0
WireConnection;280;38;272;0
WireConnection;284;38;262;0
WireConnection;259;1;256;0
WireConnection;279;38;274;0
WireConnection;276;38;263;0
WireConnection;283;38;273;0
WireConnection;342;0;341;0
WireConnection;342;1;338;0
WireConnection;299;0;279;0
WireConnection;299;1;281;0
WireConnection;299;2;276;0
WireConnection;299;3;277;0
WireConnection;278;38;257;0
WireConnection;282;38;260;0
WireConnection;295;0;289;0
WireConnection;295;1;292;0
WireConnection;302;0;290;0
WireConnection;302;1;285;0
WireConnection;302;2;286;0
WireConnection;302;3;288;0
WireConnection;303;0;278;0
WireConnection;303;1;280;0
WireConnection;303;2;291;0
WireConnection;303;3;284;0
WireConnection;296;0;293;0
WireConnection;296;1;282;0
WireConnection;296;2;287;0
WireConnection;296;3;283;0
WireConnection;287;38;266;0
WireConnection;291;38;267;0
WireConnection;277;38;269;0
WireConnection;293;38;258;0
WireConnection;288;38;271;0
WireConnection;290;38;265;0
WireConnection;289;0;259;0
WireConnection;289;1;264;0
WireConnection;292;0;270;0
WireConnection;286;38;261;0
WireConnection;285;38;275;0
WireConnection;281;38;268;0
WireConnection;357;0;353;0
WireConnection;357;2;351;0
WireConnection;357;3;354;0
WireConnection;357;4;355;0
ASEEND*/
//CHKSM=57AB53363262F60884E3C1F4E152087546A34B9C