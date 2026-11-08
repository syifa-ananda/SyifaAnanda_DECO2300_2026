// USED IN FINAL PROTOTYPE
// this script controls the physical remove zone.
// users delete furniture by moving it into this zone instead of using a delete button.

// lets this script detect objects, change the zone colour and remove furniture.
using UnityEngine;

// checks when furniture enters the remove area and then removes it from the venue.
public class DeleteZone : MonoBehaviour
{
    // keeps the visible part of the remove zone so it can change colour when used.
    public Renderer zoneRenderer;

    // keeps the transparent red colour shown while the zone is waiting for furniture.
    public Color normalColor =
        new Color(0.6f, 0.05f, 0.05f, 0.35f);

    // keeps the stronger red colour shown when furniture reaches the zone.
    public Color activeColor =
        new Color(1f, 0.05f, 0.05f, 0.75f);

    // keeps the zone's own material so its colour can change during the vr experience.
    private Material zoneMaterial;

    // runs when the remove zone starts in the scene.
    // it gets the visible material and applies the normal waiting colour.
    private void Start()
    {
        // only prepares the colour when the visible part of the zone was connected.
        if (zoneRenderer != null)
        {
            // gives this remove zone its own material so other objects do not change colour too.
            zoneMaterial = zoneRenderer.material;

            // starts the remove zone with its less intense colour.
            zoneMaterial.color = normalColor;
        }
    }

    // runs when an object's invisible collision shape enters the remove zone.
    // it checks for the furniture marker before deleting anything.
    private void OnTriggerEnter(Collider other)
    {
        // checks the entering object and its main object to make sure it is furniture.
        FurnitureSelectable furniture =
            other.GetComponentInParent<FurnitureSelectable>();

        // stops here when the object touching the zone is not removable furniture.
        if (furniture == null)
            return;

        // shows that the zone has accepted a valid furniture object.
        if (zoneMaterial != null)
            zoneMaterial.color = activeColor;

        // removes the whole furniture object instead of only the part that touched the zone.
        Destroy(furniture.gameObject);
    }

    // runs when an object's invisible collision shape leaves the remove zone.
    // it restores the normal colour ready for the next furniture object.
    private void OnTriggerExit(Collider other)
    {
        // only changes the colour when the zone's own material is ready.
        if (zoneMaterial != null)
            zoneMaterial.color = normalColor;
    }
}
