using UnityEngine;
using TMPro;
using Firebase;
using Firebase.Database;
using System;

public class DisplayPlayerData : MonoBehaviour
{
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text playerScoreText;
    [SerializeField] private TMP_Text playerHighScoreText;

    private DatabaseReference reference;

    void Awake()
    {
        // Initialize the Firebase database reference
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    void Start()
    {
        // Reassign TMP_Text references if not already assigned
        if (playerNameText == null)
        {
            playerNameText = GameObject.Find("PlayerNameText").GetComponent<TMP_Text>();
        }
        if (playerScoreText == null)
        {
            playerScoreText = GameObject.Find("PlayerScoreText").GetComponent<TMP_Text>();
        }
        if (playerHighScoreText == null)
        {
            playerHighScoreText = GameObject.Find("PlayerHighScoreText").GetComponent<TMP_Text>();
        }

        // Get the current player's data
        ObtenerDatosJugador();
    }

    // Method to get the player's data from the database
    private void ObtenerDatosJugador()
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
                    // Get the player's score
                    int score = Convert.ToInt32(playerSnapshot.Child("Score").Value);

                    // Get the player's high score
                    int highScore = Convert.ToInt32(playerSnapshot.Child("HighScore").Value);

                    // Update the text on the canvas with the obtained values
                    Debug.Log("First ActualizarTexto with playerName: " + PlayerInfo.PlayerName);
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        ActualizarTexto(PlayerInfo.PlayerName, score, highScore);
                    });
                    break;
                }
            }
        });
    }

    public void ActualizarTexto(string playerName, int score, int highScore)
    {
        Debug.Log("Text updated with playerName: " + playerName);
        // Update the text on the canvas with the obtained values
        if (playerNameText != null && playerScoreText != null && playerHighScoreText != null)
        {
            playerNameText.text = playerName;
            playerScoreText.text = score.ToString();
            playerHighScoreText.text = highScore.ToString();

            Debug.Log("Updated Player: " + playerName);
            Debug.Log("Updated Score: " + score.ToString());
            Debug.Log("Updated HighScore: " + highScore.ToString());
        }
        else
        {
            Debug.LogError("One or more TMP_Text references are not set.");
        }
    }
}
