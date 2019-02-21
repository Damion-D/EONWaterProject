//Writer: Alec

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GifPlayer : MonoBehaviour {

    public Sprite[] frames;
    public float fps;
    private Image screen;
    public bool play;
	// Use this for initialization
	void Start () {
        screen = GetComponent<Image>();
	}

    // Update is called once per frame
    void Update() {
        if (play && frames.Length > 0)
        {
            int index = (int)(Time.time * fps);
            index = index % frames.Length;
            screen.sprite = frames[index];
        }
    }

    public void SetPlay(bool val)
    {
        play = val;
        screen.color = (play) ? new Color(1, 1, 1, 1) : new Color(.5f, .5f, .5f, .5f);
    }
}
