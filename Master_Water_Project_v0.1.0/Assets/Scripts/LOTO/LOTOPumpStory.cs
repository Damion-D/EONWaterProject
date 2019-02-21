using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using UnityEngine.UI;

public class LOTOPumpStory : MonoBehaviour, ITrackableEventHandler {

    public AudioManager audioManager;
    public Utility utility;
    public GameObject pauseMenu;
    public GameObject pauseButton;

    private TrackableBehaviour mTrackableBehaviour;
    private bool storyHasStarted = false;

    private bool isLerping = true;
    private float timeStartedLerping;
    private bool notStartedLerpingYet = true;
    public float timeTakenDuringLerp = 1f;
    private float timeSinceStarted;
    private float percentageComplete;
    private Vector3 SwitchStartPosition;
    private Vector3 SwitchEndPosition;
    private Quaternion SwitchStartRotation;
    private Quaternion SwitchEndRotation;
    private Vector3 OpeningHaspStartPosition;
    private Vector3 OpeningHaspEndPosition;
    private Quaternion OpeningHaspStartRotation;
    private Quaternion OpeningHaspEndRotation;
    private Vector3 OpenedHaspStartPosition;
    private Vector3 OpenedHaspEndPosition;
    private Quaternion OpenedHaspStartRotation;
    private Quaternion OpenedHaspEndRotation;
    private Vector3 HaspBackStartPosition;
    private Vector3 HaspBackEndPosition;
    private Quaternion HaspBackStartRotation;
    private Quaternion HaspBackEndRotation;
    private Vector3 FinalClosingHaspStartPosition;
    private Vector3 FinalClosingHaspEndPosition;
    private Quaternion FinalClosingHaspStartRotation;
    private Quaternion FinalClosingHaspEndRotation;

    private Vector3 LockClaspStartStartPosition;
    private Vector3 LockClaspStartEndPosition;
    private Vector3 LockStartPosition;
    private Vector3 LockEndPosition;
    private Vector3 LockClaspOpenStartPosition;
    private Vector3 LockClaspOpenEndPosition;
    private Vector3 LockClaspClosingStartPosition;
    private Vector3 LockClaspClosingEndPosition;

    private Vector3 TagStartPosition;
    private Vector3 TagEndPosition;
    
    

    private bool openHaspRoutineFinished = false;
    private bool movedHaspRoutineFinished = false;
    private bool haspSequenceFinished = false;
    private bool openLockSequenceFinished = false;
    private bool moveLockSequenceFinished = false;
    private bool closeLockSequenceFinished = false;
    private bool tagRoutineFinished = false;

    public string hitName;
    public GameObject Switch;
    public GameObject EndSwitch;
    public GameObject HaspFrontClosed;
    public GameObject HaspFrontOpen;
    public GameObject HaspBack;
    public GameObject EndHaspPart2;
    public GameObject HaspBackEnd;
    public GameObject HaspFrontOpenEnd;
    public GameObject HaspFrontClosedEnd;

    public GameObject LockStart;
    public GameObject LockClaspStart;
    public GameObject LockClaspOpenStart;
    public GameObject LockEnd;
    public GameObject LockClaspOpenEnd;
    public GameObject LockClaspClosedEnd;

    public GameObject TagStart;
    public GameObject TagEnd;

    public GameObject wholeHasp;
    public GameObject wholePadlock;
    public GameObject wholeTag;
	// Use this for initialization
	void Start () {
        //Set up the event handler for tracking from Vuforia
        mTrackableBehaviour = GameObject.Find("ImageTarget").GetComponent<TrackableBehaviour>();

        if (mTrackableBehaviour)
            mTrackableBehaviour.RegisterTrackableEventHandler(this);

        wholeHasp.SetActive(false);
        wholePadlock.SetActive(false);
        wholeTag.SetActive(false);
        SwitchStartPosition = Switch.gameObject.transform.position;
        SwitchEndPosition = EndSwitch.gameObject.transform.position;
        SwitchStartRotation = Switch.gameObject.transform.rotation;
        SwitchEndRotation = EndSwitch.gameObject.transform.rotation;

        OpeningHaspStartPosition = HaspFrontClosed.gameObject.transform.position;
        OpeningHaspStartRotation = HaspFrontClosed.gameObject.transform.rotation;

        OpeningHaspEndPosition = HaspFrontOpen.gameObject.transform.position;
        OpeningHaspEndRotation = HaspFrontOpen.gameObject.transform.rotation;

        OpenedHaspStartPosition = OpeningHaspEndPosition;
        OpenedHaspStartRotation = OpeningHaspEndRotation;
        OpenedHaspEndPosition = HaspFrontOpenEnd.gameObject.transform.position;
        OpenedHaspEndRotation = HaspFrontOpenEnd.gameObject.transform.rotation;

        HaspBackStartPosition = HaspBack.gameObject.transform.position;
        HaspBackStartRotation = HaspBack.gameObject.transform.rotation;
        HaspBackEndPosition = HaspBackEnd.gameObject.transform.position;
        HaspBackEndRotation = HaspBackEnd.gameObject.transform.rotation;

        FinalClosingHaspStartPosition = HaspFrontOpenEnd.gameObject.transform.position;
        FinalClosingHaspEndPosition = HaspFrontClosedEnd.gameObject.transform.position;
        FinalClosingHaspStartRotation = HaspFrontOpenEnd.gameObject.transform.rotation;
        FinalClosingHaspEndRotation = HaspFrontClosedEnd.gameObject.transform.rotation;

        LockClaspStartStartPosition = LockClaspStart.gameObject.transform.position;
        LockClaspStartEndPosition = LockClaspOpenStart.gameObject.transform.position;
        LockStartPosition = LockStart.gameObject.transform.position;
        LockEndPosition = LockEnd.gameObject.transform.position;
        LockClaspOpenStartPosition = LockClaspOpenStart.gameObject.transform.position;
        LockClaspOpenEndPosition = LockClaspOpenEnd.gameObject.transform.position;
        LockClaspClosingEndPosition = LockClaspClosedEnd.gameObject.transform.position;

        TagStartPosition = TagStart.gameObject.transform.position;
        TagEndPosition = TagEnd.gameObject.transform.position;
}
	
	// Update is called once per frame
	void Update () {

        

    }

    private IEnumerator LOTOPumpNarrative()
    {
        
        audioManager.PlaySound("Introduction", 1.0f, true, 2);
        yield return null;

        while (true)
        {
            if (hitName == "BreakerBox")
            {
                StartCoroutine("FlipSwitch");
                Debug.Log("Selected BreakerBox!");
                break;
            }
            yield return null;
        }

        audioManager.PlaySound("HaspExplanation", 1.0f, true, 2);
        wholeHasp.SetActive(true);
        yield return null;

        while (true)
        {
            if (hitName == "Hasp_Front"||hitName=="Hasp_Back")
            {
                StartCoroutine("OpenHasp");
                break;
                
            }
            yield return null;
        }

        while (true)
        {
            if (openHaspRoutineFinished)
            {
                StartCoroutine("MoveHaspToEnd");
                break;
            }
            yield return null;
        }

        while (true)
        {
            if (movedHaspRoutineFinished)
            {
                StartCoroutine("CloseHaspEnd");
                wholePadlock.SetActive(true);
                break;
            }
            yield return null;
        }

        while (true)
        {
            if (haspSequenceFinished)
            {
                if (hitName == "Lock")
                {
                    StartCoroutine("OpenLock");
                    break;
                }

            }
            yield return null;
        }

        while (true)
        {
            if (openLockSequenceFinished)
            {
                wholeTag.SetActive(true);
                if (hitName == "Lockout_Tag_Start")
                {
                    StartCoroutine("PlaceTag");
                    break;
                }
            }
            yield return null;
        }

        while (true)
        {
            if (tagRoutineFinished)
            {
                
                StartCoroutine("MoveLock");
                break;
                

            }
            yield return null;
        }

        while (true)
        {
            if (moveLockSequenceFinished)
            {
                
                StartCoroutine("CloseLock");
                break;
                

            }
            yield return null;
        }

        while (true)
        {
            if (closeLockSequenceFinished)
            {
                audioManager.PlaySound("Outro", 1.0f, true, 2);
                break;
            }
            yield return null;
        }

        while (true)
        {
            if (audioManager.GetSound("Outro").hasCompleted)
            {
                yield return new WaitForSeconds(2);
                pauseMenu.SetActive(true);
                pauseButton.SetActive(false);
                utility.PauseAll();
                break;
            }
            yield return null;
        }


    }

    private IEnumerator FlipSwitch()
    {
        Debug.Log("Started FlipSwitch coroutine!");
        while (true)
        {
            if (notStartedLerpingYet)
            {
                
                isLerping = true;
                timeStartedLerping = Time.time;
                notStartedLerpingYet = false;
            }

            if (isLerping)
            {
                timeSinceStarted = Time.time - timeStartedLerping;
                percentageComplete = timeSinceStarted / timeTakenDuringLerp;
                Switch.gameObject.transform.position = Vector3.Lerp(SwitchStartPosition, SwitchEndPosition, percentageComplete);
                Switch.gameObject.transform.rotation = Quaternion.Lerp(SwitchStartRotation, SwitchEndRotation, percentageComplete);
                if (percentageComplete >= 1.0f)
                {
                    isLerping = false;
                    notStartedLerpingYet = true;
                    
                    //ReplacementMotor.gameObject.transform.position = motorEndPosition;
                    
                    /*MotorIndicator.SetActive(true);
                    MotorLabel.SetActive(true);
                    MotorLabelCapsule.SetActive(true);*/
                    //Motor.SetActive(true);
                    break;
                }
            }

            yield return null;
        }
    }

    private IEnumerator OpenHasp()
    {
        while (true)
        {
            if (notStartedLerpingYet)
            {

                isLerping = true;
                timeStartedLerping = Time.time;
                notStartedLerpingYet = false;
            }

            if (isLerping)
            {
                timeSinceStarted = Time.time - timeStartedLerping;
                percentageComplete = timeSinceStarted / timeTakenDuringLerp;
                HaspFrontClosed.gameObject.transform.position = Vector3.Lerp(OpeningHaspStartPosition, OpeningHaspEndPosition, percentageComplete);
                HaspFrontClosed.gameObject.transform.rotation = Quaternion.Lerp(OpeningHaspStartRotation, OpeningHaspEndRotation, percentageComplete);
                if (percentageComplete >= 1.0f)
                {
                    isLerping = false;
                    notStartedLerpingYet = true;
                    openHaspRoutineFinished = true;
                    //ReplacementMotor.gameObject.transform.position = motorEndPosition;

                    /*MotorIndicator.SetActive(true);
                    MotorLabel.SetActive(true);
                    MotorLabelCapsule.SetActive(true);*/
                    //Motor.SetActive(true);
                    break;
                }
            }
            yield return null;
        }
    }

    private IEnumerator MoveHaspToEnd()
    {
        Debug.Log("Started MoveHaspToEnd routine!");
        while (true)
        {
            if (notStartedLerpingYet)
            {

                isLerping = true;
                timeStartedLerping = Time.time;
                notStartedLerpingYet = false;
            }

            if (isLerping)
            {
                timeSinceStarted = Time.time - timeStartedLerping;
                percentageComplete = timeSinceStarted / timeTakenDuringLerp;
                HaspFrontClosed.gameObject.transform.position = Vector3.Lerp(OpenedHaspStartPosition, OpenedHaspEndPosition, percentageComplete);
                HaspFrontClosed.gameObject.transform.rotation = Quaternion.Lerp(OpenedHaspStartRotation, OpenedHaspEndRotation, percentageComplete);
                HaspBack.gameObject.transform.position = Vector3.Lerp(HaspBackStartPosition, HaspBackEndPosition, percentageComplete);
                HaspBack.gameObject.transform.rotation = Quaternion.Lerp(HaspBackStartRotation, HaspBackEndRotation, percentageComplete);
                if (percentageComplete >= 1.0f)
                {
                    isLerping = false;
                    notStartedLerpingYet = true;
                    movedHaspRoutineFinished = true;
                    //ReplacementMotor.gameObject.transform.position = motorEndPosition;

                    /*MotorIndicator.SetActive(true);
                    MotorLabel.SetActive(true);
                    MotorLabelCapsule.SetActive(true);*/
                    //Motor.SetActive(true);
                    break;
                }
            }
            yield return null;
        }
    }

    private IEnumerator CloseHaspEnd()
    {
        audioManager.PlaySound("HaspOnSwitchExplanation", 1.0f, true, 2);
        Debug.Log("Started CloseHaspEnd routine!");
        while (true)
        {
            if (notStartedLerpingYet)
            {

                isLerping = true;
                timeStartedLerping = Time.time;
                notStartedLerpingYet = false;
            }

            if (isLerping)
            {
                timeSinceStarted = Time.time - timeStartedLerping;
                percentageComplete = timeSinceStarted / timeTakenDuringLerp;
                HaspFrontClosed.gameObject.transform.position = Vector3.Lerp(FinalClosingHaspStartPosition, FinalClosingHaspEndPosition, percentageComplete);
                HaspFrontClosed.gameObject.transform.rotation = Quaternion.Lerp(FinalClosingHaspStartRotation, FinalClosingHaspEndRotation, percentageComplete);
                //HaspBack.gameObject.transform.position = Vector3.Lerp(HaspBackStartPosition, HaspBackEndPosition, percentageComplete);
                //HaspBack.gameObject.transform.rotation = Quaternion.Lerp(HaspBackStartRotation, HaspBackEndRotation, percentageComplete);
                if (percentageComplete >= 1.0f)
                {
                    isLerping = false;
                    notStartedLerpingYet = true;
                    haspSequenceFinished = true;
                    //movedHaspRoutineFinished = true;
                    //ReplacementMotor.gameObject.transform.position = motorEndPosition;

                    /*MotorIndicator.SetActive(true);
                    MotorLabel.SetActive(true);
                    MotorLabelCapsule.SetActive(true);*/
                    //Motor.SetActive(true);
                    break;
                }
            }
            yield return null;
        }
    }

    /*LockClaspStartStartPosition = LockClaspStart.gameObject.transform.position;
        LockClaspStartEndPosition = LockClaspOpenStart.gameObject.transform.position;
        LockStartPosition = LockStart.gameObject.transform.position;
        LockEndPosition = LockEnd.gameObject.transform.position;
        LockClaspOpenEndPosition = LockClaspOpenEnd.gameObject.transform.position;
        LockClaspClosedEndPosition = LockClaspClosedEnd.gameObject.transform.position;*/

    private IEnumerator OpenLock()
    {
        Debug.Log("Started open lock routine!");
        audioManager.PlaySound("LockExplanation", 1.0f, true, 2);
        while (true)
        {
            if (notStartedLerpingYet)
            {

                isLerping = true;
                timeStartedLerping = Time.time;
                notStartedLerpingYet = false;
            }

            if (isLerping)
            {
                timeSinceStarted = Time.time - timeStartedLerping;
                percentageComplete = timeSinceStarted / timeTakenDuringLerp;
                LockClaspStart.gameObject.transform.position = Vector3.Lerp(LockClaspStartStartPosition, LockClaspStartEndPosition, percentageComplete);
                
               
                if (percentageComplete >= 1.0f)
                {
                    isLerping = false;
                    notStartedLerpingYet = true;
                    openLockSequenceFinished = true;
                    
                    break;
                }
            }
            yield return null;
        }
    }

    private IEnumerator PlaceTag()
    {
        Debug.Log("Started place tag routine!");
        audioManager.PlaySound("TagExplanation", 1.0f, true, 2);
        while (true)
        {
            if (notStartedLerpingYet)
            {

                isLerping = true;
                timeStartedLerping = Time.time;
                notStartedLerpingYet = false;
                
            }

            if (isLerping)
            {
                timeSinceStarted = Time.time - timeStartedLerping;
                percentageComplete = timeSinceStarted / timeTakenDuringLerp;
                TagStart.gameObject.transform.position = Vector3.Lerp(TagStartPosition, TagEndPosition, percentageComplete);
                


                if (percentageComplete >= 1.0f)
                {
                    isLerping = false;
                    notStartedLerpingYet = true;
                    tagRoutineFinished = true;
                    TagStart.transform.parent = LockStart.transform;

                    break;
                }
            }
            yield return null;
        }
    }

    private IEnumerator MoveLock()
    {
        Debug.Log("Started move lock routine!");
        while (true)
        {
            if (notStartedLerpingYet)
            {

                isLerping = true;
                timeStartedLerping = Time.time;
                notStartedLerpingYet = false;
            }

            if (isLerping)
            {
                timeSinceStarted = Time.time - timeStartedLerping;
                percentageComplete = timeSinceStarted / timeTakenDuringLerp;
                LockStart.gameObject.transform.position = Vector3.Lerp(LockStartPosition, LockEndPosition, percentageComplete);
                LockClaspStart.gameObject.transform.position = Vector3.Lerp(LockClaspOpenStartPosition, LockClaspOpenEndPosition, percentageComplete);


                if (percentageComplete >= 1.0f)
                {
                    isLerping = false;
                    notStartedLerpingYet = true;
                    moveLockSequenceFinished = true;

                    break;
                }
            }
            yield return null;
        }
    }

    private IEnumerator CloseLock()
    {
        Debug.Log("Started Close lock routine!");
        while (true)
        {
            if (notStartedLerpingYet)
            {

                isLerping = true;
                timeStartedLerping = Time.time;
                notStartedLerpingYet = false;
            }

            if (isLerping)
            {
                timeSinceStarted = Time.time - timeStartedLerping;
                percentageComplete = timeSinceStarted / timeTakenDuringLerp;
                LockClaspStart.gameObject.transform.position = Vector3.Lerp(LockClaspOpenEndPosition, LockClaspClosingEndPosition, percentageComplete);


                if (percentageComplete >= 1.0f)
                {
                    isLerping = false;
                    notStartedLerpingYet = true;
                    closeLockSequenceFinished = true;

                    break;
                }
            }
            yield return null;
        }

    }

    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        if ((newStatus == TrackableBehaviour.Status.TRACKED || newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED) && previousStatus == TrackableBehaviour.Status.NO_POSE && storyHasStarted == false)
        {
            storyHasStarted = true;
            StartCoroutine("LOTOPumpNarrative");
        }
    }
}
