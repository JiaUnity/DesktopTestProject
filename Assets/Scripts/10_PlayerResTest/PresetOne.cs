using UnityEngine;
using UnityEngine.UI;

public class PresetOne : MonoBehaviour
{
    void Start()
    {
        Text text = GetComponent<Text>();

#if UNITY_STANDALONE || UNITY_EDITOR
        text.text = "This is: <color=brown>Preset 1</color>.\n\n";

#if UNITY_EDITOR
        text.text += "Build Player, come back to this scene and follow the instruction to complete the test.";

#else
        text.text += "Checklist:\n";
        text.text += "1. Player window launches in <color=brown>fullscreen</color>.\n";
        text.text += "2. Player window is in native resolution. Launch: <color=brown>" + Screen.width + "x" + Screen.height + "</color>.\n";
        text.text += "3. The music <color=brown>stops playing</color> when Player window loses focus.";

#if UNITY_STANDALONE_OSX
        text.text += "\n";
        text.text += "4. Player window <color=brown>cannot</color> swtich to windowed mode (Command + F does not work. The Green Dot and the menu item:<color=brown> Window -> Fullscreen</color> is disabled.)\n";
        text.text += "5. Dock and Menu Bar <color=brown>do not appear</color> when cursor is at the edge.\n";
#else
        text.text += " And the screen flickers during the change of focus.\n";
        text.text += "4. Player window <color=brown>cannot</color> swtich to windowed mode (Alt + Enter, etc,).\n";
        text.text += "5. A second Player instance <color=brown>does not start</color> (mouse click, press Enter, etc.)";
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
