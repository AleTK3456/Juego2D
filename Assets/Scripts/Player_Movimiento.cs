using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Este script controla el movimiento, salto, ataque y daño del jugador
public class Player_Movimiento : MonoBehaviour
{
    public float velocidad = 5f; // Velocidad de movimiento
    public int vida = 3; // Vida del jugador

    public float fuerzaSalto = 10f; // Fuerza con la que el jugador salta
    public float fuerzaRebote = 10f; // Fuerza del rebote al recibir daño

    public float longitudRaycast = 0.1f; // Longitud del raycast hacia abajo para detectar el suelo
    public LayerMask capaSuelo; // Capa que se considera como suelo

    private bool enSuelo; // Si el jugador está tocando el suelo
    private bool recibiendoDanio; // Si el jugador está recibiendo daño
    private bool atacando; // Si el jugador está atacando
    public bool muerto; // Si el jugador está muerto
    private Rigidbody2D rb; // Componente Rigidbody2D del jugador

    public Animator animator; // Referencia al componente Animator

    private Vector3 escalaOriginal; // Guarda la escala original del jugador

    void Start()
    {
        escalaOriginal = transform.localScale; // Guarda la escala original
        rb = GetComponent<Rigidbody2D>(); // Obtiene el Rigidbody2D del jugador
    }

    void Update()
    {
        if (!muerto)
        {
            if (!atacando)
            {
                Movimiento(); // Ejecuta el movimiento del jugador

                // Raycast hacia abajo para detectar si está en el suelo
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, longitudRaycast, capaSuelo);
                enSuelo = hit.collider != null;

                // Salta si está en el suelo, se presiona espacio y no está recibiendo daño
                if (enSuelo && Input.GetKeyDown(KeyCode.Space) && !recibiendoDanio)
                {
                    rb.AddForce(new Vector2(0f, fuerzaSalto), ForceMode2D.Impulse);
                }

                // Ataca si está en el suelo y se presiona la tecla J
                if (Input.GetKeyDown(KeyCode.J) && !atacando && enSuelo)
                {
                    Atacando();
                }
            }
        }

        Animaciones(); // Actualiza las animaciones
    }

    public void Animaciones()
    {
        // Cambia los estados del animator según las variables
        animator.SetBool("ensuelo", enSuelo);
        animator.SetBool("recibeDanio", recibiendoDanio);
        animator.SetBool("Atacando", atacando);
        animator.SetBool("Muerto", muerto);
    }

    public void Movimiento()
    {
        // Obtiene el valor del eje horizontal (A, D o flechas)
        float velocidadX = Input.GetAxis("Horizontal") * Time.deltaTime * velocidad;

        // Cambia la animación de movimiento según la velocidad
        animator.SetFloat("Movimiento", Mathf.Abs(velocidadX * velocidad));

        // Cambia la dirección del sprite según el movimiento
        if (velocidadX > 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(escalaOriginal.x), escalaOriginal.y, escalaOriginal.z);
        }
        else if (velocidadX < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(escalaOriginal.x), escalaOriginal.y, escalaOriginal.z);
        }

        Vector3 posicion = transform.position;

        // Si no está recibiendo daño, permite moverse
        if (!recibiendoDanio)
        {
            transform.position = new Vector3(posicion.x + velocidadX, posicion.y, posicion.z);
        }
    }

    // Método para cuando el jugador recibe daño
    public void RecibeDanio(Vector2 direccion, int cantDanio)
    {
        if (!recibiendoDanio)
        {
            recibiendoDanio = true; // Se activa el estado de daño
            vida -= cantDanio; // Resta la vida

            if (vida <= 0)
            {
                muerto = true; // Marca al jugador como muerto si la vida llega a 0
            }

            // Si no muere, aplica rebote
            if (!muerto)
            {
                Vector2 rebote = new Vector2(transform.position.x - direccion.x, 0.5f).normalized;
                rb.AddForce(rebote * fuerzaRebote, ForceMode2D.Impulse);
            }
        }
    }

    // Método que se puede llamar desde una animación para desactivar el estado de daño
    void DesactivaDanio()
    {
        recibiendoDanio = false;
        rb.velocity = Vector2.zero; // Detiene el movimiento
    }

    // Activa el estado de ataque
    public void Atacando()
    {
        atacando = true;
    }

    // Desactiva el estado de ataque
    public void DescativarAtaque()
    {
        atacando = false;
    }

    // Dibuja una línea en la escena para visualizar el raycast hacia el suelo
    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * longitudRaycast);
    }
}
