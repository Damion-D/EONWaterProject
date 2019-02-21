using System.Collections;
using UnityEngine;

public class MovementManager : MonoBehaviour {

    [Header("References")]
    public Utility utility;
    public AudioManager audioManager;
    public GameObject viewPosition;
    public Animator animator;

    [Header("Settings")]
    public bool allPartsAssembled = false;
    public float speed = 20.0f;

    [HideInInspector] public GameObject currentObject;

    public void StartAnimation()
    {
        StartCoroutine(AnimateTarget(currentObject.transform, viewPosition.transform.position));
    }

    private IEnumerator AnimateTarget(Transform target, Vector3 camPos)
    {
        animator.enabled = false;
        Vector3 startPos = target.position;
        yield return StartCoroutine(TransformTarget(target, camPos, true));

        while (true) {
            if (audioManager.GetSound(currentObject.GetComponent<PartID>().uIComponent.tag).hasCompleted)
            {
                yield return new WaitForSeconds(0.5f);
                break;
            }

            yield return null;
        }

        yield return StartCoroutine(TransformTarget(target, startPos, false));
        animator.enabled = true;
        animator.SetTrigger("AnimateTarget");

        if (target.GetComponent<PartID>().uIComponent.tag == "ElectricMotor")
            allPartsAssembled = true;
    }

    IEnumerator TransformTarget(Transform targ, Vector3 newPosition, bool movingToCam)
    {
        Vector3 d = newPosition - targ.position;
        Vector3 distance = d.normalized;

        while (targ.position != newPosition)
        {
            if (d.magnitude < distance.magnitude * Time.deltaTime * speed)
                targ.position = newPosition;
            else
                targ.position += distance * Time.deltaTime * speed;

            d = newPosition - targ.position;
            yield return null;
        }

        if (movingToCam)
            audioManager.PlaySound(currentObject.GetComponent<PartID>().uIComponent.tag);
        else
            utility.MenusOff();
    }
}
