// USED IN FINAL PROTOTYPE
// this script marks an object as furniture so spawning and the remove zone can recognise it safely.
// its older grab response remains in the file, but duplicate and button based delete are not final features.

// lets this script find the furniture controls and older selection manager.
using UnityEngine;

// lets the older code know when the user grabs the furniture.
using UnityEngine.XR.Interaction.Toolkit;

// identifies the whole furniture object and keeps its older selection connection.
public class FurnitureSelectable : MonoBehaviour
{
    // keeps the grab control that lets the older code notice when furniture is picked up.
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    // keeps the old selection manager that supported duplicate and button based delete.
    private FurnitureSelectionManager selectionManager;

    // NOT USED IN FINAL PROTOTYPE
    // this method connects grabbing to the older selection manager.
    // the script itself is still required as the marker checked by safe spawning and the remove zone.
    void Start()
    {
        // gets the grab component attached to this furniture root.
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        // searches for the old manager that stored one selected furniture object.
        selectionManager = FindFirstObjectByType<FurnitureSelectionManager>();

        // only adds the old selection listener when a grab component exists.
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrabbed);
        }
    }

    // NOT USED IN FINAL PROTOTYPE
    // this old method remembered furniture for the removed duplicate and delete buttons.
    void OnGrabbed(SelectEnterEventArgs args)
    {
        // only remembers the grabbed furniture when the older manager exists in the scene.
        if (selectionManager != null)
        {
            selectionManager.SelectFurniture(gameObject);
        }
    }

    // NOT USED IN FINAL PROTOTYPE
    // this disconnects the old grab check when the furniture is removed.
    void OnDestroy()
    {
        // avoids accessing the grab component when it is not available.
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        }
    }
}
