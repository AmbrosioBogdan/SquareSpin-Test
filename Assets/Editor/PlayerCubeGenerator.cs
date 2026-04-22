using UnityEngine;
using UnityEditor;

public class PlayerCubeGenerator
{
    private const string MATERIALS_FOLDER = "Assets/Materials/Player";
    private const string PREFABS_FOLDER = "Assets/Prefabs/Player";

    [MenuItem("Square Spin/Generator/Create Player Cube")]
    public static void GeneratePlayerCube()
    {
        // Crea cartelle
        CreateFolders();

        // Crea materiali
        CreatePlayerMaterials();

        // Crea prefab
        CreatePlayerPrefab();

        AssetDatabase.Refresh();
        Debug.Log("✓ Player cube generated successfully!");
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
        // MAT_Player_CoreMetal - corpo principale scuro
        Material coreMetal = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        coreMetal.name = "MAT_Player_CoreMetal";
        coreMetal.SetColor("_BaseColor", new Color(0.1f, 0.1f, 0.12f, 1f));
        coreMetal.SetFloat("_Metallic", 0.7f);
        coreMetal.SetFloat("_Smoothness", 0.6f);
        AssetDatabase.CreateAsset(coreMetal, $"{MATERIALS_FOLDER}/MAT_Player_CoreMetal.mat");

        // MAT_Player_Accent - accenti brillanti
        Material accent = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        accent.name = "MAT_Player_Accent";
        accent.SetColor("_BaseColor", new Color(0.0f, 0.4f, 0.9f, 1f));
        accent.SetColor("_EmissionColor", new Color(0.1f, 0.6f, 1.0f, 1f) * 2f);
        accent.EnableKeyword("_EMISSION");
        accent.SetFloat("_Metallic", 0.8f);
        accent.SetFloat("_Smoothness", 0.9f);
        AssetDatabase.CreateAsset(accent, $"{MATERIALS_FOLDER}/MAT_Player_Accent.mat");

        // MAT_Player_Glow - parti che si illuminano
        Material glow = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        glow.name = "MAT_Player_Glow";
        glow.SetColor("_BaseColor", new Color(0.3f, 0.8f, 1.0f, 1f));
        glow.SetColor("_EmissionColor", new Color(0.5f, 1.0f, 1.0f, 1f) * 3f);
        glow.EnableKeyword("_EMISSION");
        glow.SetFloat("_Metallic", 0.5f);
        glow.SetFloat("_Smoothness", 0.95f);
        AssetDatabase.CreateAsset(glow, $"{MATERIALS_FOLDER}/MAT_Player_Glow.mat");

        Debug.Log("✓ Player materials created");
    }

    private static void CreatePlayerPrefab()
    {
        // Root GameObject - il cubo principale
        GameObject playerCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        playerCube.name = "PlayerCube";

        // Rimuovi collider di default per aggiungerlo nostro
        Collider collider = playerCube.GetComponent<Collider>();
        if (collider != null)
            Object.DestroyImmediate(collider);

        // Aggiungi BoxCollider customizzato
        BoxCollider boxCollider = playerCube.AddComponent<BoxCollider>();
        boxCollider.size = Vector3.one;

        // Scala il cubo
        playerCube.transform.localScale = new Vector3(1f, 1f, 1f);

        // Carica materiali
        Material coreMat = AssetDatabase.LoadAssetAtPath<Material>($"{MATERIALS_FOLDER}/MAT_Player_CoreMetal.mat");
        Material accentMat = AssetDatabase.LoadAssetAtPath<Material>($"{MATERIALS_FOLDER}/MAT_Player_Accent.mat");
        Material glowMat = AssetDatabase.LoadAssetAtPath<Material>($"{MATERIALS_FOLDER}/MAT_Player_Glow.mat");

        // Assegna materiale principale
        Renderer renderer = playerCube.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material[] materials = new Material[1];
            materials[0] = coreMat;
            renderer.materials = materials;
        }

        // Aggiungi componenti
        playerCube.AddComponent<Rigidbody>();
        PlayerCubeController controller = playerCube.AddComponent<PlayerCubeController>();

        // Aggiungi oggetti figli per dettagli
        CreatePlayerDetails(playerCube, accentMat, glowMat);

        // Crea prefab
        string prefabPath = $"{PREFABS_FOLDER}/PlayerCube.prefab";
        PrefabUtility.SaveAsPrefabAsset(playerCube, prefabPath);

        // Cleanup
        Object.DestroyImmediate(playerCube);

        Debug.Log("✓ PlayerCube prefab created at: " + prefabPath);
    }

    private static void CreatePlayerDetails(GameObject parent, Material accentMat, Material glowMat)
    {
        // Dettagli - accenti luminosi sugli angoli del cubo
        GameObject details = new GameObject("Details");
        details.transform.parent = parent.transform;
        details.transform.localPosition = Vector3.zero;

        // Piccoli cubetti agli angoli per effetto tech
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
            corner.name = $"Corner_Glow_{i}";
            corner.transform.parent = details.transform;
            corner.transform.localPosition = cornerPositions[i];
            corner.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);

            // Rimuovi collider
            Collider collider = corner.GetComponent<Collider>();
            if (collider != null)
                Object.DestroyImmediate(collider);

            // Assegna materiale glow
            Renderer renderer = corner.GetComponent<Renderer>();
            if (renderer != null)
                renderer.material = glowMat;
        }

        // Strisce luminose laterali
        GameObject stripes = new GameObject("Stripes");
        stripes.transform.parent = details.transform;
        stripes.transform.localPosition = Vector3.zero;

        for (int i = 0; i < 4; i++)
        {
            GameObject stripe = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stripe.name = $"Stripe_{i}";
            stripe.transform.parent = stripes.transform;
            
            // Posiziona le strisce
            if (i == 0) stripe.transform.localPosition = new Vector3(0.5f, 0, 0);
            else if (i == 1) stripe.transform.localPosition = new Vector3(-0.5f, 0, 0);
            else if (i == 2) stripe.transform.localPosition = new Vector3(0, 0.5f, 0);
            else stripe.transform.localPosition = new Vector3(0, 0, 0.5f);

            stripe.transform.localScale = i < 2 ? new Vector3(0.05f, 0.6f, 0.6f) : new Vector3(0.6f, 0.05f, 0.6f);

            // Rimuovi collider
            Collider collider = stripe.GetComponent<Collider>();
            if (collider != null)
                Object.DestroyImmediate(collider);

            // Assegna materiale accent
            Renderer renderer = stripe.GetComponent<Renderer>();
            if (renderer != null)
                renderer.material = accentMat;
        }
    }
}
