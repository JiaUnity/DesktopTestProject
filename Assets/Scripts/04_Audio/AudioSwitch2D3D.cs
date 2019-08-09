using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSwitch2D3D : MonoBehaviour
{
    public AudioSource audio_source;

    private bool is_2d = true;

    // Use this for initialization
    void Start()
    {
        Switch2D3D();
    }

    public void RecieveRaycast()
    {
        Switch2D3D();
    }

    private void Switch2D3D()
    {
        is_2d = !is_2d;
        audio_source.spatialBlend = is_2d ? 0f : 1f;
        GetComponent<Renderer>().material.color = is_2d ? Color.green : Color.white;
    }
}
