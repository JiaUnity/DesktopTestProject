using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_2019_1_OR_NEWER
public class Bullet : MonoBehaviour
{
    void Start()
    {
        enabled = false;
    }
}
#else
public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        GameObject hit = collision.gameObject;
        Health health = hit.GetComponent<Health>();

        if (health != null)
            health.TakeDamage(10);

        Destroy(gameObject);
    }
}
#endif
