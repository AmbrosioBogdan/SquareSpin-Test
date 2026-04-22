using UnityEngine;
using UnityEditor;

public class GroundPlaneSetup
{
    [MenuItem("Square Spin/Setup/Add Ground Plane")]
    public static void AddGroundPlane()
    {
        // Controlla se esiste già
        GameObject existingGround = GameObject.Find("GroundPlane");
        if (existingGround != null)
        {
            Debug.LogWarning("Ground plane already exists!");
            return;
        }

        // Crea il piano di terra
        GameObject groundPlane = new GameObject("GroundPlane");
        
        // Aggiungi collider invisibile
        BoxCollider collider = groundPlane.AddComponent<BoxCollider>();
        collider.size = new Vector3(50, 0.5f, 100); // Largo e lungo per coprire tutta la pista
        collider.isTrigger = false;
        
        // Posiziona sotto la pista
        groundPlane.transform.position = new Vector3(0, -1, 0);

        // Rendi invisibile
        groundPlane.layer = LayerMask.NameToLayer("Default");

        Debug.Log("✓ Ground plane added at position (0, -1, 0)");
    }
}
