using System;
using UnityEngine;

[Serializable]
public class AnalogKey
{
    [SerializeField]
    public GameObject trigger;                  //The gameobject
    public string input_name = "Stick1 X";      // The name for the input in Input Manager
    [Range(-1f, 1f)]
    public float trigger_value = 0f;          // The value from where the key is considered pressed.
}
