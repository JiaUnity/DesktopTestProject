using UnityEditor;

public class CreateAssetBundle
{
    private static string bundleSavePath = "Assets/StreamingAssets/AssetBundles/";

    [MenuItem("Tools/Build AssetBundlers/Windows", false, 2)]
    static void BuildAssetBundleWindows()
    {
        OnBuildAssetBundle(bundleSavePath + "Windows", BuildTarget.StandaloneWindows);
    }

    [MenuItem("Tools/Build AssetBundlers/OSX", false, 2)]
    static void BuildAssetBundleOSX()
    {
        OnBuildAssetBundle(bundleSavePath + "OSX", BuildTarget.StandaloneOSX);
    }

    [MenuItem("Tools/Build AssetBundlers/Linux", false, 2)]
    static void BuildAssetBundleLinux()
    {
        OnBuildAssetBundle(bundleSavePath + "Linux", BuildTarget.StandaloneLinux64);
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
