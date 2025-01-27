using UnityEngine;

public class MusicEndChecker : MonoBehaviour
{
    public AudioSource music;
    /* public PauseMenuScript pauseMenuScript; */
    public float delay = 1f; // Same delay as in PlayMusicWithDelay

    private bool musicStarted = false;

    void Start()
    {
        // Start checking for music end after the delay
        Invoke("StartChecking", delay);
    }

    void StartChecking()
    {
        musicStarted = true;
    }

    void Update()
    {
        if (musicStarted && !music.isPlaying)
        {
            // Call TriggerGameOver on the PauseMenuScript
            /* pauseMenuScript.TriggerGameOver(); */
            // Disable this script to prevent multiple calls
            enabled = false;
        }
    }
}
