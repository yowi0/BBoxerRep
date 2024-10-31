using System.Collections;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    public GameObject touchEffect; // Efecte visual quan es toca el cub
    private bool hasBeenTouched = false; // Controla si el cub ha estat tocat
    public static int score = 0; // Variable per la puntuació global

    private Vector3 moveDirection = Vector3.back;
    public float moveSpeed;

    private void Start()
    {
        StartCoroutine(FailureCheck()); // Comprova si el cub no ha estat tocat dins del temps límit
    }

    private IEnumerator FailureCheck()
    {
        yield return new WaitForSeconds(1.8f);

        if (gameObject.activeSelf && !hasBeenTouched)
        {
            Destroy(gameObject); // Destrueix el cub si no ha estat tocat
        }
    }

    private void Update()
    {
        // Mou el cub cap a l'objectiu
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // Destrueix el cub si surt de la zona de joc
        if (transform.position.magnitude > 50)
        {
            Destroy(gameObject);
        }
    }

    private void Awake()
    {
        Destroy(gameObject, 5); // Elimina el cub després de 5 segons per evitar que ocupi memòria innecessària
    }

    // Funció per quan es toca el cub
    public void MarkAsTouched()
    {
        if (!hasBeenTouched)
        {
            hasBeenTouched = true;
            score += 10; // Incrementa la puntuació

            // Mostra l'efecte visual al tocar
            if (touchEffect != null)
            {
                Instantiate(touchEffect, transform.position, Quaternion.identity);
            }

            // Destrueix el cub després de tocar-lo
            Destroy(gameObject);
        }
    }
}
