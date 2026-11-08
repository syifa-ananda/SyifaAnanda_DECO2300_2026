// USED IN FINAL PROTOTYPE
// this script performs the current rotate [t] action on furniture.
// each press turns furniture that the user is already holding by 90 degrees left or right.

// lets this script turn the furniture and find its nearby help message.
using UnityEngine;

// turns one furniture object and tells its help message that the rotation worked.
public class RotateFurniture : MonoBehaviour
{
    // keeps the nearby help message so it can hide after the furniture turns.
    private GrabHint grabHint;

    // runs when this furniture object is first loaded.
    // it finds and remembers the help message attached to the whole furniture object.
    private void Awake()
    {
        grabHint = GetComponent<GrabHint>();
    }

    // runs when the user presses t while already holding the furniture.
    // each press turns the object another 90 degrees around the upright direction.
    public void Rotate90()
    {
        // turns the furniture by 90 degrees without moving it or changing its size.
        transform.Rotate(0f, 90f, 0f, Space.World);

        // tries to find the help message again if it was unavailable when loading.
        if (grabHint == null)
            grabHint = GetComponent<GrabHint>();

        // hides the help message after the user successfully rotates with [t].
        // the grab hint explains the control but does not perform the rotation itself.
        if (grabHint != null)
            grabHint.NotifySuccessfulRotation();
    }
}
