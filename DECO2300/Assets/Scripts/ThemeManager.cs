using UnityEngine;

public class ThemeManager : MonoBehaviour
{
    [Header("Venue Materials")]
    public Material wallMaterial;
    public Material floorMaterial;

    [Header("Venue Lights")]
    public Light directionalLight;
    public Light fillLight;

    [Header("Brightness")]
    [Range(0f, 1f)]
    public float brightness = 0.6f;

    private float baseDirectionalIntensity = 1f;
    private float baseFillIntensity = 0.5f;

    void Start()
    {
        // Default setup when the game starts
        ModernTheme();
        NormalBrightness();
    }

    // =========================
    // THEME 1 - MINIMALIST WARM
    // =========================
    public void WarmTheme()
    {
        SetMaterialColor(
            wallMaterial,
            new Color(0.82f, 0.76f, 0.65f)
        );

        SetMaterialColor(
            floorMaterial,
            new Color(0.47f, 0.38f, 0.29f)
        );

        if (directionalLight != null)
        {
            directionalLight.color =
                new Color(1f, 0.90f, 0.75f);
        }

        if (fillLight != null)
        {
            fillLight.color =
                new Color(1f, 0.70f, 0.45f);
        }

        baseDirectionalIntensity = 1.05f;
        baseFillIntensity = 0.45f;

        ApplyBrightness();
    }

    // =========================
    // THEME 2 - MODERN
    // =========================
    public void ModernTheme()
    {
        SetMaterialColor(
            wallMaterial,
            new Color(0.86f, 0.86f, 0.84f)
        );

        SetMaterialColor(
            floorMaterial,
            new Color(0.66f, 0.64f, 0.60f)
        );

        if (directionalLight != null)
        {
            directionalLight.color =
                new Color(1f, 0.98f, 0.95f);
        }

        if (fillLight != null)
        {
            fillLight.color =
                new Color(0.90f, 0.95f, 1f);
        }

        baseDirectionalIntensity = 1f;
        baseFillIntensity = 0.4f;

        ApplyBrightness();
    }

    // =========================
    // THEME 3 - DARK EVENT
    // =========================
    public void DarkTheme()
    {
        SetMaterialColor(
            wallMaterial,
            new Color(0.055f, 0.075f, 0.12f)
        );

        SetMaterialColor(
            floorMaterial,
            new Color(0.08f, 0.09f, 0.11f)
        );

        if (directionalLight != null)
        {
            directionalLight.color =
                new Color(0.45f, 0.60f, 1f);
        }

        if (fillLight != null)
        {
            fillLight.color =
                new Color(0.20f, 0.35f, 1f);
        }

        baseDirectionalIntensity = 0.55f;
        baseFillIntensity = 0.75f;

        ApplyBrightness();
    }

    // =========================
    // BRIGHTNESS PRESETS
    // =========================
    public void DimBrightness()
    {
        brightness = 0.25f;
        ApplyBrightness();
    }

    public void NormalBrightness()
    {
        brightness = 0.6f;
        ApplyBrightness();
    }

    public void BrightBrightness()
    {
        brightness = 1f;
        ApplyBrightness();
    }

    // =========================
    // APPLY BRIGHTNESS
    // =========================
    private void ApplyBrightness()
    {
        float multiplier =
            Mathf.Lerp(0.5f, 1.5f, brightness);

        if (directionalLight != null)
        {
            directionalLight.intensity =
                baseDirectionalIntensity * multiplier;
        }

        if (fillLight != null)
        {
            fillLight.intensity =
                baseFillIntensity * multiplier;
        }
    }

    // =========================
    // MATERIAL COLOR
    // =========================
    private void SetMaterialColor(
        Material material,
        Color color
    )
    {
        if (material == null)
            return;

        if (material.HasProperty("_BaseColor"))
        {
            material.SetColor("_BaseColor", color);
        }
        else if (material.HasProperty("_Color"))
        {
            material.SetColor("_Color", color);
        }
    }
}