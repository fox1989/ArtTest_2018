using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEditor;

public class EchoPFXShaderBuild 
{
	private static string[] _template;
	private static string _greyscale;
	private static string _inverse;
	private static string _color;
	private static string _add;
	private static string _multiply;
	private static string _noise;
	private static string _scanlineh;
	private static string _scanlinev;
	private static string _ramp;
	private static string _color_correct;
	private static string _overlay_lerp;
	private static string _overlay_add;
	private static string _overlay_subtract;
	private static string _overlay_screen;
	private static string _overlay_multiply;
	private static string _overlay_overlay;
	private static string _final;
	
		//=========================================================================
	public static readonly string[] rtBlendModes = new string[]
	{
		"Blend Off",
		"Blend OneMinusDstColor One",
		"Blend One One",
		"BlendOp Sub",
		"Blend DstColor Zero"
	};

	//=========================================================================
	public static string[] LoadTextFileSplit ( string ifname )
	{
 		StreamReader sr 	= new StreamReader( Application.dataPath + "/echoLogin/PFX/Templates/" + ifname );
    	String fileContents = sr.ReadToEnd();
    	sr.Close();
 
    	return ( fileContents.Split("\n"[0]) );
    }

	//=========================================================================
	public static string LoadTextFile ( string ifname )
	{
 		StreamReader sr 	= new StreamReader( Application.dataPath + "/echoLogin/PFX/Templates/" + ifname );
    	String fileContents = sr.ReadToEnd();
    	sr.Close();
 
    	return ( fileContents );
    }
	
	//=========================================================================
	public static void SaveTextFile ( string ifname, string idata )
	{
 		StreamWriter sw 	= new StreamWriter ( Application.dataPath + "/echoLogin/PFX/Resources/" + ifname );
		
		sw.Write ( idata ); 
		
    	sw.Close();

    	return;
    }

	//=========================================================================
	public static string ParseCustomCode ( ECHOPFXOPTION ipo, string icode )
	{
		int num = (int)ipo - (int)ECHOPFXOPTION.CUSTOM_FRAG_1 + 1;

		if ( icode == null || icode =="" )
			return("//NOCODE");

		icode = icode.Replace ( "_RGB", "_ioRGB" );
		icode = icode.Replace ( "_Texture", "_echoCustomF" + num.ToString() + "Tex" );
		icode = icode.Replace ( "_Texture_ST", "_echoCustomF" + num.ToString() + "Tex_ST" );
		icode = icode.Replace ( "_Fade", "_echoCustomF" + num.ToString() + "Fade" );
		icode = icode.Replace ( "_Args", "_echoCustomF" + num.ToString() + "Args" );
		icode = icode.Replace ( "_Color", "_echoCustomF" + num.ToString() + "Color" );
		icode = icode.Replace ( "_TC", "ECHODEF_CUSTOM" + num.ToString() + "_TC" );

		return ( icode );
	}

	//=========================================================================
	public static void WriteFragOption ( PossibleOpts ipo, EchoPFXRenderGroup ierg, int ipass )
	{
		switch ( ipo.type )
		{
		case ECHOPFXOPTION.GREYSCALE:
			_final += _greyscale;
			break;

		case ECHOPFXOPTION.INVERSE:
			_final += _inverse;
			break;

		case ECHOPFXOPTION.COLOR:
			_final += _color;
			break;

		case ECHOPFXOPTION.ADD:
			_final += _add;
			break;
			
		case ECHOPFXOPTION.MULTIPLY:
			_final += _multiply;
			break;
			
		case ECHOPFXOPTION.NOISE:
			_final += _noise;
			break;

		case ECHOPFXOPTION.DISTORTION:
			break;

		case ECHOPFXOPTION.SHOCKWAVE:
			break;

		case ECHOPFXOPTION.SCANLINES:
			if ( ipo.scanlines == ECHOPFXSCANLINES.HORIZONTAL || ipo.scanlines == ECHOPFXSCANLINES.BOTH )
				_final += _scanlineh;
			_final += "\n";
			if ( ipo.scanlines == ECHOPFXSCANLINES.VERTICAL || ipo.scanlines == ECHOPFXSCANLINES.BOTH )
				_final += _scanlinev;
			break;

		case ECHOPFXOPTION.LUMRAMP:
			_final += _ramp;
			break;
			
		case ECHOPFXOPTION.COLOR_CORRECT:
			_final += _color_correct;
			break;

		case ECHOPFXOPTION.OVERLAY_NORMAL:
			_final += _overlay_lerp;
			break;

		case ECHOPFXOPTION.OVERLAY_SCREEN:
			_final += _overlay_screen;
			break;
			
		case ECHOPFXOPTION.OVERLAY_ADD:
			_final += _overlay_add;
			break;

		case ECHOPFXOPTION.OVERLAY_SUBTRACT:
			_final += _overlay_subtract;
			break;

		case ECHOPFXOPTION.OVERLAY_MULTIPLY:
			_final += _overlay_multiply;
			break;

		case ECHOPFXOPTION.OVERLAY_OVERLAY:
			_final += _overlay_overlay;
			break;

		case ECHOPFXOPTION.CUSTOM_FRAG_1:
			_final += "#ifdef ECHO_PFX_CUSTOM_FRAG_1_ON\n";
			_final += ParseCustomCode ( ipo.type, ipo.customCode ) + "\n";
			_final += "#endif\n";
			break;

		case ECHOPFXOPTION.CUSTOM_FRAG_2:
			_final += "#ifdef ECHO_PFX_CUSTOM_FRAG_2_ON\n";
			_final += ipo.customCode + "\n";
			_final += "#endif\n";
			break;

		case ECHOPFXOPTION.CUSTOM_FRAG_3:
			_final += "#ifdef ECHO_PFX_CUSTOM_FRAG_3_ON\n";
			_final += ipo.customCode + "\n";
			_final += "#endif\n";
			break;
		
		case ECHOPFXOPTION.CUSTOM_FRAG_4:
			_final += "#ifdef ECHO_PFX_CUSTOM_FRAG_4_ON\n";
			_final += ipo.customCode + "\n";
			_final += "#endif\n";
			break;
		}
		
		_final += "\n";
	}

	//=========================================================================
	public static void WriteTCDefine ( PossibleOpts ipo )
	{
		if ( !ipo.useUniqueTC )
			return;

		switch ( ipo.type )
		{
		case ECHOPFXOPTION.OVERLAY_NORMAL:
			_final += "#define OVERLAY_NORMAL_TC";
			break;
			
		case ECHOPFXOPTION.OVERLAY_SCREEN:
			_final += "#define OVERLAY_SCR_TC";
			break;
			
		case ECHOPFXOPTION.OVERLAY_ADD:
			_final += "#define OVERLAY_ADD_TC";
			break;
			
		case ECHOPFXOPTION.OVERLAY_SUBTRACT:
			_final += "#define OVERLAY_SUB_TC";
			break;
			
		case ECHOPFXOPTION.OVERLAY_MULTIPLY:
			_final += "#define OVERLAY_MUL_TC";
			break;

		case ECHOPFXOPTION.OVERLAY_OVERLAY:
			_final += "#define OVERLAY_OVR_TC";
			break;

		case ECHOPFXOPTION.CUSTOM_FRAG_1:
			_final += "#define CUSTOM_FRAG_1_TC";
			break;
			
		case ECHOPFXOPTION.CUSTOM_FRAG_2:
			_final += "#define CUSTOM_FRAG_2_TC";
			break;
			
		case ECHOPFXOPTION.CUSTOM_FRAG_3:
			_final += "#define CUSTOM_FRAG_3_TC";
			break;
			
		case ECHOPFXOPTION.CUSTOM_FRAG_4:
			_final += "#define CUSTOM_FRAG_4_TC";
			break;

		default:
			break;
		}
		
		_final += "\n";
	}
	
	//=========================================================================
	public static void BuildRenderGroupShaders ( EchoPFXRenderGroup ierg )
	{
		int loop;
		int pass;
		int index;
		string line;
		List<PossibleOpts>[] po = new List<PossibleOpts>[2];
		string groupName;
		string passName;
		
		po[0] = ierg.possibleOpts1;
		po[1] = ierg.possibleOpts2;
		
		groupName = "group"+ierg.id+"_";
		
		for ( pass = 0; pass < 2; pass++ )
		{
			passName = "pass"+(pass+1);
			
			_final = "";
			
			for ( index = 0; index < _template.Length; index++ )
			{
				line = _template[index].Trim();
				
				switch ( line )
				{
				case "ECHO_SHADER_NAME:":
					_final += "Shader \"echoLogin/PFX/echologin_postfx_" + groupName + passName + "\"" + "\n";
					break;
	
				case "ECHO_BLEND_MODE:":
					_final += rtBlendModes[(int)ierg.rtBlendMode[pass]] + "\n";
					break;
							
				case "ECHO_KEYWORDS:":
					for ( loop = 0; loop < po[pass].Count; loop++ )
					{
						WriteTCDefine ( po[pass][loop] );
					}

					_final += "\n";

					for ( loop = 0; loop < po[pass].Count; loop++ )
					{
						_final += "#pragma multi_compile ";
						_final += EchoPFXRenderGroup._keywords[(int) po[pass][loop].type, 0 ] + " ";
						_final += EchoPFXRenderGroup._keywords[(int) po[pass][loop].type, 1 ] + "\n";
					}
					break;
	
				case "ECHO_FRAG_INSERT:":
					for ( loop = 0; loop < po[pass].Count; loop++ )
					{
						WriteFragOption ( po[pass][loop], ierg, pass );	
					}
					break;
	
				default:
					_final += line +"\n";
					break;
				}
			}
			
			SaveTextFile ( "echoLogin-postfx-" + groupName + passName +".shader", _final );
		}
	}
	
	//=========================================================================
	public static void BuildShaders ( List<EchoPFXRenderGroup> iergList )
	{
		_template 			= LoadTextFileSplit ( "echoLogin-postfx-template.txt" );
		_greyscale 			= LoadTextFile ( "echoLogin-postfx-frag-greyscale.cginc" );
		_inverse 			= LoadTextFile ( "echoLogin-postfx-frag-inverse.cginc" );
		_color	 			= LoadTextFile ( "echoLogin-postfx-frag-color.cginc" );
		_add 				= LoadTextFile ( "echoLogin-postfx-frag-add.cginc" );
		_multiply 			= LoadTextFile ( "echoLogin-postfx-frag-multiply.cginc" );
		_noise 				= LoadTextFile ( "echoLogin-postfx-frag-noise.cginc" );
		_scanlineh 			= LoadTextFile ( "echoLogin-postfx-frag-scanlines-horz.cginc" );
		_scanlinev 			= LoadTextFile ( "echoLogin-postfx-frag-scanlines-vert.cginc" );
		_ramp 				= LoadTextFile ( "echoLogin-postfx-frag-ramp.cginc" );
		_color_correct 		= LoadTextFile ( "echoLogin-postfx-frag-colorcorrect.cginc" );
		_overlay_lerp 		= LoadTextFile ( "echoLogin-postfx-frag-overlay-lerp.cginc" );
		_overlay_add 		= LoadTextFile ( "echoLogin-postfx-frag-overlay-add.cginc" );
		_overlay_subtract 	= LoadTextFile ( "echoLogin-postfx-frag-overlay-subtract.cginc" );
		_overlay_screen 	= LoadTextFile ( "echoLogin-postfx-frag-overlay-screen.cginc" );
		_overlay_multiply 	= LoadTextFile ( "echoLogin-postfx-frag-overlay-multiply.cginc" );
		_overlay_overlay 	= LoadTextFile ( "echoLogin-postfx-frag-overlay-overlay.cginc" );

		for ( int loop = 0; loop < iergList.Count; loop++ )
		{
			BuildRenderGroupShaders ( iergList[loop] ); 
		}
		
		Resources.UnloadUnusedAssets();
		
		AssetDatabase.Refresh();
	}
}
