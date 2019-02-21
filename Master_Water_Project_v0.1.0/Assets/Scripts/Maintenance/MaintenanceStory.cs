using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using UnityEngine.UI;

public class MaintenanceStory : MonoBehaviour, ITrackableEventHandler {

    public AudioManager audioManager;
    public System.Random rnd = new System.Random();
    public Text DripText;
    public Text InfraText;
    public Text AmpText;

    public GameObject pauseMenu;
    public GameObject pauseButton;
    public Utility utility;

    private double ampValue;
    private double infraValue;
    private double dripValue;

    private bool storyHasStarted = false;
    private TrackableBehaviour mTrackableBehaviour;
    private bool AmpButtonPressed = false;
    private bool InfraredButtonPressed = false;
    private bool DripButtonPressed = false;
    private bool AmpDone = false;
    private bool InfraDone = false;
    private bool DripDone = false;

    public bool HitMotorCapsule = false;
    public bool HitBearingCapsule = false;
    public bool HitSealCapsule = false;
    public bool MotorIsCorrect = false;
    public bool BearingIsCorrect = false;

    private bool ReplaceMotorCoroutineStarted = false;
    private bool ReplaceBearingCoroutineStarted = false;
    private bool ReplaceSealCoroutineStarted = false;
    private bool SealCoroutineFinished = false;
    private bool AllThreeFinished = false;

    private bool AmpValuesPopulated = false;
    private bool BearingValuesPopulated = false;
    private bool SealValuesPopulated = false;

    private bool isLerping = true;
    private float timeStartedLerping;
    private bool notStartedLerpingYet = true;
    public float timeTakenDuringLerp = 1f;
    private float timeSinceStarted;
    private float percentageComplete;
    private Vector3 motorStartPosition;
    private Vector3 motorEndPosition;
    private Vector3 bearingStartPosition;
    private Vector3 bearingEndPosition;
    private Vector3 sealStartPosition;
    private Vector3 sealEndPosition;

    public GameObject Motor;
    public GameObject ReplacementMotor;
    public GameObject Bearing;
    public GameObject ReplacementBearing;
    public GameObject Seal;
    public GameObject ReplacementSeal;

    public Material MotorFadeMaterial;
    public Material BearingFadeMaterial;
    public Material SealFadeMaterial;


    // Use this for initialization
    void Start()
    {
        //Set up the event handler for tracking from Vuforia
        mTrackableBehaviour = GameObject.Find("ImageTarget").GetComponent<TrackableBehaviour>();

        if (mTrackableBehaviour)
            mTrackableBehaviour.RegisterTrackableEventHandler(this);

        motorStartPosition = ReplacementMotor.gameObject.transform.position;
        motorEndPosition = Motor.gameObject.transform.position;
        bearingStartPosition = ReplacementBearing.gameObject.transform.position;
        bearingEndPosition = Bearing.gameObject.transform.position;
        sealStartPosition = ReplacementSeal.gameObject.transform.position;
        sealEndPosition = Seal.gameObject.transform.position;
        MotorFadeMaterial = GetComponent<Renderer>().material;
        BearingFadeMaterial = GetComponent<Renderer>().material;
        SealFadeMaterial = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("HitMotorCapsule is: "+HitMotorCapsule);
        //Debug.Log("HitBearingCapsule is: " + HitBearingCapsule);
        //Debug.Log("HitSealCapsule is: " + HitSealCapsule);
        //Debug.Log("Drip Explanation is finished boolean is: " + audioManager.GetSound("DripExplanation").hasCompleted);
        //Debug.Log("Infradone is: " + InfraDone);
        //Debug.Log("Ampdone is: " + AmpDone);
        //Debug.Log("Dripdone is: " + DripDone);
    }

    public double GetDoubleInRange(double minValue, double maxValue)
    {
        double result = rnd.NextDouble() * (maxValue - minValue) + minValue;
        return result;
    }

    private IEnumerator MaintenanceNarrative()
    {
        //Play intro narration

        audioManager.PlaySound("Intro", 1.0f, true, 2);
        yield return null;

        while (true)
        {
            if (audioManager.GetSound("Intro").hasCompleted && AmpButtonPressed)
            {
                audioManager.PlaySound("AmpExplanation", 4.0f, true, 2);
                AmpButtonPressed = false;
                HitMotorCapsule = false;
                

            }
            else if (audioManager.GetSound("Intro").hasCompleted && InfraredButtonPressed)
            {
                audioManager.PlaySound("InfraredExplanation", 4.0f, true, 2);
                InfraredButtonPressed = false;
                HitBearingCapsule = false;
            }
            else if (audioManager.GetSound("Intro").hasCompleted && DripButtonPressed)
            {
                audioManager.PlaySound("DripExplanation", 4.0f, true, 2);
                DripButtonPressed = false;
                HitSealCapsule = false;
            }
            else if (audioManager.GetSound("AmpExplanation").hasCompleted&&HitMotorCapsule&&ReplaceMotorCoroutineStarted==false)
            {
                ReplaceMotorCoroutineStarted = true;
                audioManager.PlaySound("CorrectSelection", 4.0f, true, 2);
                StartCoroutine("ReplaceMotor");
                AmpDone = true;
            }
            else if (audioManager.GetSound("InfraredExplanation").hasCompleted && HitBearingCapsule&&ReplaceBearingCoroutineStarted==false)
            {
                ReplaceBearingCoroutineStarted = true;
                audioManager.PlaySound("CorrectSelection", 4.0f, true, 2);
                StartCoroutine("ReplaceBearing");
                InfraDone = true;
            }
            else if (audioManager.GetSound("DripExplanation").hasCompleted)
            {
                //ReplaceSealCoroutineStarted = true;
                //audioManager.PlaySound("CorrectSelection");
                //StartCoroutine("ReplaceSeal");
                DripDone = true;
                
            }
            if (DripDone && InfraDone && AmpDone&&AllThreeFinished==false)
            {
                Debug.Log("Playing epilogue!");
                audioManager.PlaySound("Epilogue");
                AllThreeFinished = true;
                
            }
            if (audioManager.GetSound("Epilogue").hasCompleted)
            {
                yield return new WaitForSeconds(2);
                pauseMenu.SetActive(true);
                pauseButton.SetActive(false);
                utility.PauseAll();
                Debug.Log("Done!");
                break;
            }

            
            yield return null;
        }



    }

    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        if ((newStatus == TrackableBehaviour.Status.TRACKED || newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED) && previousStatus == TrackableBehaviour.Status.NO_POSE && storyHasStarted == false)
        {
            storyHasStarted = true;
            StartCoroutine(MaintenanceNarrative());
        }
    }

    public void AmperageStory()
    {
        if (AmpValuesPopulated == false)
        {
            ampValue = GetDoubleInRange(11, 21);
            dripValue = GetDoubleInRange(12, 14);
            infraValue = GetDoubleInRange(245, 250);
            //DripText.text += "\n" + System.Math.Round(dripValue, 2).ToString() + " dpm";
            //InfraText.text += "\n" + System.Math.Round(infraValue, 2).ToString() + " F";
            AmpText.text += "\n" + System.Math.Round(ampValue, 2).ToString() + " A";
            Debug.Log("Made it to case 7!");
            AmpValuesPopulated = true;
        }
        AmpButtonPressed = true;
    }

    public void InfraredStory()
    {
        if (BearingValuesPopulated == false)
        {
            ampValue = GetDoubleInRange(11, 21);
            dripValue = GetDoubleInRange(12, 14);
            infraValue = GetDoubleInRange(245, 250);
            //DripText.text += "\n" + System.Math.Round(dripValue, 2).ToString() + " dpm";
            InfraText.text += "\n" + System.Math.Round(infraValue, 2).ToString() + " F";
            //AmpText.text += "\n" + System.Math.Round(ampValue, 2).ToString() + " A";
            Debug.Log("Made it to case 7!");
            BearingValuesPopulated = true;
        }
        InfraredButtonPressed = true;
    }

    public void DripStory()
    {
        if (SealValuesPopulated == false)
        {
            ampValue = GetDoubleInRange(11, 21);
            dripValue = GetDoubleInRange(12, 14);
            infraValue = GetDoubleInRange(245, 250);
            DripText.text += "\n" + System.Math.Round(dripValue).ToString() + " dpm";
            //InfraText.text += "\n" + System.Math.Round(infraValue, 2).ToString() + " F";
            //AmpText.text += "\n" + System.Math.Round(ampValue, 2).ToString() + " A";
            Debug.Log("Made it to case 7!");
            SealValuesPopulated = true;
        }
        DripButtonPressed = true;
    }

    private IEnumerator ReplaceMotor()
    {
        for (float f = 1f; f >= 0; f -= 0.1f)
        {
            if (f <= 0.1f)
            {
                Motor.SetActive(false);
            }
            Color c = MotorFadeMaterial.color;
            c.a = f;
            MotorFadeMaterial.color = c;

            yield return null;
        }
        while (true)
        {
            if (notStartedLerpingYet)
            {
                ReplacementMotor.SetActive(true);
                isLerping = true;
                timeStartedLerping = Time.time;
                notStartedLerpingYet = false;
            }

            if (isLerping)
            {
                timeSinceStarted = Time.time - timeStartedLerping;
                percentageComplete = timeSinceStarted / timeTakenDuringLerp;
                ReplacementMotor.gameObject.transform.position = Vector3.Lerp(motorStartPosition, motorEndPosition, percentageComplete);
                if (percentageComplete >= 1.0f)
                {
                    isLerping = false;
                    notStartedLerpingYet = true;
                    ReplacementMotor.gameObject.transform.position = motorEndPosition;
                    Color c = MotorFadeMaterial.color;
                    c.a = 1f;
                    MotorFadeMaterial.color = c;
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

    private IEnumerator ReplaceBearing()
    {
        for (float f = 1f; f >= 0; f -= 0.1f)
        {
            if (f <= 0.1f)
            {
                Bearing.SetActive(false);
            }
            Color c = BearingFadeMaterial.color;
            c.a = f;
            BearingFadeMaterial.color = c;

            yield return null;
        }
        while (true)
        {
            if (notStartedLerpingYet)
            {
                ReplacementBearing.SetActive(true);
                isLerping = true;
                timeStartedLerping = Time.time;
                notStartedLerpingYet = false;
            }

            if (isLerping)
            {
                timeSinceStarted = Time.time - timeStartedLerping;
                percentageComplete = timeSinceStarted / timeTakenDuringLerp;
                ReplacementBearing.gameObject.transform.position = Vector3.Lerp(bearingStartPosition, bearingEndPosition, percentageComplete);
                if (percentageComplete >= 1.0f)
                {
                    isLerping = false;
                    notStartedLerpingYet = true;
                    ReplacementBearing.gameObject.transform.position = bearingEndPosition;
                    Color c = BearingFadeMaterial.color;
                    c.a = 1f;
                    BearingFadeMaterial.color = c;
                    //BearingIndicator.SetActive(true);
                    //BearingLabel.SetActive(true);
                    //BearingLabelCapsule.SetActive(true);
                    //Motor.SetActive(true);
                    break;
                }
            }

            yield return null;
        }

    }

    private IEnumerator ReplaceSeal()
    {
        for (float f = 1f; f >= 0; f -= 0.1f)
        {
            if (f <= 0.1f)
            {
                Seal.SetActive(false);
            }
            Color c = SealFadeMaterial.color;
            c.a = f;
            SealFadeMaterial.color = c;

            yield return null;
        }
        while (true)
        {
            if (notStartedLerpingYet)
            {
                ReplacementSeal.SetActive(true);
                isLerping = true;
                timeStartedLerping = Time.time;
                notStartedLerpingYet = false;
            }

            if (isLerping)
            {
                timeSinceStarted = Time.time - timeStartedLerping;
                percentageComplete = timeSinceStarted / timeTakenDuringLerp;
                ReplacementSeal.gameObject.transform.position = Vector3.Lerp(sealStartPosition, sealEndPosition, percentageComplete);
                if (percentageComplete >= 1.0f)
                {
                    isLerping = false;
                    notStartedLerpingYet = true;
                    ReplacementSeal.gameObject.transform.position = sealEndPosition;
                    Color c = SealFadeMaterial.color;
                    c.a = 1f;
                    SealFadeMaterial.color = c;
                    SealCoroutineFinished = true;
                    //SealIndicator.SetActive(true);
                    //SealLabel.SetActive(true);
                    //SealLabelCapsule.SetActive(true);
                    //Motor.SetActive(true);
                    break;
                }
            }

            yield return null;
        }

    }
}
