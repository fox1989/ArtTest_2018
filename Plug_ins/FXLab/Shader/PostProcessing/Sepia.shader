// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "FXLab/PostProcessing/Sepia" {
	Properties {
		_FXScreenTexture ("Screen Texture (FXScreenBufferTexture)", 2D) = "" {}
	}

	SubShader {
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
					
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest 
			#include "UnityCG.cginc"
			#include "../FXLab.cginc"

			struct v2f {
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct appdata {
				float4 vertex : POSITION;
				float4 texcoord : TEXCOORD0;
			};

			v2f vert( appdata v ) {
				v2f o;
				o.pos = UnityObjectToClipPos (v.vertex);
				o.uv = v.texcoord.xy;
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{	
				fixed3 original = sampleScreen(i.uv);
				
				// get intensity value (Y part of YIQ color space)
				fixed Y = dot (fixed3(0.299, 0.587, 0.114), original.rgb);

				// Convert to Sepia Tone by adding constant
				fixed4 sepiaConvert = float4 (0.191, -0.054, -0.221, 0.0);
				fixed4 output = sepiaConvert + Y;
				output.a = 1;
				
				return output;
			}
			ENDCG

		}
	}

	Fallback off
	CustomEditor "FXMaterialEditor"
}