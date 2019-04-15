using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAudio : MonoBehaviour
{
    [SerializeField] AudioClip Incorrect;
    [SerializeField] AudioClip Correct;
    [SerializeField] AudioClip ButtonClick;
    [SerializeField] AudioSource Button;
    [SerializeField] GameObject AudioSources;

    // Start is called before the first frame update
    void Start()
    {
        AudioSources.AddComponent<AudioSource>();
        Button = AudioSources.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ButtonClicked()
    {
        Button.clip = ButtonClick;
        Button.Play();
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
