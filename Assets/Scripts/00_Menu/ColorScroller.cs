using UnityEngine;
using System.Collections;

public class ColorScroller : MonoBehaviour
{
    public float speed = 1f;
    public float saturation = 0.8f;

    private static float hue = 0f;
    private Color targetColor;

    void Update()
    {
        hue += Time.deltaTime * speed;
        if (hue > 1f)
            hue -= 1f;

        targetColor = Color.HSVToRGB(hue, saturation, 1);
        GetComponent<Renderer>().material.color = targetColor;
    }
}
