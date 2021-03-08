// (c) copyright echoLogin LLC 2013. All rights reserved.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum ECHOPFXSCANLINES
{
	HORIZONTAL = 0,
	VERTICAL,
	BOTH
};

public enum ECHOPFXBLEND
{
	NORMAL = 0,
	SCREEN,
	ADD,
	SUBTRACT,
	MULTIPLY
};

public enum ECHORTADJUST
{
	DEVICE_SIZE = 0,
	DIVIDE,
	AUTO_DETAIL,
	CUSTOM
};

[System.Serializable]
public class PossibleOpts
{
	public ECHOPFXOPTION 	type;
	public float         	order;
	public string        	customCode;
	public bool          	useUniqueTC;
	public ECHOPFXSCANLINES scanlines;

	//=========================================================================
	public PossibleOpts ( ECHOPFXOPTION iopt )
	{
		scanlines = ECHOPFXSCANLINES.HORIZONTAL;
		order = 99 + (int)iopt;
		type  = iopt;
	}

	//=========================================================================
	public PossibleOpts ( ECHOPFXOPTION iopt, int iorder )
	{
		scanlines = ECHOPFXSCANLINES.HORIZONTAL;
		order 		= iorder;
		type  		= iopt;
		useUniqueTC = false;
	}
};

[System.Serializable]
public class EchoPFXRenderGroup 
{
	public string           		name;
	public int                      id;
	public int                      passCount;
	public bool 					active;
	public float 					cameraDepthStart;
	public float 					cameraDepthEnd;
	public int   					meshCellWidth    		= 16;
	public int   					meshCellHeight   		= 16;
	public ECHORTADJUST 			rtAdjustSize        	= ECHORTADJUST.DEVICE_SIZE;
	public int   					rtAdjustWidth  			= 0;
	public int   					rtAdjustHeight 			= 0;
	public FilterMode[] 			rtFilterMode   			= new FilterMode[2] { FilterMode.Point, FilterMode.Point };
	public ECHOPFXBLEND[] 			rtBlendMode        		= new ECHOPFXBLEND[2] { ECHOPFXBLEND.NORMAL, ECHOPFXBLEND.NORMAL };
	public List<EchoPFXEffect> 		epeList             	= new List<EchoPFXEffect>();
	public float                    autoDetailMin       	= 0.5f;
	public int                    	autoDetailLevels    	= 3;
	public float                    autoDetailFudge     	= 0.5f;
	public float 					autoDetailChangeTime	= 0.25f;

	// i think only editor needs shit below
	public List<PossibleOpts> 		possibleOpts1 		= new List<PossibleOpts>();
	public List<PossibleOpts> 		possibleOpts2 		= new List<PossibleOpts>();
	public EchoList<EchoPFXEffect> 	epeEchoList;
	// SCOTTFIND
//	public bool[] 					optsInUse1    		= new bool [(int)ECHOPFXOPTION.COUNT];
//	public bool[] 					optsInUse2    		= new bool [(int)ECHOPFXOPTION.COUNT];

	private int  					_autoDetailFrame 		= 0;
	private int 					_autoDetailChange 		= 0;
	private float 					_autoDetailTime 		= 0.25f;
	private int 					_autoDetailTargetTime	= 0;
	private int[]                   _autoDetailChangeCount;

	private EchoPFXRenderTexture[]	 	_renTex;
	private EchoPFXRenderTexture[]	 	_autoTexPass1;
	private EchoPFXRenderTexture[]	 	_autoTexPass2;
	private EchoPFXRenderTexture[][]	_autoTex;

	private List<Camera>     		_camList;
	private Mesh             		_mesh;
//	private Color32[]				_meshColor;
	private string[][]     			_finalKeywords;
	private int[][] 				_finalKeyFlags;	
	private int[][] 				_prevKeyFlags;	
	private EchoPFXRenderMaterial[] _echoRenderMaterial;
	private int                     _mainTexID;

	public static readonly string[,]   _keywords 		= new string[(int)ECHOPFXOPTION.COUNT,2]	
	{
		{ "ECHO_PFX_GREYSCALE_OFF", "ECHO_PFX_GREYSCALE_ON" }, 
		{ "ECHO_PFX_INVERSE_OFF", "ECHO_PFX_INVERSE_ON" },     
		{ "ECHO_PFX_COLOR_OFF", "ECHO_PFX_COLOR_ON" },         
		{ "ECHO_PFX_ADD_OFF", "ECHO_PFX_ADD_ON" },             
		{ "ECHO_PFX_MULT_OFF", "ECHO_PFX_MULT_ON" },           
		{ "ECHO_PFX_NOISE_OFF", "ECHO_PFX_NOISE_ON" },         
		{ "ECHO_PFX_DISTORT_OFF", "ECHO_PFX_DISTORT_ON" },     
		{ "ECHO_PFX_SHOCKWAVE_OFF", "ECHO_PFX_SHOCKWAVE_ON" }, 
		{ "ECHO_PFX_SCANLINE_OFF", "ECHO_PFX_SCANLINE_ON" },   
		{ "ECHO_PFX_RAMP_OFF", "ECHO_PFX_RAMP_ON" },           
		{ "ECHO_PFX_CORRECT_OFF", "ECHO_PFX_CORRECT_ON" },           
		{ "ECHO_PFX_OVERLAY_OFF", "ECHO_PFX_OVERLAY_ON" },     
		{ "ECHO_PFX_OVERLAY_SCR_OFF", "ECHO_PFX_OVERLAY_SCR_ON" },     
		{ "ECHO_PFX_OVERLAY_ADD_OFF", "ECHO_PFX_OVERLAY_ADD_ON" },     
		{ "ECHO_PFX_OVERLAY_SUB_OFF", "ECHO_PFX_OVERLAY_SUB_ON" },     
		{ "ECHO_PFX_OVERLAY_MUL_OFF", "ECHO_PFX_OVERLAY_MUL_ON" },     
		{ "ECHO_PFX_OVERLAY_OVR_OFF", "ECHO_PFX_OVERLAY_OVR_ON" },     
		{ "ECHO_PFX_CUSTOM_FRAG_1_OFF", "ECHO_PFX_CUSTOM_FRAG_1_ON" },     
		{ "ECHO_PFX_CUSTOM_FRAG_2_OFF", "ECHO_PFX_CUSTOM_FRAG_2_ON" },     
		{ "ECHO_PFX_CUSTOM_FRAG_3_OFF", "ECHO_PFX_CUSTOM_FRAG_3_ON" },     
		{ "ECHO_PFX_CUSTOM_FRAG_4_OFF", "ECHO_PFX_CUSTOM_FRAG_4_ON" },     
	};
	
	//=========================================================================
	public EchoPFXRenderGroup ( int iid )
	{
		id = iid;
		name = "RenderGroup:"+id;
	}

	//=========================================================================
	public void Render( Transform icamtrans )
	{
		int 					index;
		bool 					flag;
		string[]        		finalKeywords;
		EchoPFXEffect 			epe;
		EchoPFXEffect 			epeNext;
		EchoPFXOption   		epo;
		EchoPFXRenderMaterial 	erm;

	 	EchoPFXRenderTexture.Active ( _renTex, 0, passCount );
	
		GL.PushMatrix();
        GL.LoadOrtho();

		for ( int pass = 0; pass < passCount; pass++ )
		{
			erm = _echoRenderMaterial[pass];
			
			erm.ResetAllFrameCounts();
			
			Array.Copy ( _finalKeyFlags[pass], _prevKeyFlags[pass], _finalKeyFlags[pass].Length );
			Array.Clear ( _finalKeyFlags[pass], 0, _finalKeyFlags[pass].Length );

			for ( epe = epeEchoList.GetFirstActive(); epe != null; epe = epeNext )
			{
				epeNext = epeEchoList.GetNext(epe);

				if ( epe.passActive[pass])
				{
					for ( index = 0; index < epe.passOpt[pass].Count; index++ )
					{
						epo     = epe.passOpt[pass][index];

						switch ( epo.Process() )
						{
						case -1:
							epe.optionsOff++;
							if ( epe.optionsOff >= epe.optionsTotal )
							{
								epeEchoList.Deactivate ( epe );
							}
							break;

						case 0:
							break;

						default:
							// should index into order of passopts
							_finalKeyFlags[pass][epo.passOrder] = _finalKeyFlags[pass][epo.passOrder] | 1;
							
							switch ( epo.optType )
							{
							case ECHOPFXOPTION.GREYSCALE:
								erm.SetFloat ( ECHOPROP.GREYFADE, epo.fadeCur );
								break;
								
							case ECHOPFXOPTION.INVERSE:
								erm.SetFloat ( ECHOPROP.INVERSEFADE, epo.fadeCur );
								break;
								
							case ECHOPFXOPTION.COLOR:
								erm.SetVector ( ECHOPROP.COLOR, epo.rgba * epo.rgbaMultiply );
								erm.SetFloat ( ECHOPROP.COLORFADE, epo.fadeCur );
								break;
								
							case ECHOPFXOPTION.ADD:
								erm.SetVector ( ECHOPROP.ADD, epo.rgba * epo.rgbaMultiply );
								erm.SetFloat ( ECHOPROP.ADDFADE, epo.fadeCur );
								break;
								
							case ECHOPFXOPTION.MULTIPLY:
								erm.SetVector ( ECHOPROP.MULT, epo.rgba * epo.rgbaMultiply );
								erm.SetFloat ( ECHOPROP.MULTFADE, epo.fadeCur );
								break;
								
							case ECHOPFXOPTION.NOISE:
								erm.SetFloat ( ECHOPROP.NOISEFADE, epo.fadeCur );
								_echoRenderMaterial[pass].mat.SetTexture ("_NoiseTex", epo.tex );
								break;
								
							case ECHOPFXOPTION.DISTORTION:
								erm.SetVector ( ECHOPROP.DISTPARAMSH, new Vector4 ( epo.distSpeedH, epo.distAmountH, ( 1.0f / (float)meshCellWidth ) * epo.distStrengthH, epo.fadeCur ) );
								erm.SetVector ( ECHOPROP.DISTPARAMSV, new Vector4 ( epo.distSpeedV, epo.distAmountV, ( 1.0f / (float)meshCellHeight ) * epo.distStrengthV, epo.fadeCur ) );
								break;
								
							case ECHOPFXOPTION.SHOCKWAVE:
								erm.SetVector ( ECHOPROP.SHOCKPARAMS, new Vector4 ( epo.shockCenterX, epo.shockCenterY, epo.shockDistance, epo.shockSize ) );
								erm.SetFloat ( ECHOPROP.SHOCKFADE, epo.fadeCur );
								break;
								
							case ECHOPFXOPTION.SCANLINES:
								erm.SetFloat ( ECHOPROP.SCANLINEFADE, epo.fadeCur );
								if ( epo.linesAmountDivideH )
									erm.SetFloat ( ECHOPROP.SCANLINECOUNTH, (float)Screen.height / (float)epo.linesAmountH );
								else
									erm.SetFloat ( ECHOPROP.SCANLINECOUNTH, epo.linesAmountH );
								erm.SetFloat ( ECHOPROP.SCANLINESCROLLH, epo.linesScrollH );

								if ( epo.linesAmountDivideV )
									erm.SetFloat ( ECHOPROP.SCANLINECOUNTV, (float)Screen.width / (float)epo.linesAmountV );
								else
									erm.SetFloat ( ECHOPROP.SCANLINECOUNTV, epo.linesAmountV );

								erm.SetFloat ( ECHOPROP.SCANLINESCROLLV, epo.linesScrollV );
								break;
								
							case ECHOPFXOPTION.LUMRAMP:     
								erm.SetTexture ( ECHOPROP.RAMP_TEX, epo.tex );
								erm.SetFloat ( ECHOPROP.RAMPFADE, epo.fadeCur );
								break;
								
							case ECHOPFXOPTION.COLOR_CORRECT:   
								erm.SetTexture ( ECHOPROP.CORRECT_TEX, epo.tex );
								erm.SetFloat ( ECHOPROP.CORRECTFADE, epo.fadeCur );
								break;
								
							case ECHOPFXOPTION.OVERLAY_NORMAL:
								erm.SetTexture ( ECHOPROP.OVERLAY_TEX, epo.tex );
								erm.SetVector ( ECHOPROP.OVERLAY_ST, epo.overlayST );
								erm.SetFloat ( ECHOPROP.OVERLAY_FADE, epo.fadeCur );
								break;
								
							case ECHOPFXOPTION.OVERLAY_SCREEN:
								erm.SetTexture ( ECHOPROP.OVERLAYSCR_TEX, epo.tex );
								erm.SetFloat ( ECHOPROP.OVERLAYSCR_FADE, epo.fadeCur );
								erm.SetVector ( ECHOPROP.OVERLAYSCR_ST, epo.overlayST );
								break;
								
							case ECHOPFXOPTION.OVERLAY_ADD:
								erm.SetTexture ( ECHOPROP.OVERLAYADD_TEX, epo.tex );
								erm.SetFloat ( ECHOPROP.OVERLAYADD_FADE, epo.fadeCur );
								erm.SetVector ( ECHOPROP.OVERLAYADD_ST, epo.overlayST );
								break;
								
							case ECHOPFXOPTION.OVERLAY_SUBTRACT:
								erm.SetTexture ( ECHOPROP.OVERLAYSUB_TEX, epo.tex );
								erm.SetFloat ( ECHOPROP.OVERLAYSUB_FADE, epo.fadeCur );
								erm.SetVector ( ECHOPROP.OVERLAYSUB_ST, epo.overlayST );
								break;
								
							case ECHOPFXOPTION.OVERLAY_MULTIPLY:
								erm.SetTexture ( ECHOPROP.OVERLAYMUL_TEX, epo.tex );
								erm.SetFloat ( ECHOPROP.OVERLAYMUL_FADE, epo.fadeCur );
								erm.SetVector ( ECHOPROP.OVERLAYMUL_ST, epo.overlayST );
								break;

							case ECHOPFXOPTION.OVERLAY_OVERLAY:
								erm.SetTexture ( ECHOPROP.OVERLAYOVR_TEX, epo.tex );
								erm.SetFloat ( ECHOPROP.OVERLAYOVR_FADE, epo.fadeCur );
								erm.SetVector ( ECHOPROP.OVERLAYOVR_ST, epo.overlayST );
								break;

							case ECHOPFXOPTION.CUSTOM_FRAG_1:
								if ( epo.tex != null )
								{
									erm.SetTexture ( ECHOPROP.CUSTOM_FRAG_1_TEX, epo.tex );
									erm.SetVector ( ECHOPROP.CUSTOM_FRAG_1_ST, epo.overlayST );
								}
								
								erm.SetFloat ( ECHOPROP.CUSTOM_FRAG_1_FADE, epo.fadeCur );
								erm.SetVector ( ECHOPROP.CUSTOM_FRAG_1_ARGS, epo.customArgs );
								erm.SetVector ( ECHOPROP.CUSTOM_FRAG_1_COLOR, epo.rgba * epo.rgbaMultiply );
								break;
								
							case ECHOPFXOPTION.CUSTOM_FRAG_2:
								if ( epo.tex != null )
								{
									erm.SetTexture ( ECHOPROP.CUSTOM_FRAG_2_TEX, epo.tex );
									erm.SetVector ( ECHOPROP.CUSTOM_FRAG_2_ST, epo.overlayST );
								}
								
								erm.SetFloat ( ECHOPROP.CUSTOM_FRAG_2_FADE, epo.fadeCur );
								erm.SetVector ( ECHOPROP.CUSTOM_FRAG_2_ARGS, epo.customArgs );
								erm.SetVector ( ECHOPROP.CUSTOM_FRAG_2_COLOR, epo.rgba * epo.rgbaMultiply );
								break;
								
							case ECHOPFXOPTION.CUSTOM_FRAG_3:
								if ( epo.tex != null )
								{
									erm.SetTexture ( ECHOPROP.CUSTOM_FRAG_3_TEX, epo.tex );
									erm.SetVector ( ECHOPROP.CUSTOM_FRAG_3_ST, epo.overlayST );
								}
								
								erm.SetFloat ( ECHOPROP.CUSTOM_FRAG_3_FADE, epo.fadeCur );
								erm.SetVector ( ECHOPROP.CUSTOM_FRAG_3_ARGS, epo.customArgs );
								erm.SetVector ( ECHOPROP.CUSTOM_FRAG_3_COLOR, epo.rgba * epo.rgbaMultiply );
								break;
								
							case ECHOPFXOPTION.CUSTOM_FRAG_4:
								if ( epo.tex != null )
								{
									erm.SetTexture ( ECHOPROP.CUSTOM_FRAG_4_TEX, epo.tex );
									erm.SetVector ( ECHOPROP.CUSTOM_FRAG_4_ST, epo.overlayST );
								}
								
								erm.SetFloat ( ECHOPROP.CUSTOM_FRAG_4_FADE, epo.fadeCur );
								erm.SetVector ( ECHOPROP.CUSTOM_FRAG_4_ARGS, epo.customArgs );
								erm.SetVector ( ECHOPROP.CUSTOM_FRAG_4_COLOR, epo.rgba * epo.rgbaMultiply );
								break;
							}
							break;
						}
					}
				}
			}
			
			erm.SubmitShaderProperties();
			finalKeywords 	= _finalKeywords[pass];
			flag 			= false;
			
			if ( pass == 0 )
			{
				for ( index = 0; index < possibleOpts1.Count; index++ )
				{
					finalKeywords[index] = _keywords[ (int)possibleOpts1[index].type, _finalKeyFlags[pass][index] ];
					
					if ( _finalKeyFlags[pass][index] != _prevKeyFlags[pass][index] )
					{
						flag = true;
					}
				}
			}
			else
			{
				for ( index = 0; index < possibleOpts2.Count; index++ )
				{
					finalKeywords[index] = _keywords[ (int)possibleOpts2[index].type, _finalKeyFlags[pass][index] ];
					
					if ( _finalKeyFlags[pass][index] != _prevKeyFlags[pass][index] )
					{
						flag = true;
					}
				}
			}
	
			if ( flag )
			{
				_echoRenderMaterial[pass].mat.shaderKeywords = finalKeywords;
			}

#if UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1
			_echoRenderMaterial[pass].mat.SetTexture ( "_echoScreen", _renTex[pass].texture );
#else
			_echoRenderMaterial[pass].mat.SetTexture ( _mainTexID, _renTex[pass].texture );
#endif
			_echoRenderMaterial[pass].mat.SetPass(0);

			Graphics.DrawMeshNow ( _mesh, icamtrans.position, icamtrans.rotation );

			if ( passCount > 1 )
				EchoPFXRenderTexture.Active ( _renTex, 1, passCount );
		}

		GL.PopMatrix();

		_renTex[0].DiscardContents();
		
		if ( passCount > 1 )
			_renTex[1].DiscardContents();
	}
	
	//=========================================================================
	public void PrepareForRuntime ( Camera[] icameras, float ifps )
	{
		int 				loop;
		int 				screenWidth;
		int 				screenHeight;
		EchoPFXEffect       epe;
		string 				shaderName;
		float               per;

		_mainTexID = Shader.PropertyToID ("_echoScreen");
		
		passCount = 1;
		if ( possibleOpts2.Count > 0 )
			passCount++;

		ValidateOptions();

		_autoDetailTargetTime = (int)( ( 1.0f / ( (float)ifps - autoDetailFudge ) ) * 10000.0f );

		// hold the final shader keywords
		_finalKeywords 		= new string[2][];
		_finalKeywords[0] 	= new string[ possibleOpts1.Count ];
		_finalKeywords[1] 	= new string[ possibleOpts2.Count ];
		
		// used to accumylate the effects that are in use
		_finalKeyFlags = new int[2][];
		_finalKeyFlags[0] 	= new int[ possibleOpts1.Count ];
		_finalKeyFlags[1] 	= new int[ possibleOpts2.Count ];

		// used to compare if anything has changed from last frame
		_prevKeyFlags = new int[2][];
		_prevKeyFlags[0] 	= new int[ possibleOpts1.Count ];
		_prevKeyFlags[1] 	= new int[ possibleOpts2.Count ];

		FindCameras ( icameras );
		CreateMesh();
		
		_echoRenderMaterial = new EchoPFXRenderMaterial[passCount];
		
		for ( loop = 0; loop < passCount; loop++ )
		{
			shaderName = "echoLogin/PFX/echologin_postfx_group" + id + "_pass" + (loop+1);

			Shader shd = Shader.Find ( shaderName );
			
			_echoRenderMaterial[loop] = new EchoPFXRenderMaterial ( new Material ( shd ) );
		}
		
		_renTex = new EchoPFXRenderTexture[2];

		if (!EchoPFXManager.unityPro )
			rtAdjustSize = ECHORTADJUST.DEVICE_SIZE;

		// Find Render Tex Size
		switch ( rtAdjustSize )
		{
		case ECHORTADJUST.AUTO_DETAIL:
			screenWidth		= Screen.width;
			screenHeight 	= Screen.height;
			
			_autoTexPass1 			= new  EchoPFXRenderTexture[autoDetailLevels];
			_autoTexPass1[0] 		= new EchoPFXRenderTexture ( screenWidth, screenHeight, 16, rtFilterMode[0] );

			for ( loop = 1; loop < autoDetailLevels; loop++ )
			{
				per = Mathf.Lerp ( 1.0f, autoDetailMin, ( (float)loop / (float)( autoDetailLevels - 1 ) ) );
				_autoTexPass1[loop] 	= new EchoPFXRenderTexture ( (int)( (float)screenWidth * per), (int)( (float)screenHeight * per ), 16, rtFilterMode[0] );
			}

			if ( passCount > 1 )
			{
				_autoTexPass2 		= new EchoPFXRenderTexture[autoDetailLevels];
				_autoTexPass2[0] 	= new EchoPFXRenderTexture ( screenWidth, screenHeight, 0, rtFilterMode[0] );
				
				for ( loop = 1; loop < autoDetailLevels; loop++ )
				{
					per = Mathf.Lerp ( 1.0f, autoDetailMin, ( (float)loop / (float)( autoDetailLevels - 1 ) ) );
					_autoTexPass2[loop] = new EchoPFXRenderTexture ( (int)( (float)screenWidth * per), (int)( (float)screenHeight * per ), 0, rtFilterMode[0] );
				}
			}
			
			_autoTex = new EchoPFXRenderTexture[2][];
			_autoTex[0] = _autoTexPass1;
			_autoTex[1] = _autoTexPass2;
			break;
			
		case ECHORTADJUST.DIVIDE:
			screenWidth		= Screen.width / rtAdjustWidth;
			screenHeight 	= Screen.height / rtAdjustHeight;
			break;

		case ECHORTADJUST.CUSTOM:
			screenWidth		= rtAdjustWidth;
			screenHeight 	= rtAdjustHeight;
			break;

		default:
		case ECHORTADJUST.DEVICE_SIZE:
			screenWidth		= Screen.width;
			screenHeight 	= Screen.height;
			break;
		}

		if ( screenWidth < 4 )
			screenWidth = 4;
		
		if ( screenHeight < 4 )
			screenHeight = 4;

		if ( rtAdjustSize == ECHORTADJUST.AUTO_DETAIL )
		{
			_renTex[0] = _autoTexPass1[0];
	
			if ( passCount > 1 )
				_renTex[1] = _autoTexPass2[0];
		}
		else
		{
			_renTex[0] = new EchoPFXRenderTexture ( screenWidth, screenHeight, 16, rtFilterMode[0] );
			if ( passCount > 1 )
				_renTex[1] = new EchoPFXRenderTexture ( screenWidth, screenHeight, 0, rtFilterMode[1] );
		}


		epeEchoList = new EchoList<EchoPFXEffect>(epeList);

		for ( loop = 0; loop < epeList.Count; loop++ )
		{
			epe = epeList[loop];

			if ( epe.active )
			{
				epe.Start ();
				epeEchoList.Activate ( epe );
			}
		}

		_renTex[0].SetCamerasRenderTarget ( _camList );

		if ( !EchoPFXManager.unityPro )
		{
			if ( _camList.Count > 0 )
			{
				EchoPFXUnityFreeCamCopy uscript;
				uscript = _camList[0].gameObject.AddComponent<EchoPFXUnityFreeCamCopy>();
				uscript.echoRenTex = _renTex[0];

				EchoPFXUnityFreeCamSetRect rscript;
				rscript = _camList[_camList.Count-1].gameObject.AddComponent<EchoPFXUnityFreeCamSetRect>();
				rscript.echoRenTex = _renTex[0];
			}
		}
	}

	//=========================================================================
	public void ForceShaderPropertyUpdate()
	{
		for ( int loop = 0; loop < passCount; loop++ )
		{
			_echoRenderMaterial[loop].ForceUpdate();
		}
	}
	
	//=========================================================================
	public void SetAutoDetail()
	{
		if ( rtAdjustSize != ECHORTADJUST.AUTO_DETAIL || _autoDetailChange < 1 )
			return;

		if ( rtAdjustSize == ECHORTADJUST.AUTO_DETAIL )
		{
			_renTex[0] = _autoTexPass1[_autoDetailFrame];
			_renTex[0].DiscardContents();
			
			if ( passCount > 1 )
			{
				_renTex[1] = _autoTexPass2[_autoDetailFrame];
				_renTex[1].DiscardContents();
			}
		}
		
		_renTex[0].SetCamerasRenderTarget ( _camList );
	}

	//=========================================================================
	public void RemoveOptionOfType ( ECHOPFXOPTION iopt, int ipass )
	{
		for ( int loop = 0; loop < epeList.Count; loop++ )
		{
			epeList[loop].RemoveOptionOfType ( iopt, ipass );
		}
	}

	//=========================================================================
	public void ValidateOptions()
	{
		int loop;
		
		for ( loop = 0; loop < epeList.Count; loop++ )
		{
			epeList[loop].ValidateOptions ( this );
		}
	}
	
	//=========================================================================
	void FindCameras ( Camera[] icameras )
	{
		Camera c;
		
		_camList = new List<Camera>();
		
		for ( int loop = 0; loop < icameras.Length; loop++ )
		{
			c = icameras[loop];

			if ( c.depth >= cameraDepthStart && c.depth <= cameraDepthEnd )
			{
				_camList.Add ( c );
			}
		}

		_camList.Sort ( delegate ( Camera e1, Camera e2 ) 
		{
			return ( e2.depth.CompareTo ( e1.depth ) );
		});


	}
	
	//============================================================
	public void ProcessInUpdate()
	{
		if ( _autoDetailChange > 0)
		{
			_autoDetailChange--;
			return;
		}

		int delta =  _autoDetailTargetTime - ( (int)( Time.smoothDeltaTime * 10000.0f ) );
		
		if ( delta < -2 )
		{
			_autoDetailTime	= autoDetailChangeTime;
			if ( _autoDetailFrame < autoDetailLevels-1 )
			{
				_autoDetailFrame++;
				_autoDetailChange+=2;
			}
		}
		else
		{
			if ( _autoDetailTime > 0.0f )
			{
				_autoDetailTime-=Time.deltaTime;
			}
			else
			{
				if ( _autoDetailFrame > 0 && delta > 2 )
				{
					_autoDetailTime = autoDetailChangeTime;
					_autoDetailFrame--;
					_autoDetailChange+=2;
				}
			}
		}
	}

	//============================================================
	private void CreateMesh()
	{
		int 		xLoopCount;
		int 		yLoopCount;
		int 		numTri;
		int 		numVert;
		float 		umul;
		float 		vmul;
		Vector2[] 	uv;
		Vector3[] 	verts;
		Color32[]  	vcolors;
		int[] 		tris;
		int 		loopx;
		int 		loopy;
		int 		index;

		if ( meshCellWidth < 1 )
			meshCellWidth = 1;
		
		if ( meshCellWidth > 128 )
			meshCellWidth = 128;

		if ( meshCellHeight < 1 )
			meshCellHeight = 1;
		
		if ( meshCellHeight > 128 )
			meshCellHeight = 128;

		xLoopCount = meshCellWidth + 1;
		yLoopCount = meshCellHeight + 1;
		
		numVert = xLoopCount * yLoopCount;
		numTri  = meshCellWidth * meshCellHeight * 6;
		
		umul = 1.0f / meshCellWidth;
		vmul = 1.0f / meshCellHeight;
		
		uv 		= new Vector2[numVert];
		verts 	= new Vector3[numVert];
		vcolors = new Color32[numVert];
		tris 	= new int[numTri];
		
		index = 0;
		for ( loopy = 0; loopy < yLoopCount; loopy++ )
		{
			for ( loopx = 0; loopx < xLoopCount; loopx++ )
			{
				verts[index] 	= new Vector3 ( loopx * umul, loopy * vmul, 0.0f );
				uv[index] 		= new Vector2 ( (float)loopx * umul, (float)loopy * vmul );
				
				index++;
			}
		}

		index = 0;
		for ( loopy = 0; loopy < meshCellHeight; loopy++ )
		{
			for ( loopx = 0; loopx < meshCellWidth; loopx++ )
			{
				tris[index] = loopx + ( loopy * xLoopCount );
				index++;
				
				tris[index] = loopx + ( ( loopy + 1 ) * xLoopCount );
				index++;

				tris[index] = loopx + 1 + ( loopy * xLoopCount );
				index++;
				
				tris[index] = loopx + ( ( loopy + 1 ) * xLoopCount );
				index++;
				
				tris[index] = loopx + 1 + ( ( loopy + 1 ) * xLoopCount );
				index++;

				tris[index] = loopx + 1 + ( loopy * xLoopCount );
				index++;
			}
		}
		
		_mesh = new Mesh();
		
		_mesh.vertices 	= verts;
        _mesh.triangles = tris;
		_mesh.colors32 	= vcolors;
        _mesh.uv = uv;
        _mesh.RecalculateNormals();
		;
		_mesh.MarkDynamic();

		//_meshColor = _mesh.colors32;

		return;
	}


}




