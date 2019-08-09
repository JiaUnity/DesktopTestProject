using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugInfo : MonoBehaviour
{
    public Text fpsText;
    public Text debugText;

    const float fpsMeasurePeriod = 0.5f;
    private int m_FpsAccumulator = 0;
    private float m_FpsNextPeriod = 0;
    private int m_CurrentFps;
    const string display = "FPS: {0}, VSync: {1}";

    // Use this for initialization
    void Start()
    {
        m_FpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;
        debugText.text = Application.platform + ", Version Number: " + Application.unityVersion;
    }

    // Update is called once per frame
    void Update()
    {
        FPSDisplay();
    }

    void FPSDisplay()
    {
        // measure average frames per second
        m_FpsAccumulator++;
        if (Time.realtimeSinceStartup > m_FpsNextPeriod)
        {
            m_CurrentFps = (int)(m_FpsAccumulator / fpsMeasurePeriod);
            m_FpsAccumulator = 0;
            m_FpsNextPeriod += fpsMeasurePeriod;
            fpsText.text = string.Format(display, m_CurrentFps, QualitySettings.vSyncCount);
        }
    }
}
