using UnityEngine;

public class PlayerAIController : MonoBehaviour
{
    [Header("AI Settings")]
    [SerializeField] private float detectionHeight = 5f; // Altezza per rilevare ostacoli
    [SerializeField] private float detectionDistance = 3f; // Distanza di rilevazione

    [Header("Lane Info")]
    [SerializeField] private int currentLane = 1; // Lane centrale

    private float[] laneXPositions;
    private PlayerCubeController playerController;

    private void Start()
    {
        playerController = GetComponent<PlayerCubeController>();
        if (playerController == null)
        {
            Debug.LogError("PlayerAIController requires PlayerCubeController on the same object!");
            enabled = false;
            return;
        }

        // Ottieni le posizioni delle corsie da LaneConfiguration
        LaneConfiguration laneConfig = LaneConfiguration.Instance;
        if (laneConfig != null)
        {
            laneXPositions = laneConfig.GetLaneXPositions();
            Debug.Log($"AI - Lane positions: {laneXPositions[0]}, {laneXPositions[1]}, {laneXPositions[2]}");
        }
        else
        {
            Debug.LogWarning("LaneConfiguration not found in PlayerAIController!");
            laneXPositions = new float[] { -0.7f, 0f, 0.7f };
        }
    }

    private void Update()
    {
        // TEMPORANEAMENTE DISABILITATO PER DEBUG
        // Controlla ogni reactionTime
        // if (Time.time >= lastReactionTime)
        // {
        //     UpdateAILogic();
        //     lastReactionTime = Time.time + reactionTime;
        // }
    }

    private void UpdateAILogic()
    {
        // Rileva gli ostacoli nelle corsie
        ObstacleData[] obstaclesInLanes = DetectObstacles();

        // Decide quale corsia è più sicura
        int safestLane = FindSafestLane(obstaclesInLanes);

        // Muovi verso quella corsia
        if (safestLane != currentLane)
        {
            MoveToLane(safestLane);
            currentLane = safestLane;
        }
    }

    private ObstacleData[] DetectObstacles()
    {
        ObstacleData[] obstacles = new ObstacleData[laneXPositions.Length];

        for (int i = 0; i < laneXPositions.Length; i++)
        {
            obstacles[i] = new ObstacleData { laneIndex = i, hasObstacle = false, distance = Mathf.Infinity };

            // Raycast verso l'alto per rilevare ostacoli
            Vector3 rayOrigin = new Vector3(laneXPositions[i], transform.position.y + detectionHeight, transform.position.z);
            
            RaycastHit[] hits = Physics.RaycastAll(rayOrigin, Vector3.down, detectionDistance);

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.GetComponent<FallingObstacleController>() != null)
                {
                    obstacles[i].hasObstacle = true;
                    obstacles[i].distance = hit.distance;
                    break;
                }
            }
        }

        return obstacles;
    }

    private int FindSafestLane(ObstacleData[] obstacles)
    {
        int safestLane = currentLane;
        float minDistance = Mathf.Infinity;

        // Preferisci corsie senza ostacoli, poi quelle più lontane
        for (int i = 0; i < obstacles.Length; i++)
        {
            if (!obstacles[i].hasObstacle)
            {
                return i; // Nessun ostacolo, corsia sicura
            }
        }

        // Se tutte hanno ostacoli, scegli quella più distante
        for (int i = 0; i < obstacles.Length; i++)
        {
            if (obstacles[i].distance < minDistance)
            {
                minDistance = obstacles[i].distance;
                safestLane = i;
            }
        }

        return safestLane;
    }

    private void MoveToLane(int laneIndex)
    {
        // Simula input del giocatore
        if (laneIndex < currentLane)
        {
            // Muovi a sinistra
            playerController.OnInputLeft();
        }
        else if (laneIndex > currentLane)
        {
            // Muovi a destra
            playerController.OnInputRight();
        }
    }

    private struct ObstacleData
    {
        public int laneIndex;
        public bool hasObstacle;
        public float distance;
    }
}
