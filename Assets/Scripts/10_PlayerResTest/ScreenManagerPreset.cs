using UnityEngine;
using UnityEngine.UI;

public class ScreenManagerPreset
{
    public static string GetAvailableResolutions()
    {
        string result = "";
        Resolution current = new Resolution();

        foreach (Resolution res in Screen.resolutions)
        {
            if (res.height != current.height || res.width != current.width)
            {
                current = res;
                if (!string.IsNullOrEmpty(result))
                    result += ", ";
                result += current.width + "x" + current.height;
            }
        }
        return result;
    }
}
