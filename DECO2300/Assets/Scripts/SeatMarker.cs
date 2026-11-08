// NOT USED IN FINAL PROTOTYPE
// this script marked chairs for the earlier guest review seat view.
// pointing at seats and viewing from them were removed from the final user flow.

// let this old script find the chair controls and seat popup in the scene.
using UnityEngine;

// let this old script know when the controller pointed at or left a chair.
using UnityEngine.XR.Interaction.Toolkit;

// used the simple xr interaction control that made old chair seats respond to pointing.
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// connected one chair to the removed popup that let users view the venue from that seat.
public class SeatMarker : MonoBehaviour
{
    [Header("Guest Interaction")]
    // kept the old interaction control that noticed when the controller pointed at a chair.
    public XRSimpleInteractable guestInteractable;

    // kept the removed controller that displayed and entered seat view.
    private SeatViewController seatViewController;

    // NOT USED IN FINAL PROTOTYPE
    // this prepared the old chair pointing checks and kept guest interaction disabled at first.
    private void Awake()
    {
        // finds the chair's old pointing control when it was not connected manually.
        if (guestInteractable == null)
        {
            guestInteractable =
                GetComponent<XRSimpleInteractable>();
        }

        // searches the scene for the removed seat view controller.
        seatViewController =
            FindFirstObjectByType<SeatViewController>();

        // only starts the old pointing checks when the chair has its guest interaction control.
        if (guestInteractable != null)
        {
            // listens for when the controller begins pointing at this old seat.
            guestInteractable.hoverEntered.AddListener(
                OnGuestHoverEntered
            );

            // listens for when the controller stops pointing at this old seat.
            guestInteractable.hoverExited.AddListener(
                OnGuestHoverExited
            );

            // stops chair pointing from working during the normal furniture editing mode.
            guestInteractable.enabled = false;
        }
    }

    // NOT USED IN FINAL PROTOTYPE
    // this disconnected the old pointing checks when the chair was removed.
    private void OnDestroy()
    {
        // only uses the old chair interaction control when it still exists.
        if (guestInteractable != null)
        {
            // stops listening for both entering and leaving the chair.
            guestInteractable.hoverEntered.RemoveListener(
                OnGuestHoverEntered
            );

            guestInteractable.hoverExited.RemoveListener(
                OnGuestHoverExited
            );
        }
    }

    // NOT USED IN FINAL PROTOTYPE
    // this old method asked the seat view controller to show its chair popup.
    private void OnGuestHoverEntered(
        HoverEnterEventArgs args
    )
    {
        // searches again if the old controller was unavailable during awake.
        if (seatViewController == null)
        {
            seatViewController =
                FindFirstObjectByType<SeatViewController>();
        }

        // only shows the removed popup when its controller exists.
        if (seatViewController != null)
        {
            seatViewController.ShowForSeat(this);
        }
    }

    // NOT USED IN FINAL PROTOTYPE
    // this old method told the seat popup that the controller was no longer pointing at the chair.
    private void OnGuestHoverExited(
        HoverExitEventArgs args
    )
    {
        // only forwards the event when the removed controller is available.
        if (seatViewController != null)
        {
            seatViewController.SeatHoverExited(this);
        }
    }

    // NOT USED IN FINAL PROTOTYPE
    // this method turned chair pointing on or off when the old guest mode changed.
    public void SetGuestMode(bool enabled)
    {
        // only changes the old interaction component when it exists.
        if (guestInteractable != null)
        {
            guestInteractable.enabled = enabled;
        }
    }
}
