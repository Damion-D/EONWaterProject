using UnityEngine;
using System.Collections;

public class HighDischarge : MonoBehaviour {

    [Header("Refrences")]
    public Utility utility;
    public AudioManager audioManager;
    public CavitationUtility cavitationUtility;
    public GameObject pauseMenu;
    public GameObject pauseButton;
    public Material OtherHighlightMat;
    public Material InletGaugeHighlightMat;
    public bool highlighting = true;
    public float alphaZero = 0f;
    public GameObject HighDischargeLabel;

    void Start()
    {
        Debug.Log("Start is working!");
        //OtherHighlightMat = GetComponent<Renderer>().material;
        OtherHighlightMat.color = new Color(OtherHighlightMat.color.r, OtherHighlightMat.color.g, OtherHighlightMat.color.b, alphaZero);
        InletGaugeHighlightMat.color = new Color(InletGaugeHighlightMat.color.r, InletGaugeHighlightMat.color.g, InletGaugeHighlightMat.color.b, alphaZero);
        //Color clr = OtherHighlightMat.color;
        //clr.a = 0f;
        //Debug.Log("Color is: " + clr.a);
        //OtherHighlightMat.color = clr;
    }
    public void StartStory()
    {
        
        StartCoroutine(Story());
    }

    private IEnumerator HighlightGaugeFlash()
    {
        Color c = OtherHighlightMat.color;
        int direction = 1;
        int highlightSpeed = 2;
        while (highlighting)
        {
            /*if (c.a >= 0.9f)
            {
                for (float f = 1f; f >= 0; f -= 0.1f)
                {
                    Color cl = OtherHighlightMat.color;
                    cl.a = f;
                    OtherHighlightMat.color = cl;
                    yield return null;
                }
            }

            if (c.a <= 0.1f)
            {
                for (float f = 0.1f; f <= 1; f += 0.1f)
                {
                    Color co = OtherHighlightMat.color;
                    co.a = f;
                    OtherHighlightMat.color = co;
                    yield return null;
                }
            }
            yield return null;*/
            if (OtherHighlightMat.color.a >= 1)
                direction = -1;

            else if (OtherHighlightMat.color.a <= 0)
                direction = 1;

            OtherHighlightMat.color = new Color(OtherHighlightMat.color.r, OtherHighlightMat.color.g, OtherHighlightMat.color.b, OtherHighlightMat.color.a + Time.deltaTime * highlightSpeed * direction);
            yield return null;
        }
        
        
        
    }

    
    private IEnumerator Story()
    {
        utility.canSelectObjects = false;

        yield return new WaitForSeconds(1);

        audioManager.PlaySound("Narration 1");

        while (true)
        {
            if (audioManager.GetSound("Narration 1").hasCompleted)
                break;

            yield return null;
        }

        utility.canSelectObjects = true;
        //StartCoroutine("HighlightGaugeFlash");
        StartCoroutine(utility.ValveHighlight(false));
        
        audioManager.PlaySound("Normal Operation", 0.7f, true, 3);
        audioManager.PlaySound("Cavitation", cavitationUtility.fadeTarget, true, 3);

        while (true)
        {
            //StartCoroutine(utility.ValveHighlight(false));
            
            if (cavitationUtility.bothSlidersOptimal)
            {
                highlighting = false;
                StopCoroutine("HighlightGaugeFlash");
                break;
            }
                

            yield return null;
        }

        audioManager.GetSound("Normal Operation").audioSource.volume = 0.2f;
        audioManager.PlaySound("Narration 2");

        while (true)
        {
            if (audioManager.GetSound("Narration 2").hasCompleted)
                break;

            yield return null;
        }

        yield return new WaitForSeconds(1);
        pauseMenu.SetActive(true);
        pauseButton.SetActive(false);
        utility.PauseAll();
    }

    void Update()
    {
        if (!HighDischargeLabel.activeInHierarchy)
        {
            HighDischargeLabel.SetActive(true);
        }
    }
}