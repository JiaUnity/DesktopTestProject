using UnityEngine;
using UnityEngine.UI;

public class PresetThree : MonoBehaviour
{
    void Start()
    {
        Text text = GetComponent<Text>();

#if UNITY_STANDALONE || UNITY_EDITOR
        text.text = "This is: <color=brown>Preset 3</color>.\n\n";

#if UNITY_EDITOR
        text.text += "Build Player, come back to this scene and follow the instruction to complete the test.\n\n";

//#else
        text.text += "Checklist:\n";
        text.text += "1. Player window launches in <color=brown>fullscreen</color>.\n";

#if UNITY_STANDALONE_OSX
        text.text += "5. Player window <color=brown>can</color> swtich between fullscreen and windowed mode (Command + F, Green Dot, etc.)\n"; 
        text.text += "6. In fullscreen mode, Dock and Menu Bar <color=brown>appear</color> when cursor is at the edge.\n";
#else
        text.text += "2. In fullscreen mode, when Player loses focus, the music <color=brown>stops playing</color>, but the Player window remains in place.\n";
        text.text += "3. In windowed mode, Player window <color=brown>cannot</color> be resized. The maximize button in the top right corner is disabled.\n";
#endif
        text.text += "4. Only Resolutions in aspect ratios 4:3 and 16:10 should be available.\n";
        text.text += "Make sure these ones are available in the list below: <color=brown>640x480, 1920x1200</color>.\n";
        text.text += "Make sure these ones are not available in the list: <color=brown>720x480, 720x576, 1920x1080</color>.\n\n";
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
