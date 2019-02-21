//Writer: Alec

using System;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {


    public Scrollbar scrollNav;
    public Scrollbar scrollBack;
    public ScrollRect scrollRectBack;
    public ScrollRect scrollRectNav;
    public float speed;
    public bool navOn = true;
    public Image[] icons;
    public Color highlight;


    [SerializeField]
    Color basic;
    [SerializeField]
    private float targetVal;
    [SerializeField]
    private float stepSize;
    [SerializeField]
    private float numOfSteps;

    private Animation anim;
    private float side;

    // Use this for initialization
    void Start () {
        Time.timeScale = 1;
        basic = icons[0].color;
        navOn = true;
        side = 0;
        anim = this.GetComponent<Animation>();
        numOfSteps = Mathf.Round(1 / scrollBack.size);
        stepSize = 1 / (numOfSteps - 1);
	}

    // Update is called once per frame
    void LateUpdate() {
        NavigationFunction();
        NavigationSide();
	}

    void NavigationFunction()
    {
        if (navOn)
        {
            for (int i = 0; i < numOfSteps; i++)
            {
                if (scrollNav.value >= scrollBack.size * i && scrollNav.value < scrollBack.size * (i + 1))
                {
                    targetVal = stepSize * i;
                    
                }
            }
        }
        if (targetVal != scrollBack.value)
        {
            float difference = targetVal - scrollBack.value;
            float d = (difference > 0) ? 1 : -1;
            scrollBack.value = scrollBack.value + (d * speed * Time.deltaTime);

            if (Mathf.Abs(difference) < .01)
            {
                scrollBack.value = targetVal;

            }
        }

    }

    public void NavOut(float stepNum) {
        navOn = false;
        targetVal = stepSize*stepNum;
        anim.clip = anim["NavigationOut"].clip;
        anim.Play();
    }

    public void NavIn(float stepNum)
    {
        
        //navMask.SetActive(true);
        side = 0;
        float steps = Mathf.Round(1 / scrollBack.size);
        float navStepSize = 1 / (steps - 1);
        scrollNav.value = navStepSize * stepNum;
        scrollRectNav.verticalNormalizedPosition = scrollNav.value;
        if (!navOn)
        { 
            navOn = true;
            targetVal = navStepSize * stepNum;
            anim.clip = anim["NavigationIn"].clip;
            anim.Play();
        }
    }

    public void SideChange(float val) {
        side = val;
    }

    void NavigationSide() {
        if (scrollRectBack.horizontalNormalizedPosition != side)
        {
            float difference = side - scrollRectBack.horizontalNormalizedPosition;
            float d = (difference > 0) ? 1 : -1;
            scrollRectBack.horizontalNormalizedPosition += (d * speed * Time.deltaTime);

            if (Mathf.Abs(difference) < .01)
            {
                scrollRectBack.horizontalNormalizedPosition = side;
            }
            
        }
    }
    
    public void HighlightSelected (Image selected)
    {

        foreach (Image thing in icons)
        {
            if(thing == selected && Array.IndexOf(icons, thing) < (int)numOfSteps)
            {
                thing.color = highlight;
            } else if (Array.IndexOf(icons, thing) < (int)numOfSteps)
            {
                thing.color = basic;
            } 

        }
        

    }

    public void HighlightCounterpart(Image counterpart)
    {

        Color lowHighlight = new Color(highlight.r, highlight.g, highlight.b, .5f);
        Color lowBasic = new Color(basic.r, basic.g, basic.b, .5f);

        foreach (Image thing in icons)
        {
            if (thing == counterpart && Array.IndexOf(icons, thing) >= (int)numOfSteps)
            {
                thing.color = lowHighlight;
            }
            else if (Array.IndexOf(icons, thing) >= (int)numOfSteps)
            {
                thing.color = lowBasic;
            }

        }
    }

    public void DeselectAllPanes()
    {
        GifPlayer[] myItems = FindObjectsOfType(typeof(GifPlayer)) as GifPlayer[];
        GameObject[] playItems = GameObject.FindGameObjectsWithTag("PlayButton") as GameObject[];

        foreach (GifPlayer item in myItems)
        {
            //Debug.Log(item.gameObject.name);
            item.SetPlay(false);
        }
        foreach (GameObject item in playItems)
        {
            //Debug.Log(item.gameObject.name);
            item.SetActive(false);
        }
    }
}