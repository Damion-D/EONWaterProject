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

    //If restarted, skips dialogue
    [SerializeField] int restarts;
    [SerializeField] bool restarted;

    [SerializeField] AudioScript audioPlay;
    [SerializeField] Utility utilityScript;

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
        //Tests a touch by logging the name of the object detected
        if (Input.GetKeyDown(KeyCode.S))
        {
            //RaycastHit returns all the info from raycast detection
            RaycastHit hit = GlobalFunctions.DetectTouch(this);
            if (hit.transform != null)
            {
                //Logs name of detected object
                Debug.Log(hit.transform.name);
            }
        }
    }


    IEnumerator SludgeJudgeStory()
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

        if(!restarted)
        {
            audioPlay.PlayAudio(); Debug.Log("AUDIO CLIP 1");
            yield return new WaitForSeconds(audioPlay.Tracks[0].length);

            StartCoroutine(SludgeJudgeFocus());

            yield return new WaitForSeconds(audioPlay.Tracks[1].length);
        }


        //Activates indicator effect
        tankWaterTopFlash.gameObject.SetActive(true);

        //Will loop endlessly until a 'break' statement is reached
        while (true)
        {
            sJCollider.enabled = false;
            //RaycastHit returns all the info from raycast detection
            RaycastHit hit = GlobalFunctions.DetectConstantTouch();
            if (hit.transform != null)
            {
                //If the object detected is the same as the tankWaterTop
                if (hit.transform == tankWaterTop)
                {
                    tankWaterTopFlash.gameObject.SetActive(false);
                    indicator.gameObject.SetActive(true);
                    //Sets position of the sludge judge on only the horizotal (x and z) to the location the touch was detected
                    sludgeJudge.position = new Vector3(hit.point.x, sludgeJudge.position.y, hit.point.z);
                }
            }
            //Checks to see if the horizontal (no y) distance is less than the distance to insert
            else if (Vector2.Distance(new Vector2(sludgeJudge.position.x, sludgeJudge.position.z), new Vector2(insertionPoint.position.x, insertionPoint.position.z)) < insertionDist)
            {
                //Sets all animation points' horizontal position to be the same as the sludge judge so it can dip in the correct location
                sJDipPoint.position = new Vector3(sludgeJudge.position.x, sJDipPoint.position.y, sludgeJudge.position.z);
                sJSampledPoint.position = new Vector3(sludgeJudge.position.x, sJSampledPoint.position.y, sludgeJudge.position.z);
                sJStartPoint = sludgeJudge.position;

                //Disables indicator effect
                indicator.gameObject.SetActive(false);
                sJCollider.enabled = true;
                break;
            }

            //Pauses the Coroutine for the rest of the frame, resuming next frame
            //This stops the while loop from locking up the program, though it only works in a coroutine
            yield return null;
        }

        //Logs that the sludge judge can be swiped down to start the animation
        Debug.Log("Reached Dip Point");

        if(!restarted)
        {
            audioPlay.PlayAudio(); Debug.Log("AUDIO CLIP 3");
            yield return new WaitForSeconds(audioPlay.Tracks[2].length);
        }

        sludgeJudgeFlash.gameObject.SetActive(true);
        GlobalFunctions.swipeDirection = Vector2.zero;

        while (true)
        {
            //Calls DetectTouch with a swipe distance of 15 pixels in each direction (15 left and right, 15 up and down)
            RaycastHit hit = GlobalFunctions.DetectTouch(this, new Vector2(1000, 15));
            
            if(hit.transform != null)
            {
                if(hit.transform == sludgeJudge)
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
                sludgeJudgeFlash.gameObject.SetActive(false);
                break;
            }
            yield return null;
        }



        if(!restarted)
        {
            yield return new WaitForSeconds(audioPlay.Tracks[3].length + 6f);
            audioPlay.PlayAudio(); Debug.Log("AUDIO CLIP 5");
            yield return new WaitForSeconds(audioPlay.Tracks[4].length);  // AUDIO PLAY 5 
        }
        else
        {
            yield return new WaitForSeconds(audioPlay.Tracks[3].length - 6);
        }

        //Waits for the length of the full dip animation
        //yield return new WaitForSeconds(sJDipTime + sampleDialogeTime + (sJSampleTime * 2) + sJExamineTransTime + sJReturnDelay);

        sludgeSampleFlash.gameObject.SetActive(true);

        //write function -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        SetClipboardDate();
        ShowClipboard();
        clipboardKeyboard.gameObject.SetActive(true);

        while(true)
        {
            sJCollider.enabled = false;
            RaycastHit hit = GlobalFunctions.DetectTouch(this);
            
            if(hit.transform != null)
            {
                Transform hitTrans = hit.transform;

                if (hitTrans.name == "Enter" && clipboardInput == Mathf.RoundToInt(sludgeLevels))
                {
                    clipboardKeyboard.gameObject.SetActive(false);
                    break;
                }

                if (int.TryParse(hitTrans.name, out clipboardInput))
                {
                    TextMeshPro_reading[restarts].GetComponent<TMPro.TextMeshPro>().text = clipboardInput.ToString();
                }
            }
            yield return null;
        }

        sJCollider.enabled = true;

        sludgeSampleFlash.gameObject.SetActive(false);
        sludgeJudgeFlash.gameObject.SetActive(true);

        while (true)
        {
            if (GlobalFunctions.DetectTouch(this).transform == sludgeJudge)
            {
                HideClipboard();


                Debug.Log("Sludge judge tapped");
                //Starts the coroutine to return the sludge judge and tank to normalorSeconds(sJDipTime + sampleDialogeTime + sJSampleTime + sJExamineTransTime + sJReturnDelay);

                sludgeJudgeFlash.gameObject.SetActive(false);
                StartCoroutine("SludgeJudgeReturn");
                break;
            }
            yield return null;
        }

        yield return new WaitForSeconds(sJExamineTransTime + effectAppearDelay);

        if(!restarted)
        {
            audioPlay.PlayAudio(); Debug.Log("AUDIO CLIP 6");
            yield return new WaitForSeconds(audioPlay.Tracks[5].length);
        }

        tankWaterTopFlash.gameObject.SetActive(true);

        while (true)
        {
            sJCollider.enabled = false;
            //RaycastHit returns all the info from raycast detection
            RaycastHit hit = GlobalFunctions.DetectConstantTouch();
            if (hit.transform != null)
            {
                //If the object detected is the same as the tankWaterTop
                if (hit.transform == tankWaterTop)
                {
                    tankWaterTopFlash.gameObject.SetActive(false);
                    dumpIndicator.gameObject.SetActive(true);
                    //Sets position of the sludge judge on only the horizotal (x and z) to the location the touch was detected
                    sludgeJudge.position = new Vector3(hit.point.x, sludgeJudge.position.y, hit.point.z);
                }
            }
            //Checks to see if the horizontal (no y) distance is less than the distance to insert
            else if (Vector2.Distance(new Vector2(sludgeJudge.position.x, sludgeJudge.position.z), new Vector2(dumpPoint.position.x, dumpPoint.position.z)) < dumpDist)
            {
                sJCollider.enabled = true;
                
                //Sets all animation points' horizontal position to be the same as the sludge judge so it can dip in the correct location
                sJDipPoint.position = new Vector3(sludgeJudge.position.x, sJDipPoint.position.y, sludgeJudge.position.z);
                sJSampledPoint.position = new Vector3(sludgeJudge.position.x, sJSampledPoint.position.y, sludgeJudge.position.z);
                sJStartPoint = sludgeJudge.position;

                //Disables indicator effect
                dumpIndicator.gameObject.SetActive(false);
                break;
            }

            //Pauses the Coroutine for the rest of the frame, resuming next frame
            //This stops the while loop from locking up the program, though it only works in a coroutine
            yield return null;
        }

        sludgeJudgeFlash.gameObject.SetActive(true);
        GlobalFunctions.swipeDirection = Vector2.zero;

        while (true)
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
                break;
            }
            yield return null;
        }


        if (!restarted)
        {
            yield return new WaitForSeconds(audioPlay.Tracks[6].length + audioPlay.Tracks[7].length);
        }


        yield return new WaitForSeconds(sJDipTime + sJDumpReturnDelay + sJDipTime + 5);



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

        StartCoroutine(SludgeJudgeStory());
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
            StartCoroutine("SludgeJudgeStory");
        }
    }
}
