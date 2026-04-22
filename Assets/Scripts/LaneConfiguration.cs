using UnityEngine;

/// <summary>
/// Configurazione centralizzata del sistema di corsie
/// Definisce i centri di ogni corsia e i parametri della pista
/// </summary>
public class LaneConfiguration : MonoBehaviour
{
    public static LaneConfiguration Instance { get; private set; }

    [Header("Lane Dimensions")]
    [SerializeField] private float laneWidth = 2.0f; // Larghezza di ogni corsia (cubo è 1.0f, quindi spazio sufficiente)
    [SerializeField] private float laneDividerWidth = 0.1f; // Larghezza dei divisori tra corsie

    [Header("Lane Positions (Manual)")]
    [SerializeField] private float laneLeftX = -1.35f;  // Posizione colonna sinistra
    [SerializeField] private float laneCenterX = 0f;   // Posizione colonna centro
    [SerializeField] private float laneRightX = 1.35f;  // Posizione colonna destra

    [Header("Player Spawn")]
    [SerializeField] private float playerSpawnX = 0f; // Centro
    [SerializeField] private float playerSpawnZ = 5.8f; // Prima posizione lungo la pista
    [SerializeField] private float playerSpawnY = 3f; // Altezza iniziale (gravità decide il resto)

    [Header("Track")]
    [SerializeField] private float trackSegmentLength = 10f; // Lunghezza di ogni segmento della pista
    [SerializeField] private float firstTrackZ = 10f; // Prima Z della pista (prima dello spawn)

    private float[] laneXPositions; // Centro X di ogni corsia

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        CalculateLanePositions();
    }

    private void CalculateLanePositions()
    {
        // Usa le posizioni definite manualmente nell'Inspector
        laneXPositions = new float[3]
        {
            laneLeftX,    // Sinistra
            laneCenterX,  // Centro
            laneRightX    // Destra
        };

        float totalWidth = (laneWidth * 3) + (laneDividerWidth * 2);
        Debug.Log($"Lane Centers: Left={laneXPositions[0]}, Center={laneXPositions[1]}, Right={laneXPositions[2]}");
        Debug.Log($"Total Track Width: {totalWidth}");
    }

    public float GetPlayerSpawnX() => playerSpawnX;
    public float GetPlayerSpawnY() => playerSpawnY;
    public float GetPlayerSpawnZ() => playerSpawnZ;

    public float[] GetLaneXPositions() => laneXPositions;
    public float GetLaneWidth() => laneWidth;
    public float GetTrackSegmentLength() => trackSegmentLength;
    public float GetFirstTrackZ() => firstTrackZ;

    /// <summary>
    /// Calcola la Z di spawn per un ostacolo casuale sulla pista
    /// </summary>
    public float GetRandomObstacleSpawnZ()
    {
        // Gli ostacoli spawnan da Z = firstTrackZ in poi, fino a firstTrackZ + (5 segmenti * lunghezza)
        // Per esempio: 10, 20, 30, 40, 50, 60...
        int randomSegment = Random.Range(1, 6); // Segmenti 1-5
        return firstTrackZ + (randomSegment * trackSegmentLength);
    }
}
