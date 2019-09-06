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

#else
        text.text += "Checklist:\n";
        text.text += "1. Resolution Dialog <color=brown>appears</color>.\n";
        text.text += "2. In the Dialog, only <color=brown>4x3</color> and <color=brown>16x10</color> aspect ratio options are available";

#if UNITY_STANDALONE_OSX
        text.text += ", so are the options with resolution <color=brown>higher</color> than 1920 x 1200";
#endif
        text.text += ".\n";
        text.text += "3. Player window launches with the setting from the Dialog. Launch: <color=brown>" + Screen.width + "x" + Screen.height + "</color>.\n";
        text.text += "4. When Player window loses focus, the music <color=brown>keeps playing</color>.\n";

#if UNITY_STANDALONE_OSX
        text.text += "5. Player window <color=brown>can</color> swtich between fullscreen and windowed mode (Command + F, Green Dot, etc.)\n";
        text.text += "6. In fullscreen mode, Dock and Menu Bar <color=brown>appear</color> when cursor is at the edge.\n";
#else
        text.text += "5. Player window <color=brown>can</color> swtich between fullscreen and windowed mode (Alt + Enter, etc,) and there is <color=brown>no flickering</color> during the process.\n";
        text.text += "6. In fullscreen mode, Player window <color=brown>minimizes</color> when loses focus.\n";
#endif
        text.text += "7. In fullscreen mode, Player window is <color=brown>scaled</color> but <color=brown>not stretched</color> to fit the screen with a different aspect ratio.\n";
        text.text += "8. In windowed mode, Player window <color=brown>can be resized</color>.\n";
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
