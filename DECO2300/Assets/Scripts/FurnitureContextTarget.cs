// NOT USED IN FINAL PROTOTYPE
// this script showed the old context menu when the controller pointed at furniture.
// the duplicate and button based delete menu is no longer part of the final user flow.

// lets this old script find the furniture parts and menu objects it needed.
using UnityEngine;

// lets this script know when the controller starts or stops pointing at furniture.
using UnityEngine.XR.Interaction.Toolkit;

// uses the xr grab interactable, which lets the controller interact with furniture in vr.
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// tells the removed context menu when the controller points at or leaves furniture.
public class FurnitureContextTarget : MonoBehaviour
{
    // keeps the furniture's grab controls so the old pointing checks can be connected.
    private XRGrabInteractable grabInteractable;

    // keeps the removed context menu controller found in the scene.
    private FurnitureContextMenuController contextMenu;

    // NOT USED IN FINAL PROTOTYPE
    // this ran when the furniture started and connected it to the removed context menu.
    void Start()
    {
        // gets the grab interaction component from the same furniture object.
        grabInteractable = GetComponent<XRGrabInteractable>();

        // searches the scene for the old context menu controller.
        contextMenu = FindFirstObjectByType<FurnitureContextMenuController>();

        // only starts the old pointing checks when the furniture can be grabbed.
        if (grabInteractable != null)
        {
            // listens for when the controller starts or stops pointing at the furniture.
            grabInteractable.hoverEntered.AddListener(OnHoverEntered);
            grabInteractable.hoverExited.AddListener(OnHoverExited);
        }
    }

    // NOT USED IN FINAL PROTOTYPE
    // this old method showed the context menu when the controller pointed at furniture.
    void OnHoverEntered(HoverEnterEventArgs args)
    {
        // only forwards the furniture when the removed controller was found.
        if (contextMenu != null)
            contextMenu.ShowMenu(gameObject);
    }

    // NOT USED IN FINAL PROTOTYPE
    // this old method began hiding the menu when the controller stopped pointing at furniture.
    void OnHoverExited(HoverExitEventArgs args)
    {
        // only tells the old menu when its controller was found in the scene.
        if (contextMenu != null)
            contextMenu.FurnitureHoverExited(gameObject);
    }

    // NOT USED IN FINAL PROTOTYPE
    // this disconnects the old pointing checks when the furniture is removed.
    void OnDestroy()
    {
        // avoids using the grab component if it no longer exists.
        if (grabInteractable != null)
        {
            grabInteractable.hoverEntered.RemoveListener(OnHoverEntered);
            grabInteractable.hoverExited.RemoveListener(OnHoverExited);
        }
    }
}
