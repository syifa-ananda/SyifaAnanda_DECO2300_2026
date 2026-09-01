using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FurnitureSelectable : MonoBehaviour
{
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private FurnitureSelectionManager selectionManager;

    void Start()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        selectionManager = FindFirstObjectByType<FurnitureSelectionManager>();

        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrabbed);
        }
    }

    void OnGrabbed(SelectEnterEventArgs args)
    {
        if (selectionManager != null)
        {
            selectionManager.SelectFurniture(gameObject);
        }
    }

    void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        }
    }
}