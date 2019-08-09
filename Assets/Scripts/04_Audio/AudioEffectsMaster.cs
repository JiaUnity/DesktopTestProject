using UnityEngine;
using System.Collections;

public class AudioEffectsMaster : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioLowPassFilter lowPassFilter;
    public AudioHighPassFilter highPassFilter;
    public AudioEchoFilter echoFilter;
    public AudioDistortionFilter distortionFilter;
    public AudioReverbFilter reverbFilter;
    public AudioChorusFilter chorusFilter;

    private bool lowPassWave = false;
    private bool highPassWave = false;
    private bool echoWave = false;
    private bool distortionWave = false;
    private bool reverbWave = false;
    private bool chorusWave = false;
    private bool pitchFlip = false;
    private bool scheduledEnd = false;

    private bool changePitch = false;

    private float sineWave;

    void Update()
    {
        sineWave = Mathf.Sin(Time.time) + 1f;
        sineWave *= 0.5f;

        if (lowPassWave)
            lowPassFilter.cutoffFrequency = (4500f * sineWave) + 500f;
        else
            lowPassFilter.cutoffFrequency = 5000f;

        if (highPassWave)
            highPassFilter.cutoffFrequency = (4500f * sineWave) + 500f;
        else
            highPassFilter.cutoffFrequency = 5000f;

        if (echoWave)
            echoFilter.wetMix = sineWave;
        else
            echoFilter.wetMix = 1f;

        if (distortionWave)
            distortionFilter.distortionLevel = (0.8f * sineWave) + 0.1f;
        else
            distortionFilter.distortionLevel = 0.9f;

        if (chorusWave)
        {
            chorusFilter.wetMix1 = sineWave;
            chorusFilter.wetMix2 = sineWave;
            chorusFilter.wetMix3 = sineWave;
        }
        else
        {
            chorusFilter.wetMix1 = 0.5f;
            chorusFilter.wetMix2 = 0.5f;
            chorusFilter.wetMix3 = 0.5f;
        }

        if (changePitch)
        {
            if (!pitchFlip)
                audioSource.pitch = 2f;
            else if (pitchFlip)
                audioSource.pitch = 0.5f;
        }
        else
            audioSource.pitch = 1f;
    }

    public void RecieveInput(AudioEffectsChild.EffectType effectType, bool isWaveEffect)
    {
        if (!isWaveEffect)
        {
            if (effectType == AudioEffectsChild.EffectType.LowPass)
                lowPassFilter.enabled = !lowPassFilter.enabled;
            else if (effectType == AudioEffectsChild.EffectType.HighPass)
                highPassFilter.enabled = !highPassFilter.enabled;
            else if (effectType == AudioEffectsChild.EffectType.Echo)
                echoFilter.enabled = !echoFilter.enabled;
            else if (effectType == AudioEffectsChild.EffectType.Distortion)
                distortionFilter.enabled = !distortionFilter.enabled;
            else if (effectType == AudioEffectsChild.EffectType.Reverb)
                reverbFilter.enabled = !reverbFilter.enabled;
            else if (effectType == AudioEffectsChild.EffectType.Chorus)
                chorusFilter.enabled = !chorusFilter.enabled;
            else if (effectType == AudioEffectsChild.EffectType.Pitch)
                changePitch = !changePitch;
        }
        else if (isWaveEffect)
        {
            if (effectType == AudioEffectsChild.EffectType.LowPass)
                lowPassWave = !lowPassWave;
            else if (effectType == AudioEffectsChild.EffectType.HighPass)
                highPassWave = !highPassWave;
            else if (effectType == AudioEffectsChild.EffectType.Echo)
                echoWave = !echoWave;
            else if (effectType == AudioEffectsChild.EffectType.Distortion)
                distortionWave = !distortionWave;
            else if (effectType == AudioEffectsChild.EffectType.Reverb)
                reverbWave = !reverbWave;
            else if (effectType == AudioEffectsChild.EffectType.Chorus)
                chorusWave = !chorusWave;
            else if (effectType == AudioEffectsChild.EffectType.Pitch)
                pitchFlip = !pitchFlip;
        }

        if (effectType == AudioEffectsChild.EffectType.Scheduled)
        {
            if (isWaveEffect)
            {
                scheduledEnd = !scheduledEnd;
            }
            else
            {
                audioSource.PlayScheduled(AudioSettings.dspTime + 2f);

                if (scheduledEnd)
                    audioSource.SetScheduledEndTime(AudioSettings.dspTime + 4f);
            }
        }
    }
}
