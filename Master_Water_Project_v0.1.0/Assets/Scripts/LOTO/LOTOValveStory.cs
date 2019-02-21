using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using UnityEngine.UI;

public class LOTOValveStory : MonoBehaviour, ITrackableEventHandler {

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

    private bool openValveRoutineFinished = false;
    private bool shrinkValveRoutineFinished = false;
    private bool closeShrunkValveRoutineFinished = false;
    private bool openedPadlockRoutineFinished = false;
    private bool movePadlockRoutineFinished = false;
    private bool closedPadlockRoutineFinished = false;
    private bool openHaspRoutineFinished = false;
    private bool moveHaspRoutineFinished = false;
    private bool closeHaspRoutineFinished = false;
    private bool moveTagRoutineFinished = false;

    public GameObject Hasp;
    public GameObject Lock;
    public GameObject Tag;
    public GameObject ValveLock;
    public GameObject BottomValveLock;
    public GameObject ShrunkBottomValveClosed;
    public GameObject BottomValveLockOpenEnd;

    //movement variables for hasp
    public GameObject HaspOriginal;
    public GameObject HaspFrontClosedStart;
    public GameObject HaspFrontOpenStart;
    public GameObject HaspFrontOpenEnd;
    public GameObject HaspOpenEnd;
    public GameObject HaspFrontClosedEnd;

    //movement variables for tag
    public GameObject TagStart;
    public GameObject TagEnd;

    public GameObject PadlockHookClosedStart;
    public GameObject PadlockHookOpenStart;
    public GameObject PadlockOpenStart;
    public GameObject PadlockOpenEnd;
    public GameObject PadlockBottomOpenEnd;
    public GameObject PadlockBottomClosedEnd;
    public GameObject PadlockBottom;

    public GameObject LockOpenStart;
    public GameObject LockOpenEnd;

    public Vector3 LockClosedStartPosition;
    public Quaternion LockClosedStartRotation;
    public Vector3 LockClosedEndPosition;
    public Quaternion LockClosedEndRotation;
    public Vector3 LockOpenedStartPosition;
    public Quaternion LockOpenedStartRotation;
    public Vector3 LockOpenedStartScale;
    public Vector3 LockOpenedEndPosition;
    public Quaternion LockOpenedEndRotation;
    public Vector3 LockOpenedEndScale;

    public Vector3 ClosingShrunkValveStartPosition;
    public Vector3 ClosingShrunkValveEndPosition;
    public Quaternion ClosingShrunkValveStartRotation;
    public Quaternion ClosingShrunkValveEndRotation;

    public Vector3 OpeningPadlockStartPosition;
    public Vector3 OpeningPadlockEndPosition;
    public Vector3 MovingPadlockStartPosition;
    public Vector3 MovingPadlockEndPosition;
    public Quaternion MovingPadlockStartRotation;
    public Quaternion MovingPadlockEndRotation;
    public Vector3 ClosingPadlockStartPosition;
    public Vector3 ClosingPadlockEndPosition;

    //lerp variables for Hasp
    public Vector3 OpeningHaspStartPosition;
    public Quaternion OpeningHaspStartRotation;
    public Vector3 OpeningHaspEndPosition;
    public Quaternion OpeningHaspEndRotation;

    public Vector3 MoveHaspStartPosition;
    public Quaternion MoveHaspStartRotation;
    public Vector3 MoveHaspEndPosition;
    public Quaternion MoveHaspEndRotation;

    public Vector3 ClosingHaspStartPosition;
    public Quaternion ClosingHaspStartRotation;
    public Vector3 ClosingHaspEndPosition;
    public Quaternion ClosingHaspEndRotation;

    public Vector3 MovingTagStartPosition;
    public Quaternion MovingTagStartRotation;
    public Vector3 MovingTagEndPosition;
    public Quaternion MovingTagEndRotation;
    public Vector3 MovingTagStartScale;
    public Vector3 MovingTagEndScale;
    public string hitName;



    // Use this for initialization
    void Start () {

        Hasp.SetActive(false);
        Lock.SetActive(false);
        ValveLock.SetActive(false);
        Tag.SetActive(false);
        // Set up the event handler for tracking from Vuforia
         mTrackableBehaviour = GameObject.Find("ImageTarget").GetComponent<TrackableBehaviour>();

        LockClosedStartPosition = BottomValveLock.gameObject.transform.position;
        LockClosedStartRotation = BottomValveLock.gameObject.transform.rotation;
        LockClosedEndPosition = LockOpenStart.gameObject.transform.position;
        LockClosedEndRotation = LockOpenStart.gameObject.transform.rotation;

        LockOpenedStartPosition = ValveLock.gameObject.transform.position;
        LockOpenedStartRotation = ValveLock.gameObject.transform.rotation;
        LockOpenedStartScale = ValveLock.gameObject.transform.localScale;
        LockOpenedEndScale = LockOpenEnd.gameObject.transform.localScale;
        LockOpenedEndPosition = LockOpenEnd.gameObject.transform.position;
        LockOpenedEndRotation = LockOpenEnd.gameObject.transform.rotation;

        ClosingShrunkValveStartPosition = BottomValveLockOpenEnd.gameObject.transform.position;
        ClosingShrunkValveStartRotation = BottomValveLockOpenEnd.gameObject.transform.rotation;
        ClosingShrunkValveEndPosition = ShrunkBottomValveClosed.gameObject.transform.position;
        ClosingShrunkValveEndRotation = ShrunkBottomValveClosed.gameObject.transform.rotation;

        OpeningPadlockStartPosition = PadlockHookClosedStart.gameObject.transform.position;
        OpeningPadlockEndPosition = PadlockHookOpenStart.gameObject.transform.position;
        MovingPadlockStartPosition = PadlockOpenStart.gameObject.transform.position;
        MovingPadlockStartRotation = PadlockOpenStart.gameObject.transform.rotation;
        MovingPadlockEndPosition = PadlockOpenEnd.gameObject.transform.position;
        MovingPadlockEndRotation = PadlockOpenEnd.gameObject.transform.rotation;
        ClosingPadlockStartPosition = PadlockBottomOpenEnd.gameObject.transform.position;
        ClosingPadlockEndPosition = PadlockBottomClosedEnd.gameObject.transform.position;

        OpeningHaspStartPosition = HaspFrontClosedStart.gameObject.transform.position;
        OpeningHaspEndPosition = HaspFrontOpenStart.gameObject.transform.position;
        OpeningHaspStartRotation = HaspFrontClosedStart.gameObject.transform.rotation;
        OpeningHaspEndRotation = HaspFrontOpenStart.gameObject.transform.rotation;

        MoveHaspStartPosition = HaspOriginal.gameObject.transform.position;
        MoveHaspEndPosition = HaspOpenEnd.gameObject.transform.position;
        MoveHaspStartRotation = HaspOriginal.gameObject.transform.rotation;
        MoveHaspEndRotation = HaspOpenEnd.gameObject.transform.rotation;

        ClosingHaspStartPosition = HaspFrontOpenEnd.gameObject.transform.position;
        ClosingHaspEndPosition = HaspFrontClosedEnd.gameObject.transform.position;
        ClosingHaspStartRotation = HaspFrontOpenEnd.gameObject.transform.rotation;
        ClosingHaspEndRotation = HaspFrontClosedEnd.gameObject.transform.rotation;

        MovingTagStartPosition = TagStart.gameObject.transform.position;
        MovingTagEndPosition = TagEnd.gameObject.transform.position;
        MovingTagStartRotation = TagStart.gameObject.transform.rotation;
        MovingTagEndRotation = TagEnd.gameObject.transform.rotation;
        MovingTagStartScale = TagStart.gameObject.transform.localScale;
        MovingTagEndScale = TagEnd.gameObject.transform.localScale;
        

        StartCoroutine("LOTOValveNarrative");

        /*Hasp.SetActive(false);
        Lock.SetActive(false);
        Tag.SetActive(false);*/
    }

    // Update is called once per frame
    void Update () {
		
	}

    private IEnumerator LOTOValveNarrative()
    {
        Debug.Log("Started LOTOValveNarrative!");
        audioManager.PlaySound("Introduction", 1.0f, true, 2);
        ValveLock.SetActive(true);
        yield return null;

        while (true)
        {
            if (hitName == "Valve_Lockout")
            {
                
                StartCoroutine("OpenValve");
                break;
            }
            yield return null;
        }
        while (true)
        {
            if (openValveRoutineFinished)
            {
                StartCoroutine("ValveShrink");
                break;
            }
            yield return null;
        }
        while (true)
        {
            if (shrinkValveRoutineFinished)
            {
                StartCoroutine("CloseShrunkValve");
                audioManager.PlaySound("ValveExplanation", 1.0f, true, 2);
                
                break;
            }
            yield return null;
        }

        while (true)
        {
            if (closeShrunkValveRoutineFinished&&audioManager.GetSound("ValveExplanation").hasCompleted)
            {
                Hasp.SetActive(true);
                if (hitName == "Hasp_Front")
                {
                    audioManager.PlaySound("ValveCoveredExplanation", 1.0f, true, 2);
                    
                    StartCoroutine("OpenHasp");
                    break;
                }
            }
            yield return null;
        }

        while (true)
        {
            if (openHaspRoutineFinished)
            {

                    StartCoroutine("MoveHasp");
                    break;
                
            }
            yield return null;
        }

        while (true)
        {
            if (moveHaspRoutineFinished)
            {
                StartCoroutine("CloseHasp");
                break;
            }
            yield return null;
        }

        while (true)
        {
            
            if (closeHaspRoutineFinished)
            {
                Lock.SetActive(true);
                if (hitName == "Lock")
                {
                    audioManager.PlaySound("LockTagExplanation", 1.0f, true, 2);
                    StartCoroutine("OpenPadlock");
                    break;
                }
                
            }
            yield return null;
        }
        
        
        while (true)
        {
            if (openedPadlockRoutineFinished)
            {
                StartCoroutine("MovePadlock");
                
                break;
            }
            yield return null;
        }

        while (true)
        {
            if (movePadlockRoutineFinished)
            {
                Tag.SetActive(true);
                StartCoroutine("MoveTag");

                break;
            }
            yield return null;
        }
        while (true)
        {
            if (moveTagRoutineFinished&&audioManager.GetSound("LockTagExplanation").hasCompleted)
            {
                StartCoroutine("ClosePadlock");
                audioManager.PlaySound("Outro");

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

    private IEnumerator OpenValve()
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
                BottomValveLock.gameObject.transform.position = Vector3.Lerp(LockClosedStartPosition, LockClosedEndPosition, percentageComplete);
                BottomValveLock.gameObject.transform.rotation = Quaternion.Lerp(LockClosedStartRotation, LockClosedEndRotation, percentageComplete);
                if (percentageComplete >= 1.0f)
                {
                    isLerping = false;
                    notStartedLerpingYet = true;
                    openValveRoutineFinished = true;
                    break;
                }
            }

            yield return null;
        }
    }

    private IEnumerator ValveShrink()
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
                ValveLock.gameObject.transform.position = Vector3.Lerp(LockOpenedStartPosition, LockOpenedEndPosition, percentageComplete);
                ValveLock.gameObject.transform.rotation = Quaternion.Lerp(LockOpenedStartRotation, LockOpenedEndRotation, percentageComplete);
                ValveLock.gameObject.transform.localScale = Vector3.Lerp(LockOpenedStartScale, LockOpenedEndScale, percentageComplete);
                if (percentageComplete >= 1.0f)
                {
                    isLerping = false;
                    notStartedLerpingYet = true;
                    shrinkValveRoutineFinished = true;
                    break;
                }
            }

            yield return null;
        }
    }

    private IEnumerator CloseShrunkValve()
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
                BottomValveLock.gameObject.transform.position = Vector3.Lerp(ClosingShrunkValveStartPosition, ClosingShrunkValveEndPosition, percentageComplete);
                BottomValveLock.gameObject.transform.rotation = Quaternion.Lerp(ClosingShrunkValveStartRotation, ClosingShrunkValveEndRotation, percentageComplete);
                if (percentageComplete >= 1.0f)
                {
                    isLerping = false;
                    notStartedLerpingYet = true;
                    closeShrunkValveRoutineFinished = true;
                    break;
                }
            }

            yield return null;
        }
    }

    private IEnumerator OpenPadlock()
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
                PadlockHookClosedStart.gameObject.transform.position = Vector3.Lerp(OpeningPadlockStartPosition, OpeningPadlockEndPosition, percentageComplete);
                
                if (percentageComplete >= 1.0f)
                {
                    isLerping = false;
                    notStartedLerpingYet = true;
                    openedPadlockRoutineFinished = true;
                    break;
                }
            }

            yield return null;
        }
    }

    private IEnumerator MovePadlock()
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
                PadlockOpenStart.gameObject.transform.position = Vector3.Lerp(MovingPadlockStartPosition, MovingPadlockEndPosition, percentageComplete);
                PadlockOpenStart.gameObject.transform.rotation = Quaternion.Lerp(MovingPadlockStartRotation, MovingPadlockEndRotation, percentageComplete);
                if (percentageComplete >= 1.0f)
                {
                    isLerping = false;
                    notStartedLerpingYet = true;
                    movePadlockRoutineFinished = true;
                    break;
                }
            }

            yield return null;
        }
    }

    private IEnumerator ClosePadlock()
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
                PadlockBottom.gameObject.transform.position = Vector3.Lerp(ClosingPadlockStartPosition, ClosingPadlockEndPosition, percentageComplete);

                if (percentageComplete >= 1.0f)
                {
                    isLerping = false;
                    notStartedLerpingYet = true;
                    closedPadlockRoutineFinished = true;
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
                HaspFrontClosedStart.gameObject.transform.position = Vector3.Lerp(OpeningHaspStartPosition, OpeningHaspEndPosition, percentageComplete);
                HaspFrontClosedStart.gameObject.transform.rotation = Quaternion.Lerp(OpeningHaspStartRotation, OpeningHaspEndRotation, percentageComplete);
                if (percentageComplete >= 1.0f)
                {
                    isLerping = false;
                    notStartedLerpingYet = true;
                    openHaspRoutineFinished = true;
                    break;
                }
            }

            yield return null;
        }
    }

    private IEnumerator MoveHasp()
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
                HaspOriginal.gameObject.transform.position = Vector3.Lerp(MoveHaspStartPosition, MoveHaspEndPosition, percentageComplete);
                HaspOriginal.gameObject.transform.rotation = Quaternion.Lerp(MoveHaspStartRotation, MoveHaspEndRotation, percentageComplete);
                if (percentageComplete >= 1.0f)
                {
                    isLerping = false;
                    notStartedLerpingYet = true;
                    moveHaspRoutineFinished = true;
                    break;
                }
            }

            yield return null;
        }
    }

    private IEnumerator CloseHasp()
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
                HaspFrontClosedStart.gameObject.transform.position = Vector3.Lerp(ClosingHaspStartPosition, ClosingHaspEndPosition, percentageComplete);
                HaspFrontClosedStart.gameObject.transform.rotation = Quaternion.Lerp(ClosingHaspStartRotation, ClosingHaspEndRotation, percentageComplete);
                if (percentageComplete >= 1.0f)
                {
                    isLerping = false;
                    notStartedLerpingYet = true;
                    closeHaspRoutineFinished = true;
                    break;
                }
            }

            yield return null;
        }
    }

    private IEnumerator MoveTag()
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
                TagStart.gameObject.transform.position = Vector3.Lerp(MovingTagStartPosition, MovingTagEndPosition, percentageComplete);
                TagStart.gameObject.transform.rotation = Quaternion.Lerp(MovingTagStartRotation, MovingTagEndRotation, percentageComplete);
                TagStart.gameObject.transform.localScale = Vector3.Lerp(MovingTagStartScale, MovingTagEndScale, percentageComplete);
                if (percentageComplete >= 1.0f)
                {
                    isLerping = false;
                    notStartedLerpingYet = true;
                    moveTagRoutineFinished = true;
                    break;
                }
            }

            yield return null;
        }
    }
    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        Debug.Log("OnTrackableStateChanged called!");
        if ((newStatus == TrackableBehaviour.Status.TRACKED || newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED) && previousStatus == TrackableBehaviour.Status.NO_POSE && storyHasStarted == false)
        {
            storyHasStarted = true;
            Debug.Log("Tracking is working!");
            //audioManager.PlaySound("Introduction", 1.0f, true, 2);
            //StartCoroutine("LOTOValveNarrative");
        }
    }
}
