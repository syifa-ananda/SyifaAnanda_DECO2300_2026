// USED IN FINAL PROTOTYPE
// this script controls the physical lighting lever on the venue wall.
// the lever moves up and down so the user can make the venue dim, normal or bright.

// lets this script move the lever, measure its position and change the venue lights.
using UnityEngine;

// lets this script know when the user grabs or releases the lever.
using UnityEngine.XR.Interaction.Toolkit;

// uses the xr grab interactable, which lets the user hold the lever in vr.
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// lets this script follow the real or simulated controller that is holding the lever.
using UnityEngine.XR.Interaction.Toolkit.Interactors;

// keeps the lever inside its track and changes the venue brightness to match its height.
public class LightingLeverController : MonoBehaviour
{
    [Header("Lever Movement")]
    // sets how far the lever handle can move above and below its starting height.
    public float travelDistance = 0.25f;

    [Header("Lights")]
    // keeps the main room light that becomes darker or brighter with the lever.
    public Light directionalLight;

    // keeps the softer room light that also becomes darker or brighter with the lever.
    public Light fillLight;

    [Header("Brightness Range")]
    // sets how dark the main room light becomes when the lever is at the bottom.
    public float minDirectionalIntensity = 0.2f;

    // sets the main room light used when the lever is in the normal middle position.
    public float normalDirectionalIntensity = 1f;

    // sets how bright the main room light becomes when the lever is at the top.
    public float maxDirectionalIntensity = 2.4f;

    // sets how dark the softer room light becomes when the lever is at the bottom.
    public float minFillIntensity = 0.1f;

    // sets the softer room light used when the lever is in the normal middle position.
    public float normalFillIntensity = 0.5f;

    // sets how bright the softer room light becomes when the lever is at the top.
    public float maxFillIntensity = 1.2f;

    // remembers the lever's starting height, which represents the normal lighting level.
    private float startY;

    // remembers the lowest point the lever can move to, which gives the darkest lighting.
    private float minY;

    // remembers the highest point the lever can move to, which gives the brightest lighting.
    private float maxY;

    // remembers the lever's starting side position so it cannot be pulled left or right.
    private float lockedX;

    // remembers how close the lever starts to the wall so it stays inside its track.
    private float lockedZ;

    // keeps the grab controls that tell this script when the user is holding the lever.
    private XRGrabInteractable grabInteractable;

    // remembers which real or simulated controller is currently holding the lever.
    private IXRSelectInteractor activeInteractor;

    // remembers where the user first grabbed the lever so it follows the controller smoothly.
    // keeping the complete grab point means pointing the controller up or down can move
    // the lever even when the user's hand stays in almost the same place.
    private Vector3 grabPointInInteractorSpace;

    // remembers whether the script has found a reliable controller grab point yet.
    // quest may provide this point one frame after the user first grabs the lever.
    private bool hasGrabReference;

    // remembers the last valid lever height while the user is still holding it.
    // this separate value stops the xr grab system from losing the chosen level on release.
    private float heldY;

    // remembers the dim, normal or bright position that must stay after release.
    private float restingY;

    // keeps the rigidbody, which controls physics, from pulling against the lever track.
    private Rigidbody leverBody;

    // runs when the lever object is first loaded.
    // it finds and remembers the grab controls attached to the same handle.
    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        // the lever is positioned directly by this script, so physics must not push it down.
        leverBody = GetComponent<Rigidbody>();
        if (leverBody != null)
        {
            leverBody.useGravity = false;
            leverBody.isKinematic = true;
        }
    }

    // runs whenever the lever component becomes active.
    // it starts listening for the user grabbing and releasing the lever.
    private void OnEnable()
    {
        // tries again to find the grab controls if they were not available when loading.
        if (grabInteractable == null)
            grabInteractable = GetComponent<XRGrabInteractable>();

        // stops safely when the lever has no working grab controls.
        if (grabInteractable == null)
            return;

        // this script controls movement while xr grab only checks whether the lever is held.
        // turning off automatic following stops the grab system from pulling the handle
        // away from the wall before this script keeps it inside the track.
        grabInteractable.trackPosition = false;
        grabInteractable.trackRotation = false;
        grabInteractable.throwOnDetach = false;

        // listens for the user beginning and ending a grab on the lever handle.
        grabInteractable.selectEntered.AddListener(OnSelectEntered);
        grabInteractable.selectExited.AddListener(OnSelectExited);
    }

    // runs whenever the lever component becomes inactive.
    // it removes listeners and forgets any controller that was holding the lever.
    private void OnDisable()
    {
        // only stops listening when the grab controls still exist.
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnSelectEntered);
            grabInteractable.selectExited.RemoveListener(OnSelectExited);
        }

        // prevents an inactive lever from continuing to follow an old controller.
        activeInteractor = null;
        hasGrabReference = false;
    }

    // runs when the lever starts in the scene.
    // it remembers the track limits and starts the venue at the correct brightness.
    private void Start()
    {
        // checks where the lever handle was placed in the scene.
        Vector3 startPosition = transform.localPosition;

        // remembers the two directions that must stay fixed while the lever moves vertically.
        lockedX = startPosition.x;
        lockedZ = startPosition.z;

        // remembers the middle height used for the normal lighting level.
        startY = startPosition.y;

        // works out equally spaced bottom and top limits around the normal position.
        minY = startY - travelDistance;
        maxY = startY + travelDistance;

        // begins at the middle normal level and remembers it separately from the visible handle.
        heldY = startY;
        restingY = startY;

        // updates both lights to match the lever's initial height.
        UpdateLighting();
    }

    // runs after normal movement on every frame while the experience is active.
    // it follows the controller vertically, keeps the lever in its track and updates brightness.
    private void LateUpdate()
    {
        // begins with the lever's current position inside its wall track.
        Vector3 position = transform.localPosition;

        // only follows a controller while the user is actively holding the lever.
        if (activeInteractor != null)
        {
            // quest may not provide the controller grab point until the next frame.
            // remember it first without moving the lever, then follow later controller movement.
            if (!hasGrabReference)
            {
                hasGrabReference = TryCaptureGrabReference();
            }
            else if (TryGetInteractorTransform(out Transform interactorTransform))
            {
                // finds where the grabbed point should be from the controller's position and direction.
                // this also responds when a distant pointer is aimed up or down without moving the hand.
                Vector3 targetWorldPosition =
                    interactorTransform.TransformPoint(grabPointInInteractorSpace);
                Vector3 targetLocalPosition = transform.parent != null
                    ? transform.parent.InverseTransformPoint(targetWorldPosition)
                    : targetWorldPosition;

                // uses only the up or down part of the pointer or controller movement.
                position.y = targetLocalPosition.y;
            }
        }
        else
        {
            // xr may try to change the handle position while letting go.
            // the saved resting level keeps the handle from falling back to the bottom.
            position.y = restingY;
        }

        // keeps the handle inside its vertical track so it cannot be pulled away from the wall.
        position.x = lockedX;
        position.z = lockedZ;

        // stops the lever from moving higher or lower than its allowed range.
        position.y = Mathf.Clamp(
            position.y,
            minY,
            maxY
        );

        // remembers only heights reached while the user is holding the lever.
        // release uses this saved height instead of a position that xr may have changed.
        if (activeInteractor != null)
            heldY = position.y;

        // moves the visible lever handle to its final allowed position.
        transform.localPosition = position;

        // changes both room lights to match the lever's final height.
        UpdateLighting(position.y);
    }

    // runs when the user starts grabbing the lever handle.
    // it remembers which controller grabbed the lever and where the grab began.
    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        // remembers the real or simulated controller responsible for this grab.
        activeInteractor = args.interactorObject;

        // every new grab starts from the reliable middle position.
        // this prevents the lever staying stuck on dim and lets the user reach bright again.
        transform.localPosition = new Vector3(lockedX, startY, lockedZ);
        heldY = startY;
        restingY = startY;
        UpdateLighting(startY);

        // remembers the controller point now or safely tries again on the next frame.
        hasGrabReference = TryCaptureGrabReference();
    }

    // runs when the user releases the lever handle.
    // it stops following the controller and keeps the chosen dim, normal or bright level.
    private void OnSelectExited(SelectExitEventArgs args)
    {
        // saves the chosen level before forgetting which controller held the lever.
        // this avoids using a handle position that xr may already be changing during release.
        SnapToNearestLevel(heldY);

        activeInteractor = null;
        hasGrabReference = false;

        // updates both lights one final time after choosing the nearest resting level.
        UpdateLighting(restingY);
    }

    // remembers a reliable controller grab point without moving the lever on this frame.
    private bool TryCaptureGrabReference()
    {
        if (!TryGetInteractorTransform(out Transform interactorTransform))
            return false;

        // remembers where the lever sits compared with the controller when the grab begins.
        // this allows both controller movement and pointing direction to move a distant lever.
        grabPointInInteractorSpace =
            interactorTransform.InverseTransformPoint(transform.position);
        return true;
    }

    // finds the point used by the controller to hold the lever.
    // if quest has not provided that point yet, the controller's own position is used instead.
    private bool TryGetInteractorTransform(out Transform interactorTransform)
    {
        interactorTransform = null;

        if (activeInteractor == null)
            return false;

        interactorTransform =
            activeInteractor.GetAttachTransform(grabInteractable);

        if (interactorTransform != null)
            return true;

        if (activeInteractor is Component interactorComponent)
        {
            interactorTransform = interactorComponent.transform;
            return true;
        }

        return false;
    }

    // moves the released lever to one of its three clear lighting positions.
    private void SnapToNearestLevel(float releasedY)
    {
        // works out how far the released lever is between its bottom and top positions.
        float positionOnRail = Mathf.InverseLerp(
            minY,
            maxY,
            releasedY);

        // divides the track into dim, normal and bright sections.
        restingY = positionOnRail < 1f / 3f
            ? minY
            : positionOnRail > 2f / 3f
                ? maxY
                : startY;

        // moves the handle to the chosen level while keeping it inside the track.
        transform.localPosition = new Vector3(lockedX, restingY, lockedZ);
    }

    // checks the lever's current height and uses it to update the venue brightness.
    private void UpdateLighting()
    {
        UpdateLighting(transform.localPosition.y);
    }

    // works out how bright the venue should be from one lever height and updates both lights.
    private void UpdateLighting(float currentLocalY)
    {
        // measures how far the lever is between the darkest and brightest positions.
        float brightness =
            Mathf.InverseLerp(
                minY,
                maxY,
                currentLocalY
            );

        // changes the main room light when it has been connected in the scene.
        if (directionalLight != null)
        {
            // smoothly chooses the main light brightness between dim, normal and bright.
            directionalLight.intensity =
                EvaluateIntensity(
                    minDirectionalIntensity,
                    normalDirectionalIntensity,
                    maxDirectionalIntensity,
                    brightness
                );
        }

        // changes the softer room light when it has been connected in the scene.
        if (fillLight != null)
        {
            // smoothly chooses the softer light brightness from the same lever position.
            fillLight.intensity =
                EvaluateIntensity(
                    minFillIntensity,
                    normalFillIntensity,
                    maxFillIntensity,
                    brightness
                );
        }
    }

    // chooses the correct light strength from the dim, normal and bright settings.
    // the lower half changes from dim to normal and the upper half changes from normal to bright.
    private static float EvaluateIntensity(
        float minimum,
        float normal,
        float maximum,
        float brightness)
    {
        // gradually changes the venue from dim to normal while the lever is in the lower half.
        if (brightness <= 0.5f)
            return Mathf.Lerp(minimum, normal, brightness * 2f);

        // gradually changes the venue from normal to bright while the lever is in the upper half.
        return Mathf.Lerp(normal, maximum, (brightness - 0.5f) * 2f);
    }
}
