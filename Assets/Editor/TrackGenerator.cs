using UnityEngine;
using UnityEditor;
using System.IO;

public class TrackGenerator
{
    private const string MATERIALS_FOLDER = "Assets/Materials/Track";
    private const string PREFABS_FOLDER = "Assets/Prefabs/Track";
    
    // Dimensioni della pista
    private const float TOTAL_WIDTH = 9f;
    private const float SEGMENT_LENGTH = 10f;
    private const float BORDER_HEIGHT = 0.5f;
    private const float LANE_WIDTH = 3f;
    private const int NUM_LANES = 3;

    [MenuItem("Square Spin/Generator/Create Track Materials & Prefab")]
    public static void GenerateTrackAssets()
    {
        // Crea cartelle
        CreateFolders();
        
        // Crea materiali
        CreateMaterials();
        
        // Crea prefab
        CreateTrackPrefab();
        
        AssetDatabase.Refresh();
        Debug.Log("✓ Track assets generated successfully!");
    }

    private static void CreateFolders()
    {
        if (!AssetDatabase.IsValidFolder(MATERIALS_FOLDER))
        {
            AssetDatabase.CreateFolder("Assets", "Materials");
            AssetDatabase.CreateFolder("Assets/Materials", "Track");
        }

        if (!AssetDatabase.IsValidFolder(PREFABS_FOLDER))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
            AssetDatabase.CreateFolder("Assets/Prefabs", "Track");
        }
    }

    private static void CreateMaterials()
    {
        // 1. MAT_FloorMetal_Dark - Grigio scuro, metallico satinato
        Material floorMetal = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        floorMetal.name = "MAT_FloorMetal_Dark";
        floorMetal.SetColor("_BaseColor", new Color(0.15f, 0.15f, 0.15f, 1f));
        floorMetal.SetFloat("_Metallic", 0.8f);
        floorMetal.SetFloat("_Smoothness", 0.5f);
        AssetDatabase.CreateAsset(floorMetal, $"{MATERIALS_FOLDER}/MAT_FloorMetal_Dark.mat");

        // 2. MAT_Border_BlackSteel - Nero lucido, contrasto
        Material borderBlack = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        borderBlack.name = "MAT_Border_BlackSteel";
        borderBlack.SetColor("_BaseColor", new Color(0.05f, 0.05f, 0.05f, 1f));
        borderBlack.SetFloat("_Metallic", 0.9f);
        borderBlack.SetFloat("_Smoothness", 0.7f);
        AssetDatabase.CreateAsset(borderBlack, $"{MATERIALS_FOLDER}/MAT_Border_BlackSteel.mat");

        // 3. MAT_LaneGlow_Blue - Neon blu emissivo
        Material laneGlow = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        laneGlow.name = "MAT_LaneGlow_Blue";
        laneGlow.SetColor("_BaseColor", new Color(0.1f, 0.3f, 0.8f, 1f));
        laneGlow.SetColor("_EmissionColor", new Color(0.2f, 0.6f, 1.0f, 1f) * 2f);
        laneGlow.EnableKeyword("_EMISSION");
        laneGlow.SetFloat("_Metallic", 0.3f);
        laneGlow.SetFloat("_Smoothness", 0.9f);
        AssetDatabase.CreateAsset(laneGlow, $"{MATERIALS_FOLDER}/MAT_LaneGlow_Blue.mat");

        // 4. MAT_SideGlow_Purple - Neon viola bluastro
        Material sideGlow = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        sideGlow.name = "MAT_SideGlow_Purple";
        sideGlow.SetColor("_BaseColor", new Color(0.5f, 0.2f, 0.8f, 1f));
        sideGlow.SetColor("_EmissionColor", new Color(0.7f, 0.3f, 1.0f, 1f) * 2.5f);
        sideGlow.EnableKeyword("_EMISSION");
        sideGlow.SetFloat("_Metallic", 0.2f);
        sideGlow.SetFloat("_Smoothness", 0.85f);
        AssetDatabase.CreateAsset(sideGlow, $"{MATERIALS_FOLDER}/MAT_SideGlow_Purple.mat");

        // 5. MAT_Panel_Gray - Grigio per dettagli decorativi
        Material panelGray = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        panelGray.name = "MAT_Panel_Gray";
        panelGray.SetColor("_BaseColor", new Color(0.25f, 0.25f, 0.25f, 1f));
        panelGray.SetFloat("_Metallic", 0.5f);
        panelGray.SetFloat("_Smoothness", 0.4f);
        AssetDatabase.CreateAsset(panelGray, $"{MATERIALS_FOLDER}/MAT_Panel_Gray.mat");

        Debug.Log("✓ Materials created");
    }

    private static void CreateTrackPrefab()
    {
        // Root GameObject
        GameObject trackRoot = new GameObject("TrackSegment_01");
        
        // Carica i materiali
        Material floorMat = AssetDatabase.LoadAssetAtPath<Material>($"{MATERIALS_FOLDER}/MAT_FloorMetal_Dark.mat");
        Material borderMat = AssetDatabase.LoadAssetAtPath<Material>($"{MATERIALS_FOLDER}/MAT_Border_BlackSteel.mat");
        Material laneMat = AssetDatabase.LoadAssetAtPath<Material>($"{MATERIALS_FOLDER}/MAT_LaneGlow_Blue.mat");
        Material sideMat = AssetDatabase.LoadAssetAtPath<Material>($"{MATERIALS_FOLDER}/MAT_SideGlow_Purple.mat");
        Material panelMat = AssetDatabase.LoadAssetAtPath<Material>($"{MATERIALS_FOLDER}/MAT_Panel_Gray.mat");

        // Floor_Base - pavimento della pista
        GameObject floorBase = CreatePlane("Floor_Base", TOTAL_WIDTH, SEGMENT_LENGTH, Vector3.zero, floorMat);
        floorBase.transform.parent = trackRoot.transform;

        // Border_Left
        GameObject borderLeft = CreateCube("Border_Left", 0.2f, BORDER_HEIGHT, SEGMENT_LENGTH, 
            new Vector3(-TOTAL_WIDTH / 2 - 0.1f, BORDER_HEIGHT / 2, 0), borderMat);
        borderLeft.transform.parent = trackRoot.transform;

        // Border_Right
        GameObject borderRight = CreateCube("Border_Right", 0.2f, BORDER_HEIGHT, SEGMENT_LENGTH, 
            new Vector3(TOTAL_WIDTH / 2 + 0.1f, BORDER_HEIGHT / 2, 0), borderMat);
        borderRight.transform.parent = trackRoot.transform;

        // LaneDivider_1 - tra corsia 1 e 2
        float lanePos1 = -LANE_WIDTH + 0.05f;
        GameObject laneDivider1 = CreateCube("LaneDivider_1", 0.1f, 0.3f, SEGMENT_LENGTH, 
            new Vector3(lanePos1, 0.15f, 0), laneMat);
        laneDivider1.transform.parent = trackRoot.transform;

        // LaneDivider_2 - tra corsia 2 e 3
        float lanePos2 = LANE_WIDTH - 0.05f;
        GameObject laneDivider2 = CreateCube("LaneDivider_2", 0.1f, 0.3f, SEGMENT_LENGTH, 
            new Vector3(lanePos2, 0.15f, 0), laneMat);
        laneDivider2.transform.parent = trackRoot.transform;

        // SideLight_Left - luci laterali sinistra
        GameObject sideLightLeft = CreateCube("SideLight_Left", 0.15f, 0.4f, SEGMENT_LENGTH, 
            new Vector3(-TOTAL_WIDTH / 2 - 0.5f, 0.2f, 0), sideMat);
        sideLightLeft.transform.parent = trackRoot.transform;

        // SideLight_Right - luci laterali destra
        GameObject sideLightRight = CreateCube("SideLight_Right", 0.15f, 0.4f, SEGMENT_LENGTH, 
            new Vector3(TOTAL_WIDTH / 2 + 0.5f, 0.2f, 0), sideMat);
        sideLightRight.transform.parent = trackRoot.transform;

        // Detail_Panels - piccoli pannelli decorativi
        GameObject detailPanels = new GameObject("Detail_Panels");
        detailPanels.transform.parent = trackRoot.transform;
        detailPanels.transform.localPosition = Vector3.zero;

        // Aggiungi alcuni piccoli pannelli lungo la pista
        for (int i = 0; i < 5; i++)
        {
            float zPos = -SEGMENT_LENGTH / 2 + (i + 1) * (SEGMENT_LENGTH / 6);
            GameObject panel = CreateCube($"Panel_{i}", 0.3f, 0.15f, 0.2f, 
                new Vector3(-TOTAL_WIDTH / 2 + 0.3f, 0.25f, zPos), panelMat);
            panel.transform.parent = detailPanels.transform;

            GameObject panel2 = CreateCube($"Panel_{i + 5}", 0.3f, 0.15f, 0.2f, 
                new Vector3(TOTAL_WIDTH / 2 - 0.3f, 0.25f, zPos), panelMat);
            panel2.transform.parent = detailPanels.transform;
        }

        // GlowParticles - empty object per effetti futuri
        GameObject glowParticles = new GameObject("GlowParticles");
        glowParticles.transform.parent = trackRoot.transform;
        glowParticles.transform.localPosition = Vector3.zero;

        // Aggiungi componente per future animazioni
        TrackSegmentController controller = trackRoot.AddComponent<TrackSegmentController>();

        // Crea prefab
        string prefabPath = $"{PREFABS_FOLDER}/TrackSegment_01.prefab";
        PrefabUtility.SaveAsPrefabAsset(trackRoot, prefabPath);

        // Cleanup
        Object.DestroyImmediate(trackRoot);

        Debug.Log("✓ TrackSegment_01 prefab created at: " + prefabPath);
    }

    private static GameObject CreatePlane(string name, float width, float length, Vector3 position, Material material)
    {
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.name = name;
        plane.transform.position = position;
        plane.transform.localScale = new Vector3(width / 10f, 1, length / 10f);

        // Rimuovi collider per evitare conflitti
        Collider collider = plane.GetComponent<Collider>();
        if (collider != null)
            Object.DestroyImmediate(collider);

        // Assegna materiale
        Renderer renderer = plane.GetComponent<Renderer>();
        if (renderer != null)
            renderer.material = material;

        return plane;
    }

    private static GameObject CreateCube(string name, float width, float height, float depth, Vector3 position, Material material)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = name;
        cube.transform.position = position;
        cube.transform.localScale = new Vector3(width, height, depth);

        // Rimuovi collider
        Collider collider = cube.GetComponent<Collider>();
        if (collider != null)
            Object.DestroyImmediate(collider);

        // Assegna materiale
        Renderer renderer = cube.GetComponent<Renderer>();
        if (renderer != null)
            renderer.material = material;

        return cube;
    }
}
