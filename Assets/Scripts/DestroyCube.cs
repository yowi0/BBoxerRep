using UnityEngine;

public class DestroyCube : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("CubeStartGame triggered 2");

        if (other.CompareTag("CubeStartGame"))
        {
            Debug.Log("CubeStartGame triggered");

            // Destrueix el cub
            Destroy(gameObject);
        }
    }
}
