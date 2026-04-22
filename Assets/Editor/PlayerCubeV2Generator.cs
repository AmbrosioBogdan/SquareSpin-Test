using UnityEngine;
using UnityEditor;

public class PlayerCubeV2Generator
{
    private const string MATERIALS_FOLDER = "Assets/Materials/Player";
    private const string PREFABS_FOLDER = "Assets/Prefabs/Player";

    [MenuItem("Square Spin/Generator/Create Player Cube V2 (Advanced)")]
    public static void GeneratePlayerCubeV2()
    {
        CreateFolders();
        CreatePlayerMaterials();
        CreatePlayerPrefabV2();
        AssetDatabase.Refresh();
        Debug.Log("✓ Player Cube V2 created!");
    }

    private static void CreateFolders()
    {
        if (!AssetDatabase.IsValidFolder(MATERIALS_FOLDER))
        {
            AssetDatabase.CreateFolder("Assets", "Materials");
            AssetDatabase.CreateFolder("Assets/Materials", "Player");
        }
        if (!AssetDatabase.IsValidFolder(PREFABS_FOLDER))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
            AssetDatabase.CreateFolder("Assets/Prefabs", "Player");
        }
    }

    private static void CreatePlayerMaterials()
    {
        // Core scuro
        Material coreMetal = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        coreMetal.name = "MAT_Player_CoreMetal";
        coreMetal.SetColor("_BaseColor", new Color(0.08f, 0.08f, 0.1f, 1f));
        coreMetal.SetFloat("_Metallic", 0.75f);
        coreMetal.SetFloat("_Smoothness", 0.65f);
        AssetDatabase.CreateAsset(coreMetal, $"{MATERIALS_FOLDER}/MAT_Player_CoreMetal.mat");

        // Glow principale cyan
        Material glowMain = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        glowMain.name = "MAT_Player_GlowCyan";
        glowMain.SetColor("_BaseColor", new Color(0.2f, 0.9f, 1.0f, 1f));
        glowMain.SetColor("_EmissionColor", new Color(0.4f, 1.0f, 1.0f, 1f) * 3.5f);
        glowMain.EnableKeyword("_EMISSION");
        glowMain.SetFloat("_Metallic", 0.6f);
        glowMain.SetFloat("_Smoothness", 0.95f);
        AssetDatabase.CreateAsset(glowMain, $"{MATERIALS_FOLDER}/MAT_Player_GlowCyan.mat");

        // Accent blu
        Material accentBlue = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        accentBlue.name = "MAT_Player_AccentBlue";
        accentBlue.SetColor("_BaseColor", new Color(0.0f, 0.3f, 0.9f, 1f));
        accentBlue.SetColor("_EmissionColor", new Color(0.1f, 0.5f, 1.0f, 1f) * 2f);
        accentBlue.EnableKeyword("_EMISSION");
        accentBlue.SetFloat("_Metallic", 0.8f);
        accentBlue.SetFloat("_Smoothness", 0.9f);
        AssetDatabase.CreateAsset(accentBlue, $"{MATERIALS_FOLDER}/MAT_Player_AccentBlue.mat");

        // Edge glow viola
        Material glowViolet = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        glowViolet.name = "MAT_Player_GlowViolet";
        glowViolet.SetColor("_BaseColor", new Color(0.6f, 0.2f, 0.9f, 1f));
        glowViolet.SetColor("_EmissionColor", new Color(0.8f, 0.3f, 1.0f, 1f) * 2.5f);
        glowViolet.EnableKeyword("_EMISSION");
        glowViolet.SetFloat("_Metallic", 0.5f);
        glowViolet.SetFloat("_Smoothness", 0.85f);
        AssetDatabase.CreateAsset(glowViolet, $"{MATERIALS_FOLDER}/MAT_Player_GlowViolet.mat");
    }

    private static void CreatePlayerPrefabV2()
    {
        // Root
        GameObject playerCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        playerCube.name = "PlayerCubeV2";

        // Remove default collider
        Collider collider = playerCube.GetComponent<Collider>();
        if (collider != null) Object.DestroyImmediate(collider);

        // Add custom collider
        BoxCollider boxCollider = playerCube.AddComponent<BoxCollider>();
        boxCollider.size = Vector3.one;

        // Scale
        playerCube.transform.localScale = new Vector3(1f, 1f, 1f);

        // Load materials
        Material coreMat = AssetDatabase.LoadAssetAtPath<Material>($"{MATERIALS_FOLDER}/MAT_Player_CoreMetal.mat");
        Material glowMat = AssetDatabase.LoadAssetAtPath<Material>($"{MATERIALS_FOLDER}/MAT_Player_GlowCyan.mat");
        Material accentMat = AssetDatabase.LoadAssetAtPath<Material>($"{MATERIALS_FOLDER}/MAT_Player_AccentBlue.mat");
        Material violetMat = AssetDatabase.LoadAssetAtPath<Material>($"{MATERIALS_FOLDER}/MAT_Player_GlowViolet.mat");

        // Assign core material
        Renderer renderer = playerCube.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material[] materials = new Material[1];
            materials[0] = coreMat;
            renderer.materials = materials;
        }

        // Add components
        playerCube.tag = "Player"; // Tag per il riconoscimento
        Rigidbody rb = playerCube.AddComponent<Rigidbody>();
        // Congela: Z (resta fisso sulla pista), Rotazione (no flipping)
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        rb.mass = 1f;
        rb.linearDamping = 0;
        rb.angularDamping = 0.05f;
        
        PlayerCubeController controller = playerCube.AddComponent<PlayerCubeController>();

        // Create details
        CreateDetailsV2(playerCube, glowMat, accentMat, violetMat);

        // Create prefab
        string prefabPath = $"{PREFABS_FOLDER}/PlayerCubeV2.prefab";
        PrefabUtility.SaveAsPrefabAsset(playerCube, prefabPath);
        Object.DestroyImmediate(playerCube);

        Debug.Log("✓ PlayerCubeV2 prefab created");
    }

    private static void CreateDetailsV2(GameObject parent, Material glowMat, Material accentMat, Material violetMat)
    {
        // Main details container
        GameObject details = new GameObject("Details");
        details.transform.parent = parent.transform;
        details.transform.localPosition = Vector3.zero;

        // 1. Corner glow spheres - 8 angoli
        GameObject corners = new GameObject("CornerGlows");
        corners.transform.parent = details.transform;
        corners.transform.localPosition = Vector3.zero;

        Vector3[] cornerPositions = new Vector3[]
        {
            new Vector3(0.5f, 0.5f, 0.5f),
            new Vector3(-0.5f, 0.5f, 0.5f),
            new Vector3(0.5f, -0.5f, 0.5f),
            new Vector3(-0.5f, -0.5f, 0.5f),
            new Vector3(0.5f, 0.5f, -0.5f),
            new Vector3(-0.5f, 0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, -0.5f),
            new Vector3(-0.5f, -0.5f, -0.5f),
        };

        for (int i = 0; i < cornerPositions.Length; i++)
        {
            GameObject corner = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            corner.name = $"Corner_{i}";
            corner.transform.parent = corners.transform;
            corner.transform.localPosition = cornerPositions[i];
            corner.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

            Collider col = corner.GetComponent<Collider>();
            if (col != null) Object.DestroyImmediate(col);

            Renderer rend = corner.GetComponent<Renderer>();
            if (rend != null) rend.material = glowMat;
        }

        // 2. Edge lights - spigoli principali
        GameObject edges = new GameObject("EdgeLights");
        edges.transform.parent = details.transform;
        edges.transform.localPosition = Vector3.zero;

        // Spigoli verticali (4)
        Vector3[] edgePositions = new Vector3[]
        {
            new Vector3(0.5f, 0, 0.5f),
            new Vector3(-0.5f, 0, 0.5f),
            new Vector3(0.5f, 0, -0.5f),
            new Vector3(-0.5f, 0, -0.5f),
        };

        for (int i = 0; i < edgePositions.Length; i++)
        {
            GameObject edge = GameObject.CreatePrimitive(PrimitiveType.Cube);
            edge.name = $"Edge_{i}";
            edge.transform.parent = edges.transform;
            edge.transform.localPosition = edgePositions[i];
            edge.transform.localScale = new Vector3(0.06f, 1f, 0.06f);

            Collider col = edge.GetComponent<Collider>();
            if (col != null) Object.DestroyImmediate(col);

            Renderer rend = edge.GetComponent<Renderer>();
            if (rend != null) rend.material = accentMat;
        }

        // 3. Frontal panel - pannello davanti
        GameObject frontPanel = GameObject.CreatePrimitive(PrimitiveType.Cube);
        frontPanel.name = "FrontPanel";
        frontPanel.transform.parent = details.transform;
        frontPanel.transform.localPosition = new Vector3(0, 0, 0.55f);
        frontPanel.transform.localScale = new Vector3(0.8f, 0.8f, 0.1f);

        Collider frontCol = frontPanel.GetComponent<Collider>();
        if (frontCol != null) Object.DestroyImmediate(frontCol);

        Renderer frontRend = frontPanel.GetComponent<Renderer>();
        if (frontRend != null) frontRend.material = glowMat;

        // 4. Top stripe glow - striscia superiore
        GameObject topStripe = GameObject.CreatePrimitive(PrimitiveType.Cube);
        topStripe.name = "TopStripe";
        topStripe.transform.parent = details.transform;
        topStripe.transform.localPosition = new Vector3(0, 0.55f, 0);
        topStripe.transform.localScale = new Vector3(0.9f, 0.08f, 0.9f);

        Collider topCol = topStripe.GetComponent<Collider>();
        if (topCol != null) Object.DestroyImmediate(topCol);

        Renderer topRend = topStripe.GetComponent<Renderer>();
        if (topRend != null) topRend.material = violetMat;

        // 5. Center core indicator
        GameObject centerCore = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        centerCore.name = "CenterCore";
        centerCore.transform.parent = details.transform;
        centerCore.transform.localPosition = Vector3.zero;
        centerCore.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

        Collider centerCol = centerCore.GetComponent<Collider>();
        if (centerCol != null) Object.DestroyImmediate(centerCol);

        Renderer centerRend = centerCore.GetComponent<Renderer>();
        if (centerRend != null) centerRend.material = glowMat;
    }
}
