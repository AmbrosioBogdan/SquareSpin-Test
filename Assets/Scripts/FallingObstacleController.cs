using UnityEngine;

public class FallingObstacleController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeedZ = 5f;      // Velocità di movimento su Z

    [Header("Fall Settings")]
    [SerializeField] private float minHeightOnTrack = 0.65f;  // Altezza minima mentre è sulla pista (Z > 0)
    [SerializeField] private float destructionHeight = -10f;   // Altezza sotto la quale viene distrutto

    private Rigidbody rb;
    private bool hasLeftTrack = false; // Flag per sapere se è uscito dalla pista
    private bool hasHitPlayer = false; // Flag per sapere se ha colpito il player

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Assicura che l'ostacolo cada solo verticalmente (Y), bloccato su X e Z inizialmente
            rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotation;
        }

        Debug.Log($"🔴 OSTACOLO SPAWNED - Inizio movimento - Pos: X={transform.position.x:F2}, Y={transform.position.y:F2}, Z={transform.position.z:F2}");
    }

    private void FixedUpdate()
    {
        if (rb == null) return;

        Vector3 velocity = rb.linearVelocity;

        // Se è ancora sulla pista (Z > 0), muove verso -1
        if (transform.position.z > 0f && !hasLeftTrack)
        {
            // Movimento su Z: accelera verso Z negativo
            velocity.z = -moveSpeedZ;

            // Non può andare sotto minHeightOnTrack
            if (transform.position.y < minHeightOnTrack)
            {
                Vector3 pos = transform.position;
                pos.y = minHeightOnTrack;
                transform.position = pos;

                // Azzera velocità Y se negativa
                if (velocity.y < 0f) velocity.y = 0f;
            }
        }
        // Se è uscito dalla pista (Z < 0), inizia a cadere liberamente
        else if (transform.position.z <= 0f && !hasLeftTrack)
        {
            hasLeftTrack = true;
            // Sblocca il movimento su Z per la caduta libera
            rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotation;
            Debug.Log($"🔴 {gameObject.name} è uscito dalla pista a Z={transform.position.z:F2}, inizia a cadere liberamente!");
            velocity.z = 0f; // Ferma il movimento su Z
        }

        rb.linearVelocity = velocity;
    }

    private void Update()
    {
        // Se è uscito dalla pista e scende troppo, distruggi
        if (hasLeftTrack && transform.position.y < destructionHeight)
        {
            Debug.Log($"🔴 Ostacolo eliminato a Y={transform.position.y:F2}, Z={transform.position.z:F2}");
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter(Collider collision)
    {
        // Se colpisce il player
        if (collision.CompareTag("Player") || collision.GetComponent<PlayerCubeController>() != null)
        {
            if (!hasHitPlayer)
            {
                hasHitPlayer = true;
                
                // Ferma completamente l'ostacolo
                rb.linearVelocity = Vector3.zero;
                rb.constraints = RigidbodyConstraints.FreezeAll;
                
                // Notifica il player
                PlayerCubeController player = collision.GetComponent<PlayerCubeController>();
                if (player != null)
                {
                    player.OnObstacleHit(this);
                }
                
                Debug.Log($"💥 COLLISION! Obstacle ha colpito il Player e si è fermato");
            }
        }
    }
}
