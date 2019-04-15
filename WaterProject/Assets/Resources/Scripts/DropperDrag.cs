using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropperDrag : MonoBehaviour
{
    float posX; //Part of onMouseDown function
    float posZ; //Part of onMouseDown function
    float posY; //Part of onMouse Down function

    Vector3 dist; //Determines distance between two mouse clicks helps to figure out dragging an //object.
    Vector3 startPos; // Part of onMouseDown function

    [SerializeField] public GameObject dropperFlash; //The second layer inside the water dropper that //allows for water droplet to flash

    public bool interactable;

    public Transform dropperPos;
    Transform titratorPivot;
    Transform bottle;

    float distBetweenTwoPoints; //Determines distance between two gameobject positions used //in distanceBetweenTwoPoints function

    private void Start()
    {
        bottle = transform.parent;
        titratorPivot = bottle.parent;
    }

    // Update is called once per frame
    void Update()
    {
        if (interactable)
        {
            transform.parent = titratorPivot;
            distancetoSample(transform.position, dropperPos.position);
        }
        else
        {
            transform.parent = bottle;
        }
    }

    void OnMouseDown()
    {
        if (interactable)
        {
            FlashDisappear(dropperFlash);

            startPos = transform.localPosition;

            dist = Camera.main.WorldToScreenPoint(transform.position);

            posX = Input.mousePosition.x - dist.x;

            posY = Input.mousePosition.y - dist.y;

            posZ = Input.mousePosition.z - dist.z;
        }
    }

    void OnMouseDrag()
    {
        if (interactable)
        {
            float disX = Input.mousePosition.x - posX;
            float disY = Input.mousePosition.y - posY;
            float disZ = Input.mousePosition.z - posZ;
            Vector3 lastPos = Camera.main.ScreenToWorldPoint(new Vector3(disX, disY, disZ)) + titratorPivot.position;
            //transform.position = new Vector3(lastPos.x, startPos.y, startPos.z);
            transform.localPosition = new Vector3(-lastPos.x / 9, startPos.y, 0);
        }
    }

    public void FlashAppear(GameObject ObjWFlash)
    {
        ObjWFlash.SetActive(true);
    }

    public void FlashDisappear(GameObject FlashRem)
    {
        FlashRem.SetActive(false);
    }

    public void distancetoSample(Vector3 pointA, Vector3 pointB) // Checks to see if distance between point A and B is small enough, if it is you can't move dropper any more
    {
        distBetweenTwoPoints = Vector3.Distance(pointA, pointB);

        if (distBetweenTwoPoints <= 1)
        {
            interactable = false;
        }

    }

}
