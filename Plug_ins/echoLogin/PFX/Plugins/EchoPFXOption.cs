// (c) copyright echoLogin LLC 2013. All rights reserved.

using UnityEngine;
using System.Collections;

public enum ECHOPFXOPTION
{
	GREYSCALE 	= 0,
	INVERSE,   	
	COLOR,
	ADD,
	MULTIPLY,
	NOISE,
	DISTORTION,
	SHOCKWAVE,
	SCANLINES,
	LUMRAMP, 
	COLOR_CORRECT,
	OVERLAY_NORMAL,
	OVERLAY_SCREEN,
	OVERLAY_ADD,
	OVERLAY_SUBTRACT,
	OVERLAY_MULTIPLY,
	OVERLAY_OVERLAY,
	CUSTOM_FRAG_1,
	CUSTOM_FRAG_2,
	CUSTOM_FRAG_3,
	CUSTOM_FRAG_4,
	COUNT
};

[System.Serializable]
public class EchoPFXOption
{
	public ECHOPFXOPTION  	optType;
	public int       		passOrder;
	public float    		startDelay;  // mode 1
	public float    		attackTime;  // mode 2
	public float    		sustainTime; // mode 3
	public float    		releaseTime; // mode 4
	public float    		fadeMin 	 = 0;
	public float    		fadeMax 	 = 1;
	public float    		fadeCur 	 = 1;
	public Color  			rgba;
	public float    		rgbaMultiply = 1.0f;
	public float    		distAmountH;
	public float    		distSpeedH;
	public float    		distStrengthH;
	public float    		distAmountV;
	public float    		distSpeedV;
	public float    		distStrengthV;

	public bool             linesAmountDivideH;
	public int      		linesAmountH;
	public float    		linesScrollH;

	public bool             linesAmountDivideV;
	public int      		linesAmountV;
	public float    		linesScrollV;

	public float    		shockDistance;
	public float    		shockSize;
	public float    		shockCenterX;
	public float    		shockCenterY;
	public Vector4          overlayST = new Vector4 ( 1, 1, 0, 0 );
	public Vector4          overlayST_Scroll;  // xy = scroll amounts  zw = speed throttle
	public Vector4          customArgs;
	public Texture2D 		tex;
	public ECHOPFXBLEND 	texBlend;
	public bool             folded = false;

	
	private float        		_autoDelay;
	private float        		_autoAttack;
	private float        		_autoSustain;
	private float        		_autoRelease;
	private float        		_autoTime;
	private float        		_autoTimeTotal;
	private bool                _autoSustainHold;

	private float        		_autoAttackCur;
	private float        		_autoSustainCur;
	private float        		_autoReleaseCur;

	private int          		_autoMode;
	private float        		_autoPer;
	private EchoPFXRenderGroup 	_erg;
	private float               _uvAdd;
	
	//=========================================================================
	public EchoPFXOption ( ECHOPFXOPTION iopt )
	{
		optType = iopt;
		ResetDefaults();
	}
	
	//=========================================================================
	public void ResetDefaults()
	{
		startDelay 			= 0;
		attackTime      	= 0;
		sustainTime     	= 0;
		releaseTime     	= 0;
		fadeMin         	= 0;
		fadeMax  	    	= 1;
		fadeCur         	= 1;
		distAmountH     	= 1;
		distSpeedH      	= 2;
		distStrengthH   	= 1;
		distAmountV     	= 0;
		distSpeedV      	= 0;
		distStrengthV   	= 0;
		linesAmountDivideH  = true;
		linesAmountH    	= 4;    
		linesScrollH    	= 0;    
		linesAmountDivideV  = true;
		linesAmountV    	= 4;    
		linesScrollV    	= 0;    
		shockDistance   	= 0;
		shockSize       	= 0.1f;
		shockCenterX    	= 0.5f;
		shockCenterY    	= 0.5f;
		rgba        		= new Color ( 1,1,1,1 );
		rgbaMultiply    	= 1.0f;
		customArgs      	= new Vector4 ( 0,0,0,0 );
		overlayST       	= new Vector4 ( 1,1,0,0 );
		overlayST_Scroll	= new Vector4 ( 0,0,0,0 );
	}

	//=========================================================================
	public void DefaultTimingSet (  float istartDelay, float iattackTime, float isustainTime, float ireleaseTime )
	{
		startDelay	= istartDelay;
		attackTime 	= iattackTime;
		sustainTime = isustainTime;
		releaseTime = ireleaseTime;
	}

	//=========================================================================
	public void Start()
	{
		_autoMode 		= 1;

		_autoDelay 		= startDelay;
		_autoAttackCur  = _autoAttack   = attackTime;
		_autoSustainCur	= _autoSustain 	= sustainTime;
		_autoReleaseCur = _autoRelease  = releaseTime;

		if ( sustainTime < 0.0 )
			_autoSustainHold = true;
		else
			_autoSustainHold = false;

		shockDistance = 0.0f;
		_autoTime = 0.0f;
		_autoTimeTotal = _autoAttack + _autoSustain + _autoRelease;

		if ( startDelay <= 0.00001f )
		{
			_autoMode = 2;
			if ( _autoSustain <= 0.00001f && _autoAttack <= 0.00001f && _autoRelease <= 0.00001f)
				_autoMode = 0;
		}
		
		if ( _autoMode > 0 )
			fadeCur 		= fadeMin;
	}
	
	//=========================================================================
	public void Start ( float iscale )
	{
		if ( iscale < 0.0f )
			return;

		_autoMode 		= 1;
		
		_autoDelay 		= startDelay * iscale;
		_autoAttackCur  = _autoAttack   = ( attackTime * iscale );
		_autoSustainCur	= _autoSustain 	= ( sustainTime * iscale );
		_autoReleaseCur = _autoRelease  = ( releaseTime * iscale );

		if ( sustainTime < 0.0 )
			_autoSustainHold = true;
		else
			_autoSustainHold = false;

		shockDistance 	= 0.0f;
		_autoTime 		= 0.0f;
		_autoTimeTotal 	= _autoAttack + _autoSustain + _autoRelease;

		fadeCur 		= fadeMin;

		if ( startDelay <= 0.00001f )
		{
			_autoMode = 2;
			if ( _autoSustain <= 0.00001f && _autoAttack <= 0.00001f && _autoRelease <= 0.00001f)
				_autoMode = 0;
		}
		
		if ( _autoMode > 0 )
			fadeCur 		= fadeMin;
	}

	//=========================================================================
	public void Stop()
	{
		_autoMode = 4;
	}

	//=========================================================================
	public void Stop ( float ireleaseTime )
	{
		_autoMode = 4;
		_autoReleaseCur = _autoRelease = ireleaseTime;
	}

	//=========================================================================
	public int Process()
	{
		if ( _autoMode < 0 )
			return ( 0 );

		if ( overlayST_Scroll != new Vector4 (0,0,0,0) )
		{
			overlayST.z += ( ( ( overlayST_Scroll.x * EchoPFXManager.globalFPS ) * overlayST_Scroll.z ) * Time.deltaTime );
			overlayST.w += ( ( ( overlayST_Scroll.y * EchoPFXManager.globalFPS ) * overlayST_Scroll.w ) * Time.deltaTime );

			if ( overlayST.z > 1.0f )
				overlayST.z -= (float)( Mathf.Floor ( overlayST.z ) );

			if ( overlayST.z < 0.0f )
				overlayST.z -= (float)( Mathf.Floor ( overlayST.z ) );

			if ( overlayST.w > 1.0f )
				overlayST.w -= (float)( Mathf.Floor ( overlayST.w ) );
			
			if ( overlayST.w < 0.0f )
				overlayST.w -= (float)( Mathf.Floor ( overlayST.w ) );
		}

		if ( optType == ECHOPFXOPTION.SHOCKWAVE )
		{
			shockDistance = Mathf.Lerp ( 0.0f, 1.0f, _autoTime / _autoTimeTotal );
		}

		switch ( _autoMode )
		{
		case 0:
			return ( 1 );
			//break;
			
		// **** start delay
		case 1:
			_autoDelay-=Time.deltaTime;
			
			if ( _autoDelay <= 0.0f )
				_autoMode = 2;
			else
				return ( 0 );
			
			break;
			
		// **** attack ( fade in )	
		case 2:
			_autoTime += Time.deltaTime;
			
			_autoAttackCur-=Time.deltaTime;
			
			if ( _autoAttackCur > 0.0f )
				fadeCur = Mathf.Lerp ( fadeMax, fadeMin, _autoAttackCur / _autoAttack );
			else
				_autoMode = 3;
				
			break;
			
		// **** sustain  ( how long it stays )
		case 3:
			_autoTime += Time.deltaTime;
			
			if ( _autoSustainHold == false )
			{
				_autoSustainCur-=Time.deltaTime;
				if ( _autoSustainCur > 0.0f )
					fadeCur = fadeMax;
				else
					_autoMode = 4;
			}
			break;
			
		// **** release ( fade out )
		case 4:
			_autoTime += Time.deltaTime;
			
			_autoReleaseCur-=Time.deltaTime;
			
			if ( _autoReleaseCur > 0.0f )
				fadeCur = Mathf.Lerp ( fadeMin, fadeMax, _autoReleaseCur / _autoRelease );
			else
				_autoMode = -1;
			break;

		default:
			_autoMode = -1;
			break;
		}
		
		if ( fadeCur > 0.0f )
			return ( _autoMode );
		else
			return ( 0 );
	}
	
	//=========================================================================
	public void ValidateInput()
	{
//		if ( startDelay < 0.0f )
//			startDelay = 0;

		if ( attackTime < 0.0f )
			attackTime = 0;

//		if ( sustainTime < 0.0f )
//			sustainTime = 0;
		
		if ( releaseTime < 0.0f )
			releaseTime = 0;
		
		if ( fadeMin < 0.0f )
			fadeMin = 0;
		
		if ( fadeMin > 1.0f )
			fadeMin = 1;
		
		if ( fadeMax > 1.0f )
			fadeMax = 1;
		
		if ( fadeMax < 0.0f )
			fadeMax = 0;
		
		if ( rgbaMultiply < 0.0f )
			rgbaMultiply = 0.0f;
		
		if ( rgbaMultiply > 8.0f )
			rgbaMultiply = 8;
	}
	
	//=========================================================================
	public void CopyTo ( EchoPFXOption iepo )
	{
		iepo.optType 			= optType;
		iepo.startDelay 		= startDelay;
		iepo.attackTime 		= attackTime;
		iepo.sustainTime 		= sustainTime;
		iepo.releaseTime 		= releaseTime;
		iepo.fadeCur 			= fadeCur;
		iepo.fadeMin 			= fadeMin;
		iepo.fadeMax 			= fadeMax;
		iepo.rgba 				= rgba;
		iepo.rgbaMultiply 		= rgbaMultiply;
		iepo.distAmountH 		= distAmountH;
		iepo.distSpeedH 		= distSpeedH;
		iepo.distStrengthH 		= distStrengthH;
		iepo.distAmountV 		= distAmountV;
		iepo.distSpeedV 		= distSpeedV;
		iepo.distStrengthV 		= distStrengthV;
		iepo.linesAmountDivideH = linesAmountDivideH;
		iepo.linesAmountH 		= linesAmountH;
		iepo.linesScrollH 		= linesScrollH;
		iepo.linesAmountDivideV = linesAmountDivideV;
		iepo.linesAmountV 		= linesAmountV;
		iepo.linesScrollV 		= linesScrollV;
		iepo.shockDistance 		= shockDistance;
		iepo.shockSize 			= shockSize;
		iepo.shockCenterX 		= shockCenterX;
		iepo.shockCenterY 		= shockCenterY;
		iepo.tex 				= tex;
		iepo.texBlend 			= texBlend;
		iepo.customArgs         = customArgs;
		iepo.overlayST          = overlayST;
		iepo.overlayST_Scroll   = overlayST_Scroll;
	}
	
	/*
	//=========================================================================
	public EchoPFXOption Clone ()
	{
		EchoPFXOption epo = new EchoPFXOption(optType);
		
		epo.CopyTo ( this );
		
		return ( epo );
	}
	*/
};
