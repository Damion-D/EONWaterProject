using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class SludgeJudgeScenario : MonoBehaviour, ITrackableEventHandler
{
    //Sludge Judge shortened to sJ often

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

    [SerializeField] AudioScript audioPlay;
    [SerializeField] Utility utilityScript;
    [SerializeField] Transform imageTarget;
    [SerializeField] ButtonAudio audioButton;

    [Header("Object References")]
    [SerializeField] Transform sludgeJudge;
    [SerializeField] Transform sludgeJudgeFlash;
    [SerializeField] Transform sludgeJudgeSludge;
    [SerializeField] Transform sludgeSampleFlash; 
    [SerializeField] Transform mainTank;
    [SerializeField] Transform insertionPoint;
    [SerializeField] Transform tankWaterTop;
    [SerializeField] Transform tankWaterTopFlash;
    [SerializeField] Transform indicator;
    [SerializeField] Transform dumpIndicator;
    [SerializeField] Transform dumpPoint;
    [SerializeField] Transform dumpPartSystem;

    [SerializeField] UnityEngine.UI.Image darkPlane;

    [Space]
    [Header("Clipboard References")]

    [SerializeField] Transform sludgeJudgeClipboard;
    [SerializeField] Transform[] TextMeshPro_date;
    [SerializeField] Transform[] TextMeshPro_time;
    [SerializeField] Transform[] TextMeshPro_reading;
    [SerializeField] Transform clipboardShrunkPoint;

    [SerializeField] Transform clipboardKeyboard;

    [Space]
    [Header("Material Refs")]
    [SerializeField] Material[] sJMats;

    [Space]
    [Header("Pause Menu")]
    [SerializeField] GameObject playButton;
    [SerializeField] GameObject pauseButton;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject restartButton;
    [Space]

    public Camera mainCam;

    [Space]


    [Header("Other Sounds")]
    [Space]
 

    [Header("Misc Values")]
    [SerializeField] float insertionDist;
    [SerializeField] float dumpDist;
    [SerializeField] Vector2 sludgeAmount;
    [SerializeField] float sludgeLevels;
    [SerializeField] float sJMoveSpeedMult;
    [SerializeField] float sJMaxDistFromPoint;

    [Header("Animation Times")]
    [SerializeField] float sJDipTime;
    [SerializeField] float sampleDialogeTime;
    [SerializeField] float sJSampleTime;
    [SerializeField] float sJExamineTransTime;
    [SerializeField] float sJReturnDelay;
    [SerializeField] float effectAppearDelay;
    [SerializeField] float sJDumpReturnDelay;

    //Different points for the animations
    [Header("Position Object References")]
    [SerializeField] Transform sJDipPoint;
    [SerializeField] Transform sJSampledPoint;
    [SerializeField] Transform sJExaminePoint;
    [SerializeField] Transform tankShrunkPoint;
    [SerializeField] Transform sJDumpPoint;
    [SerializeField] Transform sJShrunkPoint;

    [Header("Start Positions")]
    [SerializeField] Vector3 sJStartPoint;
    [SerializeField] Vector3 sJStartScale;
    [SerializeField] Quaternion sJStartRot;
    [Space]
    [SerializeField] Vector3 tankStartPoint;
    [SerializeField] Vector3 tankStartScale;
    [SerializeField] Quaternion tankStartRot;
    [Space]
    [SerializeField] Vector3 cbStartPoint;
    [SerializeField] Vector3 cbStartScale;
    //[SerializeField] Quaternion cbStartRot;

    CapsuleCollider sJCollider;


    // Start is called before the first frame update
    void Start()
    {
        GlobalFunctions.SetMainCam();

        Input.simulateMouseWithTouches = true;
        Time.timeScale = 1;
        //Particle system is turned off, will be turned on in the dump function 
        dumpPartSystem.GetComponent<ParticleSystem>().Stop();

        sJCollider = sludgeJudge.GetComponent<CapsuleCollider>();
        sJCollider.enabled = false;

        //Set up the event handler for tracking from Vuforia
        mTrackableBehaviour = GameObject.Find("ImageTarget").GetComponent<TrackableBehaviour>();

        if (mTrackableBehaviour)
            mTrackableBehaviour.RegisterTrackableEventHandler(this);


        mainCam = Camera.main;

        SetStartPositions();


        //hides the clipboard at the beginning
        HideClipboard();
    }


    // Update is called once per frame
    void Update()
    {
        if(waitTime > 0)
        {
            waitTime -= Time.deltaTime;
            if (waitTime < 0)
                waitTime = 0;
        }
        else
        {
            switch(step)
            {
                case 0:
                    IntroOne();
                    break;

                case 1:
                    IntroTwo();
                    break;

                case 2:
                    indicator.gameObject.SetActive(true);
                    step++;
                    break;

                case 3:
                    SJFirstMove();
                    break;

                case 4:
                    SludgeJudgeFlash(true);
                    step++;
                    break;

                case 5:
                    SJFirstSwipe();
                    break;

                case 6:
                    ClipboardDelay();
                    break;

                case 7:
                    ClipboardTrans();
                    break;

                case 8:
                    ClipboardKeypadInput();
                    break;

                case 9:
                    ClipboardExit();
                    break;

                case 10:
                    DumpTrans();
                    break;

                case 11:
                    dumpIndicator.gameObject.SetActive(true);
                    step++;
                    break;

                case 12:
                    SJSecondMove();
                    break;

                case 13:
                    SJSecondSwipe();
                    break;

                case 14:
                    End();
                    break;
            }
        }

        GlobalFunctions.UpdatePrevMousePos();
    }

    void IntroOne()
    {
        sludgeLevels = Mathf.Lerp(sludgeAmount.x, sludgeAmount.y, UnityEngine.Random.Range(0f, 1f));

        sludgeLevels = Mathf.RoundToInt(sludgeLevels);

        sludgeJudgeSludge.localScale = new Vector3(0.015f, 0.015f, sludgeLevels * (1.35f / 9));
        sludgeJudgeSludge.gameObject.SetActive(false);

        /*if(Mathf.CeilToInt(sludgeLevels) - sludgeLevels < 0.2f)
        {
            sludgeLevels = Mathf.CeilToInt(sludgeLevels);
        }
        else
        {
            sludgeLevels = Mathf.Floor(sludgeLevels);
        }*/

       

        if (!restarted)
        { 
            audioPlay.PlayAudio(); Debug.Log("AUDIO CLIP 1");
            waitTime += audioPlay.Tracks[0].length;
        }
        step++;
    }

    //Intro is split into two parts because of the delay for the audio. It would start both at once if it was in one function
    void IntroTwo()
    {
        if(!restarted)
        {
            StartCoroutine(SludgeJudgeFocus());
            waitTime += audioPlay.Tracks[1].length;
        }
        
        step++;
    }

    //Moving the sludge judge to the dip position
    void SJFirstMove()
    {
        sJCollider.enabled = false;
        if (GlobalFunctions.SlideObjectHorizontal(sludgeJudge, insertionPoint, imageTarget, sJMoveSpeedMult, sJMaxDistFromPoint, insertionDist))
        {
            //Sets all animation points' horizontal position to be the same as the sludge judge so it can dip in the correct location
            sJDipPoint.position = new Vector3(sludgeJudge.position.x, sJDipPoint.position.y, sludgeJudge.position.z);
            sJSampledPoint.position = new Vector3(sludgeJudge.position.x, sJSampledPoint.position.y, sludgeJudge.position.z);
            sJStartPoint = sludgeJudge.position;

            //Disables indicator effect
            indicator.gameObject.SetActive(false);
            sJCollider.enabled = true;


            //Logs that the sludge judge can be swiped down to start the animation
            Debug.Log("Reached Dip Point");

            if (!restarted)
            {
                audioPlay.PlayAudio(); Debug.Log("AUDIO CLIP 3");
                waitTime += audioPlay.Tracks[2].length;
            }

            GlobalFunctions.swipeDirection = Vector2.zero;

            step++;
        }
    }

    //Swipe down to dip sludge judge
    void SJFirstSwipe()
    {
        //Calls DetectTouch with a swipe distance of 15 pixels in each direction (15 left and right, 15 up and down)
        RaycastHit hit = GlobalFunctions.DetectTouch(this, new Vector2(1000, 15));

        if (hit.transform != null)
        {
            if (hit.transform == sludgeJudge)
            {
                StartCoroutine(GlobalFunctions.ColorFlash(sJMats, Color.black, 0.1f, 0.25f));
            }
        }

        //Checks to see if the user swipes downwards
        if (GlobalFunctions.swipeDirection == Vector2.down)
        {
            Debug.Log("Sludge judge swiped");
            //Starts the coroutine for the sludge judge dip animation
            StartCoroutine("SludgeJudgeDip");
            SludgeJudgeFlash(false);

            if (!restarted)
            {
                waitTime += audioPlay.Tracks[3].length + 6f;
            }

            step++;
        }
    }

    void ClipboardDelay()
    {
        if (!restarted)
        {
            audioPlay.PlayAudio(); Debug.Log("AUDIO CLIP 5");
            waitTime += audioPlay.Tracks[4].length;  // AUDIO PLAY 5 
        }
        else
        {
            waitTime += audioPlay.Tracks[3].length - 6;
        }
        //Waits for the length of the full dip animation
        //yield return new WaitForSeconds(sJDipTime + sampleDialogeTime + (sJSampleTime * 2) + sJExamineTransTime + sJReturnDelay);
        step++;
    }


    void ClipboardTrans()
    {
        sludgeSampleFlash.gameObject.SetActive(true);

        //write function -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        SetClipboardDate();
        ShowClipboard();
        clipboardKeyboard.gameObject.SetActive(true);

        step++;
    }


    void ClipboardKeypadInput()
    {
        sJCollider.enabled = false;
        RaycastHit hit = GlobalFunctions.DetectTouch(this);

        if (hit.transform != null)
        {
            Transform hitTrans = hit.transform;

            if (hitTrans.name == "Enter" && clipboardInput == Mathf.RoundToInt(sludgeLevels))
            {
                audioButton.AudioCorrect();
                clipboardKeyboard.gameObject.SetActive(false);

                sJCollider.enabled = true;

                sludgeSampleFlash.gameObject.SetActive(false);
                sludgeJudgeFlash.gameObject.SetActive(true);

                step++;
            }

            else if (hitTrans.name == "Enter" && clipboardInput!= Mathf.RoundToInt(sludgeLevels))
            {
                audioButton.AudioIncorrect();
            }

            if (int.TryParse(hitTrans.name, out clipboardInput))
            {
                audioButton.ButtonClicked();
                TextMeshPro_reading[restarts].GetComponent<TMPro.TextMeshPro>().text = clipboardInput.ToString();
            }
        }
    }


    void ClipboardExit()
    {
        if (GlobalFunctions.DetectTouch(this).transform == sludgeJudge)
        {
            HideClipboard();


            Debug.Log("Sludge judge tapped");
            //Starts the coroutine to return the sludge judge and tank to normalorSeconds(sJDipTime + sampleDialogeTime + sJSampleTime + sJExamineTransTime + sJReturnDelay);

            SludgeJudgeFlash(false);
            StartCoroutine("SludgeJudgeReturn");

            waitTime += sJExamineTransTime + effectAppearDelay;

            step++;
        }
    }


    void DumpTrans()
    {
        if (!restarted)
        {
            audioPlay.PlayAudio(); Debug.Log("AUDIO CLIP 6");
            waitTime += audioPlay.Tracks[5].length;
        }

        step++;
    }


    //Moving sludge judge to dump point
    void SJSecondMove()
    {
        sJCollider.enabled = false;

        if (GlobalFunctions.SlideObjectHorizontal(sludgeJudge, dumpPoint, imageTarget, sJMoveSpeedMult, sJMaxDistFromPoint, dumpDist))
        {
            sJCollider.enabled = true;

            //Sets all animation points' horizontal position to be the same as the sludge judge so it can dip in the correct location
            sJDipPoint.position = new Vector3(sludgeJudge.position.x, sJDipPoint.position.y, sludgeJudge.position.z);
            sJSampledPoint.position = new Vector3(sludgeJudge.position.x, sJSampledPoint.position.y, sludgeJudge.position.z);
            sJStartPoint = sludgeJudge.position;

            //Disables indicator effect
            dumpIndicator.gameObject.SetActive(false);

            SludgeJudgeFlash(true);
            GlobalFunctions.swipeDirection = Vector2.zero;

            step++;
        }
    }

    void SJSecondSwipe()
    {
        //Calls DetectTouch with a swipe distance of 15 pixels in each direction (15 left and right, 15 up and down)
        RaycastHit hit = GlobalFunctions.DetectTouch(this, new Vector2(1000, 15));

        if (hit.transform != null)
        {
            if (hit.transform == sludgeJudge)
            {
                StartCoroutine(GlobalFunctions.ColorFlash(sJMats, Color.black, 0.1f, 0.25f));
            }
        }

        //Checks to see if the user swipes downwards
        if (GlobalFunctions.swipeDirection == Vector2.down)
        {
            Debug.Log("Sludge judge swiped");
            //Starts the coroutine for the sludge judge dip animation
            StartCoroutine("SludgeJudgeDump");
            sludgeJudgeFlash.gameObject.SetActive(false);

            if (!restarted)
            {
                waitTime += audioPlay.Tracks[6].length + audioPlay.Tracks[7].length;
            }
            waitTime += sJDipTime + sJDumpReturnDelay + sJDipTime + 5;
            step++;
        }
    }

    void End()
    {

        /*while (true)
        {
            sJCollider.enabled = false;
            RaycastHit hit = GlobalFunctions.DetectTouch(this);

            if (hit.transform != null)
            {
                Transform hitTrans = hit.transform;

                if (hitTrans.name == "RestartButton")
                {
                    restartButton.gameObject.SetActive(false);
                    break;
                }
            }
            yield return null;
        }*/


        pauseButton.SetActive(false);
        pauseMenu.SetActive(true);
        restartButton.SetActive(true);
        playButton.SetActive(false);

        step++;
    }


    void WaterFlash(bool on)
    {
        tankWaterTopFlash.gameObject.SetActive(on);
    }

    void SludgeJudgeFlash(bool on)
    {
        sludgeJudgeFlash.gameObject.SetActive(on);
    }
    

    public void Restart()
    {
        restarts++;
        if (restarts >= 11)
        {
            restarts = 0;
        }
        restarted = true;


        HideClipboard();
        restartButton.gameObject.SetActive(false);

        sludgeJudge.gameObject.SetActive(true);
        sludgeJudge.position = sJSampledPoint.position;
        sludgeJudge.localScale = sJSampledPoint.localScale;
        sludgeJudge.rotation = sJSampledPoint.rotation;

        mainTank.gameObject.SetActive(true);
        mainTank.position = tankStartPoint;
        mainTank.localScale = tankStartScale;
        mainTank.rotation = tankStartRot;
        
        pauseButton.SetActive(true);
        pauseMenu.SetActive(false);
        restartButton.SetActive(false);
        playButton.SetActive(true);

        //StartCoroutine(SludgeJudgeStory());
        step = 0;
    }

    IEnumerator FadeDarkPlane(float opacity)
    {
        float startTime = Time.time;
        while(true)
        {
            darkPlane.color = Color.Lerp(darkPlane.color, new Color(darkPlane.color.r, darkPlane.color.g, darkPlane.color.b, opacity), (Time.time - startTime) / 2);
        }
    }

    private void SetClipboardDate()
    {
        //set the date and time automatically
        DateTime dt = GetNow();
        DateTime theTime = realTime();

        TextMeshPro_date[restarts].GetComponent<TMPro.TextMeshPro>().text = dt.ToString("yyyy-MM-dd");
        TextMeshPro_time[restarts].GetComponent<TMPro.TextMeshPro>().text = theTime.ToString("HH:mm:ss");
        TextMeshPro_reading[restarts].GetComponent<TMPro.TextMeshPro>().text = "----";

        Debug.Log(dt.ToString("yyyy-MM-dd"));
    }

    private void ShowClipboard()
    {
        sludgeJudgeClipboard.position = Vector3.Lerp(sludgeJudgeClipboard.position, cbStartPoint, 0);
        sludgeJudgeClipboard.localScale = Vector3.Lerp(sludgeJudgeClipboard.localScale, cbStartPoint, 0);
        //sludgeJudgeClipboard.rotation = Quaternion.Lerp(sludgeJudgeClipboard.rotation, cbStartRot, 5);

        //disables the clipboard text at start
        sludgeJudgeClipboard.gameObject.SetActive(true);
        TextMeshPro_date[restarts].gameObject.SetActive(true);
        TextMeshPro_time[restarts].gameObject.SetActive(true);
        TextMeshPro_reading[restarts].gameObject.SetActive(true);
    }

    private static DateTime GetNow()
    {
        return DateTime.Now;
    }
    private static DateTime realTime()
    {
        return DateTime.Now;

    }

    private void HideClipboard()
    {
        //shrinks the clipboard before it disappears
        sludgeJudgeClipboard.position = Vector3.Lerp(cbStartPoint, sludgeJudgeClipboard.position, 1);
        sludgeJudgeClipboard.localScale = Vector3.Lerp(cbStartPoint, sludgeJudgeClipboard.localScale, 1);
        //sludgeJudgeClipboard.rotation = Quaternion.Lerp(cbStartRot, sludgeJudgeClipboard.rotation, 5);
        //disables the clipboard text at start
        sludgeJudgeClipboard.gameObject.SetActive(false);
        TextMeshPro_date[restarts].gameObject.SetActive(false);
        TextMeshPro_time[restarts].gameObject.SetActive(false);
        TextMeshPro_reading[restarts].gameObject.SetActive(false);
    }

    //Focuses on Sludge Judge, then returns to normal
    IEnumerator SludgeJudgeFocus()
    {
        float timeStart = Time.time;
        float currentTime;
        while (true)
        {
            currentTime = Time.time - timeStart;
            sludgeJudge.position = Vector3.Lerp(sJSampledPoint.position, sJExaminePoint.position, currentTime / sJExamineTransTime);
            sludgeJudge.localScale = Vector3.Lerp(sJSampledPoint.localScale, sJExaminePoint.localScale, currentTime / sJExamineTransTime);
            sludgeJudge.rotation = Quaternion.Lerp(sJSampledPoint.rotation, sJExaminePoint.rotation, currentTime / sJExamineTransTime);


            mainTank.position = Vector3.Lerp(tankStartPoint, tankShrunkPoint.position, currentTime / sJExamineTransTime);
            mainTank.localScale = Vector3.Lerp(tankStartScale, tankShrunkPoint.localScale, currentTime / sJExamineTransTime);
            mainTank.rotation = Quaternion.Lerp(tankStartRot, tankShrunkPoint.rotation, currentTime / sJExamineTransTime);

            if (currentTime >= sJExamineTransTime)
            {
                break;
            }

            yield return null;
        }

        if (!restarted)
        {
            audioPlay.PlayAudio(); Debug.Log("AUDIO CLIP 2");
            yield return new WaitForSeconds(audioPlay.Tracks[1].length);
        }

        timeStart = Time.time;
        while (true)
        {
            currentTime = Time.time - timeStart;
            sludgeJudge.position = Vector3.Lerp(sJExaminePoint.position, sJSampledPoint.position, currentTime / sJExamineTransTime);
            sludgeJudge.localScale = Vector3.Lerp(sJExaminePoint.localScale, sJSampledPoint.localScale, currentTime / sJExamineTransTime);
            sludgeJudge.rotation = Quaternion.Lerp(sJExaminePoint.rotation, sJSampledPoint.rotation, currentTime / sJExamineTransTime);


            mainTank.position = Vector3.Lerp(tankShrunkPoint.position, tankStartPoint, currentTime / sJExamineTransTime);
            mainTank.localScale = Vector3.Lerp(tankShrunkPoint.localScale, tankStartScale, currentTime / sJExamineTransTime);
            mainTank.rotation = Quaternion.Lerp(tankShrunkPoint.rotation, tankStartRot, currentTime / sJExamineTransTime);

            if (currentTime >= sJExamineTransTime)
            {
                break;
            }

            yield return null;
        }
    }

    IEnumerator SludgeJudgeDip()
    {
        if(!restarted)
            audioPlay.PlayAudio(); Debug.Log("AUDIO CLIP 4");

        //Stores the time the animation started
        float timeStart = Time.time;
        float currentTime;
        while (true)
        {
            //Sets the time currently for animation lerping
            currentTime = Time.time - timeStart;
            //Lerps (Linear Inerperlates) between the sludge judge start point, to the point it dips into the water
            //As Lerp works between 0 and 1, diving current time by the animation time will give 1 when current time has reached the animation duration
            sludgeJudge.position = Vector3.Lerp(sJStartPoint, sJDipPoint.position, currentTime / sJDipTime);
            sludgeJudge.localScale = Vector3.Lerp(sJStartScale, sJDipPoint.localScale, currentTime / sJDipTime);
            sludgeJudge.rotation = Quaternion.Lerp(sJStartRot, sJDipPoint.rotation, currentTime / sJDipTime);

            //Breaks out of the while when the current time has reached the animation time
            if(currentTime >= sJDipTime)
            {
                break;
            }

            //Pauses the Coroutine for the rest of the frame, resuming next frame
            //This stops the while loop from locking up the program, though it only works in a coroutine
            yield return null;
        }

        sludgeJudgeSludge.gameObject.SetActive(true);

        if(!restarted)
            yield return new WaitForSeconds(audioPlay.Tracks[3].length - 3f); // Used to shorten the delay between the audio and animation finishing
        else
            yield return new WaitForSeconds(2f);

        //yield return new WaitForSeconds(sampleDialogeTime);

        //Sets the 'start time' again so we can repeat the process above -- the current time will start from 0 again because of it
        //The process is the same as the above, only with different variables
        timeStart = Time.time;
        while (true)
        {
            currentTime = Time.time - timeStart;
            sludgeJudge.position = Vector3.Lerp(sJDipPoint.position, sJSampledPoint.position, currentTime / sJSampleTime);
            sludgeJudge.localScale = Vector3.Lerp(sJDipPoint.localScale, sJSampledPoint.localScale, currentTime / sJSampleTime);
            sludgeJudge.rotation = Quaternion.Lerp(sJDipPoint.rotation, sJSampledPoint.rotation, currentTime / sJSampleTime);

            if (currentTime >= sJSampleTime)
            {
                break;
            }
            
            yield return null;
        }

        yield return new WaitForSeconds(sJSampleTime);

        //Sets the 'start time' so we can repeat the process again -- the current time will start from 0 again because of it
        //The process is the same as the first and second time, only with different variables
        timeStart = Time.time;
        while (true)
        {
            currentTime = Time.time - timeStart;
            sludgeJudge.position = Vector3.Lerp(sJSampledPoint.position, sJExaminePoint.position, currentTime / sJExamineTransTime);
            sludgeJudge.localScale = Vector3.Lerp(sJSampledPoint.localScale, sJExaminePoint.localScale, currentTime / sJExamineTransTime);
            sludgeJudge.rotation = Quaternion.Lerp(sJSampledPoint.rotation, sJExaminePoint.rotation, currentTime / sJExamineTransTime);


            mainTank.position = Vector3.Lerp(tankStartPoint, tankShrunkPoint.position, currentTime / sJExamineTransTime);
            mainTank.localScale = Vector3.Lerp(tankStartScale, tankShrunkPoint.localScale, currentTime / sJExamineTransTime);
            mainTank.rotation = Quaternion.Lerp(tankStartRot, tankShrunkPoint.rotation, currentTime / sJExamineTransTime);

            if (currentTime >= sJExamineTransTime)
            {
                break;
            }
            
            yield return null;
        }
    }

    IEnumerator SludgeJudgeReturn()
    {
        //Stores the time the animation started
        float timeStart = Time.time;
        float currentTime;
        while (true)
        {
            currentTime = Time.time - timeStart;
            sludgeJudge.position = Vector3.Lerp(sJExaminePoint.position, sJSampledPoint.position, currentTime / sJExamineTransTime);
            sludgeJudge.localScale = Vector3.Lerp(sJExaminePoint.localScale, sJSampledPoint.localScale, currentTime / sJExamineTransTime);
            sludgeJudge.rotation = Quaternion.Lerp(sJExaminePoint.rotation, sJSampledPoint.rotation, currentTime / sJExamineTransTime);


            mainTank.position = Vector3.Lerp(tankShrunkPoint.position, tankStartPoint, currentTime / sJExamineTransTime);
            mainTank.localScale = Vector3.Lerp(tankShrunkPoint.localScale, tankStartScale, currentTime / sJExamineTransTime);
            mainTank.rotation = Quaternion.Lerp(tankShrunkPoint.rotation, tankStartRot, currentTime / sJExamineTransTime);

            if (currentTime >= sJExamineTransTime)
            {
                break;
            }

            yield return null;
        }
    }


    //function to dump the sludge after the second dipping animation
    IEnumerator SludgeJudgeDump()
    {
        //resetting start time
        float timeStart = Time.time;
        float currentTime;

        //coroutine to dip sludge judge the second time
        while (true)
        {
            currentTime = Time.time - timeStart;

            sludgeJudge.position = Vector3.Lerp(sJStartPoint, sJDumpPoint.position, currentTime / sJDipTime);
            if (currentTime >= sJDipTime)
            {
                break;
            }

            yield return null;
        }

        //sludge dump particle system turned set to active to dump the particles
        dumpPartSystem.GetComponent<ParticleSystem>().Play();


        timeStart = Time.time;
        float sludgeScale = sludgeJudgeSludge.localScale.z;
        while (true)
        {
            currentTime = Time.time - timeStart;
            
            sludgeJudgeSludge.localScale = new Vector3(sludgeJudgeSludge.localEulerAngles.x, sludgeJudgeSludge.localEulerAngles.y, Mathf.Lerp(sludgeJudgeSludge.localScale.z, 0, currentTime / sJDumpReturnDelay));

            if (currentTime >= sJDumpReturnDelay)
            {
                break;
            }

            yield return null;
        }
        sludgeJudgeSludge.gameObject.SetActive(false);

        timeStart = Time.time;
        dumpPartSystem.GetComponent<ParticleSystem>().Stop();
        while (true)
        {

            currentTime = Time.time - timeStart;

            sludgeJudge.position = Vector3.Lerp(sJDumpPoint.position, sJStartPoint, currentTime / sJDipTime);
            if (currentTime >= sJDipTime)
            {
                break;
            }

            yield return null;
        }


        if (!restarted)
        {
            audioPlay.PlayAudio(); Debug.Log("AUDIO CLIP 7");
            yield return new WaitForSeconds(audioPlay.Tracks[6].length);

            audioPlay.PlayAudio(); Debug.Log("AUDIO CLIP 8");
            yield return new WaitForSeconds(audioPlay.Tracks[7].length);


            audioPlay.PlayAudio(); Debug.Log("AUDIO CLIP 9");
        }
        

        timeStart = Time.time;
        while (true)
        {
            currentTime = Time.time - timeStart;
            sludgeJudge.position = Vector3.Lerp(sJStartPoint, sJShrunkPoint.position, currentTime / sJExamineTransTime);
            sludgeJudge.localScale = Vector3.Lerp(sludgeJudge.localScale, sJShrunkPoint.localScale, currentTime / sJExamineTransTime);
            //sludgeJudge.rotation = Quaternion.Lerp(sJSampledPoint.rotation, sJExaminePoint.rotation, currentTime / sJExamineTransTime);


            mainTank.position = Vector3.Lerp(tankStartPoint, tankShrunkPoint.position, currentTime / sJExamineTransTime);
            mainTank.localScale = Vector3.Lerp(tankStartScale, tankShrunkPoint.localScale, currentTime / sJExamineTransTime);
            mainTank.rotation = Quaternion.Lerp(tankStartRot, tankShrunkPoint.rotation, currentTime / sJExamineTransTime);

            if (currentTime >= sJExamineTransTime)
            {
                break;
            }

            yield return null;
        }

        sludgeJudge.gameObject.SetActive(false);
        mainTank.gameObject.SetActive(false);


        ShowClipboard();
    }



    //Sets a Vector3 to the position of an object at the start
    //Simply setting Vector3s for the objects instead of creating a seperate animation point for the start position is somewhat easier, which is why this method is used
    void SetStartPositions()
    {
        sJStartPoint = sludgeJudge.position;
        sJStartScale = sludgeJudge.localScale;
        sJStartRot = sludgeJudge.rotation;

        tankStartPoint = mainTank.position;
        tankStartScale = mainTank.localScale;
        tankStartRot = mainTank.rotation;

        cbStartPoint = sludgeJudgeClipboard.position;
        cbStartScale = sludgeJudgeClipboard.localScale;
        //cbStartRot = sludgeJudgeClipboard.rotation;
    }


    //Will begin scenario once the tracking of the object begins. This is a Vuforia-triggered function
    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        if ((newStatus == TrackableBehaviour.Status.TRACKED || newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED) && previousStatus == TrackableBehaviour.Status.NO_POSE && scenarioHasStarted == false)
        {
            Debug.Log("Starting story");
            scenarioHasStarted = true;
            //StartCoroutine("SludgeJudgeStory");
            if (step < 0)
                step = 0;
        }
    }
}
