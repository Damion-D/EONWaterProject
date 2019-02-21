using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class SludgeJudgeScenario : MonoBehaviour, ITrackableEventHandler
{
    TrackableBehaviour mTrackableBehaviour;
    bool storyHasStarted;

    [Header("Object References")]
    [SerializeField] Transform sludgeJudge;

    [Space]

    public Camera mainCam;

    [Space]

    [Header("Sludge Colours")]
    public SludgeType sludgeType;

    public Material[] coloredMats;
    public Color[] sludgeColors;


    [SerializeField] private List<SludgeType> sludgeOptions = new List<SludgeType>();

    public enum SludgeType {Primary, Chemical, ActivatedDark, ActivatedLight};

    // Start is called before the first frame update
    void Start()
    {
        //Set up the event handler for tracking from Vuforia
        mTrackableBehaviour = GameObject.Find("ImageTarget").GetComponent<TrackableBehaviour>();

        if (mTrackableBehaviour)
            mTrackableBehaviour.RegisterTrackableEventHandler(this);


        mainCam = Camera.main;
        for (int i = 0; i < 4; i++)
        {
            sludgeOptions.Add((SludgeType)i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (sludgeOptions.Count > 0 && Input.GetKeyDown(KeyCode.A))
        {
            RandomizeSludge();
            SludgeColor();
        }

        if(Input.GetKeyDown(KeyCode.S))
        {
            RaycastHit hit = DetectTouch();
            if(hit.transform != null)
            {
                Debug.Log(hit.transform.name);
            }
        }
    }

    void RandomizeSludge()
    {
        sludgeType = sludgeOptions[Random.Range(0, sludgeOptions.Count)];
        sludgeOptions.Remove(sludgeType);

        Debug.Log("Currently " + sludgeType + " sludge.");
    }

    void SludgeColor()
    {
        for (int i = 0; i < coloredMats.Length; i++)
        {
            coloredMats[i].color = sludgeColors[(int)sludgeType];
        }
    }

    //Takes a touch on the screen, and converts it into a raaycast into the scene
    RaycastHit DetectTouch()
    {
        //Checks to see if there are any current touches, which avoids errors from Input.GetTouch
        /*if (Input.touchCount < 1)
            return new RaycastHit();*/

        Touch touch = Input.GetTouch(0);
        RaycastHit hit = new RaycastHit();
        //Physics.Raycast(mainCam.ScreenPointToRay(touch.position), out hit);
        if(Input.GetMouseButtonDown(0))
        {
            Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out hit);
            Debug.Log(hit.transform.name + " was hit");
        }
        return hit;
    }

    IEnumerator SludgeJudgeStory()
    {
        while(true)
        {
            Debug.Log("While 1 started");
            /*if (DetectTouch().transform == sludgeJudge)
            {
                Debug.Log("Sludge judge tapped");
            }*/
            yield return null;
        }
    }





    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        if ((newStatus == TrackableBehaviour.Status.TRACKED || newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED) && previousStatus == TrackableBehaviour.Status.NO_POSE && storyHasStarted == false)
        {
            Debug.Log("Starting story");
            storyHasStarted = true;
            StartCoroutine("SludgeJudgeStory");
        }
    }
}
