//Writer: Levin & Xingrong

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DemoSliderEvent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    [Header("References")]
    public GameObject image;
    public Utility utility;
    public NormalOperation normalOperation;

    private Slider slider;

    private void OnEnable()
    {
        slider = GetComponent<Slider>();

        normalOperation.clickedSlider = true;
        utility.valveSelected = true;
        normalOperation.highlightMaterial.color = new Color(normalOperation.highlightMaterial.color.r, normalOperation.highlightMaterial.color.g, normalOperation.highlightMaterial.color.b, 0);
        StartCoroutine(image.GetComponent<DemoFingerAnimation>().MovingAnimation());
    }

    //Uses the event handler when the user clicks anywhere on the slider
    public void OnPointerDown(PointerEventData eventData)
    {
        if(image != null)
        {
            //Destroys the demo finger and flips a bool to ensure the animation gets stopped and doesn't error
            image.GetComponent<DemoFingerAnimation>().complete = true;
            image.SetActive(false);
        }
    }

    //Uses the event handler when the user releases from anywhere on the slider
    public void OnPointerUp(PointerEventData eventData)
    {
        //If the slider reaches the top
        if (slider.value >= 0.99)
        {
            //Turns off all the menus and allows the normal operation story to continue
            normalOperation.sliderCompleted = true;
            utility.MenusOff();
        }
    }
}