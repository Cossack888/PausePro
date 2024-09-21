
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int numberOfEnemies = 10;
    public int maxEnemies = 50;
    public float spawnRadius = 10f;
    public float navMeshSampleDistance = 1.0f;
    public float refreshTime = 60f;
    public Transform spawnCenter;
    List<GameObject> enemies = new List<GameObject>();
    void Start()
    {
        SpawnEnemies(numberOfEnemies);
        StartCoroutine(RefreshEnemies(refreshTime));
    }

    void SpawnEnemies(int numberOfEnemies)
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            Vector3 spawnLocation = GetRandomNavMeshPosition(spawnCenter.position, spawnRadius);
            if (spawnLocation != Vector3.zero)
            {
                GameObject enemy = Instantiate(enemyPrefab, spawnLocation, Quaternion.identity);
                enemies.Add(enemy);
            }
        }
    }

    IEnumerator RefreshEnemies(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (enemies.Count < maxEnemies)
        {
            SpawnEnemies(Random.Range(1, numberOfEnemies));
            StartCoroutine(RefreshEnemies(waitTime));
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
