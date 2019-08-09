using System;
using UnityEngine;

[Serializable]
public class AnalogStick
{
    public Transform stick;             // The stick to move
    public string input_x_name;
    public string input_y_name;
    [Range(0f, 100f)]
    public float move_range;            // The distance of the transform can move in each direction
    public Vector3 original_position;   // The stick's initial position in the scene
}
