using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ThemeWandSelector : MonoBehaviour
{
    public enum WandTheme
    {
        Warm,
        Modern,
        Dark
    }

    [Header("Theme")]
    public WandTheme theme;

    [Header("References")]
    public ThemeManager themeManager;
    public XRGrabInteractable grabInteractable;

    private Vector3 homePosition;
    private Quaternion homeRotation;

    private void Awake()
    {
        // Auto-find XR Grab Interactable kalau belum di-assign
        if (grabInteractable == null)
        {
            grabInteractable = GetComponent<XRGrabInteractable>();
        }

        // Simpan posisi awal wand di dinding
        homePosition = transform.localPosition;
        homeRotation = transform.localRotation;
    }

    private void OnEnable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            grabInteractable.selectExited.AddListener(OnReleased);
        }
    }

    private void OnDisable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (themeManager == null)
        {
            Debug.LogWarning("ThemeManager is not assigned.");
            return;
        }

        switch (theme)
        {
            case WandTheme.Warm:
                themeManager.WarmTheme();
                break;

            case WandTheme.Modern:
                themeManager.ModernTheme();
                break;

            case WandTheme.Dark:
                themeManager.DarkTheme();
                break;
        }

        Debug.Log("Theme selected with wand: " + theme);
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        StartCoroutine(ReturnHome());
    }

    private IEnumerator ReturnHome()
    {
        // Tunggu XR Grab selesai release dulu
        yield return null;

        transform.localPosition = homePosition;
        transform.localRotation = homeRotation;

        Debug.Log("Wand returned home: " + theme);
    }
}