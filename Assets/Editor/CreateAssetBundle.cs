using UnityEngine;
using UnityEditor;

public class CreateAssetBundle
{
    [MenuItem("Tools/Build AssetBundlers/Windows", false, 2)]
    static void BuildAssetBundleWindows()
    {
        BuildPipeline.BuildAssetBundles("AssetBundles/Windows", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }

    [MenuItem("Tools/Build AssetBundlers/MacOS", false, 2)]
    static void BuildAssetBundleMacOS()
    {
#if UNITY_2017_3_OR_NEWER
        BuildPipeline.BuildAssetBundles("AssetBundles/MacOS", BuildAssetBundleOptions.None, BuildTarget.StandaloneOSX);
#else
        BuildPipeline.BuildAssetBundles("AssetBundles/MacOS", BuildAssetBundleOptions.None, BuildTarget.StandaloneOSXUniversal);
#endif
    }

    [MenuItem("Tools/Build AssetBundlers/UWP", false, 2)]
    static void BuildAssetBundleUWP()
    {
        BuildPipeline.BuildAssetBundles("AssetBundles/UWP", BuildAssetBundleOptions.None, BuildTarget.WSAPlayer);
    }
}
