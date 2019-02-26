//Writer: Levin

using UnityEngine;

[System.Serializable]
public class Sound {

    //Settings the user can change for each Sound in the Inspector
    [Header("Settings")]
    public string name;
    public AudioClip clip;
    public bool loop;

    //Information the user can read for reference (uses the ReadOnlyAttribute script to allow the ReadOnly functionality)
    [Header("Information")]
    [ReadOnly] public bool playing;
    [ReadOnly] public bool paused;
    [ReadOnly] public int volume;
    [ReadOnly] public string length;
    [ReadOnly] public string currentTime;

    //Variables used for internal funtions that don't need to be seen by the user
    [HideInInspector] public AudioSource audioSource;
    [HideInInspector] public bool hasCompleted = false;
    [HideInInspector] public bool waiting = false;
}