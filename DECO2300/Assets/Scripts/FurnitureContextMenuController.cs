// NOT USED IN FINAL PROTOTYPE
// this script controlled the old furniture context menu.
// that menu provided duplicate and button based delete actions, which were removed from the final user flow.
// the final prototype adds furniture through the event planner and deletes it through the physical remove zone.

// lets the old menu wait briefly before hiding from the user.
using System.Collections;

// lets the old menu find the player, measure furniture and show or hide objects.
using UnityEngine;

// controls where the removed furniture popup appeared and what its old buttons did.
public class FurnitureContextMenuController : MonoBehaviour
{
    // keeps the old popup background that contained the furniture actions.
    public GameObject actionBackground;

    // keeps the player's camera so the old popup could face and stay near the user.
    public Camera playerCamera;

    // sets how long the old popup waited after the controller stopped pointing at it.
    public float hideDelay = 0.6f;

    // remembers which furniture object the removed menu was acting on.
    private GameObject selectedFurniture;

    // remembers the current hide timer so the menu can cancel it when needed.
    private Coroutine hideCoroutine;

    // remembers whether the controller is still pointing at the old popup.
    private bool pointerOverMenu;

    // NOT USED IN FINAL PROTOTYPE
    // this method ran when the controller pointed at furniture to show the removed menu.
    // it measured the object and placed the popup between the object and the player.
    public void ShowMenu(GameObject furniture)
    {
        // prevents an older hide timer from closing the menu while it is being shown again.
        CancelHide();

        // remembers the furniture that the old duplicate or delete action would affect.
        selectedFurniture = furniture;

        // finds every visible part so the menu can appear above the whole furniture object.
        Renderer[] renderers = furniture.GetComponentsInChildren<Renderer>();

        // only measures the furniture when it has at least one visible part.
        if (renderers.Length > 0)
        {
            // starts measuring the furniture with its first visible part.
            Bounds bounds = renderers[0].bounds;

            // includes every other visible part when measuring the full furniture size.
            foreach (Renderer renderer in renderers)
            {
                bounds.Encapsulate(renderer.bounds);
            }

            // finds a point just above the highest visible part of the furniture.
            float objectMenuY = bounds.max.y + 0.05f;

            // also finds a height that stays comfortable relative to the player's camera.
            float comfortableY = playerCamera.transform.position.y - 0.2f;

            // uses the lower height so the menu does not rise too far above the user.
            float finalY = Mathf.Min(objectMenuY, comfortableY);

            // finds the horizontal direction from the furniture towards the player.
            Vector3 towardPlayer =
                playerCamera.transform.position - bounds.center;

            // removes vertical movement because the height is handled separately.
            towardPlayer.y = 0f;

            // turns this into a clear direction only when the player is far enough away.
            if (towardPlayer.sqrMagnitude > 0.001f)
            {
                towardPlayer.Normalize();
            }

            // offsets the old popup slightly towards the player from the object centre.
            Vector3 menuPosition =
                bounds.center + towardPlayer * 0.35f;

            // applies the previously chosen comfortable vertical position.
            menuPosition.y = finalY;

            // moves the old menu to the chosen place beside the furniture.
            transform.position = menuPosition;
        }

        // works out which way the old floating menu must turn to face the player.
        Vector3 direction =
            transform.position - playerCamera.transform.position;

        // turns the popup towards the player's view.
        transform.rotation = Quaternion.LookRotation(direction);

        // makes the removed action background visible.
        actionBackground.SetActive(true);
    }

    // NOT USED IN FINAL PROTOTYPE
    // this method began hiding the old menu when the controller stopped pointing at the furniture.
    public void FurnitureHoverExited(GameObject furniture)
    {
        // ignores furniture that was not being used by the old menu.
        if (furniture != selectedFurniture)
            return;

        // starts the short delay before hiding the old popup.
        StartHide();
    }

    // NOT USED IN FINAL PROTOTYPE
    // this old method kept the menu visible while the controller pointed at it.
    public void MenuPointerEnter()
    {
        pointerOverMenu = true;

        // cancels any pending hide because the user reached the old menu.
        CancelHide();
    }

    // NOT USED IN FINAL PROTOTYPE
    // this old method restarted the hide timer when the controller stopped pointing at the menu.
    public void MenuPointerExit()
    {
        pointerOverMenu = false;

        // waits briefly before closing the removed popup.
        StartHide();
    }

    // NOT USED IN FINAL PROTOTYPE
    // this method restarts the short wait before the old menu disappears.
    private void StartHide()
    {
        // makes sure only one hide routine is running at a time.
        CancelHide();
        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    // NOT USED IN FINAL PROTOTYPE
    // this method waited for a short time before hiding the old context menu.
    private IEnumerator HideAfterDelay()
    {
        // gives the pointer time to move from the furniture onto the menu.
        yield return new WaitForSeconds(hideDelay);

        // only hides the old menu when the controller did not point at it during the wait.
        if (!pointerOverMenu)
        {
            actionBackground.SetActive(false);

            // forgets the furniture because no old menu action is currently available.
            selectedFurniture = null;
        }

        // remembers that the menu has finished waiting to hide.
        hideCoroutine = null;
    }

    // NOT USED IN FINAL PROTOTYPE
    // this method stops the current hide timer before another one begins.
    private void CancelHide()
    {
        // only stops the old hide timer when one is currently active.
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }
    }

    // NOT USED IN FINAL PROTOTYPE
    // this function belonged to the removed duplicate button.
    // users now add another item through the event planner menu.
    public void DuplicateSelected()
    {
        // stops when the removed menu does not have a selected furniture object.
        if (selectedFurniture == null)
            return;

        // finds all visible parts so the copy can appear beside the whole object.
        Renderer[] renderers =
            selectedFurniture.GetComponentsInChildren<Renderer>();

        // starts with a basic gap when the furniture has no visible parts to measure.
        float offset = 1f;

        // chooses a better gap when the furniture has visible parts that can be measured.
        if (renderers.Length > 0)
        {
            // starts measuring the furniture with its first visible part.
            Bounds bounds = renderers[0].bounds;

            // includes every remaining visible part in the full furniture size.
            foreach (Renderer renderer in renderers)
            {
                bounds.Encapsulate(renderer.bounds);
            }

            // adds a small gap to the widest horizontal size of the object.
            offset =
                Mathf.Max(bounds.size.x, bounds.size.z) + 0.3f;
        }

        // creates the old copy beside the selected furniture facing the same direction.
        GameObject copy = Instantiate(
            selectedFurniture,
            selectedFurniture.transform.position +
            selectedFurniture.transform.right * offset,
            selectedFurniture.transform.rotation
        );

        // gives the duplicated object a readable copy name.
        copy.name =
            selectedFurniture.name.Replace("(Clone)", "") + " Copy";

    }

    // NOT USED IN FINAL PROTOTYPE
    // this function belonged to the removed button based delete option.
    // the final prototype uses the physical remove zone instead.
    public void DeleteSelected()
    {
        // stops when the old menu does not have a selected furniture object.
        if (selectedFurniture == null)
            return;

        // removes the selected object using the old direct button action.
        Destroy(selectedFurniture);

        // clears the old selection and closes its action background.
        selectedFurniture = null;
        actionBackground.SetActive(false);
    }
}
