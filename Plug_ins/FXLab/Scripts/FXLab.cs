using UnityEngine;

public static class FXLab
{
#if FXLAB_PRO
    public static bool UseNativeRenderTextures = SystemInfo.supportsRenderTextures;
    public static bool NeedsFXLabProDirective = false;
#elif UNITY_EDITOR
    public static bool UseNativeRenderTextures = false;
    public static bool NeedsFXLabProDirective = UnityEditorInternal.InternalEditorUtility.HasPro();
#else
    public static bool UseNativeRenderTextures = false;
    public static bool NeedsFXLabProDirective = false;
#endif
}