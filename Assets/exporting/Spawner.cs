using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    public GameObject[] spawnPrefabs;
    public Transform[] spawnPoints;
    public Transform player;

    public float minInterval = 3f;
    public float maxInterval = 6f;

    public int minObjects = 5;
    public int maxObjects = 20;
    public float minDistanceFromPlayer = 3f;

    private List<GameObject> activeObjects = new List<GameObject>();
    private float spawnTimer;

    void Start()
    {
        spawnTimer = Random.Range(minInterval, maxInterval);
    }

    void Update()
    {
        activeObjects.RemoveAll(item => item == null);

        if (activeObjects.Count < minObjects)
        {
            TrySpawn();
        }
        else
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0 && activeObjects.Count < maxObjects)
            {
                TrySpawn();
                spawnTimer = Random.Range(minInterval, maxInterval);
            }
        }
    }

    void TrySpawn()
    {
        Transform ptr = spawnPoints[Random.Range(0, spawnPoints.Length)];
        float distanceToPlayer = Vector3.Distance(ptr.position, player.position);
        if (distanceToPlayer < minDistanceFromPlayer) return;

        float minDistanceBetweenObjects = 2f;

        foreach (GameObject obj in activeObjects)
        {
            if (obj == null) continue;
            float distanceToOther = Vector3.Distance(ptr.position, obj.transform.position);
            if (distanceToOther < minDistanceBetweenObjects)
            {
                return;
            }
        }

        GameObject prefab = spawnPrefabs[Random.Range(0, spawnPrefabs.Length)];
        GameObject newObj = Instantiate(prefab, ptr.position, Quaternion.identity);
        activeObjects.Add(newObj);
    }
}