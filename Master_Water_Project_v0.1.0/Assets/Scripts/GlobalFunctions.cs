using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalFunctions : MonoBehaviour
{
    static Camera mainCam;
    public static Vector2 swipeDirection;

    private void Awake()
    {
        mainCam = Camera.main;
    }

    //Takes a touch on the screen, and converts it into a raaycast into the scene
    public static RaycastHit DetectTouch()
    {
        RaycastHit hit = new RaycastHit();
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

        return hit;
    }

    static IEnumerator SwipeDetect()
    {
        float time = Time.time;
        float currentTime = 0;

        Vector2 startPos = Vector2.zero;
        Vector2 currentPos;
        Vector2 difference;

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

            if(Mathf.Abs(difference.x) > 25)
            {
                swipeDirection = new Vector2(Mathf.Sign(difference.x), 0);
            }
            else if(Mathf.Abs(difference.y) > 25)
            {
                swipeDirection = new Vector2(0, Mathf.Sign(difference.y));
            }

            yield return null;
        }
        while (currentTime < 1);
    }
}
