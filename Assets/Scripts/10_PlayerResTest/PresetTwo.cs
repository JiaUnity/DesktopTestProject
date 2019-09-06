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
        text.text += "When you launch Player for the first time under Preset 2, <color=brown>do not</color> hold any key down while launching it.";

#else
        text.text += "Launch for the first time:\n";
        text.text += "1. Player window launches in <color=brown>Windowed Mode</color>.\n";
        text.text += "2. Resolution Dialog <color=brown>did not appear</color>.\n";
        text.text += "3. Player window resolution should be 1024x768. Launch: <color=brown>" + Screen.width + "x" + Screen.height + "</color>.\n";

#if UNITY_STANDALONE_OSX
        text.text += "4. Player window <color=brown>cannot</color> be resized.\n";
        text.text += "5. Player window <color=brown>cannot</color> swtich to fullscreen (Command + F, Green Dot, etc.)\n";
        text.text += "6. The menu item:<color=brown> Window -> Fullscreen</color> is greyed out.\n";
        text.text += "7. Close this Player and launch a new one with <color=brown>Option key down</color>.\n\n";
        text.text += "After re-launch (with Option key down):\n";
        text.text += "8. Resolution Dialog <color=brown>appears</color>.\n";
        text.text += "9. Player window uses the setting from the Dialog.\n";
        text.text += "10. The <color=brown>highest resolution</color> choice in the Dialog is 1920 x 1200.\n";
#else
        text.text += "4. Player window <color=brown>cannot</color> be resized. The \"Maximize\" button is greyed out.\n";
        text.text += "5. Player window <color=brown>cannot</color> swtich to fullscreen (Alt + Enter, etc.)\n";
        text.text += "6. <color=brown>Do not</color> close this Player and launch another one with <color=brown>Shift key down</color>.\n\n";
        text.text += "For the second Player window:\n";
        text.text += "7. Resolution Dialog <color=brown>appears</color>.\n";
        text.text += "8. The first one is still running.\n";
        text.text += "9. It uses the settings from the Resolution Dialog.\n";
        text.text += "10. In <color=brown>fullscreen</color> mode, Player window <color=brown>remains</color> in place after loses focus.\n";
#endif
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
