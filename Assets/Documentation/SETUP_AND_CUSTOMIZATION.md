# Square Spin - Setup & Customization Guide

## 🎮 Setup Iniziale - Come Far Partire il Gioco

### Passo 1: Genera Assets
1. **Square Spin → Generator → Create Track Materials & Prefab**
2. **Square Spin → Generator → Create Player Cube**

### Passo 2: Setup della Scena
1. Apri la scena: **Assets/Scenes/SampleScene.unity**
2. Nel menu: **Square Spin → Setup → Setup Game Scene**

✅ **Fatto!** Quando fai partire il gioco vedrai:
- La pista con 5 segmenti
- Il cubo player al centro
- Tutto immobile per ora (nessun movimento)

---

## 🔧 GameManager - Come Funziona

Il `GameManager` script:
- ✅ Carica e istanzia il prefab player
- ✅ Carica e istanzia multipli segmenti di pista
- ✅ Posiziona tutto correttamente nella scena
- ✅ Fornisce getter pubblici per accedere agli elementi

### Proprietà Configurabili (Inspector)

```
Player Setup
├── Player Spawn Position: Vector3 (default: 0, 0.5, 0)

Track Setup
├── Track Segments Count: int (default: 5)
└── First Track Position: Vector3 (default: 0, 0, 10)
```

### Utilizzo da Script

```csharp
GameManager manager = FindObjectOfType<GameManager>();

// Ottieni il player
GameObject player = manager.GetPlayer();

// Ottieni un segmento specifico
GameObject trackSegment = manager.GetTrackSegment(0);

// Ottieni tutti i segmenti
GameObject[] allTracks = manager.GetAllTrackSegments();

// Ottieni il numero totale
int count = manager.GetTrackSegmentCount();
```

---

## 🎨 Customizer Scripts - Modifica Materiali e Prefab

### ✨ MaterialCustomizer.cs

Script che modifica **proprietà dei materiali** senza dover editare ogni materiale manualmente.

**Menu disponibili:**

```
Square Spin / Customizer /
├── Modify Track Materials        (reset a default)
├── Modify Player Materials       (reset a default)
├── Make Floor Brighter           (aumenta luminosità pavimento)
├── Make Lane Glow Stronger       (aumenta glow blu)
├── Make Side Lights More Purple  (rende viola)
├── Make Player Glow Cyan         (rende cyan)
└── Reset All Materials           (reset totale)
```

**Cosa modifica:**
- Colore base (`_BaseColor`)
- Metallicità (`_Metallic`)
- Smoothness (`_Smoothness`)
- Emissione (`_EmissionColor`)

**Esempio di uso:**
```csharp
// Nel file MaterialCustomizer.cs, aggiungi una nuova funzione:

[MenuItem("Square Spin/Customizer/Make Floor Neon Green")]
public static void MakeFloorNeonGreen()
{
    Material floor = AssetDatabase.LoadAssetAtPath<Material>($"{MATERIALS_FOLDER}/Track/MAT_FloorMetal_Dark.mat");
    if (floor != null)
    {
        floor.SetColor("_BaseColor", new Color(0.0f, 0.5f, 0.2f, 1f));
        floor.SetColor("_EmissionColor", new Color(0.0f, 1.0f, 0.5f, 1f) * 2f);
        floor.EnableKeyword("_EMISSION");
        EditorUtility.SetDirty(floor);
        Debug.Log("✓ Floor is now neon green");
    }
}
```

---

### 🔨 PrefabCustomizer.cs

Script che modifica **struttura e componenti dei prefab**.

**Menu disponibili:**

```
Square Spin / Customizer /
├── Modify Track Prefab Size
├── Modify Player Prefab Size
├── Add More Glow Particles to Track
├── Make Track Wider
├── Make Track Longer
├── Make Player Bigger
└── Info / Show Prefab Modification Info
```

**Cosa modifica:**
- Scale e dimensioni
- Aggiunge/rimuove child objects
- Modifica componenti
- Salva automaticamente nel prefab

**Esempio di uso:**
```csharp
// Nel file PrefabCustomizer.cs, aggiungi:

[MenuItem("Square Spin/Customizer/Add More Lane Dividers")]
public static void AddMoreLaneDividers()
{
    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PREFABS_FOLDER}/Track/TrackSegment_01.prefab");
    if (prefab == null) return;

    GameObject editingPrefab = PrefabUtility.LoadPrefabContents($"{PREFABS_FOLDER}/Track/TrackSegment_01.prefab");
    
    // Crea nuovi divisori
    for (int i = 0; i < 2; i++)
    {
        GameObject divider = new GameObject($"ExtraLaneDivider_{i}");
        divider.transform.parent = editingPrefab.transform;
        divider.transform.localPosition = new Vector3(i - 0.5f, 0.15f, 0);
        
        // Aggiungi mesh, materiale, etc.
    }

    PrefabUtility.SaveAsPrefabAsset(editingPrefab, $"{PREFABS_FOLDER}/Track/TrackSegment_01.prefab");
    PrefabUtility.UnloadPrefabContents(editingPrefab);
    
    Debug.Log("✓ Lane dividers added");
}
```

---

## 🤖 Sì, Si Possono Generare Script per Modificare Asset!

### ✅ Risposta alla Tua Domanda

**Sì, è completamente possibile generare script per modificare l'aspetto di prefab e materiali già creati.**

Ci sono 3 approcci:

---

### **Approccio 1: Script Editor (Menu Context)**
✅ **Quello che abbiamo fatto**

```csharp
[MenuItem("Custom/Modify Something")]
public static void ModifySomething()
{
    // Carica asset
    Material mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/MyMat.mat");
    
    // Modifica proprietà
    mat.SetColor("_BaseColor", new Color(1, 0, 0));
    
    // Salva
    EditorUtility.SetDirty(mat);
    AssetDatabase.SaveAssets();
}
```

**Vantaggi:**
- Esecuzione manuale dal menu
- Facile debug
- Salva direttamente nei file

**Svantaggi:**
- Richiede click nel menu
- Non automatico

---

### **Approccio 2: Script Batch Processor**

```csharp
[MenuItem("Custom/Batch Process Materials")]
public static void BatchProcessAllMaterials()
{
    // Trova tutti i materiali
    string[] guids = AssetDatabase.FindAssets("t:Material", new[] { "Assets/Materials" });
    
    foreach (string guid in guids)
    {
        string path = AssetDatabase.GUIDToAssetPath(guid);
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        
        // Modifica ogni materiale
        if (mat.name.Contains("Glow"))
        {
            mat.SetFloat("_Metallic", 0.5f);
        }
    }
    
    AssetDatabase.SaveAssets();
    Debug.Log("✓ Batch processing complete");
}
```

**Vantaggi:**
- Processa molti asset contemporaneamente
- Pattern potente per modifiche massive

---

### **Approccio 3: InitializeOnLoad Callback**

```csharp
[InitializeOnLoad]
public class AutoMaterialUpdater
{
    static AutoMaterialUpdater()
    {
        EditorApplication.update += OnEditorUpdate;
    }

    private static void OnEditorUpdate()
    {
        // Esegue all'avvio dell'Editor
        // Utile per aggiornamenti automatici
    }
}
```

**Vantaggi:**
- Automatico all'avvio
- Nessun click necessario

---

## 💾 Come Aggiungere Modifiche Personalizzate

### Passo 1: Apri MaterialCustomizer.cs o PrefabCustomizer.cs

### Passo 2: Aggiungi una nuova funzione

```csharp
[MenuItem("Square Spin/Customizer/My Custom Change")]
public static void MyCustomChange()
{
    // Carica asset
    Material myMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Track/MAT_FloorMetal_Dark.mat");
    
    if (myMaterial != null)
    {
        // Modifica
        myMaterial.SetColor("_BaseColor", new Color(0.5f, 0.5f, 0.5f));
        
        // Salva
        EditorUtility.SetDirty(myMaterial);
        Debug.Log("✓ Change applied");
    }
}
```

### Passo 3: Ricompila e usa dal menu

---

## 🎯 Casi d'Uso Comuni

| Caso | Soluzione |
|------|-----------|
| Cambiare colore materiale | MaterialCustomizer + SetColor |
| Modificare scala prefab | PrefabCustomizer + LoadPrefabContents |
| Aggiungere componente a prefab | PrefabCustomizer + AddComponent |
| Processare 100 materiali insieme | Batch script con FindAssets |
| Automatizzare al startup | InitializeOnLoad callback |
| Creare varianti di materiali | Batch duplicator script |

---

## 📝 Esempio Completo: Script per Creare Varianti di Colore

```csharp
using UnityEditor;
using UnityEngine;

public class MaterialVariantGenerator
{
    [MenuItem("Square Spin/Customizer/Generate Color Variants")]
    public static void GenerateColorVariants()
    {
        Material original = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Track/MAT_LaneGlow_Blue.mat");
        
        Color[] variants = new Color[]
        {
            new Color(0.2f, 0.6f, 1.0f),   // Blu
            new Color(1.0f, 0.2f, 0.8f),   // Rosa
            new Color(0.2f, 1.0f, 0.8f),   // Cyan
        };
        
        foreach (Color color in variants)
        {
            Material newMat = new Material(original);
            newMat.SetColor("_BaseColor", color);
            newMat.SetColor("_EmissionColor", color * 2f);
            
            string path = $"Assets/Materials/Track/MAT_LaneGlow_{ColorToName(color)}.mat";
            AssetDatabase.CreateAsset(newMat, path);
        }
        
        AssetDatabase.SaveAssets();
        Debug.Log("✓ Color variants created");
    }
    
    private static string ColorToName(Color c)
    {
        if (c.b > c.g) return "Blue";
        if (c.r > c.g) return "Red";
        return "Green";
    }
}
```

---

## ✨ Riepilogo

✅ **Tutto pronto per giocare:**
- GameManager istanzia player e track
- Nessun movimento ancora
- Logiche pronte per aggiungere azioni

✅ **Customizer script per modifiche rapide:**
- MaterialCustomizer per colori e materiali
- PrefabCustomizer per struttura e componenti
- Facili da estendere con nuovi menu

✅ **Sì, puoi generare script di modifica:**
- Menu context (quello che usi ora)
- Batch processor per asset multipli
- InitializeOnLoad per automazione
- Tutti salvano direttamente nei file
