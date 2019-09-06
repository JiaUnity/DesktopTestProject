using UnityEngine;
using UnityEngine.Networking;

public class EnemySpawner : NetworkBehaviour
{
    public GameObject m_enemyPrefab;
    public int m_enemyNumber = 4;

    public override void OnStartServer()
    {
        for (int i = 0; i < m_enemyNumber; i++)
        {
            Vector3 spawnPosition = new Vector3(
                Random.Range(-8f, 8f),
                0f,
                Random.Range(-8f, 8f)
            );

            Quaternion spawnRotation = Quaternion.Euler(0f, Random.Range(0, 180), 0f);

            GameObject enemy = Instantiate(m_enemyPrefab, spawnPosition, spawnRotation);
            NetworkServer.Spawn(enemy);
        }
    }
}
