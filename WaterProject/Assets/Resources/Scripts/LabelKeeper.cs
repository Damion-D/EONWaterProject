using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabelKeeper : MonoBehaviour {

    public GameObject LevelLabel;
    public GameObject PauseLabel;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!LevelLabel.activeInHierarchy&&!PauseLabel.activeInHierarchy)
        {
            LevelLabel.SetActive(true);
        }

        if (PauseLabel.activeInHierarchy)
        {
            LevelLabel.SetActive(false);
        }
	}
}
