using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlanController : MonoBehaviour
{
    public Transform player;
    
    public float detectionRadius = 20f;
    public float speed = 3.0f;

    [Header("Vida")]
    [SerializeField] public static float vida;
    [SerializeField] private float maximoVida;
    [SerializeField] private BarraDeVidaAlan barraDeVida;

    private Rigidbody2D rb;
    private Vector2 movement;

    private Animator animator;
    private Vector2 posicionInicial;
    private Launcher launcher;

    [Header("Charge")]
    [SerializeField] private float velocidadCharge;
    [SerializeField] private float distanciaMinimaCharge; // La distancia mínima para detener la carga
    [SerializeField] private float distanciaMaximaCharge; // La distancia máxima para detener la carga

    private bool preparando = false;

    [Header("Ataque Carga")]
    private bool atacando = false;
    [SerializeField] private Transform controladorAtaque;
    [SerializeField] private float radioAtaque;
    [SerializeField] private float dañoAtaque;

    void Start()
    {
        vida = maximoVida;
        barraDeVida.InicializarBarraDeVida(vida);
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        posicionInicial = new Vector2(15.85f, -1.82f);
        
        launcher = GetComponent<Launcher>();
        
        // Inicia la coroutine que elige entre las dos acciones
        StartCoroutine(DecidirAccion());
    }
    public void TomarDaño(float daño)
    {
        vida -= daño;
        barraDeVida.CambiarVidaActual(vida);
        // murio xd
        if (vida <= 0)
        {
            Destroy(gameObject);
        }
    }
    public void Curar(float curacion)
    {   // no se cura mas pq ya esta al tope de vida
        if ((vida + curacion) > maximoVida)
        {
            vida = maximoVida;
        }
        else
        {
            vida += curacion;
            barraDeVida.CambiarVidaActual(vida);
        }
    }
    private IEnumerator DecidirAccion()
    {
        while (true) // Repite indefinidamente
        {
            
            yield return new WaitForSeconds(Random.Range(0.5f, 2f)); // Espera un tiempo aleatorio entre 2 y 5 segundos

            // Elige una acción de forma aleatoria
            if (Random.value > 0.5f) // 50% de probabilidad
            {
                StartCoroutine(ChargeCoroutine());
            }
            else
            {
                TirarBomba();
                yield return new WaitForSeconds(0.5f);
                TirarBomba();
                yield return new WaitForSeconds(0.5f);
                TirarBomba();
                yield return new WaitForSeconds(0.5f);
                TirarBomba();
                yield return new WaitForSeconds(0.5f);
                TirarBomba();
                yield return new WaitForSeconds(0.5f);
                TirarBomba();
                
            }
        }
    }
    void Update()
    {
        /*
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer < detectionRadius)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            movement = new Vector2(direction.x, 0);
        }
        else {
            movement = Vector2.zero;
        }

        rb.MovePosition(rb.position + movement * speed * Time.deltaTime); */
    }

    private IEnumerator ChargeCoroutine()
    {
        //aca se coloca durante un tiempo una animacion de aviso
        preparando = true;
        animator.SetBool("preparandoCarga", preparando);

        yield return new WaitForSeconds(1f);

        preparando = false;

        animator.SetBool("preparandoCarga", preparando);

        // Calcula la dirección hacia el jugador
        Vector2 direccion = (player.position - transform.position).normalized;

        // Establece la velocidad de la carga hacia el jugador
        rb.velocity = new Vector2(direccion.x * velocidadCharge, 0);

        float distanciaRecorrida = 0f; // Distancia recorrida durante la carga

        // Espera hasta que el personaje esté lo suficientemente cerca del jugador
        while (Vector2.Distance(transform.position, player.position) > distanciaMinimaCharge && distanciaRecorrida < distanciaMaximaCharge)
        {
            distanciaRecorrida += Mathf.Abs(direccion.x * velocidadCharge * Time.deltaTime); // Calcula la distancia recorrida
            yield return null; // Espera un frame y vuelve a comprobar la distancia
        }

        // Detiene el movimiento al llegar a la distancia mínima o alcanzar la distancia máxima
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(0.1f);

        // Ejecuta un ataque
        StartCoroutine(Atacar());

        // Luego vuelve a la posición inicial
        yield return new WaitForSeconds(0.8f);
        rb.position = posicionInicial;
    }

    private IEnumerator Atacar()
    {
        atacando = true;
        animator.SetBool("estaAtacando", atacando);
        Collider2D[] objetos = Physics2D.OverlapCircleAll(controladorAtaque.position, radioAtaque);
        foreach (Collider2D collision in objetos)
        {
            if (collision.CompareTag("Player"))
            {
                collision.GetComponent<MovimientoJugador>().TomarDaño(dañoAtaque);

            }
        }
        yield return new WaitForSeconds(0.15f);

        atacando = false;
        animator.SetBool("estaAtacando", atacando);
    }
    private void OnDrawGizmos()
    {
        if (controladorAtaque != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(controladorAtaque.position, radioAtaque);
        }
    }

    private void TirarBomba() {
        launcher = GetComponent<Launcher>();

        if (launcher != null)
        {
            launcher.FireProjectile();
        }
    }
}
