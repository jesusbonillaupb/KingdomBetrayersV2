using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1 : MonoBehaviour
{
    private Animator animator;

    public Rigidbody2D rb2D;

    public Transform jugador;
    [SerializeField] private object player;

    // Inicialmente mirando hacia la izquierda
    private bool mirandoIzquierda = true;

    [Header("Vida")]
    [SerializeField] private float vida;

    [Header("Ataque")]
    [SerializeField] private Transform controladorAtaque;
    [SerializeField] private float radioAtaque;
    [SerializeField] private float dañoAtaque;

    //[SerializeField] private Barradevida barraDeVida;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        //barraDeVida.inicializarBarraDeVida(vida);
        jugador = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }
    void Update()
    {
        float distanciaJugador = Vector2.Distance(transform.position, jugador.position);
        animator.SetFloat("distanciaJugador", distanciaJugador);

    }

    public void TomarDaño(float daño)
    {
        vida -= daño;
        if (vida <= 0)
        {
            animator.SetTrigger("Muerte");
        }
    }

    private void Muerte()
    {
        Destroy(gameObject);
    }

    public void MirarJugador()
    {
        if ((jugador.position.x < transform.position.x && !mirandoIzquierda) ||
            (jugador.position.x > transform.position.x && mirandoIzquierda))
        {
            // Cambia la dirección en la que está mirando
            mirandoIzquierda = !mirandoIzquierda;
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + 180, 0);
        }
    }

    public void Ataque()
    {
        Collider2D[] objetos = Physics2D.OverlapCircleAll(controladorAtaque.position, radioAtaque );
        foreach (Collider2D collision in objetos)
        {
            if (collision.CompareTag("Player"))
            {
                // collision.GetComponent<CombateJugador>().TomarDaño(dañoAtaque);

            }
        }
    }

    private void OnDrawGizmos()
    {
        if (controladorAtaque != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(controladorAtaque.position, radioAtaque);
        }
    }

    
}
