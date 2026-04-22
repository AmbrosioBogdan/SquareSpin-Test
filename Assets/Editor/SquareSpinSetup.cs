using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

/// <summary>
/// SETUP PRINCIPALE - Usa questo per configurare il gioco completo
/// Questo è l'UNICO file che hai bisogno di usare per il setup iniziale
/// </summary>
public class SquareSpinSetup
{
    [MenuItem("Square Spin/🎮 SETUP GIOCO COMPLETO")]
    public static void SetupCompleteGame()
    {
        Debug.Log("========================================");
        Debug.Log("  SQUARE SPIN - SETUP COMPLETO");
        Debug.Log("========================================");

        // Step 1: Pulizia
        Debug.Log("\n[1/5] Pulizia della scena...");
        CleanupScene();

        // Step 2: Crea materiali e prefab
        Debug.Log("[2/5] Creazione materiali e prefab...");
        CreateTrackAssets();
        CreatePlayerAssets();

        // Step 3: Setup del gioco
        Debug.Log("[3/5] Setup del GameManager...");
        SetupGameManager();

        // Step 4: Setup ambiente
        Debug.Log("[4/5] Setup della camera e ambiente...");
        SetupEnvironmentAndCamera();

        // Step 5: Ground plane
        Debug.Log("[5/5] Creazione del ground plane...");
        CreateGroundPlane();

        // Salva
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        AssetDatabase.Refresh();

        Debug.Log("\n========================================");
        Debug.Log("✅ SETUP COMPLETO!");
        Debug.Log("========================================");
        Debug.Log("\nIstruzioni:");
        Debug.Log("1. Premi PLAY nel editor");
        Debug.Log("2. Premi SPACE per iniziare a giocare");
        Debug.Log("3. Usa A/D o frecce per muoverti tra le corsie");
        Debug.Log("4. Premi R per resettare");
        Debug.Log("\n");
    }

    private static void CleanupScene()
    {
        // Elimina oggetti vecchi
        GameObject gm = GameObject.Find("GameManager");
        if (gm != null) Object.DestroyImmediate(gm);

        GameObject env = GameObject.Find("Environment");
        if (env != null) Object.DestroyImmediate(env);

        GameObject ground = GameObject.Find("GroundPlane");
        if (ground != null) Object.DestroyImmediate(ground);

        GameObject player = GameObject.Find("Player");
        if (player != null) Object.DestroyImmediate(player);

        for (int i = 0; i < 10; i++)
        {
            GameObject track = GameObject.Find($"TrackSegment_{i}");
            if (track != null) Object.DestroyImmediate(track);
        }
    }

    private static void CreateTrackAssets()
    {
        // Usa gli script di generazione
        string trackScript = AssetDatabase.FindAssets("TrackGenerator")[0];
        string trackScriptPath = AssetDatabase.GUIDToAssetPath(trackScript);
        
        // Verifica se il prefab esiste già
        GameObject trackPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Track/TrackSegment_01.prefab");
        
        if (trackPrefab == null)
        {
            Debug.Log("Generando materiali e prefab della pista...");
            TrackGenerator.GenerateTrackAssets();
        }
    }

    private static void CreatePlayerAssets()
    {
        // Verifica se il prefab esiste già
        GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Player/PlayerCube.prefab");
        
        if (playerPrefab == null)
        {
            Debug.Log("Generando materiali e prefab del player...");
            PlayerCubeGenerator.GeneratePlayerCube();
        }
    }

    private static void SetupGameManager()
    {
        // Crea GameManager
        GameObject gameManagerObj = new GameObject("GameManager");
        GameManager gameManager = gameManagerObj.AddComponent<GameManager>();
        gameManager.gameObject.AddComponent<GameStateManager>();

        // Carica prefab
        GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Player/PlayerCube.prefab");
        GameObject trackPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Track/TrackSegment_01.prefab");

        // Assegna tramite reflection
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
        // Setup camera
        Camera mainCamera = Object.FindFirstObjectByType<Camera>();
        if (mainCamera != null)
        {
            mainCamera.transform.position = new Vector3(0, 12, -0.3f);
            mainCamera.transform.rotation = Quaternion.Euler(41, 0, 0);
            mainCamera.fieldOfView = 50f;
            mainCamera.backgroundColor = new Color(0, 0, 0, 1);
            mainCamera.clearFlags = CameraClearFlags.Color;
        }

        // Setup render settings (nero + luce ambiente minima)
        RenderSettings.fog = false;
        RenderSettings.ambientLight = new Color(0.2f, 0.2f, 0.2f);
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;

        // Crea ambiente e particelle
        GameObject environment = new GameObject("Environment");

        // Particelle luminescenti
        CreateFloatingParticles(environment);

        // Luce principale
        CreateMainLight();
    }

    private static void CreateFloatingParticles(GameObject parent)
    {
        GameObject particleObj = new GameObject("FloatingParticles");
        particleObj.transform.parent = parent.transform;
        particleObj.transform.position = Vector3.zero;

        ParticleSystem ps = particleObj.AddComponent<ParticleSystem>();

        // Main module
        var mainModule = ps.main;
        mainModule.duration = 10f;
        mainModule.loop = true;
        mainModule.startLifetime = 5f;
        mainModule.startSpeed = new ParticleSystem.MinMaxCurve(0.2f, 0.5f);

        // Emission
        var emissionModule = ps.emission;
        emissionModule.rateOverTime = 10f;

        // Shape
        var shapeModule = ps.shape;
        shapeModule.shapeType = ParticleSystemShapeType.Sphere;
        shapeModule.radius = 20f;

        // Renderer
        var rendererModule = ps.GetComponent<ParticleSystemRenderer>();
        if (rendererModule != null)
        {
            rendererModule.renderMode = ParticleSystemRenderMode.Billboard;
            rendererModule.material = GetParticleMaterial();
        }

        // Colore
        var colorModule = ps.colorOverLifetime;
        colorModule.enabled = true;
        colorModule.color = new ParticleSystem.MinMaxGradient(
            new Gradient()
            {
                colorKeys = new GradientColorKey[] 
                {
                    new GradientColorKey(new Color(0.2f, 0.6f, 1.0f, 1f), 0),
                    new GradientColorKey(new Color(0.3f, 0.8f, 1.0f, 0.5f), 1f)
                },
                alphaKeys = new GradientAlphaKey[]
                {
                    new GradientAlphaKey(0.5f, 0),
                    new GradientAlphaKey(0, 1f)
                }
            }
        );

        // Size over lifetime
        var sizeModule = ps.sizeOverLifetime;
        sizeModule.enabled = true;
        sizeModule.size = new ParticleSystem.MinMaxCurve(0.1f, new AnimationCurve(
            new Keyframe(0, 1),
            new Keyframe(1, 0)
        ));
    }

    private static void CreateMainLight()
    {
        GameObject lightObj = new GameObject("MainLight");
        Light light = lightObj.AddComponent<Light>();
        
        light.type = LightType.Directional;
        light.intensity = 1.5f;
        light.color = new Color(0.8f, 0.9f, 1.0f);
        
        lightObj.transform.rotation = Quaternion.Euler(45, 45, 0);
    }

    private static Material GetParticleMaterial()
    {
        Shader shader = Shader.Find("Particles/Standard Unlit");
        Material mat = new Material(shader);
        mat.SetColor("_Color", new Color(0.3f, 0.8f, 1.0f, 0.5f));
        return mat;
    }

    private static void CreateGroundPlane()
    {
        GameObject groundPlane = new GameObject("GroundPlane");
        
        BoxCollider collider = groundPlane.AddComponent<BoxCollider>();
        collider.size = new Vector3(50, 0.5f, 100);
        collider.isTrigger = false;
        
        groundPlane.transform.position = new Vector3(0, -1, 0);
    }
}
