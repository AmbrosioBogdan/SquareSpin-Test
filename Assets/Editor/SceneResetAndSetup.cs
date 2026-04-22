using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

public class SceneResetAndSetup
{
    [MenuItem("Square Spin/Setup/RESET Scene and Setup")]
    public static void ResetAndSetup()
    {
        // Pulisci oggetti vecchi
        CleanupScene();
        
        // Esegui setup completo
        CompleteGameSetup.CompleteSetup();
        
        // Salva la scena
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        
        Debug.Log("✅ Scene reset and setup complete!");
    }

    private static void CleanupScene()
    {
        // Elimina GameManager
        GameObject gm = GameObject.Find("GameManager");
        if (gm != null) Object.DestroyImmediate(gm);

        // Elimina Environment
        GameObject env = GameObject.Find("Environment");
        if (env != null) Object.DestroyImmediate(env);

        // Elimina GroundPlane
        GameObject ground = GameObject.Find("GroundPlane");
        if (ground != null) Object.DestroyImmediate(ground);

        // Elimina Player e Track dai vecchi istanziamenti
        GameObject player = GameObject.Find("Player");
        if (player != null) Object.DestroyImmediate(player);

        for (int i = 0; i < 10; i++)
        {
            GameObject track = GameObject.Find($"TrackSegment_{i}");
            if (track != null) Object.DestroyImmediate(track);
        }

        Debug.Log("✓ Scene cleaned up");
    }
}
