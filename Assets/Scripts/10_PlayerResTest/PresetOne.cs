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
        text.text += "2. Resolution Dialog <color=brown>did not appear</color>.\n";
        text.text += "3. Player window is in native resolution. Launch: <color=brown>" + Screen.width + "x" + Screen.height + "</color>.\n";
        text.text += "4. The music <color=brown>stops playing</color> when Player window loses focus.\n";

#if UNITY_STANDALONE_OSX
        text.text += "5. Player window <color=brown>can</color> swtich to windowed mode (Command + F, Green Dot, etc.)\n";
//#if !UNITY_2018_1_OR_NEWER
        text.text += "6. Dock and Menu Bar <color=brown>do not appear</color> when cursor is at the edge.\n";
//#endif
#else
        text.text += "5. Player window <color=brown>can</color> swtich to windowed mode (Alt + Enter, etc,) and the screen flickers during the process.\n";
        text.text += "6. A second Player instance <color=brown>does not start</color> (mouse click, press Enter, etc.)";
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
