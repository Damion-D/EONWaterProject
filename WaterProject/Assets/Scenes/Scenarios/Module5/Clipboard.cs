using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clipboard : MonoBehaviour
{
    [Header("Gameobjects")]
    [Space]
    [SerializeField]Transform keypad;
    [SerializeField]Transform clipboard;
    [Space]
    [Header("Transforms")]
    [Space]
    [SerializeField] Transform[] TextMeshPro_date;
    [SerializeField] Transform[] TextMeshPro_time;
    [SerializeField] Transform[] TextMeshPro_freeChlorine;
    [SerializeField] Transform[] TextMeshPro_totalChlorine;
    [SerializeField] Transform[] TextMeshPro_freeToTotal;
    [SerializeField] Transform[] TextMeshPro_monoChlorine;
    [SerializeField] Transform[] TextMeshPro_diChlorine;
    [SerializeField] Transform[] TextMeshPro_monoToTotal;
    [SerializeField] Transform[] TextMeshPro_Ammonia;
    [Space]
    [Header("Rows")]
    [SerializeField] Transform[] TextMeshPro_Row;
    [Space]
    [Header("Numbers")]
    [SerializeField]int step;
    [SerializeField]int restarts;
    [SerializeField] float chlorineTotal; //for testing purposes, this will be random
    [SerializeField]float chlorineMono; //for testing purposes, this will be random
    [SerializeField] float chlorineDi; //Total - Mono is this
    [SerializeField]float clipboardInput;
    [SerializeField]float stepClock;
    [SerializeField]float animDisplayTime;
    [SerializeField]string currentReading_str;
    [SerializeField]float currentReading_int = 0;
    [SerializeField]int numbersInputted;

    private void Start()
    {
        //stepClock = 10;
        restarts = 0;
        TextMeshPro_Row[restarts].gameObject.SetActive(true);
        //keypad.gameObject.SetActive(true);
    }
    void Update()
    {
        //randomizes and rounds the number for chlorine total
        float t1 = UnityEngine.Random.Range(2.6f,3f);
        t1 = Mathf.Round(t1 * 100f) / 100f;
        //randomizes and rounds the number for chlorine monos
        float m1 = UnityEngine.Random.Range(2.4f, 2.5f);
        m1 = Mathf.Round(m1 * 100f) / 100f;

        stepClock -= Time.deltaTime;

        if (stepClock < 0)
        {
            switch (step)
            {
                case 0:
                    StartCoroutine(ShowClip());
                    //once clock hits 0, add a step and reset clock
                    SetClipboardText();
                    step++;
                    stepClock = 5;
                    break;
                case 1:
                    //set a random total reading for testing

                    currentReading_str = "Total";
                    chlorineTotal = t1;

                    //once clock hits 0, add a step and reset clock
                    step++;
                    stepClock = 5;
                    break;
                case 2:
                    ClipboardKeypadInput();
                    break;
                case 3:
                    //repeat step 2, but for mono
                    currentReading_str = "Mono";
                    chlorineMono = m1;

                    //once clock hits 0, add a step and reset clock
                    step++;
                    stepClock = 5;
                    //pause for audio
                    break;
                case 4:
                    ClipboardKeypadInput();
                    break;
                case 5:
                    //once clock hits 0, add a step and reset clock
                    step++;
                    stepClock = 5;
                    //pause for audio
                    break;
                case 6:
                    //rounds the chlorineDi float
                    chlorineDi = chlorineTotal - chlorineMono;
                    chlorineDi = Mathf.Round(chlorineDi * 100f) / 100f;
                    //displays it in the text box
                    TextMeshPro_diChlorine[restarts].GetComponent<TMPro.TextMeshPro>().text = chlorineDi.ToString();
                    //advances program
                    step++;
                    break;
                case 7:
                    //once clock hits 0, add a step and reset clock
                    step++;
                    stepClock = 5;
                    //pause for audio
                    break;
                case 8:
                    //mono / total * 100

                    float percentMtoT = chlorineMono / chlorineTotal * 100;
                    percentMtoT = Mathf.Round(percentMtoT * 10f) / 10f;

                    TextMeshPro_monoToTotal[restarts].GetComponent<TMPro.TextMeshPro>().text = percentMtoT.ToString() + "%";
                    break;

            }

            

        }
    }

   public void SetClipboardText()
    {
        //set the date and time automatically
        DateTime dt = GetNow();
        DateTime theTime = realTime();

        //put all auto stuff under this line
        TextMeshPro_date[restarts].GetComponent<TMPro.TextMeshPro>().text = dt.ToString("MM-dd");
        TextMeshPro_time[restarts].GetComponent<TMPro.TextMeshPro>().text = theTime.ToString("HH:mm");
        TextMeshPro_freeChlorine[restarts].GetComponent<TMPro.TextMeshPro>().text = "0";
        TextMeshPro_freeToTotal[restarts].GetComponent<TMPro.TextMeshPro>().text = "0";
        TextMeshPro_Ammonia[restarts].GetComponent<TMPro.TextMeshPro>().text = "4:1";
    }

    void ClipboardKeypadInput()
    {
        
        RaycastHit hit = GlobalFunctions.DetectTouch(this);

        Debug.Log(hit.transform);

        keypad.gameObject.SetActive(true);

        switch (currentReading_str)
        {
            case "Total":
                currentReading_int = chlorineTotal;
                /*Since there's 2 things to input for this, this system sets up a "current reading" thing. 
                If the user is inputting total chlorimines, "currentReading_str" is set to "Total", and the script goes and gets the total chlorimine number*/

                break;

            case "Mono":
                currentReading_int = chlorineMono;
                break;

        }

        //if on the total chlorine reading step, set current reading to "Total" and the int to "chlorineTotal"
        if (hit.transform != null)
        {
            Transform hitTrans = hit.transform;

            switch (currentReading_str)
            {
                case "Total":
                   float.TryParse(TextMeshPro_totalChlorine[restarts].GetComponent<TMPro.TextMeshPro>().text, out clipboardInput);
                    break;

                case "Mono":
                    float.TryParse(TextMeshPro_monoChlorine[restarts].GetComponent<TMPro.TextMeshPro>().text, out clipboardInput);
                    break;

                case "Di":
                    float.TryParse(TextMeshPro_diChlorine[restarts].GetComponent<TMPro.TextMeshPro>().text, out clipboardInput);
                    break;
            }


            if (hitTrans.name == "Enter" && clipboardInput == currentReading_int)
             {
                 keypad.gameObject.SetActive(false);

                //if number less than 3 digits
                for (int i = 0; i > 3 - numbersInputted; i++)
                {
                    switch (currentReading_str)
                    {
                        case "Total":
                            TextMeshPro_totalChlorine[restarts].GetComponent<TMPro.TextMeshPro>().text += "0";
                            break;

                        case "Mono":
                            TextMeshPro_monoChlorine[restarts].GetComponent<TMPro.TextMeshPro>().text += "0";
                            break;

                        case "Di":
                            TextMeshPro_diChlorine[restarts].GetComponent<TMPro.TextMeshPro>().text += "0";
                            break;
                    }
                }

                //once clock hits 0, add a step and reset clock
                step++;
                stepClock = 5;
                //step++;
            }

             if (float.TryParse(hitTrans.name, out clipboardInput))
             {
                 switch(currentReading_str)
                {
                        case "Total":
                        if(numbersInputted == 0)
                        {
                            TextMeshPro_totalChlorine[restarts].GetComponent<TMPro.TextMeshPro>().text = clipboardInput.ToString() + ".";
                        }
                        else
                        {
                            TextMeshPro_totalChlorine[restarts].GetComponent<TMPro.TextMeshPro>().text += clipboardInput.ToString();
                        }
                         
                        break;

                        case "Mono":
                        if (numbersInputted == 0)
                        {
                            TextMeshPro_monoChlorine[restarts].GetComponent<TMPro.TextMeshPro>().text = clipboardInput.ToString() + ".";
                        }
                        else
                        {
                            TextMeshPro_monoChlorine[restarts].GetComponent<TMPro.TextMeshPro>().text += clipboardInput.ToString();
                        }
                        break;

                        case "Di":
                        if (numbersInputted == 0)
                        {
                            TextMeshPro_diChlorine[restarts].GetComponent<TMPro.TextMeshPro>().text = clipboardInput.ToString() + ".";
                        }
                        else
                        {
                            TextMeshPro_diChlorine[restarts].GetComponent<TMPro.TextMeshPro>().text += clipboardInput.ToString();
                        }
                        break;
                }

                numbersInputted += 1;
                if (numbersInputted > 2)
                    numbersInputted = 0;
            }
        }
    }

    //gets date & time
    private static DateTime GetNow()
    {
        return DateTime.Now;
    }
    private static DateTime realTime()
    {
        return DateTime.Now;

    }

    //lerps clipboard
    IEnumerator ShowClip()
    {
        float startTime = Time.time; //sets start time to seconds in program
        float currentTime;
        stepClock = animDisplayTime;
        while (true)
        {
            currentTime = Time.time - startTime; //sets current time to current time in program

            clipboard.localScale = Vector3.Lerp(new Vector3(0, 0, 0), new Vector3(0.015f, 0.015f, 0.015f), currentTime / animDisplayTime);
            //scales object up to 100, the end bit here divides the current time by the animation time. Once they both match, anim over
            if (currentTime > animDisplayTime)
                //this breaks off the coroutine once its done
                break;

            yield return null;
        }
        
    }
    //restart function just in case
    public void Restart()
    {
        restarts++;
        if (restarts >= 11)
        {
            restarts = 0;
        }
        step = 0;
    }
}
