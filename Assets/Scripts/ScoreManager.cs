using UnityEngine;
using TMPro;
using Firebase;
using Firebase.Database;
using System;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    private DatabaseReference reference;
    private DisplayPlayerData displayPlayerData;

    public GameObject scorePopupPrefab; // Prefab del Canvas amb el text "+10"

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Initialize the Firebase database reference
        reference = FirebaseDatabase.DefaultInstance.RootReference;

        // Obtain the DisplayPlayerData component
        displayPlayerData = FindObjectOfType<DisplayPlayerData>();

        // Verificar que el prefab esté asignado
        if (scorePopupPrefab == null)
        {
            Debug.LogError("Score Popup Prefab is not assigned!");
        }
    }

    public void AddScore(Vector3 position, int scoreIncrement)
    {
        ShowScorePopup(position, scoreIncrement);
        UpdatePlayerScore(scoreIncrement);
    }

    private void ShowScorePopup(Vector3 position, int scoreIncrement)
    {
        if (scorePopupPrefab == null)
        {
            Debug.LogError("Score Popup Prefab is not assigned!");
            return;
        }

        GameObject popup = Instantiate(scorePopupPrefab, position, Quaternion.identity);

        // Asignar la cámara al Canvas del popup
        Canvas popupCanvas = popup.GetComponent<Canvas>();
        if (popupCanvas != null)
        {
            popupCanvas.worldCamera = Camera.main;
        }

        TMP_Text popupText = popup.GetComponentInChildren<TMP_Text>();
        popupText.text = "+" + scoreIncrement.ToString();
        Destroy(popup, 1.0f); // Destruir el popup després de 1 segon
    }

    private void UpdatePlayerScore(int scoreIncrement)
    {
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

                    // Get the current score and high score
                    int currentScore = Convert.ToInt32(playerSnapshot.Child("Score").Value);
                    int currentHighScore = Convert.ToInt32(playerSnapshot.Child("HighScore").Value);

                    // Update the score
                    reference.Child("Players").Child(playerKey).Child("Score").SetValueAsync(currentScore + scoreIncrement).ContinueWith(updateTask =>
                    {
                        if (updateTask.IsFaulted)
                        {
                            Debug.LogError("Error updating score: " + updateTask.Exception);
                        }
                        else
                        {
                            Debug.Log("Score updated successfully");

                            // Update the PlayerInfo Score
                            PlayerInfo.Score = currentScore + scoreIncrement;

                            // Check and update high score if needed
                            if (PlayerInfo.Score > currentHighScore)
                            {
                                PlayerInfo.HighScore = PlayerInfo.Score;
                                reference.Child("Players").Child(playerKey).Child("HighScore").SetValueAsync(PlayerInfo.HighScore);
                            }

                            // Call the ActualizarTexto method on DisplayPlayerData with the new score and high score values
                            UnityMainThreadDispatcher.Instance().Enqueue(() =>
                            {
                                displayPlayerData.ActualizarTexto(PlayerInfo.PlayerName, PlayerInfo.Score, PlayerInfo.HighScore);
                            });
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
}
