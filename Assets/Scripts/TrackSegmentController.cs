using UnityEngine;

public class TrackSegmentController : MonoBehaviour
{
    [Header("Glow Animation")]
    [SerializeField] private bool animateGlow = true;
    [SerializeField] private float glowSpeed = 2f;
    [SerializeField] private float glowIntensityMin = 1.5f;
    [SerializeField] private float glowIntensityMax = 3f;

    private Material[] glowMaterials;
    private Color[] originalEmissionColors;

    private void Start()
    {
        // Raccogli tutti i materiali emissivi
        FindGlowMaterials();
    }

    private void FindGlowMaterials()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        System.Collections.Generic.List<Material> glowList = new System.Collections.Generic.List<Material>();
        System.Collections.Generic.List<Color> colorList = new System.Collections.Generic.List<Color>();

        foreach (Renderer renderer in renderers)
        {
            foreach (Material mat in renderer.materials)
            {
                if (mat.HasProperty("_EmissionColor"))
                {
                    Color emissionColor = mat.GetColor("_EmissionColor");
                    // Controlla se il materiale ha effettivamente emissione
                    if (emissionColor.r + emissionColor.g + emissionColor.b > 0.1f)
                    {
                        glowList.Add(mat);
                        colorList.Add(emissionColor);
                    }
                }
            }
        }

        glowMaterials = glowList.ToArray();
        originalEmissionColors = colorList.ToArray();
    }

    private void Update()
    {
        if (animateGlow && glowMaterials != null && glowMaterials.Length > 0)
        {
            float pulse = Mathf.Lerp(glowIntensityMin, glowIntensityMax, (Mathf.Sin(Time.time * glowSpeed) + 1f) * 0.5f);

            for (int i = 0; i < glowMaterials.Length; i++)
            {
                if (glowMaterials[i] != null)
                {
                    glowMaterials[i].SetColor("_EmissionColor", originalEmissionColors[i] * pulse);
                }
            }
        }
    }

    public void SetGlowAnimation(bool enabled)
    {
        animateGlow = enabled;
    }

    public void SetGlowSpeed(float speed)
    {
        glowSpeed = speed;
    }
}
