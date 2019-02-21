using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaugeHighlighter : MonoBehaviour {

    public Material GaugeHighlightMat;
    public Material InletGaugeHighlightMat;
    public GameObject OutletSlider;
    public GameObject InletSlider;
    private bool highlightingOutlet = true;
    private bool highlightingInlet = true;
    public float alphaZero = 0f;
	// Use this for initialization
	void Start () {
        Debug.Log("Start on GaugeHighlighter is working!");
        InletGaugeHighlightMat.color = new Color(InletGaugeHighlightMat.color.r, InletGaugeHighlightMat.color.g, InletGaugeHighlightMat.color.b, alphaZero);
        GaugeHighlightMat.color = new Color(GaugeHighlightMat.color.r, GaugeHighlightMat.color.g, GaugeHighlightMat.color.b, alphaZero);
    }
	
	// Update is called once per frame
	void Update () {

        highlightingInlet = InletSlider.activeInHierarchy;
        highlightingOutlet = OutletSlider.activeInHierarchy;

        if (OutletSlider.activeInHierarchy)
        {
            GaugeHighlightMat.color = new Color(GaugeHighlightMat.color.r, GaugeHighlightMat.color.g, GaugeHighlightMat.color.b, 255f);
            InletGaugeHighlightMat.color = new Color(InletGaugeHighlightMat.color.r, InletGaugeHighlightMat.color.g, InletGaugeHighlightMat.color.b, alphaZero);
            /*StartCoroutine("HighlightOutletGaugeFlash");
            StopCoroutine("HighlightInletGaugeFlash");*/
        }

        if (InletSlider.activeInHierarchy)
        {
            GaugeHighlightMat.color = new Color(GaugeHighlightMat.color.r, GaugeHighlightMat.color.g, GaugeHighlightMat.color.b, alphaZero);
            InletGaugeHighlightMat.color = new Color(InletGaugeHighlightMat.color.r, InletGaugeHighlightMat.color.g, InletGaugeHighlightMat.color.b, 255f);
            /*StartCoroutine("HighlightInletGaugeFlash");
            StopCoroutine("HighlightOutletGaugeFlash");*/
        }
		
	}

    private IEnumerator HighlightOutletGaugeFlash()
    {
        InletGaugeHighlightMat.color = new Color(InletGaugeHighlightMat.color.r, InletGaugeHighlightMat.color.g, InletGaugeHighlightMat.color.b, alphaZero);
        Color c = GaugeHighlightMat.color;
        int direction = 1;
        int highlightSpeed = 2;
        while (highlightingOutlet)
        {
            if (GaugeHighlightMat.color.a >= 1)
                direction = -1;

            else if (GaugeHighlightMat.color.a <= 0)
                direction = 1;

            GaugeHighlightMat.color = new Color(GaugeHighlightMat.color.r, GaugeHighlightMat.color.g, GaugeHighlightMat.color.b, GaugeHighlightMat.color.a + Time.deltaTime * highlightSpeed * direction);
            yield return null;
        }



    }

    private IEnumerator HighlightInletGaugeFlash()
    {
        GaugeHighlightMat.color = new Color(GaugeHighlightMat.color.r, GaugeHighlightMat.color.g, GaugeHighlightMat.color.b, alphaZero);
        Color c = InletGaugeHighlightMat.color;
        int direction = 1;
        int highlightSpeed = 2;
        while (highlightingInlet)
        {
            if (InletGaugeHighlightMat.color.a >= 1)
                direction = -1;

            else if (InletGaugeHighlightMat.color.a <= 0)
                direction = 1;

            InletGaugeHighlightMat.color = new Color(InletGaugeHighlightMat.color.r, InletGaugeHighlightMat.color.g, InletGaugeHighlightMat.color.b, InletGaugeHighlightMat.color.a + Time.deltaTime * highlightSpeed * direction);
            yield return null;
        }



    }
}
