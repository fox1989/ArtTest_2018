using UnityEngine;
using System.Collections;

public class EchoPFXUnityFreeCamCopy : MonoBehaviour 
{
	public EchoPFXRenderTexture echoRenTex;

	//============================================================
	void OnPostRender()
	{
		if ( echoRenTex != null )
		{
			echoRenTex.ReadPixels(0);
			GL.Clear ( true, true, new Color (0,0,0,0));
		}
	}
}
