Shader "DirtyLens_01_Geo"
{
	Properties 
	{
_FlareMultiplier("_FlareMultiplier", Float) = 10
_Flare("_Flare", 2D) = "black" {}
_DirtyLensTexture("_DirtyLensTexture", 2D) = "black" {}
_FullScreenMask("_FullScreenMask", 2D) = "black" {}

	}
	
	SubShader 
	{
		Tags
		{
"Queue"="Transparent"
"IgnoreProjector"="True"
"RenderType"="Transparent"

		}

		
Cull Back
ZWrite On
ZTest LEqual
ColorMask RGBA
Fog{
}


		CGPROGRAM
#pragma surface surf BlinnPhongEditor  nolightmap noforwardadd alpha decal:add vertex:vert
#pragma target 2.0


float _FlareMultiplier;
sampler2D _Flare;
sampler2D _DirtyLensTexture;
sampler2D _FullScreenMask;

			struct EditorSurfaceOutput {
				half3 Albedo;
				half3 Normal;
				half3 Emission;
				half3 Gloss;
				half Specular;
				half Alpha;
				half4 Custom;
			};
			
			inline half4 LightingBlinnPhongEditor_PrePass (EditorSurfaceOutput s, half4 light)
			{
half3 spec = light.a * s.Gloss;
half4 c;
c.rgb = (s.Albedo * light.rgb + light.rgb * spec);
c.a = s.Alpha;
return c;

			}

			inline half4 LightingBlinnPhongEditor (EditorSurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
			{
				half3 h = normalize (lightDir + viewDir);
				
				half diff = max (0, dot ( lightDir, s.Normal ));
				
				float nh = max (0, dot (s.Normal, h));
				float spec = pow (nh, s.Specular*128.0);
				
				half4 res;
				res.rgb = _LightColor0.rgb * diff;
				res.w = spec * Luminance (_LightColor0.rgb);
				res *= atten * 2.0;

				return LightingBlinnPhongEditor_PrePass( s, res );
			}
			
			struct Input {
				float2 uv_Flare;
float4 screenPos;

			};

			void vert (inout appdata_full v, out Input o) {
float4 VertexOutputMaster0_0_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_1_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_2_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_3_NoInput = float4(0,0,0,0);


			}
			

			void surf (Input IN, inout EditorSurfaceOutput o) {
				o.Normal = float3(0.0,0.0,1.0);
				o.Alpha = 1.0;
				o.Albedo = 0.0;
				o.Emission = 0.0;
				o.Gloss = 0.0;
				o.Specular = 0.0;
				o.Custom = 0.0;
				
float4 Tex2D0=tex2D(_Flare,(IN.uv_Flare.xyxy).xy);
float4 Multiply1=Tex2D0 * _FlareMultiplier.xxxx;
float4 Tex2D2=tex2D(_FullScreenMask,((IN.screenPos.xy/IN.screenPos.w).xyxy).xy);
float4 Tex2D1=tex2D(_DirtyLensTexture,((IN.screenPos.xy/IN.screenPos.w).xyxy).xy);
float4 Multiply4=Tex2D2 * Tex2D1;
float4 Multiply3=Multiply1 * Multiply4;
float4 Multiply0=Multiply3 * Multiply4;
float4 Master0_1_NoInput = float4(0,0,1,1);
float4 Master0_2_NoInput = float4(0,0,0,0);
float4 Master0_3_NoInput = float4(0,0,0,0);
float4 Master0_4_NoInput = float4(0,0,0,0);
float4 Master0_5_NoInput = float4(1,1,1,1);
float4 Master0_7_NoInput = float4(0,0,0,0);
float4 Master0_6_NoInput = float4(1,1,1,1);
o.Albedo = Multiply0;

				o.Normal = normalize(o.Normal);
			}
		ENDCG
	}
	Fallback ""
}