// NOT USED IN FINAL PROTOTYPE
// this script belongs to an earlier theme wand workflow that applied themes when a wand was grabbed.
// no current final scene or furniture model uses this script.
// the final prototype changes themes only after a wand is placed into the holder.

// let the old wand wait briefly before returning to the wall.
using System.Collections;

// let the old script move the wand, remember its home position and show testing messages.
using UnityEngine;

// let the old script know when the user grabbed or released the wand.
using UnityEngine.XR.Interaction.Toolkit;

// used the xr grab interactable that let the user pick up the old wand in vr.
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// applied a theme immediately on grab and returned the old wand after release.
public class ThemeWandSelector : MonoBehaviour
{
    // lists the three themes available to the older wand selector.
    public enum WandTheme
    {
        // represented the beige, brown and muted red theme.
        Warm,

        // represented the cool blue-grey default theme.
        Modern,

        // represented the charcoal and burgundy theme.
        Dark
    }

    [Header("Theme")]
    // selected which theme the old wand applied when grabbed.
    public WandTheme theme;

    [Header("References")]
    // pointed to the theme manager that changed venue colours in the older workflow.
    public ThemeManager themeManager;

    // kept the grab controls used by the old wand.
    public XRGrabInteractable grabInteractable;

    // remembered where the old wand started compared with its wall holder.
    private Vector3 homePosition;

    // remembered which direction the old wand faced compared with its wall holder.
    private Quaternion homeRotation;

    // NOT USED IN FINAL PROTOTYPE
    // this prepared the old grab controls and remembered where the wand should return.
    private void Awake()
    {
        // finds the grab controls on the old wand when they were not connected in unity.
        if (grabInteractable == null)
        {
            grabInteractable = GetComponent<XRGrabInteractable>();
        }

        // saves the old wand's starting position and direction for returning it later.
        homePosition = transform.localPosition;
        homeRotation = transform.localRotation;
    }

    // NOT USED IN FINAL PROTOTYPE
    // this listened for grabbing and releasing the older wand.
    private void OnEnable()
    {
        // only listens for the user when the old wand has working grab controls.
        if (grabInteractable != null)
        {
            // listened for grab to apply the old theme immediately.
            grabInteractable.selectEntered.AddListener(OnGrabbed);

            // listened for release to return the old wand home.
            grabInteractable.selectExited.AddListener(OnReleased);
        }
    }

    // NOT USED IN FINAL PROTOTYPE
    // this stopped listening for grabs and releases when the old wand became inactive.
    private void OnDisable()
    {
        // only stops listening when the old wand's grab controls still exist.
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }
    }

    // NOT USED IN FINAL PROTOTYPE
    // this older method changed the venue theme as soon as the wand was grabbed.
    private void OnGrabbed(SelectEnterEventArgs args)
    {
        // reports and stops when the old wand is not connected to the theme manager.
        if (themeManager == null)
        {
            Debug.LogWarning("ThemeManager is not assigned.");
            return;
        }

        // checks which theme identity was assigned to the old wand.
        switch (theme)
        {
            // applies the warm theme through the current manager method.
            case WandTheme.Warm:
                themeManager.WarmTheme();
                break;

            // applies the modern theme through the current manager method.
            case WandTheme.Modern:
                themeManager.ModernTheme();
                break;

            // applies the dark theme through the current manager method.
            case WandTheme.Dark:
                themeManager.DarkTheme();
                break;
        }

        // reports the older grab-selected theme in the unity console.
        Debug.Log("Theme selected with wand: " + theme);
    }

    // NOT USED IN FINAL PROTOTYPE
    // this older method began returning the wand after release.
    private void OnReleased(SelectExitEventArgs args)
    {
        StartCoroutine(ReturnHome());
    }

    // NOT USED IN FINAL PROTOTYPE
    // this old method waited one frame and returned the wand to its saved wall placement.
    private IEnumerator ReturnHome()
    {
        // waits one frame so the xr grab system can finish letting go of the old wand.
        yield return null;

        // returns the wand to its saved wall position and direction.
        transform.localPosition = homePosition;
        transform.localRotation = homeRotation;

        // reports the returned old wand in the unity console for testing.
        Debug.Log("Wand returned home: " + theme);
    }
}
