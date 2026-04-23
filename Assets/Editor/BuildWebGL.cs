using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.IO;

public class BuildWebGL
{
    public static void Build()
    {
        string buildPath = "Build/WebGL/v.1.0.1";
        
        // Crea la cartella se non esiste
        Directory.CreateDirectory(buildPath);
        
        // Setup build options
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] { "Assets/Scenes/SampleScene.unity" };
        buildPlayerOptions.locationPathName = buildPath + "/";
        buildPlayerOptions.target = BuildTarget.WebGL;
        buildPlayerOptions.options = BuildOptions.None;
        
        // Esegui il build
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        
        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log("✅ WebGL Build completato con successo!");
        }
        else
        {
            Debug.LogError("❌ WebGL Build fallito!");
        }
    }
}
