using System.Collections;
using UnityEngine;

public class SeatViewController : MonoBehaviour
{
    [Header("Seat Menu")]
    public GameObject actionBackground;

    [Header("References")]
    public Transform xrOrigin;
    public Camera playerCamera;
    public GuestViewManager guestViewManager;

    [Header("Guest UI")]
    public GameObject guestModeMenu;
    public GameObject seatExitMenu;

    [Header("Locomotion")]
    public GameObject locomotionObject;

    [Header("Menu Behaviour")]
    public float hideDelay = 2f;

    private SeatMarker selectedSeat;
    private Coroutine hideCoroutine;
    private bool pointerOverMenu;
    private bool isSeated = false;

    private Vector3 lockedCameraPosition;


    private void Start()
    {
        if (seatExitMenu != null)
            seatExitMenu.SetActive(false);

        if (actionBackground != null)
            actionBackground.SetActive(false);
    }


    // =========================================
    // KEEP PLAYER LOCKED WHILE SITTING
    // =========================================

    private void LateUpdate()
    {
        if (!isSeated)
            return;

        if (xrOrigin == null || playerCamera == null)
            return;

        Vector3 correction =
            lockedCameraPosition -
            playerCamera.transform.position;

        xrOrigin.position += correction;
    }


    // =========================================
    // HOVER CHAIR
    // =========================================

    public void ShowForSeat(SeatMarker seat)
    {
        if (isSeated)
            return;

        if (seat == null)
            return;

        CancelHide();

        selectedSeat = seat;

        Renderer[] renderers =
            seat.GetComponentsInChildren<Renderer>();

        if (renderers.Length > 0)
        {
            Bounds bounds = renderers[0].bounds;

            foreach (Renderer renderer in renderers)
            {
                bounds.Encapsulate(renderer.bounds);
            }

            transform.position =
                new Vector3(
                    bounds.center.x,
                    bounds.max.y + 0.12f,
                    bounds.center.z
                );
        }

        FacePlayer();

        // IMPORTANT:
        // Guest menu disembunyikan supaya tidak block ray
        if (guestModeMenu != null)
            guestModeMenu.SetActive(false);

        if (actionBackground != null)
            actionBackground.SetActive(true);

        Debug.Log(
            "Selected seat: " +
            seat.gameObject.name
        );
    }


    // =========================================
    // LEAVE CHAIR HOVER
    // =========================================

    public void SeatHoverExited(SeatMarker seat)
    {
        if (isSeated)
            return;

        if (seat != selectedSeat)
            return;

        StartHide();
    }


    // =========================================
    // HOVER VIEW FROM SEAT BUTTON
    // =========================================

    public void MenuPointerEnter()
    {
        pointerOverMenu = true;
        CancelHide();
    }

    public void MenuPointerExit()
    {
        pointerOverMenu = false;
        StartHide();
    }


    // =========================================
    // CLICK VIEW FROM SEAT
    // =========================================

    public void ViewSelectedSeat()
    {
        if (selectedSeat == null)
        {
            Debug.LogWarning(
                "No seat selected."
            );
            return;
        }

        if (guestViewManager == null)
        {
            Debug.LogWarning(
                "GuestViewManager is not assigned."
            );
            return;
        }

        // Move camera to selected chair
        guestViewManager.GoToSeat(selectedSeat);

        // Lock current seated eye position
        if (playerCamera != null)
        {
            lockedCameraPosition =
                playerCamera.transform.position;
        }

        isSeated = true;

        // Disable locomotion while seated
        if (locomotionObject != null)
            locomotionObject.SetActive(false);

        // Hide chair popup
        if (actionBackground != null)
            actionBackground.SetActive(false);

        // Guest menu stays hidden
        if (guestModeMenu != null)
            guestModeMenu.SetActive(false);

        // Show ONLY Exit Seat View
        if (seatExitMenu != null)
            seatExitMenu.SetActive(true);

        Debug.Log(
            "Viewing from seat: " +
            selectedSeat.gameObject.name
        );
    }


    // =========================================
    // EXIT SEAT
    // =========================================

    public void ExitSeatView()
    {
        isSeated = false;

        // Allow movement again
        if (locomotionObject != null)
            locomotionObject.SetActive(true);

        // Return to center review point
        if (guestViewManager != null)
            guestViewManager.GoCenter();

        // Hide seat HUD
        if (seatExitMenu != null)
            seatExitMenu.SetActive(false);

        // Restore Guest Mode navigation
        if (guestModeMenu != null)
            guestModeMenu.SetActive(true);

        selectedSeat = null;
        pointerOverMenu = false;

        CancelHide();

        if (actionBackground != null)
            actionBackground.SetActive(false);
    }


    // =========================================
    // FACE VIEW FROM SEAT MENU TO PLAYER
    // =========================================

    private void FacePlayer()
    {
        if (playerCamera == null)
            return;

        Vector3 direction =
            transform.position -
            playerCamera.transform.position;

        if (direction.sqrMagnitude > 0.001f)
        {
            transform.rotation =
                Quaternion.LookRotation(direction);
        }
    }


    // =========================================
    // HIDE POPUP
    // =========================================

    private void StartHide()
    {
        if (isSeated)
            return;

        CancelHide();

        hideCoroutine =
            StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(hideDelay);

        if (!pointerOverMenu && !isSeated)
        {
            if (actionBackground != null)
                actionBackground.SetActive(false);

            // Kalau user batal memilih kursi,
            // tampilkan Guest menu lagi.
            if (guestModeMenu != null)
                guestModeMenu.SetActive(true);
        }

        hideCoroutine = null;
    }


    private void CancelHide()
    {
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }
    }


    // =========================================
    // RESET WHEN EXITING GUEST MODE
    // =========================================

    public void HideImmediately()
    {
        CancelHide();

        isSeated = false;
        selectedSeat = null;
        pointerOverMenu = false;

        if (locomotionObject != null)
            locomotionObject.SetActive(true);

        if (actionBackground != null)
            actionBackground.SetActive(false);

        if (seatExitMenu != null)
            seatExitMenu.SetActive(false);
    }
}