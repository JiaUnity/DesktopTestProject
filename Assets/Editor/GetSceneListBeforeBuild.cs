using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

// To get the list of "Enabled" scenes in the Build Settings before Build or Playmode starts
// Save the list to SceneList.asset in Resources folder
#if UNITY_EDITOR
[InitializeOnLoad]
[CustomEditor(typeof(SceneSelect))]
#if UNITY_2018_1_OR_NEWER
public class GetSceneListBeforeBuild : Editor, IPreprocessBuildWithReport, IPostprocessBuildWithReport
#else
public class GetSceneListBeforeBuild : Editor, IPreprocessBuild, IPostprocessBuild
#endif
{
    // Get scene list before entering Playmode
    static GetSceneListBeforeBuild()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeChange;
        EditorApplication.playModeStateChanged += OnPlayModeChange;
    }

    private static void OnPlayModeChange(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
            SetSceneListAsAsset();
        else if (state == PlayModeStateChange.ExitingPlayMode)
            SetSceneListUpdated(false);
    }

    // Get scene list before building Player
    public int callbackOrder { get { return 0; } }

#if UNITY_2018_1_OR_NEWER
    public void OnPreprocessBuild(UnityEditor.Build.Reporting.BuildReport report)
    {
        SetSceneListAsAsset();
    }

    public void OnPostprocessBuild(UnityEditor.Build.Reporting.BuildReport report)
    {
        SetSceneListUpdated(false);
    }

#else
    public void OnPreprocessBuild(BuildTarget target, string path)
    {
        SetSceneListAsAsset();
    }

    public void OnPostprocessBuild(BuildTarget target, string path)
    {
        SetSceneListUpdated(false);
    }

#endif

    private static void SetSceneListAsAsset()
    {
        List<string> sceneNames = new List<string>();
        foreach (EditorBuildSettingsScene buildScene in EditorBuildSettings.scenes)
        {
            if (buildScene.enabled)
            {
                SceneAsset sa = AssetDatabase.LoadAssetAtPath<SceneAsset>(buildScene.path);
                sceneNames.Add(sa.name);
            }
        }

        SceneList list = GetSceneList();
        list.sceneNames = sceneNames.ToArray();
        list.isUpdated = true;
        EditorUtility.SetDirty(list);
        AssetDatabase.SaveAssets();
    }

    private static void SetSceneListUpdated(bool isUp)
    {
        SceneList list = GetSceneList();
        if (list.isUpdated != isUp)
        {
            list.isUpdated = isUp;
            EditorUtility.SetDirty(list);
            AssetDatabase.SaveAssets();
        }
    }

    private static SceneList GetSceneList()
    {
        SceneList list = AssetDatabase.LoadAssetAtPath<SceneList>("Assets/Resources/SceneList.asset");
        if (list == null)
        {
            list = ScriptableObject.CreateInstance<SceneList>();
            AssetDatabase.CreateAsset(list, "Assets/Resources/SceneList.asset");
        }
        return list;
    }
}
#endif
