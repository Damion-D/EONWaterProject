using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class TitrationScenario : MonoBehaviour, ITrackableEventHandler
{
    //Water Sample is often shortened to wS

    //Variables to begin the scenario
    TrackableBehaviour mTrackableBehaviour;
    bool scenarioHasStarted;
    int clipboardInput = 0;

    [SerializeField] float waitTime;
    [SerializeField] int step = -1;
    [Space]
    //If restarted, skips dialogue
    [SerializeField] int restarts;
    [SerializeField] bool restarted;
    [Space]
    [SerializeField] AudioScript audioPlay;
    [SerializeField] Utility utilityScript;
    [SerializeField] Transform imageTarget;
    [SerializeField] ButtonAudio audioButton;

    public Camera mainCam;

    [Header("Object References")]
    [SerializeField] MeshRenderer[] splitCurve;
    [SerializeField] Transform wholeBreakpointCurve;
    [Space]
    [SerializeField] Transform waterSample;
    [SerializeField] CapsuleCollider wSCollider;
    [SerializeField] GameObject wSFlash;
    [SerializeField] GameObject agitatorIndicator;
    [SerializeField] Transform agitatorPoint;

    [Space]
    [Header("Clipboard References")]

    [Space]
    [Header("Material Refs")]
    [SerializeField] Material[] wSMats;

    [Space]
    [Header("Pause Menu")]
    [SerializeField] GameObject playButton;
    [SerializeField] GameObject pauseButton;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject restartButton;

    [Space]
    [Header("Other Sounds")]

    [Space]
    [Header("Misc Values")]
    [SerializeField] float dragSpeedMult;
    [SerializeField] float maxDistFromDragDest;
    [SerializeField] float sampleToAgitatorDist;

    [Space]
    [Header("Animation Times")]
    [SerializeField] float waterSampleAttachTime;
    [SerializeField] float curveDisplayTime;

    [Space]
    [Header("Animation References")]
    [SerializeField] Transform waterSampleAttachPoint;

    /*[Space]
    [Header("Start Positions")]*/

    // Start is called before the first frame update
    void Start()
    {
        GlobalFunctions.SetMainCam();

        Input.simulateMouseWithTouches = true;
        Time.timeScale = 1;

        //Set up the event handler for tracking from Vuforia
        mTrackableBehaviour = GameObject.Find("ImageTarget").GetComponent<TrackableBehaviour>();

        if (mTrackableBehaviour)
            mTrackableBehaviour.RegisterTrackableEventHandler(this);


        mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (waitTime > 0)
        {
            waitTime -= Time.deltaTime;
            if (waitTime < 0)
                waitTime = 0;
        }
        else
        {
            switch (step)
            {
                case 0:
                    CurveDisplay1();
                    break;

                case 1:
                    CurveDisplay2();
                    break;

                case 2:
                    CurveDisplay3();
                    break;

                case 3:
                    CurveDisplay4();
                    break;

                case 4:

                    break;

                case 5:

                    break;

                case 6:

                    break;

                case 7:

                    break;

                case 8:

                    break;

                case 9:

                    break;

                case 10:

                    break;

                case 11:

                    break;

                case 12:

                    break;

                case 13:

                    break;

                case 14:
                    End();
                    break;
            }
        }

        GlobalFunctions.UpdatePrevMousePos();
    }


    void CurveDisplay1()
    {
        //highlights whole curve, also starts anim coroutine
        StartCoroutine(DisplayCurve());
        splitCurve[0].enabled = true;
        splitCurve[1].enabled = true;
        splitCurve[2].enabled = true;
        waitTime += 5 + curveDisplayTime;
        step++;
    }

    void CurveDisplay2()
    {
        //activates first part of curve, highlight
        splitCurve[0].enabled = true;
        splitCurve[1].enabled = false;
        splitCurve[2].enabled = false;
        waitTime += 5;
        step++;
    }

    void CurveDisplay3()
    {
        //activates 3rd part of curve, highlight
        splitCurve[0].enabled = false;
        splitCurve[1].enabled = false;
        splitCurve[2].enabled = true;
        waitTime += 5;
        step++;
    }

    void CurveDisplay4()
    {
        //shuts off whole curve
        StartCoroutine(HideCurve());
        splitCurve[0].enabled = false;
        splitCurve[1].enabled = false;
        splitCurve[2].enabled = false;
        waitTime += 5 + curveDisplayTime;
        step++;
    }


    void MoveSample()
    {
        wSCollider.enabled = false;
        if (GlobalFunctions.SlideObjectHorizontal(waterSample, agitatorPoint, imageTarget, dragSpeedMult, maxDistFromDragDest, sampleToAgitatorDist))
        {
            //Disables indicator effect
            agitatorIndicator.gameObject.SetActive(false);
            wSCollider.enabled = true;


            //Logs that the sludge judge can be swiped down to start the animation
            Debug.Log("Sample Attach Point");
            

            GlobalFunctions.swipeDirection = Vector2.zero;

            step++;
        }
    }

    void SwipeSampleUp()
    {
        //Calls DetectTouch with a swipe distance of 15 pixels in each direction (15 left and right, 15 up and down)
        RaycastHit hit = GlobalFunctions.DetectTouch(this, new Vector2(1000, 15));

        if (hit.transform != null)
        {
            if (hit.transform == waterSample)
            {
                StartCoroutine(GlobalFunctions.ColorFlash(wSMats, Color.black, 0.1f, 0.25f));
            }
        }

        //Checks to see if the user swipes downwards
        if (GlobalFunctions.swipeDirection == Vector2.up)
        {
            Debug.Log("Water sample swiped");
            //Starts the coroutine for the sludge judge dip animation
            StartCoroutine("WaterSampleAttach");
            WaterSampleFlash(false);

            /*if (!restarted)
            {
                waitTime += audioPlay.Tracks[3].length + 6f;
            }*/

            step++;
        }
    }

    void End()
    {
        pauseButton.SetActive(false);
        pauseMenu.SetActive(true);
        restartButton.SetActive(true);
        playButton.SetActive(false);

        step++;
    }

    public void Restart()
    {
        restarts++;
        if (restarts >= 11)
        {
            restarts = 0;
        }
        restarted = true;
        

        restartButton.gameObject.SetActive(false);
        

        pauseButton.SetActive(true);
        pauseMenu.SetActive(false);
        restartButton.SetActive(false);
        playButton.SetActive(true);
        
        step = 0;
    }


    void WaterSampleFlash(bool on)
    {
        wSFlash.SetActive(on);
    }




    IEnumerator DisplayCurve()
    {
        float startTime = Time.time; //sets start time to seconds in program
        float currentTime;

        while (true)
        {
            currentTime = Time.time - startTime; //sets current time to current time in program

            wholeBreakpointCurve.localScale = Vector3.Lerp(new Vector3(0, 0, 0), new Vector3(20, 20, 20), currentTime / curveDisplayTime);
            //scales object up to 100, the end bit here divides the current time by the animation time. Once they both match, anim over

            if (currentTime > curveDisplayTime)
                //this breaks off the coroutine once its done
                break;

            yield return null;
        }
    }

    IEnumerator HideCurve()
    {
        float startTime = Time.time; //sets start time to seconds in program
        float currentTime;

        while (true)
        {
            currentTime = Time.time - startTime; //sets current time to current time in program

            wholeBreakpointCurve.localScale = Vector3.Lerp(new Vector3(20, 20, 20), new Vector3(0, 0, 0), currentTime / curveDisplayTime);
            //scales object up to 100, the end bit here divides the current time by the animation time. Once they both match, anim over

            if (currentTime > curveDisplayTime)
                //this breaks off the coroutine once its done
                break;

            yield return null;
        }
    }


    IEnumerator WaterSampleAttach()
    {
        /*if (!restarted)
            audioPlay.PlayAudio(); Debug.Log("AUDIO CLIP 4");*/

        //Stores the time the animation started
        float timeStart = Time.time;
        float currentTime;

        Vector3 sampleStartPos = waterSample.position;

        while (true)
        {
            //Sets the time currently for animation lerping
            currentTime = Time.time - timeStart;
            //Lerps (Linear Inerperlates) between the sludge judge start point, to the point it dips into the water
            //As Lerp works between 0 and 1, diving current time by the animation time will give 1 when current time has reached the animation duration
            waterSample.position = Vector3.Lerp(sampleStartPos, waterSampleAttachPoint.position, currentTime / waterSampleAttachTime);

            //Breaks out of the while when the current time has reached the animation time
            if (currentTime >= waterSampleAttachTime)
            {
                break;
            }

            //Pauses the Coroutine for the rest of the frame, resuming next frame
            //This stops the while loop from locking up the program, though it only works in a coroutine
            yield return null;
        }
    }



    //Will begin scenario once the tracking of the object begins. This is a Vuforia-triggered function
    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        if ((newStatus == TrackableBehaviour.Status.TRACKED || newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED) && previousStatus == TrackableBehaviour.Status.NO_POSE && scenarioHasStarted == false)
        {
            Debug.Log("Starting story");
            scenarioHasStarted = true;

            if (step < 0)
                step = 0;
        }
    }
}
