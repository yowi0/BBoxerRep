using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Firebase.Database;

public class PauseMenuScript : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public bool activePauseUI = false;
    public AudioSource music;

    private DatabaseReference reference;
    private bool isGameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the Firebase database reference
        reference = FirebaseDatabase.DefaultInstance.RootReference;

        DisplayUI();
    }

    public void DisplayUI()
    {
        if (isGameOver) return;

        if (activePauseUI)
        {
            pauseMenuUI.SetActive(false);
            activePauseUI = false;
            Time.timeScale = 1;
            music.UnPause();
        }
        else
        {
            pauseMenuUI.SetActive(true);
            activePauseUI = true;
            Time.timeScale = 0;
            music.Pause();
        }
    }

    public void PauseButtonPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            DisplayUI();
        }
    }

    public void RestartGame()
    {
        Debug.Log("Try to restart");
        // Check if player name is set
        if (string.IsNullOrEmpty(PlayerInfo.PlayerName))
        {
            Debug.LogError("Player name is not set.");
            return;
        }

        // Query the database to get the current player's data
        reference.Child("Players").OrderByChild("Name").EqualTo(PlayerInfo.PlayerName).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error getting player data: " + task.Exception);
                return;
            }

            // Get the player data from the query result
            DataSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                foreach (DataSnapshot playerSnapshot in snapshot.Children)
                {
                    // Get the player's key
                    string playerKey = playerSnapshot.Key;

                    // Set the score to 0
                    reference.Child("Players").Child(playerKey).Child("Score").SetValueAsync(0).ContinueWith(updateTask =>
                    {
                        if (updateTask.IsFaulted)
                        {
                            Debug.LogError("Error resetting score: " + updateTask.Exception);
                        }
                        else
                        {
                            Debug.Log("Score reset successfully");

                            // Reset the PlayerInfo score
                            PlayerInfo.ResetScore();

                        }
                    });

                    break;
                }
            }
            else
            {
                Debug.LogError("Player not found.");
            }
        });
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    public void TriggerGameOver()
    {
        isGameOver = true;
        pauseMenuUI.SetActive(true);
        activePauseUI = true;
        Time.timeScale = 0;
        music.Pause();
    }
}
