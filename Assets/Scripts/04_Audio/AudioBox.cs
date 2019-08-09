using UnityEngine;
using System.Collections;

public class AudioBox : MonoBehaviour
{
    public AudioClip[] noises;
    public Rigidbody player;
    private FixedJoint fixedJoint;

    void Update()
    {
        if (Input.GetButtonDown("Shoot") && fixedJoint != null)
        {
            Destroy(fixedJoint);
        }
    }

    void OnCollisionEnter()
    {
        GetComponent<AudioSource>().PlayOneShot(noises[Random.Range(0, noises.Length)], 1f);
    }

    public void RecieveRaycast()
    {
        if (fixedJoint == null)
        {
            fixedJoint = gameObject.AddComponent<FixedJoint>();
            fixedJoint.connectedBody = player;
        }
        else if (fixedJoint)
        {
            Destroy(fixedJoint);
        }
    }
}
