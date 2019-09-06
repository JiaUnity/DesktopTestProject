using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    public GameObject m_bulletPrefab;
    public Transform m_bulletSpawn;

    private int m_bulletSpeed = 20;
    private int m_turnSpeed = 600;
    private int m_moveSpeed = 10;

    // Use this for initialization
    void Start()
    {
        if (!isLocalPlayer)
            Destroy(transform.Find("Camera").gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
            return;

        float turnAmount = Input.GetAxis("Mouse X");
        Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        move = move.normalized * m_moveSpeed * Time.deltaTime;

        transform.Translate(move);
        transform.Rotate(0, turnAmount * m_turnSpeed * Time.deltaTime, 0);

        if (Input.GetButtonDown("Shoot"))
            CmdFire();
    }

    public override void OnStartLocalPlayer()
    {
        GetComponent<MeshRenderer>().material.color = Color.yellow;
    }

    [Command]
    private void CmdFire()
    {
        // Create the Bullet from the Bullet prefab
        GameObject bullet = Instantiate(m_bulletPrefab, m_bulletSpawn.position, m_bulletSpawn.rotation);

        // Add velocity
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * m_bulletSpeed;

        // Spawn the bullet on the Clients
        NetworkServer.Spawn(bullet);

        // Destroy the bullet after 2 seconds
        Destroy(bullet, 2f);
    }
}
