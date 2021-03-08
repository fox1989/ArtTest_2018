// (c) copyright echoLogin LLC 2013. All rights reserved.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

//---------------------------------------------------------------
[System.Serializable]
public class EchoPFXManager : MonoBehaviour 
{
	public static Camera                    manCam              = null;
	public static bool 						unityPro 			= false;
	public static int 						tick 				= 0; 
	public static bool 						initFlag			= false;
	public static float                     globalFPS           = 60;
	public List<EchoPFXRenderGroup> 		ergList 			= new List<EchoPFXRenderGroup>(4);
	public float                      		managerCameraDepth  = 10;
	public int                              frameRate           = 60;
	private Transform                   	_camTransform;

	//============================================================
	public void Awake()
	{
		EchoPFX.ergList = ergList;

		for ( int loop = 0; loop < ergList.Count; loop++ )
		{
			ergList[loop].ForceShaderPropertyUpdate();
		}
	}

	//============================================================
	public void OnLevelWasLoaded ( int ilevel )
	{
		for ( int loop = 0; loop < ergList.Count; loop++ )
		{
			ergList[loop].ForceShaderPropertyUpdate();
		}
	}

	//============================================================
	public int GetRenderGroupID ( string iname )
	{
		for ( int loop = 0; loop < ergList.Count; loop++ )
		{
			if ( ergList[loop].name == iname )
				return ( loop );
		}
		
		return ( -1 );
	}

	//============================================================
	public int GetEffectID ( string iname, int iergindex )
	{
		for ( int index = 0; index < ergList[iergindex].epeList.Count; index++ )
		{
			if ( ergList[iergindex].epeList[iergindex].name == iname )
				return ( index );
		}

		return ( -1 );
	}

	//============================================================
	public void ShockWaveOptions ( int iergindex, int iepeindex, int ipass, float icenterx, float icentery )
	{
		//EchoPFXOption epo;
		//SCOTTFIND
		//epo = ergList[iergindex].epeList[iepeindex].passOpts[ipass][(int)ECHOPFXOPTION.SHOCKWAVE];
		//epo.shockCenterX = icenterx;
		//epo.shockCenterY = icentery;
	}

	//============================================================
	public void StartEffect ( int ierg_index, int ieffect_index )
	{
		ergList[ierg_index].epeList[ieffect_index].Start();
	}
	
	//============================================================
	public void UpdateAllRenderGroups()
	{
		for ( int loop = 0; loop < ergList.Count; loop++ )
		{
			ergList[loop].ValidateOptions();
		}
	}

	// Move this to render group class
	//============================================================
	/*
	public bool AddFXObject ( int iergindex, int ix1, int iy1, int ix2, int iy2, float iduration )
	{
		EchoPFXRenderGroup erg;
		EchoPFXAreaEffect efo;

		erg = ergList[iergindex];
		
		if ( ix1 >= erg.meshCellWidth || ix2 < 0 )
			return ( false );

		if ( iy1 >= erg.meshCellHeight || iy2 < 0 )
			return ( false );
		
		if ( ix1 < 0 )
			ix1 = 0;
		
		if ( iy1 < 0 )
	//		iy1 = 0;

		if ( ix2 > erg.meshCellWidth  )
			ix2 = erg.meshCellWidth;

		if ( iy2 > erg.meshCellHeight  )
			iy2 = erg.meshCellHeight;
		
		efo = EchoPFXAreaEffect.GetFree();
		if ( efo == null )
			return(false);
		
		efo.duration = iduration;

		efo.durType = 0;

		if ( iduration < 0 )
			efo.durType = 1;
		else if ( iduration > 0 )
			efo.durType = 2;
		
		efo.x1 = ix1;
		efo.y1 = iy1;
		efo.x2 = ix2;
		efo.y2 = iy2;
		
		return ( true );
	}
	//============================================================
	public bool AddFXObject ( int iergindex, float ix1, float iy1, float ix2, float iy2, float iduration )
	{
		EchoPFXRenderGroup erg;
		int x1,y1,x2,y2;

		erg = ergList[iergindex];
		
		x1 = (int)( ix1 * (float)(erg.meshCellWidth + 1 ) );
		y1 = (int)( iy1 * (float)(erg.meshCellHeight + 1 ) );
		x2 = (int)( ix2 * (float)(erg.meshCellWidth + 1 ) );
		y2 = (int)( iy2 * (float)(erg.meshCellHeight + 1 ) );
		
		return ( AddFXObject ( iergindex, x1, y1, x2, y2, iduration ) );
	}
	
		//============================================================
	void ProcessAreaEffects ( EchoPFXRenderGroup ierg )
	{
		EchoPFXAreaEffect efo, next;
		int loopx, loopy;
		int meshwidth = ierg.meshCellWidth+1;
		
		Array.Clear ( ierg.meshColor, 0, ierg.meshColor.Length );
		
//		Debug.Log("Process Area Effects ="+ierg.area1ObjList.Count);
		
		for ( efo = EchoPFXAreaEffect.GetFirst(); efo != null; efo = next )
		{
			next = EchoPFXAreaEffect.GetNext ( efo );
			
			for ( loopy = efo.y1; loopy < efo.y2; loopy++ )
			{
				for ( loopx = efo.x1; loopx < efo.x2; loopx++ )
				{
					ierg.meshColor[loopx + ( loopy * ( meshwidth ) ) ].r = 255;
				}
			}
			
			switch ( efo.durType )
			{
			case 1:
				EchoPFXAreaEffect.Active2Inactive ( efo );
				break;
				
			case 2:
				efo.duration -= Time.deltaTime;
				if ( efo.duration <= 0 )
				{
					EchoPFXAreaEffect.Active2Inactive ( efo );
				}
				break;
				
			default:
				break;
			}
		}
		
		ierg.mesh.colors32 = ierg.meshColor;		
	}

	
	*/
	

	//============================================================
	public void SortRenderGroups()
	{
		ergList.Sort ( delegate ( EchoPFXRenderGroup e1, EchoPFXRenderGroup e2 ) 
		{
			return ( e1.cameraDepthStart.CompareTo ( e2.cameraDepthStart ) );
		});
	}

	//============================================================
	void Start()
	{
		Camera[] cams;
		Camera c;

		EchoPFX.ergList = ergList;

		globalFPS = frameRate;

		if ( SystemInfo.supportsImageEffects && SystemInfo.supportsRenderTextures )
			unityPro = true;

		Application.targetFrameRate = frameRate;

		cams = Camera.allCameras;

		// setup this camera
		c = gameObject.AddComponent <Camera>() as Camera;
		if ( c == null )
		{
			Debug.LogError("NO CAMERA ON ECHOPOSTFXMANAGER");
			return;
		}

		c.backgroundColor 		= new Color ( 0, 0, 0, 0 );
		c.clearFlags 			= CameraClearFlags.Nothing;
		c.orthographic 			= true;
		c.orthographicSize 		= 1;
		c.nearClipPlane     	= 0.1f;
		c.farClipPlane      	= 100;
		c.cullingMask       	= 0;
		c.allowHDR 					= false;
		c.useOcclusionCulling 	= false;
		c.hideFlags            	= HideFlags.HideInInspector | HideFlags.HideInHierarchy | HideFlags.NotEditable;
		//c.hideFlags             = HideFlags.None;
		c.depth                	= managerCameraDepth;
		c.rect                  = new Rect(0,0,1,1);

		manCam = c;
		
		_camTransform = GetComponent<Camera>().transform;
		
		//EchoPFXAreaEffect.Init ( 32 );
		
		// sort to make sure they are processed in proper order
		SortRenderGroups();

		for ( int loop = 0; loop < ergList.Count; loop++ )
		{
			ergList[loop].PrepareForRuntime ( cams, frameRate );
		}

		return;
		
	}
	
	//============================================================
	void OnPostRender()
	{
		int index;
		for ( index = 0; index < ergList.Count; index++ )
		{
			ergList[index].Render(_camTransform);
		}
		
		for ( index = 0; index < ergList.Count; index++ )
		{
			ergList[index].SetAutoDetail();
		}

			// mesh stuff  ( put a check here to test if mesh effects are even being used )
//			ProcessAreaEffects ( erg );
			
	}

	//============================================================
	void Update()
	{
		for ( int loop = 0; loop < ergList.Count; loop++ )
		{
			ergList[loop].ProcessInUpdate();
		}
	}
}
