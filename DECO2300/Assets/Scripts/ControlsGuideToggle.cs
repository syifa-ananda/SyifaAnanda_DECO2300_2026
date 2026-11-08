// USED IN FINAL PROTOTYPE
// this script controls the book button for the event planning guide.
// it opens and closes the current guide without changing any other menu.

// lets this script show and hide objects while the vr experience is running.
using UnityEngine;

// shows or hides the event planning guide when the user presses its book button.
public class ControlsGuideToggle : MonoBehaviour
{
    // keeps the guide panel that explains adding, grabbing, rotating, removing, lighting and themes.
    [SerializeField] private GameObject guidePanel;

    // runs whenever the book button and guide become available in the scene.
    // it makes sure the guide starts hidden until the user presses the book button.
    private void OnEnable()
    {
        // only tries to hide the guide when the panel has been connected correctly.
        if (guidePanel != null)
            guidePanel.SetActive(false);
    }

    // runs when the user presses the book button in the venue.
    // it changes the guide from open to closed or from closed to open.
    public void ToggleGuide()
    {
        // avoids an error if the guide panel was not connected in unity.
        if (guidePanel != null)
            guidePanel.SetActive(!guidePanel.activeSelf);
    }
}
