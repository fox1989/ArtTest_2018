// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "FXLab/PostProcessing/Chromatic Aberration" {
    Properties {
		_FXScreenTexture ("Screen Texture (FXScreenBufferTexture)", 2D) = "" {}
		_Shift ("Shift", Range(0, 20)) = 10
		_Bias ("Bias", Float) = -0.5
		_Size ("Size", Float) = 2
		_BlurSteps ("Blur Steps", Range(1, 10)) = 20		
	}
	SubShader {
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
			
			float _Shift;
			float _Bias;
			float _Size;
			float _BlurSteps;
			
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
			
			float2 applyChromatic(float2 dir, float factor)
			{
				factor *= saturate(saturate(length(dir) + _Bias) * _Size);
				factor = 1 - saturate(factor/100);
				dir *=  factor;
				return dir;
			}
					
			fixed4 frag( v2f o ) : COLOR
			{				
				float2 sampledPosition = o.uv;
				
				float2 spUv = o.uv * 2 - 1;
				
				_BlurSteps = floor(_BlurSteps);
				
				fixed red = sampleScreen(o.uv).r;
				float3 color = 0;
				
				for (int i = 0; i < _BlurSteps; ++i)
				{
					float factor = (i + 1) / _BlurSteps;
					float2 greenFactor = applyChromatic(spUv, _Shift * 0.5 * factor);
					float2 blueFactor = applyChromatic(spUv, _Shift * 1.0 * factor);
					
					float2 uvGreen = greenFactor * 0.5 + 0.5;
					float2 uvBlue = blueFactor * 0.5 + 0.5;
					
					float green = sampleScreen(uvGreen).g;
					float blue = sampleScreen(uvBlue).b;
					
					color += float3(red, green, blue);
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
