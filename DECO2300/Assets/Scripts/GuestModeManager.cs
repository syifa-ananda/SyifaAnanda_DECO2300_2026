using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GuestModeManager : MonoBehaviour
{
    [Header("Edit Mode UI")]
    public GameObject furnitureMenu;
    public GameObject themeMenu;
    public GameObject contextActionBackground;

    [Header("Guest Mode UI")]
    public GameObject guestModeMenu;

    [Header("Seat View")]
    public SeatViewController seatViewController;

    private XRGrabInteractable[] furnitureGrabs;
    private SeatMarker[] seats;

    private int furnitureLayer;

    private void Start()
    {
        furnitureLayer = LayerMask.NameToLayer("Furniture");

        if (guestModeMenu != null)
            guestModeMenu.SetActive(false);

        RefreshSeats();

        // Seat hover OFF saat masih Edit Mode
        foreach (SeatMarker seat in seats)
        {
            if (seat != null)
                seat.SetGuestMode(false);
        }
    }

    public void EnterGuestMode()
    {
        furnitureGrabs =
            FindObjectsByType<XRGrabInteractable>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        // Furniture tidak bisa di-grab
        foreach (XRGrabInteractable grab in furnitureGrabs)
        {
            if (grab != null &&
                grab.gameObject.layer == furnitureLayer)
            {
                grab.enabled = false;
            }
        }

        // Chair sekarang bisa di-hover sebagai seat
        RefreshSeats();

        foreach (SeatMarker seat in seats)
        {
            if (seat != null)
                seat.SetGuestMode(true);
        }

        if (contextActionBackground != null)
            contextActionBackground.SetActive(false);

        if (furnitureMenu != null)
            furnitureMenu.SetActive(false);

        if (themeMenu != null)
            themeMenu.SetActive(false);

        if (guestModeMenu != null)
            guestModeMenu.SetActive(true);
    }

    public void ExitGuestMode()
    {
        // Seat hover OFF lagi
        RefreshSeats();

        foreach (SeatMarker seat in seats)
        {
            if (seat != null)
                seat.SetGuestMode(false);
        }

        // Furniture bisa di-grab lagi
        if (furnitureGrabs != null)
        {
            foreach (XRGrabInteractable grab in furnitureGrabs)
            {
                if (grab != null &&
                    grab.gameObject.layer == furnitureLayer)
                {
                    grab.enabled = true;
                }
            }
        }

        if (seatViewController != null)
            seatViewController.HideImmediately();

        if (guestModeMenu != null)
            guestModeMenu.SetActive(false);

        if (furnitureMenu != null)
            furnitureMenu.SetActive(true);

        if (themeMenu != null)
            themeMenu.SetActive(true);

        if (contextActionBackground != null)
            contextActionBackground.SetActive(false);
    }

    private void RefreshSeats()
    {
        seats =
            FindObjectsByType<SeatMarker>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );
    }
}