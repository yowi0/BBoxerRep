using UnityEngine;
using TMPro;
using Firebase.Database;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PassScene : MonoBehaviour
{
    [SerializeField] private GameObject cube;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private int sceneNumber; // Número de la escena a cargar

    void Update()
    {
        // Verificar si se ha ingresado un nombre de 5 letras
        if (inputField.text.Length == 5)
        {
            cube.SetActive(true); // Activar el cubo si hay 5 letras ingresadas
        }
        else
        {
            cube.SetActive(false); // Desactivar el cubo si no hay 5 letras ingresadas
        }
    }

    // Método para destruir el cubo y cargar la siguiente escena
    public void DestroyCube2()
    {
        // Destruir el cubo después de un retraso de 3 segundos
        LoadScene();
    }

    // Método para cargar la siguiente escena
    private void LoadScene()
    {
        CrearNuevoJugador(inputField.text);
        PlayerInfo.PlayerName = inputField.text; // Store player name in static class
        SceneManager.LoadScene(sceneNumber);
    }

    // Método para crear un nuevo jugador en la base de datos
    private void CrearNuevoJugador(string playerName)
    {
        // Obtener la referencia a la base de datos
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

        // Crear un nuevo jugador con los valores especificados
        Jugador nuevoJugador = new Jugador
        {
            Name = playerName,
            HighScore = 0,
            Score = 0
        };

        // Convertir el nuevo jugador a un diccionario
        var jugadorDict = nuevoJugador.ToDictionary();

        // Crear el nuevo jugador en la base de datos
        reference.Child("Players").Push().SetValueAsync(jugadorDict).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Nuevo jugador creado en la base de datos.");
            }
            else
            {
                Debug.LogError("Error al crear el nuevo jugador en la base de datos: " + task.Exception);
            }
        });
    }

    // Clase para representar un jugador
    public class Jugador
    {
        public string Name;
        public int HighScore;
        public int Score;

        // Convertir el jugador a un diccionario
        public Dictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            result["Name"] = Name;
            result["HighScore"] = HighScore;
            result["Score"] = Score;
            return result;
        }
    }
}
