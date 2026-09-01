using UnityEngine;

public class LightingLeverController : MonoBehaviour
{
    [Header("Lever Movement")]
    public float travelDistance = 0.25f;

    [Header("Lights")]
    public Light directionalLight;
    public Light fillLight;

    [Header("Brightness Range")]
    public float minDirectionalIntensity = 0.25f;
    public float maxDirectionalIntensity = 1.3f;

    public float minFillIntensity = 0.05f;
    public float maxFillIntensity = 0.7f;

    private float startY;
    private float minY;
    private float maxY;

    private float lockedX;
    private float lockedZ;

    private void Start()
    {
        Vector3 startPosition = transform.localPosition;

        lockedX = startPosition.x;
        lockedZ = startPosition.z;

        startY = startPosition.y;

        minY = startY - travelDistance;
        maxY = startY + travelDistance;

        UpdateLighting();
    }

    private void LateUpdate()
    {
        Vector3 position = transform.localPosition;

        // Lock X and Z so lever cannot be pulled off the wall
        position.x = lockedX;
        position.z = lockedZ;

        // Only allow vertical movement
        position.y = Mathf.Clamp(
            position.y,
            minY,
            maxY
        );

        transform.localPosition = position;

        UpdateLighting();
    }

    private void UpdateLighting()
    {
        float brightness =
            Mathf.InverseLerp(
                minY,
                maxY,
                transform.localPosition.y
            );

        if (directionalLight != null)
        {
            directionalLight.intensity =
                Mathf.Lerp(
                    minDirectionalIntensity,
                    maxDirectionalIntensity,
                    brightness
                );
        }

        if (fillLight != null)
        {
            fillLight.intensity =
                Mathf.Lerp(
                    minFillIntensity,
                    maxFillIntensity,
                    brightness
                );
        }
    }
}