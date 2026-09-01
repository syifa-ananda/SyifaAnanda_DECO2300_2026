using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ThemeWand : MonoBehaviour
{
    public enum WandTheme
    {
        Warm,
        Modern,
        Dark
    }

    [Header("Theme Type")]
    public WandTheme theme;

    [Header("XR")]
    public XRGrabInteractable grabInteractable;

    private Vector3 homePosition;
    private Quaternion homeRotation;

    private bool isInHolder = false;
    private Coroutine returnCoroutine;

    private void Awake()
    {
        if (grabInteractable == null)
            grabInteractable = GetComponent<XRGrabInteractable>();

        // Save original wall position
        homePosition = transform.position;
        homeRotation = transform.rotation;
    }

    private void OnEnable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectExited.AddListener(OnReleased);
        }
    }

    private void OnDisable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }
    }

    public void ApplyTheme(ThemeManager themeManager)
    {
        if (themeManager == null)
            return;

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

        Debug.Log("Applied theme: " + theme);
    }

    public void SetInHolder(bool value)
    {
        isInHolder = value;

        if (isInHolder && returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
            returnCoroutine = null;
        }
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        if (returnCoroutine != null)
            StopCoroutine(returnCoroutine);

        returnCoroutine = StartCoroutine(CheckReturnHome());
    }

    private IEnumerator CheckReturnHome()
    {
        // Give the socket time to catch the wand
        yield return new WaitForSeconds(0.1f);

        if (!isInHolder)
        {
            ReturnHome();
        }

        returnCoroutine = null;
    }

    public void ReturnHome()
    {
        transform.SetPositionAndRotation(
            homePosition,
            homeRotation
        );

        Debug.Log(
            "Returned wand to wall: " + theme
        );
    }
}