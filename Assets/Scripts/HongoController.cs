using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Este script controla el comportamiento del enemigo tipo "hongo"
public class HongoController : MonoBehaviour
{
    public int vida = 2; // Vida del hongo
    public Transform player; // Referencia al jugador para poder seguirlo
    public float detectionRadius = 8.0f; // Distancia a la que el hongo detecta al jugador
    public float speed = 5.0f; // Velocidad de movimiento del hongo
    public float fuerzaRebote = 10f; // Fuerza con la que el hongo es empujado al recibir daño

    public Puntos puntosScript; // Referencia al script de puntos
    public float puntosAlMorir = 100; // Puntos que se otorgan al jugador al derrotar al hongo

    private Rigidbody2D rb;
    private Vector2 movement; // Dirección de movimiento
    private bool enMovimiento;
    private bool recibiendoDanio;
    private bool playerVivo;
    private bool muerto;

    private Animator animator;
    private Vector3 escalaOriginal; // Escala original para invertirla y girar el sprite

    void Start()
    {
        playerVivo = true;
        rb = GetComponent<Rigidbody2D>(); // Obtiene el componente Rigidbody2D del hongo
        animator = GetComponent<Animator>(); // Obtiene el componente Animator del hongo
        escalaOriginal = transform.localScale; // Guarda la escala original
    }

    void Update()
    {
        // Solo se mueve si el jugador está vivo y el hongo no ha muerto
        if (playerVivo && !muerto)
        {
            Movimiento();
        }

        // Actualiza los parámetros del Animator
        animator.SetBool("enMovimiento", enMovimiento);
        animator.SetBool("recibeDanio", recibiendoDanio);
        animator.SetBool("Muerto", muerto);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Si colisiona con el jugador, le causa daño
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 direccionDanio = new Vector2(transform.position.x, 0); // Dirección desde donde viene el daño

            Player_Movimiento player_Movimiento = collision.gameObject.GetComponent<Player_Movimiento>();

            player_Movimiento.RecibeDanio(direccionDanio, 1);
            playerVivo = !player_Movimiento.muerto; // Verifica si el jugador murió
            if (!playerVivo)
            {
                enMovimiento = false;
            }
        }
    }

    public void Movimiento()
    {
        // Calcula la distancia entre el hongo y el jugador
        float ditanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Si el jugador está dentro del rango de detección, lo sigue
        if (ditanceToPlayer < detectionRadius)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            movement = new Vector2(direction.x, 0);
            enMovimiento = true;
        }
        else
        {
            movement = Vector2.zero;
            enMovimiento = false;
        }

        // Gira el sprite del hongo según la dirección de movimiento
        if (movement.x > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(escalaOriginal.x), escalaOriginal.y, escalaOriginal.z);
        }
        else if (movement.x < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(escalaOriginal.x), escalaOriginal.y, escalaOriginal.z);
        }

        // Se mueve solo si no está recibiendo daño
        if (!recibiendoDanio)
        {
            rb.MovePosition(rb.position + movement * speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si es golpeado por la espada, recibe daño
        if (collision.CompareTag("Espada"))
        {
            Vector2 direccionDanio = new Vector2(collision.gameObject.transform.position.x, 0);
            RecibeDanio(direccionDanio, 1);
        }
    }

    public void RecibeDanio(Vector2 direccion, int cantDanio)
    {
        if (!recibiendoDanio)
        {
            vida -= cantDanio; // Reduce la vida
            recibiendoDanio = true;
            if (vida <= 0)
            {
                // Si su vida llega a 0, muere y otorga puntos al jugador
                muerto = true;
                enMovimiento = false;
                if (puntosScript != null)
                {
                    puntosScript.SumarPuntos(puntosAlMorir);
                }
            }
            else
            {
                // Si no muere, hace un rebote en la dirección opuesta
                Vector2 rebote = new Vector2(transform.position.x - direccion.x, 0.5f).normalized;
                rb.AddForce(rebote * fuerzaRebote, ForceMode2D.Impulse);
                rb.velocity = Vector2.zero;
            }
        }
    }

    void EliminarCuerpo()
    {
        // Elimina el objeto del juego (se puede llamar con animación de muerte)
        Destroy(gameObject);
    }

    void DesactivaDanio()
    {
        // Permite que el hongo vuelva a moverse y recibir daño
        recibiendoDanio = false;
        rb.MovePosition(rb.position + movement * speed * Time.deltaTime);
    }
}
