using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Esta clase se encarga de manejar los puntos del jugador y mostrarlos en pantalla
public class Puntos : MonoBehaviour
{
    // Variable pública que almacena la cantidad de puntos
    public float puntos;

    // Referencia al componente de texto en la UI (TextMeshPro)
    private TextMeshProUGUI textMesh;

    // Al iniciar, obtiene el componente TextMeshProUGUI que está en el mismo objeto
    private void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    // Cada frame actualiza el texto de la UI con el valor actual de los puntos
    private void Update()
    {
        textMesh.text = puntos.ToString("0"); // Muestra los puntos como número entero
    }

    // Método público para sumar puntos desde otras partes del código
    public void SumarPuntos(float puntosEntrada)
    {
        puntos += puntosEntrada; // Suma los puntos recibidos al total
    }
}
