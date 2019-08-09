using UnityEngine;
using System.Collections;

public class AudioControlsChild : MonoBehaviour
{
    public enum ControlType { Rewind, Play, Pause, Stop, FastForward, VolumeUp, VolumeDown }

    public AudioControlsMaster audioMaster;
    public ControlType controlType;

    public void RecieveRaycast()
    {
        audioMaster.RecieveInput(controlType);
    }
}
