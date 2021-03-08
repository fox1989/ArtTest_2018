using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;

namespace UnityEditor
{

	public class EchoPFXLoadSave
	{
		static XmlDocument xfile;
		static XmlElement 	root;
		private static int _curID = 0;
		private static string  myPath = "";
		
		//============================================================
		public static void SaveOption ( EchoPFXOption iepo, XmlElement iparent )
		{
			XmlElement 		ele;

			ele = xfile.CreateElement("Option");
			iparent.AppendChild ( ele );
			iparent = ele;

			ele = xfile.CreateElement("optType");
			ele.InnerText = iepo.optType.ToString();
			iparent.AppendChild ( ele );

			ele = xfile.CreateElement("passOrder");
			ele.InnerText = iepo.passOrder.ToString();
			iparent.AppendChild ( ele );

			ele = xfile.CreateElement("startDelay");
			ele.InnerText = iepo.startDelay.ToString();
			iparent.AppendChild ( ele );

			ele = xfile.CreateElement("attackTime");
			ele.InnerText = iepo.attackTime.ToString();
			iparent.AppendChild ( ele );

			ele = xfile.CreateElement("sustainTime");
			ele.InnerText = iepo.sustainTime.ToString();
			iparent.AppendChild ( ele );

			ele = xfile.CreateElement("releaseTime");
			ele.InnerText = iepo.releaseTime.ToString();
			iparent.AppendChild ( ele );

			ele = xfile.CreateElement("fadeMin");
			ele.InnerText = iepo.fadeMin.ToString();
			iparent.AppendChild ( ele );

			ele = xfile.CreateElement("fadeMax");
			ele.InnerText = iepo.fadeMax.ToString();
			iparent.AppendChild ( ele );

			ele = xfile.CreateElement("fadeCur");
			ele.InnerText = iepo.fadeCur.ToString();
			iparent.AppendChild ( ele );
		
			ele = xfile.CreateElement("rgba");
			ele.InnerText = ((Vector4)iepo.rgba).ToString();
			iparent.AppendChild ( ele );

			ele = xfile.CreateElement("rgbaMultiply");
			ele.InnerText = iepo.rgbaMultiply.ToString();
			iparent.AppendChild ( ele );
		
			ele = xfile.CreateElement("distAmountH");
			ele.InnerText = iepo.distAmountH.ToString();
			iparent.AppendChild ( ele );
		
			ele = xfile.CreateElement("distSpeedH");
			ele.InnerText = iepo.distSpeedH.ToString();
			iparent.AppendChild ( ele );
		
			ele = xfile.CreateElement("distStrengthH");
			ele.InnerText = iepo.distStrengthH.ToString();
			iparent.AppendChild ( ele );

			ele = xfile.CreateElement("distAmountV");
			ele.InnerText = iepo.distAmountV.ToString();
			iparent.AppendChild ( ele );
			
			ele = xfile.CreateElement("distSpeedV");
			ele.InnerText = iepo.distSpeedV.ToString();
			iparent.AppendChild ( ele );
			
			ele = xfile.CreateElement("distStrengthV");
			ele.InnerText = iepo.distStrengthV.ToString();
			iparent.AppendChild ( ele );

			ele = xfile.CreateElement("linesAmountDivideH");
			ele.InnerText = iepo.linesAmountDivideH.ToString();
			iparent.AppendChild ( ele );

			ele = xfile.CreateElement("linesAmountH");
			ele.InnerText = iepo.linesAmountH.ToString();
			iparent.AppendChild ( ele );

			ele = xfile.CreateElement("linesScrollH");
			ele.InnerText = iepo.linesScrollH.ToString();
			iparent.AppendChild ( ele );

			ele = xfile.CreateElement("linesAmountDivideV");
			ele.InnerText = iepo.linesAmountDivideV.ToString();
			iparent.AppendChild ( ele );
			
			ele = xfile.CreateElement("linesAmountV");
			ele.InnerText = iepo.linesAmountV.ToString();
			iparent.AppendChild ( ele );
			
			ele = xfile.CreateElement("linesScrollV");
			ele.InnerText = iepo.linesScrollV.ToString();
			iparent.AppendChild ( ele );

			ele = xfile.CreateElement("shockDistance");
			ele.InnerText = iepo.shockDistance.ToString();
			iparent.AppendChild ( ele );
			
			ele = xfile.CreateElement("shockSize");
			ele.InnerText = iepo.shockSize.ToString();
			iparent.AppendChild ( ele );
			
			ele = xfile.CreateElement("shockCenterX");
			ele.InnerText = iepo.shockCenterX.ToString();
			iparent.AppendChild ( ele );
			
			ele = xfile.CreateElement("shockCenterY");
			ele.InnerText = iepo.shockCenterY.ToString();
			iparent.AppendChild ( ele );
			
			ele = xfile.CreateElement("tex");
			ele.InnerText = AssetDatabase.GetAssetPath ( iepo.tex );
			iparent.AppendChild ( ele );
			
			ele = xfile.CreateElement("texBlend");
			ele.InnerText = iepo.texBlend.ToString();
			iparent.AppendChild ( ele );

			ele = xfile.CreateElement("customArgs");
			ele.InnerText = iepo.customArgs.ToString();
			iparent.AppendChild ( ele );

			ele = xfile.CreateElement("overlayST");
			ele.InnerText = iepo.overlayST.ToString();
			iparent.AppendChild ( ele );

			ele = xfile.CreateElement("overlayST_Scroll");
			ele.InnerText = iepo.overlayST_Scroll.ToString();
			iparent.AppendChild ( ele );
		}

		//============================================================
		public static void SaveEffect ( EchoPFXEffect iepe, XmlElement iparent )
		{
			XmlElement 		eleParent;
			XmlElement 		eleOpts;
			XmlElement 		ele;
			int 			loop;

			eleParent = xfile.CreateElement("Effect");
			iparent.AppendChild ( eleParent );

			ele = xfile.CreateElement("name");
			ele.InnerText = iepe.name;
			eleParent.AppendChild ( ele );

			ele = xfile.CreateElement("active");
			ele.InnerText = iepe.active.ToString();
			eleParent.AppendChild ( ele );

			// pass 1 option defaults
			eleOpts = xfile.CreateElement("OptionsPass1");
			eleParent.AppendChild ( eleOpts );

			// save pass 1 Options
			for ( loop = 0; loop < iepe.passOpt1.Count; loop++)
			{
				SaveOption ( iepe.passOpt1[loop], eleOpts );
			}

			// pass 2 option defaults
			eleOpts = xfile.CreateElement("OptionsPass2");
			eleParent.AppendChild ( eleOpts );
			
			// save pass 1 Options
			for ( loop = 0; loop < iepe.passOpt2.Count; loop++)
			{
				SaveOption ( iepe.passOpt2[loop], eleOpts );
			}
		}

		//============================================================
		public static void SaveRenderGroup ( EchoPFXRenderGroup ierg, XmlElement iparent )
		{
			XmlElement 		eleParent;
			XmlElement 		ele;
			XmlElement 		eleOpts;
			XmlElement 		opar;
			PossibleOpts 	po;
			int 			loop;

			eleParent = xfile.CreateElement("RenderGroup");
			iparent.AppendChild ( eleParent );

			ele = xfile.CreateElement("name");
			ele.InnerText = ierg.name;
			eleParent.AppendChild ( ele );

			ele = xfile.CreateElement("id");
			ele.InnerText = ierg.id.ToString();
			eleParent.AppendChild ( ele );

			ele = xfile.CreateElement("passCount");
			ele.InnerText = ierg.passCount.ToString();
			eleParent.AppendChild ( ele );

			ele = xfile.CreateElement("active");
			ele.InnerText = ierg.active.ToString();
			eleParent.AppendChild ( ele );

			ele = xfile.CreateElement("cameraDepthStart");
			ele.InnerText = ierg.cameraDepthStart.ToString();
			eleParent.AppendChild ( ele );

			ele = xfile.CreateElement("cameraDepthEnd");
			ele.InnerText = ierg.cameraDepthEnd.ToString();
			eleParent.AppendChild ( ele );

			ele = xfile.CreateElement("meshCellWidth");
			ele.InnerText = ierg.meshCellWidth.ToString();
			eleParent.AppendChild ( ele );

			ele = xfile.CreateElement("meshCellHeight");
			ele.InnerText = ierg.meshCellHeight.ToString();
			eleParent.AppendChild ( ele );

			ele = xfile.CreateElement("rtAdjustSize");
			ele.InnerText = ierg.rtAdjustSize.ToString();
			eleParent.AppendChild ( ele );

			ele = xfile.CreateElement("rtAdjustWidth");
			ele.InnerText = ierg.rtAdjustWidth.ToString();
			eleParent.AppendChild ( ele );

			ele = xfile.CreateElement("rtAdjustHeight");
			ele.InnerText = ierg.rtAdjustHeight.ToString();
			eleParent.AppendChild ( ele );

			ele = xfile.CreateElement("rtFilterMode1");
			ele.InnerText = ierg.rtFilterMode[0].ToString();
			eleParent.AppendChild ( ele );

			ele = xfile.CreateElement("rtFilterMode2");
			ele.InnerText = ierg.rtFilterMode[1].ToString();
			eleParent.AppendChild ( ele );

			ele = xfile.CreateElement("rtBlendMode1");
			ele.InnerText = ierg.rtBlendMode[0].ToString();
			eleParent.AppendChild ( ele );

			ele = xfile.CreateElement("rtBlendMode2");
			ele.InnerText = ierg.rtBlendMode[1].ToString();
			eleParent.AppendChild ( ele );

			// possible options pass 1
			eleOpts = xfile.CreateElement("PossibleOpts1");
			eleParent.AppendChild ( eleOpts );

			for ( loop = 0; loop < ierg.possibleOpts1.Count; loop++ )
			{
				po = ierg.possibleOpts1[loop];

				// parent
				opar = xfile.CreateElement("Option");
				eleOpts.AppendChild ( opar );

				ele = xfile.CreateElement("type");
				ele.InnerText = po.type.ToString();
				opar.AppendChild ( ele );

				ele = xfile.CreateElement("code");
				ele.InnerText = po.customCode;
				opar.AppendChild ( ele );
			}

			// possible options pass 2
			eleOpts = xfile.CreateElement("PossibleOpts2");
			eleParent.AppendChild ( eleOpts );
			
			for ( loop = 0; loop < ierg.possibleOpts2.Count; loop++ )
			{
				po = ierg.possibleOpts2[loop];
				
				// parent
				opar = xfile.CreateElement("Option");
				eleOpts.AppendChild ( opar );
				
				ele = xfile.CreateElement("type");
				ele.InnerText = po.type.ToString();
				opar.AppendChild ( ele );
				
				ele = xfile.CreateElement("code");
				ele.InnerText = po.customCode.ToString();
				opar.AppendChild ( ele );
			}

			// save effects
			eleOpts = xfile.CreateElement("Effects");
			eleParent.AppendChild ( eleOpts );

			for ( loop = 0; loop < ierg.epeList.Count; loop++ )
			{
				SaveEffect ( ierg.epeList[loop], eleOpts );
			}

		}

		//============================================================
		public static void Save ( EchoPFXManager iepfxm )
		{
			EchoPFXRenderGroup erg;
			StreamWriter sw;
			string fileName;

			fileName = EditorUtility.SaveFilePanel ( "Save PostFX setup", myPath, "echoPOSTFXSettings", "xml" );

			if ( fileName == null || fileName == "" )
				return;

			xfile = new XmlDocument();
	
			xfile.AppendChild ( xfile.CreateXmlDeclaration ( "1.0", null, null ) );

			root = xfile.CreateElement("PostFX");
			root.SetAttribute ( "FPS", iepfxm.frameRate.ToString() );
			root.SetAttribute ( "DEPTH", iepfxm.managerCameraDepth.ToString() );
			xfile.AppendChild ( root );

			for ( int loop = 0; loop < iepfxm.ergList.Count; loop++ )
			{
				erg = iepfxm.ergList[loop];
				SaveRenderGroup ( erg, root );

			}

			sw = new StreamWriter ( fileName );
			xfile.Save ( sw );
			sw.Close();
		}

		//============================================================
		private static int NodeParseInt ( XmlNode inode, string iname )
		{
			string value = null;
			
			if ( inode[iname] != null )
				value = inode[iname].InnerText;

			if ( value == null || value == "" )
				return(0);

			return ( int.Parse ( value ) );
		}

		//============================================================
		private static float NodeParseFloat ( XmlNode inode, string iname )
		{
			string value = null;

			if ( inode[iname] != null )
				value = inode[iname].InnerText;
			
			if ( value == null || value == "" )
				return(0.0f);
			
			return ( float.Parse ( value ) );
		}

		//============================================================
		private static bool NodeParseBool ( XmlNode inode, string iname )
		{
			string value = null;
			
			if ( inode[iname] != null )
				value = inode[iname].InnerText;

			if ( value == null || value == "" )
				return ( false );
			
			return ( bool.Parse ( value ) );
		}

		//============================================================
		private static int NodeParseEnum ( XmlNode inode, string iname, System.Type ienumType, int idefault )
		{
			string value = null;
			
			if ( inode[iname] != null )
				value = inode[iname].InnerText;
			else
				Debug.Log ("ERROR");
			
			if ( value == null || value == "" )
				return ( idefault );
			
			return ( (int)System.Enum.Parse ( ienumType, value ) );
		}

		//============================================================
		private static int NodeParseEnum ( XmlNode inode, System.Type ienumType, int idefault )
		{
			string value = null;
			
			if ( inode != null )
				value = inode.InnerText;

			if ( value == null || value == "" )
				return ( idefault );
			
			return ( (int)System.Enum.Parse ( ienumType, value ) );
		}

		//============================================================
		private static Texture2D NodeParseTex2D ( XmlNode inode, string iname )
		{
			string value = null;
			Texture2D tex2D;
			
			if ( inode[iname] != null )
				value = inode[iname].InnerText;
			
			if ( value == null || value == "" )
				return ( null );

			tex2D = AssetDatabase.LoadAssetAtPath(value, typeof(Texture2D)) as Texture2D;

			return ( tex2D );
		}


		//============================================================
		private static string NodeParseString ( XmlNode inode )
		{
			string value = null;
			
			if ( inode != null )
				value = inode.InnerText;

			if ( value == null || value == "" )
				return ( "" );
			
			return ( value );
		}

		//============================================================
		public static Vector4 NodeParseVector4 (  XmlNode inode, string iname )
		{
			string[] d;
			Vector4 v4 		= new Vector4(0,0,0,0);
			string value 	= null;


			if ( inode[iname] != null )
				value = inode[iname].InnerText;
			
			if ( value == null || value == "" )
				value = "0,0,0,0";

			value = value.Replace ('(',' ' );
			value = value.Replace (')',' ' );

			d = value.Split (","[0]);

			v4.x = float.Parse( d[0] );
			v4.y = float.Parse( d[1] );
			v4.z = float.Parse( d[2] );
			v4.w = float.Parse( d[3] );
			
			return ( v4 );
		}

		//============================================================
		private static EchoPFXOption LoadOption ( XmlNode inode )
		{
			ECHOPFXOPTION opt = ( ECHOPFXOPTION )NodeParseEnum ( inode, "optType", typeof ( ECHOPFXOPTION ), (int)ECHOPFXOPTION.GREYSCALE );
			EchoPFXOption epo = new EchoPFXOption ( opt );

			epo.passOrder 		= NodeParseInt ( inode, "passOrder" );
			epo.startDelay 		= NodeParseFloat ( inode, "startDelay" );
			epo.attackTime 		= NodeParseFloat ( inode, "attackTime" );
			epo.sustainTime 	= NodeParseFloat ( inode, "sustainTime" );
			epo.releaseTime 	= NodeParseFloat ( inode, "releaseTime" );
			epo.fadeMin 		= NodeParseFloat ( inode, "fadeMin" );
			epo.fadeMax 		= NodeParseFloat ( inode, "fadeMax" );
			epo.fadeCur 		= NodeParseFloat ( inode, "fadeCur" );
			epo.rgba      		= (Color)NodeParseVector4 ( inode, "rgba" );
			epo.rgbaMultiply 	= NodeParseFloat ( inode, "rgbaMultiply" );
			epo.distAmountH 	= NodeParseFloat ( inode, "distAmountH" );
			epo.distSpeedH 		= NodeParseFloat ( inode, "distSpeedH" );
			epo.distStrengthH	= NodeParseFloat ( inode, "distStrengthH" );

			epo.distAmountV 	= NodeParseFloat ( inode, "distAmountV" );
			epo.distSpeedV 		= NodeParseFloat ( inode, "distSpeedV" );
			epo.distStrengthV	= NodeParseFloat ( inode, "distStrengthV" );

			epo.linesAmountDivideH	= NodeParseBool ( inode, "linesAmountDivideH" );
			epo.linesAmountH		= NodeParseInt ( inode, "linesAmountH" );
			epo.linesScrollH		= NodeParseFloat ( inode, "LinesScrollH" );

			epo.linesAmountDivideV	= NodeParseBool ( inode, "linesAmountDivideV" );
			epo.linesAmountV		= NodeParseInt ( inode, "linesAmountV" );
			epo.linesScrollV		= NodeParseFloat ( inode, "LinesScrollV" );

			epo.shockDistance 	= NodeParseFloat ( inode, "shockDistance" );
			epo.shockSize 		= NodeParseFloat ( inode, "shockSize" );
			epo.shockCenterX 	= NodeParseFloat ( inode, "shockCenterX" );
			epo.shockCenterY 	= NodeParseFloat ( inode, "shockCenterY" );
			epo.tex 			= NodeParseTex2D ( inode, "tex" );
			epo.texBlend        = ( ECHOPFXBLEND )NodeParseEnum ( inode, "texBlend", typeof ( ECHOPFXBLEND ), (int)ECHOPFXBLEND.NORMAL );
			epo.customArgs      = NodeParseVector4 ( inode, "customArgs" );
			epo.overlayST     	= NodeParseVector4 ( inode, "overlayST" );
			epo.overlayST_Scroll= NodeParseVector4 ( inode, "overlayST_Scroll" );

			return ( epo );
		}

		//============================================================
		private static EchoPFXEffect LoadEffect ( XmlNode inode )
		{
			EchoPFXEffect epe = new EchoPFXEffect();
			XmlNode onode;
			int loop;

			epe.name = inode["name"].InnerText;
			epe.active = NodeParseBool ( inode, "active" );

			onode = inode["OptionsPass1"];
			epe.passOpt1.Clear();

			if ( onode != null )
			{
				for ( loop = 0; loop < onode.ChildNodes.Count; loop++ )
				{
					epe.passOpt1.Add ( LoadOption(onode.ChildNodes[loop]) );
				}
			}

			onode = inode["OptionsPass2"];
			epe.passOpt2.Clear();
			
			if ( onode != null )
			{
				for ( loop = 0; loop < onode.ChildNodes.Count; loop++ )
				{
					epe.passOpt2.Add ( LoadOption(onode.ChildNodes[loop]) );
				}
			}

			return ( epe );
		}

		//============================================================
		private static EchoPFXRenderGroup LoadRenderGroup ( XmlNode inode )
		{
			EchoPFXRenderGroup erg = new EchoPFXRenderGroup(_curID++);
			XmlNode node;
			XmlNode childNode;
			int loop;
			ECHOPFXOPTION fxo;
			PossibleOpts po;

			node = inode.FirstChild;

			erg.name 				= inode["name"].InnerText;
			erg.id 					= NodeParseInt ( inode, "id" );
			erg.passCount 			= NodeParseInt ( inode, "passCount" );
			erg.active      		= NodeParseBool ( inode, "active" );
			erg.cameraDepthStart    = NodeParseFloat ( inode,"cameraDepthStart" );
			erg.cameraDepthEnd    	= NodeParseFloat ( inode,"cameraDepthEnd" );
			erg.meshCellWidth    	= NodeParseInt ( inode,"meshCellWidth" );
			erg.meshCellHeight    	= NodeParseInt ( inode,"meshCellHeight" );
			erg.rtAdjustSize    	= (ECHORTADJUST)NodeParseEnum ( inode, "rtAdjustSize", typeof ( ECHORTADJUST ), (int)ECHORTADJUST.DEVICE_SIZE );
			erg.rtAdjustWidth   	= NodeParseInt ( inode,"rtAdjustWidth" );
			erg.rtAdjustHeight    	= NodeParseInt ( inode,"rtAdjustHeight" );

			erg.rtFilterMode        = new FilterMode[2];
			erg.rtFilterMode[0]    	= ( FilterMode )NodeParseEnum ( inode, "rtFilterMode1", typeof ( FilterMode ),(int)FilterMode.Point );
			erg.rtFilterMode[1]    	= ( FilterMode )NodeParseEnum ( inode, "rtFilterMode2", typeof ( FilterMode ),(int)FilterMode.Point );

			erg.rtBlendMode      	= new ECHOPFXBLEND[2];
			erg.rtBlendMode[0]    	= ( ECHOPFXBLEND )NodeParseEnum ( inode, "rtBlendMode1", typeof ( ECHOPFXBLEND ),(int)ECHOPFXBLEND.NORMAL );
			erg.rtBlendMode[1]    	= ( ECHOPFXBLEND )NodeParseEnum ( inode, "rtBlendMode2", typeof ( ECHOPFXBLEND ),(int)ECHOPFXBLEND.NORMAL );


			node = inode["PossibleOpts1"];
			erg.possibleOpts1.Clear();
			for ( loop = 0; loop < node.ChildNodes.Count; loop ++ )
			{
				childNode = node.ChildNodes[loop];

				fxo  = ( ECHOPFXOPTION )NodeParseEnum ( childNode["type"], typeof ( ECHOPFXOPTION ),(int)ECHOPFXOPTION.COUNT );
				if ( fxo != ECHOPFXOPTION.COUNT )
				{
					po 				= new PossibleOpts( fxo, loop );
					po.customCode 	= NodeParseString ( childNode["code"] );
					erg.possibleOpts1.Add ( po );
				}
			}

			node = inode["PossibleOpts2"];
			erg.possibleOpts2.Clear();
			for ( loop = 0; loop < node.ChildNodes.Count; loop ++ )
			{
				childNode = node.ChildNodes[loop];
				
				fxo  = ( ECHOPFXOPTION )NodeParseEnum ( childNode["type"], typeof ( ECHOPFXOPTION ),(int)ECHOPFXOPTION.COUNT );
				if ( fxo != ECHOPFXOPTION.COUNT )
				{
					po 				= new PossibleOpts( fxo, loop );
					po.customCode 	= NodeParseString ( node.ChildNodes[loop]["code"] );
					erg.possibleOpts2.Add ( po );
				}
			}


			node = inode["Effects"];
			erg.epeList.Clear();
			for ( loop = 0; loop < node.ChildNodes.Count; loop ++ )
			{
				erg.epeList.Add ( LoadEffect ( node.ChildNodes[loop] ) );
			}
	
			return ( erg );
		}

		//============================================================
		public static EchoPFXRenderGroup Load ( EchoPFXManager iepfxm )
		{
			XmlNode lroot;
			string fileName;
			StreamReader sr;

			fileName = EditorUtility.OpenFilePanel ( "Open PostFX setup", myPath, "xml" );

			if ( fileName == null || fileName == "" )
				return ( null );

			sr = new StreamReader(fileName);

			xfile = new XmlDocument();
			xfile.LoadXml(sr.ReadToEnd());
			sr.Close();

			lroot = xfile["PostFX"];

			iepfxm.frameRate 			= int.Parse ( lroot.Attributes["FPS"].Value );
			iepfxm.managerCameraDepth 	= int.Parse ( lroot.Attributes["DEPTH"].Value );

			// Render Groups
			iepfxm.ergList.Clear();
			_curID = 0;

			for ( int loop = 0; loop < lroot.ChildNodes.Count; loop++ )
			{
				iepfxm.ergList.Add ( LoadRenderGroup ( lroot.ChildNodes[loop] ) );
			}

			if ( iepfxm.ergList.Count > 0 )
			{
				return ( iepfxm.ergList[0] );
			}

			return ( null );
		}
	}
}
