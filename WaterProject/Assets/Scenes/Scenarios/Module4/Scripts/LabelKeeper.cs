using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LabelKeeper : MonoBehaviour {

    public GameObject LevelLabel;
    public GameObject PauseLabel;

    public string levelName;
	// Use this for initialization
	void Start () {
        LevelLabel.GetComponentInChildren<Text>().text = levelName;
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
