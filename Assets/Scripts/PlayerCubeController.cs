using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCubeController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float laneChangeSpeed = 5f;

    [Header("Lane System")]
    [SerializeField] private int currentLane = 1; // Inizio al centro

    [Header("Glow Animation")]
    [SerializeField] private bool animateGlow = true;
    [SerializeField] private float glowPulseSpeed = 3f;

    private float[] lanePositions; // Inizializzato da LaneConfiguration
    private Rigidbody rb;
    private Material[] glowMaterials;
    private Color[] originalEmissionColors;
    private float targetXPosition;

    [Header("Obstacle Avoidance")]
    [SerializeField] private float obstacleDetectionDistance = 40f; // Distanza sufficiente per rilevare ostacoli in arrivo
    [SerializeField] private float detectionWidth = 4f; // Larghezza totale per coprire tutte le corsie

    private bool isGameOver = false;
    private FallingObstacleController collidingObstacle = null;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Blocca rotazione e movimento su Z
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;

        // Ottieni le posizioni delle corsie da LaneConfiguration
        LaneConfiguration laneConfig = LaneConfiguration.Instance;
        if (laneConfig != null)
        {
            lanePositions = laneConfig.GetLaneXPositions();
            Debug.Log($"Lane positions loaded: {lanePositions[0]}, {lanePositions[1]}, {lanePositions[2]}");
        }
        else
        {
            Debug.LogWarning("LaneConfiguration not found, using default lane positions");
            lanePositions = new float[] { -0.7f, 0f, 0.7f };
        }

        FindGlowMaterials();
        targetXPosition = lanePositions[currentLane];
        
        // LOG: Posizione iniziale
        Debug.Log($"🎮 PLAYER SPAWNED at Position: X={transform.position.x:F2}, Y={transform.position.y:F2}, Z={transform.position.z:F2}");
    }

    private void FindGlowMaterials()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        System.Collections.Generic.List<Material> glowList = new System.Collections.Generic.List<Material>();
        System.Collections.Generic.List<Color> colorList = new System.Collections.Generic.List<Color>();

        foreach (Renderer renderer in renderers)
        {
            foreach (Material mat in renderer.materials)
            {
                if (mat.HasProperty("_EmissionColor"))
                {
                    Color emissionColor = mat.GetColor("_EmissionColor");
                    if (emissionColor.r + emissionColor.g + emissionColor.b > 0.1f)
                    {
                        glowList.Add(mat);
                        colorList.Add(emissionColor);
                    }
                }
            }
        }

        glowMaterials = glowList.ToArray();
        originalEmissionColors = colorList.ToArray();
    }

    private void Update()
    {
        if (!isGameOver)
        {
            CheckForObstaclesAndEvade();
        }
        UpdateGlow();
    }

    private void CheckForObstaclesAndEvade()
    {
        // Centro della box di rilevamento davanti al player
        // Posizionato a metà della distanza di rilevamento per coprire meglio gli ostacoli in arrivo
        Vector3 detectionBoxCenter = transform.position + Vector3.forward * (obstacleDetectionDistance * 0.75f);
        
        // OverlapBox prende halfExtents, non la size intera!
        Vector3 halfExtents = new Vector3(detectionWidth / 2f, 1.5f, obstacleDetectionDistance * 0.5f);

        Collider[] obstacles = Physics.OverlapBox(detectionBoxCenter, halfExtents);

        if (Time.frameCount % 30 == 0)
        {
            Debug.Log($"🔍 CheckForObstaclesAndEvade - Rilevati {obstacles.Length} collider, Player Lane={currentLane} X={transform.position.x:F2}");
            Debug.Log($"📦 Detection Box - Center: Z={detectionBoxCenter.z:F2}, Range Z=[{detectionBoxCenter.z - halfExtents.z:F2}, {detectionBoxCenter.z + halfExtents.z:F2}]");
        }
        
        // Disegna la detection box per debugging
        DrawDetectionBox(detectionBoxCenter, halfExtents);

        // Analizza gli ostacoli per ogni corsia
        bool[] laneHasDanger = new bool[lanePositions.Length];
        float laneXTolerance = 0.7f; // Tolleranza per considerare un ostacolo sulla stessa corsia
        
        foreach (Collider col in obstacles)
        {
            // Cerca FallingObstacleController prima sul GameObject corrente, poi nei genitori
            FallingObstacleController obstacleController = col.GetComponent<FallingObstacleController>();
            if (obstacleController == null)
            {
                obstacleController = col.GetComponentInParent<FallingObstacleController>();
            }
            
            if (obstacleController != null)
            {
                float obstacleX = col.transform.position.x;
                
                Debug.Log($"📍 Ostacolo rilevato - X={obstacleX:F2}, Z={col.transform.position.z:F2}, Collider tag: {col.gameObject.name}");
                
                // Verifica quale corsia ha il pericolo
                for (int i = 0; i < lanePositions.Length; i++)
                {
                    float distToLaneCenter = Mathf.Abs(lanePositions[i] - obstacleX);
                    if (distToLaneCenter < laneXTolerance)
                    {
                        laneHasDanger[i] = true;
                        Debug.Log($"⚠️ Pericolo rilevato sulla corsia {i} (Lane X={lanePositions[i]:F2}, Ostacolo X={obstacleX:F2}, dist={distToLaneCenter:F2})");
                    }
                }
            }
        }

        // Se la corsia attuale ha pericolo, cerca una corsia sicura
        if (laneHasDanger[currentLane])
        {
            Debug.Log($"🚨 PERICOLO SULLA CORSIA ATTUALE {currentLane}!");
            
            // Trova la corsia più sicura (senza ostacoli)
            int safeLane = -1;
            for (int i = 0; i < lanePositions.Length; i++)
            {
                if (!laneHasDanger[i])
                {
                    safeLane = i;
                    Debug.Log($"✅ Corsia sicura trovata: {i}");
                    break;
                }
            }
            
            // Se non c'è una corsia completamente sicura, scegli la più vicina alternativa
            if (safeLane == -1)
            {
                safeLane = currentLane == 0 ? 1 : (currentLane == 2 ? 1 : (currentLane == 1 ? 0 : 2));
                Debug.Log($"⚠️ Nessuna corsia sicura! Scelgo alternativa: {safeLane}");
            }
            
            // Cambia verso la corsia sicura
            if (safeLane < currentLane)
            {
                OnInputLeft();
                Debug.Log($"➡️ EVADE A SINISTRA verso Lane {safeLane}");
            }
            else if (safeLane > currentLane)
            {
                OnInputRight();
                Debug.Log($"⬅️ EVADE A DESTRA verso Lane {safeLane}");
            }
        }
    }

    public void OnInputLeft()
    {
        if (currentLane > 0) currentLane--;
        targetXPosition = lanePositions[currentLane];
        Debug.Log($"🎮 SPOSTAMENTO A SINISTRA → Lane {currentLane} (X={targetXPosition:F2})");
    }

    public void OnInputRight()
    {
        if (currentLane < lanePositions.Length - 1) currentLane++;
        targetXPosition = lanePositions[currentLane];
        Debug.Log($"🎮 SPOSTAMENTO A DESTRA → Lane {currentLane} (X={targetXPosition:F2})");
    }

    private void FixedUpdate()
    {
        // Movimento su X verso targetXPosition
        Vector3 velocity = rb.linearVelocity;
        
        float distanceToTarget = targetXPosition - transform.position.x;
        float desiredVelocityX = distanceToTarget * laneChangeSpeed;
        desiredVelocityX = Mathf.Clamp(desiredVelocityX, -2f, 2f);
        
        velocity.x = desiredVelocityX;
        
        // Se è abbastanza vicino al target, snappa sulla posizione esatta
        if (Mathf.Abs(distanceToTarget) < 0.15f)
        {
            Vector3 pos = transform.position;
            pos.x = targetXPosition; // SNAP alla posizione esatta
            transform.position = pos;
            velocity.x = 0f;
        }
        
        rb.linearVelocity = velocity;
        
        // Limita Y tra 1.5 (minimo) e 0.5 (massimo)
        if (transform.position.y > 0.5f)
        {
            Vector3 pos = transform.position;
            pos.y = 0.5f;
            transform.position = pos;
            
            // Azzera la velocità Y se positiva (evita rimbalzi)
            velocity = rb.linearVelocity;
            if (velocity.y > 0f) velocity.y = 0f;
            rb.linearVelocity = velocity;
        }
        
        if (transform.position.y < 1f)
        {
            Vector3 pos = transform.position;
            pos.y = 0.65f;
            transform.position = pos;
            
            // Azzera la velocità Y se negativa (evita continue cadute)
            velocity = rb.linearVelocity;
            if (velocity.y < 0f) velocity.y = 0f;
            rb.linearVelocity = velocity;
        }
    }

    private void UpdateGlow()
    {
        if (animateGlow && glowMaterials != null)
        {
            float pulse = Mathf.Lerp(1.5f, 2.5f, (Mathf.Sin(Time.time * glowPulseSpeed) + 1f) * 0.5f);

            for (int i = 0; i < glowMaterials.Length; i++)
            {
                if (glowMaterials[i] != null)
                {
                    glowMaterials[i].SetColor("_EmissionColor", originalEmissionColors[i] * pulse);
                }
            }
        }
    }

    private void DrawDetectionBox(Vector3 center, Vector3 halfExtents)
    {
        // Disegna un cubo di debug usando linee
        Vector3[] corners = new Vector3[8];
        
        // Calcola i 8 vertici del cubo
        corners[0] = center + new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z);
        corners[1] = center + new Vector3(halfExtents.x, -halfExtents.y, -halfExtents.z);
        corners[2] = center + new Vector3(halfExtents.x, halfExtents.y, -halfExtents.z);
        corners[3] = center + new Vector3(-halfExtents.x, halfExtents.y, -halfExtents.z);
        corners[4] = center + new Vector3(-halfExtents.x, -halfExtents.y, halfExtents.z);
        corners[5] = center + new Vector3(halfExtents.x, -halfExtents.y, halfExtents.z);
        corners[6] = center + new Vector3(halfExtents.x, halfExtents.y, halfExtents.z);
        corners[7] = center + new Vector3(-halfExtents.x, halfExtents.y, halfExtents.z);

        // Disegna i bordi del cubo
        Color debugColor = Color.yellow;
        
        // Bottom face
        Debug.DrawLine(corners[0], corners[1], debugColor);
        Debug.DrawLine(corners[1], corners[2], debugColor);
        Debug.DrawLine(corners[2], corners[3], debugColor);
        Debug.DrawLine(corners[3], corners[0], debugColor);
        
        // Top face
        Debug.DrawLine(corners[4], corners[5], debugColor);
        Debug.DrawLine(corners[5], corners[6], debugColor);
        Debug.DrawLine(corners[6], corners[7], debugColor);
        Debug.DrawLine(corners[7], corners[4], debugColor);
        
        // Vertical edges
        Debug.DrawLine(corners[0], corners[4], debugColor);
        Debug.DrawLine(corners[1], corners[5], debugColor);
        Debug.DrawLine(corners[2], corners[6], debugColor);
        Debug.DrawLine(corners[3], corners[7], debugColor);
    }

    public void OnObstacleHit(FallingObstacleController obstacle)
    {
        isGameOver = true;
        collidingObstacle = obstacle;
        Debug.Log($"💥 GAME OVER! Player colpito da ostacolo");
        
        // Notifica il GameStateManager
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.SetGameState(GameStateManager.GameState.GameOver);
        }
    }

    public bool IsGameOver() => isGameOver;

    public void ResetGameOver()
    {
        isGameOver = false;
        collidingObstacle = null;
        currentLane = 1; // Torna alla corsia centrale
        targetXPosition = lanePositions[currentLane];
        Debug.Log("🎮 Player reset - Pronto per giocare di nuovo!");
    }

    public int GetCurrentLane() => currentLane;
    public float GetMoveSpeed() => moveSpeed;
    public void SetMoveSpeed(float speed) => moveSpeed = speed;
}
