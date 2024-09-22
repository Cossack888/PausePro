
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class HealthSpawner : MonoBehaviour
{
    public GameObject healthPrefab;
    public int numberOfHearts = 20;
    public int maxHearts = 50;
    public float spawnRadius = 100f;
    public float navMeshSampleDistance = 1.0f;
    public float refreshTime = 60f;
    public Transform spawnCenter;
    List<GameObject> hearts = new List<GameObject>();
    void Start()
    {
        SpawnHearts(numberOfHearts);
        StartCoroutine(RefreshHearts(refreshTime));
    }

    void SpawnHearts(int numberOfHearts)
    {
        for (int i = 0; i < numberOfHearts; i++)
        {
            Vector3 spawnLocation = GetRandomNavMeshPosition(spawnCenter.position, spawnRadius);
            if (spawnLocation != Vector3.zero)
            {
                Instantiate(healthPrefab, spawnLocation, Quaternion.identity);
            }
        }
        foreach (Collectible heart in FindObjectsOfType<Collectible>())
        {
            if (!hearts.Contains(heart.gameObject))
            {
                if (heart.heart)
                {
                    hearts.Add(heart.gameObject);
                }

            }
        }
    }

    IEnumerator RefreshHearts(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (hearts.Count < maxHearts)
        {
            SpawnHearts(Random.Range(1, numberOfHearts));
            StartCoroutine(RefreshHearts(waitTime));
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
