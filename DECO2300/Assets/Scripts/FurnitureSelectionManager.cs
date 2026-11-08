// NOT USED IN FINAL PROTOTYPE
// this script stored a furniture selection for the old duplicate and button based delete actions.
// those buttons were removed, while current deletion is handled by the physical remove zone.

// lets the old system remember, measure, copy and remove furniture objects.
using UnityEngine;

// remembers furniture for the removed duplicate and delete button system.
public class FurnitureSelectionManager : MonoBehaviour
{
    // stored the furniture selected by the old action buttons.
    public GameObject selectedFurniture;

    // NOT USED IN FINAL PROTOTYPE
    // this method remembered a grabbed object for the removed duplicate and delete buttons.
    public void SelectFurniture(GameObject furniture)
    {
        selectedFurniture = furniture;

        // shows the selected furniture name in the unity console for testing.
        Debug.Log("Selected: " + furniture.name);
    }

    // NOT USED IN FINAL PROTOTYPE
    // this function belonged to the old button based delete option.
    public void DeleteSelected()
    {
        // reports and stops when the old system has no selected furniture.
        if (selectedFurniture == null)
        {
            Debug.Log("No furniture selected.");
            return;
        }

        // removes the object directly through the old button action.
        Destroy(selectedFurniture);

        // forgets the selected furniture after it has been removed.
        selectedFurniture = null;
    }

    // NOT USED IN FINAL PROTOTYPE
    // this function belonged to the old duplicate button.
    // the final prototype adds more furniture through the event planner.
    public void DuplicateSelected()
    {
        // reports and stops when the old system has no selected furniture.
        if (selectedFurniture == null)
        {
            Debug.Log("No furniture selected.");
            return;
        }

        // finds every visible part to work out how far the old copy should move sideways.
        Renderer[] renderers = selectedFurniture.GetComponentsInChildren<Renderer>();

        // starts with a basic distance when the furniture has no visible parts to measure.
        float offsetDistance = 1f;

        // chooses a better distance when the visible furniture can be measured.
        if (renderers.Length > 0)
        {
            // starts measuring the furniture with its first visible part.
            Bounds bounds = renderers[0].bounds;

            // includes all other visible parts in the full furniture size.
            foreach (Renderer renderer in renderers)
                bounds.Encapsulate(renderer.bounds);

            // uses the widest horizontal dimension and adds a small gap.
            offsetDistance = Mathf.Max(bounds.size.x, bounds.size.z) + 0.3f;
        }

        // chooses a place beside the selected furniture on its right side.
        Vector3 newPosition =
            selectedFurniture.transform.position +
            selectedFurniture.transform.right * offsetDistance;

        // creates the old copy beside the furniture facing the same direction.
        GameObject copy = Instantiate(
            selectedFurniture,
            newPosition,
            selectedFurniture.transform.rotation
        );

        // gives the duplicate a readable copy name.
        copy.name = selectedFurniture.name.Replace("(Clone)", "") + " Copy";

        // makes the new copy the old system's current selection.
        selectedFurniture = copy;
    }
}
