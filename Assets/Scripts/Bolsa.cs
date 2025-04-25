using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Este script otorga puntos al jugador cuando recoge una bolsa
public class Bolsa : MonoBehaviour
{
    public float puntos = 5f; // Cantidad de puntos que da la bolsa
    public Puntos puntosScript; // Referencia al script que maneja los puntos del jugador

    // Este método se activa cuando otro objeto entra en el área de la bolsa (collider con trigger)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica si el objeto que entró tiene la etiqueta "Player"
        if (collision.CompareTag("Player"))
        {
            // Si el script de puntos está asignado, suma los puntos
            if (puntosScript != null)
            {
                puntosScript.SumarPuntos(puntos);
            }

            // Destruye la bolsa después de recogerla
            Destroy(gameObject);
        }
    }
}
