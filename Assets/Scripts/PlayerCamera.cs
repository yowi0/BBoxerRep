using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour // Canvia el nom de la classe
{
    public float mouseSensitivity = 100f; // Sensibilitat del moviment del ratolí
    private float xRotation = 0f; // Control de rotació en l'eix X

    private void Start()
    {
        // Amaga el cursor i el bloqueja al centre de la pantalla
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Obtenció del moviment del ratolí
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        
        // Actualització de la rotació vertical (amunt/avall) i limitació a +/- 90 graus
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Limitem la rotació vertical per evitar girs excessius
        
        // Aplicar la rotació local de la càmera: horitzontal (Y) i vertical (X)
        transform.localRotation = Quaternion.Euler(xRotation, transform.localEulerAngles.y + mouseX, 0f);
        
        // Si el jugador prem el botó esquerre del ratolí (per exemple, per accions com disparar)
        if (Input.GetMouseButtonDown(0))
        {
            // Aquí podries afegir accions per a la càmera, si cal
            // (com activar una funció específica o efectes visuals)
        }
    }
}
