using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CavitationUtility : MonoBehaviour {

    [Header("References")]
    public Utility utility;
    public AudioManager audioManager;
    public SliderUpdate inletSlider;
    public SliderUpdate outletSlider;
    public FlowManager InletFlow;
    public FlowManager OutletFlow;

    [Header("Settings")]
    [Range(0, 2)] public int inletTarget;
    [Range(0, 2)] public int outletTarget;

    [HideInInspector] public float fadeTarget;
    [HideInInspector] public bool runCompletion = false;
    [HideInInspector] public bool bothSlidersOptimal;

    private bool inletOptimal;
    private bool outletOptimal;

    private void Awake()
    {
        inletSlider.Initialize();
        outletSlider.Initialize();
    }

    private void Update()
    {
            
        switch (inletTarget)
        {
            case 0:
                if (inletSlider.currentVal <= 0.33f)
                {
                    inletSlider.volume = 0.0f;
                    inletOptimal = true;
                }
                else if (inletSlider.currentVal > 0.66f)
                    inletSlider.volume = 0.5f;
                else
                    inletSlider.volume = 0.25f;
                break;
            case 1:
                if (inletSlider.currentVal > 0.33f && inletSlider.currentVal <= 0.66f)
                {
                    inletSlider.volume = 0.0f;
                    inletOptimal = true;
                }
                else
                    inletSlider.volume = 0.25f;
                break;
            case 2:
                if (inletSlider.currentVal > 0.66f && inletSlider.currentVal <= 1.0f)
                {
                    inletSlider.volume = 0.0f;
                    inletOptimal = true;
                }
                else if (inletSlider.currentVal <= 0.33f)
                    inletSlider.volume = 0.5f;
                else
                    inletSlider.volume = 0.25f;
                break;
            default:
                Debug.LogError("Inlet Value Not In Range");
                break;
        }

        switch (outletTarget)
        {
            case 0:
                if (outletSlider.currentVal <= 0.33f)
                {
                    outletSlider.volume = 0.0f;
                    outletOptimal = true;
                }
                else if (outletSlider.currentVal > 0.66f)
                    outletSlider.volume = 0.5f;
                else
                    outletSlider.volume = 0.25f;
                break;
            case 1:
                if (outletSlider.currentVal > 0.33f && outletSlider.currentVal <= 0.66f)
                {
                    outletSlider.volume = 0.0f;
                    outletOptimal = true;
                }
                else
                    outletSlider.volume = 0.25f;
                break;
            case 2:
                if (outletSlider.currentVal > 0.66f && outletSlider.currentVal <= 1.0f)
                {
                    outletSlider.volume = 0.0f;
                    outletOptimal = true;
                }
                else if (outletSlider.currentVal <= 0.33f)
                    outletSlider.volume = 0.5f;
                else
                    outletSlider.volume = 0.25f;
                break;
            default:
                Debug.LogError("Outlet Value Not In Range");
                break;
        }

        InletFlow.flashing = !inletOptimal;
        OutletFlow.flashing = !outletOptimal;

        fadeTarget = inletSlider.volume + outletSlider.volume;

        if (audioManager.GetSound("Cavitation").audioSource != null && !audioManager.fadingInSounds.Contains(audioManager.GetSound("Cavitation")))
            audioManager.GetSound("Cavitation").audioSource.volume = inletSlider.volume + outletSlider.volume;

        if (audioManager.GetSound("Recirculation") != null && audioManager.GetSound("Recirculation").audioSource != null && !audioManager.fadingInSounds.Contains(audioManager.GetSound("Recirculation")))
            audioManager.GetSound("Recirculation").audioSource.volume = inletSlider.volume + outletSlider.volume;
    }

    //Checks if the sliders are both in the correct sections (Called from SliderCompletionChecker which is on both sliders)
    public IEnumerator CheckComplete()
    {
        while (runCompletion)
        {
            if (Mathf.Round(inletSlider.currentVal * 100) == Mathf.Round(inletSlider.slider.value * 100) && Mathf.Round(outletSlider.currentVal * 100) == Mathf.Round(outletSlider.slider.value * 100) && inletOptimal && outletOptimal)
            {
                Debug.Log("Both sliders in optimal range");
                bothSlidersOptimal = true;
                utility.MenusOff();
                utility.canSelectObjects = false;
                inletOptimal = false;
                outletOptimal = false;
            }

            inletOptimal = false;
            outletOptimal = false;

            yield return null;
        }
    }
}