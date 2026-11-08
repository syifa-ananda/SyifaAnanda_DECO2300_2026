// USED IN FINAL PROTOTYPE
// this script applies the modern, warm and dark venue themes.
// it changes the walls, floor, ceiling, carpet, windows, lights and ui to match the chosen theme.
// modern is applied as the default theme when the final scene starts.

// lets this script change the visible room surfaces, lights and colours.
using UnityEngine;

// lets this script change the event planner panel and button colours.
using UnityEngine.UI;

// lets this script change the text colours in the event planner and guide.
using TMPro;

// changes the room and interface colours to match the theme chosen by the user.
public class ThemeManager : MonoBehaviour
{
    // keeps the five surface materials needed to create one complete venue theme.
    [System.Serializable]
    public class EnvironmentThemeMaterials
    {
        // keeps the material used by the back, left and right venue walls.
        public Material wall;

        // keeps the material used by the venue floor.
        public Material floor;

        // keeps the material used by the decorative venue ceiling.
        public Material ceiling;

        // keeps the material used by the carpet in the central aisle.
        public Material aisle;

        // keeps the material shown on every visible part of the two right wall windows.
        public Material window;
    }

    [Header("Legacy Venue Materials")]
    // keeps an older wall material connection so existing scenes continue to work.
    // the current theme methods use the dedicated material groups below instead.
    [HideInInspector]
    public Material wallMaterial;

    // keeps an older floor material connection so existing scenes continue to work.
    // the current theme choices do not use this older value.
    [HideInInspector]
    public Material floorMaterial;

    [Header("Environment Renderers")]
    // keeps the three visible venue walls whose colours change with the selected theme.
    [SerializeField] private Renderer[] wallRenderers;

    // keeps the visible main floor whose colour changes with the selected theme.
    [SerializeField] private Renderer floorRenderer;

    // keeps the visible decorative ceiling whose colour changes with the selected theme.
    [SerializeField] private Renderer ceilingRenderer;

    // keeps the visible central aisle carpet whose colour changes with the selected theme.
    [SerializeField] private Renderer aisleRenderer;

    // keeps both complete windows so every visible frame part can change with the theme.
    [SerializeField] private Transform[] windowRoots;

    [Header("Environment Theme Materials")]
    // contains the cool blue-grey materials used by the default modern theme.
    [SerializeField] private EnvironmentThemeMaterials modernMaterials;

    // contains the beige, brown and muted red materials used by the warm theme.
    [SerializeField] private EnvironmentThemeMaterials warmMaterials;

    // contains the charcoal, burgundy and muted purple materials used by the dark theme.
    [SerializeField] private EnvironmentThemeMaterials darkMaterials;

    [Header("Theme-Responsive UI")]
    // keeps the event planner background so its colour follows the active theme.
    [SerializeField] private Image eventPlannerPanel;

    // keeps the thin event planner divider that shows the active accent colour.
    [SerializeField] private Image eventPlannerDivider;

    // keeps the five event planner buttons whose normal and pressed colours follow the theme.
    [SerializeField] private Button[] eventPlannerButtons;

    // keeps the event planner title and labels readable in every theme.
    [SerializeField] private TMP_Text[] eventPlannerTexts;

    // keeps the guide text so each section heading can follow the theme accent.
    [SerializeField] private TMP_Text guideContent;

    [Header("Venue Lights")]
    // keeps the main room light whose colour changes with the venue theme.
    public Light directionalLight;

    // keeps the softer room light whose colour also changes with the venue theme.
    public Light fillLight;

    [Header("Brightness")]
    // remembers the brightness level used by both room lights.
    [Range(0f, 1f)]
    public float brightness = 0.6f;

    // remembers the active theme's normal main light strength before the lever changes it.
    private float baseDirectionalIntensity = 1f;

    // remembers the active theme's normal softer light strength before the lever changes it.
    private float baseFillIntensity = 0.5f;

    // runs when the theme system starts in the final scene.
    // it makes modern the default theme and begins at the normal brightness preset.
    void Start()
    {
        // applies the cool modern environment and ui before the user chooses a wand.
        ModernTheme();

        // sets the shared brightness value to the existing normal level.
        NormalBrightness();
    }

    // runs when the warm theme wand is inserted into the current holder.
    // it applies beige, brown and muted red environment colours with warm ui styling.
    public void WarmTheme()
    {
        // changes the room surfaces to the five warm theme materials.
        ApplyEnvironmentMaterials(warmMaterials);

        // changes the interface to a dark brown panel, light text, coral accents and warm buttons.
        ApplyUITheme(
            // sets the warm panel and readable primary text colours.
            Rgb(36, 27, 23, 230),
            Rgb(255, 247, 238),

            // sets the coral accent shared by the divider and guide headings.
            Rgb(232, 117, 82),

            // sets how warm buttons look normally, when pointed at and when pressed.
            Rgb(26, 20, 17),
            Rgb(73, 48, 39),
            Rgb(113, 67, 50));

        // changes the main light to a soft warm colour when it has been connected.
        if (directionalLight != null)
        {
            directionalLight.color =
                new Color(1f, 0.90f, 0.75f);
        }

        // changes the fill light to a warmer orange tone when available.
        if (fillLight != null)
        {
            fillLight.color =
                new Color(1f, 0.70f, 0.45f);
        }

        // remembers the normal warm light strengths before the lever changes brightness.
        baseDirectionalIntensity = 1.05f;
        baseFillIntensity = 0.45f;

        // keeps the user's current brightness after changing to the warm theme.
        ApplyBrightness();
    }

    // runs at startup or when the modern wand is inserted into the holder.
    // it applies the cool blue-grey environment and matching blue ui accent.
    public void ModernTheme()
    {
        // changes the room surfaces to the five modern theme materials.
        ApplyEnvironmentMaterials(modernMaterials);

        // changes the interface to a navy panel, light text, blue accents and modern buttons.
        ApplyUITheme(
            // sets the modern panel and readable primary text colours.
            Rgb(18, 27, 38, 230),
            Rgb(244, 247, 250),

            // sets the blue accent shared by the divider and guide headings.
            Rgb(88, 166, 231),

            // sets how modern buttons look normally, when pointed at and when pressed.
            Rgb(13, 21, 31),
            Rgb(32, 56, 77),
            Rgb(49, 90, 120));

        // uses a near-neutral colour for the main light in the default experience.
        if (directionalLight != null)
        {
            directionalLight.color =
                new Color(1f, 0.98f, 0.95f);
        }

        // uses a subtle cool colour for the modern fill light.
        if (fillLight != null)
        {
            fillLight.color =
                new Color(0.90f, 0.95f, 1f);
        }

        // remembers the normal modern light strengths before the lever changes brightness.
        baseDirectionalIntensity = 1f;
        baseFillIntensity = 0.4f;

        // keeps the user's current brightness after changing to the modern theme.
        ApplyBrightness();
    }

    // runs when the dark theme wand is inserted into the current holder.
    // it applies charcoal and burgundy surfaces with a restrained purple ui accent.
    public void DarkTheme()
    {
        // changes the room surfaces to the five dark theme materials.
        ApplyEnvironmentMaterials(darkMaterials);

        // changes the interface to a dark panel, light text, purple accents and dark buttons.
        ApplyUITheme(
            // sets the dark panel and readable primary text colours.
            Rgb(18, 19, 27, 230),
            Rgb(247, 244, 250),

            // sets the purple accent shared by the divider and guide headings.
            Rgb(166, 90, 217),

            // sets how dark buttons look normally, when pointed at and when pressed.
            Rgb(11, 12, 18),
            Rgb(48, 35, 58),
            Rgb(73, 48, 84));

        // changes the main light to a darker cool colour when available.
        if (directionalLight != null)
        {
            directionalLight.color =
                new Color(0.45f, 0.60f, 1f);
        }

        // gives the dark theme a stronger blue fill light when available.
        if (fillLight != null)
        {
            fillLight.color =
                new Color(0.20f, 0.35f, 1f);
        }

        // remembers the normal dark theme light strengths before the lever changes brightness.
        baseDirectionalIntensity = 0.55f;
        baseFillIntensity = 0.75f;

        // keeps the user's current brightness after changing to the dark theme.
        ApplyBrightness();
    }

    // NOT USED IN FINAL PROTOTYPE
    // this preset belonged to an earlier button based brightness option.
    // the final user controls brightness with the physical lighting lever.
    public void DimBrightness()
    {
        // remembers the old dim setting and applies it to both room lights.
        brightness = 0.25f;
        ApplyBrightness();
    }

    // runs during startup to establish the manager's existing normal brightness value.
    public void NormalBrightness()
    {
        // remembers the normal setting and applies it to both room lights.
        brightness = 0.6f;
        ApplyBrightness();
    }

    // NOT USED IN FINAL PROTOTYPE
    // this preset belonged to an earlier button based brightness option.
    // the final user controls brightness with the physical lighting lever.
    public void BrightBrightness()
    {
        // remembers the old bright setting and applies it to both room lights.
        brightness = 1f;
        ApplyBrightness();
    }

    // changes both room lights using the current theme and saved brightness setting.
    private void ApplyBrightness()
    {
        // turns the saved brightness into a light strength from half power to one and a half power.
        float multiplier =
            Mathf.Lerp(0.5f, 1.5f, brightness);

        // changes the main room light only when it has been connected in the scene.
        if (directionalLight != null)
        {
            directionalLight.intensity =
                baseDirectionalIntensity * multiplier;
        }

        // changes the softer room light only when it has been connected in the scene.
        if (fillLight != null)
        {
            fillLight.intensity =
                baseFillIntensity * multiplier;
        }
    }

    // changes the visible room materials to the selected theme.
    private void ApplyEnvironmentMaterials(
        EnvironmentThemeMaterials materials)
    {
        // stops when the selected theme materials were not connected in unity.
        if (materials == null)
            return;

        // changes the back, left and right walls when their list is available.
        if (wallRenderers != null)
        {
            // gives every venue wall the same colour and surface from the active theme.
            foreach (Renderer wallRenderer in wallRenderers)
                SetRendererMaterial(wallRenderer, materials.wall);
        }

        // gives the floor, ceiling and central carpet their matching theme materials.
        SetRendererMaterial(floorRenderer, materials.floor);
        SetRendererMaterial(ceilingRenderer, materials.ceiling);
        SetRendererMaterial(aisleRenderer, materials.aisle);

        // stops after the main surfaces when the complete window objects were not connected.
        if (windowRoots == null)
            return;

        // checks both right wall windows so every visible frame part is included.
        foreach (Transform windowRoot in windowRoots)
        {
            // skips a missing window while still changing the other window.
            if (windowRoot == null)
                continue;

            // finds every visible window part, including parts that are currently hidden.
            Renderer[] windowRenderers =
                windowRoot.GetComponentsInChildren<Renderer>(true);

            // gives each visible window part the matching material from the active theme.
            foreach (Renderer windowRenderer in windowRenderers)
                SetRendererMaterial(windowRenderer, materials.window);
        }
    }

    // safely changes one visible room part to one prepared theme material.
    private static void SetRendererMaterial(
        Renderer targetRenderer,
        Material material)
    {
        // stops when either the visible room part or selected material is missing.
        if (targetRenderer == null || material == null)
            return;

        // uses the prepared material so the script does not create unnecessary copies.
        targetRenderer.sharedMaterial = material;
    }

    // changes the event planner and guide colours to match one venue theme.
    private void ApplyUITheme(
        Color panelColor,
        Color primaryTextColor,
        Color accentColor,
        Color buttonNormalColor,
        Color buttonHoverColor,
        Color buttonPressedColor)
    {
        // changes the event planner background when it has been connected in unity.
        if (eventPlannerPanel != null)
            eventPlannerPanel.color = panelColor;

        // changes the thin divider to the theme's blue, coral or purple accent.
        if (eventPlannerDivider != null)
            eventPlannerDivider.color = accentColor;

        // only changes buttons when their list has been connected in unity.
        if (eventPlannerButtons != null)
        {
            // gives table, chair, stage, sofa and plant buttons the same theme colours.
            foreach (Button button in eventPlannerButtons)
            {
                // skips a missing button without stopping the other interface colours from changing.
                if (button == null)
                    continue;

                // keeps the button's current colour settings so unrelated details stay unchanged.
                ColorBlock colors = button.colors;

                // sets how the button looks normally, when pointed at, pressed or selected.
                colors.normalColor = buttonNormalColor;
                colors.highlightedColor = buttonHoverColor;

                // uses the stronger pressed colour and matches the selected colour to pointing.
                colors.pressedColor = buttonPressedColor;
                colors.selectedColor = buttonHoverColor;

                // gives the finished set of colours back to this button.
                button.colors = colors;

                // keeps the base image white so the theme colours appear correctly.
                if (button.targetGraphic != null)
                    button.targetGraphic.color = Color.white;
            }
        }

        // only changes labels when the event planner text list is available.
        if (eventPlannerTexts != null)
        {
            // keeps the title and all five button labels readable using light text.
            foreach (TMP_Text text in eventPlannerTexts)
            {
                // skips missing text without stopping the remaining labels from changing.
                if (text != null)
                    text.color = primaryTextColor;
            }
        }

        // gives the guide section headings the same active accent colour.
        SetGuideAccent(accentColor);
    }

    // changes only known guide heading colours while leaving all wording and body text unchanged.
    private void SetGuideAccent(Color accentColor)
    {
        // stops when the guide text was not connected in the scene.
        if (guideContent == null)
            return;

        // prepares the active colour in the special text format used by the guide headings.
        string accentTag =
            $"<color=#{ColorUtility.ToHtmlStringRGB(accentColor)}>";

        // copies the current guide content before replacing only known heading colours.
        string updatedText = guideContent.text;

        // lists every old theme colour that a guide heading may currently use.
        string[] knownAccentTags =
        {
            "<color=#8ADFFF>",
            "<color=#58A6E7>",
            "<color=#E87552>",
            "<color=#A65AD9>"
        };

        // replaces every known heading colour with the newly selected theme accent.
        foreach (string knownTag in knownAccentTags)
            updatedText = updatedText.Replace(knownTag, accentTag);

        // shows the updated colours without changing the guide wording or layout.
        guideContent.text = updatedText;
    }

    // creates an exact interface colour from simple red, green, blue and transparency values.
    private static Color32 Rgb(byte red, byte green, byte blue, byte alpha = 255)
    {
        // returns the colour used by the current theme's ui settings.
        return new Color32(red, green, blue, alpha);
    }
}
