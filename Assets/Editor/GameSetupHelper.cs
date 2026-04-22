using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class GameSetupHelper
{
    [MenuItem("Square Spin/Setup/Setup Game Scene")]
    public static void SetupGameScene()
    {
        // Controlla se GameManager esiste già
        GameManager existingManager = Object.FindFirstObjectByType<GameManager>();
        if (existingManager != null)
        {
            Debug.LogWarning("GameManager already exists in scene!");
            return;
        }

        // Crea un GameObject per il GameManager
        GameObject gameManagerObj = new GameObject("GameManager");
        GameManager gameManager = gameManagerObj.AddComponent<GameManager>();

        // Carica i prefab
        GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Player/PlayerCube.prefab");
        GameObject trackPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Track/TrackSegment_01.prefab");

        // Assegna i prefab tramite reflection (per evitare di esporre proprietà pubbliche)
        var playerField = typeof(GameManager).GetField("playerCubePrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var trackField = typeof(GameManager).GetField("trackSegmentPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (playerField != null && playerPrefab != null)
            playerField.SetValue(gameManager, playerPrefab);

        if (trackField != null && trackPrefab != null)
            trackField.SetValue(gameManager, trackPrefab);

        // Salva la scena
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        Debug.Log("✓ GameManager setup complete! Player and Track will spawn at runtime.");
    }

    [MenuItem("Square Spin/Setup/Assign Prefabs to GameManager")]
    public static void AssignPrefabsToGameManager()
    {
        GameManager gameManager = Object.FindFirstObjectByType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in scene! Run 'Setup Game Scene' first.");
            return;
        }

        // Carica i prefab
        GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Player/PlayerCube.prefab");
        GameObject trackPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Track/TrackSegment_01.prefab");

        // Assegna tramite reflection
        var playerField = typeof(GameManager).GetField("playerCubePrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var trackField = typeof(GameManager).GetField("trackSegmentPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (playerField != null && playerPrefab != null)
            playerField.SetValue(gameManager, playerPrefab);

        if (trackField != null && trackPrefab != null)
            trackField.SetValue(gameManager, trackPrefab);

        EditorUtility.SetDirty(gameManager);
        AssetDatabase.Refresh();

        Debug.Log("✓ Prefabs assigned to GameManager");
    }
}
