using UnityEngine;
using System.Collections;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject[] prefabObjects;

    public bool randomStartRotation = true;
    public float destroyAfterDelay = 0f;
    public bool withForce = false;

    public float force;
    public ForceMode forceMode;
    public Vector3 torque = Vector3.forward;
    public ForceMode torqueMode;

    public float spawnInterval = 0.5f;

    private float lastSpawnTime = Mathf.NegativeInfinity;
    private GameObject tempSpawnObject;
    private int randomSpawnIndex = 0;

    void Update()
    {
        if (Time.time > lastSpawnTime + spawnInterval)
        {
            lastSpawnTime = Time.time;
            randomSpawnIndex = Random.Range(0, prefabObjects.Length);

            tempSpawnObject = Instantiate(prefabObjects[randomSpawnIndex], transform.position, transform.rotation) as GameObject;
            tempSpawnObject.transform.parent = transform;

            if (randomStartRotation)
                tempSpawnObject.transform.localRotation = Random.rotation;

            if (withForce)
            {
                if (force != 0f)
                    tempSpawnObject.GetComponent<Rigidbody>().AddForce(transform.forward * force, forceMode);

                if (torque != Vector3.zero)
                    tempSpawnObject.GetComponent<Rigidbody>().AddTorque(torque, torqueMode);
            }

            if (destroyAfterDelay != 0)
                StartCoroutine(DelayedDestruction(tempSpawnObject));
        }
    }

    IEnumerator DelayedDestruction(GameObject destroyMe)
    {
        yield return new WaitForSeconds(destroyAfterDelay);

        if (destroyMe)
            Destroy(destroyMe);
    }
}
