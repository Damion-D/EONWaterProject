using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaintenanceMenuManager : MonoBehaviour {

    public GameObject AmperageMeter;
    public GameObject InfraredTherm;
    public GameObject Drips;
    public Text DripText;

    public void ToggleAmperage()
    {
        InfraredTherm.SetActive(false);
        Drips.SetActive(false);
        AmperageMeter.SetActive(true);
    }

    public void ToggleInfrared()
    {
        InfraredTherm.SetActive(true);
        Drips.SetActive(false);
        AmperageMeter.SetActive(false);
    }

    public void ToggleDrips()
    {
        InfraredTherm.SetActive(false);
        Drips.SetActive(true);
        AmperageMeter.SetActive(false);
    }

    /*public void TestDripText()
    {
        DripText.text += " so fat";
    }*/
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void LateUpdate()
    {
    }
}
