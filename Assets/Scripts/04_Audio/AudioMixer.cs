using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioMixer : MonoBehaviour
{
    public AudioSource audio_source;
    public AudioMixerGroup audio_mixer;

    private bool is_mixer = false;

    // Use this for initialization
    void Start()
    {
        ToggleMixer();
    }

    public void RecieveRaycast()
    {
        ToggleMixer();
    }

    private void ToggleMixer()
    {
        is_mixer = !is_mixer;
        audio_source.outputAudioMixerGroup = is_mixer ? audio_mixer : null;
        GetComponent<Renderer>().material.color = is_mixer ? Color.green : Color.white;
    }
}
