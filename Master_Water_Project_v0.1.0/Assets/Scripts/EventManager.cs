//Writer: Alec

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{

    public delegate void RaycastAction(GameObject UI);
    public static event RaycastAction OnRaycastHit;

    [SerializeField]
    private Camera cam;

    public GameObject Canvas;
    void Start()
    {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        EventManager.OnRaycastHit += this.ToggleUI;
    }

    void OnDisabled()
    {

        EventManager.OnRaycastHit -= this.ToggleUI;
    }

    public static void RaycastHit(GameObject UI)
    {
        if (OnRaycastHit != null)
        {
            OnRaycastHit(UI);
        }
    }

    private void ToggleUI(GameObject UI)
    {
        Debug.Log("Toggle");
        for (int i = 0; i < Canvas.transform.childCount; i++)
        {
            Canvas.transform.GetChild(i).gameObject.SetActive(false);
        }
        UI.SetActive(true);
    }

    private void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            // Get movement of the finger since last frame

            Ray ray = cam.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                if (hit.collider.tag == "Interactable")
                {
                    EventManager.RaycastHit(hit.collider.gameObject);
                }
            }
            // Move object across XY plane
            //transform.Translate(-touchDeltaPosition.x * speed, -touchDeltaPosition.y * speed, 0);
        }
    }
}