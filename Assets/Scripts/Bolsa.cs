using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bolsa : MonoBehaviour
{
    public float puntos = 5f;
    public Puntos puntosScript;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (puntosScript != null)
            {
                puntosScript.SumarPuntos(puntos);
            }
            Destroy(gameObject);
        }
    }
}
