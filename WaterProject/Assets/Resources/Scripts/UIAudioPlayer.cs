using UnityEngine;

public class UIAudioPlayer : MonoBehaviour {

    public AudioManager audioManager;
    public MovementManager movementManager;
    public GameObject part;

    public bool playable;

    private void OnEnable()
    {
        if (movementManager.allPartsAssembled)
        {
            while (audioManager.playingSounds.Count > 0)
                audioManager.StopSound(audioManager.playingSounds[0].name);

            audioManager.PlaySound(gameObject.tag);
            return;
        }

        if (playable)
        {
            playable = false;
            movementManager.currentObject = part;
            movementManager.StartAnimation();
        }
        else
            return; //Add audio here later
    }
}