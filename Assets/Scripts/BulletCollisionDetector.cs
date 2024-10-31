using UnityEngine;
using Firebase;
using Firebase.Database;
using System;
using TMPro;

public class BulletCollisionDetector : MonoBehaviour
{
    private DatabaseReference reference;
    private DisplayPlayerData displayPlayerData;

    public GameObject AddscorePopupPrefab; // Prefab del Canvas con el texto "+5"
    public GameObject RemovescorePopupPrefab;
    private HealthBar healthBar; // Cambiar a privado

    private bool collisionHandled = false; // Añadir esta línea

    void Start()
    {
        // Initialize the Firebase database reference
        reference = FirebaseDatabase.DefaultInstance.RootReference;

        // Obtain the DisplayPlayerData component
        displayPlayerData = FindObjectOfType<DisplayPlayerData>();

        // Find the HealthBar component in the scene
        healthBar = FindObjectOfType<HealthBar>();

        // Verificar que el prefab esté asignado
        if (AddscorePopupPrefab == null)
        {
            Debug.LogError("Add Score Popup Prefab is not assigned!");
        }
        if (RemovescorePopupPrefab == null)
        {
            Debug.LogError("Remove Score Popup Prefab is not assigned!");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collisionHandled) return;
        collisionHandled = true;

        Debug.Log("CollisionEntered test");

        // Initialize score increment variable
        int scoreIncrement = 0;

        // Check the tag of the collided object and set the score increment
        if (collision.gameObject.CompareTag("Diana_White"))
        {
            Debug.Log("Collision with Diana_White");
            scoreIncrement = 1;
        }
        else if (collision.gameObject.CompareTag("Diana_Black"))
        {
            Debug.Log("Collision with Diana_Black");
            scoreIncrement = 3;
        }
        else if (collision.gameObject.CompareTag("Diana_Blue"))
        {
            Debug.Log("Collision with Diana_Blue");
            scoreIncrement = 5;
        }
        else if (collision.gameObject.CompareTag("Diana_Red"))
        {
            Debug.Log("Collision with Diana_Red");
            scoreIncrement = 10;
        }
        else if (collision.gameObject.CompareTag("Diana_Yellow"))
        {
            Debug.Log("Collision with Diana_Yellow");
            scoreIncrement = 20;
        }
        else if (collision.gameObject.CompareTag("Sliceable"))
        {
            Debug.Log("Collision with Cube");
            UpdatePlayerScore(-5);
            Destroy(collision.transform.parent.gameObject);
            GameObject popup = Instantiate(RemovescorePopupPrefab, collision.transform.position, Quaternion.identity);
            Canvas popupCanvas = popup.GetComponent<Canvas>();
            if (popupCanvas != null)
            {
                popupCanvas.worldCamera = Camera.main;
            }

            TMP_Text popupText = popup.GetComponentInChildren<TMP_Text>();
            popupText.text = "-5";
            if (healthBar != null)
            {
                healthBar.TakeDamage();
            }
            Destroy(popup, 1.0f); // Destruir el popup después de 1 segundo
        }

        // If scoreIncrement is greater than 0, update the player's score in Firebase
        if (scoreIncrement > 0)
        {

            // Mostrar texto "+5"
            ShowScorePopup(collision.transform.position, scoreIncrement);

            UpdatePlayerScore(scoreIncrement);
            // Destroy the parent of the collided object
            Destroy(collision.transform.parent.gameObject);
        }

        // Destroy the bullet
        Destroy(gameObject);
    }

    private void ShowScorePopup(Vector3 position, int scoreIncrement)
    {
        if (AddscorePopupPrefab == null)
        {
            Debug.LogError("Score Popup Prefab is not assigned!");
            return;
        }

        GameObject popup = Instantiate(AddscorePopupPrefab, position, Quaternion.identity);

        // Asignar la cámara al Canvas del popup
        Canvas popupCanvas = popup.GetComponent<Canvas>();
        if (popupCanvas != null)
        {
            popupCanvas.worldCamera = Camera.main;
        }

        TMP_Text popupText = popup.GetComponentInChildren<TMP_Text>();
        popupText.text = "+" + scoreIncrement.ToString();
        Destroy(popup, 1.0f); // Destruir el popup después de 1 segundo
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
