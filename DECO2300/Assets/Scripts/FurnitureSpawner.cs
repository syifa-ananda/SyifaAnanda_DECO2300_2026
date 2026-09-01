using UnityEngine;

public class FurnitureSpawner : MonoBehaviour
{
    public Transform playerCamera;
    public Transform floor;
    public float minimumDistance = 2f;
    public float extraSpace = 1f;

    public void SpawnFurniture(GameObject furniturePrefab)
    {
        GameObject furniture = Instantiate(furniturePrefab);

        Renderer[] renderers = furniture.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
            return;

        Bounds bounds = renderers[0].bounds;

        foreach (Renderer renderer in renderers)
            bounds.Encapsulate(renderer.bounds);

        Vector3 forward = playerCamera.forward;
        forward.y = 0f;
        forward.Normalize();

        float objectRadius = Mathf.Max(bounds.extents.x, bounds.extents.z);

        float distance = Mathf.Max(
            minimumDistance,
            objectRadius + extraSpace
        );

        Vector3 targetPosition =
            playerCamera.position + forward * distance;

        // Center object horizontally
        Vector3 horizontalOffset = new Vector3(
            targetPosition.x - bounds.center.x,
            0f,
            targetPosition.z - bounds.center.z
        );

        furniture.transform.position += horizontalOffset;

        // Recalculate after moving
        renderers = furniture.GetComponentsInChildren<Renderer>();
        bounds = renderers[0].bounds;

        foreach (Renderer renderer in renderers)
            bounds.Encapsulate(renderer.bounds);

        // Put bottom of object exactly on floor
        float floorY = floor.position.y;
        float yOffset = floorY - bounds.min.y;

        furniture.transform.position += Vector3.up * yOffset;
    }
}