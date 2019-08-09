using UnityEngine;
using System.Collections;

public class AudioCap : MonoBehaviour
{
    public Transform capPivot;

    void Update()
    {
        if (transform.position.y > capPivot.position.y)
            transform.position = Vector3.Lerp(transform.position, capPivot.position, Time.deltaTime * 3f);
        else
            transform.position = capPivot.position;
    }
}
