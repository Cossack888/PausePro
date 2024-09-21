using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int numberOfEnemies = 10;
    public float spawnRadius = 10f;
    public float navMeshSampleDistance = 1.0f;
    public Transform spawnCenter;
    void Start()
    {
        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            Vector3 spawnLocation = GetRandomNavMeshPosition(spawnCenter.position, spawnRadius);
            if (spawnLocation != Vector3.zero)
            {
                Instantiate(enemyPrefab, spawnLocation, Quaternion.identity);
            }
        }
    }

    public Vector3 GetRandomNavMeshPosition(Vector3 center, float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += center;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return Vector3.zero;
    }
}
