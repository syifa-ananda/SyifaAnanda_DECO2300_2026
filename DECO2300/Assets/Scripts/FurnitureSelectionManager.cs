using UnityEngine;

public class FurnitureSelectionManager : MonoBehaviour
{
    public GameObject selectedFurniture;

    public void SelectFurniture(GameObject furniture)
    {
        selectedFurniture = furniture;
        Debug.Log("Selected: " + furniture.name);
    }

    public void DeleteSelected()
    {
        if (selectedFurniture == null)
        {
            Debug.Log("No furniture selected.");
            return;
        }

        Destroy(selectedFurniture);
        selectedFurniture = null;
    }

    public void DuplicateSelected()
    {
        if (selectedFurniture == null)
        {
            Debug.Log("No furniture selected.");
            return;
        }

        // Cari ukuran object supaya copy tidak muncul tepat menumpuk
        Renderer[] renderers = selectedFurniture.GetComponentsInChildren<Renderer>();

        float offsetDistance = 1f;

        if (renderers.Length > 0)
        {
            Bounds bounds = renderers[0].bounds;

            foreach (Renderer renderer in renderers)
                bounds.Encapsulate(renderer.bounds);

            offsetDistance = Mathf.Max(bounds.size.x, bounds.size.z) + 0.3f;
        }

        Vector3 newPosition =
            selectedFurniture.transform.position +
            selectedFurniture.transform.right * offsetDistance;

        GameObject copy = Instantiate(
            selectedFurniture,
            newPosition,
            selectedFurniture.transform.rotation
        );

        copy.name = selectedFurniture.name.Replace("(Clone)", "") + " Copy";

        selectedFurniture = copy;
    }
}