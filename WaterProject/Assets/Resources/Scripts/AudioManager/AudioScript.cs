using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioScript : MonoBehaviour
{
   public List<AudioClip> Tracks;
    [SerializeField] int trackCount;
    [SerializeField] Camera ARCam;
    AudioSource Listen;
    [SerializeField] AudioSource Button;
    [SerializeField] AudioClip Incorrect;
    [SerializeField] AudioClip Correct;
    [SerializeField] AudioClip ButtonClick;
    [SerializeField] GameObject Clipboard;

    // Start is called before the first frame update
    void Start()
    {
        trackCount = 0;
        ARCam.gameObject.AddComponent<AudioSource>();
        Listen = ARCam.GetComponent<AudioSource>();
        Listen.playOnAwake = false;
       


        
    }

    // Update is called once per frame
    void Update()
    {
        PlayButtonSound();
        
    }

    private void Awake()
    {
       // Listen.playOnAwake = false;
    }



    public void PlayAudio()
    {
        Listen.clip = Tracks[trackCount];

        Listen.Play();

        trackCount++;
    }

    public void PauseAudio()
    {
        Listen.Pause();
    }

    public void UnpauseAudio()
    {
        Listen.UnPause();
    }

    public void PlayButtonSound()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Button.clip = ButtonClick;
            Button.Play();


        }
    }

    public void AudioCorrect()
    {
        Button.clip = Correct;
        Button.Play();
    }

    public void AudioIncorrect()
    {
        Button.clip = Incorrect;
        Button.Play();

    }


}
