using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;


public class ResolutionSettings : MonoBehaviour
{
    static string scene_name = "10_Resolution & Presentation";

    //---------------------------------------------------
    //---- Menu Item Function to set Player settings ----
    //---------------------------------------------------
    //-- Preset 1: Fullscreen with native resolution and without dialog --
    [MenuItem("QA/Resolution Test/Reset")]
    static void ResetSetting()
    {
        if (!SwitchSceneIfOkay())
            return;

        PlayerSettings.fullScreenMode = FullScreenMode.Windowed;

        PlayerSettings.defaultIsNativeResolution = false;
        PlayerSettings.defaultScreenWidth = 1024;
        PlayerSettings.defaultScreenHeight = 768;
        PlayerSettings.macRetinaSupport = true;
        PlayerSettings.runInBackground = true;

        PlayerSettings.captureSingleScreen = true;
        PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.HiddenByDefault;
        PlayerSettings.resizableWindow = true;
        PlayerSettings.visibleInBackground = true;
        PlayerSettings.allowFullscreenSwitch = true;
        PlayerSettings.forceSingleInstance = false;

        PlayerSettings.SetAspectRatio(AspectRatio.Aspect4by3, true);
        PlayerSettings.SetAspectRatio(AspectRatio.Aspect5by4, true);
        PlayerSettings.SetAspectRatio(AspectRatio.Aspect16by9, true);
        PlayerSettings.SetAspectRatio(AspectRatio.Aspect16by10, true);
        PlayerSettings.SetAspectRatio(AspectRatio.AspectOthers, true);

        OpenTestScene(0);
    }

    [MenuItem("QA/Resolution Test/Preset 1")]
    static void Setting1()
    {
        if (!SwitchSceneIfOkay())
            return;

#if UNITY_EDITOR_OSX
        PlayerSettings.fullScreenMode = FullScreenMode.FullScreenWindow;
#else
        PlayerSettings.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
#endif

        PlayerSettings.defaultIsNativeResolution = true;
        PlayerSettings.macRetinaSupport = true;
        PlayerSettings.runInBackground = false;

        //PlayerSettings.captureSingleScreen = true;
        PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.Disabled;
        PlayerSettings.resizableWindow = false;
        PlayerSettings.visibleInBackground = true;
        PlayerSettings.allowFullscreenSwitch = true;
        PlayerSettings.forceSingleInstance = true;

        PlayerSettings.SetAspectRatio(AspectRatio.Aspect4by3, true);
        PlayerSettings.SetAspectRatio(AspectRatio.Aspect5by4, true);
        PlayerSettings.SetAspectRatio(AspectRatio.Aspect16by9, true);
        PlayerSettings.SetAspectRatio(AspectRatio.Aspect16by10, true);
        PlayerSettings.SetAspectRatio(AspectRatio.AspectOthers, true);

        OpenTestScene(1);
    }

    //-- Preset 2: Windowed with hidden resolution dialog --
    [MenuItem("QA/Resolution Test/Preset 2")]
    static void Setting2()
    {
        if (!SwitchSceneIfOkay())
            return;

        PlayerSettings.fullScreenMode = FullScreenMode.Windowed;

        PlayerSettings.defaultIsNativeResolution = false;
        PlayerSettings.defaultScreenWidth = 1024;
        PlayerSettings.defaultScreenHeight = 768;
        PlayerSettings.macRetinaSupport = false;
        PlayerSettings.runInBackground = false;

        PlayerSettings.captureSingleScreen = true;
        PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.HiddenByDefault;
        PlayerSettings.resizableWindow = false;
        PlayerSettings.visibleInBackground = true;
        PlayerSettings.allowFullscreenSwitch = false;
        PlayerSettings.forceSingleInstance = false;

        PlayerSettings.SetAspectRatio(AspectRatio.Aspect4by3, true);
        PlayerSettings.SetAspectRatio(AspectRatio.Aspect5by4, true);
        PlayerSettings.SetAspectRatio(AspectRatio.Aspect16by9, true);
        PlayerSettings.SetAspectRatio(AspectRatio.Aspect16by10, true);
        PlayerSettings.SetAspectRatio(AspectRatio.AspectOthers, true);

        OpenTestScene(2);
    }

    //-- Preset 3: Windowed with resolution dialog --
    [MenuItem("QA/Resolution Test/Preset 3")]
    static void Setting3()
    {
        if (!SwitchSceneIfOkay())
            return;

#if UNITY_EDITOR_OSX
        PlayerSettings.fullScreenMode = FullScreenMode.MaximizedWindow;
#else
        PlayerSettings.fullScreenMode = FullScreenMode.FullScreenWindow;
#endif

        PlayerSettings.defaultIsNativeResolution = false;
        PlayerSettings.defaultScreenWidth = 1024;
        PlayerSettings.defaultScreenHeight = 768;
        PlayerSettings.macRetinaSupport = true;
        PlayerSettings.runInBackground = true;

        PlayerSettings.captureSingleScreen = false;
        PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.Enabled;
        PlayerSettings.resizableWindow = true;
        PlayerSettings.visibleInBackground = false;
        PlayerSettings.allowFullscreenSwitch = true;
        PlayerSettings.forceSingleInstance = false;

        PlayerSettings.SetAspectRatio(AspectRatio.Aspect4by3, true);
        PlayerSettings.SetAspectRatio(AspectRatio.Aspect5by4, false);
        PlayerSettings.SetAspectRatio(AspectRatio.Aspect16by9, false);
        PlayerSettings.SetAspectRatio(AspectRatio.Aspect16by10, true);
        PlayerSettings.SetAspectRatio(AspectRatio.AspectOthers, false);

        OpenTestScene(3);
    }

    private static bool SwitchSceneIfOkay()
    {
        // Open the Resolution Test Scene
        Scene testScene = SceneManager.GetActiveScene();
        if (testScene.name != scene_name)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                testScene = EditorSceneManager.OpenScene("Assets/Scenes/" + scene_name + ".unity", OpenSceneMode.Single);
                return true;
            }
            else
                return false;
        }
        else
            return true;
    }

    // Open the scene for Player settings test in Editor
    private static void OpenTestScene(int presetNum)
    {
        // Find the Preset instruction UI canvas from root objects.
        List<GameObject> rootObjectsList = new List<GameObject>();
        SceneManager.GetActiveScene().GetRootGameObjects(rootObjectsList);

        Text[] presetText = null;
        foreach (GameObject ob in rootObjectsList)
        {
            if (ob.CompareTag("ResTestPreset"))
            {
                presetText = ob.GetComponentsInChildren<Text>(true);
                break;
            }
        }

        // Show instruction for selected preset
        for (int i = 0; i < presetText.Length; i++)
        {
            if (presetNum == i)
            {
                presetText[i].gameObject.SetActive(true);
                Selection.objects = new Object[] { presetText[i].gameObject };
            }
            else
                presetText[i].gameObject.SetActive(false);
        }

        // Save scene for build
        // Debug.Log("Current Player Setting is Preset " + presetNum + ".\nSave Scene.");
        // EditorSceneManager.SaveScene(testScene);
    }
}
