using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Obstacle Settings")]
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private Transform spawnParent;

    [Header("Obstacle Start Position")]
    [SerializeField] private float obstacleSpawnY = 0.65f;
    [SerializeField] private float obstacleSpawnZ = 48f;

    [Header("Spawn Pattern")]
    [SerializeField] private float spawnIntervalMin = 2.5f;
    [SerializeField] private float spawnIntervalMax = 4f;

    private float[] laneXPositions = new float[] { -1.35f, 0f, 1.35f };
    private int spawnCount = 0;
    private float nextSpawnTime = 0f;
    private int lastLaneUsed = -1;

    private void Start()
    {
        Debug.Log($"✅ ObstacleSpawner START");
    }

    private void OnEnable()
    {
        Debug.Log($"✅ ObstacleSpawner.OnEnable() - Generazione in partenza");
        nextSpawnTime = Time.time + 1f;
        lastLaneUsed = -1;
    }

    private void OnDisable()
    {
        Debug.Log($"❌ ObstacleSpawner.OnDisable() - Generazione fermata");
    }

    private void Update()
    {
        // Log MASSIVO ogni frame per debuggare
        if (Time.frameCount % 30 == 0)
        {
            Debug.Log($"🔍 ObstacleSpawner.Update() - Time.time={Time.time:F2}, nextSpawnTime={nextSpawnTime:F2}, enabled={enabled}, gameObject.active={gameObject.activeInHierarchy}");
        }

        // Genera un ostacolo alla volta a intervalli regolari
        if (Time.time >= nextSpawnTime)
        {
            Debug.Log($"⏰ TRIGGER SPAWN! Time.time={Time.time:F2} >= nextSpawnTime={nextSpawnTime:F2}");
            
            // Scegli una colonna che NON sia stata usata nell'ultimo spawn
            int selectedLane;
            do
            {
                selectedLane = Random.Range(0, laneXPositions.Length);
            } while (selectedLane == lastLaneUsed && laneXPositions.Length > 1);

            float xPos = laneXPositions[selectedLane];
            SpawnObstacle(xPos, selectedLane);
            
            lastLaneUsed = selectedLane;
            nextSpawnTime = Time.time + Random.Range(spawnIntervalMin, spawnIntervalMax);
            Debug.Log($"📅 Prossimo spawn tra {nextSpawnTime - Time.time:F2}s");
        }
    }

    private void SpawnObstacle(float xPos, int laneIndex)
    {
        if (obstaclePrefab == null)
        {
            Debug.LogError("❌ Obstacle prefab not assigned!");
            return;
        }

        spawnCount++;
        Vector3 spawnPos = new Vector3(xPos, obstacleSpawnY, obstacleSpawnZ);
        GameObject obstacle = Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);
        
        if (spawnParent != null)
        {
            obstacle.transform.parent = spawnParent;
        }

        string laneName = laneIndex == 0 ? "SINISTRA" : (laneIndex == 1 ? "CENTRO" : "DESTRA");
        Debug.Log($"🔴 OSTACOLO #{spawnCount} GENERATO - Lane: {laneName} | Pos: X={xPos:F2}, Y={obstacleSpawnY:F2}, Z={obstacleSpawnZ:F2}");
    }

    public void ClearAllObstacles()
    {
        FallingObstacleController[] allObstacles = FindObjectsByType<FallingObstacleController>(FindObjectsSortMode.None);
        foreach (FallingObstacleController obstacle in allObstacles)
        {
            Destroy(obstacle.gameObject);
        }
        
        spawnCount = 0;
        nextSpawnTime = Time.time + 1f;
        lastLaneUsed = -1;
        Debug.Log($"🗑️ TUTTI GLI OSTACOLI ELIMINATI - Reset spawner");
    }
}
