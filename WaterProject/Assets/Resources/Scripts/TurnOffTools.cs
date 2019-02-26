using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOffTools : MonoBehaviour {

    public GameObject DripCounterLabel;
    public GameObject AmpMeterLabel;
    public GameObject ThermLabel;
    public GameObject PauseLabel;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (PauseLabel.activeInHierarchy)
        {
            Debug.Log("You are inside your if statement, dingus");
            AmpMeterLabel.SetActive(false);
            ThermLabel.SetActive(false);
            DripCounterLabel.SetActive(false);
        }

        if (!PauseLabel.activeInHierarchy)
        {
            AmpMeterLabel.SetActive(true);
            ThermLabel.SetActive(true);
            DripCounterLabel.SetActive(true);
            PauseLabel.SetActive(true);
        }
		
	}
}
