using System.Collections;
using UnityEngine;

public class FurnitureContextMenuController : MonoBehaviour
{
    public GameObject actionBackground;
    public Camera playerCamera;
    public float hideDelay = 0.6f;

    private GameObject selectedFurniture;
    private Coroutine hideCoroutine;
    private bool pointerOverMenu;

    public void ShowMenu(GameObject furniture)
    {
        CancelHide();

        selectedFurniture = furniture;

        Renderer[] renderers = furniture.GetComponentsInChildren<Renderer>();

        if (renderers.Length > 0)
        {
            Bounds bounds = renderers[0].bounds;

            foreach (Renderer renderer in renderers)
            {
                bounds.Encapsulate(renderer.bounds);
            }

            float objectMenuY = bounds.max.y + 0.05f;

            float comfortableY = playerCamera.transform.position.y - 0.2f;

            float finalY = Mathf.Min(objectMenuY, comfortableY);

            Vector3 towardPlayer =
                playerCamera.transform.position - bounds.center;

            towardPlayer.y = 0f;

            if (towardPlayer.sqrMagnitude > 0.001f)
            {
                towardPlayer.Normalize();
            }

            Vector3 menuPosition =
                bounds.center + towardPlayer * 0.35f;

            menuPosition.y = finalY;

            transform.position = menuPosition;
        }

        Vector3 direction =
            transform.position - playerCamera.transform.position;

        transform.rotation = Quaternion.LookRotation(direction);

        actionBackground.SetActive(true);
    }

    public void FurnitureHoverExited(GameObject furniture)
    {
        if (furniture != selectedFurniture)
            return;

        StartHide();
    }

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

    private void StartHide()
    {
        CancelHide();
        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(hideDelay);

        if (!pointerOverMenu)
        {
            actionBackground.SetActive(false);
            selectedFurniture = null;
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

    public void DuplicateSelected()
    {
        if (selectedFurniture == null)
            return;

        Renderer[] renderers =
            selectedFurniture.GetComponentsInChildren<Renderer>();

        float offset = 1f;

        if (renderers.Length > 0)
        {
            Bounds bounds = renderers[0].bounds;

            foreach (Renderer renderer in renderers)
            {
                bounds.Encapsulate(renderer.bounds);
            }

            offset =
                Mathf.Max(bounds.size.x, bounds.size.z) + 0.3f;
        }

        GameObject copy = Instantiate(
            selectedFurniture,
            selectedFurniture.transform.position +
            selectedFurniture.transform.right * offset,
            selectedFurniture.transform.rotation
        );

        copy.name =
            selectedFurniture.name.Replace("(Clone)", "") + " Copy";

    }

    public void DeleteSelected()
    {
        if (selectedFurniture == null)
            return;

        Destroy(selectedFurniture);

        selectedFurniture = null;
        actionBackground.SetActive(false);
    }
}