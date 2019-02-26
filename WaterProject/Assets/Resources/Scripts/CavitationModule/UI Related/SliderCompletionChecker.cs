using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SliderCompletionChecker : MonoBehaviour, IPointerUpHandler, IPointerDownHandler {

    public CavitationUtility cavitationUtility;

    public void OnPointerUp(PointerEventData eventData)
    {
        cavitationUtility.runCompletion = true;
        StartCoroutine(cavitationUtility.CheckComplete());
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        cavitationUtility.runCompletion = false;
    }
}