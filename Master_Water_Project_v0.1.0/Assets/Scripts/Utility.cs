using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using Vuforia;

public class Utility : MonoBehaviour, ITrackableEventHandler {

    [Header("References")]
    public AudioManager audioManager;
    public GameObject storyManager;
    public GameObject components;
    public GameObject pauseButton;
    public GameObject playButton;
    public Material inletHighlight;
    public Material outletHighlight;
    public MaintScenarioStory maintScenarioStory;
    public MaintenanceStory mainStory;
    public PPEStory ppeStory;
    public LOTOPumpStory lotopumpstory;
    public LOTOValveStory lotovalvestory;

    [Header("Settings")]
    public float highlightSpeed = 2;
    public bool initializeUI = true;

    [HideInInspector] public bool canSelectObjects = true;
    [HideInInspector] public bool valveSelected;
    [HideInInspector] public string correctValve;

    private TrackableBehaviour mTrackableBehaviour;
    private bool isShuttingDown = false;
    private bool storyHasStarted = false;
    private bool tracking;

    private void Awake()
    {
        //Set up the event handler for tracking from Vuforia
        mTrackableBehaviour = GameObject.Find("ImageTarget").GetComponent<TrackableBehaviour>();

        if (mTrackableBehaviour)
            mTrackableBehaviour.RegisterTrackableEventHandler(this);

        //Turns on all UI Components so that any script under components or its children will be able to run necessary code
        if (initializeUI)
            for (int i = 0; i < components.transform.childCount; i++)
                components.transform.GetChild(i).gameObject.SetActive(true);
    }

    private void Start()
    {
        //Disables every UI Component after being enabled in Awake (to allow code in all awakes under the components to run)
        MenusOff();
        valveSelected = false;
        inletHighlight.color = new Color(inletHighlight.color.r, inletHighlight.color.g, inletHighlight.color.b, 0);
        outletHighlight.color = new Color(outletHighlight.color.r, outletHighlight.color.g, outletHighlight.color.b, 0);
    }

    private void Update()
    {
        //Actively checks if the user is clicking (in the editor and on mobile)
        if (((Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began) || Input.GetMouseButtonDown(0)) && canSelectObjects)
        {
            Ray ray;

            //Ensures that touch works in both the editor and when deployed to mobile
            if (Application.platform == RuntimePlatform.WindowsEditor)
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            else
                ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

            RaycastHit hit;
            
            //Checks if the user is not pressing on UI
            if (!IsPointerOverUIObject())
            {
                
                if (Physics.Raycast(ray, out hit))
                {
                    //Debug.Log(hit.collider.name);
                    Debug.Log(hit.collider.gameObject.tag);
                    ppeStory.hitName = hit.collider.gameObject.name;
                    lotopumpstory.hitName = hit.collider.gameObject.name;
                    lotovalvestory.hitName = hit.collider.gameObject.name;
                    Debug.Log("LOTOValve hitName is: "+lotovalvestory.hitName);
                    //Debug.Log("PPEStory hitname is: " + ppeStory.hitName);
                    //Debug.Log(hit.collider.gameObject.name);
                    //these booleans confirm the ray has hit one of the part labels
                    //and selected that part as the part to 'fix'; they originate in the 'mainScenarioStory' script
                    if (hit.collider.gameObject.tag == "MotorCapsule")
                    {
                        Debug.Log("Hit Motor Capsule!");
                        maintScenarioStory.HitMotorCapsule = true;
                        mainStory.HitMotorCapsule = true;
                        if (maintScenarioStory.MotorIsCorrect)
                        {
                            audioManager.PlaySound("CorrectAnswer", 0.3f, true, 2);
                        }
                    }
                    if (hit.collider.gameObject.tag == "BearingCapsule")
                    {
                        maintScenarioStory.HitBearingCapsule = true;
                        mainStory.HitBearingCapsule = true;
                        if (maintScenarioStory.BearingIsCorrect)
                        {
                            audioManager.PlaySound("CorrectAnswer", 0.3f, true, 2);
                        }
                    }
                    if (hit.collider.gameObject.tag == "SealCapsule")
                    {
                        maintScenarioStory.HitSealCapsule = true;
                        mainStory.HitSealCapsule = true;
                        if (maintScenarioStory.SealIsCorrect)
                        {
                            audioManager.PlaySound("CorrectAnswer", 0.3f, true, 2);
                        }
                    }

                    /*if (hit.collider.gameObject.tag == "Boots")
                    {
                        ppeStory.hitBoots = true;
                        ppeStory.hitName = "Boots";
                        Debug.Log("Hit boots!");
                        Debug.Log(ppeStory.hitName);
                    }*/
                    //Turns off all ui elements except the one directly linked to the selected object
                    MenusOff();
                    hit.collider.gameObject.GetComponent<PartID>().uIComponent.SetActive(true);

                    
                }
                else
                {
                    MenusOff();
                }
            }

          
        }
        
        //Debug.Log("HitMotorCapsule is: " + maintScenarioStory.HitMotorCapsule);
    }

    //Stops the UI from dissapearing on tap (temporary hotfix)
    private bool IsPointerOverUIObject() {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    //Keeps track of what the current tracking state is from Vuforia
    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        //Only resumes if the previous state was not tracking
        if ((newStatus == TrackableBehaviour.Status.TRACKED || newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED) && previousStatus == TrackableBehaviour.Status.NO_POSE)
        {
            tracking = true;
            for (int i = 0; i < components.transform.parent.childCount; i++)
            {
                if(components.transform.parent.GetChild(i).gameObject.tag!="DontTurnOff")
                {
                    components.transform.parent.GetChild(i).gameObject.SetActive(false);
                    pauseButton.SetActive(true);
                }
                
            }

            ResumeAll();

            //Start story once
            if (!storyHasStarted)
            {
                storyHasStarted = true;
                storyManager.SendMessage("StartStory");
            }
        }

        //When not tracking
        if (newStatus == TrackableBehaviour.Status.NO_POSE && !isShuttingDown)
        {
            tracking = false;
            PauseAll();
        }
    }

    //Pauses the majority of functions
    public void PauseAll()
    {
        Time.timeScale = 0;
        audioManager.PauseAll();
    }

    //Resumes all paused functions
    public void ResumeAll()
    {
        if (tracking)
            audioManager.ResumeAll();
        Time.timeScale = 1;

        //Turns the pause button on
        for (int i = 0; i < components.transform.parent.childCount; i++)
            if (components.transform.parent.GetChild(i).name == "Pause_Button" || components.transform.parent.GetChild(i).name == "UI_Components")
                components.transform.parent.GetChild(i).gameObject.SetActive(true);
    }

    //Turns off all of the menus
    public void MenusOff()
    {
        
        for (int i = 0; i < components.transform.childCount; i++)
        {
            if (components.transform.GetChild(i).gameObject.tag != "PermanentUI")
            {
                components.transform.GetChild(i).gameObject.SetActive(false);
            }

            else
            {
                components.transform.GetChild(i).gameObject.SetActive(true);
            }
                
        }
            
    }

    public IEnumerator ValveHighlight(bool inOrOutlet) {
        int direction = 1;

        Material highlightMaterial;

        //Determines whether the inlet or outlet will be highlighted
        if (inOrOutlet) {
            highlightMaterial = inletHighlight;
            correctValve = "Inlet";
        }
        else {
            highlightMaterial = outletHighlight;
            correctValve = "Outlet";
        }

        //Stops animatinig once the valve has been clicked
        while (!valveSelected) {
            //Based off of the materials alpha, the direction will change which will lead to the object fading in or out
            if (highlightMaterial.color.a >= 1)
                direction = -1;

            else if (highlightMaterial.color.a <= 0)
                direction = 1;

            highlightMaterial.color = new Color(highlightMaterial.color.r, highlightMaterial.color.g, highlightMaterial.color.b, highlightMaterial.color.a + Time.deltaTime * highlightSpeed * direction);
            yield return null;
        }
    }

    //Loads back to the main menu
    public void GoHome()
    {
        SceneManager.LoadScene(0);
    }

    //Used to stop certain functions from running which would otherwise cause errors
    private void OnApplicationQuit()
    {
        isShuttingDown = true;
    }
}