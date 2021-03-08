#define ECHO_DEBUG
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.IO;

[CustomEditor (typeof(EchoPFXManager))]
public class EchoPFXManagerEditor : Editor 
{
	private static int _curid = 0;
	private	EchoPFXRenderGroup 	_erg;
	private	EchoPFXManager epfxm;
	private int _epeIndex 			= 0;
	private int _ergIndex 			= 0;
	private bool _myfxFold 			= true;
	private bool _myrgFold 			= true;
	private int  _curMode   		= 0;
	private int _passIndex 			= 0;
	private PossibleOpts _customOpt = null;
	private bool _needCompile       = false;
	private bool _playMaker         = false;

	public static readonly string[] optNames = new string[(int)ECHOPFXOPTION.COUNT]
	{
		"Greyscale",
		"Inverse",
		"Color",
		"Add",
		"Multiply",
		"Noise",
		"Distort",
		"Shockwave",
		"Scanline",
		"Ramp",
		"Color Correct",
		"Texture",
		"Texture Screen",
		"Texture Add",
		"Texture Sub",
		"Texture Multiply",
		"Texture Overlay",
		"Custom 1",
		"Custom 2",
		"Custom 3",
		"Custom 4",
	};

	//=========================================================================
	public string TrimString ( string istr )
	{
		char[] trimChars = {' '};
		
		return ( istr.Trim(trimChars) );
	}
	
	

	//=========================================================================
	public int GetGroupID()
	{
		for ( int loop = 0; loop < epfxm.ergList.Count; loop++ )
		{
			if ( _curid == epfxm.ergList[loop].id )
			{
				loop = 0;
				_curid++;
			}
		}
		
		return ( _curid );
	}
	
	//=========================================================================
	void SavePrefs()
	{
		//EditorPrefs.DeleteAll();
		EditorPrefs.SetInt ( "echo_ergIndex", _ergIndex );
		EditorPrefs.SetInt ( "echo__epeIndex", _epeIndex  );
		EditorPrefs.SetInt ( "echo_passIndex", _passIndex  );
		EditorPrefs.SetInt ( "echo_curMode", _curMode );
		EditorPrefs.SetInt ( "echo_curID", _curid );
		EditorPrefs.SetBool ( "echo_needCompile", _needCompile );
		EditorPrefs.SetBool ("echoPlayMaker", _playMaker );
	}

	//=========================================================================
	void LoadPrefs()
	{
		_curMode 		= EditorPrefs.GetInt ( "echo_curMode", 0 );
		_curid 			= EditorPrefs.GetInt ( "echo_curid", 0 );
		_needCompile 	= EditorPrefs.GetBool ( "echo_needCompile", false );
		_ergIndex   	= EditorPrefs.GetInt ( "echo_ergIndex", 0 );
		_epeIndex 		= EditorPrefs.GetInt ( "echo__epeIndex", 0 );
		_passIndex 		= EditorPrefs.GetInt ( "echo_passIndex", 0 );
		_playMaker  	= EditorPrefs.GetBool ("echoPlayMaker", false );
	}
	
	//=========================================================================
	void OnDisable()
	{
		//EditorPrefs.DeleteAll();
		SavePrefs();
		_customOpt = null;
	}

	//=========================================================================
	void OnEnable()
	{
		EchoPFXEffect epe;
		LoadPrefs();

		_customOpt 	= null;
		epfxm 		= (EchoPFXManager)target;

		serializedObject.Update();
//		postFX.Update();

		if ( epfxm.ergList.Count < 1 )
		{
			SetNewRenderGroup ( new EchoPFXRenderGroup( GetGroupID() ), true );
		}
		
		if ( _ergIndex < 0 || _ergIndex >= epfxm.ergList.Count )
			_ergIndex = 0;

		_erg = epfxm.ergList[ _ergIndex ];

		if ( _epeIndex < 0 || _epeIndex >= _erg.epeList.Count )
		{
			if ( _erg.epeList.Count < 1 )
			{
				epe 		= new EchoPFXEffect();
				epe.name 	= "New Effect";
				_erg.epeList.Add ( epe );
				_epeIndex 		= _erg.epeList.IndexOf ( epe );
			}
		}

		EditorApplication.playmodeStateChanged = AutoCompileShaders;
		
		//epfxm.UpdateAllRenderGroups();
	}

	//=========================================================================
	private static bool ReplaceInFile (string ifilePath, string isearchText, string ireplaceText)
	{
		bool enabled = false;
		
		StreamReader reader = new StreamReader (ifilePath);
		string content = reader.ReadToEnd ();
		reader.Close ();

		if (content.StartsWith(isearchText))
		{
			enabled = true;
			content = content.Replace ( isearchText, ireplaceText );
		}
		else
		{
			enabled = false;
			content = content.Replace ( ireplaceText, isearchText );
		}
		
		StreamWriter writer = new StreamWriter (ifilePath);
		writer.Write (content);
		writer.Close ();
		
		return ( enabled );
	}
	
	//=========================================================================
	private void SetNewRenderGroup ( EchoPFXRenderGroup ierg, bool iadd )
	{

		if ( _erg != ierg )
			_customOpt 	= null;
		else
			return;

		_erg 		=  ierg;

		if ( iadd )
		{
			float highnum = -2;
		
			epfxm.ergList.Add ( _erg );

			for ( int loop = 0; loop < epfxm.ergList.Count; loop++ )
			{
				if ( epfxm.ergList[loop] != ierg )
				{
					if ( epfxm.ergList[loop].cameraDepthEnd > highnum )
					{
						highnum = epfxm.ergList[loop].cameraDepthEnd;
					}
				}
			}
			
			highnum++;
			
			_erg.cameraDepthStart = highnum;
			_erg.cameraDepthEnd   = highnum+1;

		}

		_ergIndex = epfxm.ergList.IndexOf ( _erg );
		_epeIndex 	= 0;
	}
	
	//=========================================================================
	private float EchoSlider ( string iname, float ival, float imin, float imax, int itwidth = 56 )
	{
		float rv;
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField ( iname, GUILayout.MaxWidth ( itwidth ) );
		rv = EditorGUILayout.Slider ( ival, imin, imax );
		EditorGUILayout.EndHorizontal();
		
		return ( rv );
	}

	//=========================================================================
	private int EchoSlider ( string iname, int ival, int imin, int imax )
	{
		int rv;
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField ( iname, GUILayout.MaxWidth(56) );
		rv = (int)EditorGUILayout.Slider ( ival, imin, imax );
		EditorGUILayout.EndHorizontal();
		
		return ( rv );
	}

	//=========================================================================
	public void PostEffectOptionsGUI ( EchoPFXOption iepo )
	{
		EditorGUILayout.LabelField ( "TIMING:" );
		// attack time
		iepo.attackTime = EditorGUILayout.FloatField ( "Fade In",iepo.attackTime );
		
		// sustain time
		iepo.sustainTime = EditorGUILayout.FloatField ( "Sustain", iepo.sustainTime  );
		
		// release time
		iepo.releaseTime = EditorGUILayout.FloatField ( "Fade Out", iepo.releaseTime  );

		iepo.startDelay = EditorGUILayout.FloatField ("Start Delay", iepo.startDelay ); 
		if ( iepo.startDelay < 0 )
			iepo.startDelay = 0;

		EditorGUILayout.Space();

		EditorGUILayout.LabelField ( "FADE OPTIONS:" );

		iepo.fadeMin = EchoSlider ("Min", iepo.fadeMin, 0, 1 ); 
		iepo.fadeMax = EchoSlider ("Max", iepo.fadeMax, 0, 1 );
		iepo.fadeCur = EchoSlider ("Current", iepo.fadeCur, iepo.fadeMin, iepo.fadeMax ); 
		
		EditorGUILayout.Space();
		
		iepo.ValidateInput();
	}
	 
	//=========================================================================
	public bool PostFxOptionGUI ( List<EchoPFXOption> iepolist, int ipos, string iname )
	{
		bool flag = true;
		EchoPFXOption epo = iepolist[ipos];

		// toggle, start, end, duration
		GUILayout.Box("", new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(1)});

		EditorGUILayout.BeginHorizontal();

		if ( epo.folded )
		{
			if ( GUILayout.Button ( "▶", GUILayout.MaxWidth(24) ) )
			{
				epo.folded = false;
			}
			else
			{
				flag = false;
			}
		}
		else
		{
			if ( GUILayout.Button ( "▼", GUILayout.MaxWidth(24) ) )
			{
				epo.folded = true;
			}
		}


		GUILayout.Box( iname, new GUILayoutOption[]{GUILayout.ExpandWidth(true)});
		if ( GUILayout.Button ( "Remove", GUILayout.MaxWidth(64) ) )
		{
			iepolist.RemoveAt ( ipos );
			flag = false;
		}

		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Space();

		if ( flag )
			PostEffectOptionsGUI ( epo );

		return ( flag );
	}

	//=========================================================================
	public void PostFxColorGUI ( EchoPFXOption iepo )
	{
		EditorGUILayout.LabelField ( "COLOR OPTIONS:" );

		EditorGUILayout.BeginHorizontal();
		iepo.rgba = EditorGUILayout.ColorField ( iepo.rgba );
		EditorGUILayout.LabelField ( " ",GUILayout.Width(26) );

		EditorGUILayout.EndHorizontal();
		iepo.rgbaMultiply = EchoSlider ( "Amplify", iepo.rgbaMultiply, 1, 8 ); 
	}

	//=========================================================================
	public void PostFxCustomGUI ( EchoPFXOption iepo, int ipass )
	{
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField ( "CUSTOM:" );
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		
		iepo.customArgs = EditorGUILayout.Vector4Field ( "Parameters", iepo.customArgs );

		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Space();

		PostFxColorGUI ( iepo );
		EditorGUILayout.Space();

		PostFxOverlayGUI ( iepo, ipass );
	}

	//=========================================================================
	public void PostFxScanLinesGUI ( EchoPFXOption iepo, PossibleOpts ipo )
	{
		if ( ipo.scanlines == ECHOPFXSCANLINES.HORIZONTAL || ipo.scanlines == ECHOPFXSCANLINES.BOTH )
		{
			EditorGUILayout.LabelField ( "HORIZONTAL:" );
			EditorGUILayout.BeginHorizontal();
			iepo.linesAmountDivideH = GUILayout.Toggle ( iepo.linesAmountDivideH, "" );
			EditorGUILayout.LabelField ( "Divide Screen Height By Count" );
			EditorGUILayout.EndHorizontal();
			
			iepo.linesAmountH = EditorGUILayout.IntField ("Count", iepo.linesAmountH );
			
			iepo.linesScrollH = EditorGUILayout.FloatField ( "Scroll", iepo.linesScrollH );
		}

		if ( ipo.scanlines == ECHOPFXSCANLINES.BOTH )
			EditorGUILayout.Space();

		if ( ipo.scanlines == ECHOPFXSCANLINES.VERTICAL || ipo.scanlines == ECHOPFXSCANLINES.BOTH )
		{
			EditorGUILayout.LabelField ( "VERTICAL:" );
			EditorGUILayout.BeginHorizontal();
			iepo.linesAmountDivideV = GUILayout.Toggle ( iepo.linesAmountDivideV, "" );
			EditorGUILayout.LabelField ( "Divide Screen Width By Count" );
			EditorGUILayout.EndHorizontal();
			
			iepo.linesAmountV = EditorGUILayout.IntField ("Count", iepo.linesAmountV );
			
			iepo.linesScrollV = EditorGUILayout.FloatField ( "Scroll", iepo.linesScrollV );
		}
	}

	//=========================================================================
	public void PostFxDistortionGUI ( EchoPFXOption iepo )
	{
		EditorGUILayout.LabelField ( "HORIZONTAL:" );

		iepo.distAmountH = EditorGUILayout.FloatField ( "Amount", iepo.distAmountH );
		iepo.distSpeedH = EditorGUILayout.FloatField ( "Speed", iepo.distSpeedH );
		iepo.distStrengthH = EditorGUILayout.FloatField ( "Strength", iepo.distStrengthH );

		if ( iepo.distAmountH < 0 )
			iepo.distAmountH = 0;

		if ( iepo.distStrengthH < 0 )
			iepo.distStrengthH = 0;

		EditorGUILayout.Space();
		EditorGUILayout.LabelField ( "VERTICAL:" );

		iepo.distAmountV = EditorGUILayout.FloatField ( "Amount", iepo.distAmountV );
		iepo.distSpeedV = EditorGUILayout.FloatField ( "Speed", iepo.distSpeedV );
		iepo.distStrengthV = EditorGUILayout.FloatField ( "Strength", iepo.distStrengthV );


		if ( iepo.distAmountV < 0 )
			iepo.distAmountV = 0;
		
		if ( iepo.distStrengthV < 0 )
			iepo.distStrengthV = 0;

	}
	
	//=========================================================================
	public void PostFxShockwaveGUI ( EchoPFXOption iepo )
	{
		EditorGUILayout.LabelField ( "OPTIONS:" );

		iepo.shockSize 		= EchoSlider ( "Size", iepo.shockSize, 0, 1 );
		iepo.shockDistance 	= EchoSlider ( "Distance", iepo.shockDistance, 0, 1 );
		iepo.shockCenterX 	= EchoSlider ( "Center X", iepo.shockCenterX, 0, 1 );
		iepo.shockCenterY 	= EchoSlider ( "Center Y", iepo.shockCenterY, 0, 1 );
	}

	//=========================================================================
	public void PostFxTextureGUI ( EchoPFXOption iepo )
	{
		iepo.tex = (Texture2D)EditorGUILayout.ObjectField ( iepo.tex, typeof ( Texture2D ), true, GUILayout.Width(64), GUILayout.Height(64) );
	}

	//=========================================================================
	public bool HasTexCoord (EchoPFXOption iepo, int ipass )
	{
		List<PossibleOpts> poList;

		if ( _erg == null )
			return ( false );

		if ( ipass == 0 )
			poList = _erg.possibleOpts1;
		else
			poList = _erg.possibleOpts2;

		for ( int loop = 0; loop < poList.Count; loop++ )
		{
			if ( poList[loop].type == iepo.optType )
				return ( poList[loop].useUniqueTC );
		}

		return ( false );
	}

	//=========================================================================
	public void PostFxOverlayGUI ( EchoPFXOption iepo, int ipass )
	{
		bool hasTC = HasTexCoord ( iepo, ipass );

		EditorGUILayout.LabelField ( "TEXTURE:" );

		if ( iepo.tex != null && hasTC )
			iepo.overlayST = EditorGUILayout.Vector4Field ( "Tiling = xy  Offset = zw", iepo.overlayST );
		EditorGUILayout.Space();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.BeginVertical();

		EditorGUILayout.BeginHorizontal(GUILayout.Width(64));

		iepo.tex = (Texture2D)EditorGUILayout.ObjectField ( iepo.tex, typeof ( Texture2D ), true, GUILayout.Width(64), GUILayout.Height(64) );
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();

		if ( iepo.tex != null && hasTC )
		{
			EditorGUILayout.BeginVertical();
			
			iepo.overlayST_Scroll.x	= EchoSlider ( "Scroll U", iepo.overlayST_Scroll.x, 0.0f, 1.0f, 64 );
			iepo.overlayST_Scroll.y	= EchoSlider ( "Scroll V", iepo.overlayST_Scroll.y, 0.0f, 1.0f, 64 );
			iepo.overlayST_Scroll.z	= EchoSlider ( "U Throttle", iepo.overlayST_Scroll.z, -1.0f, 1.0f, 64 );
			iepo.overlayST_Scroll.w	= EchoSlider ( "V Throttle", iepo.overlayST_Scroll.w, -1.0f, 1.0f, 64 );
			
			
			EditorGUILayout.EndVertical();
		}

		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Space();

	}

	//=========================================================================
	public void RenderGroupGUI()
	{
		if ( _erg == null )
			return;
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Render Group Name: ", GUILayout.Width(128) );
		_erg.name = EditorGUILayout.TextField ( _erg.name, GUILayout.Width(128) );
		_erg.name = TrimString ( _erg.name );
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space();
		
		EditorGUILayout.LabelField("Screen Mesh Quads" );
		
		_erg.meshCellWidth = EchoSlider ( "Horz:", _erg.meshCellWidth, 2, 64 );
		_erg.meshCellHeight = EchoSlider ( "Vert:", _erg.meshCellHeight, 2, 64 );

		EditorGUILayout.Space();

		EditorGUILayout.Space();
		
		EditorGUILayout.LabelField ( "Pass 1:");
		if ( _erg != epfxm.ergList[0] )
		{
			EditorGUI.BeginChangeCheck();
			_erg.rtBlendMode[0] = (ECHOPFXBLEND)EditorGUILayout.EnumPopup ("BlendMode", _erg.rtBlendMode[0] );
			if ( EditorGUI.EndChangeCheck() )
				_needCompile = true;
		}
		else
			_erg.rtBlendMode[0] = ECHOPFXBLEND.NORMAL;

		_erg.rtFilterMode[0] = (FilterMode)EditorGUILayout.EnumPopup ("Blit Filter Mode", _erg.rtFilterMode[0] );
		EditorGUILayout.Space();

		EditorGUILayout.LabelField ( "Pass 2:" );
		if ( _erg != epfxm.ergList[0] )
		{
			EditorGUI.BeginChangeCheck();
			_erg.rtBlendMode[1] = (ECHOPFXBLEND)EditorGUILayout.EnumPopup ("BlendMode", _erg.rtBlendMode[1] );
			if ( EditorGUI.EndChangeCheck() )
				_needCompile = true;
		}
		else
			_erg.rtBlendMode[1] = ECHOPFXBLEND.NORMAL;

		_erg.rtFilterMode[1] = (FilterMode)EditorGUILayout.EnumPopup ("Blit Filter Mode", _erg.rtFilterMode[1] );
		EditorGUILayout.Space();
		
		if ( SystemInfo.supportsImageEffects && SystemInfo.supportsRenderTextures )
		{
			EditorGUILayout.LabelField("Render Texture Size" );
			_erg.rtAdjustSize = (ECHORTADJUST)EditorGUILayout.EnumPopup ("Method", _erg.rtAdjustSize );
			
			if ( _erg.rtAdjustSize != ECHORTADJUST.DEVICE_SIZE )
			{
				if ( _erg.rtAdjustSize == ECHORTADJUST.AUTO_DETAIL )
				{
					//_erg.autoDetailLevels = EditorGUILayout.IntField ("Number Of Levels", _erg.autoDetailLevels );
					_erg.autoDetailLevels 		= EditorGUILayout.IntSlider ( "Number Of Levels", _erg.autoDetailLevels, 2, 5 );
					_erg.autoDetailMin 			= (float)(EditorGUILayout.IntSlider ( "Minimum Percent", (int)(_erg.autoDetailMin*100.0f), 20, 90 ) / 100.0f );
					_erg.autoDetailFudge 		= EditorGUILayout.Slider ( "Fudge Time",_erg.autoDetailFudge, 0.0f, 3.0f );
					_erg.autoDetailChangeTime   = EditorGUILayout.Slider ( "Change Up Time", _erg.autoDetailChangeTime, 0.0f, 2.0f );
				}
				else
				{
					_erg.rtAdjustWidth = EditorGUILayout.IntField ("Cell Width", _erg.rtAdjustWidth );
					_erg.rtAdjustHeight = EditorGUILayout.IntField ("Cell Height", _erg.rtAdjustHeight );
				}
			}

			EditorGUILayout.Space();
		}

		EditorGUILayout.LabelField("Cameras Depth Range" );
		_erg.cameraDepthStart = EditorGUILayout.FloatField ("Start:", _erg.cameraDepthStart );
		_erg.cameraDepthEnd = EditorGUILayout.FloatField ("End:", _erg.cameraDepthEnd );

		EditorGUILayout.Space();
	}

	//=========================================================================
	private bool IsOptUsed ( List<EchoPFXOption> iepolist, ECHOPFXOPTION iopt )
	{
		for ( int loop = 0; loop < iepolist.Count; loop++ )
		{
			if ( iepolist[loop].optType == iopt )
				return ( true );
		}

		return ( false );
	}
	
	//=========================================================================
	public void EffectPassMake( int ipassid )
	{
		List<EchoPFXOption>[]	passOpt 		= new List<EchoPFXOption>[2];
		List<PossibleOpts>[]    possibleOpts 	= new List<PossibleOpts>[2];
		ECHOPFXOPTION[]  		optLookup;
		EchoPFXOption 			epo;
		int 					loop;
		string[]        		popupNames;
		int 					count;
		int                     index;
		EchoPFXEffect           epe;

		if ( _erg == null )
		{
			return;
		}

		epe = _erg.epeList[_epeIndex];

		passOpt[0] = epe.passOpt1;
		passOpt[1] = epe.passOpt2;

		possibleOpts[0] = _erg.possibleOpts1;
		possibleOpts[1] = _erg.possibleOpts2;

		count = 0;
		for ( loop = 0; loop < possibleOpts[ipassid].Count; loop++ )
		{
			if ( IsOptUsed ( passOpt[ipassid], possibleOpts[ipassid][loop].type ) == false )
			{
				count++;
			}
		}

		popupNames 		= new string[count+1];
		popupNames[0] 	= "Add Effect Option";
		optLookup 		= new ECHOPFXOPTION[count+1];

		count = 1;
		for ( loop = 0; loop < possibleOpts[ipassid].Count; loop++ )
		{
			if ( IsOptUsed ( passOpt[ipassid], possibleOpts[ipassid][loop].type ) == false )
			{
				popupNames[count] = optNames[(int)possibleOpts[ipassid][loop].type];
				optLookup[count] = possibleOpts[ipassid][loop].type;
				count++;
			}
		}

		if ( count > 1 )
		{
			GUILayout.BeginHorizontal();
			index = EditorGUILayout.Popup ( 0, popupNames );
			if ( index > 0 )
			{
				passOpt[ipassid].Add ( new EchoPFXOption ( optLookup[index] ) );
				_erg.ValidateOptions();
			}
			GUILayout.EndHorizontal();
		}

		epe.passOpt[0] = epe.passOpt1;
		epe.passOpt[1] = epe.passOpt2;

		for ( loop = 0; loop < epe.passOpt[ipassid].Count; loop++ )
		{
			epo = epe.passOpt[ipassid][loop];
			
			if ( epo == null )
				continue;

			switch ( epo.optType )
			{
			case ECHOPFXOPTION.GREYSCALE:
				PostFxOptionGUI ( passOpt[ipassid], loop, "Greyscale:" );
				break;

			case ECHOPFXOPTION.INVERSE:
				PostFxOptionGUI ( passOpt[ipassid], loop, "Inverse:" );
				break;
			
			case ECHOPFXOPTION.COLOR:
				if ( PostFxOptionGUI ( passOpt[ipassid], loop, "Color:" ) )
					PostFxColorGUI ( epo );
				break;
			
			case ECHOPFXOPTION.ADD:
				if ( PostFxOptionGUI ( passOpt[ipassid], loop, "Add:" ) )
					PostFxColorGUI ( epo );
				break;
			
			case ECHOPFXOPTION.MULTIPLY:
				if ( PostFxOptionGUI ( passOpt[ipassid], loop, "Multiply:" ) )
					PostFxColorGUI ( epo );
				break;
			
			case ECHOPFXOPTION.NOISE:
				if ( PostFxOptionGUI ( passOpt[ipassid], loop, "Noise:" ) )
					PostFxTextureGUI ( epo );
				break;
			
			case ECHOPFXOPTION.DISTORTION:
				if ( PostFxOptionGUI ( passOpt[ipassid], loop, "Distortion:" ) )
					PostFxDistortionGUI (epo);
				break;
			
			case ECHOPFXOPTION.SHOCKWAVE:
				if ( PostFxOptionGUI ( passOpt[ipassid], loop, "Shockwave:" ) )
					PostFxShockwaveGUI ( epo );
				break;

			case ECHOPFXOPTION.SCANLINES:
				if ( PostFxOptionGUI ( passOpt[ipassid], loop, "Scanlines:" ) )
				{
					PossibleOpts po;
					for (int i=0;i<possibleOpts[ipassid].Count;i++)
					{
						po = possibleOpts[ipassid][i];
						if ( po.type == epo.optType )
						{
							PostFxScanLinesGUI ( epo, po );
							break;
						}
					}
				}
				break;
			
			case ECHOPFXOPTION.LUMRAMP:
				if ( PostFxOptionGUI ( passOpt[ipassid], loop, "Lum Ramp:" ) )
					PostFxTextureGUI ( epo );
				break;

			case ECHOPFXOPTION.COLOR_CORRECT:
				if ( PostFxOptionGUI ( passOpt[ipassid], loop, "Color Correct:" ) )
					PostFxTextureGUI ( epo );
				break;

			case ECHOPFXOPTION.OVERLAY_NORMAL:
				if ( PostFxOptionGUI ( passOpt[ipassid], loop, "Texture:" ) )
					PostFxOverlayGUI ( epo, ipassid );
				break;
				
			case ECHOPFXOPTION.OVERLAY_SCREEN:
				if ( PostFxOptionGUI ( passOpt[ipassid], loop, "Texture Screen:" ) )
					PostFxOverlayGUI ( epo, ipassid );
				break;

			case ECHOPFXOPTION.OVERLAY_ADD:
				if ( PostFxOptionGUI ( passOpt[ipassid], loop, "Texture Add:" ) )
					PostFxOverlayGUI ( epo, ipassid );
				break;
			
			case ECHOPFXOPTION.OVERLAY_SUBTRACT:
				if ( PostFxOptionGUI ( passOpt[ipassid], loop, "Texture Subtract:" ) )
					PostFxOverlayGUI ( epo, ipassid );
				break;

			case ECHOPFXOPTION.OVERLAY_MULTIPLY:
				if ( PostFxOptionGUI ( passOpt[ipassid], loop, "Texture Multiply:" ) )
					PostFxOverlayGUI ( epo, ipassid );
				break;

			case ECHOPFXOPTION.OVERLAY_OVERLAY:
				if ( PostFxOptionGUI ( passOpt[ipassid], loop, "Texture Overlay:" ) )
					PostFxOverlayGUI ( epo, ipassid );
				break;

			case ECHOPFXOPTION.CUSTOM_FRAG_1:
				if ( PostFxOptionGUI ( passOpt[ipassid], loop, "Custom Frag 1:" ) )
					PostFxCustomGUI ( epo, ipassid );
				break;

			case ECHOPFXOPTION.CUSTOM_FRAG_2:
				if ( PostFxOptionGUI ( passOpt[ipassid], loop, "Custom Frag 2:" ) )
					PostFxCustomGUI ( epo, ipassid );
				break;

			case ECHOPFXOPTION.CUSTOM_FRAG_3:
				if ( PostFxOptionGUI ( passOpt[ipassid], loop, "Custom Frag 3:" ) )
					PostFxCustomGUI ( epo, ipassid );
				break;

			case ECHOPFXOPTION.CUSTOM_FRAG_4:
				if ( PostFxOptionGUI ( passOpt[ipassid], loop, "Custom Frag 4:" ) )
					PostFxCustomGUI ( epo, ipassid );
				break;
			}
		}
	}
	
	//=========================================================================
	public void EffectEditorGUI()
	{
		Color oldColor = GUI.color;
		EchoPFXEffect epe;

		if ( _erg == null )
			return;

		if ( _epeIndex < 0 )
			return;

		if ( _epeIndex >= _erg.epeList.Count )
		{
			_epeIndex = 0;
			return;
		}

		epe = _erg.epeList[_epeIndex];

		// postfx options		
		EditorGUILayout.Space();
		
		if ( epe != null )
		{
			EditorGUILayout.BeginHorizontal();
	
			EditorGUILayout.LabelField ( "Name:", GUILayout.MaxWidth(42) );
			epe.name = EditorGUILayout.TextField ( epe.name );
			epe.name = TrimString ( epe.name );

			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.Space();
			
			GUILayout.BeginHorizontal();
			for ( int loop = 0; loop < 2; loop++ )
			{
				if ( loop == _passIndex )
					GUI.color = new Color ( 0.5f, 0.7f, 2.0f, 1.0f );
				
				if ( GUILayout.Button ( "Pass "+(loop+1) ) )
				{
					_passIndex = loop;
				}
				
				GUI.color = oldColor;
			}
			GUILayout.EndHorizontal();
		
			// title
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Effect Options:");
			EditorGUILayout.EndHorizontal();
			
			EffectPassMake( _passIndex );

			EditorGUILayout.Space();
		}
	}
	
	//=========================================================================
	public void MyEffectsGUI()
	{
		int loop;
		int delIndex 	= -1;
		EchoPFXEffect epfx;
		Color old_color = GUI.color;
		EchoPFXEffect epe;

		EditorGUILayout.Space();
		
		if ( GUILayout.Button("Add New Effect"))
		{
			epe 		= new EchoPFXEffect();
			epe.name 	= "New Effect";
			_erg.epeList.Add ( epe );
			_epeIndex = _erg.epeList.IndexOf ( epe );
		}
		
		EditorGUILayout.Space();

		
		if ( _erg == null || _erg.epeList.Count < 1 )
		{
			EditorGUILayout.Space();
			EditorGUILayout.LabelField ( "Effect List Empty: Add a New Effect" );
			EditorGUILayout.Space();
			return;
		}
		

		if ( _erg == null )
			return;

		if ( _epeIndex >=_erg.epeList.Count )
		{
			_epeIndex = 0;
			return;
		}

		epe = _erg.epeList[_epeIndex];
		for ( loop = 0; loop < _erg.epeList.Count; loop++ )
		{
			epfx = _erg.epeList[ loop ];
			
			GUILayout.BeginHorizontal();
			
			epfx.active = GUILayout.Toggle ( epfx.active, "", GUILayout.Width(32) );

			if ( loop == _epeIndex )
				GUI.color = new Color ( 0.5f, 0.7f, 2.0f, 1.0f );
			
			if ( GUILayout.Button ( epfx.name ) )
			{
				EditorGUIUtility.systemCopyBuffer = epfx.name;
				epe   = epfx;
				_epeIndex = loop;
				_curMode  = 2;
			}
			
			GUI.color = old_color;

			if ( GUILayout.Button ( "-", GUILayout.Width(32) ) )
			{
				_customOpt = null;
				delIndex = loop;
			}
			
			GUILayout.EndHorizontal();
		}

		if ( delIndex >= 0 && _erg.epeList.Count > 0 )
		{
			_erg.epeList.RemoveAt ( delIndex );
			
			if (  _epeIndex >= _erg.epeList.Count )
				_epeIndex = _erg.epeList.Count-1;
		}
	}
	
	//=========================================================================
	private string[] MakePopupNames ( List<PossibleOpts> iopts, int[] ilookup )
	{
		int loop;
		string[] names;
		int count 			= 0;
		bool[] optsInUse    = new bool [(int)ECHOPFXOPTION.COUNT];

		Array.Clear ( optsInUse, 0, optsInUse.Length ); 

		for ( loop = 0; loop < iopts.Count; loop++ )
		{
			optsInUse[(int)iopts[loop].type] = true;
		}

		for ( loop = 0; loop < optsInUse.Length; loop++ )
		{
			if ( !optsInUse[loop] )
			{
				ilookup[count] = loop;
				count++;
			}
		}

		names 		= new string[count+1];
		names[0] 	= "Add Effect Option";
		count 		= 0;
		
		for ( loop = 1; loop < names.Length; loop++ )
		{
			names [loop] = optNames [ilookup[count]];
			count++;
		}

		return ( names );
	}

	//=========================================================================
	public static int HasCustomOptions ( ECHOPFXOPTION ipo )
	{
		int rc = 0;

		switch ( ipo )
		{
		case ECHOPFXOPTION.OVERLAY_NORMAL:
		case ECHOPFXOPTION.OVERLAY_SCREEN:
		case ECHOPFXOPTION.OVERLAY_ADD:
		case ECHOPFXOPTION.OVERLAY_SUBTRACT:
		case ECHOPFXOPTION.OVERLAY_MULTIPLY:
		case ECHOPFXOPTION.OVERLAY_OVERLAY:
			rc = 1;
			break;

		case ECHOPFXOPTION.CUSTOM_FRAG_1:
		case ECHOPFXOPTION.CUSTOM_FRAG_2:
		case ECHOPFXOPTION.CUSTOM_FRAG_3:
		case ECHOPFXOPTION.CUSTOM_FRAG_4:
			rc = 2;
			break;

		case ECHOPFXOPTION.SCANLINES:
			rc = 3;
			break;

		}

		return ( rc );
	}
	
	//=========================================================================
	public void EffectBuildOptionsGUI()
	{
		int 			index1;		
		int 			loop;
		PossibleOpts 	opt;
		string[]        popupNames1;
		string[]        popupNames2;
		string[][]      popupNames;
		int[]           lookup1;
		int[]           lookup2;
		int[][]         lookup;
		int             count;
		PossibleOpts   	po;
		List<PossibleOpts>[] 	possibleOpts;

		if ( _erg == null )
		{
			return;
		}

		EditorGUI.BeginChangeCheck();
		lookup1 = new int [ (int)ECHOPFXOPTION.COUNT ];
		lookup2 = new int [ (int)ECHOPFXOPTION.COUNT ];

		_erg.possibleOpts1.Sort ( delegate ( PossibleOpts e1, PossibleOpts e2 ) 
		{
				return ( e1.order.CompareTo ( e2.order ) );
		});

		_erg.possibleOpts2.Sort ( delegate ( PossibleOpts e1, PossibleOpts e2 ) 
		{
			return ( e1.order.CompareTo ( e2.order ) );
		});

		// setup pass one stuff
		for ( loop = 0; loop < _erg.possibleOpts1.Count; loop++ )
		{
			_erg.possibleOpts1[loop].order = loop;
		}
		
		popupNames1 = MakePopupNames ( _erg.possibleOpts1, lookup1 );

		// setup pass two stuff
		
		for ( loop = 0; loop < _erg.possibleOpts2.Count; loop++ )
		{
			_erg.possibleOpts2[loop].order = loop;
		}
		
		popupNames2 = MakePopupNames ( _erg.possibleOpts2, lookup2 );

		popupNames = new string[2][];
		lookup = new int[2][];
		possibleOpts = new List<PossibleOpts>[2];

		popupNames[0] = popupNames1;
		popupNames[1] = popupNames2;

		lookup[0] = lookup1;
		lookup[1] = lookup2;

		possibleOpts[0] = _erg.possibleOpts1;
		possibleOpts[1] = _erg.possibleOpts2;

		EditorGUILayout.BeginHorizontal();
		for ( int pass = 0; pass < 2; pass++ )
		{
			EditorGUILayout.BeginVertical();
			EditorGUILayout.LabelField ("Pass:"+(pass+1), GUILayout.MaxWidth(48) );

			if ( possibleOpts[pass].Count < 6 )
			{
				index1 = EditorGUILayout.Popup ( 0, popupNames[pass], GUILayout.MinWidth(64) ) - 1;
			}
			else
			{
				EditorGUILayout.LabelField ( "Options: ", GUILayout.MinWidth(64) );
				index1 = -1;
			}
		
			if ( index1 >= 0 )
			{
				po = new PossibleOpts ( (ECHOPFXOPTION)lookup[pass][index1] ); 
				po.order = possibleOpts[pass].Count;
				possibleOpts[pass].Add ( po );
				_erg.ValidateOptions();
			}

			// draw selected options
			count = 0;
			for ( loop = possibleOpts[pass].Count-1; loop >= 0; loop-- )
			{
				opt = possibleOpts[pass][possibleOpts[pass].Count-1-loop];
				
				count++;
				
				EditorGUILayout.BeginHorizontal();
				
				if ( HasCustomOptions ( opt.type ) == 0 )
				{
					GUILayout.Label ( optNames[(int)opt.type] );
				}
				else
				{
					if ( GUILayout.Button ( optNames[(int)opt.type], GUILayout.MinWidth(48) ) )
					{
						_customOpt = opt;
					}
				}
				
				if ( loop !=  possibleOpts[pass].Count-1 )
				{
					if ( GUILayout.Button ( "UP", GUILayout.Width(32) ) )
					{
						_customOpt = null;
						opt.order-=1.5f;
					}
				}
				else
				{
					GUILayout.Label ( " ", GUILayout.Width(32) );
				}
				
				if ( loop > 0 )
				{
					if ( GUILayout.Button ( "DN", GUILayout.Width(32) ) )
					{
						_customOpt = null;
						opt.order+=1.5f;
					}
				}
				else
				{
					GUILayout.Label ( " ", GUILayout.Width(32) );
				}
				
				if ( GUILayout.Button ( "-", GUILayout.Width(24) ) )
				{
					_customOpt = null;
					_erg.RemoveOptionOfType ( opt.type, pass );
					possibleOpts[pass].Remove ( opt );
				}
				
				GUILayout.EndHorizontal();
			}

			EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical(GUILayout.Width(16));
			EditorGUILayout.EndVertical();
		}

		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space();
		EditorGUILayout.Space();

		epfxm.UpdateAllRenderGroups();

		// 			sampler2D	_echoCustomF1Tex;
		// fixed 	_echoCustomF1Fade;
		//fixed4     _echoCustomF1Args;


		if ( _customOpt != null )
		{
			GUILayout.Box( "", new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(2)});
			EditorGUILayout.LabelField ("Options:" );
			EditorGUILayout.Space();

			switch ( HasCustomOptions ( _customOpt.type ) )
			{
			case 1:
				_customOpt.useUniqueTC = GUILayout.Toggle ( _customOpt.useUniqueTC, " Use Unique TexCoord ( Scroll UV Effects )" );
				
				EditorGUILayout.Space();
				if ( GUILayout.Button ( "Done Editing", GUILayout.Width(128) ) )
					_customOpt = null;
				break;

			case 2:
				_customOpt.useUniqueTC = GUILayout.Toggle ( _customOpt.useUniqueTC, " Use Unique TexCoord ( Scroll UV Effects )" );
				
				EditorGUILayout.Space();
				EditorGUILayout.LabelField ("Fragment Shader Code:" );
				_customOpt.customCode = EditorGUILayout.TextArea ( _customOpt.customCode, GUILayout.Height(128) );
				
				if ( GUILayout.Button ( "Done Editing", GUILayout.Width(128) ) )
					_customOpt = null;
				
				//SCOTTFIND
				EditorGUILayout.Space();
				LayoutHelp ( "fixed3"	, "_RGB.", "Screen RGB Colors" );
				LayoutHelp ( "sampler2D", "_Tex", "Texture" );
				LayoutHelp ( "float4"	, "_Tex_ST", "Tiling(xy) Offset(zw)" );
				LayoutHelp ( "texcoord"	, "_TC" , "Texture Coords" );
				LayoutHelp ( "fixed"	, "_Fade", "Percent 0.0-1.0" );
				LayoutHelp ( "fixed4"	, "_Args", "User Values" );
				LayoutHelp ( "fixed4"	, "_Color", "RGBA" );
				break;

			case 3:
				_customOpt.scanlines = (ECHOPFXSCANLINES)EditorGUILayout.EnumPopup ("Scanline Types", _customOpt.scanlines );

				if ( GUILayout.Button ( "Done Editing", GUILayout.Width(128) ) )
					_customOpt = null;
				break;

			default:
				break;
			}


		}

		EditorGUILayout.Space();

		if ( EditorGUI.EndChangeCheck() )
			_needCompile = true;

	}

	public void LayoutHelp ( string item1, string item2, string item3 )
	{
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField ( item1, GUILayout.Width(72) );
		EditorGUILayout.LabelField ( item2, GUILayout.Width(96) );
		EditorGUILayout.LabelField ( item3 );
		GUILayout.EndHorizontal();
	}
	
	//=========================================================================
	public void MyRenderGroupsGUI()
	{
		int loop;
		int delIndex = -1;
		EchoPFXRenderGroup erg;
		Color old_color = GUI.color;

		EditorGUILayout.Space();
		if ( GUILayout.Button ( "Add Render Group") )
		{
			SetNewRenderGroup ( new EchoPFXRenderGroup( GetGroupID() ), true );
			epfxm.SortRenderGroups();
		}

		EditorGUILayout.Space();

		if ( epfxm.ergList.Count < 1 )
		{
			EditorGUILayout.Space();
			EditorGUILayout.LabelField ( "No Render Groups !" );
			EditorGUILayout.Space();
			return;
		}
		
		for ( loop = 0; loop < epfxm.ergList.Count; loop++ )
		{
			erg = epfxm.ergList[ loop ];
			
			if ( erg == null )
			{
				continue;
			}
			
			GUILayout.BeginHorizontal();

			if ( loop == _ergIndex )
				GUI.color = new Color ( 0.5f, 0.7f, 2.0f, 1.0f );

			if ( GUILayout.Button ( erg.name ) )
			{
				EditorGUIUtility.systemCopyBuffer = erg.name;
				SetNewRenderGroup ( erg, false );
				_curMode = 0;
			}
			
			GUI.color = old_color;

			if ( GUILayout.Button ( "-", GUILayout.Width(32) ) )
			{
				_customOpt = null;
				delIndex = loop;
			}
			
			GUILayout.EndHorizontal();

		}
		
		if ( delIndex >= 0 )
		{
			if ( epfxm.ergList.Count > 0 )
			{
				epfxm.ergList.RemoveAt ( delIndex );

				_ergIndex--;
				
				if ( _ergIndex < 0 )
				{
					_ergIndex = 0;
					if ( epfxm.ergList.Count < 1 )
						_erg = null;
				}
			}
			else
			{
				_erg 		= null;
				_ergIndex 	= 0;
			}
		}
	}

	//=========================================================================
	public bool TogglePlayMakerActions()
	{
		string searchText = "#if false";
		string replaceText = "#if true";
		bool enabled = false;

		enabled = ReplaceInFile ( Application.dataPath + "/echoLogin/PFX/Plugins/PlayMakerActions/EchoTriggerPostEffect.cs", searchText, replaceText);
		enabled = ReplaceInFile ( Application.dataPath + "/echoLogin/PFX/Plugins/PlayMakerActions/EchoTriggerShockwaveXY.cs", searchText, replaceText);

		AssetDatabase.Refresh();

		return ( enabled );
	}
	
	//=========================================================================
	public override void OnInspectorGUI()
	{
		string[] names = { "Group", "Build", "Make" };
		int loop;
		Color oldColor = GUI.color;
		string cname;
		bool oldToggle;
		
		serializedObject.Update();
//		postFX.Update();
		
		if ( epfxm.ergList.Count > 0 )
		{
			SetNewRenderGroup ( epfxm.ergList[ _ergIndex ], false );
		}
		
		EditorGUILayout.Space();
		
		epfxm.managerCameraDepth 	= EditorGUILayout.FloatField (  "Manager Depth", epfxm.managerCameraDepth );
		epfxm.frameRate 			= EditorGUILayout.IntField (  "Frame Rate", epfxm.frameRate );

		oldToggle = _playMaker;
		_playMaker 					= EditorGUILayout.Toggle ("PlayMaker Actions", _playMaker );
		if ( oldToggle != _playMaker )
		{
			_playMaker = TogglePlayMakerActions();
		}

		GUILayout.BeginHorizontal();
		if ( GUILayout.Button ( "Export" ) )
		{
			_customOpt 	= null;
			EchoPFXLoadSave.Save( epfxm );
		}
		if ( GUILayout.Button ( "Import" ) )
		{
			_customOpt 		= null;
			_needCompile 	= true;
			SetNewRenderGroup ( EchoPFXLoadSave.Load( epfxm ), false );
			EditorUtility.SetDirty(target);
		}
		GUILayout.EndHorizontal();

		
		EditorGUILayout.Space();
		GUILayout.Box( "", new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(8)});
		EditorGUILayout.Space();

		_myrgFold = EditorGUILayout.Foldout (_myrgFold, new GUIContent("Render Groups") );
		if ( _myrgFold )
			MyRenderGroupsGUI();
		
		EditorGUILayout.Space();
		
		EditorGUILayout.Space();
		GUILayout.Box( "", new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(8)});
		EditorGUILayout.Space();	
		
		_myfxFold = EditorGUILayout.Foldout ( _myfxFold, new GUIContent("Post Effects") );

		if ( _myfxFold )
			MyEffectsGUI();
		
		EditorGUILayout.Space();
		GUILayout.Box("", new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(8)});
		EditorGUILayout.Space();

		EditorGUILayout.Space();

		if ( _needCompile )
		{
			cname = ">> Compile Post FX Shaders <<";
			GUI.color = new Color ( 2.0f, 0.5f, 0.0f, 1.0f );
		}
		else
		{
			cname = "Compile Post FX Shaders";
		}

		if ( GUILayout.Button ( cname ,GUILayout.Height(24) ) )
		{
			EchoPFXShaderBuild.BuildShaders ( epfxm.ergList );
			_needCompile = false;
		}
		GUI.color = oldColor;

		EditorGUILayout.Space();
		GUILayout.Box("", new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(8)});

		EditorGUILayout.Space();
		
		GUILayout.BeginHorizontal();
		for ( loop = 0; loop < 3; loop++ )
		{
			if ( loop == _curMode )
				GUI.color = new Color ( 0.5f, 0.7f, 2.0f, 1.0f );
			
			if ( GUILayout.Button ( names[loop] ) )
			{
				_customOpt 	= null;
				_curMode 	= loop;
				epfxm.UpdateAllRenderGroups();
			}
			
			GUI.color = oldColor;
		}
		GUILayout.EndHorizontal();

		EditorGUILayout.Space();
		
		switch ( _curMode )
		{
		case 1:
			EffectBuildOptionsGUI();
			break;
		
		case 2:
			EffectEditorGUI();
			break;
			
		default:
			RenderGroupGUI();
			break;
		}

		if ( GUI.changed )
		{
			EditorUtility.SetDirty(target);
		}

		serializedObject.ApplyModifiedProperties();
		
	}

	public void AutoCompileShaders()
	{
		if ( _needCompile )
		{
			EchoPFXShaderBuild.BuildShaders ( epfxm.ergList );
			_needCompile = false;
		}
	}
}

