using UnityEngine;

public class DeleteZone : MonoBehaviour
{
    public Renderer zoneRenderer;

    public Color normalColor =
        new Color(0.6f, 0.05f, 0.05f, 0.35f);

    public Color activeColor =
        new Color(1f, 0.05f, 0.05f, 0.75f);

    private Material zoneMaterial;

    private void Start()
    {
        if (zoneRenderer != null)
        {
            zoneMaterial = zoneRenderer.material;
            zoneMaterial.color = normalColor;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        FurnitureSelectable furniture =
            other.GetComponentInParent<FurnitureSelectable>();

        if (furniture == null)
            return;

        if (zoneMaterial != null)
            zoneMaterial.color = activeColor;

        Destroy(furniture.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (zoneMaterial != null)
            zoneMaterial.color = normalColor;
    }
}