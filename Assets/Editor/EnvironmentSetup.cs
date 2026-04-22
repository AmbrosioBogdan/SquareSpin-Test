using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

public class EnvironmentSetup
{
    [MenuItem("Square Spin/Setup/Configure Camera & Environment")]
    public static void SetupEnvironment()
    {
        // Configurazione camera
        SetupCamera();
        
        // Configurazione ambiente (sfondo nero + particelle)
        CreateEnvironment();
        
        // Aggiungi collider ai prefab
        AddCollidersToTrackPrefab();
        AddCollidersToPlayerPrefab();
        
        Debug.Log("✓ Environment setup complete!");
    }

    private static void SetupCamera()
    {
        Camera mainCamera = Object.FindFirstObjectByType<Camera>();
        if (mainCamera == null)
        {
            Debug.LogError("No camera found in scene!");
            return;
        }

        // Posiziona la camera più vicino alla pista per una vista migliore
        mainCamera.transform.position = new Vector3(0, 12, -0.3f);
        mainCamera.transform.rotation = Quaternion.Euler(41, 0, 0);
        mainCamera.fieldOfView = 50f;

        // Background nero
        mainCamera.backgroundColor = new Color(0, 0, 0, 1);
        mainCamera.clearFlags = CameraClearFlags.Color;

        Debug.Log("✓ Camera configured");
    }

    private static void CreateEnvironment()
    {
        // Crea object per l'ambiente
        GameObject environment = new GameObject("Environment");
        
        // Skybox nero (se non esiste)
        RenderSettings.fog = false;
        RenderSettings.ambientLight = new Color(0.2f, 0.2f, 0.2f);
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;

        // Crea particelle luminescenti
        CreateFloatingParticles(environment);

        // Crea una luce direzionale per illuminare la pista
        CreateMainLight();

        Debug.Log("✓ Environment created");
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

        // Shape (sfera grande intorno alla pista)
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

        // Colore e glow blu/cyan
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

        // Size over lifetime (fade)
        var sizeModule = ps.sizeOverLifetime;
        sizeModule.enabled = true;
        sizeModule.size = new ParticleSystem.MinMaxCurve(0.1f, new AnimationCurve(
            new Keyframe(0, 1),
            new Keyframe(1, 0)
        ));

        Debug.Log("✓ Floating particles created");
    }

    private static void CreateMainLight()
    {
        GameObject lightObj = new GameObject("MainLight");
        Light light = lightObj.AddComponent<Light>();
        
        light.type = LightType.Directional;
        light.intensity = 1.5f;
        light.color = new Color(0.8f, 0.9f, 1.0f); // Blu freddo
        
        lightObj.transform.rotation = Quaternion.Euler(45, 45, 0);

        Debug.Log("✓ Main light created");
    }

    private static Material GetParticleMaterial()
    {
        // Crea un materiale semplice per le particelle
        Shader shader = Shader.Find("Particles/Standard Unlit");
        Material mat = new Material(shader);
        mat.SetColor("_Color", new Color(0.3f, 0.8f, 1.0f, 0.5f));
        return mat;
    }

    private static void AddCollidersToTrackPrefab()
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Track/TrackSegment_01.prefab");
        if (prefab == null)
        {
            Debug.LogError("Track prefab not found!");
            return;
        }

        GameObject editingPrefab = PrefabUtility.LoadPrefabContents("Assets/Prefabs/Track/TrackSegment_01.prefab");

        // Aggiungi BoxCollider al Floor_Base
        Transform floor = editingPrefab.transform.Find("Floor_Base");
        if (floor != null && floor.GetComponent<Collider>() == null)
        {
            BoxCollider floorCollider = floor.gameObject.AddComponent<BoxCollider>();
            floorCollider.isTrigger = false;
        }

        // Aggiungi BoxCollider ai bordi
        Transform borderLeft = editingPrefab.transform.Find("Border_Left");
        if (borderLeft != null && borderLeft.GetComponent<Collider>() == null)
        {
            BoxCollider borderCollider = borderLeft.gameObject.AddComponent<BoxCollider>();
            borderCollider.isTrigger = false;
        }

        Transform borderRight = editingPrefab.transform.Find("Border_Right");
        if (borderRight != null && borderRight.GetComponent<Collider>() == null)
        {
            BoxCollider borderCollider = borderRight.gameObject.AddComponent<BoxCollider>();
            borderCollider.isTrigger = false;
        }

        PrefabUtility.SaveAsPrefabAsset(editingPrefab, "Assets/Prefabs/Track/TrackSegment_01.prefab");
        PrefabUtility.UnloadPrefabContents(editingPrefab);

        Debug.Log("✓ Colliders added to track prefab");
    }

    private static void AddCollidersToPlayerPrefab()
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Player/PlayerCube.prefab");
        if (prefab == null)
        {
            Debug.LogError("Player prefab not found!");
            return;
        }

        GameObject editingPrefab = PrefabUtility.LoadPrefabContents("Assets/Prefabs/Player/PlayerCube.prefab");

        // Il player dovrebbe già avere BoxCollider, assicurati che sia corretto
        BoxCollider collider = editingPrefab.GetComponent<BoxCollider>();
        if (collider == null)
        {
            collider = editingPrefab.AddComponent<BoxCollider>();
        }
        collider.isTrigger = false;
        collider.size = Vector3.one;

        // Assicura che il Rigidbody sia impostato correttamente
        Rigidbody rb = editingPrefab.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            rb.mass = 1f;
            rb.linearDamping = 0;
            rb.angularDamping = 0;
        }

        PrefabUtility.SaveAsPrefabAsset(editingPrefab, "Assets/Prefabs/Player/PlayerCube.prefab");
        PrefabUtility.UnloadPrefabContents(editingPrefab);

        Debug.Log("✓ Colliders configured for player prefab");
    }
}
