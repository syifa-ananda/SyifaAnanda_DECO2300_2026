// NOT USED IN FINAL PROTOTYPE
// this script controlled the earlier view from seat popup and seated camera state.
// guest review, seat view and its exit menu are no longer part of the final prototype.

// let the old seat popup wait briefly before hiding from the user.
using System.Collections;

// let the old feature move the player, measure chairs and show or hide its menus.
using UnityEngine;

// managed chair pointing, the fixed seated camera and exit controls for the removed seat view.
public class SeatViewController : MonoBehaviour
{
    [Header("Seat Menu")]
    // pointed to the old view from seat popup background.
    public GameObject actionBackground;

    [Header("References")]
    // pointed to the whole player setup moved during old seat viewing.
    public Transform xrOrigin;

    // pointed to the player's camera that stayed fixed during the old seated view.
    public Camera playerCamera;

    // connected this popup to the removed guest viewpoint movement system.
    public GuestViewManager guestViewManager;

    [Header("Guest UI")]
    // pointed to the removed guest navigation menu hidden while examining a chair.
    public GameObject guestModeMenu;

    // pointed to the old exit seat view menu shown after sitting.
    public GameObject seatExitMenu;

    [Header("Locomotion")]
    // pointed to the movement object disabled while the old seated view was active.
    public GameObject locomotionObject;

    [Header("Menu Behaviour")]
    // set how long the old chair popup waited after the controller stopped pointing at it.
    public float hideDelay = 2f;

    // remembered the chair currently targeted by the removed seat popup.
    private SeatMarker selectedSeat;

    // kept the active hide timer so old pointer actions could cancel it.
    private Coroutine hideCoroutine;

    // remembered whether the pointer was over the removed seat popup.
    private bool pointerOverMenu;

    // remembered whether the user was currently locked in the old seated view.
    private bool isSeated = false;

    // remembered the camera position that the old seated view needed to keep.
    private Vector3 lockedCameraPosition;

    // NOT USED IN FINAL PROTOTYPE
    // this started both removed seat menus hidden.
    private void Start()
    {
        // hides the old exit menu until an old seat view was entered.
        if (seatExitMenu != null)
            seatExitMenu.SetActive(false);

        // hides the old chair popup until a seat was hovered.
        if (actionBackground != null)
            actionBackground.SetActive(false);
    }

    // NOT USED IN FINAL PROTOTYPE
    // this ran after each frame and stopped the camera drifting during the old seated view.
    private void LateUpdate()
    {
        // stops when the old seated state is not active.
        if (!isSeated)
            return;

        // stops when the old player setup or camera is unavailable.
        if (xrOrigin == null || playerCamera == null)
            return;

        // works out how far the camera moved away from its fixed seated position.
        Vector3 correction =
            lockedCameraPosition -
            playerCamera.transform.position;

        // moves the player setup in the opposite direction to keep the camera in place.
        xrOrigin.position += correction;
    }

    // NOT USED IN FINAL PROTOTYPE
    // this showed the old view from seat popup above a chair the controller pointed at.
    public void ShowForSeat(SeatMarker seat)
    {
        // ignores another chair while the user is already inside an old seated view.
        if (isSeated)
            return;

        // stops when no valid chair marker was supplied.
        if (seat == null)
            return;

        // prevents an older hide timer from closing the newly shown popup.
        CancelHide();

        // remembers which old seat the popup action would use.
        selectedSeat = seat;

        // finds every visible part of the chair to measure its complete size.
        Renderer[] renderers =
            seat.GetComponentsInChildren<Renderer>();

        // only chooses a popup position when the chair has at least one visible part.
        if (renderers.Length > 0)
        {
            // starts measuring the chair with its first visible part.
            Bounds bounds = renderers[0].bounds;

            // includes every other visible part in the chair's full size.
            foreach (Renderer renderer in renderers)
            {
                bounds.Encapsulate(renderer.bounds);
            }

            // places the removed popup slightly above the centre of the chair.
            transform.position =
                new Vector3(
                    bounds.center.x,
                    bounds.max.y + 0.12f,
                    bounds.center.z
                );
        }

        // turns the old floating popup towards the player's camera.
        FacePlayer();

        // hides the removed guest navigation menu so it does not block rays to the seat popup.
        if (guestModeMenu != null)
            guestModeMenu.SetActive(false);

        // displays the old view from seat action above the selected chair.
        if (actionBackground != null)
            actionBackground.SetActive(true);

        // records the old selected seat in the unity console for testing.
        Debug.Log(
            "Selected seat: " +
            seat.gameObject.name
        );
    }

    // NOT USED IN FINAL PROTOTYPE
    // this began hiding the old chair popup when the controller stopped pointing at its seat.
    public void SeatHoverExited(SeatMarker seat)
    {
        // keeps the popup state unchanged while an old seated view is active.
        if (isSeated)
            return;

        // ignores pointing changes from chairs other than the selected old seat.
        if (seat != selectedSeat)
            return;

        // starts the old delayed hide routine.
        StartHide();
    }

    // NOT USED IN FINAL PROTOTYPE
    // this old method kept the seat popup visible while the controller pointed at it.
    public void MenuPointerEnter()
    {
        pointerOverMenu = true;

        // cancels the hide delay because the controller reached the old popup.
        CancelHide();
    }

    // NOT USED IN FINAL PROTOTYPE
    // this old method restarted hiding when the controller stopped pointing at the seat popup.
    public void MenuPointerExit()
    {
        pointerOverMenu = false;
        StartHide();
    }

    // NOT USED IN FINAL PROTOTYPE
    // this old button moved the player into the direct view from the selected chair.
    public void ViewSelectedSeat()
    {
        // reports and stops when no old chair selection is available.
        if (selectedSeat == null)
        {
            Debug.LogWarning(
                "No seat selected."
            );
            return;
        }

        // reports and stops when the removed viewpoint manager was not assigned.
        if (guestViewManager == null)
        {
            Debug.LogWarning(
                "GuestViewManager is not assigned."
            );
            return;
        }

        // asks the old guest manager to move the player's view to the selected chair.
        guestViewManager.GoToSeat(selectedSeat);

        // remembers the camera position reached by the old seated movement.
        if (playerCamera != null)
        {
            lockedCameraPosition =
                playerCamera.transform.position;
        }

        // remembers that the user is seated so the camera stays fixed each frame.
        isSeated = true;

        // disables player movement while the removed seated view is active.
        if (locomotionObject != null)
            locomotionObject.SetActive(false);

        // hides the old chair popup after its action is selected.
        if (actionBackground != null)
            actionBackground.SetActive(false);

        // keeps the removed guest navigation hidden while seated.
        if (guestModeMenu != null)
            guestModeMenu.SetActive(false);

        // shows the old exit action as the only available seat view control.
        if (seatExitMenu != null)
            seatExitMenu.SetActive(true);

        // records the old seated chair in the unity console for testing.
        Debug.Log(
            "Viewing from seat: " +
            selectedSeat.gameObject.name
        );
    }

    // NOT USED IN FINAL PROTOTYPE
    // this old button left seat view and returned the user to guest navigation.
    public void ExitSeatView()
    {
        // stops the old camera locking behaviour.
        isSeated = false;

        // restores player movement after leaving the old seated view.
        if (locomotionObject != null)
            locomotionObject.SetActive(true);

        // moves the old guest review player setup back to its centre viewpoint.
        if (guestViewManager != null)
            guestViewManager.GoCenter();

        // hides the removed exit seat menu.
        if (seatExitMenu != null)
            seatExitMenu.SetActive(false);

        // restores the removed guest navigation menu after exiting the seat.
        if (guestModeMenu != null)
            guestModeMenu.SetActive(true);

        // clears the old seat selection and pointer state.
        selectedSeat = null;
        pointerOverMenu = false;

        // stops any popup hide timer left from the old controller pointing flow.
        CancelHide();

        // makes sure the removed chair popup is closed.
        if (actionBackground != null)
            actionBackground.SetActive(false);
    }

    // NOT USED IN FINAL PROTOTYPE
    // this turned the old chair popup towards the player's camera.
    private void FacePlayer()
    {
        // stops when no player camera was assigned for the old popup.
        if (playerCamera == null)
            return;

        // works out which way the floating popup must face to point towards the camera.
        Vector3 direction =
            transform.position -
            playerCamera.transform.position;

        // only turns the popup when there is a clear direction towards the player.
        if (direction.sqrMagnitude > 0.001f)
        {
            transform.rotation =
                Quaternion.LookRotation(direction);
        }
    }

    // NOT USED IN FINAL PROTOTYPE
    // this restarted the old short wait before hiding the popup.
    private void StartHide()
    {
        // keeps the old popup state unchanged while seated.
        if (isSeated)
            return;

        // prevents more than one popup hide timer from running at the same time.
        CancelHide();

        // begins the wait that gave the pointer time to reach the old popup.
        hideCoroutine =
            StartCoroutine(HideAfterDelay());
    }

    // NOT USED IN FINAL PROTOTYPE
    // this old method waited before hiding the removed chair popup.
    private IEnumerator HideAfterDelay()
    {
        // waits for the configured old popup delay.
        yield return new WaitForSeconds(hideDelay);

        // only closes the old popup when the pointer is away and the user is not seated.
        if (!pointerOverMenu && !isSeated)
        {
            // hides the old chair action background.
            if (actionBackground != null)
                actionBackground.SetActive(false);

            // restores the old guest menu when the user leaves the chair without selecting it.
            if (guestModeMenu != null)
                guestModeMenu.SetActive(true);
        }

        // remembers that the old popup has finished waiting to hide.
        hideCoroutine = null;
    }

    // NOT USED IN FINAL PROTOTYPE
    // this old method stopped a popup hide timer when it was no longer needed.
    private void CancelHide()
    {
        // only stops the old popup hide timer when one is currently running.
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }
    }

    // NOT USED IN FINAL PROTOTYPE
    // this immediately reset old seat view state when guest review ended.
    public void HideImmediately()
    {
        // stops any delayed popup hide before resetting the old mode.
        CancelHide();

        // clears the old seated, selected seat and pointer states.
        isSeated = false;
        selectedSeat = null;
        pointerOverMenu = false;

        // restores locomotion in case the old seat view had disabled it.
        if (locomotionObject != null)
            locomotionObject.SetActive(true);

        // closes the old chair popup and exit menu.
        if (actionBackground != null)
            actionBackground.SetActive(false);

        if (seatExitMenu != null)
            seatExitMenu.SetActive(false);
    }
}
