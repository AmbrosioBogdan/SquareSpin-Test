using UnityEngine;
using UnityEditor;

public class MaterialCustomizer
{
    private const string MATERIALS_FOLDER = "Assets/Materials";

    [MenuItem("Square Spin/Customizer/Modify Track Materials")]
    public static void ModifyTrackMaterials()
    {
        ModifyMaterial("Track/MAT_FloorMetal_Dark", new Color(0.15f, 0.15f, 0.15f, 1f), 0.8f, 0.5f, false);
        ModifyMaterial("Track/MAT_Border_BlackSteel", new Color(0.05f, 0.05f, 0.05f, 1f), 0.9f, 0.7f, false);
        ModifyMaterial("Track/MAT_LaneGlow_Blue", new Color(0.1f, 0.3f, 0.8f, 1f), 0.3f, 0.9f, true, new Color(0.2f, 0.6f, 1.0f, 1f) * 2f);
        ModifyMaterial("Track/MAT_SideGlow_Purple", new Color(0.5f, 0.2f, 0.8f, 1f), 0.2f, 0.85f, true, new Color(0.7f, 0.3f, 1.0f, 1f) * 2.5f);
        ModifyMaterial("Track/MAT_Panel_Gray", new Color(0.25f, 0.25f, 0.25f, 1f), 0.5f, 0.4f, false);

        Debug.Log("✓ Track materials modified");
        AssetDatabase.Refresh();
    }

    [MenuItem("Square Spin/Customizer/Modify Player Materials")]
    public static void ModifyPlayerMaterials()
    {
        ModifyMaterial("Player/MAT_Player_CoreMetal", new Color(0.1f, 0.1f, 0.12f, 1f), 0.7f, 0.6f, false);
        ModifyMaterial("Player/MAT_Player_Accent", new Color(0.0f, 0.4f, 0.9f, 1f), 0.8f, 0.9f, true, new Color(0.1f, 0.6f, 1.0f, 1f) * 2f);
        ModifyMaterial("Player/MAT_Player_Glow", new Color(0.3f, 0.8f, 1.0f, 1f), 0.5f, 0.95f, true, new Color(0.5f, 1.0f, 1.0f, 1f) * 3f);

        Debug.Log("✓ Player materials modified");
        AssetDatabase.Refresh();
    }

    private static void ModifyMaterial(string materialPath, Color baseColor, float metallic, float smoothness, bool hasEmission, Color emissionColor = default)
    {
        Material mat = AssetDatabase.LoadAssetAtPath<Material>($"{MATERIALS_FOLDER}/{materialPath}.mat");
        
        if (mat == null)
        {
            Debug.LogError($"Material not found: {materialPath}");
            return;
        }

        mat.SetColor("_BaseColor", baseColor);
        mat.SetFloat("_Metallic", metallic);
        mat.SetFloat("_Smoothness", smoothness);

        if (hasEmission && emissionColor != default)
        {
            mat.SetColor("_EmissionColor", emissionColor);
            mat.EnableKeyword("_EMISSION");
        }

        EditorUtility.SetDirty(mat);
    }

    // Menu per modificare colori specifici
    [MenuItem("Square Spin/Customizer/Make Floor Brighter")]
    public static void MakeFloorBrighter()
    {
        Material floor = AssetDatabase.LoadAssetAtPath<Material>($"{MATERIALS_FOLDER}/Track/MAT_FloorMetal_Dark.mat");
        if (floor != null)
        {
            floor.SetColor("_BaseColor", new Color(0.25f, 0.25f, 0.25f, 1f));
            EditorUtility.SetDirty(floor);
            Debug.Log("✓ Floor is now brighter");
        }
    }

    [MenuItem("Square Spin/Customizer/Make Lane Glow Stronger")]
    public static void MakeLaneGlowStronger()
    {
        Material lane = AssetDatabase.LoadAssetAtPath<Material>($"{MATERIALS_FOLDER}/Track/MAT_LaneGlow_Blue.mat");
        if (lane != null)
        {
            lane.SetColor("_EmissionColor", new Color(0.2f, 0.6f, 1.0f, 1f) * 4f);
            EditorUtility.SetDirty(lane);
            Debug.Log("✓ Lane glow is now stronger");
        }
    }

    [MenuItem("Square Spin/Customizer/Make Side Lights More Purple")]
    public static void MakeSideLightsMorePurple()
    {
        Material side = AssetDatabase.LoadAssetAtPath<Material>($"{MATERIALS_FOLDER}/Track/MAT_SideGlow_Purple.mat");
        if (side != null)
        {
            side.SetColor("_BaseColor", new Color(0.6f, 0.1f, 0.9f, 1f));
            side.SetColor("_EmissionColor", new Color(0.8f, 0.2f, 1.0f, 1f) * 3f);
            EditorUtility.SetDirty(side);
            Debug.Log("✓ Side lights are now more purple");
        }
    }

    [MenuItem("Square Spin/Customizer/Make Player Glow Cyan")]
    public static void MakePlayerGlowCyan()
    {
        Material glow = AssetDatabase.LoadAssetAtPath<Material>($"{MATERIALS_FOLDER}/Player/MAT_Player_Glow.mat");
        if (glow != null)
        {
            glow.SetColor("_BaseColor", new Color(0.2f, 1.0f, 0.9f, 1f));
            glow.SetColor("_EmissionColor", new Color(0.3f, 1.0f, 1.0f, 1f) * 3.5f);
            EditorUtility.SetDirty(glow);
            Debug.Log("✓ Player glow is now cyan");
        }
    }

    [MenuItem("Square Spin/Customizer/Reset All Materials")]
    public static void ResetAllMaterials()
    {
        ModifyTrackMaterials();
        ModifyPlayerMaterials();
        Debug.Log("✓ All materials reset to default");
    }
}
