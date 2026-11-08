// USED IN FINAL PROTOTYPE
// this script represents one modern, warm or dark theme wand.
// the user grabs the wand, places it into the theme holder and the wand applies its theme.
// a released wand returns to its wall position when the holder did not catch it.

// lets the wand wait briefly before deciding whether to return to the wall.
using System.Collections;

// lets this script move the wand, remember its home position and show testing messages.
using UnityEngine;

// lets this script know when the user releases the wand.
using UnityEngine.XR.Interaction.Toolkit;

// uses the xr grab interactable, which lets the user pick up the theme wand in vr.
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// remembers one wand's theme and returns it to the wall when it misses the holder.
public class ThemeWand : MonoBehaviour
{
    // lists the three venue themes that a wand can represent.
    public enum WandTheme
    {
        // represents the beige, brown and muted red venue theme.
        Warm,

        // represents the cool blue-grey default venue theme.
        Modern,

        // represents the charcoal, burgundy and muted purple venue theme.
        Dark
    }

    [Header("Theme Type")]
    // selects which theme this specific physical wand applies in the holder.
    public WandTheme theme;

    [Header("XR")]
    // keeps the grab controls that let the user pick up this wand.
    public XRGrabInteractable grabInteractable;

    // remembers where the wand originally sits on the venue wall.
    private Vector3 homePosition;

    // remembers which direction the wand originally faces on the wall.
    private Quaternion homeRotation;

    // remembers whether the current theme holder has safely accepted this wand.
    private bool isInHolder = false;

    // keeps the short release check so another one can be cancelled if needed.
    private Coroutine returnCoroutine;

    // runs when the wand is first loaded.
    // it finds the grab controls and remembers the wand's original wall placement.
    private void Awake()
    {
        // uses the grab controls on this wand when they were not connected in unity.
        if (grabInteractable == null)
            grabInteractable = GetComponent<XRGrabInteractable>();

        // saves the wand's original wall position and direction for returning it later.
        homePosition = transform.position;
        homeRotation = transform.rotation;
    }

    // runs whenever this wand becomes active.
    // it listens for the moment grab [g] releases the wand.
    private void OnEnable()
    {
        // only listens for release when the wand has working grab controls.
        if (grabInteractable != null)
        {
            grabInteractable.selectExited.AddListener(OnReleased);
        }
    }

    // runs whenever this wand becomes inactive.
    // it removes the release listener added while the wand was active.
    private void OnDisable()
    {
        // only stops listening when the wand's grab controls still exist.
        if (grabInteractable != null)
        {
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }
    }

    // runs when the theme holder accepts this wand in its socket.
    // it tells the theme manager which colours this wand represents.
    public void ApplyTheme(ThemeManager themeManager)
    {
        // stops safely when the holder is not connected to the theme manager.
        if (themeManager == null)
            return;

        // checks which theme identity was assigned to this physical wand.
        switch (theme)
        {
            // applies the warm environment, lighting and ui colours.
            case WandTheme.Warm:
                themeManager.WarmTheme();
                break;

            // applies the modern environment, lighting and ui colours.
            case WandTheme.Modern:
                themeManager.ModernTheme();
                break;

            // applies the dark environment, lighting and ui colours.
            case WandTheme.Dark:
                themeManager.DarkTheme();
                break;
        }

        // reports the applied theme in the unity console for testing.
        Debug.Log("Applied theme: " + theme);
    }

    // runs when the holder catches or releases this wand.
    // it controls whether the release check should return the wand to the wall.
    public void SetInHolder(bool value)
    {
        // remembers whether the theme holder currently contains this wand.
        isInHolder = value;

        // cancels a pending return when the socket has successfully caught the wand.
        if (isInHolder && returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
            returnCoroutine = null;
        }
    }

    // runs when the user releases grab [g] on this wand.
    // it starts a short check that gives the socket time to accept the wand.
    private void OnReleased(SelectExitEventArgs args)
    {
        // prevents more than one release check from running at the same time.
        if (returnCoroutine != null)
            StopCoroutine(returnCoroutine);

        // begins the delayed check for holder acceptance.
        returnCoroutine = StartCoroutine(CheckReturnHome());
    }

    // waits briefly after release, then returns the wand only when it is outside the holder.
    private IEnumerator CheckReturnHome()
    {
        // gives the holder a short moment to confirm that it caught the wand.
        yield return new WaitForSeconds(0.1f);

        // sends the wand home when the holder did not mark it as inserted.
        if (!isInHolder)
        {
            ReturnHome();
        }

        // records that the delayed release check has completed.
        returnCoroutine = null;
    }

    // returns this wand to the wall position and direction saved when it loaded.
    public void ReturnHome()
    {
        // moves and turns the wand back to its original wall placement in one operation.
        transform.SetPositionAndRotation(
            homePosition,
            homeRotation
        );

        // reports the returned wand in the unity console for testing.
        Debug.Log(
            "Returned wand to wall: " + theme
        );
    }
}
