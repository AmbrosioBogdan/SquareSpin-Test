using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

/// <summary>
/// SETUP V2 - Con ostacoli e AI
/// </summary>
public class SquareSpinSetupV2
{
    [MenuItem("Square Spin/🎮 SETUP V2 (Obstacles + AI)")]
    public static void SetupV2Game()
    {
        Debug.Log("========================================");
        Debug.Log("  SQUARE SPIN V2 - SETUP COMPLETO");
        Debug.Log("========================================");

        // Step 1: Pulizia
        Debug.Log("\n[1/7] Pulizia della scena...");
        CleanupScene();

        // Step 2: Genera assets V2
        Debug.Log("[2/7] Creazione materiali e prefab V2...");
        CreateTrackV2Assets();
        CreatePlayerV2Assets();
        CreateObstacleAssets();

        // Step 3: Setup GameManager
        Debug.Log("[3/7] Setup del GameManager...");
        SetupGameManager();

        // Step 4: Setup ambiente
        Debug.Log("[4/7] Setup della camera e ambiente...");
        SetupEnvironmentAndCamera();

        // Step 5: Ground plane
        Debug.Log("[5/7] Creazione del ground plane...");
        CreateGroundPlane();

        // Step 6: Setup Obstacle Spawner
        Debug.Log("[6/7] Setup del sistema di ostacoli...");
        SetupObstacleSpawner();

        // Step 7: Aggiungi AI al player
        Debug.Log("[7/7] Aggiunta dell'AI al player...");
        AddPlayerAI();

        // Salva
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        AssetDatabase.Refresh();

        Debug.Log("\n========================================");
        Debug.Log("✅ SETUP V2 COMPLETO!");
        Debug.Log("========================================");
        Debug.Log("\nIstruzioni:");
        Debug.Log("1. Premi PLAY nel editor");
        Debug.Log("2. L'AI giocherà automaticamente");
        Debug.Log("3. Eviterà gli ostacoli che cadono");
        Debug.Log("4. Premi R per resettare");
        Debug.Log("\n");
    }

    private static void CleanupScene()
    {
        // Elimina tutto
        string[] objectNames = { "GameManager", "Environment", "GroundPlane", "Player", "ObstacleSpawner", "LaneConfiguration" };
        
        for (int i = 0; i < 20; i++)
        {
            GameObject track = GameObject.Find($"TrackSegment_{i}");
            if (track != null) Object.DestroyImmediate(track);
        }

        foreach (string name in objectNames)
        {
            GameObject obj = GameObject.Find(name);
            if (obj != null) Object.DestroyImmediate(obj);
        }

        // Elimina tutti gli ostacoli
        FallingObstacleController[] obstacles = Object.FindObjectsByType<FallingObstacleController>(FindObjectsSortMode.None);
        foreach (var obs in obstacles)
        {
            Object.DestroyImmediate(obs.gameObject);
        }
    }

    private static void CreateTrackV2Assets()
    {
        GameObject trackPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Track/TrackSegment_V2.prefab");
        if (trackPrefab == null)
        {
            Debug.Log("Generando Track V2...");
            TrackGeneratorV2.GenerateTrackV2();
        }
    }

    private static void CreatePlayerV2Assets()
    {
        GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Player/PlayerCubeV2.prefab");
        if (playerPrefab == null)
        {
            Debug.Log("Generando Player Cube V2...");
            PlayerCubeV2Generator.GeneratePlayerCubeV2();
        }
    }

    private static void CreateObstacleAssets()
    {
        GameObject obstaclePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Obstacles/FallingObstacle.prefab");
        if (obstaclePrefab == null)
        {
            Debug.Log("Generando Falling Obstacle...");
            ObstacleGenerator.GenerateFallingObstacle();
        }
    }

    private static void SetupGameManager()
    {
        GameObject gameManagerObj = new GameObject("GameManager");
        GameManager gameManager = gameManagerObj.AddComponent<GameManager>();
        gameManager.gameObject.AddComponent<GameStateManager>();

        // Aggiungi LaneConfiguration
        gameManagerObj.AddComponent<LaneConfiguration>();

        GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Player/PlayerCubeV2.prefab");
        GameObject trackPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Track/TrackSegment_V2.prefab");

        var playerField = typeof(GameManager).GetField("playerCubePrefab", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var trackField = typeof(GameManager).GetField("trackSegmentPrefab", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (playerField != null && playerPrefab != null)
            playerField.SetValue(gameManager, playerPrefab);

        if (trackField != null && trackPrefab != null)
            trackField.SetValue(gameManager, trackPrefab);

        EditorUtility.SetDirty(gameManager);
    }

    private static void SetupEnvironmentAndCamera()
    {
        Camera mainCamera = Object.FindFirstObjectByType<Camera>();
        if (mainCamera != null)
        {
            mainCamera.transform.position = new Vector3(0, 12, -0.3f);
            mainCamera.transform.rotation = Quaternion.Euler(41, 0, 0);
            mainCamera.fieldOfView = 50f;
            mainCamera.backgroundColor = new Color(0, 0, 0, 1);
            mainCamera.clearFlags = CameraClearFlags.Color;
        }

        RenderSettings.fog = false;
        RenderSettings.ambientLight = new Color(0.2f, 0.2f, 0.2f);
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;

        GameObject environment = new GameObject("Environment");

        // Particelle (delegato a EnvironmentSetup, ma per adesso skip)
        GameObject lightObj = new GameObject("MainLight");
        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.5f;
        light.color = new Color(0.8f, 0.9f, 1.0f);
        lightObj.transform.rotation = Quaternion.Euler(45, 45, 0);
    }

    private static void CreateGroundPlane()
    {
        GameObject groundPlane = new GameObject("GroundPlane");
        BoxCollider collider = groundPlane.AddComponent<BoxCollider>();
        collider.size = new Vector3(50, 0.5f, 100);
        collider.isTrigger = false;
        groundPlane.transform.position = new Vector3(0, -1, 0);
    }

    private static void SetupObstacleSpawner()
    {
        GameObject spawnerObj = new GameObject("ObstacleSpawner");
        ObstacleSpawner spawner = spawnerObj.AddComponent<ObstacleSpawner>();

        GameObject obstaclePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Obstacles/FallingObstacle.prefab");
        
        var prefabField = typeof(ObstacleSpawner).GetField("obstaclePrefab", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (prefabField != null && obstaclePrefab != null)
            prefabField.SetValue(spawner, obstaclePrefab);

        EditorUtility.SetDirty(spawner);
    }

    private static void AddPlayerAI()
    {
        // L'AI sarà aggiunto al player quando viene istanziato da GameManager
        // Ma prepariamo il prefab per includerlo
        GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Player/PlayerCubeV2.prefab");
        if (playerPrefab != null)
        {
            GameObject editingPrefab = PrefabUtility.LoadPrefabContents("Assets/Prefabs/Player/PlayerCubeV2.prefab");
            
            if (editingPrefab.GetComponent<PlayerAIController>() == null)
            {
                editingPrefab.AddComponent<PlayerAIController>();
            }

            PrefabUtility.SaveAsPrefabAsset(editingPrefab, "Assets/Prefabs/Player/PlayerCubeV2.prefab");
            PrefabUtility.UnloadPrefabContents(editingPrefab);
        }
    }
}
