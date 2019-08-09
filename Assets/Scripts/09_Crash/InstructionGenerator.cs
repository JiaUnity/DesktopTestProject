using UnityEngine;
using UnityEngine.UI;

public class InstructionGenerator : MonoBehaviour
{
    public Text instruc;

    // Use this for initialization
    void Start()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN

#if UNITY_2018_1_OR_NEWER
        instruc.text += "Once crashed, the Crash Handler should appear and generate Crash Reports, which can be found at:\n";
#else
        instruc.text += "The current version of Unity does NOT support Crash Handler, but the crash report can be found at:\n";
#endif
        instruc.text += "<color=brown>" + UnityEngine.Windows.CrashReporting.crashReportFolder + "</color>";

#if UNITY_EDITOR_WIN
        instruc.text += "\nOnce the crash is handled, the Bug Report should appear with all the files generated.";
#endif
#else
        instruc.text += "<color=brown>Since Crash Handler is only available for Windows Editor and Standalone player, </color>";
#if UNITY_EDITOR
        instruc.text += "when it crashes, the Bug Report may appear depending on the crash type.";
#elif UNITY_WSA
        instruc.text += "the application will close quietly.";
#else
        instruc.text += "only the default OS crashing message may appear.";
#endif
#endif
    }
}
