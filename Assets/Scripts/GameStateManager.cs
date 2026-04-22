using UnityEngine;
using UnityEngine.InputSystem;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public enum GameState { Idle, Playing, GameOver }
    
    [SerializeField] private GameState currentState = GameState.Idle;
    
    [Header("Auto Start")]
    [SerializeField] private float autoStartDelay = 5f; // Avvio automatico dopo 5 secondi
    private float idleTimer = 0f;
    
    private PlayerCubeController playerController;
    private PlayerAIController playerAIController;
    private Rigidbody playerRigidbody;
    private ObstacleSpawner obstacleSpawner;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        GameManager gameManager = GetComponent<GameManager>();
        if (gameManager != null)
        {
            GameObject player = gameManager.GetPlayer();
            if (player != null)
            {
                playerController = player.GetComponent<PlayerCubeController>();
                playerAIController = player.GetComponent<PlayerAIController>();
                playerRigidbody = player.GetComponent<Rigidbody>();
            }
        }

        // Trova l'ObstacleSpawner nella scena
        obstacleSpawner = FindFirstObjectByType<ObstacleSpawner>();
        
        if (obstacleSpawner == null)
        {
            Debug.LogError("❌ ObstacleSpawner NOT FOUND in scene!");
        }
        else
        {
            Debug.Log($"✅ ObstacleSpawner found: {obstacleSpawner.gameObject.name}");
        }

        // Stato iniziale: tutto fermo
        SetGameState(GameState.Idle);
    }

    private void Update()
    {
        // Usa il nuovo Input System
        if (Keyboard.current == null)
        {
            Debug.LogWarning("⚠️ Keyboard.current is NULL!");
            return;
        }
        
        // DEBUG: Log che Update è stato chiamato
        if (Time.frameCount % 120 == 0)
        {
            Debug.Log($"🔄 GameStateManager.Update() - currentState={currentState}");
        }
        
        // Auto-start dopo 5 secondi in stato Idle
        if (currentState == GameState.Idle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= autoStartDelay)
            {
                Debug.Log($"⏱️ AUTO-START dopo {autoStartDelay} secondi - Passando a Playing");
                SetGameState(GameState.Playing);
                idleTimer = 0f; // Reset timer
                return;
            }
        }
        
        // Premi SPACE per iniziare il gioco (anticipare l'auto-start)
        if (currentState == GameState.Idle && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Debug.Log($"🎮 SPACE PREMUTO - Passando da Idle a Playing");
            SetGameState(GameState.Playing);
            idleTimer = 0f; // Reset timer
        }

        // Premi R per resettare (da qualsiasi stato)
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            Debug.Log($"🔄 R PREMUTO - Reset game");
            ResetGame();
        }
    }

    public void SetGameState(GameState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case GameState.Idle:
                PauseGame();
                break;
            case GameState.Playing:
                ResumeGame();
                break;
            case GameState.GameOver:
                PauseGame();
                // Auto-reset dopo game over
                Invoke(nameof(ResetGame), 2f);
                break;
        }

        Debug.Log($"Game state changed to: {currentState}");
    }

    private void PauseGame()
    {
        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector3.zero;
            playerRigidbody.isKinematic = false;
        }

        if (playerController != null)
        {
            playerController.enabled = false;
        }

        if (playerAIController != null)
        {
            playerAIController.enabled = false;
        }

        if (obstacleSpawner != null)
        {
            obstacleSpawner.enabled = false;
        }

        // Non pausiamo il tempo nel Idle, così le particelle continuano a muoversi
    }

    private void ResumeGame()
    {
        if (playerRigidbody != null)
        {
            // Congela l'asse Z per impedire al cubo di scivolare in avanti
            playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
            playerRigidbody.linearVelocity = Vector3.zero;
        }

        if (playerController != null)
        {
            playerController.enabled = true;
        }

        if (playerAIController != null)
        {
            playerAIController.enabled = true;
        }

        if (obstacleSpawner != null)
        {
            obstacleSpawner.enabled = true;
            Debug.Log($"✅ ObstacleSpawner ABILITATO dal GameStateManager");
        }
        else
        {
            Debug.LogError("❌ ObstacleSpawner è NULL nel GameStateManager!");
        }

        // Tempo già in esecuzione
    }

    public GameState GetCurrentState() => currentState;
    public bool IsPlaying() => currentState == GameState.Playing;
    public bool IsIdle() => currentState == GameState.Idle;

    public void ResetGame()
    {
        // Pulisci tutti gli ostacoli
        if (obstacleSpawner != null)
        {
            obstacleSpawner.ClearAllObstacles();
        }

        // Reimposta la posizione del player
        if (playerRigidbody != null && playerController != null)
        {
            // Torna alla posizione iniziale (Lane 1, centro)
            Vector3 resetPos = new Vector3(0f, 0.65f, 5.8f);
            playerRigidbody.MovePosition(resetPos);
            playerRigidbody.linearVelocity = Vector3.zero;
            
            // Resetta il flag di game over
            playerController.ResetGameOver();
        }

        // Ricomincia il gioco
        idleTimer = 0f; // Reset timer per auto-start
        SetGameState(GameState.Playing);
        Debug.Log("🔄 GAME RESET - Ricomincio!");
    }
}
