using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Vuforia;

public class TitrationScenario : MonoBehaviour, ITrackableEventHandler
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
    [SerializeField] ButtonAudio audioButton;

    public Camera mainCam;

    [Header("Object References")]
    [SerializeField] MeshRenderer[] splitCurve;
    [SerializeField] Transform wholeBreakpointCurve;
    [Space]
    [SerializeField] GameObject sqBottleFlash;
    [SerializeField] Transform sqBottle;
    [SerializeField] Transform pipetteWater;
    [SerializeField] Transform pipetFlash;
    [Space]
    [SerializeField] Transform waterSample;
    [SerializeField] CapsuleCollider wSCollider;
    [SerializeField] GameObject wSFlash;
    [SerializeField] GameObject agitatorIndicator;
    [SerializeField] Transform agitatorPoint;
    [Space]
    [SerializeField] Transform needleKnob;     // References the top knob
    [SerializeField] Transform needlePivot;    // References the top needle
    [SerializeField] Transform pipetteKnob;    // References the knob on the pipette squeeze bottle
    [SerializeField] GameObject bottomButtonPivot;  // This references the bottom knob
    [Space]
    [SerializeField] GameObject upperKnobFlash;
    [SerializeField] GameObject lowerKnobFlash;
    [SerializeField] GameObject pipetKnobFlash;
    [Space]
    [SerializeField] GameObject dropper;
    [SerializeField] Transform waterDropPos;
    [SerializeField] GameObject dropOgPos;
    [SerializeField] GameObject waterDropPart; //The water droplet
    [SerializeField] Transform twoMLBottle;
    [Space]
    [SerializeField] DropperDrag dropperDragScript;
    [Space]
    [SerializeField] Transform clipboardKeyboard;
    [SerializeField] Transform clipboard;
    [SerializeField] Transform clipboardPivot;
    [SerializeField] Transform titrator;
    Transform pipetValueContainer;
    [SerializeField] TMPro.TextMeshPro totalText;
    [SerializeField] TMPro.TextMeshPro diText;

    [Header("Script References")]
    [SerializeField] PipetteSqueeze pipetteSqueeze;
    [SerializeField] Clipboard clipboardScript;

    [Space]
    [Header("Clipboard References")]
    [SerializeField] public Transform[] textMeshPro_Row;
    [Space]
    [SerializeField] public Transform[] textMeshPro_date;
    [SerializeField] public Transform[] textMeshPro_time;
    [SerializeField] public Transform[] textMeshPro_freeChlorine;
    [SerializeField] public Transform[] textMeshPro_totalChlorine;
    [SerializeField] public Transform[] textMeshPro_freeToTotal;
    [SerializeField] public Transform[] textMeshPro_monoChlorine;
    [SerializeField] public Transform[] textMeshPro_diChlorine;
    [SerializeField] public Transform[] textMeshPro_monoToTotal;
    [SerializeField] public Transform[] textMeshPro_Ammonia;

    [Space]
    [Header("Material Refs")]
    [SerializeField] Material[] wSMats;
    [SerializeField] Material lineSegment;
    [SerializeField] Material lineSegmentFlash;
    [SerializeField] Material sampleWaterMat;

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
    float titratorAngleOffset;
    [SerializeField] float pippetteAmountToAdd;
    float currentPipetteAmount;
    [Space]
    [SerializeField] float dragSpeedMult;
    [SerializeField] float maxDistFromDragDest;
    [SerializeField] float sampleToAgitatorDist;
    [Space]
    [SerializeField] float needleSpeed;        // Refrences the speed the needle will travel by during testing process
    [SerializeField] float waterLevel;  // Refrences the current water level
    [SerializeField] bool firstRun;            // Refrences where we are in our steps
    [SerializeField] float WaterSpeed;
    public Quaternion KnobPosition { get; private set; }
    [Space]
    [SerializeField] float chlorineTotal; //for testing purposes, this will be random
    [SerializeField] float chlorineMono; //for testing purposes, this will be random
    [SerializeField] float chlorineDi; //Total - Mono is this
    [SerializeField] float stepClock;
    [SerializeField] string currentReading_str;
    [SerializeField] float currentReading_int = 0;
    [SerializeField] int numbersInputted;
    [Space]
    [SerializeField] float knobSpeed = 80f; // How fast the knob is being turned times Time.DeltaTime
    List<float> knobValues = new List<float>(); // A list to store any positions we will need with the knob turning
    [SerializeField] int knobRotIndex; // Shows what index from KnobValues is currently selected
    [Space]
    [SerializeField] float dropperRotSpeed; //Determines rotation speed when rotating an object
    //public SolutionScript moveSolution; // Refers to a script attached to the solution jar. The script 
    //attached to solution jar has onMouseDown and onMouseDrag
    int waterDropRun; //Used to determine what stage the program is at, when it is incremented //this prevents for example the solution jar from being lerped again to the same position
    float angleRot; //What angle an object is rotating to
    float rotTrans; // The euler axis the gameobject is rotating on converted to a float value
    float timer; // allows for a delay in the water changing color
    float test; // Represents adding time to the timer
    bool timerRun; // Determines whether the timer is starting or not
    bool waterWait; // If true it allows the rotation and lerping of water dropper
    int runCount; //What run count determines the color of the solution depending on what //process is being used

    float timer2; //Timer that delays the lerping/rotation of water dropper
    int timesClicked; // Ensures that the user has to wait a second before they can add more //drops to the solution. Also allows animation of water droplet to play
    int count; // Number that points to the index of the water in the list the solution is changing to

    bool rotationStart;

    public static GameObject WaterDroplet;

    List<Color> waterMaterials = new List<Color>();


    [SerializeField] GameObject h20Part; // The water inside the jar

    [SerializeField] Color32 waterColor1;
    [SerializeField] Color32 waterColor2;
    [SerializeField] Color32 waterColor3;
    [SerializeField] Color32 waterColor4;
    [SerializeField] Color32 waterColor5;


    [Space]
    [Header("Animation Times")]
    [SerializeField] float titratorExpandTime;
    [SerializeField] float waterSampleAttachTime;
    [SerializeField] float curveDisplayTime;
    [SerializeField] float clipboardDisplayTime;

    [Space]
    [Header("Animation References")]
    [SerializeField] float knobMGLRot; // The knob value for the mg/L position on bottom knob

    [SerializeField] float knobStandbyRot; // The knob value for the standby position on bottom knob

    [SerializeField] float knobOffRot; // The knob value for the off position on bottom knob

    [SerializeField] Transform titratorScaleStart;
    [SerializeField] Transform titratorScaleEnd;
    [SerializeField] Transform waterSampleAttachPoint;
    [SerializeField] GameObject jarDropPos; // Position of where solution jar will be after all 4 //drops are added to it.


    /*[Space]
    [Header("Start Positions")]*/

    // Start is called before the first frame update
    void Start()
    {
        GlobalFunctions.SetMainCam();

        totalText.color = new Color(0, 0, 0, 1);
        diText.color = new Color(0, 0, 0, 1);

        pipetValueContainer = totalText.transform.parent;

        Input.simulateMouseWithTouches = true;
        Time.timeScale = 1;

        //Set up the event handler for tracking from Vuforia
        mTrackableBehaviour = GameObject.Find("ImageTarget").GetComponent<TrackableBehaviour>();

        if (mTrackableBehaviour)
            mTrackableBehaviour.RegisterTrackableEventHandler(this);


        knobRotIndex = 0;


        knobValues.Add(knobMGLRot);
        knobValues.Add(knobStandbyRot);
        knobValues.Add(knobOffRot);  // This sets the values of each position and adds them to the list

        waterMaterials.Add(waterColor1);
        waterMaterials.Add(waterColor2);
        waterMaterials.Add(waterColor3);
        waterMaterials.Add(waterColor4);
        waterMaterials.Add(waterColor5);

        timer = 0;
        timesClicked = 1;
        dropperRotSpeed = 70f;
        waterDropRun = 0;

        count = 0;
        runCount = 1;


        sampleWaterMat.SetColor("_FoamColor", waterMaterials[count]);
        sampleWaterMat.SetColor("_DepthGradientDeep", waterMaterials[count]);
        sampleWaterMat.SetColor("_DepthGradientShallow", waterMaterials[count]);

        mainCam = Camera.main;
        

        firstRun = true;
        needleSpeed = 20f;
        waterLevel = 500f;
        //this is supposed to increase speed if the knob is held open too long
        WaterSpeed = 1f;


        pipetteWater.localScale = new Vector3(pipetteWater.localScale.x, 0, pipetteWater.localScale.z);
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
                    CurveDisplay1();
                    break;

                case 1:
                    TurnCurveTowardsCamera();
                    break;

                case 2:
                    CurveDisplay2();
                    break;

                case 3:
                    TurnCurveTowardsCamera();
                    break;

                case 4:
                    CurveDisplay3();
                    break;

                case 5:
                    TurnCurveTowardsCamera();
                    break;

                case 6:
                    CurveDisplay4();
                    break;

                case 7:
                    TurnCurveTowardsCamera();
                    break;

                case 8:
                    CurveDisplay5();
                    break;

                case 9:
                    DisplayTitrator();
                    break;

                case 10:
                    //StartCoroutine(AngleTitrator(20, 0.5f));
                    AngleTitrator(20, 0.075f);
                    TurnTowardsCamera(titrator.parent, titratorAngleOffset);

                    SquirtBottleTouchFill();
                    break;

                case 11:
                    //StartCoroutine(AngleTitrator(-20, 0.5f));
                    AngleTitrator(-20, 0.075f);
                    TurnTowardsCamera(titrator.parent, titratorAngleOffset);

                    AgitatorIndicator(true);
                    waterSample.gameObject.SetActive(true);
                    step++;
                    break;

                case 12:
                    //StartCoroutine(AngleTitrator(-20, 0.5f));
                    AngleTitrator(-20, 0.075f);
                    TurnTowardsCamera(titrator.parent, titratorAngleOffset);

                    MoveSample();
                    break;

                case 13:
                    //StartCoroutine(AngleTitrator(-20, 0.5f));
                    AngleTitrator(-20, 0.075f);
                    TurnTowardsCamera(titrator.parent, titratorAngleOffset);

                    SwipeSampleUp();
                    break;

                case 14:
                    //StartCoroutine(AngleTitrator(-10, 0.5f));
                    AngleTitrator(-10, 0.075f);
                    TurnTowardsCamera(titrator.parent, titratorAngleOffset);

                    LowerKnobFlash(true);
                    if (GlobalFunctions.DetectTouch(this).transform == bottomButtonPivot.transform)
                        step++;
                    break;

                case 15:
                    //StartCoroutine(AngleTitrator(-10, 0.5f));
                    AngleTitrator(-10, 0.075f);
                    TurnTowardsCamera(titrator.parent, titratorAngleOffset);

                    TurnOnTitrator();
                    break;

                case 16:
                    //StartCoroutine(AngleTitrator(-40, 0.5f));
                    AngleTitrator(-40, 0.075f);
                    TurnTowardsCamera(titrator.parent, titratorAngleOffset);

                    dropperDragScript.interactable = true;
                    dropperDragScript.FlashAppear(dropperDragScript.dropperFlash);
                    StartCoroutine(RaiseDropper(dropper.transform, 5.25f));
                    step++;
                    break;

                case 17:
                    //StartCoroutine(AngleTitrator(-40, 0.5f));
                    AngleTitrator(-40, 0.075f);
                    TurnTowardsCamera(titrator.parent, titratorAngleOffset);

                    if (!dropperDragScript.interactable)
                        step++;
                    break;

                case 18:
                    //StartCoroutine(AngleTitrator(-40, 0.5f));
                    AngleTitrator(-40, 0.075f);
                    TurnTowardsCamera(titrator.parent, titratorAngleOffset);

                    DropperDragged();
                    break;

                case 19:
                    //StartCoroutine(AngleTitrator(-40, 0.5f));
                    AngleTitrator(-40, 0.075f);
                    TurnTowardsCamera(titrator.parent, titratorAngleOffset);

                    DropperReturn();
                    break;

                case 20:
                    //StartCoroutine(AngleTitrator(20, 0.5f));
                    TurnTowardsCamera(titrator.parent, titratorAngleOffset);

                    SetReadings();
                    needleSpeed = 200;
                    UpperKnobFlash(true);
                    step++;
                    break;

                case 21:
                    //StartCoroutine(AngleTitrator(20, 0.5f));
                    AngleTitrator(20, 0.075f);
                    TurnTowardsCamera(titrator.parent, titratorAngleOffset);

                    TitrationTotal();
                    break;

                case 22:
                    //StartCoroutine(AngleTitrator(-40, 0.5f));
                    AngleTitrator(-40, 0.075f);
                    TurnTowardsCamera(titrator.parent, titratorAngleOffset);

                    count = 0;
                    dropperRotSpeed = 70;
                    dropperDragScript.interactable = true;
                    dropperDragScript.FlashAppear(dropperDragScript.dropperFlash);
                    StartCoroutine(RaiseDropper(dropper.transform, 5.25f));
                    step++;
                    break;

                case 23:
                    //StartCoroutine(AngleTitrator(-40, 0.5f));
                    AngleTitrator(-40, 0.075f);
                    TurnTowardsCamera(titrator.parent, titratorAngleOffset);

                    if (!dropperDragScript.interactable)
                        step++;
                    break;

                case 24:
                    //StartCoroutine(AngleTitrator(-40, 0.5f));
                    AngleTitrator(-40, 0.075f);
                    TurnTowardsCamera(titrator.parent, titratorAngleOffset);

                    DropperDragged2();
                    break;

                case 25:
                    //StartCoroutine(AngleTitrator(-40, 0.5f));
                    AngleTitrator(-40, 0.075f);
                    TurnTowardsCamera(titrator.parent, titratorAngleOffset);

                    DropperReturn2();
                    break;

                case 26:
                    //StartCoroutine(AngleTitrator(20, 0.5f));
                    AngleTitrator(20, 0.075f);
                    TurnTowardsCamera(titrator.parent, titratorAngleOffset);

                    needleSpeed = 200;
                    UpperKnobFlash(true);
                    step++;
                    break;

                case 27:
                    //StartCoroutine(AngleTitrator(20, 0.5f));
                    AngleTitrator(20, 0.075f);
                    TurnTowardsCamera(titrator.parent, titratorAngleOffset);

                    TitrationDi();
                    break;

                case 28:
                    //StartCoroutine(AngleTitrator(0, 0.5f));
                    TurnTowardsCamera(titrator.parent, titratorAngleOffset);

                    HideTitrator();
                    break;

                case 29:
                    DisplayClipboard();
                    break;

                case 30:
                    SetClipboardText();
                    break;

                case 31:
                    SetTotalReading();
                    break;

                case 32:
                    ClipboardKeypadInput();
                    break;

                case 33:
                    SetDiReading();
                    break;

                case 34:
                    ClipboardKeypadInput();
                    break;

                case 35:
                    SetChlorineMonoCount();
                    break;

                case 36:
                    SetMonoToTotal();
                    break;

                case 37:
                    End();
                    break;
            }
        }
        TurnTowardsCamera(clipboardPivot, 180);
        GlobalFunctions.UpdatePrevMousePos();
    }


    void Intro()
    {
        titrator.parent.localScale = Vector3.zero;
        clipboard.localScale = Vector3.one * 0.0001f;
        //clipboardKeyboard.transform.localScale = Vector3.zero;
        wholeBreakpointCurve.gameObject.SetActive(true);

        //pipetteSqueeze.enabled = false;
        titrator.gameObject.SetActive(false);
        clipboard.gameObject.SetActive(false);
        clipboardKeyboard.gameObject.SetActive(false);
        waterSample.gameObject.SetActive(false);
    }

    void TurnCurveTowardsCamera()
    {
        TurnTowardsCamera(wholeBreakpointCurve, 0);
        inStepWaitTime -= Time.deltaTime;
        if (inStepWaitTime <= 0)
            step++;
    }

    void CurveDisplay1()
    {
        //highlights whole curve, also starts anim coroutine
        StartCoroutine(DisplayCurve());
        splitCurve[0].material = lineSegmentFlash;
        splitCurve[1].material = lineSegmentFlash;
        splitCurve[2].material = lineSegmentFlash;
        inStepWaitTime = 5 + curveDisplayTime;
        step++;
    }

    void CurveDisplay2()
    {
        //activates first part of curve, highlight
        splitCurve[0].material = lineSegmentFlash;
        splitCurve[1].material = lineSegment;
        splitCurve[2].material = lineSegment;
        inStepWaitTime = 5;
        step++;
    }

    void CurveDisplay3()
    {
        //activates 2rd part of curve, highlight
        splitCurve[0].material = lineSegment;
        splitCurve[1].material = lineSegmentFlash;
        splitCurve[2].material = lineSegment;
        inStepWaitTime = 5;
        step++;
    }

    void CurveDisplay4()
    {
        //activates 3rd part of curve, highlight
        splitCurve[0].material = lineSegment;
        splitCurve[1].material = lineSegment;
        splitCurve[2].material = lineSegmentFlash;
        inStepWaitTime = 5;
        step++;
    }

    void CurveDisplay5()
    {
        //shuts off whole curve
        StartCoroutine(HideCurve());
        splitCurve[0].material = lineSegment;
        splitCurve[1].material = lineSegment;
        splitCurve[2].material = lineSegment;
        inStepWaitTime = 5 + curveDisplayTime;
        step++;
    }
    

    void DisplayTitrator()
    {
        titrator.gameObject.SetActive(true);
        StartCoroutine(ExpandTitrator());
        waitTime += titratorExpandTime;
        step++;
    }

    void SquirtBottleTouchFill()
    {
        SquirtBottleFlash(true);

        RaycastHit touched = GlobalFunctions.DetectConstantTouch();

        if (touched.transform != null)
        {
            if (touched.transform == sqBottle.transform)
            {
                currentPipetteAmount += pippetteAmountToAdd * Time.deltaTime;
                currentPipetteAmount = Mathf.Clamp01(currentPipetteAmount);
                pipetteWater.localScale = new Vector3(pipetteWater.localScale.x, Mathf.Lerp(0, 1, currentPipetteAmount), pipetteWater.localScale.z);
                if (currentPipetteAmount == 1)
                {
                    sqBottleFlash.gameObject.SetActive(false);
                    pipetFlash.gameObject.SetActive(true);
                    waitTime += 3;
                    step++;
                }

            }
        }
    }


    void MoveSample()
    {
        wSCollider.enabled = false;
        pipetFlash.gameObject.SetActive(false);
        if (GlobalFunctions.SlideObjectHorizontal(waterSample, agitatorPoint, titrator.parent, dragSpeedMult, maxDistFromDragDest, sampleToAgitatorDist))
        {
            //Disables indicator effect
            AgitatorIndicator(false);
            WaterSampleFlash(true);
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

            StartCoroutine(Display2MLBottle());

            waitTime += 3;
            step++;
        }
    }


    void TurnOnTitrator()
    {
        LowerKnobFlash(false);
        if (KnobMovement(bottomButtonPivot, knobMGLRot, knobSpeed))
        {
            step++;
        }
    }


    void DropperDragged()
    {

        MoveDrop(dropper.transform.position, waterDropPos.position, dropper, 0.5f);
        
        sampleWaterMat.SetColor("_FoamColor", waterMaterials[count]);
        sampleWaterMat.SetColor("_DepthGradientDeep", waterMaterials[count]);
        sampleWaterMat.SetColor("_DepthGradientShallow", waterMaterials[count]);

        if (count >= 4)
        {
            waitTime += 0.5f;
            step++;
        }
    }

    void DropperReturn()
    {
        StartCoroutine(ReturnDropper(dropper.transform));
        waitTime++;
        step++;
    }


    void SetReadings()
    {
        chlorineTotal = UnityEngine.Random.Range(2.6f, 3f);
        chlorineDi = UnityEngine.Random.Range(0.1f, 0.4f);

        chlorineTotal = Mathf.Round(chlorineTotal * 100) / 100;
        chlorineDi = Mathf.Round(chlorineDi * 100) / 100;

        waterLevel = chlorineTotal;
        
        StartCoroutine(Hide2MLBottle());
    }


    void TitrationTotal()
    {
        //while the water is above zreo the needle will deflect right on only the first run
        if (GlobalFunctions.DetectConstantTouch().transform == pipetteKnob)
        {
            //store the original position of the turned knob to turn it back when the user is done ---------------fix this
            KnobPosition = pipetteKnob.transform.rotation;
            //rotates the knob on the right
            pipetteKnob.transform.Rotate(0, 0, WaterSpeed * 10 * Time.deltaTime);
            //increases the speed as the user turns the right knob on the pipette
            //speed = WaterSpeed + speed;
            needleSpeed = 50;
            //tells the user how fast the water is moving through the cathod tube
            Debug.Log("Speed increase: " + needleSpeed);
            //subtracts the water level int each time the button is pressed
            waterLevel -= Time.deltaTime / 4;
            //displays the level for testing purposes
            Debug.Log(waterLevel);

            pipetteWater.localScale = new Vector3(pipetteWater.localScale.x, pipetteWater.localScale.y - (Time.deltaTime / 4), pipetteWater.localScale.z);
            if (pipetteWater.localScale.y <= 0)
                pipetteWater.localScale = new Vector3(1, pipetteWater.localScale.y + 1, 1);

            PipetKnobFlash(false);
        }
        else
            needleSpeed = Mathf.Clamp(needleSpeed - WaterSpeed, 0, 100);
        //rotate back
        //pipetteKnob.transform.rotation = knobPosition;
        //we constantly deflect right at the moment even without user interaction----------------------------------------

        if (Vector2.SignedAngle(needlePivot.up, Vector2.up) < 92)
            DeflectRight();
        else
            UpperKnobFlash(true);

        if (GlobalFunctions.DetectConstantTouch().transform == needleKnob)
        {
            if (Vector2.SignedAngle(needlePivot.up, Vector2.up) > -92)
            {
                UpperKnobFlash(false);

                //resets the speed when the user spins the needle so it doesnt get too fast
                needleSpeed = 10f;
                //moves the needle the opposite direction of the deflection
                needlePivot.transform.Rotate(-Vector3.forward * 100 * Time.deltaTime);
                //moves the knob
                needleKnob.transform.Rotate(-Vector3.forward * 100 * Time.deltaTime);
                Debug.Log(firstRun);
            }
            else
                PipetKnobFlash(true);
        }

        totalText.text = (Mathf.Round(Mathf.Lerp(chlorineTotal, 0, waterLevel / chlorineTotal) * 100) / 100).ToString() + " (Total)";
        

        if (waterLevel <= 0)
        {
            waterLevel = chlorineDi;
            step++;
            UpperKnobFlash(false);
            StartCoroutine(Display2MLBottle());
            waitTime += 3;
        }
    }


    void DropperDragged2()
    {
        MoveDrop(dropper.transform.position, waterDropPos.position, dropper, 0.5f);

        int index = Mathf.Abs(4 - count*2);
        sampleWaterMat.SetColor("_FoamColor", waterMaterials[index]);
        sampleWaterMat.SetColor("_DepthGradientDeep", waterMaterials[index]);
        sampleWaterMat.SetColor("_DepthGradientShallow", waterMaterials[index]);

        if (count >= 2)
        {
            waitTime += 0.5f;
            step++;
        }
    }

    void DropperReturn2()
    {
        StartCoroutine(ReturnDropper(dropper.transform));
        step++;
    }
    

    void TitrationDi()
    {
        //another check to make sure we are above zero
        //this is where the pipette knob touch detect will be attached to replace key code "S"---------------------------
        if (GlobalFunctions.DetectConstantTouch().transform == pipetteKnob)
        {
            //rotate the knob on the right
            pipetteKnob.transform.Rotate(0, 0, WaterSpeed * 10 * Time.deltaTime);
            //increases the speed as the user turns the right knob on the pipette
            //speed = WaterSpeed + speed;
            needleSpeed = 50;
            //tells the user how fast the water is moving through the cathod tube
            Debug.Log("Speed increase: " + needleSpeed);
            //subtracts the water level int each time the button is pressed
            waterLevel -= Time.deltaTime / 4;
            //displays the level for testing purposes
            Debug.Log(waterLevel);


            pipetteWater.localScale = new Vector3(pipetteWater.localScale.x, pipetteWater.localScale.y - (Time.deltaTime / 4), pipetteWater.localScale.z);
            if (pipetteWater.localScale.y <= 0)
                pipetteWater.localScale = new Vector3(1, pipetteWater.localScale.y + 1, 1);

            PipetKnobFlash(false);
        }
        else
            needleSpeed = Mathf.Clamp(needleSpeed - WaterSpeed, 0, 100);

        //set up a flash to indicate the user should interact with the pipetteKnob to continue--------------------------

        //rotate back
        // pipetteKnob.transform.rotation = knobPosition;

        //we constantly also deflect left as well ----------------------------------------------------------------------

        if (Vector2.SignedAngle(needlePivot.up, Vector2.up) > -92)
            DeflectLeft();
        else
            UpperKnobFlash(true);

        if (GlobalFunctions.DetectConstantTouch().transform == needleKnob && waterLevel > 0)
        {
            if (Vector2.SignedAngle(needlePivot.up, Vector2.up) < 92)
            {
                UpperKnobFlash(false);

                //resets the speed when the user spins the needle so it doesnt get too fast
                needleSpeed = 10f;
                //moves needle
                needlePivot.transform.Rotate(Vector3.forward * 100 * Time.deltaTime);
                //moves knob
                needleKnob.transform.Rotate(Vector3.forward * 100 * Time.deltaTime);
            }
            else
                PipetKnobFlash(true);
        }

        diText.text = (Mathf.Round(Mathf.Lerp(chlorineDi, 0, waterLevel / chlorineDi) * 100) / 100).ToString() + " (Di)";

        if (waterLevel <= 0)
        {
            StartCoroutine(Hide2MLBottle());
            step++;
            UpperKnobFlash(false);
            waitTime += 3;
        }
    }
    

    void HideTitrator()
    {
        pipetValueContainer.transform.parent = clipboardPivot;
        pipetValueContainer.transform.localPosition = new Vector3(0.216f, 0.133f, -0.266f);
        pipetValueContainer.transform.localEulerAngles = new Vector3(-65, -180, 0);
        StartCoroutine(ShrinkTitrator());
        //titrator.localScale = Vector3.zero;
        //waterSample.localScale = Vector3.zero;
        step++;
    }


    void DisplayClipboard()
    {
        clipboard.gameObject.SetActive(true);
        clipboardKeyboard.gameObject.SetActive(true);
        clipboard.localScale = Vector3.one * 0.0001f;
        StartCoroutine(ShowClip());

        SetClipboardText();
        step++;
        waitTime += 1;
    }

    void SetClipboardText()
    {
        //set the date and time automatically
        DateTime dt = GetNow();
        DateTime theTime = realTime();

        //put all auto stuff under this line
        textMeshPro_date[restarts].GetComponent<TMPro.TextMeshPro>().text = dt.ToString("MM-dd");
        textMeshPro_time[restarts].GetComponent<TMPro.TextMeshPro>().text = theTime.ToString("HH:mm");
        textMeshPro_freeChlorine[restarts].GetComponent<TMPro.TextMeshPro>().text = "0";
        textMeshPro_freeToTotal[restarts].GetComponent<TMPro.TextMeshPro>().text = "0";
        textMeshPro_Ammonia[restarts].GetComponent<TMPro.TextMeshPro>().text = "4:1";
        step++;
    }

    void SetTotalReading()
    {
        currentReading_str = "Total";
        currentReading_int = chlorineTotal;

        totalText.color = new Color(0, 0, 0, 1);
        diText.color = new Color(0, 0, 0, 0.25f);

        step++;
    }

    void SetDiReading()
    {
        //repeat step 2, but for mono
        currentReading_str = "Di";
        currentReading_int = chlorineDi;

        totalText.color = new Color(0, 0, 0, 0.25f);
        diText.color = new Color(0, 0, 0, 1);

        //once clock hits 0, add a step and reset clock
        step++;
        //pause for audio
    }

    void ClipboardKeypadInput()
    {
        RaycastHit hit = GlobalFunctions.DetectTouch(this);

        Debug.Log(hit.transform);

        clipboardKeyboard.gameObject.SetActive(true);

        //if on the total chlorine reading step, set current reading to "Total" and the int to "chlorineTotal"
        if (hit.transform != null)
        {
            Transform hitTrans = hit.transform;

            switch (currentReading_str)
            {
                case "Total":
                    float.TryParse(textMeshPro_totalChlorine[restarts].GetComponent<TMPro.TextMeshPro>().text, out clipboardInput);
                    break;

                case "Mono":
                    float.TryParse(textMeshPro_monoChlorine[restarts].GetComponent<TMPro.TextMeshPro>().text, out clipboardInput);
                    break;

                case "Di":
                    float.TryParse(textMeshPro_diChlorine[restarts].GetComponent<TMPro.TextMeshPro>().text, out clipboardInput);
                    break;
            }


            if (hitTrans.name == "Enter" && clipboardInput == currentReading_int)
            {
                clipboardKeyboard.gameObject.SetActive(false);

                //if number less than 3 digits
                for (int i = 0; i > 3 - numbersInputted; i++)
                {
                    switch (currentReading_str)
                    {
                        case "Total":
                            textMeshPro_totalChlorine[restarts].GetComponent<TMPro.TextMeshPro>().text += "0";
                            break;

                        case "Mono":
                            textMeshPro_monoChlorine[restarts].GetComponent<TMPro.TextMeshPro>().text += "0";
                            break;

                        case "Di":
                            textMeshPro_diChlorine[restarts].GetComponent<TMPro.TextMeshPro>().text += "0";
                            break;
                    }
                }
                numbersInputted = 0;

                //once clock hits 0, add a step and reset clock
                step++;
                stepClock = 5;
                //step++;
            }

            if (float.TryParse(hitTrans.name, out clipboardInput))
            {
                switch (currentReading_str)
                {
                    case "Total":
                        if (numbersInputted == 0)
                        {
                            textMeshPro_totalChlorine[restarts].GetComponent<TMPro.TextMeshPro>().text = clipboardInput.ToString() + ".";
                        }
                        else
                        {
                            textMeshPro_totalChlorine[restarts].GetComponent<TMPro.TextMeshPro>().text += clipboardInput.ToString();
                        }

                        break;

                    case "Mono":
                        if (numbersInputted == 0)
                        {
                            textMeshPro_monoChlorine[restarts].GetComponent<TMPro.TextMeshPro>().text = clipboardInput.ToString() + ".";
                        }
                        else
                        {
                            textMeshPro_monoChlorine[restarts].GetComponent<TMPro.TextMeshPro>().text += clipboardInput.ToString();
                        }
                        break;

                    case "Di":
                        if (numbersInputted == 0)
                        {
                            textMeshPro_diChlorine[restarts].GetComponent<TMPro.TextMeshPro>().text = clipboardInput.ToString() + ".";
                        }
                        else
                        {
                            textMeshPro_diChlorine[restarts].GetComponent<TMPro.TextMeshPro>().text += clipboardInput.ToString();
                        }
                        break;
                }

                numbersInputted++;
                if (numbersInputted > 2)
                    numbersInputted = 0;
            }
        }
    }

    void SetChlorineMonoCount()
    {
        totalText.color = new Color(0, 0, 0, 1);
        diText.color = new Color(0, 0, 0, 1);
        //rounds the chlorineDi float
        chlorineMono = chlorineTotal - chlorineDi;
        chlorineMono = Mathf.Round(chlorineMono * 100f) / 100f;
        //displays it in the text box
        textMeshPro_monoChlorine[restarts].GetComponent<TMPro.TextMeshPro>().text = chlorineMono.ToString();
        //advances program
        step++;
    }

    void SetMonoToTotal()
    {
        //mono / total * 100

        float percentMtoT = chlorineMono / chlorineTotal * 100;
        percentMtoT = Mathf.Round(percentMtoT * 10f) / 10f;

        textMeshPro_monoToTotal[restarts].GetComponent<TMPro.TextMeshPro>().text = percentMtoT.ToString() + "%";
        waitTime += 5;
        step++;
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

        currentPipetteAmount = 0;
        pipetteWater.localScale = new Vector3(1, 0, 1);

        clipboard.transform.localScale = Vector3.one * 0.001f;
        clipboardKeyboard.gameObject.SetActive(false);

        pipetValueContainer.localPosition = Vector3.zero;
        pipetValueContainer.localEulerAngles = Vector3.zero;
        pipetValueContainer.localScale = Vector3.one;

        waterSample.localPosition = new Vector3(0.266f, 0.173f, 0.272f);

        pauseButton.SetActive(true);
        pauseMenu.SetActive(false);
        restartButton.SetActive(false);
        playButton.SetActive(true);
        
        step = 0;
    }


    public void TurnTowardsCamera(Transform turningObj, float angleOffset)
    {
        Vector2 facingVector;

        Vector2 direction = new Vector2(-mainCam.transform.forward.x, -mainCam.transform.forward.z);
        float angle = Vector2.SignedAngle(direction, Vector2.up);
        angle += angleOffset;

        facingVector.x = Mathf.Sin(angle * Mathf.Deg2Rad);
        facingVector.y = Mathf.Cos(angle * Mathf.Deg2Rad);

        turningObj.forward = new Vector3(facingVector.x, 0, facingVector.y);
    }


    public bool KnobMovement(GameObject ObjToMov, float knobValue, float speed)
    {
        if (knobRotIndex >= knobValues.Count) { return true; } // If the function is activated but there //are no more values left in the list to go through this function won’t run

        ObjToMov.transform.Rotate(0, 0, speed * Time.deltaTime);
        float knobEulerAngle = ObjToMov.transform.localEulerAngles.z; // Moves only the z axis of the //object
        
        if (Mathf.Abs(knobEulerAngle - knobValue) < 5f)  // This makes sure that the knob will stop //at its desired position
        {
            knobRotIndex++;
        }
        return false;
    }

    public void MoveDrop(Vector3 startPos, Vector3 endPos, GameObject objToMov, float rate)
    {
        objToMov.transform.position = Vector3.Lerp(startPos, endPos, rate);

        RotateObj(dropper, 50, dropper.transform.eulerAngles.z);
    }

    public void RotateObj(GameObject rotObj, float ang, float rotPoint) //Rotates a GameObject that you select by an angle you want to rotate it to and what axis you want to rotate represented by RotPoint
    {
        rotationStart = true;

        if (rotationStart)
        {
            rotObj.transform.Rotate(0, 0, dropperRotSpeed * Time.deltaTime); // This rotates dropper so it properly faces solution

            //float RotPoint = RotObj.transform.eulerAngles.z;
            rotTrans = rotPoint;
            angleRot = ang;


            if (Mathf.Abs(rotPoint - ang) < 5) // Tells program to stop rotating object once the axis reaches the desired angle
            {
                // speed = 0;
                rotationStart = false; // Ensures that rotation stops once dropper is rotated at a certain angle to solution
                if (rotationStart == false) { dropperRotSpeed = 0; }

                if (runCount == 1)
                {
                    WaterDrop(4);
                }
                else
                {
                    WaterDrop(2);
                }

            }

        }
    }

    public void WaterDrop(int dropCount) // Adds drops from dropper to solution
    {

        if (count < dropCount)
        {

            if (Input.GetMouseButtonDown(0)/* && timesClicked == 1*/)
            {
                Debug.Log(timer);
                waitTime += 0.75f;
                timer = 0;
                count++;
                //timesClicked++;

                WaterDroplet = Instantiate(waterDropPart, dropper.transform.position, Quaternion.Euler(-65f, -90f, 0f));


            }

        }

        /*if (count >= dropCount)
        {
            waterDropRun++;
            timer2 = 0;
            timer = 0;
            waterWait = true;
            count = 0;

        }*/

    }

    
    public void DeflectLeft()
    {
        //simulating the deflection to the right
        needlePivot.transform.Rotate(0, 0, -needleSpeed * Time.deltaTime);
    }

    public void DeflectRight()
    {
        //simulating deflecton left ----------------------
        needlePivot.transform.Rotate(0, 0, needleSpeed * Time.deltaTime);
    }


    void SquirtBottleFlash(bool on)
    {
        sqBottleFlash.gameObject.SetActive(on);
    }

    void AgitatorIndicator(bool on)
    {
        agitatorIndicator.SetActive(on);
    }

    void WaterSampleFlash(bool on)
    {
        wSFlash.SetActive(on);
    }

    void LowerKnobFlash(bool on)
    {
        lowerKnobFlash.SetActive(on);
    }

    void UpperKnobFlash(bool on)
    {
        upperKnobFlash.SetActive(on);
    }

    void PipetKnobFlash(bool on)
    {
        pipetKnobFlash.SetActive(on);
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

    
    void AngleTitrator(float newAngle, float changeRate)
    {
        titratorAngleOffset = Mathf.Lerp(titratorAngleOffset, newAngle, changeRate);
    }


    IEnumerator DisplayCurve()
    {
        float startTime = Time.time; //sets start time to seconds in program
        float currentTime;

        while (true)
        {
            currentTime = Time.time - startTime; //sets current time to current time in program

            wholeBreakpointCurve.localScale = Vector3.Lerp(new Vector3(0, 0, 0), new Vector3(15, 15, 15), currentTime / curveDisplayTime);
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

            wholeBreakpointCurve.localScale = Vector3.Lerp(new Vector3(15, 15, 15), new Vector3(0, 0, 0), currentTime / curveDisplayTime);
            //scales object up to 100, the end bit here divides the current time by the animation time. Once they both match, anim over

            if (currentTime > curveDisplayTime)
                //this breaks off the coroutine once its done
                break;

            yield return null;
        }
    }
    
    IEnumerator ExpandTitrator()
    {

        //titrator.gameObject.SetActive(true);
        float currentTime;
        float startTime = Time.time;

        while (true)
        {
            //StartCoroutine(AngleTitrator(0, 0.5f));
            AngleTitrator(0, 0.01f);
            TurnTowardsCamera(titrator.parent, titratorAngleOffset);

            currentTime = Time.time - startTime;

            //titrator.localScale = Vector3.Lerp(titratorScaleStart.localScale, titratorScaleEnd.localScale, currentTime / titratorExpandTime);
            titrator.parent.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, currentTime / titratorExpandTime);

            if (currentTime > titratorExpandTime)
            {
                break;
            }

            yield return null;
        }


    }

    IEnumerator ShrinkTitrator()
    {

        //titrator.gameObject.SetActive(true);
        float currentTime;
        float startTime = Time.time;

        while (true)
        {

            //StartCoroutine(AngleTitrator(0, 0.5f));
            AngleTitrator(0, 0.01f);
            TurnTowardsCamera(titrator.parent, titratorAngleOffset);

            currentTime = Time.time - startTime;


            //titrator.localScale = Vector3.Lerp(titratorScaleEnd.localScale, titratorScaleStart.localScale, currentTime / titratorExpandTime);
            titrator.parent.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, currentTime / titratorExpandTime);

            if (currentTime > titratorExpandTime)
            {
                break;
            }

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

    //lerps clipboard
    IEnumerator ShowClip()
    {
        float startTime = Time.time; //sets start time to seconds in program
        float currentTime;
        while (true)
        {
            currentTime = Time.time - startTime; //sets current time to current time in program

            clipboard.localScale = Vector3.Lerp(Vector3.one * 0.001f, new Vector3(0.015f, 0.015f, 0.015f), currentTime / clipboardDisplayTime);
            //scales object up to 100, the end bit here divides the current time by the animation time. Once they both match, anim over
            if (currentTime > clipboardDisplayTime)
                //this breaks off the coroutine once its done
                break;

            yield return null;
        }

    }


    IEnumerator Display2MLBottle()
    {
        float startTime = Time.time;
        float currentTime;
        while (true)
        {
            currentTime = Time.time - startTime;
            twoMLBottle.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 2, currentTime * 2);

            if (currentTime >= 0.5f)
                break;
            yield return null;
        }
    }

    IEnumerator Hide2MLBottle()
    {
        float startTime = Time.time;
        float currentTime;
        while (true)
        {
            currentTime = Time.time - startTime;
            twoMLBottle.localScale = Vector3.Lerp(Vector3.one * 2, Vector3.zero, currentTime * 2);

            if (currentTime >= 0.5f)
                break;
            yield return null;
        }
    }


    IEnumerator RaiseDropper(Transform dropper, float height)
    {
        float startTime = Time.time;
        float currentTime;

        //float dropperStartHeight = dropper.localPosition.y;
        float dropperStartHeight = dropper.position.y;

        while (true)
        {
            currentTime = Time.time - startTime;
            //dropper.localPosition = Vector3.Lerp(new Vector3(0, dropperStartHeight, 0), new Vector3(0, height, 0), currentTime * 2);
            dropper.position = Vector3.Lerp(new Vector3(twoMLBottle.position.x, dropperStartHeight, twoMLBottle.position.z), new Vector3(twoMLBottle.position.x, height, twoMLBottle.position.z), currentTime * 2);

            if (currentTime >= 0.5f)
                break;
            yield return null;
        }
    }


    IEnumerator ReturnDropper(Transform dropper)
    {
        float startTime = Time.time;
        float currentTime;

        Vector3 dropperStartPos = dropper.localPosition;
        Vector3 startRot = dropper.eulerAngles;

        while (true)
        {
            currentTime = Time.time - startTime;
            dropper.localPosition = Vector3.Lerp(dropperStartPos, new Vector3(0, dropperStartPos.y, 0), currentTime * 2);

            if (currentTime >= 0.5f)
                break;
            yield return null;
        }
        startTime = Time.time;
        while (true)
        {
            currentTime = Time.time - startTime;
            dropper.localPosition = Vector3.Lerp(new Vector3(0, dropperStartPos.y, 0), Vector3.zero, currentTime * 2);
            dropper.eulerAngles = Vector3.Lerp(startRot, Vector3.zero, currentTime * 4);

            if (currentTime >= 0.5f)
                break;
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
