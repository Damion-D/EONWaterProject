using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipetteSqueeze : MonoBehaviour
{

    [SerializeField] Transform titrator;
    [SerializeField] Transform scaleStart;
    [SerializeField] Transform scaleEnd;
    [SerializeField] GameObject sqBottleFlash;
    [SerializeField] Transform sqBottle;
    [SerializeField] Transform pipetteWater;
    [SerializeField] float pippetteAmountToAdd;
    [SerializeField] Transform pipetFlash;

    float currentPipetteAmount;
    


    // Start is called before the first frame update
    void Start()
    {
        //titrator.gameObject.SetActive(false);
        //sqBottleFlash.gameObject.SetActive(false);
        GlobalFunctions.SetMainCam();
        pipetFlash.gameObject.SetActive(false);
        pipetteWater.localScale = new Vector3(pipetteWater.localScale.x, 0, pipetteWater.localScale.z);

        StartCoroutine(Intro()); 
        
    }

    // Update is called once per frame
    void Update()
    {
        SquirtBottleTouchFill();
    }

    IEnumerator Intro()
    {
        titrator.localScale = scaleStart.localScale;
        
        //titrator.gameObject.SetActive(true);
        float currentTime;
        float startTime = Time.time;

        while (true)
        {


            currentTime = Time.time - startTime;
            
            
            titrator.localScale = Vector3.Lerp(scaleStart.localScale, scaleEnd.localScale, currentTime/2);

            if (currentTime > 3)
            {
                SquirtBottleFlash();
                break;
            }
            
            yield return null;
        }

        
    }

    void SquirtBottleFlash()
    {
        sqBottleFlash.gameObject.SetActive(true);
    }

    void SquirtBottleTouchFill()
    {


        RaycastHit touched = GlobalFunctions.DetectConstantTouch();

            if (touched.transform != null)
            {
                if (touched.transform == sqBottle.transform)
                {
                    currentPipetteAmount += pippetteAmountToAdd;
                    currentPipetteAmount = Mathf.Clamp01(currentPipetteAmount);
                    pipetteWater.localScale = new Vector3(pipetteWater.localScale.x, Mathf.Lerp(0, 0.04882142f, currentPipetteAmount), pipetteWater.localScale.z);
                    if(currentPipetteAmount == 1 )
                    {
                        sqBottleFlash.gameObject.SetActive(false);
                        pipetFlash.gameObject.SetActive(true);
                    }

                }
            }


    }
}
