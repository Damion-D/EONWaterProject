using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using UnityEngine.UI;

public class PPEStory : MonoBehaviour, ITrackableEventHandler {

    public AudioManager audioManager;
    public Utility utility;
    public GameObject pauseMenu;
    public GameObject pauseButton;

    //public GameObject boots;

    private TrackableBehaviour mTrackableBehaviour;

    public bool storyHasStarted = false;
    private bool scenarioFinished = false;
    private bool bootsSelected = false;
    private bool glassesSelected = false;
    private bool objectSelected = false;
    private bool lastTimeAround = false;

    public string hitName;
    public string correctGearName;

    public bool hitBoots = false;

    public Material BootsYellow;
    public Material LaceMat;
    public Material EyeletsMat;
    public Material EarmuffsBlack;
    public Material EarmuffsYellow;
    public Material EarmuffsMetal;
    public Material GlassesFrame;
    public Material GlassesLenses;
    public Material HatMat;
    public Material GlovesYellow;
    public Material GlovesBrown;

    public GameObject Boots;
    public GameObject Glasses;
    public GameObject Hat;
    public GameObject Earmuffs;
    public GameObject Gloves;
    public GameObject BootLeft;

	// Use this for initialization
	void Start () {
        //Set up the event handler for tracking from Vuforia
        mTrackableBehaviour = GameObject.Find("ImageTarget").GetComponent<TrackableBehaviour>();

        if (mTrackableBehaviour)
            mTrackableBehaviour.RegisterTrackableEventHandler(this);

      
        /*BootsYellow = GetComponent<Renderer>().material;
        LaceMat = GetComponent<Renderer>().material;
        EyeletsMat = GetComponent<Renderer>().material;
        EarmuffsBlack = GetComponent<Renderer>().material;
        EarmuffsYellow = GetComponent<Renderer>().material;
        EarmuffsMetal = GetComponent<Renderer>().material;
        GlassesFrame = GetComponent<Renderer>().material;
        GlassesLenses = GetComponent<Renderer>().material;
        HatMat = GetComponent<Renderer>().material;
        GlovesYellow = GetComponent<Renderer>().material;
        GlovesBrown = GetComponent<Renderer>().material;*/

        /*Color cl1 = BootsYellow.color;
        Color cl2 = LaceMat.color;
        Color cl3 = EyeletsMat.color;
        Color cl4 = EarmuffsBlack.color;
        Color cl5 = EarmuffsYellow.color;
        Color cl6 = EarmuffsMetal.color;
        Color cl7 = GlassesFrame.color;
        Color cl8 = GlassesLenses.color;
        Color cl9 = HatMat.color;
        Color cl10 = GlovesYellow.color;
        Color cl11 = GlovesBrown.color;
        cl1.a=255;
        cl2.a=255;
        cl3.a=255;
        cl4.a=255;
        cl5.a=255;
        cl6.a=255;
        cl7.a=255;
        cl8.a=111;
        cl9.a=255;
        cl10.a=255;
        cl11.a=255;
        BootsYellow.color = cl1;
        LaceMat.color = cl2;
        EyeletsMat.color = cl3;
        EarmuffsBlack.color = cl4;
        EarmuffsYellow.color = cl5;
        EarmuffsMetal.color = cl6;
        GlassesFrame.color = cl7;
        GlassesLenses.color = cl8;
        HatMat.color = cl9;
        GlovesYellow.color = cl10;
        GlovesBrown.color = cl11;*/
    }
	
	// Update is called once per frame
	void Update () {
        //Debug.Log(hitName);
        //Debug.Log("The HearingProtectionAnswer .hasCompleted property is: " + audioManager.GetSound("HearingProtectionAnswer").hasCompleted);
	}

    private IEnumerator PPENarrative()
    {
        audioManager.PlaySound("Introduction", 1.0f, true, 2);
        correctGearName = "Boots";
        yield return null;

        while (true)
        {
            switch (hitName)
            {
                case "Boots":
                    if (correctGearName == "Boots"&&objectSelected==false)
                    {
                        correctGearName = "Glasses";
                        objectSelected = true;
                        audioManager.PlaySound("BootsAnswer", 1.0f, true, 2);
                        StartCoroutine("BootsFade");
                        
                        
                        

                    }

                    if(correctGearName != "Boots"&&objectSelected==false)
                    {
                        //bootsNotSelectedYet = false;
                        objectSelected = true;
                        audioManager.PlaySound("IncorrectAnswer", 1.0f, true, 2);
                    }

                    if (objectSelected)
                    {
                        
                        hitName = "Reset";
                    }
                    break;

                case "Glasses":
                    if (correctGearName == "Glasses"&&objectSelected==false)
                    {
                        objectSelected = true;
                        audioManager.PlaySound("GlassesAnswer", 1.0f, true, 2);
                        correctGearName = "HardHat";
                        StartCoroutine("GlassesFade");
                    }

                    if(correctGearName != "Glasses"&&objectSelected==false)
                    {
                        objectSelected = true;
                        audioManager.PlaySound("IncorrectAnswer", 1.0f, true, 2);
                    }

                    if (objectSelected)
                    {
                        hitName = "Reset";
                    }
                    break;

                case "HardHat":
                    if (correctGearName == "HardHat" && objectSelected == false)
                    {
                        objectSelected = true;
                        audioManager.PlaySound("HardHatAnswer", 1.0f, true, 2);
                        correctGearName = "HearingProtection";
                        //StartCoroutine("HatFade");
                    }

                    if (correctGearName != "HardHat" && objectSelected == false)
                    {
                        objectSelected = true;
                        audioManager.PlaySound("IncorrectAnswer", 1.0f, true, 2);
                    }

                    if (objectSelected)
                    {
                        hitName = "Reset";
                    }
                    break;

                case "HearingProtection":
                    if (correctGearName == "HearingProtection" && objectSelected == false)
                    {
                        objectSelected = true;
                        audioManager.PlaySound("HearingProtectionAnswer", 1.0f, true, 2);
                        correctGearName = "Gloves";
                        //StartCoroutine("EarmuffsFade");
                    }

                    if (correctGearName != "HearingProtection" && objectSelected == false)
                    {
                        objectSelected = true;
                        audioManager.PlaySound("IncorrectAnswer", 1.0f, true, 2);
                    }

                    if (objectSelected)
                    {
                        //Earmuffs.SetActive(false);
                        hitName = "Reset";
                    }
                    break;
                    

                case "Gloves":
                    
                    if (correctGearName == "Gloves" && objectSelected == false)
                    {
                        objectSelected = true;
                        audioManager.PlaySound("GlovesAnswer", 1.0f, true, 2);
                        correctGearName = "Finished";
                        //StartCoroutine("GlovesFade");
                        scenarioFinished = true;
                    }

                    if (correctGearName != "Gloves" && objectSelected == false)
                    {
                        objectSelected = true;
                        audioManager.PlaySound("IncorrectAnswer", 1.0f, true, 2);
                    }

                    if (objectSelected)
                    {
                        hitName = "Reset";
                    }
                    break;
                    

                case "Reset":
                    if (audioManager.GetSound("BootsAnswer").hasCompleted)
                    {
                        StartCoroutine("BootsFade");
                    }
                    if (audioManager.GetSound("GlassesAnswer").hasCompleted)
                    {
                        StartCoroutine("GlassesFade");
                    }
                    if (audioManager.GetSound("HardHatAnswer").hasCompleted)
                    {
                        StartCoroutine("HatFade");
                    }
                    if (audioManager.GetSound("HearingProtectionAnswer").hasCompleted)
                    {
                        StartCoroutine("EarmuffsFade");
                        //Earmuffs.SetActive(false);
                    }
                    if (audioManager.GetSound("GlovesAnswer").hasCompleted)
                    {
                        StartCoroutine("GlovesFade");
                    }
                    objectSelected = false;
                    break;
            }

            if (scenarioFinished&&audioManager.GetSound("GlovesAnswer").hasCompleted)
            {
                audioManager.PlaySound("Epilogue", 1.0f, true, 2);
                break;
            }

            yield return null;
        }

        while (true)
        {
            if (audioManager.GetSound("Epilogue").hasCompleted && lastTimeAround == false)
            {
                lastTimeAround = true;
                yield return new WaitForSeconds(2);
                pauseMenu.SetActive(true);
                pauseButton.SetActive(false);
                utility.PauseAll();
                break;
            }

            yield return null;
        }
        
        




    }
    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        if ((newStatus == TrackableBehaviour.Status.TRACKED || newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED) && previousStatus == TrackableBehaviour.Status.NO_POSE && storyHasStarted == false)
        {
            /*BootsYellow = GetComponent<Renderer>().material;
            LaceMat = GetComponent<Renderer>().material;
            EyeletsMat = GetComponent<Renderer>().material;
            EarmuffsBlack = GetComponent<Renderer>().material;
            EarmuffsYellow = GetComponent<Renderer>().material;
            EarmuffsMetal = GetComponent<Renderer>().material;
            GlassesFrame = GetComponent<Renderer>().material;
            GlassesLenses = GetComponent<Renderer>().material;
            HatMat = GetComponent<Renderer>().material;
            GlovesYellow = GetComponent<Renderer>().material;
            GlovesBrown = GetComponent<Renderer>().material;

            Color c1 = BootsYellow.color;
            Color c2 = LaceMat.color;
            Color c3 = EyeletsMat.color;
            Color c4 = EarmuffsBlack.color;
            Color c5 = EarmuffsYellow.color;
            Color c6 = EarmuffsMetal.color;
            Color c7 = GlassesFrame.color;
            Color c8 = GlassesLenses.color;
            Color c9 = HatMat.color;
            Color c10 = GlovesYellow.color;
            Color c11 = GlovesBrown.color;
            c1.a = 255;
            c2.a = 255;
            c3.a = 255;
            c4.a = 255;
            c5.a = 255;
            c6.a = 255;
            c7.a = 255;
            c8.a = 111;
            c9.a = 255;
            c10.a = 255;
            c11.a = 255;
            BootsYellow.color = c1;
            LaceMat.color = c2;
            EyeletsMat.color = c3;
            EarmuffsBlack.color = c4;
            EarmuffsYellow.color = c5;
            EarmuffsMetal.color = c6;
            GlassesFrame.color = c7;
            GlassesLenses.color = c8;
            HatMat.color = c9;
            GlovesYellow.color = c10;
            GlovesBrown.color = c11;*/
            storyHasStarted = true;
            StartCoroutine(PPENarrative());
        }
    }

    private IEnumerator BootsFade()
    {
       
        for (float f = 1f; f >= 0; f -= 0.01f)
        {
            
            if (f <= 0.01f)
            {
                Boots.SetActive(false);
                Color cl1 = BootsYellow.color;
                Color cl2 = LaceMat.color;
                Color cl3 = EyeletsMat.color;
                cl1.a = 255;
                cl2.a = 255;
                cl3.a = 255;
                BootsYellow.color = cl1;
                LaceMat.color = cl2;
                EyeletsMat.color = cl3;
            }
            Color c1 = BootsYellow.color;
            Color c2 = LaceMat.color;
            Color c3 = EyeletsMat.color;
            c1.a = f;
            c2.a = f;
            c3.a = f;
            BootsYellow.color = c1;
            LaceMat.color = c2;
            EyeletsMat.color = c3;

            yield return null;
        }
    }

    private IEnumerator GlassesFade()
    {
        for (float f = 1f; f >= 0; f -= 0.01f)
        {
            if (f <= 0.01f)
            {
                Glasses.SetActive(false);
            }
            Color c1 = GlassesFrame.color;
            Color c2 = GlassesLenses.color;
            c1.a = f;
            c2.a = f;
            GlassesFrame.color = c1;
            GlassesLenses.color = c2;

            yield return null;
        }
    }

    private IEnumerator HatFade()
    {
        for (float f = 1f; f >= 0; f -= 0.01f)
        {
            if (f <= 0.01f)
            {
                Hat.SetActive(false);
            }
            Color c = HatMat.color;
            c.a = f;
            HatMat.color = c;

            yield return null;
        }
    }

    private IEnumerator EarmuffsFade()
    {
        for (float f = 1f; f >= 0; f -= 0.01f)
        {
            if (f <= 0.01f)
            {
                Earmuffs.SetActive(false);
            }
            Color c1 = EarmuffsBlack.color;
            Color c2 = EarmuffsYellow.color;
            Color c3 = EarmuffsMetal.color;
            c1.a = f;
            c2.a = f;
            c3.a = f;
            EarmuffsBlack.color = c1;
            EarmuffsYellow.color = c2;
            EarmuffsMetal.color = c3;

            yield return null;
        }
    }

    private IEnumerator GlovesFade()
    {
        for (float f = 1f; f >= 0; f -= 0.01f)
        {
            if (f <= 0.01f)
            {
                Gloves.SetActive(false);
            }
            Color c1 = GlovesYellow.color;
            Color c2 = GlovesBrown.color;
            c1.a = f;
            c2.a = f;
            GlovesYellow.color = c1;
            GlovesBrown.color = c2;

            yield return null;
        }
    }


    void OnApplicationQuit()
    {
        Debug.Log("Application's done!");
        Boots.SetActive(true);
        /*BootsYellow = GetComponent<Renderer>().material;
        LaceMat = GetComponent<Renderer>().material;
        EyeletsMat = GetComponent<Renderer>().material;
        EarmuffsBlack = GetComponent<Renderer>().material;
        EarmuffsYellow = GetComponent<Renderer>().material;
        EarmuffsMetal = GetComponent<Renderer>().material;
        GlassesFrame = GetComponent<Renderer>().material;
        GlassesLenses = GetComponent<Renderer>().material;
        HatMat = GetComponent<Renderer>().material;
        GlovesYellow = GetComponent<Renderer>().material;
        GlovesBrown = GetComponent<Renderer>().material;*/

        Color clr1 = BootsYellow.color;
        Color clr2 = LaceMat.color;
        Color clr3 = EyeletsMat.color;
        Color clr4 = EarmuffsBlack.color;
        Color clr5 = EarmuffsYellow.color;
        Color clr6 = EarmuffsMetal.color;
        Color clr7 = GlassesFrame.color;
        Color clr8 = GlassesLenses.color;
        Color clr9 = HatMat.color;
        Color clr10 = GlovesYellow.color;
        Color clr11 = GlovesBrown.color;
        clr1.a = 255f;
        clr2.a = 255f;
        clr3.a = 255f;
        clr4.a = 255f;
        clr5.a = 255f;
        clr6.a = 255f;
        clr7.a = 255f;
        clr8.a = 111f;
        clr9.a = 255f;
        clr10.a = 255f;
        clr11.a = 255f;
        BootsYellow.color = clr1;
        LaceMat.color = clr2;
        EyeletsMat.color = clr3;
        EarmuffsBlack.color = clr4;
        EarmuffsYellow.color = clr5;
        EarmuffsMetal.color = clr6;
        GlassesFrame.color = clr7;
        GlassesLenses.color = clr8;
        HatMat.color = clr9;
        GlovesYellow.color = clr10;
        GlovesBrown.color = clr11;
    }
}
