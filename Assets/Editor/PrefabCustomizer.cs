using UnityEngine;
using UnityEditor;

public class PrefabCustomizer
{
    private const string PREFABS_FOLDER = "Assets/Prefabs";

    [MenuItem("Square Spin/Customizer/Modify Track Prefab Size")]
    public static void ModifyTrackSize()
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PREFABS_FOLDER}/Track/TrackSegment_01.prefab");
        if (prefab == null)
        {
            Debug.LogError("Track prefab not found!");
            return;
        }

        // Modifica la scala del segmento
        prefab.transform.localScale = new Vector3(1f, 1f, 1f);
        PrefabUtility.SavePrefabAsset(prefab);
        Debug.Log("✓ Track prefab size modified");
    }

    [MenuItem("Square Spin/Customizer/Modify Player Prefab Size")]
    public static void ModifyPlayerSize()
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PREFABS_FOLDER}/Player/PlayerCube.prefab");
        if (prefab == null)
        {
            Debug.LogError("Player prefab not found!");
            return;
        }

        prefab.transform.localScale = new Vector3(1f, 1f, 1f);
        PrefabUtility.SavePrefabAsset(prefab);
        Debug.Log("✓ Player prefab size modified");
    }

    [MenuItem("Square Spin/Customizer/Add More Glow Particles to Track")]
    public static void AddMoreGlowParticles()
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PREFABS_FOLDER}/Track/TrackSegment_01.prefab");
        if (prefab == null)
        {
            Debug.LogError("Track prefab not found!");
            return;
        }

        // Carica il prefab in edit mode
        GameObject editingPrefab = PrefabUtility.LoadPrefabContents($"{PREFABS_FOLDER}/Track/TrackSegment_01.prefab");

        // Trova il nodo GlowParticles
        Transform glowParticles = editingPrefab.transform.Find("GlowParticles");
        if (glowParticles == null)
        {
            Debug.LogError("GlowParticles not found!");
            PrefabUtility.UnloadPrefabContents(editingPrefab);
            return;
        }

        // Aggiungi un ParticleSystem
        GameObject particleObj = new GameObject("Particles_Trail");
        particleObj.transform.parent = glowParticles;
        particleObj.transform.localPosition = Vector3.zero;

        ParticleSystem ps = particleObj.AddComponent<ParticleSystem>();
        // Configurazione base del particle system
        var mainModule = ps.main;
        mainModule.duration = 2f;
        mainModule.loop = true;
        mainModule.startColor = new ParticleSystem.MinMaxGradient(new Color(0.2f, 0.6f, 1.0f, 0.5f));

        PrefabUtility.SaveAsPrefabAsset(editingPrefab, $"{PREFABS_FOLDER}/Track/TrackSegment_01.prefab");
        PrefabUtility.UnloadPrefabContents(editingPrefab);

        Debug.Log("✓ Glow particles added to track");
    }

    [MenuItem("Square Spin/Customizer/Make Track Wider")]
    public static void MakeTrackWider()
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PREFABS_FOLDER}/Track/TrackSegment_01.prefab");
        if (prefab == null)
        {
            Debug.LogError("Track prefab not found!");
            return;
        }

        GameObject editingPrefab = PrefabUtility.LoadPrefabContents($"{PREFABS_FOLDER}/Track/TrackSegment_01.prefab");

        // Modifica il pavimento
        Transform floor = editingPrefab.transform.Find("Floor_Base");
        if (floor != null)
        {
            floor.localScale = new Vector3(1.5f, 1f, 1f);
        }

        PrefabUtility.SaveAsPrefabAsset(editingPrefab, $"{PREFABS_FOLDER}/Track/TrackSegment_01.prefab");
        PrefabUtility.UnloadPrefabContents(editingPrefab);

        Debug.Log("✓ Track is now wider");
    }

    [MenuItem("Square Spin/Customizer/Make Track Longer")]
    public static void MakeTrackLonger()
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PREFABS_FOLDER}/Track/TrackSegment_01.prefab");
        if (prefab == null)
        {
            Debug.LogError("Track prefab not found!");
            return;
        }

        GameObject editingPrefab = PrefabUtility.LoadPrefabContents($"{PREFABS_FOLDER}/Track/TrackSegment_01.prefab");

        // Modifica il pavimento
        Transform floor = editingPrefab.transform.Find("Floor_Base");
        if (floor != null)
        {
            floor.localScale = new Vector3(floor.localScale.x, 1f, 1.5f);
        }

        PrefabUtility.SaveAsPrefabAsset(editingPrefab, $"{PREFABS_FOLDER}/Track/TrackSegment_01.prefab");
        PrefabUtility.UnloadPrefabContents(editingPrefab);

        Debug.Log("✓ Track is now longer");
    }

    [MenuItem("Square Spin/Customizer/Make Player Bigger")]
    public static void MakePlayerBigger()
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PREFABS_FOLDER}/Player/PlayerCube.prefab");
        if (prefab == null)
        {
            Debug.LogError("Player prefab not found!");
            return;
        }

        GameObject editingPrefab = PrefabUtility.LoadPrefabContents($"{PREFABS_FOLDER}/Player/PlayerCube.prefab");
        editingPrefab.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

        PrefabUtility.SaveAsPrefabAsset(editingPrefab, $"{PREFABS_FOLDER}/Player/PlayerCube.prefab");
        PrefabUtility.UnloadPrefabContents(editingPrefab);

        Debug.Log("✓ Player is now bigger");
    }

    [MenuItem("Square Spin/Customizer/Info/Show Prefab Modification Info")]
    public static void ShowInfo()
    {
        EditorUtility.DisplayDialog("Prefab Customizer",
            "Usa questi menu per modificare i prefab direttamente.\n\n" +
            "Ogni modifica viene salvata automaticamente nel prefab.\n\n" +
            "Per personalizzazioni più complesse, usa il Project folder e modifica i prefab manualmente.",
            "OK");
    }
}
