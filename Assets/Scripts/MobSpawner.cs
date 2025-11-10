using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class MobSpawner : MonoBehaviour
{
    [Header("Basic Settings")]
    public GameObject[] mobPrefabs;
    public Transform[] spawnPoints;
    public float spawnDelay = 3f;
    public int mobsToSpawn = 5;
    public float spawnRadius = 2f;
    
    [Header("Advanced Settings")]
    public bool useAdvancedSpawning = true;
    public bool spawnNearPlayer = true;
    public float playerDetectionRange = 30f;
    public float minDistanceFromPlayer = 5f;
    
    [Header("Wave Settings")]
    public bool useWaves = false;
    public int wavesPerRound = 3;
    public float timeBetweenWaves = 10f;
    public int mobsPerWave = 3;
    
    private int spawnedCount = 0;
    private int currentWave = 0;
    private Transform player;
    private bool isSpawning = false;
    private int currentSpawnPointIndex = 0;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        

        
        if (useWaves)
        {
            StartCoroutine(SpawnWaves());
        }
        else
        {
            StartCoroutine(BasicSpawning());
        }
    }
    
    IEnumerator BasicSpawning()
    {
        yield return new WaitForSeconds(1f);
        
        while (spawnedCount < mobsToSpawn)
        {
            SpawnMob();
            yield return new WaitForSeconds(spawnDelay);
        }
    }
    
    IEnumerator SpawnWaves()
    {
        yield return new WaitForSeconds(1f);
        
        while (currentWave < wavesPerRound)
        {
            Debug.Log($"Starting Wave {currentWave + 1}");
            
            int mobsInWave = 0;
            while (mobsInWave < mobsPerWave && spawnedCount < mobsToSpawn)
            {
                SpawnMob();
                mobsInWave++;
                yield return new WaitForSeconds(spawnDelay);
            }
            
            currentWave++;
            
            if (currentWave < wavesPerRound)
            {
                yield return new WaitForSeconds(timeBetweenWaves);
            }
        }
    }

    void SpawnMob()
    {
        if (spawnedCount >= mobsToSpawn) return;
        if (spawnPoints.Length == 0) return;
        if (mobPrefabs.Length == 0) return;

        Transform spawnPoint = GetNextSpawnPoint();
        if (spawnPoint == null) return;

        GameObject mobPrefab = mobPrefabs[Random.Range(0, mobPrefabs.Length)];
        
        Vector3 spawnPos = GetSpawnPosition(spawnPoint);
        
        GameObject newMob = Instantiate(mobPrefab, spawnPos, spawnPoint.rotation);
        
        SetupSpawnedMob(newMob);
        
        spawnedCount++;
        currentSpawnPointIndex = (currentSpawnPointIndex + 1) % spawnPoints.Length;
        Debug.Log($"Spawned {mobPrefab.name} at {spawnPoint.name}. Total spawned: {spawnedCount}");
    }
    
    Transform GetNextSpawnPoint()
    {
        if (spawnPoints.Length == 0) return null;
        
        if (spawnNearPlayer && player != null)
        {
            // Try to find a valid spawn point starting from current index
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                int index = (currentSpawnPointIndex + i) % spawnPoints.Length;
                Transform point = spawnPoints[index];
                
                if (point == null) continue;
                
                float distanceToPlayer = Vector3.Distance(point.position, player.position);
                
                if (distanceToPlayer <= playerDetectionRange && distanceToPlayer >= minDistanceFromPlayer)
                {
                    return point;
                }
            }
            
            // If no valid point found, use the next one anyway
            return spawnPoints[currentSpawnPointIndex];
        }
        
        // Use spawn points in order
        return spawnPoints[currentSpawnPointIndex];
    }
    
    Vector3 GetSpawnPosition(Transform spawnPoint)
    {
        Vector3 basePos = spawnPoint.position;
        
        Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPos = basePos + new Vector3(randomOffset.x, 0, randomOffset.y);
        
        if (Physics.Raycast(spawnPos + Vector3.up * 10f, Vector3.down, out RaycastHit hit, 50f))
        {
            spawnPos.y = hit.point.y;
        }
        
        if (NavMesh.SamplePosition(spawnPos, out NavMeshHit navHit, 5f, NavMesh.AllAreas))
        {
            spawnPos = navHit.position;
        }
        else
        {
            Debug.LogWarning($"{name}: no NavMesh near {spawnPos}");
        }
        
        return spawnPos;
    }
    
    void SetupSpawnedMob(GameObject mob)
    {
        MobAI mobAI = mob.GetComponent<MobAI>();
        if (mobAI != null)
        {
        }
        
    }
    
    public void StartSpawning()
    {
        if (!isSpawning)
        {
            isSpawning = true;
            spawnedCount = 0;
            currentWave = 0;
            
            if (useWaves)
            {
                StartCoroutine(SpawnWaves());
            }
            else
            {
                StartCoroutine(BasicSpawning());
            }
        }
    }
    
    public void StopSpawning()
    {
        StopAllCoroutines();
        isSpawning = false;
    }
    
    public void ResetSpawner()
    {
        StopSpawning();
        spawnedCount = 0;
        currentWave = 0;
        currentSpawnPointIndex = 0;
    }
    
    void OnDrawGizmosSelected()
    {
        if (spawnPoints == null) return;
        
        foreach (Transform point in spawnPoints)
        {
            if (point == null) continue;
            
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(point.position, 0.5f);
            
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(point.position, spawnRadius);
        }
        
        if (spawnNearPlayer && player != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(player.position, playerDetectionRange);
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.position, minDistanceFromPlayer);
        }
    }
}
