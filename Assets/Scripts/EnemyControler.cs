﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Este script controla el comportamiento de un enemigo que persigue al jugador,
// puede recibir daño, morir y otorgar puntos al morir.
public class EnemyControler : MonoBehaviour
{
    public int vida = 5; // Vida inicial del enemigo
    public Transform player; // Referencia al jugador
    public float detectionRadius = 8.0f; // Radio de detección para perseguir al jugador
    public float speed = 5.0f; // Velocidad de movimiento del enemigo
    public float fuerzaRebote = 10f; // Fuerza con la que el enemigo es empujado al recibir daño

    public Puntos puntosScript; // Referencia al script que maneja los puntos
    public float puntosAlMorir = 100; // Puntos que se otorgan al morir

    private Rigidbody2D rb; // Rigidbody2D del enemigo
    private Vector2 movement; // Dirección de movimiento
    private bool enMovimiento; // Si el enemigo se está moviendo
    private bool recibiendoDanio; // Si el enemigo está recibiendo daño
    private bool playerVivo; // Si el jugador está vivo
    private bool muerto; // Si el enemigo está muerto

    private Animator animator; // Referencia al Animator
    private Vector3 escalaOriginal; // Escala original del enemigo (para voltear sprite)

    void Start()
    {
        playerVivo = true; // Se asume que el jugador inicia vivo
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        escalaOriginal = transform.localScale;
    }

    void Update()
    {
        // Solo se mueve si el jugador está vivo y el enemigo no ha muerto
        if (playerVivo && !muerto)
        {
            Movimiento();
        }

        // Actualiza los parámetros de animación
        animator.SetBool("enMovimiento", enMovimiento);
        animator.SetBool("recibeDanio", recibiendoDanio);
        animator.SetBool("Muerto", muerto);
    }

    // Cuando el enemigo colisiona con el jugador
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 direccionDanio = new Vector2(transform.position.x, 0);

            // Obtiene el script del jugador y le aplica daño
            Player_Movimiento player_Movimiento = collision.gameObject.GetComponent<Player_Movimiento>();
            player_Movimiento.RecibeDanio(direccionDanio, 1);

            // Revisa si el jugador murió y detiene el movimiento si es así
            playerVivo = !player_Movimiento.muerto;
            if (!playerVivo)
            {
                enMovimiento = false;
            }
        }
    }

    // Movimiento del enemigo hacia el jugador si está dentro del rango de detección
    public void Movimiento()
    {
        float ditanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (ditanceToPlayer < detectionRadius)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            movement = new Vector2(direction.x, 0); // Solo se mueve en X
            enMovimiento = true;
        }
        else
        {
            movement = Vector2.zero;
            enMovimiento = false;
        }

        // Cambia la dirección del sprite según la dirección de movimiento
        if (movement.x > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(escalaOriginal.x), escalaOriginal.y, escalaOriginal.z);
        }
        else if (movement.x < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(escalaOriginal.x), escalaOriginal.y, escalaOriginal.z);
        }

        // Si no está recibiendo daño, puede moverse
        if (!recibiendoDanio)
        {
            rb.MovePosition(rb.position + movement * speed * Time.deltaTime);
        }
    }

    // Cuando el enemigo es golpeado por el ataque del jugador (espada)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Espada"))
        {
            Vector2 direccionDanio = new Vector2(collision.gameObject.transform.position.x, 0);
            RecibeDanio(direccionDanio, 1);
        }
    }

    // Lógica para recibir daño
    public void RecibeDanio(Vector2 direccion, int cantDanio)
    {
        if (!recibiendoDanio)
        {
            vida -= cantDanio;
            recibiendoDanio = true;

            // Si la vida llega a 0, muere
            if (vida <= 0)
            {
                muerto = true;
                enMovimiento = false;

                // Suma puntos al jugador si el script está asignado
                if (puntosScript != null)
                {
                    puntosScript.SumarPuntos(puntosAlMorir);
                }
            }
            else
            {
                // Si no muere, aplica rebote y detiene el movimiento momentáneamente
                Vector2 rebote = new Vector2(transform.position.x - direccion.x, 0.5f).normalized;
                rb.AddForce(rebote * fuerzaRebote, ForceMode2D.Impulse);
                rb.velocity = Vector2.zero;
            }
        }
    }

    // Método llamado por animación para destruir al enemigo al morir
    void EliminarCuerpo()
    {
        Destroy(gameObject);
    }

    // Método llamado por animación para permitir que el enemigo se mueva nuevamente
    void DesactivaDanio()
    {
        recibiendoDanio = false;
        rb.MovePosition(rb.position + movement * speed * Time.deltaTime);
    }
}
