using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

#if UNITY_2019_1_OR_NEWER
public class Health : MonoBehaviour
{
    void Start()
    {
        enabled = false;
    }
}
#else
public class Health : NetworkBehaviour
{
    public const int m_maxHealth = 100;

    [SyncVar(hook = "OnChangeHealth")]
    public int m_currentHealth = m_maxHealth;
    public RectTransform m_healthBarUI;
    public bool m_destroyOnDeath;

    private NetworkStartPosition[] m_spawnPoints;

    private void Start()
    {
        if (isLocalPlayer)
            m_spawnPoints = FindObjectsOfType<NetworkStartPosition>();
    }

    public void TakeDamage(int amount)
    {
        if (!isServer)
            return;

        m_currentHealth -= amount;
        if (m_currentHealth <= 0)
        {
            if (m_destroyOnDeath)
                Destroy(gameObject);
            else
            {
                m_currentHealth = m_maxHealth;
                RpcRespawn();
            }
        }
    }

    void OnChangeHealth(int health)
    {
        m_healthBarUI.sizeDelta = new Vector2(health, m_healthBarUI.sizeDelta.y);
    }

    [ClientRpc]
    void RpcRespawn()
    {
        if (isLocalPlayer)
        {
            Vector3 spawnPoint = Vector3.zero;

            // If there is a spawn point array that's not empty, pick one at random
            if (m_spawnPoints != null && m_spawnPoints.Length > 0)
                spawnPoint = m_spawnPoints[Random.Range(0, m_spawnPoints.Length)].transform.position;

            transform.position = spawnPoint;
        }
    }
}
#endif
