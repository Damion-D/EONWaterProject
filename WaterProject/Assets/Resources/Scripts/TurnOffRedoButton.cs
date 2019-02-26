using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOffRedoButton : MonoBehaviour {
    public GameObject RedoButton;
	// Use this for initialization
	void Start () {
        RedoButton.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
