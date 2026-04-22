using UnityEngine;
using UnityEditor;

public class ObstacleGenerator
{
    private const string MATERIALS_FOLDER = "Assets/Materials/Obstacles";
    private const string PREFABS_FOLDER = "Assets/Prefabs/Obstacles";

    [MenuItem("Square Spin/Generator/Create Falling Obstacle")]
    public static void GenerateFallingObstacle()
    {
        CreateFolders();
        CreateObstacleMaterials();
        CreateObstaclePrefab();
        AssetDatabase.Refresh();
        Debug.Log("✓ Falling Obstacle created!");
    }

    private static void CreateFolders()
    {
        if (!AssetDatabase.IsValidFolder(MATERIALS_FOLDER))
        {
            AssetDatabase.CreateFolder("Assets", "Materials");
            AssetDatabase.CreateFolder("Assets/Materials", "Obstacles");
        }
        if (!AssetDatabase.IsValidFolder(PREFABS_FOLDER))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
            AssetDatabase.CreateFolder("Assets/Prefabs", "Obstacles");
        }
    }

    private static void CreateObstacleMaterials()
    {
        // Red danger metal
        Material redMetal = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        redMetal.name = "MAT_Obstacle_RedDanger";
        redMetal.SetColor("_BaseColor", new Color(0.8f, 0.1f, 0.1f, 1f));
        redMetal.SetColor("_EmissionColor", new Color(1.0f, 0.2f, 0.2f, 1f) * 2f);
        redMetal.EnableKeyword("_EMISSION");
        redMetal.SetFloat("_Metallic", 0.7f);
        redMetal.SetFloat("_Smoothness", 0.6f);
        AssetDatabase.CreateAsset(redMetal, $"{MATERIALS_FOLDER}/MAT_Obstacle_RedDanger.mat");

        // Dark red edge
        Material darkRed = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        darkRed.name = "MAT_Obstacle_DarkRed";
        darkRed.SetColor("_BaseColor", new Color(0.4f, 0.05f, 0.05f, 1f));
        darkRed.SetFloat("_Metallic", 0.8f);
        darkRed.SetFloat("_Smoothness", 0.5f);
        AssetDatabase.CreateAsset(darkRed, $"{MATERIALS_FOLDER}/MAT_Obstacle_DarkRed.mat");
    }

    private static void CreateObstaclePrefab()
    {
        // Root object
        GameObject obstacleRoot = new GameObject("FallingObstacle");

        Material redMat = AssetDatabase.LoadAssetAtPath<Material>($"{MATERIALS_FOLDER}/MAT_Obstacle_RedDanger.mat");
        Material darkRedMat = AssetDatabase.LoadAssetAtPath<Material>($"{MATERIALS_FOLDER}/MAT_Obstacle_DarkRed.mat");

        // Main cube - 2.0 wide (matches new lane width)
        GameObject mainCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        mainCube.name = "MainCube";
        mainCube.transform.parent = obstacleRoot.transform;
        mainCube.transform.localPosition = Vector3.zero;
        mainCube.transform.localScale = new Vector3(2.0f, 0.8f, 2.0f); // Matches new lane width

        Collider mainCollider = mainCube.GetComponent<Collider>();
        if (mainCollider != null) Object.DestroyImmediate(mainCollider);

        BoxCollider boxCollider = mainCube.AddComponent<BoxCollider>();
        boxCollider.size = new Vector3(2.0f, 0.8f, 2.0f);

        Renderer mainRend = mainCube.GetComponent<Renderer>();
        if (mainRend != null) mainRend.material = redMat;

        // Rigidbody - cade sempre
        Rigidbody rb = obstacleRoot.AddComponent<Rigidbody>();
        rb.mass = 2f;
        rb.linearDamping = 0;
        rb.angularDamping = 0;
        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

        // Details - strisce di pericolo
        GameObject details = new GameObject("Details");
        details.transform.parent = obstacleRoot.transform;
        details.transform.localPosition = Vector3.zero;

        // Warning stripes
        for (int i = 0; i < 3; i++)
        {
            GameObject stripe = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stripe.name = $"Stripe_{i}";
            stripe.transform.parent = details.transform;
            stripe.transform.localPosition = new Vector3(0, -0.3f + (i * 0.3f), 0);
            stripe.transform.localScale = new Vector3(2.0f, 0.1f, 2.0f); // Matches new lane width

            Collider stripeCol = stripe.GetComponent<Collider>();
            if (stripeCol != null) Object.DestroyImmediate(stripeCol);

            Renderer stripeRend = stripe.GetComponent<Renderer>();
            if (stripeRend != null) stripeRend.material = darkRedMat;
        }

        // Danger indicator - sfera sulla cima
        GameObject dangerIndicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        dangerIndicator.name = "DangerIndicator";
        dangerIndicator.transform.parent = details.transform;
        dangerIndicator.transform.localPosition = new Vector3(0, 0.6f, 0);
        dangerIndicator.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

        Collider indicatorCol = dangerIndicator.GetComponent<Collider>();
        if (indicatorCol != null) Object.DestroyImmediate(indicatorCol);

        Renderer indicatorRend = dangerIndicator.GetComponent<Renderer>();
        if (indicatorRend != null) indicatorRend.material = redMat;

        // Add obstacle controller
        FallingObstacleController obstacleController = obstacleRoot.AddComponent<FallingObstacleController>();

        // Create prefab
        string prefabPath = $"{PREFABS_FOLDER}/FallingObstacle.prefab";
        PrefabUtility.SaveAsPrefabAsset(obstacleRoot, prefabPath);
        Object.DestroyImmediate(obstacleRoot);

        Debug.Log("✓ FallingObstacle prefab created");
    }
}
