using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Este script hace que la cámara siga al jugador suavemente
public class Camara_Controller : MonoBehaviour
{
    public Transform objetivo; // Objeto que la cámara debe seguir (por lo general, el jugador)
    public float velocidadCamara = 0.025f; // Qué tan rápido se mueve la cámara hacia el objetivo
    public Vector3 desplazamiento; // Desplazamiento desde el objetivo (para ajustar la posición de la cámara)

    private void LateUpdate()
    {
        // Calcula la posición que queremos que tenga la cámara (objetivo + desplazamiento)
        Vector3 posicionDeseada = objetivo.position + desplazamiento;

        // Suaviza el movimiento de la cámara usando Lerp (interpolación lineal)
        Vector3 posicionSuavizada = Vector3.Lerp(transform.position, posicionDeseada, velocidadCamara);

        // Aplica la nueva posición suavizada a la cámara
        transform.position = posicionSuavizada;
    }
}
