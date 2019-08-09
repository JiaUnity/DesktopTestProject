using UnityEngine;
using System.Collections;

public class LightOnPhysicsEvents : MonoBehaviour
{
    [System.Serializable]
    public enum States{ TriggerEnter, TriggerExit, TriggerStay, CollisionEnter, CollisionExit, CollisionStay };
    public States states;

    private int turnOffFrame = -1;

    void Start()
    {
        LightOff();
    }

    void Update()
    {
        if ((states == States.CollisionStay || states == States.TriggerStay) && Time.frameCount >= turnOffFrame)
            LightOff();
    }

    void LightOn()
    {
        GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 0.5f);
    }

    void LightOff()
    {
        GetComponent<Renderer>().material.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.attachedRigidbody && states == States.CollisionEnter)
            LightOn();
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.collider.attachedRigidbody && states == States.CollisionExit)
            LightOff();
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.collider.attachedRigidbody && states == States.CollisionStay)
        {
            turnOffFrame = Time.frameCount + 2;
            LightOn();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody && states == States.TriggerEnter)
            LightOn();
    }

    void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody && states == States.TriggerExit)
            LightOff();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.attachedRigidbody && states == States.TriggerStay)
        {
            turnOffFrame = Time.frameCount + 5;
            LightOn();
        }
    }
}
