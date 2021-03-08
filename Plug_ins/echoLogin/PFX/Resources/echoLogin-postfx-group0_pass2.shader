// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "echoLogin/PFX/echologin_postfx_group0_pass2"
{
//=========================================================================
SubShader
{
Tags { "Queue" = "Geometry" "IgnoreProjector"="True" "RenderType"="Opaque" }

Pass
{
Blend Off
ZTest Always
Cull Off
ZWrite Off
Fog { Mode Off }
Lighting Off

CGPROGRAM

#pragma vertex vert
#pragma fragment frag
#pragma exclude_renderers flash
#pragma fragmentoption ARB_precision_hint_fastest


#pragma multi_compile ECHO_PFX_MULT_OFF ECHO_PFX_MULT_ON
#pragma multi_compile ECHO_PFX_NOISE_OFF ECHO_PFX_NOISE_ON
#pragma multi_compile ECHO_PFX_SCANLINE_OFF ECHO_PFX_SCANLINE_ON
#pragma multi_compile ECHO_PFX_OVERLAY_MUL_OFF ECHO_PFX_OVERLAY_MUL_ON
#pragma multi_compile ECHO_PFX_OVERLAY_ADD_OFF ECHO_PFX_OVERLAY_ADD_ON

#ifdef OVERLAY_NORMAL_TC
#ifdef OVERLAY_SCR_TC
#define ECHODEF_TC2_SIZE half4
#define ECHODEF_OVERLAY_TC v.tc2.xy
#define ECHODEF_OVERLAY_SCR_TC v.tc2.wz
#else
#define ECHODEF_TC2_SIZE half2
#define ECHODEF_OVERLAY_TC v.tc2.xy
#define ECHODEF_OVERLAY_SCR_TC v.tc1.xy
#endif
#else
#ifdef OVERLAY_SCR_TC
#define ECHODEF_TC2_SIZE half2
#define ECHODEF_OVERLAY_TC v.tc1.xy
#define ECHODEF_OVERLAY_SCR_TC v.tc2.xy
#else
#define ECHODEF_OVERLAY_TC v.tc1.xy
#define ECHODEF_OVERLAY_SCR_TC v.tc1.xy
#endif
#endif

#ifdef OVERLAY_ADD_TC
#ifdef OVERLAY_SUB_TC
#define ECHODEF_TC3_SIZE half4
#define ECHODEF_OVERLAY_ADD_TC v.tc3.xy
#define ECHODEF_OVERLAY_SUB_TC v.tc3.wz
#else
#define ECHODEF_TC3_SIZE half2
#define ECHODEF_OVERLAY_ADD_TC v.tc3.xy
#define ECHODEF_OVERLAY_SUB_TC v.tc1.xy
#endif
#else
#ifdef OVERLAY_SUB_TC
#define ECHODEF_TC3_SIZE half2
#define ECHODEF_OVERLAY_ADD_TC v.tc1.xy
#define ECHODEF_OVERLAY_SUB_TC v.tc3.xy
#else
#define ECHODEF_OVERLAY_ADD_TC v.tc1.xy
#define ECHODEF_OVERLAY_SUB_TC v.tc1.xy
#endif
#endif


#ifdef OVERLAY_MUL_TC
#ifdef OVERLAY_OVR_TC
#define ECHODEF_TC4_SIZE half4
#define ECHODEF_OVERLAY_MUL_TC v.tc4.xy
#define ECHODEF_OVERLAY_OVR_TC v.tc4.wz
#else
#define ECHODEF_TC4_SIZE half2
#define ECHODEF_OVERLAY_MUL_TC v.tc4.xy
#define ECHODEF_OVERLAY_OVR_TC v.tc1.xy
#endif
#else
#ifdef OVERLAY_OVR_TC
#define ECHODEF_TC4_SIZE half2
#define ECHODEF_OVERLAY_MUL_TC v.tc1.xy
#define ECHODEF_OVERLAY_OVR_TC v.tc4.xy
#else
#define ECHODEF_OVERLAY_MUL_TC v.tc1.xy
#define ECHODEF_OVERLAY_OVR_TC v.tc1.xy
#endif
#endif

#ifdef CUSTOM_FRAG_1_TC
#ifdef CUSTOM_FRAG_2_TC
#define ECHODEF_TC5_SIZE half4
#define ECHODEF_CUSTOM1_TC v.tc5.xy
#define ECHODEF_CUSTOM2_TC v.tc5.wz
#else
#define ECHODEF_TC5_SIZE half2
#define ECHODEF_CUSTOM1_TC v.tc5.xy
#define ECHODEF_CUSTOM2_TC v.tc1.xy
#endif
#else
#ifdef CUSTOM_FRAG_2_TC
#define ECHODEF_TC5_SIZE half2
#define ECHODEF_CUSTOM1_TC v.tc1.xy
#define ECHODEF_CUSTOM2_TC v.tc5.xy
#else
#define ECHODEF_CUSTOM1_TC v.tc1.xy
#define ECHODEF_CUSTOM2_TC v.tc1.xy
#endif
#endif

#ifdef CUSTOM_FRAG_3_TC
#ifdef CUSTOM_FRAG_4_TC
#define ECHODEF_TC6_SIZE half4
#define ECHODEF_CUSTOM3_TC v.tc5.xy
#define ECHODEF_CUSTOM4_TC v.tc5.wz
#else
#define ECHODEF_TC6_SIZE half2
#define ECHODEF_CUSTOM3_TC v.tc5.xy
#define ECHODEF_CUSTOM4_TC v.tc1.xy
#endif
#else
#ifdef CUSTOM_FRAG_3_TC
#define ECHODEF_TC6_SIZE half2
#define ECHODEF_CUSTOM3_TC v.tc1.xy
#define ECHODEF_CUSTOM4_TC v.tc5.xy
#else
#define ECHODEF_CUSTOM3_TC v.tc1.xy
#define ECHODEF_CUSTOM4_TC v.tc1.xy
#endif
#endif

#define PI 3.141592

sampler2D	_echoScreen;
float4		_echoScreen_ST;
float4 		_echoScreen_TexelSize;

sampler2D   _echoRampTex;
sampler2D   _NoiseTex;
sampler2D   echoCorrectTex;


#ifdef ECHO_PFX_OVERLAY_ON
fixed 		_echoOverlayFade;
sampler2D   _OverlayTex;
float4      _OverlayTex_ST;
#endif

#ifdef ECHO_PFX_OVERLAY_SCR_ON
fixed 		_echoOverlayScrFade;
sampler2D   _OverlayTexScr;
float4      _OverlayTexScr_ST;
#endif

#ifdef ECHO_PFX_OVERLAY_ADD_ON
fixed 		_echoOverlayAddFade;
sampler2D   _OverlayTexAdd;
float4      _OverlayTexAdd_ST;
#endif

#ifdef ECHO_PFX_OVERLAY_SUB_ON
fixed 		_echoOverlaySubFade;
sampler2D   _OverlayTexSub;
float4      _OverlayTexSub_ST;
#endif

#ifdef ECHO_PFX_OVERLAY_MUL_ON
fixed 		_echoOverlayMulFade;
sampler2D   _OverlayTexMul;
float4      _OverlayTexMul_ST;
#endif

#ifdef ECHO_PFX_OVERLAY_OVR_ON
fixed 		_echoOverlayOvrFade;
sampler2D   _OverlayTexOvr;
float4      _OverlayTexOvr_ST;
#endif

#ifdef ECHO_PFX_DISTORT_ON
float4      _echoDistParamsH;  // x = speed, y = perioid, z = amplitude, w = fade
float4      _echoDistParamsV;  // x = speed, y = perioid, z = amplitude. w = fade
#endif

#ifdef ECHO_PFX_SHOCKWAVE_ON
float4      _echoShockParams;  // xy = uv, z = dist, w = size
float       _echoShockFade;
#endif

fixed 		_echoGreyFade;
fixed 		_echoInverseFade;
fixed4 		_echoColor;
fixed 		_echoColorFade;
fixed4 		_echoAdd;
fixed       _echoAddFade;
fixed4 		_echoMult;
fixed 		_echoMultFade;
fixed 		_echoNoiseFade;
fixed 		_echoThermalFade;

fixed 		_echoScanLineFade;
half 		_echoScanLineCountH;
half 		_echoScanLineScrollH;
half 		_echoScanLineCountV;
half 		_echoScanLineScrollV;

float       _echoRampFade;
float       _echoCorrectFade;

sampler2D	_echoCustomF1Tex;
float4      _echoCustomF1Tex_ST;
fixed 		_echoCustomF1Fade;
fixed4      _echoCustomF1Args;
fixed4      _echoCustomF1Color;

sampler2D	_echoCustomF2Tex;
float4      _echoCustomF2Tex_ST;
fixed 		_echoCustomF2Fade;
fixed4      _echoCustomF2Args;
fixed4      _echoCustomF2Color;

sampler2D	_echoCustomF3Tex;
float4      _echoCustomF3Tex_ST;
fixed 		_echoCustomF3Fade;
fixed4      _echoCustomF3Args;
fixed4      _echoCustomF3Color;

sampler2D	_echoCustomF4Tex;
float4      _echoCustomF4Tex_ST;
fixed 		_echoCustomF4Fade;
fixed4      _echoCustomF4Args;
fixed4      _echoCustomF4Color;

// =============================================
struct VertInput
{
float4 vertex	: POSITION;
float2 texcoord	: TEXCOORD0;
float4 color    : COLOR;
};

// =============================================
struct Varys
{
half4 pos		: SV_POSITION;

#ifdef ECHO_PFX_NOISE_ON
half4 tc1	: TEXCOORD0;
#else
half2 tc1	: TEXCOORD0;
#endif

#ifdef ECHODEF_TC2_SIZE
ECHODEF_TC2_SIZE tc2 : TEXCOORD1;
#endif

#ifdef ECHODEF_TC3_SIZE
ECHODEF_TC3_SIZE tc3 : TEXCOORD2;
#endif

#ifdef ECHODEF_TC4_SIZE
ECHODEF_TC4_SIZE tc4 : TEXCOORD3;
#endif

#ifdef ECHODEF_TC5_SIZE
ECHODEF_TC5_SIZE tc5 : TEXCOORD4;
#endif

#ifdef ECHODEF_TC6_SIZE
ECHODEF_TC5_SIZE tc6 : TEXCOORD5;
#endif

//                fixed3 color    : TEXCOORD1;
};

// =============================================
Varys vert ( VertInput ad )
{
Varys v;

#ifdef ECHO_PFX_SHOCKWAVE_ON
float dist = distance ( ad.texcoord.xy, _echoShockParams.xy );

float s1 = _echoShockParams.z - _echoShockParams.w;
float s2 = _echoShockParams.z + _echoShockParams.w;

if ( s1 > 0 && dist >= s1 && dist <= s2 )
{
float 	d 		= dist - _echoShockParams.z;
float 	dtime 	= d * ( 1.0 - pow ( abs ( d * 10.0 ), 0.8 ) );
float2 	duv 	= normalize ( ad.texcoord.xy - _echoShockParams.xy );
float 	uvadd 	= clamp ( duv * ( dtime ), -2, 2 );
ad.texcoord.xy += uvadd * _echoShockFade;
}
#endif

#ifdef ECHO_PFX_DISTORT_ON
ad.texcoord.x +=  sin ( ( _echoDistParamsV.y * ad.texcoord.y - ( _Time.y * _echoDistParamsV.x ) ) ) * _echoDistParamsV.z * _echoDistParamsV.w;
ad.texcoord.y +=  sin ( ( _echoDistParamsH.y * ad.texcoord.x - ( _Time.y * _echoDistParamsH.x ) ) ) * _echoDistParamsH.z * _echoDistParamsH.w;
#endif

#ifdef ECHO_PFX_NOISE_ON
v.tc1.z  	=	ad.texcoord.x + ( sin ( _Time.x * 2.1 * 128.0 ) * 0.5 ) + 0.5;
v.tc1.w  	=	ad.texcoord.y + ( cos ( _Time.z * 1.92 * 256.0 ) * 0.5 ) + 0.5;
#endif

#ifdef OVERLAY_NORMAL_TC
#ifdef ECHO_PFX_OVERLAY_ON
ECHODEF_OVERLAY_TC = _OverlayTex_ST.xy * ( ad.texcoord.xy + _OverlayTex_ST.zw );
#endif
#endif

#ifdef OVERLAY_SCR_TC
#ifdef ECHO_PFX_OVERLAY_SCR_ON
ECHODEF_OVERLAY_SCR_TC = _OverlayTexScr_ST.xy * ( ad.texcoord.xy + _OverlayTexScr_ST.zw );
#endif
#endif

#ifdef OVERLAY_ADD_TC
#ifdef ECHO_PFX_OVERLAY_ADD_ON
ECHODEF_OVERLAY_ADD_TC = _OverlayTexAdd_ST.xy * ( ad.texcoord.xy + _OverlayTexAdd_ST.zw );
#endif
#endif

#ifdef OVERLAY_SUB_TC
#ifdef ECHO_PFX_OVERLAY_SUB_ON
ECHODEF_OVERLAY_SUB_TC = _OverlayTexSub_ST.xy * ( ad.texcoord.xy + _OverlayTexSub_ST.zw );
#endif
#endif

#ifdef OVERLAY_MUL_TC
#ifdef ECHO_PFX_OVERLAY_MUL_ON
ECHODEF_OVERLAY_MUL_TC = _OverlayTexMul_ST.xy * ( ad.texcoord.xy + _OverlayTexMul_ST.zw );
#endif
#endif

#ifdef OVERLAY_OVR_TC
#ifdef ECHO_PFX_OVERLAY_OVR_ON
ECHODEF_OVERLAY_OVR_TC = _OverlayTexOvr_ST.xy * ( ad.texcoord.xy + _OverlayTexOvr_ST.zw );
#endif
#endif

v.pos			= UnityObjectToClipPos ( ad.vertex );
v.tc1.xy  		= _echoScreen_ST.xy * ( ad.texcoord.xy + _echoScreen_ST.zw );
// 				v.color         = ad.color;

return v;
}

// =============================================
fixed4 frag ( Varys v ):COLOR
{
fixed3 _ioRGB = tex2D ( _echoScreen, v.tc1.xy ).xyz;

#ifdef ECHO_PFX_MULT_ON
				_ioRGB.xyz *= lerp ( fixed3 ( 1, 1, 1 ), _ioRGB.xyz * _echoMult.xyz, _echoMultFade );
#endif


#ifdef ECHO_PFX_NOISE_ON
				fixed3 scolor 	= tex2D ( _NoiseTex, v.tc1.zw ).xyz;
				_ioRGB.xyz 		= lerp ( _ioRGB.xyz, scolor.xyz, _echoNoiseFade );
#endif


#if ECHO_PFX_SCANLINE_ON
				fixed scanval =  (fixed) ( (int)( v.tc1.y * _echoScanLineCountH + ( _Time.y * _echoScanLineScrollH ) ) % 2 );
				_ioRGB.xyz *= lerp ( fixed3(1,1,1), fixed3 ( scanval, scanval, scanval ), _echoScanLineFade );
#endif 




#ifdef ECHO_PFX_OVERLAY_MUL_ON
				fixed3 mocolor 	= tex2D ( _OverlayTexMul, ECHODEF_OVERLAY_MUL_TC ).xyz;
				// multiply
				_ioRGB.xyz 		= lerp ( _ioRGB.xyz, _ioRGB.xyz * mocolor, _echoOverlayMulFade );
#endif

#ifdef ECHO_PFX_OVERLAY_ADD_ON
				fixed3 aocolor 	= tex2D ( _OverlayTexAdd, ECHODEF_OVERLAY_ADD_TC ).xyz;
				_ioRGB.xyz 		= _ioRGB.xyz += lerp ( fixed3(0,0,0), aocolor.xyz, _echoOverlayAddFade );
#endif

return fixed4 ( _ioRGB, 1 );
}

ENDCG
}
}
}

