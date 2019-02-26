//Writer: Levin & Xingrong

using UnityEngine;
using System.Collections;

public class NormalOperation : MonoBehaviour {

    [Header("References")]
    public AudioManager audioManager;
    public DemoFingerAnimation demoFingerAnimation;
    public Material highlightMaterial;
    public Utility utility;
    public GameObject pauseMenu;
    public GameObject pauseButton;

    [HideInInspector] public bool clickedSlider;
    [HideInInspector] public bool sliderCompleted = false;

    private bool storyHasStarted = false;

    public void StartStory()
    {
        StartCoroutine(Story());
    }

    private IEnumerator Story()
    {
        //Disabled user interaction until a certain point in the narration
        utility.canSelectObjects = false;

        clickedSlider = false;

        //Makes sure the highlight for the outlet isn't visible
        highlightMaterial.color = new Color(highlightMaterial.color.r, highlightMaterial.color.g, highlightMaterial.color.b, 0);

        //Start normal operation and narration
        audioManager.PlaySound("Normal Operation", 1.0f, true, 2);
        yield return new WaitForSeconds(2);
        audioManager.PlaySound("Narration 1", 1.0f);

        //Wait until the narration has reached a certain point before continuing (may be changed/removed later)
        while (true)
        {
            if (audioManager.GetSound("Narration 1").hasCompleted)
                break;

            yield return null;
        }

        //Demo the cavitation sound while the narration is paused
        audioManager.PlaySound("Cavitation", 1.0f, true, 1);
        yield return new WaitForSeconds(3);
        audioManager.StopSound("Cavitation", true, 1);
        yield return new WaitForSeconds(1);
        audioManager.PlaySound("Narration 2");

        //Wait for the narration to complete
        while (true)
        {
            if (audioManager.GetSound("Narration 2").hasCompleted)
                break;

            yield return null;
        }

        //Start the highlight on the valve and let the user click on it
        StartCoroutine(utility.ValveHighlight(false));
        utility.canSelectObjects = true;

        while (!clickedSlider) {
            yield return null;
        }

        audioManager.PlaySound("Narration 3");

        //Wait until user has finshed the slider demo
        while (!sliderCompleted)
            yield return null;

        audioManager.StopSound("Narration 3");

        audioManager.PlaySound("Narration 4");

        while (true) {
            if (audioManager.GetSound("Narration 4").hasCompleted)
                break;

            yield return null;
        }

        yield return new WaitForSeconds(1);
        pauseMenu.SetActive(true);
        pauseButton.SetActive(false);
        utility.PauseAll();
    }
}