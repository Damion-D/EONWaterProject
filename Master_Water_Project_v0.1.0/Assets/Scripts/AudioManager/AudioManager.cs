//Writer: Levin

/*-------------------------------------------------------------------------------------------------
 * IMPORTANT:
 * The audio manager only works in conjunction with the custom Sound object (which includes the 
 * ReadOnlyAttribute script). The Sound object is contained in its own script and contains all the
 * elements seen in the inspector along with some other backend values
-------------------------------------------------------------------------------------------------*/

using System.Collections; //Needed for Coroutines
using System.Collections.Generic; //Needed for Lists
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public Sound[] sounds; //Creates an array of custom Sound objects in the inspector

    //CAN be used to ensure a sound has completed fading before being affected by any other volume changes in other scripts
    [HideInInspector] public List<Sound> fadingInSounds = new List<Sound>();
    [HideInInspector] public List<Sound> fadingOutSounds = new List<Sound>();

    //These arrays keep track of Sounds that are in a specific state, to allow certain functionality
    [HideInInspector] public List<Sound> playingSounds = new List<Sound>();
    [HideInInspector] public List<Sound> pausedSounds = new List<Sound>();

    //Ensures that there is an AudioSource on startup for the rest of the functionality to work
    private void Awake()
    {
        if (!(GetComponents<AudioSource>().Length > 0))
            gameObject.AddComponent<AudioSource>();
    }

    //Responisble for starting any kind of audio with an optional fade in
    public void PlaySound(string soundName, float volume = 1.0f, bool fadeIn = false, int fadeLength = 1)
    {
        Sound currentSound = null;

        //Checks if the Sound requested by the user exists
        foreach (Sound sound in sounds)
        {
            if (sound.name == soundName)
                currentSound = sound;
        }

        //Throws an error and exits the function if the Sound requested by the user doesn't exist
        if (currentSound == null)
        {
            Debug.LogError("The sound \"" + soundName + "\" can not be found");
            return;
        }

        //Checks if the first, and only, AudioSource is available (via isPlaying)
        if (GetComponents<AudioSource>().Length > 1 || GetComponents<AudioSource>()[0].isPlaying)
            currentSound.audioSource = gameObject.AddComponent<AudioSource>(); //Adds an AudioSource and assign it to currentSound

        //Uses the only AudioSource for the one most recently requested
        else
            currentSound.audioSource = GetComponent<AudioSource>();   

        //Applies all the settings from the Sound object to its own, current AudioSource component
        currentSound.audioSource.clip = currentSound.clip;
        currentSound.audioSource.loop = currentSound.loop;

        //Calculates and formats the track length for a field in the Inspector
        currentSound.length = currentSound.audioSource.clip.length.ToString("0.00") + " Seconds";

        //Used to reset the completion status of a completed Sound object (Used within CompletionChecker function)
        currentSound.hasCompleted = false;

        //Starts a fade in if specified by the user with their parameters
        if (fadeIn)
        {
            playingSounds.Add(currentSound);

            //Ensures the audio starts at 0 (The user specified setting will be used later in the coroutine)
            currentSound.audioSource.volume = 0;

            //Starts the audio before the fade in to prevent an abrupt start
            currentSound.audioSource.Play();
            StartCoroutine(CompletionChecker(currentSound));

            //Calls the Fader coroutine with user specified parameters
            StartCoroutine(Fader(currentSound, true, volume, fadeLength));
            return;
        }

        //Plays the Sound with the same settings as, but without, the fade in
        playingSounds.Add(currentSound);
        currentSound.audioSource.volume = volume;
        currentSound.audioSource.Play();
        StartCoroutine(CompletionChecker(currentSound));
    }

    //Responisble for stopping any kind of audio with an optional fade out
    public void StopSound(string soundName, bool fadeOut = false, int fadeLength = 1)
    {
        Sound currentSound = null;

        //Searches for a matching Sound in the sounds that are currently being played
        foreach (Sound sound in playingSounds)
        {
            if (sound.name == soundName)
            {
                currentSound = sound;
                break;
            }
        }

        //Throws an error and exits the function if the Sound is not currently being played
        if (currentSound == null)
        {
            Debug.LogWarning("The sound \"" + soundName + "\" is not currently being played");
            return;
        }

        //Starts the fade out with user specified settings
        if (fadeOut)
        {
            StartCoroutine(Fader(currentSound, false, currentSound.audioSource.volume, fadeLength));
            return;
        }

        //Stops the Sound with the same settings as, but without, the fade out
        playingSounds.Remove(currentSound);
        currentSound.audioSource.Stop();

        //Destroys the AudioSource if it isn't the last one (for optimization purposes)
        if (GetComponents<AudioSource>().Length > 1)
            Destroy(currentSound.audioSource);
    }

    /*---------------------------------------------------------------------------------------------
     *  COROUTINES: The Coroutines below logically run while the rest of the main code runs if they
     *  are called at any point within it. There may be a multitude of each of these Coroutines
     *  running at the same time
    ---------------------------------------------------------------------------------------------*/

    //Controls the fades for both PlaySound and StopSound
    private IEnumerator Fader(Sound currentSound, bool fadeInOrOut, float volume, int fadeLength)
    {
        //Starts the fade in for PlaySound
        if (fadeInOrOut)
        {
            
            fadingInSounds.Add(currentSound);

            //Loops until the user specified volume is reached
            while (currentSound.audioSource.volume < volume)
            {
                //Increases the volume over time, taking into consideration the target volume
                currentSound.audioSource.volume += Time.deltaTime / fadeLength * volume;
                yield return null;
            }

            fadingInSounds.Remove(currentSound);
        }
        //Starts the fade out for StopSound
        else if (!fadeInOrOut)
        {
            fadingOutSounds.Add(currentSound);

            //Gets the current volume in case the original volume was changed during runtime (allows fade out to still be timed correctly)
            float startingSound = currentSound.audioSource.volume;

            //Decreases the volume until it is inaudible
            while (currentSound.audioSource.volume > 0)
            {
                //Decreases the volume over time, taking into consideration the level the audio started to fade out at
                currentSound.audioSource.volume -= Time.deltaTime / fadeLength * startingSound;
                yield return null;
            }

            fadingOutSounds.Remove(currentSound);

            //Stops the current Sound once the fade out is complete
            playingSounds.Remove(currentSound);
            currentSound.audioSource.Stop();

            //Deletes the AudioSource unless i'ts the last one attached to the GameObject
            if (GetComponents<AudioSource>().Length > 1)
                Destroy(currentSound.audioSource);
        }
    }

    //Sets a bool to true if the audio has finished playing once
    private IEnumerator CompletionChecker(Sound currentSound)
    {
        //Sets certain fields in the Inspector window
        currentSound.playing = currentSound.audioSource.isPlaying;
        currentSound.paused = false;

        //Loop keeps running until the audio is complete
        while (!currentSound.hasCompleted && currentSound.audioSource != null)
        {
            //Updates certain fields and bools every frame to ensure proper functionality and that the proper information is dispayed in the inspector
            currentSound.volume = Mathf.FloorToInt(currentSound.audioSource.volume * 100);
            currentSound.audioSource.loop = currentSound.loop;

            //Stops the audio from being considered complete when it is paused
            if (currentSound.waiting)
            {
                currentSound.paused = true;
                yield return null;
                continue;
            }

            //Continues the completion check after the audio is resumed
            currentSound.paused = false;

            //Checks if the audio has stopped playing (note that the .waiting property checks ensures this isn't triggered when the audio is paused)
            if (!currentSound.audioSource.isPlaying)
            {
                //Updates certain fields and bools to ensure proper functionality and that the proper information is dispayed in the inspector
                currentSound.hasCompleted = true;
                currentSound.playing = currentSound.audioSource.isPlaying;

                //Removes the sound now that it has completed playing
                playingSounds.Remove(currentSound);

                //Deletes the AudioSource unless it's the last one attached to the GameObject
                if (GetComponents<AudioSource>().Length > 1)
                    Destroy(currentSound.audioSource);

                yield break;
            }

            //Updates an Inspector field only when the track is running (does not run when paused)
            currentSound.currentTime = currentSound.audioSource.time.ToString("0.00") + " Seconds";
            yield return null;
        }
    }

    /*---------------------------------------------------------------------------------------------
     * FUNCTIONS: The Functions below each allow for certain functionality which can affect one or
     * more sounds, or allow the user to access needed information. Anything can be added here and
     * it will not influence the main functionality of the AudioManager script.
     * 
     * IMPORTANT: You must use the pause and functions below instead of the direct AudioSource
     * methods to ensure that the hasCompleted boolean works properly
    ---------------------------------------------------------------------------------------------*/

    //Allows user to access any functionality from the Sound which includes all AudioSource methods and function
    public Sound GetSound(string soundName)
    {
        foreach (Sound currentSound in sounds)
            if (currentSound.name == soundName)
                return currentSound;

        //If no Sound is found
        Debug.LogWarning("The sound \"" + soundName + "\" can not be found");
        return null;
    }

    //Allows user to pause a specific Sound by name
    public void Pause(string soundName)
    {
        //Ensures that the requested Sound was actually playing
        foreach(Sound currentSound in playingSounds)
        {
            if (currentSound.name == soundName)
            {
                pausedSounds.Add(currentSound);

                //Used to pause the CompletionChecker coroutine and stop it from ending prematurely
                currentSound.waiting = true;

                currentSound.audioSource.Pause();
                playingSounds.Remove(currentSound);
                return;
            }
        }

        //If no Sound is found
        Debug.LogError("The sound \"" + soundName + "\" is not currently being played");
    }

    //Allows user to resume a specific Sound by name
    public void Resume(string soundName)
    {
        //Ensures the requested Sound was
        foreach (Sound currentSound in pausedSounds)
        {
            if (currentSound.name == soundName)
            {
                playingSounds.Add(currentSound);
                currentSound.audioSource.Play();

                //Used to resume the CompletionChecker
                currentSound.waiting = false;

                pausedSounds.Remove(currentSound);
                return;
            }
        }

        //If no Sound is found
        Debug.LogError("The sound \"" + soundName + "\" is not currently paused");
    }

    //Allows user to pause each Sound that is currently playing
    public void PauseAll()
    {
        foreach (Sound currentSound in playingSounds)
        {
            pausedSounds.Add(currentSound);

            //Ensures that each Sounds CompletionChecker is paused and does not end prematurely
            currentSound.waiting = true;

            currentSound.audioSource.Pause();
        }

        //Clears the playingSounds ArrayList after each Sound is paused
        playingSounds.Clear();
    }

    public void ResumeAll()
    {
        foreach (Sound currentSound in pausedSounds)
        {
            playingSounds.Add(currentSound);
            currentSound.audioSource.Play();

            //Ensures that each Sounds CompletionChecker is resumed
            currentSound.waiting = false;
        }
        
        //Clears the pausedSounds ArrayList after each Sound is resumed
        pausedSounds.Clear();
    }
}