using UnityEditor;
using UnityEngine;

public class ViewAdjuster
{
    [MenuItem("Square Spin/View/Fix Camera & Player Position")]
    public static void FixCameraAndPlayer()
    {
        // Aggiusta la camera
        Camera mainCamera = Object.FindFirstObjectByType<Camera>();
        if (mainCamera != null)
        {
            // Posiziona più vicino per vedere la pista meglio
            mainCamera.transform.position = new Vector3(0, 8, -3);
            mainCamera.transform.LookAt(new Vector3(0, 1, 5));
            mainCamera.fieldOfView = 50f;
            Debug.Log("✓ Camera adjusted");
        }

        // Aggiusta la posizione del player
        GameManager gameManager = Object.FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            var playerField = typeof(GameManager).GetField("playerSpawnPosition", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (playerField != null)
            {
                playerField.SetValue(gameManager, new Vector3(0, 1.5f, 0));
                Debug.Log("✓ Player spawn position updated to center of track");
            }
        }

        Debug.Log("✓ View adjustment complete!");
    }
}
