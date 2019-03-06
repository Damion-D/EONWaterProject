using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioScript : MonoBehaviour
{
   public List<AudioClip> Tracks;
    [SerializeField] int trackCount;
    [SerializeField] Camera ARCam;
    AudioSource Listen;
    // Start is called before the first frame update
    void Start()
    {
        trackCount = 0;
        ARCam.gameObject.AddComponent<AudioSource>();
        Listen = ARCam.GetComponent<AudioSource>();


        
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
