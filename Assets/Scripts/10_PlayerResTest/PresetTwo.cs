using UnityEngine;
using UnityEngine.UI;

public class PresetTwo : MonoBehaviour
{
    void Start()
    {
        Text text = GetComponent<Text>();

#if UNITY_STANDALONE || UNITY_EDITOR
        text.text = "This is: <color=brown>Preset 2</color>.\n\n";

#if UNITY_EDITOR
        text.text += "Build Player, come back to this scene and follow the instruction to complete the test.\n\n";

#else
        text.text += "Checklist:\n";
        text.text += "1. Player window launches in <color=brown>Windowed Mode</color>.\n";
        text.text += "2. Player window resolution should be 1024x768. Launch: <color=brown>" + Screen.width + "x" + Screen.height + "</color>.\n";
        text.text += "3. Player window <color=brown>can</color> be resized or maximized.\n";
        text.text += "4. The music <color=brown>keeps playing</color> after Player window loses focus.\n";

#if UNITY_STANDALONE_OSX

        text.text += "5. Player window <color=brown>can</color> swtich to fullscreen (Command + F, Green Dot, Menu item: Window -> Fullscreen.)\n";
        text.text += "6. Make sure the highest resolution available in the list below is <color=brown>1920x1200</color>.\n";
        text.text += "7. Only Resolutions in aspect ratios 4:3 and 16:10 should be available.\n";
        text.text += "Make sure these ones are available in the list below: <color=brown>640x480, 1920x1200</color>.\n";
        text.text += "Make sure these ones are not available in the list: <color=brown>720x480, 720x576, 1920x1080</color>.\n\n";

#else
        text.text += "5. Player window <color=brown>can</color> swtich to fullscreen (Alt + Enter, etc.), and the screen does not flicker during the process.\n";
        text.text += "6. When Player window loses focus in fullscreen mode, it <color=brown>minimizes</color> into taskbar.\n";
        text.text += "7. An extra Player can start with the current one still running.\n";
        text.text += "8. Resolutions in all aspect ratios should be available. Make sure these ones are available in the list below: <color=brown>640x480, 720x480, 720x576, 1920x1080, 1920x1200</color>\n\n";
#endif
        text.text += "Available resolutions: <color=blue>" + ScreenManagerPreset.GetAvailableResolutions() + "</color>\n";

#endif

#else
        text.text = "This test only works for Standalone Player.\n\n";
        text.text += "In other words, there is nothing to see here. Move along.";
#endif
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.DeleteAll();
    }
}
