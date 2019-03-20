using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalFunctions : MonoBehaviour
{
    public static Camera mainCam;
    public static Vector2 swipeDirection;

    public static bool swiping;
    public static bool colorFlash;

    private void Start()
    {
        //Stores main camera for raycasts
        mainCam = Camera.main;
    }

    //Takes a touch on the screen, and converts it into a raaycast into the scene
    public static RaycastHit DetectTouch(MonoBehaviour calledFrom, Vector2 swipeDistances)
    {
        mainCam = Camera.main;
        RaycastHit hit = new RaycastHit();

        if (Input.GetMouseButtonDown(0))
            Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out hit);

        //StartCoroutine requires a MonoBehaviour to run from, but cannot use this MonoBehaviour script since this function is static, which is why a MonoBehaviour is passed into this function
        if (hit.transform != null)
            calledFrom.StartCoroutine(SwipeDetect(swipeDistances));

        return hit;
    }

    //To make usage easier, if there won't be a swipe detection, you can pass in only the monoBehavior (usually just by typing 'this') 
    //It will call the version of the function above, but only pass in (0, 0) for the swipe distance
    public static RaycastHit DetectTouch(MonoBehaviour calledFrom)
    {
        return DetectTouch(calledFrom, Vector2.zero);
    }

    //Detects touches constantly rather than just when you first touch the screen
    public static RaycastHit DetectConstantTouch()
    {
        mainCam = Camera.main;
        RaycastHit hit = new RaycastHit();


        if (Input.GetMouseButton(0))
            Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out hit);

        return hit;
    }


    //Coroutine to detect swipes
    public static IEnumerator SwipeDetect(Vector2 swipeDistances)
    {
        if(!swiping)
        {
            swiping = true;
            //Stores start time
            float time = Time.time;
            float currentTime = 0;

            Vector2 startPos = Vector2.zero;
            Vector2 currentPos;
            Vector2 difference;

            //Resets Swipe Direction to zero to avoid accidental detection of the last recorded swipe
            swipeDirection = Vector2.zero;

            //Uses mouse if in editor
            startPos = Input.mousePosition;

            do
            {
                //Finds difference from the current time to the stored time
                currentTime = Time.time - time;


                if (!Input.GetMouseButton(0))
                    break;
                currentPos = Input.mousePosition;

                //Finds difference in initial touch position to the current touch position
                difference = currentPos - startPos;

                Debug.Log("Difference: " + difference);

                //If the absolute value (always positive) of the difference.x is greater than the swipe distance.x, set difference to either (1,0) or (-1,0) depending on direction
                if (Mathf.Abs(difference.x) > Mathf.Abs(swipeDistances.x))
                {
                    swipeDirection = new Vector2(Mathf.Sign(difference.x), 0);
                    break;
                }
                //If the absolute value (always positive) of the difference.y is greater than the swipe distance.y, set difference to either (0,1) or (0,-1) depending on direction
                else if (Mathf.Abs(difference.y) > Mathf.Abs(swipeDistances.y))
                {
                    swipeDirection = new Vector2(0, Mathf.Sign(difference.y));
                    break;
                }

                //Pauses the Coroutine for the rest of the frame, resuming next frame
                //This stops the while loop from locking up the program, though it only works in a coroutine
                yield return null;
            }
            //If the swipe isn't completed, will stop once coroutine runs longer than half a second
            while (currentTime < 0.5f);

            swiping = false;
        }
    }


    public static IEnumerator ColorFlash(Material material, Color color, float transTime, float pause)
    {
        if(!colorFlash)
        {
            colorFlash = true;
            Debug.Log("Color Flash");
            Color startingColor = material.color;
            float startingTime = Time.time;
            float currentTime;

            while (true)
            {
                currentTime = Time.time - startingTime;
                material.color = Color.Lerp(startingColor, color, currentTime / transTime);
                if (currentTime >= transTime)
                {
                    break;
                }
                yield return null;
            }

            yield return new WaitForSeconds(pause);

            Debug.Log("Color Flash Return");
            startingTime = Time.time;
            while (true)
            {
                currentTime = Time.time - startingTime;
                material.color = Color.Lerp(color, startingColor, currentTime / transTime);
                if (currentTime >= transTime)
                {
                    break;
                }
                yield return null;
            }
            colorFlash = false;
        }
    }

    public static IEnumerator ColorFlash(Material[] material, Color color, float transTime, float pause)
    {
        if (!colorFlash)
        {
            colorFlash = true;
            Debug.Log("Color Flash");
            Color[] startingColor = new Color[material.Length];
            float startingTime = Time.time;
            float currentTime;

            for (int i = 0; i < material.Length; i++)
            {
                startingColor[i] = material[i].color;
            }

            while (true)
            {
                currentTime = Time.time - startingTime;

                for (int i = 0; i < material.Length; i++)
                {
                    material[i].color = Color.Lerp(startingColor[i], color, currentTime / transTime);
                }
                
                if (currentTime >= transTime)
                {
                    break;
                }
                yield return null;
            }

            yield return new WaitForSeconds(pause);

            Debug.Log("Color Flash Return");
            startingTime = Time.time;
            while (true)
            {
                currentTime = Time.time - startingTime;

                for (int i = 0; i < material.Length; i++)
                {
                    material[i].color = Color.Lerp(color, startingColor[i], currentTime / transTime);
                }

                if (currentTime >= transTime)
                {
                    break;
                }
                yield return null;
            }
            colorFlash = false;
        }
    }
}
