using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("FXLab/FX Post Process")]
public class FXPostProcess : MonoBehaviour
{
    private static Texture2D dummyTexture;
	
	public Material Material;
    public List<FXTextureAssigner.RenderTextureAssignment> Assignments = new List<FXTextureAssigner.RenderTextureAssignment>();

#if FXLAB_PRO
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (!FXLab.UseNativeRenderTextures || !TryBeginRender())
        {
            Graphics.Blit(src, dest);
            return;
        }
        //Graphics.Blit(src, dest);
        GL.PushMatrix();
        GL.LoadPixelMatrix(0, src.width, src.height, 0);
        var area = new Rect(0, 0, src.width, src.height);
        Graphics.DrawTexture(area, dummyTexture, Material);
        GL.PopMatrix();
        Graphics.Blit(src, dest);

        //Graphics.Blit(src, dest, Material);
    }
#endif

    void OnPostRender()
    {
        if (FXLab.UseNativeRenderTextures || !TryBeginRender())
            return;

        GL.PushMatrix();
        GL.LoadPixelMatrix(0, GetComponent<Camera>().pixelRect.width, GetComponent<Camera>().pixelRect.height, 0);
        var area = GetComponent<Camera>().pixelRect;
        Graphics.DrawTexture(area, dummyTexture, Material);
        GL.PopMatrix();
    }


    private bool TryBeginRender()
    {
        if (!enabled || Material == null)
            return false;

        if (!dummyTexture)
        {
            dummyTexture = new Texture2D(1, 1);
            dummyTexture.hideFlags = HideFlags.HideAndDontSave;
        }

        Shader.SetGlobalVector("_CameraViewDirection", GetComponent<Camera>().transform.forward);
        Shader.SetGlobalVector("_CameraRotation", GetComponent<Camera>().transform.eulerAngles);

        foreach (var assignment in Assignments)
            assignment.Apply(gameObject);

        return true;
    }

    void Start()
    {
		// to display the enable checkbox, dont remove this
    }
}