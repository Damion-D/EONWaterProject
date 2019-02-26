using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;
using UnityEngine.SceneManagement;


public class MaintScenarioStory : MonoBehaviour, ITrackableEventHandler {
    public AudioManager audioManager;
    public Utility utility;
    public bool storyHasStarted = false;
    private TrackableBehaviour mTrackableBehaviour;
    private bool AmpButtonPressed = false;
    private bool InfraredButtonPressed = false;
    private bool DripButtonPressed = false;
    public int scenarioIndex;
    public System.Random rnd = new System.Random();
    private bool scenarioFinished = false;
    private double ampValue;
    private double infraValue;
    private double dripValue;
    private bool scenePicked = false;
    private bool valuesPopulated = false;

    public Text DripText;
    public Text InfraText;
    public Text AmpText;
    public bool HitMotorCapsule = false;
    public bool HitBearingCapsule = false;
    public bool HitSealCapsule = false;
    public bool MotorIsCorrect = false;
    public bool BearingIsCorrect = false;
    public bool SealIsCorrect = false;

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
    //public Animation MotorAnimation;
    //public Animation BearingAnimation;
    //public Animation SealAnimation;
    //public Animation ReplacementAnimations;
    public Animator ReplacementAnimator;

    public GameObject pauseMenu;
    // Use this for initialization
    public GameObject Motor;
    public GameObject ReplacementMotor;
    public GameObject MotorIndicator;
    public GameObject MotorLabel;
    public GameObject MotorLabelCapsule;
    public GameObject Bearing;
    public GameObject ReplacementBearing;
    public GameObject Seal;
    public GameObject ReplacementSeal;
    public GameObject RedoButton;

    public Material MotorFadeMaterial;
    public Material BearingFadeMaterial;
    public Material SealFadeMaterial;

    void Start () {
        //Set up the event handler for tracking from Vuforia
        mTrackableBehaviour = GameObject.Find("ImageTarget").GetComponent<TrackableBehaviour>();

        if (mTrackableBehaviour)
            mTrackableBehaviour.RegisterTrackableEventHandler(this);

        Motor.SetActive(true);
        motorStartPosition = ReplacementMotor.gameObject.transform.position;
        motorEndPosition = Motor.gameObject.transform.position;
        bearingStartPosition = ReplacementBearing.gameObject.transform.position;
        bearingEndPosition = Bearing.gameObject.transform.position;
        sealStartPosition = ReplacementSeal.gameObject.transform.position;
        sealEndPosition = Seal.gameObject.transform.position;
        MotorFadeMaterial = GetComponent<Renderer>().material;
        BearingFadeMaterial = GetComponent<Renderer>().material;
        SealFadeMaterial = GetComponent<Renderer>().material;

        //RedoButton.SetActive(false);
        Debug.Log("Turned off redobutton?");

    }

    public double GetDoubleInRange(double minValue, double maxValue)
    {
        double result = rnd.NextDouble() * (maxValue - minValue) + minValue;
        return result;
    }
	
    private IEnumerator MaintenanceScenariosNarrative()
    {

        //play intro narration
        audioManager.PlaySound("MaintScenariosIntro", 4.0f, true, 2);
        yield return null;

        while (true)
        {
            //once intro narration is done, play one of the scenarios
            if (audioManager.GetSound("MaintScenariosIntro").hasCompleted&&scenarioFinished == false)
            {
                if (scenePicked==false)
                {
                    scenarioIndex = rnd.Next(1, 14);
                    //scenarioIndex = 1;
                    scenePicked = true;
                    Debug.Log("Picked your scene!");
                }
                               
                switch (scenarioIndex)
                {
                    //Bad amp range is above 10, Bad Drip range is above 3, excessive is above 30, Bad infra range is above 239 fahrenheit, above average Infra range is above 220F
                    case 1:
                        //Amp is very wrong, Drip is very wrong, Infra is very wrong
                        //RedoButton.SetActive(false);
                        MotorIsCorrect = true;
                        BearingIsCorrect = true;
                        SealIsCorrect = true;
                        Debug.Log("Made it to case 1!");
                        ReplacementAnimator.SetBool("MotorBool", false);
                        if(valuesPopulated == false)
                        {
                            ampValue = GetDoubleInRange(15, 21);
                            dripValue = GetDoubleInRange(35, 40);
                            infraValue = GetDoubleInRange(270, 280);
                            DripText.text += "\n"+System.Math.Round(dripValue).ToString() + " dpm";
                            InfraText.text += "\n"+System.Math.Round(infraValue, 2).ToString() + " F";
                            AmpText.text += "\n"+System.Math.Round(ampValue, 2).ToString() + " A";
                            valuesPopulated = true;
                        }

                       
                        if(HitMotorCapsule&&HitBearingCapsule&&HitSealCapsule)
                        {
                            

                            StartCoroutine("ReplaceMotor");
                            StartCoroutine("ReplaceBearing");
                            StartCoroutine("ReplaceSeal");
                            ampValue = GetDoubleInRange(7, 9);
                            dripValue = GetDoubleInRange(2, 4);
                            infraValue = GetDoubleInRange(190, 225);
                            DripText.text = "Baseline: 3-4dpm"+"\n" + System.Math.Round(dripValue).ToString() + " dpm";
                            InfraText.text = "Baseline: 190F"+"\n" + System.Math.Round(infraValue, 2).ToString() + " F";
                            AmpText.text = "Baseline: 10A"+"\n" + System.Math.Round(ampValue, 2).ToString() + " A";
                            Debug.Log("You finished the scenario!");
                            
                            
                            yield return new WaitForSeconds(5);
                            
                            pauseMenu.SetActive(true);
                            RedoButton.SetActive(true);
                            scenarioFinished = true;
                        }
                        
                        break;

                    case 2:
                        //Amp is right, Drip is very wrong, Infra is very wrong
                        MotorIsCorrect = false;
                        BearingIsCorrect = true;
                        SealIsCorrect = true;
                        if (valuesPopulated == false)
                        {
                            ampValue = GetDoubleInRange(1, 9);
                            dripValue = GetDoubleInRange(35, 40);
                            infraValue = GetDoubleInRange(270, 280);
                            DripText.text += "\n"+System.Math.Round(dripValue).ToString() + " dpm";
                            InfraText.text += "\n" + System.Math.Round(infraValue, 2).ToString() + " F";
                            AmpText.text += "\n" + System.Math.Round(ampValue, 2).ToString() + " A";
                            Debug.Log("Made it to case 2!");
                            valuesPopulated = true;
                        }
                       
                        if (!HitMotorCapsule && HitBearingCapsule && HitSealCapsule)
                        {
                            StartCoroutine("ReplaceSeal");
                            StartCoroutine("ReplaceBearing");
                            dripValue = GetDoubleInRange(2, 4);
                            infraValue = GetDoubleInRange(190, 225);
                            DripText.text = "Baseline: 3-4dpm" + "\n" + System.Math.Round(dripValue).ToString() + " dpm";
                            InfraText.text = "Baseline: 190F" + "\n" + System.Math.Round(infraValue, 2).ToString() + " F";
                            Debug.Log("You finished the scenario!");
                            yield return new WaitForSeconds(5);
                            RedoButton.SetActive(true);
                            pauseMenu.SetActive(true);
                            scenarioFinished = true;
                        }
                       
                        if (HitMotorCapsule&&audioManager.GetSound("ErrorMessage").playing==false)
                        {
                            
                            audioManager.PlaySound("ErrorMessage", 4.0f, true, 2);
                            HitMotorCapsule = false;
                            HitSealCapsule = false;
                            HitBearingCapsule = false;

                        }

                       
                        break;

                    case 3:
                        //Amp is right, Drip is right, Infra is very wrong
                        MotorIsCorrect = false;
                        BearingIsCorrect = true;
                        SealIsCorrect = false;
                        if (valuesPopulated == false)
                        {
                            ampValue = GetDoubleInRange(1, 9);
                            dripValue = GetDoubleInRange(2, 4);
                            infraValue = GetDoubleInRange(270, 280);
                            DripText.text += "\n" + System.Math.Round(dripValue).ToString() + " dpm";
                            InfraText.text += "\n" + System.Math.Round(infraValue, 2).ToString() + " F";
                            AmpText.text += "\n" + System.Math.Round(ampValue, 2).ToString() + " A";
                            Debug.Log("Made it to case 3!");
                            valuesPopulated = true;
                        }
                        
                        if (!HitMotorCapsule && HitBearingCapsule && !HitSealCapsule)
                        {
                            StartCoroutine("ReplaceBearing");
                            infraValue = GetDoubleInRange(190, 225);
                            InfraText.text = "Baseline: 190F" + "\n" + System.Math.Round(infraValue, 2).ToString() + " F";
                            Debug.Log("You finished the scenario!");
                            yield return new WaitForSeconds(5);
                            RedoButton.SetActive(true);
                            pauseMenu.SetActive(true);
                            scenarioFinished = true;
                        }
                        
                        if (HitMotorCapsule == true || HitSealCapsule == true)
                        {
                            if (audioManager.GetSound("ErrorMessage").playing == false)
                            {
                                
                                audioManager.PlaySound("ErrorMessage", 4.0f, true, 2);
                                HitMotorCapsule = false;
                                HitSealCapsule = false;
                                HitBearingCapsule = false;
                            }
                            
                            
                        }
                        break;

                    case 4:
                        //Amp is right, Drip is very wrong, Infra is right
                        MotorIsCorrect = false;
                        BearingIsCorrect = false;
                        SealIsCorrect = true;
                        if (valuesPopulated == false)
                        {
                            ampValue = GetDoubleInRange(1, 9);
                            dripValue = GetDoubleInRange(35, 40);
                            infraValue = GetDoubleInRange(190, 225);
                            DripText.text += "\n" + System.Math.Round(dripValue).ToString() + " dpm";
                            InfraText.text += "\n" + System.Math.Round(infraValue, 2).ToString() + " F";
                            AmpText.text += "\n" + System.Math.Round(ampValue, 2).ToString() + " A";
                            Debug.Log("Made it to case 4!");
                            valuesPopulated = true;
                        }

 
                        if (!HitMotorCapsule && !HitBearingCapsule && HitSealCapsule)
                        {
                            StartCoroutine("ReplaceSeal");
                            dripValue = GetDoubleInRange(2, 4);
                            DripText.text = "Baseline: 3-4dpm" + "\n" + System.Math.Round(dripValue).ToString() + " dpm";
                            Debug.Log("You finished the scenario!");
                            yield return new WaitForSeconds(5);
                            RedoButton.SetActive(true);
                            pauseMenu.SetActive(true);
                            scenarioFinished = true;
                        }
                       
                        if (HitMotorCapsule == true || HitBearingCapsule == true)
                        {
                            if (audioManager.GetSound("ErrorMessage").playing == false)
                            {
                                
                                audioManager.PlaySound("ErrorMessage", 4.0f, true, 2);
                                HitMotorCapsule = false;
                                HitSealCapsule = false;
                                HitBearingCapsule = false;
                            }
                            
                        }
                        break;

                    case 5:
                        //Amp is very wrong, Drip is very wrong, Infra is right
                        MotorIsCorrect = true;
                        BearingIsCorrect = false;
                        SealIsCorrect = true;
                        if (valuesPopulated == false)
                        {
                            ampValue = GetDoubleInRange(20, 21);
                            dripValue = GetDoubleInRange(35, 40);
                            infraValue = GetDoubleInRange(190, 225);
                            DripText.text += "\n" + System.Math.Round(dripValue).ToString() + " dpm";
                            InfraText.text += "\n" + System.Math.Round(infraValue, 2).ToString() + " F";
                            AmpText.text += "\n" + System.Math.Round(ampValue, 2).ToString() + " A";
                            Debug.Log("Made it to case 5!");
                            valuesPopulated = true;
                        }
                       
                        if (HitMotorCapsule && !HitBearingCapsule && HitSealCapsule)
                        {
                            StartCoroutine("ReplaceMotor");
                            StartCoroutine("ReplaceSeal");
                            ampValue = GetDoubleInRange(7, 9);
                            dripValue = GetDoubleInRange(2, 4);
                            DripText.text += "\n" + System.Math.Round(dripValue).ToString() + " dpm";
                            AmpText.text += "\n" + System.Math.Round(ampValue, 2).ToString() + " A";
                            Debug.Log("You finished the scenario!");
                            yield return new WaitForSeconds(5);
                            RedoButton.SetActive(true);
                            pauseMenu.SetActive(true);
                            scenarioFinished = true;
                        }
                       
                        if (HitBearingCapsule)
                        {
                            if (audioManager.GetSound("ErrorMessage").playing == false)
                            {
                                
                                audioManager.PlaySound("ErrorMessage", 4.0f, true, 2);
                                HitMotorCapsule = false;
                                HitSealCapsule = false;
                                HitBearingCapsule = false;
                            }
                            
                        }
                        break;

                    case 6:
                        //Amp is very wrong, Drip is right, Infra is very wrong
                        MotorIsCorrect = true;
                        BearingIsCorrect = true;
                        SealIsCorrect = false;
                        if (valuesPopulated == false)
                        {
                            ampValue = GetDoubleInRange(18, 21);
                            dripValue = GetDoubleInRange(1, 4);
                            infraValue = GetDoubleInRange(270, 280);
                            DripText.text += "\n" + System.Math.Round(dripValue).ToString() + " dpm";
                            InfraText.text += "\n" + System.Math.Round(infraValue, 2).ToString() + " F";
                            AmpText.text += "\n" + System.Math.Round(ampValue, 2).ToString() + " A";
                            Debug.Log("Made it to case 6!");
                            valuesPopulated = true;
                        }
                        
                        if (HitMotorCapsule && HitBearingCapsule && !HitSealCapsule)
                        {
                            StartCoroutine("ReplaceMotor");
                            StartCoroutine("ReplaceBearing");
                            ampValue = GetDoubleInRange(7, 9);
                            infraValue = GetDoubleInRange(190, 225);
                            InfraText.text = "Baseline: 190F" + "\n" + System.Math.Round(infraValue, 2).ToString() + " F";
                            AmpText.text = "Baseline: 10A" + "\n" + System.Math.Round(ampValue, 2).ToString() + " A";
                            Debug.Log("You finished the scenario!");
                            yield return new WaitForSeconds(5);
                            RedoButton.SetActive(true);
                            pauseMenu.SetActive(true);
                            scenarioFinished = true;
                        }
                        if (HitSealCapsule)
                        {
                            if (audioManager.GetSound("ErrorMessage").playing == false)
                            {

                                
                                audioManager.PlaySound("ErrorMessage", 4.0f, true, 2);
                                HitMotorCapsule = false;
                                HitSealCapsule = false;
                                HitBearingCapsule = false;

                            }
                        }
                        break;

                    case 7:
                        //Amp is very wrong, Drip is right, Infra is right
                        MotorIsCorrect = true;
                        BearingIsCorrect = false;
                        SealIsCorrect = false;
                        if (valuesPopulated == false)
                        {
                            ampValue = GetDoubleInRange(18, 21);
                            dripValue = GetDoubleInRange(2, 4);
                            infraValue = GetDoubleInRange(220, 230);
                            DripText.text += "\n" + System.Math.Round(dripValue).ToString() + " dpm";
                            InfraText.text += "\n" + System.Math.Round(infraValue, 2).ToString() + " F";
                            AmpText.text += "\n" + System.Math.Round(ampValue, 2).ToString() + " A";
                            Debug.Log("Made it to case 7!");
                            valuesPopulated = true;
                        }
                        if (HitMotorCapsule)
                        {
                            if (audioManager.GetSound("CorrectAnswer").playing == false)
                            {
                                audioManager.PlaySound("CorrectAnswer", 4.0f, true, 2);
                            }
                            if (audioManager.GetSound("CorrectAnswer").hasCompleted)
                            {
                                audioManager.StopSound("CorrectAnswer", true, 2);
                            }
                        }
                        if (HitMotorCapsule && !HitBearingCapsule && !HitSealCapsule)
                        {
                            StartCoroutine("ReplaceMotor");
                            ampValue = GetDoubleInRange(7, 9);
                            AmpText.text = "Baseline: 10A" + "\n" + System.Math.Round(ampValue, 2).ToString() + " A";
                            Debug.Log("You finished the scenario!");
                            yield return new WaitForSeconds(5);
                            RedoButton.SetActive(true);
                            pauseMenu.SetActive(true);
                            scenarioFinished = true;
                        }
                        if (HitBearingCapsule == true || HitSealCapsule == true)
                        {
                            if (audioManager.GetSound("ErrorMessage").playing == false)
                            {
                                                               
                                audioManager.PlaySound("ErrorMessage", 4.0f, true, 2);
                                HitMotorCapsule = false;
                                HitSealCapsule = false;
                                HitBearingCapsule = false;

                            }
                        }
                        break;

                    case 8:
                        //Amp is slightly wrong, Drip is slightly wrong, Infra is slightly wrong
                        RedoButton.SetActive(false);
                        MotorIsCorrect = true;
                        BearingIsCorrect = true;
                        SealIsCorrect = true;
                        Debug.Log("Made it to case 8!");
                        ReplacementAnimator.SetBool("MotorBool", false);
                        if (valuesPopulated == false)
                        {
                            ampValue = GetDoubleInRange(11.5, 12);
                            dripValue = GetDoubleInRange(5, 9);
                            infraValue = GetDoubleInRange(235, 239);
                            DripText.text += "\n" + System.Math.Round(dripValue).ToString() + " dpm";
                            InfraText.text += "\n" + System.Math.Round(infraValue, 2).ToString() + " F";
                            AmpText.text += "\n" + System.Math.Round(ampValue, 2).ToString() + " A";
                            valuesPopulated = true;
                        }


                        if (HitMotorCapsule && HitBearingCapsule && HitSealCapsule)
                        {
                            if (audioManager.GetSound("MonitorReadings").playing == false)
                            {
                                audioManager.PlaySound("MonitorReadings", 4.0f, true, 2);
                            }
                            if (audioManager.GetSound("MonitorReadings").hasCompleted)
                            {
                                audioManager.StopSound("MonitorReadings", true, 2);
                            }

                            StartCoroutine("ReplaceMotor");
                            StartCoroutine("ReplaceBearing");
                            StartCoroutine("ReplaceSeal");
                            ampValue = GetDoubleInRange(7, 9);
                            dripValue = GetDoubleInRange(2, 4);
                            infraValue = GetDoubleInRange(190, 225);
                            DripText.text = "Baseline: 3-4dpm" + "\n" + System.Math.Round(dripValue).ToString() + " dpm";
                            InfraText.text = "Baseline: 190F" + "\n" + System.Math.Round(infraValue, 2).ToString() + " F";
                            AmpText.text = "Baseline: 10A" + "\n" + System.Math.Round(ampValue, 2).ToString() + " A";
                            Debug.Log("You finished the scenario!");
                            yield return new WaitForSeconds(5);
                            RedoButton.SetActive(true);
                            pauseMenu.SetActive(true);
                            scenarioFinished = true;
                        }

                        break;

                    case 9:
                        //Amp is right, Drip is slightly wrong, Infra is slightly wrong
                        MotorIsCorrect = false;
                        BearingIsCorrect = true;
                        SealIsCorrect = true;
                        if (valuesPopulated == false)
                        {
                            ampValue = GetDoubleInRange(1, 9);
                            dripValue = GetDoubleInRange(5, 6);
                            infraValue = GetDoubleInRange(235, 239);
                            DripText.text += "\n" + System.Math.Round(dripValue).ToString() + " dpm";
                            InfraText.text += "\n" + System.Math.Round(infraValue, 2).ToString() + " F";
                            AmpText.text += "\n" + System.Math.Round(ampValue, 2).ToString() + " A";
                            Debug.Log("Made it to case 9!");
                            valuesPopulated = true;
                        }

                        if (!HitMotorCapsule && HitBearingCapsule && HitSealCapsule)
                        {
                            if (audioManager.GetSound("MonitorReadings").playing == false)
                            {
                                audioManager.PlaySound("MonitorReadings", 4.0f, true, 2);
                            }
                            if (audioManager.GetSound("MonitorReadings").hasCompleted)
                            {
                                audioManager.StopSound("MonitorReadings", true, 2);
                            }
                            StartCoroutine("ReplaceSeal");
                            StartCoroutine("ReplaceBearing");
                            dripValue = GetDoubleInRange(2, 4);
                            infraValue = GetDoubleInRange(190, 225);
                            DripText.text = "Baseline: 3-4dpm" + "\n" + System.Math.Round(dripValue).ToString() + " dpm";
                            InfraText.text = "Baseline: 190F" + "\n" + System.Math.Round(infraValue, 2).ToString() + " F";
                            Debug.Log("You finished the scenario!");
                            yield return new WaitForSeconds(5);
                            RedoButton.SetActive(true);
                            pauseMenu.SetActive(true);
                            scenarioFinished = true;
                        }

                        if (HitMotorCapsule && audioManager.GetSound("ErrorMessage").playing == false)
                        {

                            audioManager.PlaySound("ErrorMessage", 4.0f, true, 2);
                            HitMotorCapsule = false;
                            HitSealCapsule = false;
                            HitBearingCapsule = false;

                        }


                        break;

                    case 10:
                        //Amp is right, Drip is right, Infra is slightly wrong
                        MotorIsCorrect = false;
                        BearingIsCorrect = true;
                        SealIsCorrect = false;
                        if (valuesPopulated == false)
                        {
                            ampValue = GetDoubleInRange(1, 9);
                            dripValue = GetDoubleInRange(2, 4);
                            infraValue = GetDoubleInRange(235, 239);
                            DripText.text += "\n" + System.Math.Round(dripValue).ToString() + " dpm";
                            InfraText.text += "\n" + System.Math.Round(infraValue, 2).ToString() + " F";
                            AmpText.text += "\n" + System.Math.Round(ampValue, 2).ToString() + " A";
                            Debug.Log("Made it to case 10!");
                            valuesPopulated = true;
                        }

                        if (!HitMotorCapsule && HitBearingCapsule && !HitSealCapsule)
                        {
                            if (audioManager.GetSound("MonitorReadings").playing == false)
                            {
                                audioManager.PlaySound("MonitorReadings", 4.0f, true, 2);
                            }
                            if (audioManager.GetSound("MonitorReadings").hasCompleted)
                            {
                                audioManager.StopSound("MonitorReadings", true, 2);
                            }
                            StartCoroutine("ReplaceBearing");
                            infraValue = GetDoubleInRange(190, 225);
                            InfraText.text = "Baseline: 190F" + "\n" + System.Math.Round(infraValue, 2).ToString() + " F";
                            Debug.Log("You finished the scenario!");
                            yield return new WaitForSeconds(5);
                            RedoButton.SetActive(true);
                            pauseMenu.SetActive(true);
                            scenarioFinished = true;
                        }

                        if (HitMotorCapsule == true || HitSealCapsule == true)
                        {
                            if (audioManager.GetSound("ErrorMessage").playing == false)
                            {

                                audioManager.PlaySound("ErrorMessage", 4.0f, true, 2);
                                HitMotorCapsule = false;
                                HitSealCapsule = false;
                                HitBearingCapsule = false;
                            }


                        }
                        break;

                    case 11:
                        //Amp is right, Drip is slightly wrong, Infra is right
                        MotorIsCorrect = false;
                        BearingIsCorrect = false;
                        SealIsCorrect = true;
                        if (valuesPopulated == false)
                        {
                            ampValue = GetDoubleInRange(1, 9);
                            dripValue = GetDoubleInRange(5, 6);
                            infraValue = GetDoubleInRange(190, 225);
                            DripText.text += "\n" + System.Math.Round(dripValue).ToString() + " dpm";
                            InfraText.text += "\n" + System.Math.Round(infraValue, 2).ToString() + " F";
                            AmpText.text += "\n" + System.Math.Round(ampValue, 2).ToString() + " A";
                            Debug.Log("Made it to case 11!");
                            valuesPopulated = true;
                        }


                        if (!HitMotorCapsule && !HitBearingCapsule && HitSealCapsule)
                        {
                            if (audioManager.GetSound("MonitorReadings").playing == false)
                            {
                                audioManager.PlaySound("MonitorReadings", 4.0f, true, 2);
                            }
                            if (audioManager.GetSound("MonitorReadings").hasCompleted)
                            {
                                audioManager.StopSound("MonitorReadings", true, 2);
                            }
                            StartCoroutine("ReplaceSeal");
                            dripValue = GetDoubleInRange(2, 4);
                            DripText.text = "Baseline: 3-4dpm" + "\n" + System.Math.Round(dripValue).ToString() + " dpm";
                            Debug.Log("You finished the scenario!");
                            yield return new WaitForSeconds(5);
                            RedoButton.SetActive(true);
                            pauseMenu.SetActive(true);
                            scenarioFinished = true;
                        }

                        if (HitMotorCapsule == true || HitBearingCapsule == true)
                        {
                            if (audioManager.GetSound("ErrorMessage").playing == false)
                            {

                                audioManager.PlaySound("ErrorMessage", 4.0f, true, 2);
                                HitMotorCapsule = false;
                                HitSealCapsule = false;
                                HitBearingCapsule = false;
                            }

                        }
                        break;

                    case 12:
                        //Amp is slightly wrong, Drip is slightly wrong, Infra is right
                        MotorIsCorrect = true;
                        BearingIsCorrect = false;
                        SealIsCorrect = true;
                        if (valuesPopulated == false)
                        {
                            ampValue = GetDoubleInRange(11.5, 12);
                            dripValue = GetDoubleInRange(5, 6);
                            infraValue = GetDoubleInRange(190, 225);
                            DripText.text += "\n" + System.Math.Round(dripValue).ToString() + " dpm";
                            InfraText.text += "\n" + System.Math.Round(infraValue, 2).ToString() + " F";
                            AmpText.text += "\n" + System.Math.Round(ampValue, 2).ToString() + " A";
                            Debug.Log("Made it to case 12!");
                            valuesPopulated = true;
                        }

                        if (HitMotorCapsule && !HitBearingCapsule && HitSealCapsule)
                        {
                            if (audioManager.GetSound("MonitorReadings").playing == false)
                            {
                                audioManager.PlaySound("MonitorReadings", 4.0f, true, 2);
                            }
                            if (audioManager.GetSound("MonitorReadings").hasCompleted)
                            {
                                audioManager.StopSound("MonitorReadings", true, 2);
                            }
                            StartCoroutine("ReplaceMotor");
                            StartCoroutine("ReplaceSeal");
                            ampValue = GetDoubleInRange(7, 9);
                            dripValue = GetDoubleInRange(2, 4);
                            DripText.text = "Baseline: 3-4dpm" + "\n" + System.Math.Round(dripValue).ToString() + " dpm";
                            AmpText.text = "Baseline: 10A" + "\n" + System.Math.Round(ampValue, 2).ToString() + " A";
                            Debug.Log("You finished the scenario!");
                            yield return new WaitForSeconds(5);
                            RedoButton.SetActive(true);
                            pauseMenu.SetActive(true);
                            scenarioFinished = true;
                        }

                        if (HitBearingCapsule)
                        {
                            if (audioManager.GetSound("ErrorMessage").playing == false)
                            {

                                audioManager.PlaySound("ErrorMessage", 4.0f, true, 2);
                                HitMotorCapsule = false;
                                HitSealCapsule = false;
                                HitBearingCapsule = false;
                            }

                        }
                        break;

                    case 13:
                        //Amp is slightly wrong, Drip is right, Infra is slightly wrong
                        MotorIsCorrect = true;
                        BearingIsCorrect = true;
                        SealIsCorrect = false;
                        if (valuesPopulated == false)
                        {
                            ampValue = GetDoubleInRange(11.5, 12);
                            dripValue = GetDoubleInRange(1, 4);
                            infraValue = GetDoubleInRange(235, 239);
                            DripText.text += "\n" + System.Math.Round(dripValue).ToString() + " dpm";
                            InfraText.text += "\n" + System.Math.Round(infraValue, 2).ToString() + " F";
                            AmpText.text += "\n" + System.Math.Round(ampValue, 2).ToString() + " A";
                            Debug.Log("Made it to case 13!");
                            valuesPopulated = true;
                        }

                        if (HitMotorCapsule && HitBearingCapsule && !HitSealCapsule)
                        {
                            if (audioManager.GetSound("MonitorReadings").playing == false)
                            {
                                audioManager.PlaySound("MonitorReadings", 4.0f, true, 2);
                            }
                            if (audioManager.GetSound("MonitorReadings").hasCompleted)
                            {
                                audioManager.StopSound("MonitorReadings", true, 2);
                            }
                            StartCoroutine("ReplaceMotor");
                            StartCoroutine("ReplaceBearing");
                            ampValue = GetDoubleInRange(7, 9);
                            infraValue = GetDoubleInRange(190, 225);
                            InfraText.text = "Baseline: 190F" + "\n" + System.Math.Round(infraValue, 2).ToString() + " F";
                            AmpText.text = "Baseline: 10A" + "\n" + System.Math.Round(ampValue, 2).ToString() + " A";
                            Debug.Log("You finished the scenario!");
                            yield return new WaitForSeconds(5);
                            RedoButton.SetActive(true);
                            pauseMenu.SetActive(true);
                            scenarioFinished = true;
                        }
                        if (HitSealCapsule)
                        {
                            if (audioManager.GetSound("ErrorMessage").playing == false)
                            {


                                audioManager.PlaySound("ErrorMessage", 4.0f, true, 2);
                                HitMotorCapsule = false;
                                HitSealCapsule = false;
                                HitBearingCapsule = false;

                            }
                        }
                        break;

                    case 14:
                        //Amp is slightly wrong, Drip is right, Infra is right
                        MotorIsCorrect = true;
                        BearingIsCorrect = false;
                        SealIsCorrect = false;
                        if (valuesPopulated == false)
                        {
                            ampValue = GetDoubleInRange(11.5, 12);
                            dripValue = GetDoubleInRange(2, 4);
                            infraValue = GetDoubleInRange(190, 215);
                            DripText.text += "\n" + System.Math.Round(dripValue).ToString() + " dpm";
                            InfraText.text += "\n" + System.Math.Round(infraValue, 2).ToString() + " F";
                            AmpText.text += "\n" + System.Math.Round(ampValue, 2).ToString() + " A";
                            Debug.Log("Made it to case 14!");
                            valuesPopulated = true;
                        }
                        if (HitMotorCapsule)
                        {
                            if (audioManager.GetSound("MonitorReadings").playing == false)
                            {
                                audioManager.PlaySound("MonitorReadings", 4.0f, true, 2);
                            }
                            if (audioManager.GetSound("MonitorReadings").hasCompleted)
                            {
                                audioManager.StopSound("MonitorReadings", true, 2);
                            }
                        }
                        if (HitMotorCapsule && !HitBearingCapsule && !HitSealCapsule)
                        {
                            StartCoroutine("ReplaceMotor");
                            ampValue = GetDoubleInRange(7, 9);
                            AmpText.text = "Baseline: 10A" + "\n" + System.Math.Round(ampValue, 2).ToString() + " A";
                            Debug.Log("You finished the scenario!");
                            yield return new WaitForSeconds(5);
                            RedoButton.SetActive(true);
                            pauseMenu.SetActive(true);
                            scenarioFinished = true;
                        }
                        if (HitBearingCapsule == true || HitSealCapsule == true)
                        {
                            if (audioManager.GetSound("ErrorMessage").playing == false)
                            {

                                audioManager.PlaySound("ErrorMessage", 4.0f, true, 2);
                                HitMotorCapsule = false;
                                HitSealCapsule = false;
                                HitBearingCapsule = false;

                            }
                        }
                        break;
                }
                
            }

            if(scenarioFinished)
            {
                Debug.Log("Made it inside scenarioFinished statement");
                RedoButton.SetActive(true);
                break;
            }

            yield return null;
            
        }

    }
	// Update is called once per frame
	void Update () {
      //Debug.Log("HitMotorCapsule is: "+HitMotorCapsule);
        //Debug.Log("HitBearingCapsule is: " + HitBearingCapsule);
        //Debug.Log("HitSealCapsule is: " + HitSealCapsule);
    }

    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        if ((newStatus == TrackableBehaviour.Status.TRACKED || newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED) && previousStatus == TrackableBehaviour.Status.NO_POSE && storyHasStarted == false)
        {
            storyHasStarted = true;
            StartCoroutine(MaintenanceScenariosNarrative());
        }
    }

    private IEnumerator ReplaceMotor()
    {
        for(float f = 1f; f>=0; f -= 0.1f)
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
                    MotorIndicator.SetActive(true);
                    MotorLabel.SetActive(true);
                    MotorLabelCapsule.SetActive(true);
                    //pauseMenu.SetActive(true);
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
                    //pauseMenu.SetActive(true);
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
                    //pauseMenu.SetActive(true);
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

    public void RedoScenario()
    {
        SceneManager.LoadScene(11);
    }
}
