using UnityEngine;
using UnityEditor;

public class TrackGeneratorV2
{
    private const string MATERIALS_FOLDER = "Assets/Materials/Track";
    private const string PREFABS_FOLDER = "Assets/Prefabs/Track";

    // Dimensioni pista V2
    private const int NUM_LANES = 3;
    private const float LANE_WIDTH = 2.0f; // Larghezza di ogni corsia (cubo è 1.0f)
    private const float LANE_DIVIDER_WIDTH = 0.1f; // Divisori tra corsie
    private const float TOTAL_WIDTH = (LANE_WIDTH * NUM_LANES) + (LANE_DIVIDER_WIDTH * (NUM_LANES - 1)); // 6.2
    private const float SEGMENT_LENGTH = 10f;
    private const float BORDER_HEIGHT = 0.5f;
    private const float BORDER_THICKNESS = 0.2f;

    [MenuItem("Square Spin/Generator/Create Track V2 (Uniform Lanes)")]
    public static void GenerateTrackV2()
    {
        CreateMaterials();
        CreateTrackPrefab();
        AssetDatabase.Refresh();
        Debug.Log("✓ Track V2 created!");
    }

    private static void CreateMaterials()
    {
        // Floor metal dark
        Material floorMetal = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        floorMetal.name = "MAT_FloorMetal_Dark";
        floorMetal.SetColor("_BaseColor", new Color(0.15f, 0.15f, 0.15f, 1f));
        floorMetal.SetFloat("_Metallic", 0.8f);
        floorMetal.SetFloat("_Smoothness", 0.5f);
        AssetDatabase.CreateAsset(floorMetal, $"{MATERIALS_FOLDER}/MAT_FloorMetal_Dark.mat");

        // Border black steel
        Material borderBlack = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        borderBlack.name = "MAT_Border_BlackSteel";
        borderBlack.SetColor("_BaseColor", new Color(0.05f, 0.05f, 0.05f, 1f));
        borderBlack.SetFloat("_Metallic", 0.9f);
        borderBlack.SetFloat("_Smoothness", 0.7f);
        AssetDatabase.CreateAsset(borderBlack, $"{MATERIALS_FOLDER}/MAT_Border_BlackSteel.mat");

        // Lane glow blue
        Material laneGlow = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        laneGlow.name = "MAT_LaneGlow_Blue";
        laneGlow.SetColor("_BaseColor", new Color(0.1f, 0.3f, 0.8f, 1f));
        laneGlow.SetColor("_EmissionColor", new Color(0.2f, 0.6f, 1.0f, 1f) * 2f);
        laneGlow.EnableKeyword("_EMISSION");
        laneGlow.SetFloat("_Metallic", 0.3f);
        laneGlow.SetFloat("_Smoothness", 0.9f);
        AssetDatabase.CreateAsset(laneGlow, $"{MATERIALS_FOLDER}/MAT_LaneGlow_Blue.mat");

        // Side glow purple
        Material sideGlow = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        sideGlow.name = "MAT_SideGlow_Purple";
        sideGlow.SetColor("_BaseColor", new Color(0.5f, 0.2f, 0.8f, 1f));
        sideGlow.SetColor("_EmissionColor", new Color(0.7f, 0.3f, 1.0f, 1f) * 2.5f);
        sideGlow.EnableKeyword("_EMISSION");
        sideGlow.SetFloat("_Metallic", 0.2f);
        sideGlow.SetFloat("_Smoothness", 0.85f);
        AssetDatabase.CreateAsset(sideGlow, $"{MATERIALS_FOLDER}/MAT_SideGlow_Purple.mat");

        // Panel gray
        Material panelGray = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        panelGray.name = "MAT_Panel_Gray";
        panelGray.SetColor("_BaseColor", new Color(0.25f, 0.25f, 0.25f, 1f));
        panelGray.SetFloat("_Metallic", 0.5f);
        panelGray.SetFloat("_Smoothness", 0.4f);
        AssetDatabase.CreateAsset(panelGray, $"{MATERIALS_FOLDER}/MAT_Panel_Gray.mat");

        Debug.Log("✓ Track materials created");
    }

    private static void CreateTrackPrefab()
    {
        GameObject trackRoot = new GameObject("TrackSegment_V2");

        // Load materials
        Material floorMat = AssetDatabase.LoadAssetAtPath<Material>($"{MATERIALS_FOLDER}/MAT_FloorMetal_Dark.mat");
        Material borderMat = AssetDatabase.LoadAssetAtPath<Material>($"{MATERIALS_FOLDER}/MAT_Border_BlackSteel.mat");
        Material laneMat = AssetDatabase.LoadAssetAtPath<Material>($"{MATERIALS_FOLDER}/MAT_LaneGlow_Blue.mat");
        Material sideMat = AssetDatabase.LoadAssetAtPath<Material>($"{MATERIALS_FOLDER}/MAT_SideGlow_Purple.mat");
        Material panelMat = AssetDatabase.LoadAssetAtPath<Material>($"{MATERIALS_FOLDER}/MAT_Panel_Gray.mat");

        // Floor base
        GameObject floorBase = CreatePlane("Floor_Base", TOTAL_WIDTH, SEGMENT_LENGTH, Vector3.zero, floorMat);
        floorBase.transform.parent = trackRoot.transform;

        // Borders
        float borderXLeft = -TOTAL_WIDTH / 2 - BORDER_THICKNESS / 2;
        float borderXRight = TOTAL_WIDTH / 2 + BORDER_THICKNESS / 2;

        GameObject borderLeft = CreateCube("Border_Left", BORDER_THICKNESS, BORDER_HEIGHT, SEGMENT_LENGTH,
            new Vector3(borderXLeft, BORDER_HEIGHT / 2, 0), borderMat);
        borderLeft.transform.parent = trackRoot.transform;

        GameObject borderRight = CreateCube("Border_Right", BORDER_THICKNESS, BORDER_HEIGHT, SEGMENT_LENGTH,
            new Vector3(borderXRight, BORDER_HEIGHT / 2, 0), borderMat);
        borderRight.transform.parent = trackRoot.transform;

        // Lane dividers - tra le corsie
        GameObject laneDividers = new GameObject("LaneDividers");
        laneDividers.transform.parent = trackRoot.transform;
        laneDividers.transform.localPosition = Vector3.zero;

        // Calcola le posizioni dei divisori
        float totalTrackWidth = (LANE_WIDTH * NUM_LANES) + (LANE_DIVIDER_WIDTH * (NUM_LANES - 1));
        float laneOffset = (totalTrackWidth / 2f) - (LANE_WIDTH / 2f);
        
        // Divider tra lane 1 e 2
        float divider1X = -laneOffset + LANE_WIDTH / 2f;
        GameObject divider1 = CreateCube("Divider_1", LANE_DIVIDER_WIDTH, 0.3f, SEGMENT_LENGTH,
            new Vector3(divider1X, 0.15f, 0), laneMat);
        divider1.transform.parent = laneDividers.transform;

        // Divider tra lane 2 e 3
        float divider2X = laneOffset - LANE_WIDTH / 2f;
        GameObject divider2 = CreateCube("Divider_2", LANE_DIVIDER_WIDTH, 0.3f, SEGMENT_LENGTH,
            new Vector3(divider2X, 0.15f, 0), laneMat);
        divider2.transform.parent = laneDividers.transform;

        // Side lights
        GameObject sideLightLeft = CreateCube("SideLight_Left", 0.15f, 0.4f, SEGMENT_LENGTH,
            new Vector3(borderXLeft - 0.3f, 0.2f, 0), sideMat);
        sideLightLeft.transform.parent = trackRoot.transform;

        GameObject sideLightRight = CreateCube("SideLight_Right", 0.15f, 0.4f, SEGMENT_LENGTH,
            new Vector3(borderXRight + 0.3f, 0.2f, 0), sideMat);
        sideLightRight.transform.parent = trackRoot.transform;

        // Detail panels
        GameObject detailPanels = new GameObject("DetailPanels");
        detailPanels.transform.parent = trackRoot.transform;
        detailPanels.transform.localPosition = Vector3.zero;

        for (int i = 0; i < 5; i++)
        {
            float zPos = -SEGMENT_LENGTH / 2 + (i + 1) * (SEGMENT_LENGTH / 6);
            GameObject panel = CreateCube($"Panel_{i}", 0.3f, 0.15f, 0.2f,
                new Vector3(borderXLeft - 0.5f, 0.25f, zPos), panelMat);
            panel.transform.parent = detailPanels.transform;

            GameObject panel2 = CreateCube($"Panel_{i + 5}", 0.3f, 0.15f, 0.2f,
                new Vector3(borderXRight + 0.5f, 0.25f, zPos), panelMat);
            panel2.transform.parent = detailPanels.transform;
        }

        // Glow particles
        GameObject glowParticles = new GameObject("GlowParticles");
        glowParticles.transform.parent = trackRoot.transform;
        glowParticles.transform.localPosition = Vector3.zero;

        // Controller
        TrackSegmentController controller = trackRoot.AddComponent<TrackSegmentController>();

        // Create prefab
        string prefabPath = $"{PREFABS_FOLDER}/TrackSegment_V2.prefab";
        PrefabUtility.SaveAsPrefabAsset(trackRoot, prefabPath);
        Object.DestroyImmediate(trackRoot);

        Debug.Log($"✓ TrackSegment_V2 created - Total Width: {TOTAL_WIDTH}, Lane Width: {LANE_WIDTH}");
    }

    private static GameObject CreatePlane(string name, float width, float length, Vector3 position, Material material)
    {
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.name = name;
        plane.transform.position = position;
        plane.transform.localScale = new Vector3(width / 10f, 1, length / 10f);

        Collider collider = plane.GetComponent<Collider>();
        if (collider != null) Object.DestroyImmediate(collider);

        // Aggiungi un BoxCollider solido per il floor della pista
        BoxCollider boxCollider = plane.AddComponent<BoxCollider>();
        boxCollider.size = new Vector3(width, 1f, length); // Solido e piatto
        boxCollider.isTrigger = false;

        Renderer renderer = plane.GetComponent<Renderer>();
        if (renderer != null) renderer.material = material;

        return plane;
    }

    private static GameObject CreateCube(string name, float width, float height, float depth, Vector3 position, Material material)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = name;
        cube.transform.position = position;
        cube.transform.localScale = new Vector3(width, height, depth);

        Collider collider = cube.GetComponent<Collider>();
        if (collider != null) Object.DestroyImmediate(collider);

        Renderer renderer = cube.GetComponent<Renderer>();
        if (renderer != null) renderer.material = material;

        return cube;
    }
}
