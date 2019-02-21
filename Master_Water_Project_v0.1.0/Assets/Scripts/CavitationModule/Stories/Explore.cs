using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Explore : MonoBehaviour {

    public AudioManager audioManager;
    public CavitationUtility cavitationUtility;
    public Utility utility;
    public GameObject pauseButton;
    public GameObject pauseMenu;

    private delegate void SelectedFunction();
    SelectedFunction selectedFunction;

    private string selectedAudio;

    private void Awake()
    {
        System.Random random = new System.Random();

        switch (random.Next(0, 3))
        {
            case 0:
                print("Current Scenario: Low Suction");
                cavitationUtility.inletTarget = 1;
                cavitationUtility.outletTarget = 2;
                cavitationUtility.inletSlider.GetComponent<Slider>().value = 0.1f;
                cavitationUtility.outletSlider.GetComponent<Slider>().value = 0.7f;
                selectedFunction = LowSuction;
                selectedAudio = "Low Suction Story";
                break;
            case 1:
                print("Current Scenario: High Discharge");
                cavitationUtility.inletTarget = 1;
                cavitationUtility.outletTarget = 0;
                cavitationUtility.inletSlider.GetComponent<Slider>().value = 0.4f;
                cavitationUtility.outletSlider.GetComponent<Slider>().value = 0.9f;
                selectedFunction = HighDischarge;
                selectedAudio = "High Discharge Story";
                break;
            case 2:
                print("Current Scenario: Recirculation");
                cavitationUtility.inletTarget = 1;
                cavitationUtility.outletTarget = 2;
                cavitationUtility.inletSlider.GetComponent<Slider>().value = 0.5f;
                cavitationUtility.outletSlider.GetComponent<Slider>().value = 0.3f;
                selectedFunction = Recirculation;
                selectedAudio = "Recirculation Story";
                break;
            default:
                Debug.LogError("A random number outside the range of 0 - 2 was created");
                break;
        }
    }

    public void StartStory()
    {
        StartCoroutine(Story());
    }

    private IEnumerator Story() {
        audioManager.PlaySound("Narration 1");

        while (true) {
            if (audioManager.GetSound("Narration 1").hasCompleted)
                break;

            yield return null;
        }

        audioManager.PlaySound(selectedAudio);

        while (true) {
            if (audioManager.GetSound(selectedAudio).hasCompleted)
                break;

            yield return null;
        }

        audioManager.PlaySound("Narration 2");

        while (true) {
            if (audioManager.GetSound("Narration 2").hasCompleted)
                break;

            yield return null;
        }

        selectedFunction();

        while (true) {
            if (cavitationUtility.bothSlidersOptimal) {
                audioManager.GetSound("Normal Operation").audioSource.volume = 0.2f;
                audioManager.PlaySound("Narration 3");
                break;
            }

            yield return null;
        }

        while (true) {
            if (audioManager.GetSound("Narration 3").hasCompleted)
                break;

            yield return null;
        }

        yield return new WaitForSeconds(1);
        pauseMenu.SetActive(true);
        pauseButton.SetActive(false);
        utility.PauseAll();
    }

    private void LowSuction()
    {
        audioManager.PlaySound("Normal Operation", 0.7f, true, 3);
        audioManager.PlaySound("Cavitation", cavitationUtility.fadeTarget, true, 3);
    }

    private void HighDischarge()
    {
        audioManager.PlaySound("Normal Operation", 0.7f, true, 3);
        audioManager.PlaySound("Cavitation", cavitationUtility.fadeTarget, true, 3);
    }

    private void Recirculation()
    {
        audioManager.PlaySound("Normal Operation", 0.7f, true, 3);
        audioManager.PlaySound("Cavitation", cavitationUtility.fadeTarget, true, 3);
        audioManager.PlaySound("Recirculation", cavitationUtility.fadeTarget, true, 3);
    }
}