using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Vuforia;

public class pHAndDOScenario : MonoBehaviour, ITrackableEventHandler
{
    //Water Sample is often shortened to wS

    //Variables to begin the scenario
    TrackableBehaviour mTrackableBehaviour;
    bool scenarioHasStarted;
    float clipboardInput = 0;

    [SerializeField] float waitTime;
    [SerializeField] float inStepWaitTime;
    [SerializeField] int step = -1;
    [Space]
    //If restarted, skips dialogue
    [SerializeField] int restarts;
    [SerializeField] bool restarted;
    [Space]
    [SerializeField] AudioScript audioPlay;
    [SerializeField] Utility utilityScript;
    [SerializeField] Transform imageTarget;

    public Camera mainCam;

    [Header("Object References")]
    [SerializeField] Transform clipboardKeyboard;
    [SerializeField] Transform clipboard;

    [Space]
    [Header("Clipboard References")]

    [Space]
    [Header("Material Refs")]

    [Space]
    [Header("Pause Menu")]
    [SerializeField] GameObject playButton;
    [SerializeField] GameObject pauseButton;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject restartButton;

    /*[Space]
    [Header("Other Sounds")]

    [Space]
    [Header("Misc Values")]


    [Space]
    [Header("Animation Times")]

    [Space]
    [Header("Animation References")]

    [Space]
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

        Intro();
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

                    break;

                case 1:

                    break;

                case 2:

                    break;

                case 3:

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
                    End();
                    break;
            }
        }

        GlobalFunctions.UpdatePrevMousePos();
    }


    void Intro()
    {
        clipboard.localScale = Vector3.one * 0.0001f;

        clipboard.gameObject.SetActive(false);
        clipboardKeyboard.gameObject.SetActive(false);
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

        clipboard.transform.localScale = Vector3.one * 0.001f;
        clipboardKeyboard.gameObject.SetActive(false);

        pauseButton.SetActive(true);
        pauseMenu.SetActive(false);
        restartButton.SetActive(false);
        playButton.SetActive(true);

        step = 0;
    }
    

    //gets date & time for clipboard
    private static DateTime GetNow()
    {
        return DateTime.Now;
    }

    private static DateTime realTime()
    {
        return DateTime.Now;

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
