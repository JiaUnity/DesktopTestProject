using UnityEngine;
using System.Collections;

public class Bobbing : MonoBehaviour
{
    public Vector3 bobAmount;
    public Vector3 rotateAmount;

    public Vector3 bobSpeed;
    public Vector3 rotateSpeed;

    private Vector3 startPos;
    private Vector3 startRot;

    private Vector3 targetPos;
    private Vector3 targetRot;

    void Start()
    {
        startPos = transform.localPosition;
        startRot = transform.localEulerAngles;
    }

    void Update()
    {
        targetPos = new Vector3(startPos.x + (bobAmount.x * Mathf.Sin(Time.time * bobSpeed.x)),
            startPos.y + (bobAmount.y * Mathf.Sin(Time.time * bobSpeed.y)),
            startPos.z + (bobAmount.z * Mathf.Sin(Time.time * bobSpeed.z)));

        targetRot = new Vector3(startRot.x + (rotateAmount.x * Mathf.Sin(Time.time * rotateSpeed.x)),
            startRot.y + (rotateAmount.y * Mathf.Sin(Time.time * rotateSpeed.y)),
            startRot.z + (rotateAmount.z * Mathf.Sin(Time.time * rotateSpeed.z)));

        transform.localEulerAngles = targetRot;
        transform.localPosition = targetPos;
    }
}
