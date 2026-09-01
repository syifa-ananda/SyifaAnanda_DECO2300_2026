using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SeatMarker : MonoBehaviour
{
    [Header("Guest Interaction")]
    public XRSimpleInteractable guestInteractable;

    private SeatViewController seatViewController;

    private void Awake()
    {
        if (guestInteractable == null)
        {
            guestInteractable =
                GetComponent<XRSimpleInteractable>();
        }

        seatViewController =
            FindFirstObjectByType<SeatViewController>();

        if (guestInteractable != null)
        {
            guestInteractable.hoverEntered.AddListener(
                OnGuestHoverEntered
            );

            guestInteractable.hoverExited.AddListener(
                OnGuestHoverExited
            );

            // Guest interaction OFF saat Edit Mode.
            guestInteractable.enabled = false;
        }
    }

    private void OnDestroy()
    {
        if (guestInteractable != null)
        {
            guestInteractable.hoverEntered.RemoveListener(
                OnGuestHoverEntered
            );

            guestInteractable.hoverExited.RemoveListener(
                OnGuestHoverExited
            );
        }
    }

    private void OnGuestHoverEntered(
        HoverEnterEventArgs args
    )
    {
        if (seatViewController == null)
        {
            seatViewController =
                FindFirstObjectByType<SeatViewController>();
        }

        if (seatViewController != null)
        {
            seatViewController.ShowForSeat(this);
        }
    }

    private void OnGuestHoverExited(
        HoverExitEventArgs args
    )
    {
        if (seatViewController != null)
        {
            seatViewController.SeatHoverExited(this);
        }
    }

    public void SetGuestMode(bool enabled)
    {
        if (guestInteractable != null)
        {
            guestInteractable.enabled = enabled;
        }
    }
}