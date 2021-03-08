// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "FXLab/PostProcessing/Blur" {
    Properties {
		_FXScreenTexture ("Screen Texture (FXScreenBufferTexture)", 2D) = "" {}
		_BlurRange ("Blur Range", Float) = 10
		_BlurSteps ("Blur Steps", Range(1, 10)) = 10		
	}
	SubShader {
		Blend SrcAlpha OneMinusSrcAlpha  
		Tags { "Queue"="Overlay" "IgnoreProjector"="True" "RenderType"="Opaque"}
		Lighting Off
		Cull Off
		Fog { Mode Off }
		ZWrite Off
						
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag 
			#pragma target 3.0
			#pragma glsl
			
			#include "UnityCG.cginc"
			
			#define FORCE_TEX2DLOD
			#include "../FXLab.cginc"
			
			float _BlurSteps;
			float _BlurRange;

			struct appdata {
				float4 vertex : POSITION;
				float4 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};
									
			v2f vert (appdata v) {
				v2f o;
				o.pos = UnityObjectToClipPos( v.vertex );
				o.uv = v.texcoord.xy;
				return o;
			}
					
			fixed4 frag( v2f o ) : COLOR
			{
				_BlurSteps = floor(_BlurSteps);
				float3 color = 0;
				for (int i = 0; i < _BlurSteps; ++i)
				{
					float factor = (i + 1) / _BlurSteps;
					color += sampleScreenBlurred(o.uv, _BlurRange / 1000 * factor);
				}
				color /= _BlurSteps;
			
				return fixed4(color, 1);
			}
			ENDCG
		}
	}
    Fallback off
	CustomEditor "FXMaterialEditor"
}
