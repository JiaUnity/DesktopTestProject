using UnityEngine;
using UnityEngine.UI;

public class DefaultInstruction : MonoBehaviour
{
    void Start()
    {
        Text text = GetComponent<Text>();

#if UNITY_STANDALONE || UNITY_EDITOR
        text.text = "It seems there is no Preset is chosen yet.\n\n";

#if UNITY_EDITOR
        text.text += "You need to exit Play mode then do the following:\n";

#elif UNITY_STANDALONE
        text.text += "You need to close the Player and go back into Editor to do the following:\n";

#endif
        text.text += "1. Go to <color=brown>Tools > Resolution Test</color> and pick a preset.\n";
        text.text += "2. <color=brown>Do not</color> make any change to this scene or Player setting.\n";
        text.text += "3. Build the Player.\n";
        text.text += "4. Load this scene in the Player and follow the instruction.\n\n";
        text.text += "The 3 presets will cover all the options for Resolution & Presentation in the Player settings. Therefore this project needs to be built 3 times, and each with a different preset for full coverage.\n";

#else
        text.text = "This test only works for Standalone Player.\n\n";
        text.text = "In other words, there is nothing to see here. Move along.";
#endif
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.DeleteAll();
    }
}
