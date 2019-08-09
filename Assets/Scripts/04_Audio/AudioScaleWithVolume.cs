using UnityEngine;
using System.Collections;

public class AudioScaleWithVolume : MonoBehaviour
{
    [System.Serializable]
    public enum WhatToScale{ Transform, Light };
    public WhatToScale whatToScale;

    public bool keepStartScale = true;
    public Vector3 axes = Vector3.one;
    public float volume = 2f; // set how much the scale will vary
    public Renderer colourRenderer;
    public AudioSource audioSource;

    Vector3 startScale = Vector3.zero;
    int qSamples = 1024;  // array size
    float refValue = 0.1f; // RMS value for 0 dB
    float rmsValue;   // sound level - RMS
    float dbValue;    // sound level - dB

    private float[] samples; // audio samples

    void Start()
    {
        samples = new float[qSamples];

        if (keepStartScale)
            startScale = transform.localScale;

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void GetVolume()
    {
        if (audioSource.time > 0f)
        {
            audioSource.GetOutputData(samples, 0); // fill array with samples
            int i;
            float sum = 0f;

            for (i = 0; i < qSamples; i++)
            {
                sum += samples[i] * samples[i]; // sum squared samples
            }

            rmsValue = Mathf.Sqrt(sum / qSamples); // rms = square root of average
            dbValue = 20 * Mathf.Log10(rmsValue / refValue); // calculate dB

            if (dbValue < -160)
                dbValue = -160; // clamp it to -160dB min
        }
        else
            rmsValue = 0f;
    }

    void Update()
    {
        GetVolume();

        switch (whatToScale)
        {
            case WhatToScale.Transform:
                transform.localScale = startScale + axes * (volume * rmsValue);
                if (colourRenderer)
                    colourRenderer.material.color = Color.Lerp(Color.green, Color.red, volume * rmsValue);
                break;
            case WhatToScale.Light:
                GetComponent<Light>().intensity = (volume * rmsValue) * 8f;
                break;
        }
    }
}
