using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Degradado : MonoBehaviour
{
    public float sangreInicial = 20;
    public float sangreActual = 20;
    public Gradient gradienteSangre;
    private MeshRenderer miMeshRenderer; // Variable para almacenar el MeshRenderer del objeto que contiene el script

    void Start()
    {
        // Obtén el MeshRenderer del objeto que contiene el script
        miMeshRenderer = GetComponent<MeshRenderer>();
        
        // Verifica si se encontró el MeshRenderer
        if (miMeshRenderer == null)
        {
            Debug.LogError("No se encontró MeshRenderer en el objeto que contiene el script.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            sangreActual--;
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            sangreActual++;
        }
        sangreActual = Mathf.Clamp(sangreActual, 0, sangreInicial);

        // Evalúa el gradiente para obtener el color
        Color color = gradienteSangre.Evaluate(sangreActual / sangreInicial);

        // Asigna el color al material del objeto
        if (miMeshRenderer != null)
        {
            miMeshRenderer.material.color = color;
        }
    }
}
