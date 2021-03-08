// (c) copyright echoLogin LLC 2013. All rights reserved.

using UnityEngine;
using System.Collections;

//*****************************************************************************
public enum ECHOPROP
{
	DISTPARAMSH = 0,
	DISTPARAMSV,
	SHOCKPARAMS,
	SHOCKFADE,
	GREYFADE,
	INVERSEFADE,
	COLOR,
	COLORFADE,
	ADD,
	ADDFADE,
	MULT,
	MULTFADE,
	NOISEFADE,
	CORRECTFADE,
	CORRECT_TEX,
	OVERLAY_FADE,
	OVERLAY_TEX,
	OVERLAY_ST,
	OVERLAYSCR_FADE,
	OVERLAYSCR_TEX,
	OVERLAYSCR_ST,
	OVERLAYADD_FADE,
	OVERLAYADD_TEX,
	OVERLAYADD_ST,
	OVERLAYSUB_FADE,
	OVERLAYSUB_TEX,
	OVERLAYSUB_ST,
	OVERLAYMUL_FADE,
	OVERLAYMUL_TEX,
	OVERLAYMUL_ST,
	OVERLAYOVR_FADE,
	OVERLAYOVR_TEX,
	OVERLAYOVR_ST,
	RAMPFADE,
	RAMP_TEX,
	SCANLINEFADE,
	SCANLINECOUNTH,  
	SCANLINESCROLLH,
	SCANLINECOUNTV,  
	SCANLINESCROLLV,
	CUSTOM_FRAG_1_FADE,
	CUSTOM_FRAG_1_ARGS,
	CUSTOM_FRAG_1_COLOR,
	CUSTOM_FRAG_1_TEX,
	CUSTOM_FRAG_1_ST,
	CUSTOM_FRAG_2_FADE,
	CUSTOM_FRAG_2_ARGS,
	CUSTOM_FRAG_2_COLOR,
	CUSTOM_FRAG_2_TEX,
	CUSTOM_FRAG_2_ST,
	CUSTOM_FRAG_3_FADE,
	CUSTOM_FRAG_3_ARGS,
	CUSTOM_FRAG_3_COLOR,
	CUSTOM_FRAG_3_TEX,
	CUSTOM_FRAG_3_ST,
	CUSTOM_FRAG_4_FADE,
	CUSTOM_FRAG_4_ARGS,
	CUSTOM_FRAG_4_COLOR,
	CUSTOM_FRAG_4_TEX,
	CUSTOM_FRAG_4_ST,
	COUNT
};

public enum ECHOPFXMATHS
{
	DIRECT,
	ADD,
	MULTIPLY,
	AVERAGE
};

//-----------------------------------------------------------------------------
public class EchoMaterialProperty
{
	public string   		name;
	public int      		numType;
	public bool 			active;
	public float 			floatVal;
	public Vector4 			vec4Val;
	public float 			floatVal_hold;
	public Vector4 			vec4Val_hold;
	public Texture  		texVal;
	public Texture  		texVal_hold;
	public int      		frameCount;
	public int      		propID;
	public ECHOPFXMATHS 	mathtOpt;
	public float            avgStart;

	//=========================================================================
	public EchoMaterialProperty ( string ipname, Material imat, Vector4 ivec, ECHOPFXMATHS imath )
	{
		name    	= ipname;
		vec4Val 	= ivec;
		active 		= true;
		numType 	= 1;
		frameCount 	= 0;
		mathtOpt    = imath;
		avgStart    = 1.0f;

		vec4Val_hold = vec4Val;
		vec4Val_hold.x *= -1;

		propID = Shader.PropertyToID ( ipname );
	}
	
	//=========================================================================
	public EchoMaterialProperty ( string ipname, Material imat, float ifloat, ECHOPFXMATHS imath )
	{
		name    	= ipname;
		floatVal 	= ifloat;
		active 		= true;
		numType 	= 0;
		frameCount 	= 0;
		mathtOpt    = imath;
		avgStart    = 1.0f;

		floatVal_hold = floatVal * -1;

		propID = Shader.PropertyToID ( ipname );
	}

	//=========================================================================
	public EchoMaterialProperty ( string ipname, Material imat )
	{
		name    	= ipname;
		texVal      = null;
		active 		= true;
		numType 	= 2;
		frameCount 	= 0;
		avgStart    = 1.0f;

		texVal_hold = null;

		propID = Shader.PropertyToID ( ipname );
	}
}

//-----------------------------------------------------------------------------
public class EchoPFXRenderMaterial
{
	public Material mat;
	private EchoMaterialProperty[]  _matProp;
	private Vector4 _zeroVec4 = new Vector4(0,0,0,0);
	private Vector4 _oneVec4 = new Vector4(1,1,1,1);


	//=========================================================================
	public EchoPFXRenderMaterial ( Material imat )
	{
		mat 									= imat;
		_matProp            					= new EchoMaterialProperty[(int)ECHOPROP.COUNT];

		_matProp[(int)ECHOPROP.DISTPARAMSH] 	= new EchoMaterialProperty ("_echoDistParamsH", imat, new Vector4 ( 0, 0, 0, 0 ), ECHOPFXMATHS.DIRECT );
		_matProp[(int)ECHOPROP.DISTPARAMSV] 	= new EchoMaterialProperty ("_echoDistParamsV", imat, new Vector4 ( 0, 0, 0, 0 ), ECHOPFXMATHS.DIRECT );
		_matProp[(int)ECHOPROP.SHOCKPARAMS]		= new EchoMaterialProperty ("_echoShockParams", imat, new Vector4 ( 0, 0, 0, 0 ), ECHOPFXMATHS.DIRECT );
		_matProp[(int)ECHOPROP.SHOCKFADE]    	= new EchoMaterialProperty ("_echoShockFade", imat, 0, ECHOPFXMATHS.AVERAGE );

		_matProp[(int)ECHOPROP.GREYFADE]    	= new EchoMaterialProperty ( "_echoGreyFade", imat, 0, ECHOPFXMATHS.ADD );
		_matProp[(int)ECHOPROP.INVERSEFADE] 	= new EchoMaterialProperty ( "_echoInverseFade", imat, 0, ECHOPFXMATHS.AVERAGE  );
		
		_matProp[(int)ECHOPROP.COLOR]			= new EchoMaterialProperty ( "_echoColor", imat, new Vector4 ( 1, 1, 1, 1 ), ECHOPFXMATHS.ADD );
		_matProp[(int)ECHOPROP.COLORFADE]   	= new EchoMaterialProperty ( "_echoColorFade", imat, 0, ECHOPFXMATHS.AVERAGE  );
		
		_matProp[(int)ECHOPROP.ADD]				= new EchoMaterialProperty ( "_echoAdd", imat, new Vector4 ( 0, 0, 0, 0 ), ECHOPFXMATHS.ADD );
		_matProp[(int)ECHOPROP.ADDFADE]   		= new EchoMaterialProperty ( "_echoAddFade", imat, 0, ECHOPFXMATHS.AVERAGE );
		
		_matProp[(int)ECHOPROP.MULT]			= new EchoMaterialProperty ( "_echoMult", imat, new Vector4 ( 1, 1, 1, 1 ), ECHOPFXMATHS.MULTIPLY );
		_matProp[(int)ECHOPROP.MULTFADE]   		= new EchoMaterialProperty ( "_echoMultFade", imat, 0, ECHOPFXMATHS.AVERAGE  );
		
		_matProp[(int)ECHOPROP.NOISEFADE]   	= new EchoMaterialProperty ( "_echoNoiseFade", imat, 0, ECHOPFXMATHS.AVERAGE );

		_matProp[(int)ECHOPROP.RAMPFADE]  		= new EchoMaterialProperty ( "_echoRampFade", imat, 0, ECHOPFXMATHS.AVERAGE );
		_matProp[(int)ECHOPROP.RAMP_TEX]  		= new EchoMaterialProperty ( "_echoRampTex", imat );

		_matProp[(int)ECHOPROP.CORRECTFADE]  	= new EchoMaterialProperty ( "_echoCorrectFade", imat, 0, ECHOPFXMATHS.AVERAGE  );
		_matProp[(int)ECHOPROP.CORRECT_TEX]		= new EchoMaterialProperty ( "_echoCorrectTex", imat );

		_matProp[(int)ECHOPROP.OVERLAY_FADE]  		= new EchoMaterialProperty ( "_echoOverlayFade", imat, 0, ECHOPFXMATHS.AVERAGE );
		_matProp[(int)ECHOPROP.OVERLAY_TEX]  		= new EchoMaterialProperty ( "_OverlayTex", imat );
		_matProp[(int)ECHOPROP.OVERLAY_ST]  		= new EchoMaterialProperty ( "_OverlayTex_ST", imat, new Vector4 ( 1, 1, 0, 0 ), ECHOPFXMATHS.DIRECT );

		_matProp[(int)ECHOPROP.OVERLAYSCR_FADE] 	= new EchoMaterialProperty ( "_echoOverlayScrFade", imat, 0, ECHOPFXMATHS.AVERAGE );
		_matProp[(int)ECHOPROP.OVERLAYSCR_TEX]		= new EchoMaterialProperty ( "_OverlayTexScr", imat );
		_matProp[(int)ECHOPROP.OVERLAYSCR_ST]  		= new EchoMaterialProperty ( "_OverlayTexScr_ST", imat, new Vector4 ( 1, 1, 0, 0 ), ECHOPFXMATHS.DIRECT );

		_matProp[(int)ECHOPROP.OVERLAYADD_FADE] 	= new EchoMaterialProperty ( "_echoOverlayAddFade", imat, 0, ECHOPFXMATHS.AVERAGE );
		_matProp[(int)ECHOPROP.OVERLAYADD_TEX] 		= new EchoMaterialProperty ( "_OverlayTexAdd", imat );
		_matProp[(int)ECHOPROP.OVERLAYADD_ST]  		= new EchoMaterialProperty ( "_OverlayTexAdd_ST", imat, new Vector4 ( 1, 1, 0, 0 ), ECHOPFXMATHS.DIRECT );

		_matProp[(int)ECHOPROP.OVERLAYSUB_FADE] 	= new EchoMaterialProperty ( "_echoOverlaySubFade", imat, 0, ECHOPFXMATHS.AVERAGE );
		_matProp[(int)ECHOPROP.OVERLAYSUB_TEX] 		= new EchoMaterialProperty ( "_OverlayTexSub", imat );
		_matProp[(int)ECHOPROP.OVERLAYSUB_ST]  		= new EchoMaterialProperty ( "_OverlayTexSub_ST", imat, new Vector4 ( 1, 1, 0, 0 ), ECHOPFXMATHS.DIRECT );

		_matProp[(int)ECHOPROP.OVERLAYMUL_FADE] 	= new EchoMaterialProperty ( "_echoOverlayMulFade", imat, 0, ECHOPFXMATHS.AVERAGE );
		_matProp[(int)ECHOPROP.OVERLAYMUL_TEX]		= new EchoMaterialProperty ( "_OverlayTexMul", imat );
		_matProp[(int)ECHOPROP.OVERLAYMUL_ST]  		= new EchoMaterialProperty ( "_OverlayTexMul_ST", imat, new Vector4 ( 1, 1, 0, 0 ), ECHOPFXMATHS.DIRECT );

		_matProp[(int)ECHOPROP.OVERLAYOVR_FADE] 	= new EchoMaterialProperty ( "_echoOverlayOvrFade", imat, 0, ECHOPFXMATHS.AVERAGE );
		_matProp[(int)ECHOPROP.OVERLAYOVR_TEX]		= new EchoMaterialProperty ( "_OverlayTexOvr", imat );
		_matProp[(int)ECHOPROP.OVERLAYOVR_ST]  		= new EchoMaterialProperty ( "_OverlayTexOvr_ST", imat, new Vector4 ( 1, 1, 0, 0 ), ECHOPFXMATHS.DIRECT );

		_matProp[(int)ECHOPROP.SCANLINEFADE]  		= new EchoMaterialProperty ( "_echoScanLineFade", imat, 0, ECHOPFXMATHS.AVERAGE );
		_matProp[(int)ECHOPROP.SCANLINECOUNTH]		= new EchoMaterialProperty ( "_echoScanLineCountH", imat, 0, ECHOPFXMATHS.AVERAGE );
		_matProp[(int)ECHOPROP.SCANLINESCROLLH]  	= new EchoMaterialProperty ( "_echoScanLineScrollH", imat, 0, ECHOPFXMATHS.ADD );

		_matProp[(int)ECHOPROP.SCANLINECOUNTV]		= new EchoMaterialProperty ( "_echoScanLineCountV", imat, 0, ECHOPFXMATHS.AVERAGE );
		_matProp[(int)ECHOPROP.SCANLINESCROLLV]  	= new EchoMaterialProperty ( "_echoScanLineScrollV", imat, 0, ECHOPFXMATHS.ADD );

		_matProp[(int)ECHOPROP.CUSTOM_FRAG_1_FADE]  = new EchoMaterialProperty ( "_echoCustomF1Fade", imat, 0, ECHOPFXMATHS.AVERAGE );
		_matProp[(int)ECHOPROP.CUSTOM_FRAG_1_ARGS]  = new EchoMaterialProperty ( "_echoCustomF1Args", imat, new Vector4 ( 0, 0, 0, 0 ), ECHOPFXMATHS.AVERAGE );
		_matProp[(int)ECHOPROP.CUSTOM_FRAG_1_COLOR]	= new EchoMaterialProperty ( "_echoCustomF1Color", imat, new Vector4 ( 1, 1, 1, 1 ), ECHOPFXMATHS.MULTIPLY );
		_matProp[(int)ECHOPROP.CUSTOM_FRAG_1_TEX]  	= new EchoMaterialProperty ( "_echoCustomF1Tex_TEX", imat );
		_matProp[(int)ECHOPROP.CUSTOM_FRAG_1_ST]  	= new EchoMaterialProperty ( "_echoCustomF1Tex_ST", imat, new Vector4 ( 1, 1, 0, 0 ), ECHOPFXMATHS.DIRECT );

		_matProp[(int)ECHOPROP.CUSTOM_FRAG_2_FADE]  = new EchoMaterialProperty ( "_echoCustomF2Fade", imat, 0, ECHOPFXMATHS.AVERAGE );
		_matProp[(int)ECHOPROP.CUSTOM_FRAG_2_ARGS]  = new EchoMaterialProperty ( "_echoCustomF2Args", imat, new Vector4 ( 0, 0, 0, 0 ), ECHOPFXMATHS.AVERAGE );
		_matProp[(int)ECHOPROP.CUSTOM_FRAG_2_COLOR]	= new EchoMaterialProperty ( "_echoCustomF2Color", imat, new Vector4 ( 1, 1, 1, 1 ), ECHOPFXMATHS.MULTIPLY  );
		_matProp[(int)ECHOPROP.CUSTOM_FRAG_2_TEX]  	= new EchoMaterialProperty ( "_echoCustomF2Tex", imat );
		_matProp[(int)ECHOPROP.CUSTOM_FRAG_2_ST]  	= new EchoMaterialProperty ( "_echoCustomF2Tex_ST", imat, new Vector4 ( 1, 1, 0, 0 ), ECHOPFXMATHS.DIRECT );

		_matProp[(int)ECHOPROP.CUSTOM_FRAG_3_FADE]  = new EchoMaterialProperty ( "_echoCustomF3Fade", imat, 0, ECHOPFXMATHS.AVERAGE );
		_matProp[(int)ECHOPROP.CUSTOM_FRAG_3_ARGS]  = new EchoMaterialProperty ( "_echoCustomF3Args", imat, new Vector4 ( 0, 0, 0, 0 ), ECHOPFXMATHS.AVERAGE );
		_matProp[(int)ECHOPROP.CUSTOM_FRAG_3_COLOR]	= new EchoMaterialProperty ( "_echoCustomF3Color", imat, new Vector4 ( 1, 1, 1, 1 ), ECHOPFXMATHS.MULTIPLY  );
		_matProp[(int)ECHOPROP.CUSTOM_FRAG_3_TEX]  	= new EchoMaterialProperty ( "_echoCustomF3Tex", imat );
		_matProp[(int)ECHOPROP.CUSTOM_FRAG_3_ST]  	= new EchoMaterialProperty ( "_echoCustomF3Tex_ST", imat, new Vector4 ( 1, 1, 0, 0 ), ECHOPFXMATHS.DIRECT );

		_matProp[(int)ECHOPROP.CUSTOM_FRAG_4_FADE]  = new EchoMaterialProperty ( "_echoCustomF4Fade", imat, 0, ECHOPFXMATHS.AVERAGE );
		_matProp[(int)ECHOPROP.CUSTOM_FRAG_4_ARGS]  = new EchoMaterialProperty ( "_echoCustomF4Args", imat, new Vector4 ( 0, 0, 0, 0 ), ECHOPFXMATHS.AVERAGE );
		_matProp[(int)ECHOPROP.CUSTOM_FRAG_4_COLOR]	= new EchoMaterialProperty ( "_echoCustomF4Color", imat, new Vector4 ( 1, 1, 1, 1 ), ECHOPFXMATHS.MULTIPLY  );
		_matProp[(int)ECHOPROP.CUSTOM_FRAG_4_TEX]  	= new EchoMaterialProperty ( "_echoCustomF4Tex", imat );
		_matProp[(int)ECHOPROP.CUSTOM_FRAG_4_ST]  	= new EchoMaterialProperty ( "_echoCustomF4Tex_ST", imat, new Vector4 ( 1, 1, 0, 0 ), ECHOPFXMATHS.DIRECT );
	}

	//=========================================================================
	public void ForceUpdate()
	{
		for ( int loop = 0; loop < (int)ECHOPROP.COUNT; loop++ )
		{
			_matProp[loop].vec4Val_hold.x 	= _matProp[loop].vec4Val.x-98;
			_matProp[loop].floatVal_hold 	= _matProp[loop].floatVal-98;
			_matProp[loop].texVal_hold 		= null;
		}
	}

	//=========================================================================
	public void ResetAllFrameCounts()
	{
		EchoMaterialProperty emp;

		//SCOTTFIND undoing opts ?
		for ( int loop = 0; loop < (int)ECHOPROP.COUNT; loop++ )
		{
			emp = _matProp[loop];

			switch ( emp.mathtOpt )
			{
			case ECHOPFXMATHS.DIRECT:
				break;

			case ECHOPFXMATHS.ADD:
				emp.vec4Val = _zeroVec4;
				emp.floatVal = 0;
				break;

			case ECHOPFXMATHS.AVERAGE:
				emp.vec4Val = _zeroVec4;
				emp.floatVal = 0;
				emp.avgStart = 1.0f;
				break;

			case ECHOPFXMATHS.MULTIPLY:
				emp.vec4Val = _oneVec4;
				emp.floatVal = 1;
				break;

			}
		}
	}

	//=========================================================================
	public void SetTexture ( ECHOPROP iep, Texture itex )
	{
		EchoMaterialProperty emp = _matProp[(int)iep];

		emp.active 	= true;
		emp.texVal 	= itex;
	}

	//=========================================================================
	public void SetTextureAlways ( ECHOPROP iep, Texture itex )
	{
		EchoMaterialProperty emp = _matProp[(int)iep];
		
		emp.active 		= true;
		emp.texVal 		= itex;
		emp.texVal_hold = null;
	}

	//=========================================================================
	public void SetVector ( ECHOPROP iep, Vector4 ivec4 )
	{
		EchoMaterialProperty emp = _matProp[(int)iep];

		emp.active 	= true;

		switch ( emp.mathtOpt )
		{
		case ECHOPFXMATHS.DIRECT:
			emp.vec4Val = ivec4;
			break;
			
		case ECHOPFXMATHS.ADD:
			emp.vec4Val += ivec4;
			break;
			
		case ECHOPFXMATHS.AVERAGE:
			emp.vec4Val = ( emp.vec4Val + ivec4 ) * emp.avgStart;
			emp.avgStart = 0.5f;
			break;
			
		case ECHOPFXMATHS.MULTIPLY:
			emp.vec4Val.x *= ivec4.x;
			emp.vec4Val.y *= ivec4.y;
			emp.vec4Val.z *= ivec4.z;
			emp.vec4Val.w *= ivec4.w;
			break;
		}
	}

	//=========================================================================
	public void SetFloat ( ECHOPROP iep, float inum )
	{
		EchoMaterialProperty emp = _matProp[(int)iep];
		
		emp.active 		= true;

		switch ( emp.mathtOpt )
		{
		case ECHOPFXMATHS.DIRECT:
			emp.floatVal = inum;
			break;
			
		case ECHOPFXMATHS.ADD:
			emp.floatVal += inum;
			break;
			
		case ECHOPFXMATHS.AVERAGE:
			emp.floatVal = ( emp.floatVal + inum ) * emp.avgStart;
			emp.avgStart = 0.5f;
			break;
			
		case ECHOPFXMATHS.MULTIPLY:
			emp.floatVal *= inum;
			break;
		}

		emp.floatVal 	= inum;
	}

	//=========================================================================
	public void SubmitShaderProperties ()
	{
		EchoMaterialProperty emp;
		
		for ( int loop = 0; loop < (int)ECHOPROP.COUNT; loop++ )
		{
			emp = _matProp[loop];
			
			if ( emp.active )
			{
				emp.active = false;

				switch ( emp.numType )
				{
				case 0:
					if ( emp.floatVal != emp.floatVal_hold )
					{
#if UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1
						mat.SetFloat ( emp.name, emp.floatVal );
#else
						mat.SetFloat ( emp.propID, emp.floatVal );
#endif
						emp.floatVal_hold = emp.floatVal;
					}
					break;

				case 1:
					if ( emp.vec4Val != emp.vec4Val_hold )
					{
#if UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1
						mat.SetColor ( emp.name, emp.vec4Val );
#else
						mat.SetColor ( emp.propID, emp.vec4Val );
#endif
						emp.vec4Val_hold = emp.vec4Val;
					}
					break;

				case 2:
					if ( emp.texVal != emp.texVal_hold )
					{
#if UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1
						mat.SetTexture ( emp.name, emp.texVal );
#else
						mat.SetTexture ( emp.propID, emp.texVal );
#endif
						emp.texVal_hold = emp.texVal;
					}
					break;
				}
			}
		}
	}
}
