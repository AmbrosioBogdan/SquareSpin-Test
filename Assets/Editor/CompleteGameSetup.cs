using UnityEditor;
using UnityEngine;

public class CompleteGameSetup
{
    [MenuItem("Square Spin/Setup/COMPLETE SETUP (All-in-One)")]
    public static void CompleteSetup()
    {
        Debug.Log("Starting complete game setup...");

        // 1. Setup GameManager
        GameSetupHelper.SetupGameScene();
        
        // 2. Aggiungi Ground Plane
        GroundPlaneSetup.AddGroundPlane();
        
        // 3. Setup ambiente (camera, particelle, collider, luci)
        EnvironmentSetup.SetupEnvironment();

        AssetDatabase.Refresh();
        
        Debug.Log("✅ COMPLETE SETUP FINISHED!");
        Debug.Log("\nPer giocare:");
        Debug.Log("1. Esegui Play");
        Debug.Log("2. Premi SPACE per iniziare");
        Debug.Log("3. Usa A/D o frecce per muoverti");
        Debug.Log("4. Premi R per resettare");
    }
}
