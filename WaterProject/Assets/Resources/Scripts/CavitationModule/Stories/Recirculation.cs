using UnityEngine;
using System.Collections;

public class Recirculation : MonoBehaviour {

    [Header("Refrences")]
    public Utility utility;
    public AudioManager audioManager;
    public CavitationUtility cavitationUtility;
    public GameObject pauseMenu;
    public GameObject pauseButton;

    public void StartStory()
    {
        StartCoroutine(Story());
    }

    private IEnumerator Story()
    {
        utility.canSelectObjects = false;

        yield return new WaitForSeconds(1);

        audioManager.PlaySound("Narration 1");

        while (true)
        {
            if (audioManager.GetSound("Narration 1").hasCompleted)
                break;

            yield return null;
        }

        StartCoroutine(utility.ValveHighlight(false));
        utility.canSelectObjects = true;
        audioManager.PlaySound("Normal Operation", 0.7f, true, 3);
        audioManager.PlaySound("Cavitation", cavitationUtility.fadeTarget, true, 3);
        audioManager.PlaySound("Recirculation", cavitationUtility.fadeTarget, true, 3);

        while (true)
        {
            if (cavitationUtility.bothSlidersOptimal)
                break;

            yield return null;
        }

        audioManager.GetSound("Normal Operation").audioSource.volume = 0.2f;
        audioManager.PlaySound("Narration 2");

        while (true)
        {
            if (audioManager.GetSound("Narration 2").hasCompleted)
                break;

            yield return null;
        }

        yield return new WaitForSeconds(1);
        pauseMenu.SetActive(true);
        pauseButton.SetActive(false);
        utility.PauseAll();
    }
}