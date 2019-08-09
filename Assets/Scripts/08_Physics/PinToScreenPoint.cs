using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class PinToScreenPoint : MonoBehaviour
{
    public Camera cam;
    public bool pinHorizontal = false;
    public bool pinVertical = false;
    public Vector3 pinPosition;
    public Vector2 offset;

    private Vector3 startPos;
    private Vector3 cameraViewPoint;

    void Start()
    {
        Reposition();
    }

    void Update()
    {
        Reposition();
    }

    void Reposition()
    {
        startPos = transform.position;
        cameraViewPoint = cam.ViewportToWorldPoint(pinPosition);

        if (pinHorizontal && !pinVertical)
            transform.position = new Vector3(cameraViewPoint.x + offset.x, startPos.y, startPos.z);
        else if (!pinHorizontal && pinVertical)
            transform.position = new Vector3(startPos.x, cameraViewPoint.y + offset.y, startPos.z);
        else
            transform.position = new Vector3(cameraViewPoint.x + offset.x, cameraViewPoint.y + offset.y, startPos.z);
    }
}
