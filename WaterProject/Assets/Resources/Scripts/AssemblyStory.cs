using UnityEngine;
using System.Collections;

public class AssemblyStory : MonoBehaviour {

    [Header("References")]
    public MovementManager movementManager;
    public AudioManager audioManager;
    public Utility utility;
    public GameObject pauseMenu;
    public GameObject pauseButton;

    public void StartStory()
    {
        utility.canSelectObjects = false;
        StartCoroutine(Story());
    }

    private IEnumerator Story()
    {
        yield return new WaitForSeconds(0.5f);
        audioManager.PlaySound("Narration 2");

        while (true) {
            if (audioManager.GetSound("Narration 2").hasCompleted)
                break;

            yield return null;
        }

        utility.canSelectObjects = true;

        while (true)
        {
            if (movementManager.allPartsAssembled)
                break;

            yield return null;
        }

        yield return new WaitForSeconds(2);
        pauseMenu.SetActive(true);
        pauseButton.SetActive(false);
        utility.PauseAll();
    }
}