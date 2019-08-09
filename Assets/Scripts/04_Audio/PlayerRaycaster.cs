using UnityEngine;
using System.Collections;

public class PlayerRaycaster : MonoBehaviour
{
    public LayerMask targetLayer;

    void Update()
    {
        if (Input.GetButtonUp("Shoot"))
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 3, targetLayer))
            {
                hit.collider.SendMessage("RecieveRaycast");
            }
        }
    }
}
