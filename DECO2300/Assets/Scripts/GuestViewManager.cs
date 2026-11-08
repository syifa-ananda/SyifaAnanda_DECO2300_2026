// NOT USED IN FINAL PROTOTYPE
// this script belonged to the earlier guest review and fixed viewpoint system.
// guest review and direct seat views were removed from the final user flow.

// let the old review mode move the player, measure chairs and face the venue correctly.
using UnityEngine;

// moved the whole player setup between old review points or directly to a selected chair.
public class GuestViewManager : MonoBehaviour
{
    [Header("Player")]
    // pointed to the whole player setup moved by the removed review controls.
    public Transform xrOrigin;

    // pointed to the player's camera for the old seated eye position calculation.
    public Camera playerCamera;

    [Header("Guest View Points")]
    // stored the old front review position.
    public Transform frontView;

    // stored the old centre review position.
    public Transform centerView;

    // stored the old back review position.
    public Transform backView;

    // stored the old left review position.
    public Transform leftView;

    // stored the old right review position.
    public Transform rightView;

    [Header("Seat View")]
    // pointed to the venue centre so an old seated view could face into the room.
    public Transform venueCenter;

    // set how high the player's eyes appeared above the bottom of a selected chair.
    public float seatedEyeHeight = 1.2f;


    // NOT USED IN FINAL PROTOTYPE
    // this old button moved the player to the front review point.
    public void GoFront()
    {
        MoveToView(frontView);
    }

    // NOT USED IN FINAL PROTOTYPE
    // this old button moved the player to the centre review point.
    public void GoCenter()
    {
        MoveToView(centerView);
    }

    // NOT USED IN FINAL PROTOTYPE
    // this old button moved the player to the back review point.
    public void GoBack()
    {
        MoveToView(backView);
    }

    // NOT USED IN FINAL PROTOTYPE
    // this old button moved the player to the left review point.
    public void GoLeft()
    {
        MoveToView(leftView);
    }

    // NOT USED IN FINAL PROTOTYPE
    // this old button moved the player to the right review point.
    public void GoRight()
    {
        MoveToView(rightView);
    }

    // NOT USED IN FINAL PROTOTYPE
    // this old method moved and turned the player to one fixed review point.
    private void MoveToView(Transform target)
    {
        // stops when either the player setup or chosen review point is missing.
        if (xrOrigin == null || target == null)
            return;

        // places the complete player setup at the chosen old viewpoint.
        xrOrigin.position = target.position;

        // uses only the target's y rotation so the player stays upright.
        xrOrigin.rotation = Quaternion.Euler(
            0f,
            target.eulerAngles.y,
            0f
        );
    }

    // NOT USED IN FINAL PROTOTYPE
    // this method moved the player's camera to the old direct view from a selected seat.
    public void GoToSeat(SeatMarker seat)
    {
        // reports and stops when the chair, player setup or camera is missing.
        if (seat == null ||
            xrOrigin == null ||
            playerCamera == null)
        {
            Debug.LogWarning("Seat View missing reference.");
            return;
        }

        // finds every visible part of the selected chair to measure its full size.
        Renderer[] renderers =
            seat.GetComponentsInChildren<Renderer>();

        // reports and stops when the old seat has no visible part that can be measured.
        if (renderers.Length == 0)
        {
            Debug.LogWarning("Seat has no renderer.");
            return;
        }

        // starts measuring the chair with its first visible part.
        Bounds bounds = renderers[0].bounds;

        // includes every other visible part in the chair's full size.
        foreach (Renderer renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }

        // places the old seated eye point above the bottom and at the horizontal chair centre.
        Vector3 targetEyePosition = new Vector3(
            bounds.center.x,
            bounds.min.y + seatedEyeHeight,
            bounds.center.z
        );

        // starts with the player's current direction if the venue centre is missing.
        Quaternion targetRotation = xrOrigin.rotation;

        // calculates a direction into the room when a venue centre was assigned.
        if (venueCenter != null)
        {
            // finds the direction from the seated eye point towards the venue centre.
            Vector3 direction =
                venueCenter.position - targetEyePosition;

            // keeps the seated view level by removing vertical tilt.
            direction.y = 0f;

            // only turns the player when there is a clear direction into the venue.
            if (direction.sqrMagnitude > 0.001f)
            {
                targetRotation =
                    Quaternion.LookRotation(
                        direction,
                        Vector3.up
                    );
            }
        }

        // moves the player setup so the camera reaches the calculated old seated view.
        MoveCameraTo(
            targetEyePosition,
            targetRotation
        );

        // records the selected old seat in the unity console for testing.
        Debug.Log(
            "Moved to seat: " + seat.gameObject.name
        );
    }

    // NOT USED IN FINAL PROTOTYPE
    // this old method moved the whole player so the actual camera reached the chosen point.
    private void MoveCameraTo(
        Vector3 targetCameraPosition,
        Quaternion targetRotation
    )
    {
        // checks where the camera sits inside the player setup before moving it.
        Vector3 cameraLocalPosition =
            xrOrigin.InverseTransformPoint(
                playerCamera.transform.position
            );

        // keeps the old seated view level so the player does not tilt up or down.
        Quaternion horizontalRotation =
            Quaternion.Euler(
                0f,
                targetRotation.eulerAngles.y,
                0f
            );

        // turns the complete player setup towards the chosen direction.
        xrOrigin.rotation = horizontalRotation;

        // offsets the rig so the main camera itself lands on the requested eye point.
        xrOrigin.position =
            targetCameraPosition -
            horizontalRotation * cameraLocalPosition;
    }
}
