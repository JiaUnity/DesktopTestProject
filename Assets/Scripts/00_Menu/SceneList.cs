using UnityEngine;

/// <summary>
/// This is for the SceneList.Asset in Resources folder
/// The purpose is to dynamically generate the list of scenes selected to be built in Build Settings,
/// so the project has a menu that's alwasy match the build.
/// </summary>
public class SceneList : ScriptableObject
{
    public string[] sceneNames;
    public bool isUpdated = false;
}
