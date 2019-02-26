//Writer: Alec & Levin

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ARSetupStory : MonoBehaviour {

    public AudioManager audioManager;
    public MeshRenderer meshRenderer;
    public Image settings;
    public Image checker;

    private void Start()
    {
        StartCoroutine(Intro());
    }

    IEnumerator Intro()
    {
        yield return new WaitForSeconds(1);
        audioManager.PlaySound("Narration 1");

        //Checks if tracking has started and if the audio has completed
        while (true)
        {
            if (audioManager.GetSound("Narration 1").hasCompleted && meshRenderer.enabled)
                break;

            yield return null;
        }

        //Change square color and start sound
        checker.color = new Color(0, 1, 0);

        yield return new WaitForSeconds(1);

        //Enable the home menu button
        settings.gameObject.SetActive(true);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(0);
    }
}