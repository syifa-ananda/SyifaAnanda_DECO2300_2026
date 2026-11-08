// NOT USED IN FINAL PROTOTYPE
// this script belonged to the earlier guest review mode.
// guest review and its seat viewing flow are no longer available in the final prototype.
// the code remains in the project but is not part of the current demo.

// let the old mode show menus, identify furniture and find seats in the scene.
using UnityEngine;

// let the old mode turn furniture grabbing off while guests reviewed the venue.
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// switched the earlier prototype between furniture editing and guest review modes.
public class GuestModeManager : MonoBehaviour
{
    [Header("Edit Mode UI")]
    // pointed to the furniture menu hidden during the removed guest review mode.
    public GameObject furnitureMenu;

    // pointed to the theme menu hidden during the removed guest review mode.
    public GameObject themeMenu;

    // pointed to the old furniture context action background.
    public GameObject contextActionBackground;

    [Header("Guest Mode UI")]
    // pointed to the navigation menu used only by the removed guest review flow.
    public GameObject guestModeMenu;

    [Header("Seat View")]
    // connected the old guest mode to its removed seat view controller.
    public SeatViewController seatViewController;

    // remembered every furniture grab control found when the old guest mode started.
    private XRGrabInteractable[] furnitureGrabs;

    // stored every chair marker available to the old seat review system.
    private SeatMarker[] seats;

    // remembered the furniture group used to recognise which grab controls belonged to furniture.
    private int furnitureLayer;

    // NOT USED IN FINAL PROTOTYPE
    // this prepared the removed guest review objects and kept chair pointing off in edit mode.
    private void Start()
    {
        // finds the furniture group number used by the old furniture checks.
        furnitureLayer = LayerMask.NameToLayer("Furniture");

        // starts the removed guest navigation menu hidden.
        if (guestModeMenu != null)
            guestModeMenu.SetActive(false);

        // finds seat markers that existed for the old review system.
        RefreshSeats();

        // checks every old seat so edit mode does not respond when the controller points at chairs.
        foreach (SeatMarker seat in seats)
        {
            // safely disables guest interaction on each available seat.
            if (seat != null)
                seat.SetGuestMode(false);
        }
    }

    // NOT USED IN FINAL PROTOTYPE
    // this method entered the removed guest review mode and disabled furniture editing.
    public void EnterGuestMode()
    {
        // finds all grab controls, including hidden ones, for the old mode switch.
        furnitureGrabs =
            FindObjectsByType<XRGrabInteractable>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        // checks every grab control and turns off the ones that belong to furniture.
        foreach (XRGrabInteractable grab in furnitureGrabs)
        {
            // leaves missing controls and objects outside the furniture group unchanged.
            if (grab != null &&
                grab.gameObject.layer == furnitureLayer)
            {
                grab.enabled = false;
            }
        }

        // finds the seats again in case the user added more chairs after the experience started.
        RefreshSeats();

        // lets the controller point at each available seat during old guest review.
        foreach (SeatMarker seat in seats)
        {
            if (seat != null)
                seat.SetGuestMode(true);
        }

        // closes the old furniture popup so it cannot remain over guest review.
        if (contextActionBackground != null)
            contextActionBackground.SetActive(false);

        // hides editing menus during the removed guest review state.
        if (furnitureMenu != null)
            furnitureMenu.SetActive(false);

        if (themeMenu != null)
            themeMenu.SetActive(false);

        // shows the old guest navigation menu after editing controls are hidden.
        if (guestModeMenu != null)
            guestModeMenu.SetActive(true);
    }

    // NOT USED IN FINAL PROTOTYPE
    // this method left the removed guest review mode and restored furniture editing.
    public void ExitGuestMode()
    {
        // refreshes the old seat list before disabling its guest interaction again.
        RefreshSeats();

        // checks every old seat and stops chair pointing after leaving guest review.
        foreach (SeatMarker seat in seats)
        {
            if (seat != null)
                seat.SetGuestMode(false);
        }

        // only restores grabs when the old entry method previously collected them.
        if (furnitureGrabs != null)
        {
            // goes through the stored grab components from the removed mode.
            foreach (XRGrabInteractable grab in furnitureGrabs)
            {
                // restores only working grab controls that belong to furniture.
                if (grab != null &&
                    grab.gameObject.layer == furnitureLayer)
                {
                    grab.enabled = true;
                }
            }
        }

        // immediately clears any old active seat view before returning to editing.
        if (seatViewController != null)
            seatViewController.HideImmediately();

        // hides the removed guest navigation menu.
        if (guestModeMenu != null)
            guestModeMenu.SetActive(false);

        // restores the old edit menus that were hidden during guest review.
        if (furnitureMenu != null)
            furnitureMenu.SetActive(true);

        if (themeMenu != null)
            themeMenu.SetActive(true);

        // keeps the removed furniture popup closed until the controller points at furniture again.
        if (contextActionBackground != null)
            contextActionBackground.SetActive(false);
    }

    // NOT USED IN FINAL PROTOTYPE
    // this old method found every seat used by the removed guest review flow.
    private void RefreshSeats()
    {
        // includes hidden chair seats because the old mode needed to manage all of them.
        seats =
            FindObjectsByType<SeatMarker>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );
    }
}
