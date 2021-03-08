// (c) copyright echoLogin LLC 2013. All rights reserved.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EchoPFXRenderTexture  
{
	private Rect     		_scnRect 	= new Rect ( 0,0,0,0 );
	private Texture2D 		_tex2d 		= null;
	private RenderTexture 	_renTex 	= null;
	public 	Texture         texture 	= null;
	private float 			_xper; 
	private float 			_yper;

	//=========================================================================
	public EchoPFXRenderTexture (  int iwidth, int iheight, int idepth, FilterMode ifiltermode  )
	{

		if ( EchoPFXManager.unityPro )
		{
			_renTex 			= new RenderTexture ( iwidth, iheight, idepth );
			_renTex.useMipMap	= false;
			_renTex.anisoLevel 	= 0;
			_renTex.wrapMode	= TextureWrapMode.Clamp;
			_renTex.filterMode	= ifiltermode;
			_renTex.Create();

			texture = _renTex;
		}
		else
		{
			_xper = (float)iwidth / (float)Screen.width; 
			_yper = (float)iheight / (float)Screen.height;

			_scnRect = new Rect( 0f, 0f, Screen.width * _xper, Screen.height * _yper );

			float xdiv = Screen.width / ( Screen.width * _xper );
			float ydiv = Screen.height / ( Screen.height * _yper );

			_xper /= xdiv;
			_yper /= ydiv;

			_tex2d 				= new Texture2D ( iwidth, iheight, TextureFormat.RGB24, false );

			if ( _tex2d == null )
				_tex2d 			= new Texture2D ( iwidth, iheight, TextureFormat.ARGB32, false );

			_tex2d.anisoLevel 	= 0;
			_tex2d.wrapMode 	= TextureWrapMode.Clamp;
			_tex2d.filterMode 	= ifiltermode;
			_tex2d.Apply();

			texture = _tex2d;
		}
	}

	//=========================================================================
	public Rect GetRect()
	{
		return new Rect ( 0,0, _xper, _yper );
	}

	//=========================================================================
	public void ReadPixels ( int ipass )
	{
		_tex2d.ReadPixels ( _scnRect, 0, 0, false );
		_tex2d.Apply();
	}

	//=========================================================================
	public static void Active ( EchoPFXRenderTexture[] iert, int ipass, int ipasscount )
	{
		if ( EchoPFXManager.unityPro )
		{
			if ( ipasscount > 1 )
			{
				if ( ipass == 0 )
					RenderTexture.active = iert[1]._renTex;
				else
					RenderTexture.active = null;
			}
			else
				RenderTexture.active = null;
		}
		else
		{
			if ( ipass == 1 )
				iert[ipass].ReadPixels ( ipass );
		}
	}

	//=========================================================================
	public void DiscardContents()
	{
		if ( EchoPFXManager.unityPro )
			_renTex.DiscardContents();
	}

	//=========================================================================
	public void SetCamerasRenderTarget ( List<Camera> icams )
	{
		if ( EchoPFXManager.unityPro )
		{
			for ( int loop = 0; loop < icams.Count; loop++ )
			{
				icams[loop].targetTexture = _renTex;
			}
		}
		else
		{
			for ( int loop = 0; loop < icams.Count; loop++ )
			{
				icams[loop].rect = new Rect ( 0,0, _xper, _yper );
			}
		}
	}
}
