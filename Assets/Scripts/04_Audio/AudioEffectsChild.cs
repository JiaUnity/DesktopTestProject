using UnityEngine;
using System.Collections;

public class AudioEffectsChild : MonoBehaviour
{
    public enum EffectType { LowPass, HighPass, Echo, Distortion, Reverb, Chorus, Pitch, Scheduled }

    public AudioEffectsMaster effectsMaster;
    public EffectType effectType;
    public bool isWaveEffect = false;

    private bool isActive = false;

    public void RecieveRaycast()
    {
        effectsMaster.RecieveInput(effectType, isWaveEffect);

        if (effectType == EffectType.Scheduled && !isWaveEffect)
        {
            StartCoroutine(FlashColour(2f, GetComponent<Renderer>()));
        }
        else
        {
            if (!isActive)
                GetComponent<Renderer>().material.color = Color.green;
            else
                GetComponent<Renderer>().material.color = Color.white;

            isActive = !isActive;
        }
    }

    float startFlashTime;

    IEnumerator FlashColour(float duration, Renderer targetObject)
    {
        startFlashTime = Time.time;

        while (Time.time < startFlashTime + duration)
        {
            targetObject.material.color = Color.green;
            yield return null;
        }

        targetObject.material.color = Color.white;
    }
}
