//Writers: Alec, Levin & Xingrong

using UnityEngine;
using UnityEngine.UI;

public class SliderUpdate : MonoBehaviour {

    [Header("References")]
    public Utility utility;
    public Transform gaugeNeedle;
    public Transform valveOrientation;
    public FlowManager flow;
    public float minRange = 230f;
    public float maxRange = 480f;

    [Header("Settings")]
    public float speed = 1.0f;

    [Header("Properties")]
    [ReadOnly] public float currentVal;
    [ReadOnly] public float volume;

    [HideInInspector] public Slider slider;

    private void OnEnable() {
        if (valveOrientation.name.Contains(utility.correctValve))
            utility.valveSelected = true;
        utility.inletHighlight.color = new Color(utility.inletHighlight.color.r, utility.inletHighlight.color.g, utility.inletHighlight.color.b, 0);
        utility.outletHighlight.color = new Color(utility.outletHighlight.color.r, utility.outletHighlight.color.g, utility.outletHighlight.color.b, 0);
    }

    public void Initialize()
    {
        slider = GetComponent<Slider>();
        currentVal = slider.value;
        UpdateAngles();
    }

    private void Update()
    {
        //CurrentVal will always lerp to the actual value of the slider to avoid jumping of sound levels
        if (currentVal != slider.value)
        {
            //Calculates the distance between currentVal and the sliders value
            float distance = (slider.value - currentVal) * Time.deltaTime * speed;

            //Updates currentVal until it reaches the value of the slider
            if (Mathf.Abs(distance) < Mathf.Abs(slider.value - currentVal))
                currentVal += distance;
            else
                currentVal = slider.value;

            UpdateAngles();
        }
    }

    //Updates the angles of the gauges and valves
    public void UpdateAngles()
    {
        flow.currentSpeed = currentVal;
        //Updates the gauge needles angle within a certain range with currentVal
        float gaugeAngle = Mathf.Lerp(minRange, maxRange, Mathf.InverseLerp(slider.minValue, slider.maxValue, currentVal));
        gaugeNeedle.eulerAngles = new Vector3(0, 0, gaugeAngle);

        //Updates the valves angle within a certain range with currentVal
        float valveAngle = Mathf.Lerp(0, 180, Mathf.InverseLerp(slider.minValue, slider.maxValue, currentVal));
        valveOrientation.eulerAngles = new Vector3(valveOrientation.eulerAngles.x, valveOrientation.eulerAngles.y, valveAngle);
    }
}