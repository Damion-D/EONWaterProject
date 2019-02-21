using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotorCollisionBool : MonoBehaviour {

    public MaintScenarioStory maintScenarioStory;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter(Collision col)
    {
        //maintScenarioStory.HitMotorCapsule = true;
    }
}
