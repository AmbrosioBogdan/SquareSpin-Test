using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject playerCubePrefab;
    [SerializeField] private GameObject trackSegmentPrefab;

    // Nota: Le posizioni di spawn sono definite in LaneConfiguration
    // [SerializeField] private Vector3 playerSpawnPosition = new Vector3(0, 2f, 0f);

    [Header("Track Setup")]
    [SerializeField] private int trackSegmentsCount = 5;
    [SerializeField] private Vector3 firstTrackPosition = new Vector3(0, 0, 10);

    private GameObject playerInstance;
    private GameObject[] trackInstances;

    private void Awake()
    {
        // Aggiungi GameStateManager se non esiste
        if (GetComponent<GameStateManager>() == null)
        {
            gameObject.AddComponent<GameStateManager>();
        }
    }

    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        // Ottieni la configurazione delle corsie
        LaneConfiguration laneConfig = LaneConfiguration.Instance;
        if (laneConfig == null)
        {
            Debug.LogError("LaneConfiguration not found in scene!");
            return;
        }

        // Crea il player nella posizione corretta
        if (playerCubePrefab != null)
        {
            Vector3 spawnPos = new Vector3(laneConfig.GetPlayerSpawnX(), laneConfig.GetPlayerSpawnY(), laneConfig.GetPlayerSpawnZ());
            playerInstance = Instantiate(playerCubePrefab, spawnPos, Quaternion.identity);
            playerInstance.name = "Player";
            Debug.Log("✓ Player spawned at " + spawnPos);
        }
        else
        {
            Debug.LogError("PlayerCube prefab not assigned!");
        }

        // Crea i segmenti della pista
        if (trackSegmentPrefab != null)
        {
            trackInstances = new GameObject[trackSegmentsCount];
            for (int i = 0; i < trackSegmentsCount; i++)
            {
                Vector3 trackPosition = firstTrackPosition + new Vector3(0, 0, i * laneConfig.GetTrackSegmentLength());
                trackInstances[i] = Instantiate(trackSegmentPrefab, trackPosition, Quaternion.identity);
                trackInstances[i].name = $"TrackSegment_{i}";
            }
            Debug.Log($"✓ Track created with {trackSegmentsCount} segments");
        }
        else
        {
            Debug.LogError("TrackSegment prefab not assigned!");
        }
    }

    // Getter pubblici per accedere agli oggetti
    public GameObject GetPlayer() => playerInstance;
    public GameObject GetTrackSegment(int index) => index >= 0 && index < trackInstances.Length ? trackInstances[index] : null;
    public GameObject[] GetAllTrackSegments() => trackInstances;
    public int GetTrackSegmentCount() => trackSegmentsCount;
}
