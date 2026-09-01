using UnityEngine;

public class GuestViewManager : MonoBehaviour
{
    [Header("Player")]
    public Transform xrOrigin;
    public Camera playerCamera;

    [Header("Guest View Points")]
    public Transform frontView;
    public Transform centerView;
    public Transform backView;
    public Transform leftView;
    public Transform rightView;

    [Header("Seat View")]
    public Transform venueCenter;
    public float seatedEyeHeight = 1.2f;


    // =========================
    // FIXED VIEWPOINTS
    // =========================

    public void GoFront()
    {
        MoveToView(frontView);
    }

    public void GoCenter()
    {
        MoveToView(centerView);
    }

    public void GoBack()
    {
        MoveToView(backView);
    }

    public void GoLeft()
    {
        MoveToView(leftView);
    }

    public void GoRight()
    {
        MoveToView(rightView);
    }


    private void MoveToView(Transform target)
    {
        if (xrOrigin == null || target == null)
            return;

        // Untuk fixed viewpoint,
        // pindahkan XR Origin seperti sebelumnya.
        xrOrigin.position = target.position;

        xrOrigin.rotation = Quaternion.Euler(
            0f,
            target.eulerAngles.y,
            0f
        );
    }


    // =========================
    // DIRECT SEAT VIEW
    // =========================

    public void GoToSeat(SeatMarker seat)
    {
        if (seat == null ||
            xrOrigin == null ||
            playerCamera == null)
        {
            Debug.LogWarning("Seat View missing reference.");
            return;
        }

        Renderer[] renderers =
            seat.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
            Debug.LogWarning("Seat has no renderer.");
            return;
        }

        Bounds bounds = renderers[0].bounds;

        foreach (Renderer renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }

        // Posisi MATA ketika duduk
        Vector3 targetEyePosition = new Vector3(
            bounds.center.x,
            bounds.min.y + seatedEyeHeight,
            bounds.center.z
        );

        // Arahkan ke center venue
        Quaternion targetRotation = xrOrigin.rotation;

        if (venueCenter != null)
        {
            Vector3 direction =
                venueCenter.position - targetEyePosition;

            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                targetRotation =
                    Quaternion.LookRotation(
                        direction,
                        Vector3.up
                    );
            }
        }

        MoveCameraTo(
            targetEyePosition,
            targetRotation
        );

        Debug.Log(
            "Moved to seat: " + seat.gameObject.name
        );
    }


    // =========================
    // XR CAMERA POSITIONING
    // =========================

    private void MoveCameraTo(
        Vector3 targetCameraPosition,
        Quaternion targetRotation
    )
    {
        // Cari posisi Main Camera relatif terhadap XR Origin
        Vector3 cameraLocalPosition =
            xrOrigin.InverseTransformPoint(
                playerCamera.transform.position
            );

        Quaternion horizontalRotation =
            Quaternion.Euler(
                0f,
                targetRotation.eulerAngles.y,
                0f
            );

        // Putar rig
        xrOrigin.rotation = horizontalRotation;

        // Geser rig supaya posisi MAIN CAMERA,
        // bukan origin-nya, tepat di target eye position
        xrOrigin.position =
            targetCameraPosition -
            horizontalRotation * cameraLocalPosition;
    }
}