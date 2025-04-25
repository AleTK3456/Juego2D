using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Este script se encarga de dar puntos al jugador cuando recoge una moneda
public class Moneda : MonoBehaviour
{
    public float puntos = 5f; // Cantidad de puntos que da esta moneda
    public Puntos puntosScript; // Referencia al script que maneja los puntos del jugador

    // Este método se activa cuando otro objeto entra en el collider de la moneda (debe ser un trigger)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica si el objeto que colisionó tiene la etiqueta "Player"
        if (collision.CompareTag("Player"))
        {
            // Si el script de puntos está asignado, suma los puntos
            if (puntosScript != null)
            {
                puntosScript.SumarPuntos(puntos);
            }

            // Destruye la moneda después de ser recogida
            Destroy(gameObject);
        }
    }
}