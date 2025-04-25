using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControler : MonoBehaviour
{
    public int vida = 5;
    public Transform player;
    public float detectionRadius = 8.0f;
    public float speed = 5.0f;
    public float fuerzaRebote = 10f;

    public Puntos puntosScript; // Referencia al script de puntos
    public float puntosAlMorir = 100; // Puntos que se suman al morir

    private Rigidbody2D rb;
    private Vector2 movement;
    private bool enMovimiento;
    private bool recibiendoDanio;
    private bool playerVivo;
    private bool muerto;

    private Animator animator;
    private Vector3 escalaOriginal;

    void Start()
    {
        playerVivo = true;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        escalaOriginal = transform.localScale;
    }

    void Update()
    {
        if (playerVivo && !muerto)
        {
            Movimiento();
        }

        animator.SetBool("enMovimiento", enMovimiento);
        animator.SetBool("recibeDanio", recibiendoDanio);
        animator.SetBool("Muerto", muerto);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 direccionDanio = new Vector2(transform.position.x, 0);

            Player_Movimiento player_Movimiento = collision.gameObject.GetComponent<Player_Movimiento>();

            player_Movimiento.RecibeDanio(direccionDanio, 1);
            playerVivo = !player_Movimiento.muerto;
            if (!playerVivo)
            {
                enMovimiento = false;
            }
        }
    }

    public void Movimiento()
    {
        float ditanceToPlayer = Vector2.Distance(transform.position, player.position);

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

        // Voltear con el jugador sin modificar el tamaño original
        if (movement.x > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(escalaOriginal.x), escalaOriginal.y, escalaOriginal.z);
        }
        else if (movement.x < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(escalaOriginal.x), escalaOriginal.y, escalaOriginal.z);
        }

        if (!recibiendoDanio)
        {
            rb.MovePosition(rb.position + movement * speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
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
            vida -= cantDanio;
            recibiendoDanio = true;
            if (vida <= 0)
            {
                muerto = true;
                enMovimiento = false;
                if (puntosScript != null)
                {
                    puntosScript.SumarPuntos(puntosAlMorir); // Sumar puntos al morir
                }
            }
            else
            {
                Vector2 rebote = new Vector2(transform.position.x - direccion.x, 0.5f).normalized;
                rb.AddForce(rebote * fuerzaRebote, ForceMode2D.Impulse);
                rb.velocity = Vector2.zero;
            }
        }
    }

    void EliminarCuerpo()
    {
        Destroy(gameObject);
    }

    void DesactivaDanio()
    {
        recibiendoDanio = false;
        rb.MovePosition(rb.position + movement * speed * Time.deltaTime);
    }
}
