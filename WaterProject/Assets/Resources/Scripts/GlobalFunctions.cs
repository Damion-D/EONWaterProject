using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalFunctions : MonoBehaviour
{
    public static Camera mainCam;
    public static Vector2 swipeDirection;

    private void Start()
    {
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
            if (Input.GetMouseButtonDown(0))
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

        //StartCoroutine requires a MonoBehaviour to run from, but cannot use this MonoBehaviour script since this function is static, which is why a MonoBehaviour is passed into this function
        if (hit.transform != null)
            calledFrom.StartCoroutine(SwipeDetect(swipeDistances));

        return hit;
    }

    public static RaycastHit DetectTouch(MonoBehaviour calledFrom)
    {
        return DetectTouch(calledFrom, Vector2.zero);
    }

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

    public static IEnumerator SwipeDetect(Vector2 swipeDistances)
    {
        float time = Time.time;
        float currentTime = 0;

        Vector2 startPos = Vector2.zero;
        Vector2 currentPos;
        Vector2 difference;

        //Resets Swipe Direction to zero to avoid accidental detection of the last recorded swipe
        swipeDirection = Vector2.zero;

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
            currentTime = Time.time - time;


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

            difference = currentPos - startPos;

            Debug.Log("Difference: " + difference);

            if(Mathf.Abs(difference.x) > Mathf.Abs(swipeDistances.x))
            {
                swipeDirection = new Vector2(Mathf.Sign(difference.x), 0);
            }
            else if(Mathf.Abs(difference.y) > Mathf.Abs(swipeDistances.y))
            {
                swipeDirection = new Vector2(0, Mathf.Sign(difference.y));
            }

            yield return null;
        }
        while (currentTime < 0.5f);
    }
}
