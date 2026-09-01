using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ThemeWandHolder : MonoBehaviour
{
    [Header("References")]
    public XRSocketInteractor socketInteractor;
    public ThemeManager themeManager;

    private ThemeWand currentWand;

    private void Awake()
    {
        if (socketInteractor == null)
            socketInteractor = GetComponent<XRSocketInteractor>();
    }

    private void OnEnable()
    {
        if (socketInteractor != null)
        {
            socketInteractor.selectEntered.AddListener(OnWandInserted);
            socketInteractor.selectExited.AddListener(OnWandRemoved);
        }
    }

    private void OnDisable()
    {
        if (socketInteractor != null)
        {
            socketInteractor.selectEntered.RemoveListener(OnWandInserted);
            socketInteractor.selectExited.RemoveListener(OnWandRemoved);
        }
    }

    private void OnWandInserted(SelectEnterEventArgs args)
    {
        ThemeWand wand =
            args.interactableObject.transform.GetComponent<ThemeWand>();

        if (wand == null)
            wand =
                args.interactableObject.transform.GetComponentInParent<ThemeWand>();

        if (wand == null)
            return;

        currentWand = wand;

        // Tell wand it is safely inside the holder
        wand.SetInHolder(true);

        // Apply theme only after insertion
        wand.ApplyTheme(themeManager);

        Debug.Log(
            "Wand inserted into pedestal: " +
            wand.theme
        );
    }

    private void OnWandRemoved(SelectExitEventArgs args)
    {
        ThemeWand wand =
            args.interactableObject.transform.GetComponent<ThemeWand>();

        if (wand == null)
            wand =
                args.interactableObject.transform.GetComponentInParent<ThemeWand>();

        if (wand == null)
            return;

        wand.SetInHolder(false);

        if (currentWand == wand)
            currentWand = null;

        Debug.Log(
            "Wand removed from pedestal: " +
            wand.theme
        );
    }
}