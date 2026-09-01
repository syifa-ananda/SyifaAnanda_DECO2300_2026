using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class FurnitureContextTarget : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    private FurnitureContextMenuController contextMenu;

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        contextMenu = FindFirstObjectByType<FurnitureContextMenuController>();

        if (grabInteractable != null)
        {
            grabInteractable.hoverEntered.AddListener(OnHoverEntered);
            grabInteractable.hoverExited.AddListener(OnHoverExited);
        }
    }

    void OnHoverEntered(HoverEnterEventArgs args)
    {
        if (contextMenu != null)
            contextMenu.ShowMenu(gameObject);
    }

    void OnHoverExited(HoverExitEventArgs args)
    {
        if (contextMenu != null)
            contextMenu.FurnitureHoverExited(gameObject);
    }

    void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.hoverEntered.RemoveListener(OnHoverEntered);
            grabInteractable.hoverExited.RemoveListener(OnHoverExited);
        }
    }
}