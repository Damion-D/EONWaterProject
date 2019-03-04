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

    [Header("Object References")]
    [SerializeField] Transform sludgeJudge;
    [SerializeField] Transform sludgeJudgeFlash;
    [SerializeField] Transform mainTank;
    [SerializeField] Transform insertionPoint;
    [SerializeField] Transform tankWaterTop;
    [SerializeField] Transform tankWaterTopFlash;
    [SerializeField] Transform indicator;
    [SerializeField] Transform dumpIndicator;
    [SerializeField] Transform dumpPoint;

    [Space]

    public Camera mainCam;

    [Space]

    [Header("Sludge Colours")]
    public SludgeType sludgeType;

    public Material[] coloredMats;
    public Color[] sludgeColors;

    [SerializeField] private List<SludgeType> sludgeOptions = new List<SludgeType>();
    [Space]


    [Header("Misc Values")]
    [SerializeField] float insertionDist;
    [SerializeField] float dumpDist;

    [Header("Animation Times")]
    [SerializeField] float sJDipTime;
    [SerializeField] float sampleDialogeTime;
    [SerializeField] float sJSampleTime;
    [SerializeField] float sJExamineTransTime;
    [SerializeField] float sJReturnDelay;
    [SerializeField] float effectAppearDelay;

    //Different points for the animations
    [Header("Position Object References")]
    [SerializeField] Transform sJDipPoint;
    [SerializeField] Transform sJSampledPoint;
    [SerializeField] Transform sJExaminePoint;
    [SerializeField] Transform tankShrunkPoint;

    [Header("Start Positions")]
    [SerializeField] Vector3 sJStartPoint;
    [SerializeField] Vector3 sJStartScale;
    [SerializeField] Quaternion sJStartRot;
    [Space]
    [SerializeField] Vector3 tankStartPoint;
    [SerializeField] Vector3 tankStartScale;
    [SerializeField] Quaternion tankStartRot;


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

        SetStartPositions();
    }

    // Update is called once per frame
    void Update()
    {
        //Old functionality to change between different sludge types, may be removed in the near future
        if (sludgeOptions.Count > 0 && Input.GetKeyDown(KeyCode.A))
        {
            RandomizeSludge();
            SludgeColor();
        }

        //Tests a touch by logging the name of the object detected
        if(Input.GetKeyDown(KeyCode.S))
        {
            //RaycastHit returns all the info from raycast detection
            RaycastHit hit = GlobalFunctions.DetectTouch(this);
            if(hit.transform != null)
            {
                //Logs name of detected object
                Debug.Log(hit.transform.name);
            }
        }
    }

    void RandomizeSludge()
    {
        //Sets sludge type to one of the available options
        sludgeType = sludgeOptions[Random.Range(0, sludgeOptions.Count)];
        //Removes set sludge type
        sludgeOptions.Remove(sludgeType);
        //Logs sludge type
        Debug.Log("Currently " + sludgeType + " sludge.");
    }

    void SludgeColor()
    {
        //Sets the color of all materials in 'coloredMats' to the color of the sludge based on type
        for (int i = 0; i < coloredMats.Length; i++)
        {
            coloredMats[i].color = sludgeColors[(int)sludgeType];
        }
    }
    

    IEnumerator SludgeJudgeStory()
    {
        //Will loop endlessly until a 'break' statement is reached
        while(true)
        {
            //RaycastHit returns all the info from raycast detection
            RaycastHit hit = GlobalFunctions.DetectConstantTouch();
            if(hit.transform != null)
            {
                //If the object detected is the same as the tankWaterTop
                if (hit.transform == tankWaterTop)
                {
                    //Sets position of the sludge judge on only the horizotal (x and z) to the location the touch was detected
                    sludgeJudge.position = new Vector3(hit.point.x, sludgeJudge.position.y, hit.point.z);
                }
            }
            //Checks to see if the horizontal (no y) distance is less than the distance to insert
            else if(Vector2.Distance(new Vector2(sludgeJudge.position.x, sludgeJudge.position.z), new Vector2(insertionPoint.position.x, insertionPoint.position.z)) < insertionDist)
            {
                //Sets all animation points' horizontal position to be the same as the sludge judge so it can dip in the correct location
                sJDipPoint.position = new Vector3(sludgeJudge.position.x, sJDipPoint.position.y, sludgeJudge.position.z);
                sJSampledPoint.position = new Vector3(sludgeJudge.position.x, sJSampledPoint.position.y, sludgeJudge.position.z);
                sJStartPoint = sludgeJudge.position;

                //Disables indicator effect
                indicator.gameObject.SetActive(false);
                break;
            }

            //Pauses the Coroutine for the rest of the frame, resuming next frame
            //This stops the while loop from locking up the program, though it only works in a coroutine
            yield return null;
        }

        //Logs that the sludge judge can be swiped down to start the animation
        Debug.Log("Reached Dip Point");

        while (true)
        {
            //Calls DetectTouch with a swipe distance of 100 pixels in each direction (100 left and right, 100 up and down)
            GlobalFunctions.DetectTouch(this, new Vector2(100, 100));
            //Checks to see if the user swipes downwards
            if (GlobalFunctions.swipeDirection == Vector2.down)
            {
                Debug.Log("Sludge judge swiped");
                //Starts the coroutine for the sludge judge dip animation
                StartCoroutine("SludgeJudgeDip");
                break;
            }
            yield return null;
        }

        //Waits for the length of the full dip animation
        yield return new WaitForSeconds(sJDipTime + sampleDialogeTime + (sJSampleTime * 2) + sJExamineTransTime + sJReturnDelay);

        sludgeJudgeFlash.gameObject.SetActive(true);

        while (true)
        {
            if (GlobalFunctions.DetectTouch(this).transform == sludgeJudge)
            {
                Debug.Log("Sludge judge tapped");
                //Starts the coroutine to return the sludge judge and tank to normalorSeconds(sJDipTime + sampleDialogeTime + sJSampleTime + sJExamineTransTime + sJReturnDelay);

                sludgeJudgeFlash.gameObject.SetActive(false);
                StartCoroutine("SludgeJudgeReturn");
                break;
            }
            yield return null;
        }

        yield return new WaitForSeconds(sJExamineTransTime + effectAppearDelay);

        dumpIndicator.gameObject.SetActive(true);

        while (true)
        {
            //RaycastHit returns all the info from raycast detection
            RaycastHit hit = GlobalFunctions.DetectConstantTouch();
            if (hit.transform != null)
            {
                //If the object detected is the same as the tankWaterTop
                if (hit.transform == tankWaterTop)
                {
                    //Sets position of the sludge judge on only the horizotal (x and z) to the location the touch was detected
                    sludgeJudge.position = new Vector3(hit.point.x, sludgeJudge.position.y, hit.point.z);
                }
            }
            //Checks to see if the horizontal (no y) distance is less than the distance to insert
            else if (Vector2.Distance(new Vector2(sludgeJudge.position.x, sludgeJudge.position.z), new Vector2(dumpPoint.position.x, dumpPoint.position.z)) < dumpDist)
            {
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

    }

    IEnumerator SludgeJudgeDip()
    {
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


        yield return new WaitForSeconds(sampleDialogeTime);

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
