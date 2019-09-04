using UnityEngine;
using UnityEditor;

public class CreateAssetBundle
{
    private static string bundleSavePath = "Assets/StreamingAssets/AssetBundles/";

    [MenuItem("Tools/Build AssetBundlers/Standalone", false, 2)]
    static void BuildAssetBundleStandalone()
    {
#if UNITY_EDITOR_WIN
        OnBuildAssetBundle(bundleSavePath + "Standalone", BuildTarget.StandaloneWindows);
#elif UNITY_EDITOR_OSX
        OnBuildAssetBundle(bundleSavePath + "Standalone", BuildTarget.StandaloneOSX);
#elif UNITY_EDITOR_LINUX
        OnBuildAssetBundle(bundleSavePath + "Standalone", BuildTarget.StandaloneLinux64);
#endif
    }

    [MenuItem("Tools/Build AssetBundlers/UWP", false, 2)]
    static void BuildAssetBundleUWP()
    {
        OnBuildAssetBundle(bundleSavePath + "UWP", BuildTarget.WSAPlayer);
    }

    private static void OnBuildAssetBundle(string path, BuildTarget target)
    {
        BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.ForceRebuildAssetBundle, target);
    }
}
