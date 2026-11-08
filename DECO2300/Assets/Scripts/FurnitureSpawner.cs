// USED IN FINAL PROTOTYPE
// this script handles the five furniture buttons in the event planner.
// it creates a table, chair, stage, sofa or plant in front of the player and looks for clear space nearby.

// lets this script create furniture, measure its size, detect obstacles and keep it stable.
using UnityEngine;

// creates the furniture chosen from the event planner and places it safely on the venue floor.
public class FurnitureSpawner : MonoBehaviour
{
    // adds a small gap so newly created furniture does not almost touch another object.
    private const float SpawnClearance = 0.08f;

    // sets the distance between each nearby position checked around the normal spawn point.
    private const float SearchStep = 0.25f;

    // limits how far the safe spawn search can move away from the intended position.
    private const float MaximumSearchRadius = 3f;

    // lists the directions checked while looking for a clear place near the normal spawn area.
    // cardinal, diagonal and wider angle directions give the search several nearby options.
    private static readonly Vector2[] SearchDirections =
    {
        // checks the four straight directions around the normal spawn point.
        Vector2.right,
        Vector2.left,
        Vector2.up,
        Vector2.down,

        // checks the four even diagonal directions between those straight directions.
        new Vector2(1f, 1f).normalized,
        new Vector2(-1f, 1f).normalized,
        new Vector2(1f, -1f).normalized,
        new Vector2(-1f, -1f).normalized,

        // checks four wider x directions to find gaps that the first eight paths may miss.
        new Vector2(2f, 1f).normalized,
        new Vector2(-2f, 1f).normalized,
        new Vector2(2f, -1f).normalized,
        new Vector2(-2f, -1f).normalized,

        // checks four wider z directions to complete the nearby search pattern.
        new Vector2(1f, 2f).normalized,
        new Vector2(-1f, 2f).normalized,
        new Vector2(1f, -2f).normalized,
        new Vector2(-1f, -2f).normalized
    };

    // keeps the player's camera position and direction so furniture appears in front of them.
    public Transform playerCamera;

    // keeps the venue floor used to place furniture at the right height and inside the room.
    public Transform floor;

    // sets the closest allowed distance between the player and a new furniture object.
    public float minimumDistance = 2f;

    // adds space beyond the new object's radius so large furniture is not too close.
    public float extraSpace = 1f;

    // runs when the user presses a furniture button in the event planner.
    // each menu button sends its table, chair, stage, sofa or plant model into this method.
    public void SpawnFurniture(GameObject furniturePrefab)
    {
        // creates one furniture object from the model chosen by the menu button.
        GameObject furniture = Instantiate(furniturePrefab);

        // finds every visible part so the complete furniture size can be measured.
        Renderer[] renderers = furniture.GetComponentsInChildren<Renderer>();

        // stops when the furniture model has no visible part that can be measured.
        if (renderers.Length == 0)
            return;

        // starts measuring the furniture with its first visible part.
        Bounds bounds = renderers[0].bounds;

        // includes every other visible part in the full furniture size.
        foreach (Renderer renderer in renderers)
            bounds.Encapsulate(renderer.bounds);

        // gets the direction the player is currently facing.
        Vector3 forward = playerCamera.forward;

        // ignores whether the player is looking up or down so furniture stays on the floor.
        forward.y = 0f;
        forward.Normalize();

        // uses the wider side to estimate how much room the new furniture needs.
        float objectRadius = Mathf.Max(bounds.extents.x, bounds.extents.z);

        // keeps the spawn at least the normal distance away and gives larger objects extra room.
        float distance = Mathf.Max(
            minimumDistance,
            objectRadius + extraSpace
        );

        // chooses the normal furniture spawn point directly in front of the player.
        Vector3 targetPosition =
            playerCamera.position + forward * distance;

        // finds how far the visible centre must move to reach the horizontal target point.
        Vector3 horizontalOffset = new Vector3(
            targetPosition.x - bounds.center.x,
            0f,
            targetPosition.z - bounds.center.z
        );

        // moves the whole new furniture object sideways without changing its height yet.
        furniture.transform.position += horizontalOffset;

        // measures the visible parts again after moving the furniture sideways.
        renderers = furniture.GetComponentsInChildren<Renderer>();
        bounds = renderers[0].bounds;

        // measures the complete furniture size again at its new position.
        foreach (Renderer renderer in renderers)
            bounds.Encapsulate(renderer.bounds);

        // reads the venue floor height used by the current scene.
        float floorY = floor.position.y;

        // calculates the vertical movement needed to place the visible bottom on the floor.
        float yOffset = floorY - bounds.min.y;

        // lifts or lowers the furniture so it rests on the floor surface.
        furniture.transform.position += Vector3.up * yOffset;

        // tries a nearby position if another furniture object already uses the normal spawn area.
        PlaceAtNearestFreePosition(furniture);

        // stops any unwanted movement so the new furniture starts still and stable.
        ResetSpawnMotion(furniture);
    }

    // runs after the new furniture has been placed on the floor.
    // it keeps the normal position when clear or checks nearby offsets when occupied.
    private void PlaceAtNearestFreePosition(GameObject furniture)
    {
        // updates unity so every invisible collision shape matches the furniture's latest position.
        Physics.SyncTransforms();

        // stops the safety check when the furniture has no active solid collision shape to measure.
        if (!TryGetColliderBounds(furniture, out Bounds furnitureBounds))
            return;

        // finds the marker that identifies the newly created object as furniture.
        FurnitureSelectable spawnedFurniture =
            furniture.GetComponentInChildren<FurnitureSelectable>();

        // finds every active furniture object already placed in the venue.
        FurnitureSelectable[] existingFurniture =
            FindObjectsByType<FurnitureSelectable>(
                FindObjectsInactive.Exclude,
                FindObjectsSortMode.None);

        // remembers the normal spawn position so every nearby option starts from the same place.
        Vector3 normalPosition = furniture.transform.position;

        // keeps the normal spawn point when it is inside the floor and does not overlap furniture.
        if (IsPositionFree(
                furnitureBounds,
                Vector3.zero,
                spawnedFurniture,
                existingFurniture))
            return;

        // works out how many rows of nearby positions need to be checked.
        int searchSteps = Mathf.CeilToInt(MaximumSearchRadius / SearchStep);

        // checks positions from the closest ring outward so the nearest safe option wins.
        for (int step = 1; step <= searchSteps; step++)
        {
            // works out how far this row is from the normal spawn point.
            float radius = step * SearchStep;

            // checks each prepared horizontal direction at the current distance.
            foreach (Vector2 direction in SearchDirections)
            {
                // turns this direction into a sideways and forward movement across the floor.
                Vector3 offset = new Vector3(
                    direction.x * radius,
                    0f,
                    direction.y * radius);

                // skips this position when it leaves the floor or touches other furniture.
                if (!IsPositionFree(
                        furnitureBounds,
                        offset,
                        spawnedFurniture,
                        existingFurniture))
                    continue;

                // moves the new furniture to the first clear nearby position found.
                furniture.transform.position = normalPosition + offset;

                // updates unity immediately so the accepted collision shape is in the right place.
                Physics.SyncTransforms();
                return;
            }
        }

        // reports when no clear nearby point was found within the allowed search area.
        Debug.LogWarning(
            $"No collision-free furniture spawn position was found within " +
            $"{MaximumSearchRadius:F1} metres of the normal spawn point.",
            furniture);
    }

    // checks one possible nearby position for the new furniture.
    // it rejects places outside the floor or inside another furniture object's solid shape.
    private bool IsPositionFree(
        Bounds furnitureBounds,
        Vector3 offset,
        FurnitureSelectable spawnedFurniture,
        FurnitureSelectable[] existingFurniture)
    {
        // copies the furniture measurement so the original value stays unchanged.
        Bounds candidateBounds = furnitureBounds;

        // moves only the copied measurement to the possible position being tested.
        candidateBounds.center += offset;

        // rejects positions where any horizontal edge would extend beyond the visible floor.
        if (!IsInsideFloor(candidateBounds))
            return false;

        // checks the candidate against every active furniture marker in the scene.
        foreach (FurnitureSelectable existing in existingFurniture)
        {
            // ignores missing entries and the new object checking against itself.
            if (existing == null || existing == spawnedFurniture)
                continue;

            // gets the solid invisible shapes that describe this existing furniture object.
            Collider[] colliders =
                existing.GetComponentsInChildren<Collider>(false);

            // compares this possible position with every solid part of that furniture.
            foreach (Collider existingCollider in colliders)
            {
                // ignores missing, inactive or trigger shapes because they do not block furniture placement.
                if (existingCollider == null ||
                    !existingCollider.enabled ||
                    existingCollider.isTrigger)
                    continue;

                // rejects the candidate as soon as one real furniture overlap is found.
                if (OverlapsFurniture(candidateBounds, existingCollider.bounds))
                    return false;
            }
        }

        // accepts the candidate after all floor and furniture checks pass.
        return true;
    }

    // checks whether every edge of a possible furniture position stays inside the visible floor.
    private bool IsInsideFloor(Bounds candidateBounds)
    {
        // allows placement when no floor was connected because its edges cannot be measured.
        if (floor == null)
            return true;

        // measures the visible floor to find the allowed room area.
        Renderer floorRenderer = floor.GetComponentInChildren<Renderer>();

        // also allows placement when the floor has no visible part that defines its edges.
        if (floorRenderer == null)
            return true;

        // compares every side of the possible furniture position with the floor edges.
        Bounds floorBounds = floorRenderer.bounds;
        return candidateBounds.min.x >= floorBounds.min.x &&
               candidateBounds.max.x <= floorBounds.max.x &&
               candidateBounds.min.z >= floorBounds.min.z &&
               candidateBounds.max.z <= floorBounds.max.z;
    }

    // compares the space used by two furniture objects to see whether they would touch.
    // the small clearance prevents newly spawned furniture from appearing almost inside another item.
    private static bool OverlapsFurniture(Bounds candidate, Bounds existing)
    {
        // checks whether both objects use some of the same height in the room.
        bool overlapsVertically =
            candidate.min.y < existing.max.y - 0.01f &&
            candidate.max.y > existing.min.y + 0.01f;

        // checks whether the objects are too close from side to side or front to back.
        bool overlapsHorizontally =
            Mathf.Abs(candidate.center.x - existing.center.x) <
                candidate.extents.x + existing.extents.x + SpawnClearance &&
            Mathf.Abs(candidate.center.z - existing.center.z) <
                candidate.extents.z + existing.extents.z + SpawnClearance;

        // treats the objects as overlapping only when both vertical and horizontal checks agree.
        return overlapsVertically && overlapsHorizontally;
    }

    // measures the full solid shape of a new furniture model using all of its active colliders.
    // it reports failure when the model has no solid shape that can be used for safe placement.
    private static bool TryGetColliderBounds(
        GameObject furniture,
        out Bounds bounds)
    {
        // finds active collision shapes on every visible part of the furniture.
        Collider[] colliders = furniture.GetComponentsInChildren<Collider>(false);

        // remembers whether the first usable collision shape has started the measurement.
        bool hasBounds = false;

        // starts with an empty measurement that the first usable collision shape will replace.
        bounds = default;

        // checks every collision shape that may describe the furniture's physical size.
        foreach (Collider furnitureCollider in colliders)
        {
            // ignores missing, inactive or trigger shapes because they do not block other furniture.
            if (furnitureCollider == null ||
                !furnitureCollider.enabled ||
                furnitureCollider.isTrigger)
                continue;

            // uses the first usable collision shape to begin measuring the furniture.
            if (!hasBounds)
            {
                bounds = furnitureCollider.bounds;
                hasBounds = true;
            }
            else
            {
                // expands the measurement to include every additional solid collision shape.
                bounds.Encapsulate(furnitureCollider.bounds);
            }
        }

        // reports whether any usable furniture size was found.
        return hasBounds;
    }

    // runs after placement to stop a newly created rigidbody from drifting or spinning.
    private static void ResetSpawnMotion(GameObject furniture)
    {
        // uses the rigidbody, which controls how the whole furniture object moves and reacts.
        Rigidbody rigidbody = furniture.GetComponent<Rigidbody>();

        // stops safely when this furniture model has no rigidbody controlling its movement.
        if (rigidbody == null)
            return;

        // stops drifting and spinning while still allowing the user to move the furniture later.
        rigidbody.linearVelocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
    }
}
