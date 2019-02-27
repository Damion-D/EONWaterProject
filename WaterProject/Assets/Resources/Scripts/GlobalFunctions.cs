using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalFunctions : MonoBehaviour
{
    public static Camera mainCam;
    public static Vector2 swipeDirection;

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

        //Uses mouse input if in the editor
        if (Application.isEditor)
        {
            //ScreenPointToRay allows for the touch to shoot a raycast into the scene, using the camera to figure out the origin and direction
            if (Input.GetMouseButtonDown(0))
                Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out hit);
        }
        else
        {
            //Checks to see if there are any current touches, which avoids errors from Input.GetTouch
            if (Input.touchCount < 1)
                return new RaycastHit();

            //ScreenPointToRay allows for the touch to shoot a raycast into the scene, using the camera to figure out the origin and direction
            Touch touch = Input.GetTouch(0);
            Physics.Raycast(mainCam.ScreenPointToRay(touch.position), out hit);
        }

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

        //Uses mouse input if in the editor
        if (Application.isEditor)
        {
            if (Input.GetMouseButton(0))
                Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out hit);
        }
        else
        {
            //Checks to see if there are any current touches, which avoids errors from Input.GetTouch
            if (Input.touchCount < 1)
                return new RaycastHit();

            Touch touch = Input.GetTouch(0);
            Physics.Raycast(mainCam.ScreenPointToRay(touch.position), out hit);
        }

        return hit;
    }


    //Coroutine to detect swipes
    public static IEnumerator SwipeDetect(Vector2 swipeDistances)
    {
        //Stores start time
        float time = Time.time;
        float currentTime = 0;

        Vector2 startPos = Vector2.zero;
        Vector2 currentPos;
        Vector2 difference;

        //Resets Swipe Direction to zero to avoid accidental detection of the last recorded swipe
        swipeDirection = Vector2.zero;

        //Uses mouse if in editor
        if(Application.isEditor)
        {
            startPos = Input.mousePosition;
        }
        else if(Input.touchCount > 0)
        {
            startPos = Input.touches[0].position;
        }

        do
        {
            //Finds difference from the current time to the stored time
            currentTime = Time.time - time;

            //Uses mouse if in editor
            if (Application.isEditor)
            {
                if (!Input.GetMouseButton(0))
                    break;
                currentPos = Input.mousePosition;
            }
            else
            {
                if (Input.touchCount == 0)
                    break;
                currentPos = Input.touches[0].position;
            }

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
    }
}
