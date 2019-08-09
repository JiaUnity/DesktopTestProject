using System.IO;
using UnityEngine;

public class OpenCrashReportFolder : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        gameObject.SetActive(true);
#else
        gameObject.SetActive(false);
#endif
    }

    public void OpenCrashFolder()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        string crashFolder = UnityEngine.Windows.CrashReporting.crashReportFolder;

        while (!Directory.Exists(crashFolder))
            crashFolder = Directory.GetParent(crashFolder).FullName;

        try
        {
            crashFolder = "\"" + crashFolder.Replace("/", "\\") + "\"";
            Debug.Log(crashFolder);
            System.Diagnostics.Process.Start("explorer.exe", crashFolder);
        }
        catch (System.ComponentModel.Win32Exception e)
        {
            // just silently skip error
            // we currently have no platform define for the current OS we are in, so we resort to this
            e.HelpLink = ""; // do anything with this variable to silence warning about not using it
        }
#endif
    }
}
