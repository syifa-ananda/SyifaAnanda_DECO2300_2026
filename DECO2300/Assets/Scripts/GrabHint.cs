// USED IN FINAL PROTOTYPE
// this script controls the grab [g] and rotate [t] instruction shown near furniture.
// it reacts when the controller points, grabs or rotates and keeps the message facing the player.
// it only explains the controls, while the rotation script performs the actual turn.

// lets this script show the message and keep it facing the player's camera.
using UnityEngine;

// lets this script know when the controller points at, grabs or rotates furniture.
using UnityEngine.XR.Interaction.Toolkit;

// uses the xr grab interactable, which lets the user pick up furniture in vr.
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// lets this script change the words shown in the floating help message.
using TMPro;

// updates the message and visibility of one furniture interaction hint.
public class GrabHint : MonoBehaviour
{
    // keeps the furniture's grab controls so the script knows what the user is doing.
    [SerializeField] private XRGrabInteractable grabInteractable;

    // keeps the complete help message that appears, disappears and turns towards the player.
    [SerializeField] private GameObject indicator;

    // keeps the words inside the message so they can change between grab and rotate.
    [SerializeField] private TMP_Text hintText;

    // keeps the exact grab reminder shown while the controller points at free furniture.
    private const string GrabMessage = "Grab [G]";

    // keeps the exact rotate reminder shown after the furniture has been picked up.
    private const string RotateMessage = "Rotate [T]";

    // remembers whether the user is currently holding the rotate control.
    private bool rotateInputHeld;

    // remembers the player's camera so the message can always face their current view.
    private Camera xrCamera;

    // runs when the furniture object is first loaded.
    // it finds missing references and makes sure the hint starts hidden.
    private void Awake()
    {
        // uses the grab controls on this furniture when they were not connected in unity.
        if (grabInteractable == null)
            grabInteractable = GetComponent<XRGrabInteractable>();

        // finds the message inside the hint when it was not connected directly in unity.
        if (hintText == null && indicator != null)
            hintText = indicator.GetComponentInChildren<TMP_Text>(true);

        // prevents the instruction appearing before the controller points or grabs.
        HideIndicator();
    }

    // runs whenever this component becomes active.
    // it listens for pointing, grabbing and rotating to decide which instruction is needed.
    private void OnEnable()
    {
        // begins with no rotate input held each time the hint is enabled.
        rotateInputHeld = false;

        // hides the message and stops when this furniture has no working grab controls.
        if (grabInteractable == null)
        {
            HideIndicator();
            return;
        }

        // listens for when the controller starts or stops pointing at this furniture.
        grabInteractable.hoverEntered.AddListener(OnHoverEntered);
        grabInteractable.hoverExited.AddListener(OnHoverExited);

        // listens for when the user picks up or lets go of the furniture with grab [g].
        grabInteractable.selectEntered.AddListener(OnSelectEntered);
        grabInteractable.selectExited.AddListener(OnSelectExited);

        // listens for when the user presses or releases rotate [t] while holding the furniture.
        grabInteractable.activated.AddListener(OnActivated);
        grabInteractable.deactivated.AddListener(OnDeactivated);

        // immediately shows the correct message for what the user is currently doing.
        RefreshHint();
    }

    // runs whenever this component becomes inactive.
    // it removes all listeners and leaves the hint safely hidden.
    private void OnDisable()
    {
        // only removes listeners when the grab component still exists.
        if (grabInteractable != null)
        {
            // stops listening for the controller pointing at or leaving the furniture.
            grabInteractable.hoverEntered.RemoveListener(OnHoverEntered);
            grabInteractable.hoverExited.RemoveListener(OnHoverExited);

            // stops listening for the user picking up or letting go of the furniture.
            grabInteractable.selectEntered.RemoveListener(OnSelectEntered);
            grabInteractable.selectExited.RemoveListener(OnSelectExited);

            // stops listening for the user pressing or releasing rotate [t].
            grabInteractable.activated.RemoveListener(OnActivated);
            grabInteractable.deactivated.RemoveListener(OnDeactivated);
        }

        // clears the remembered rotate state ready for the next enable.
        rotateInputHeld = false;

        // makes sure no old instruction remains visible on disabled furniture.
        HideIndicator();
    }

    // runs after the normal frame updates while this furniture is active.
    // it keeps the visible message facing the player's head as the user moves.
    private void LateUpdate()
    {
        // only turns the message while it exists and can currently be seen.
        if (indicator != null && indicator.activeInHierarchy)
            FaceCamera();
    }

    // runs when the controller first points at this furniture.
    // it normally changes the hidden indicator to grab [g].
    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        RefreshHint();
    }

    // runs when the controller stops pointing at this furniture.
    // it hides the hint unless another current interaction still needs a message.
    private void OnHoverExited(HoverExitEventArgs args)
    {
        RefreshHint();
    }

    // runs when grab [g] picks up the furniture.
    // it clears any old rotate state and changes the message to rotate [t].
    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        rotateInputHeld = false;
        RefreshHint();
    }

    // runs when grab [g] releases the furniture.
    // if the controller is still hovering, refresh shows grab [g] again.
    private void OnSelectExited(SelectExitEventArgs args)
    {
        rotateInputHeld = false;
        RefreshHint();
    }

    // runs when the user presses the rotate control while holding the furniture.
    // in the current simulator this represents pressing rotate [t], so the hint is hidden.
    private void OnActivated(ActivateEventArgs args)
    {
        rotateInputHeld = true;
        RefreshHint();
    }

    // runs when the user releases the rotate control.
    // while the object remains grabbed, the rotate [t] reminder can appear again.
    private void OnDeactivated(DeactivateEventArgs args)
    {
        rotateInputHeld = false;
        RefreshHint();
    }

    // runs when the rotation script reports that a 90 degree turn was completed.
    // it hides the reminder after the successful action without performing rotation itself.
    public void NotifySuccessfulRotation()
    {
        rotateInputHeld = true;
        RefreshHint();
    }

    // checks what the user is doing and chooses whether to show grab, rotate or nothing.
    private void RefreshHint()
    {
        // hides the message and stops safely if the furniture has no grab controls to check.
        if (grabInteractable == null)
        {
            HideIndicator();
            return;
        }

        // checks whether the user is currently holding the furniture with grab [g].
        if (grabInteractable.isSelected)
        {
            // pressing rotate [t] hides the instruction during the action.
            if (rotateInputHeld)
                HideIndicator();
            else
                // otherwise remind the user that rotate [t] is available while holding.
                ShowIndicator(RotateMessage);

            // stops because holding the furniture is more important than only pointing at it.
            return;
        }

        // pointing at free furniture shows grab [g], while looking away hides the message.
        if (grabInteractable.isHovered)
            ShowIndicator(GrabMessage);
        else
            HideIndicator();
    }

    // updates the hint text and makes the indicator visible.
    private void ShowIndicator(string message)
    {
        // only tries to show content when an indicator object was assigned.
        if (indicator != null)
        {
            // updates the message when the text component is available.
            if (hintText != null)
                hintText.text = message;

            // shows the complete indicator and immediately turns it towards the user.
            indicator.SetActive(true);
            FaceCamera();
        }
    }

    // hides the complete interaction hint without changing furniture behaviour.
    private void HideIndicator()
    {
        // avoids an error when this furniture model has no help message connected.
        if (indicator != null)
            indicator.SetActive(false);
    }

    // keeps the interaction message facing the player's head so it is always readable.
    private void FaceCamera()
    {
        // finds the player's current camera when the remembered camera is missing or inactive.
        if (xrCamera == null || !xrCamera.isActiveAndEnabled)
            xrCamera = Camera.main;

        // stops when either the player's camera or the indicator is unavailable.
        if (xrCamera == null || indicator == null)
            return;

        // works out which way the message should face to point towards the player's camera.
        Vector3 direction = indicator.transform.position - xrCamera.transform.position;

        // removes vertical tilt so the hint stays upright rather than leaning up or down.
        direction.y = 0f;

        // only turns the message when there is a clear direction towards the player.
        if (direction.sqrMagnitude > 0.001f)
            indicator.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }
}
