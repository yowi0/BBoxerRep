using UnityEngine;

public class DestroyCube : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("CubeStartGame triggered 2");
        if (other.CompareTag("CubeStartGame"))
        {
            Debug.Log("CubeStartGame triggered");
            // Llamar a la función para destruir el cubo y cargar la escena en PassScene
            PassScene passSceneScript = FindObjectOfType<PassScene>();
            if (passSceneScript != null)
            {
                passSceneScript.DestroyCube2();
            }
        }
    }
}
