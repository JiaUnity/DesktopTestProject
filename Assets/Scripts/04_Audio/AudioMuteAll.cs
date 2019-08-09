using UnityEngine;
using System.Collections;

public class AudioMuteAll : MonoBehaviour
{
    public GameObject muteIconEmitter;

    public void RecieveRaycast()
    {
        AudioListener.pause = !AudioListener.pause;

        muteIconEmitter.SetActive(AudioListener.pause);
    }
}
