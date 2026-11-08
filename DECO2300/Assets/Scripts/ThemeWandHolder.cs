// USED IN FINAL PROTOTYPE
// this script controls the socket on the current theme pedestal.
// inserting a modern, warm or dark wand changes the venue to that theme.

// lets this script find the holder parts and show testing messages.
using UnityEngine;

// lets this script know when a wand enters or leaves the holder.
using UnityEngine.XR.Interaction.Toolkit;

// uses the xr socket, which is the holder that catches a theme wand in vr.
using UnityEngine.XR.Interaction.Toolkit.Interactors;

// connects the physical wand holder to the system that changes venue colours.
public class ThemeWandHolder : MonoBehaviour
{
    [Header("References")]
    // keeps the xr socket that notices when a wand enters or leaves the holder.
    public XRSocketInteractor socketInteractor;

    // keeps the theme manager that changes the room surfaces, lights and interface.
    public ThemeManager themeManager;

    // remembers which theme wand is currently accepted by this holder.
    private ThemeWand currentWand;

    // runs when the holder is first loaded.
    // it finds the holder on the same object when it was not connected in unity.
    private void Awake()
    {
        // safely uses the xr socket found on this holder object.
        if (socketInteractor == null)
            socketInteractor = GetComponent<XRSocketInteractor>();
    }

    // runs whenever the holder becomes active.
    // it listens for theme wands entering and leaving the socket.
    private void OnEnable()
    {
        // only listens for wands when the holder has a working xr socket.
        if (socketInteractor != null)
        {
            // listens for the moment a wand is accepted by the pedestal.
            socketInteractor.selectEntered.AddListener(OnWandInserted);

            // listens for the moment a wand is removed from the pedestal.
            socketInteractor.selectExited.AddListener(OnWandRemoved);
        }
    }

    // runs whenever the holder becomes inactive.
    // it removes the socket listeners added while active.
    private void OnDisable()
    {
        // only removes listeners when the socket still exists.
        if (socketInteractor != null)
        {
            socketInteractor.selectEntered.RemoveListener(OnWandInserted);
            socketInteractor.selectExited.RemoveListener(OnWandRemoved);
        }
    }

    // runs when the user places an object into the theme holder.
    // it makes sure the inserted object is a theme wand before changing the venue.
    private void OnWandInserted(SelectEnterEventArgs args)
    {
        // first checks the exact object accepted by the holder for a theme wand script.
        ThemeWand wand =
            args.interactableObject.transform.GetComponent<ThemeWand>();

        // also checks the main object because a smaller wand part may enter the holder.
        if (wand == null)
            wand =
                args.interactableObject.transform.GetComponentInParent<ThemeWand>();

        // ignores any socket object that is not part of a theme wand.
        if (wand == null)
            return;

        // remembers the valid theme wand currently placed in the holder.
        currentWand = wand;

        // tells the wand that the socket caught it so it does not return to the wall.
        wand.SetInHolder(true);

        // asks the inserted wand to apply its modern, warm or dark theme.
        wand.ApplyTheme(themeManager);

        // reports the inserted theme in the unity console for testing.
        Debug.Log(
            "Wand inserted into pedestal: " +
            wand.theme
        );
    }

    // runs when the user removes an object from the theme holder.
    // it clears the wand's holder state so normal release return can work again.
    private void OnWandRemoved(SelectExitEventArgs args)
    {
        // first checks the exact object leaving the holder for a theme wand script.
        ThemeWand wand =
            args.interactableObject.transform.GetComponent<ThemeWand>();

        // also checks the parent when the socket interacted with a child object.
        if (wand == null)
            wand =
                args.interactableObject.transform.GetComponentInParent<ThemeWand>();

        // ignores removed socket objects that are not theme wands.
        if (wand == null)
            return;

        // tells the valid wand that it is no longer safely held by the socket.
        wand.SetInHolder(false);

        // forgets the wand only when it is the one currently remembered by the holder.
        if (currentWand == wand)
            currentWand = null;

        // reports the removed theme in the unity console for testing.
        Debug.Log(
            "Wand removed from pedestal: " +
            wand.theme
        );
    }
}
