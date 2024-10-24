using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class MovimientoJugador : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] public static float vida;
    [SerializeField] private float maximoVida;
    [SerializeField] private BarraDeVida barraDeVida;
    private Rigidbody2D rb2D;


    [Header("Movimiento")]
    private float inputX;
    private float movimientoHorizontal = 0f;
    [SerializeField] private float velocidadDeMovimiento;
    private float suavizadoDeMovimiento = 0f;
    private Vector3 velocidad = Vector3.zero;
    private bool mirandoDerecha = true;
    private bool dasheando = false;
    

    [Header("Salto")]
    [SerializeField] private float fuerzaDeSalto;
    [SerializeField] private LayerMask queEsSuelo;
    [SerializeField] private Transform controladorSuelo;
    [SerializeField] private Vector3 dimensionesCajaSuelo;
    [SerializeField] private bool enSuelo;
    [SerializeField] private float coyoteTime;
    private float tiempoEnElAire;
    private bool salto = false;

    [Header("DeslizarPared")]
    [SerializeField] private Transform controladorPared;
    [SerializeField] private Vector3 dimensionesCajaPared;
    [SerializeField] private bool enPared;
    [SerializeField] private bool deslizando;
    [SerializeField] private float velocidadDeslizar;
    [SerializeField] private float fuerzaDeDespeguePared;

    [Header("Dash")]
    [SerializeField] private float velocidadDash;
    [SerializeField] private float tiempoDash;
    [SerializeField] private float cooldownDash;
    private float tiempoDeDash;
    private float gravedadInicial;
    private bool puedeHacerDash = true;
    private bool sePuedeMover = true;
    [Header("Ataque")]
    private bool atacando = false;
    [SerializeField] private float cooldownAtaque = 0.5f; // Tiempo de espera entre ataques
    private bool puedeAtacar = true;  // Controla si el jugador puede atacar o no

    [SerializeField] private Transform controladorAtaque;
    [SerializeField] private float radioAtaque;
    [SerializeField] private float dañoAtaque;
    

    [Header("Animator")]
    private Animator animator;

    [Header("Particulas")]
    [SerializeField] private ParticleSystem particulas;

    private void Start()
    {
        vida = maximoVida;
        barraDeVida.InicializarBarraDeVida(vida);
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        gravedadInicial = rb2D.gravityScale;
    }
    // se le ingresa una cantidad de daño a tomar
    public void TomarDaño(float daño)
    {
        vida -= daño;
        barraDeVida.CambiarVidaActual(vida);
        // murio xd
        if (vida <= 0) {
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
    private void Update()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        movimientoHorizontal = inputX * velocidadDeMovimiento;

        animator.SetFloat("Horizontal", Mathf.Abs(movimientoHorizontal));

        if (Input.GetButtonDown("Jump"))
        {
            salto = true;
        }
        

        if (!enSuelo && enPared && inputX != 0)
        {
            deslizando = false;
            suavizadoDeMovimiento = 0.3f;
        }
        else
        {
            deslizando= false;
            suavizadoDeMovimiento = 0f;
        }

        if(Input.GetKeyDown(KeyCode.LeftShift) && puedeHacerDash && tiempoDeDash > cooldownDash)
        {
            StartCoroutine(Dash());

        
        }
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {

            StartCoroutine(Atacar());
            
        }




    }

    private void FixedUpdate()
    {
        enSuelo = Physics2D.OverlapBox(controladorSuelo.position, dimensionesCajaSuelo, 0f, queEsSuelo);
        enPared = Physics2D.OverlapBox(controladorPared.position, dimensionesCajaPared, 0f, queEsSuelo);

        animator.SetBool("enSuelo", enSuelo);

        tiempoDeDash += Time.deltaTime;

        if (enSuelo == true || deslizando == true)
        {
            tiempoEnElAire = 0;
        }
        else
        {
            tiempoEnElAire += Time.deltaTime;
        }

        if (sePuedeMover) { 
            Mover(movimientoHorizontal * Time.fixedDeltaTime, salto);
        }
        salto = false;
    }

    private void Mover(float mover, bool saltar)
    {
        Vector3 velocidadObjetivo = new Vector2(mover, rb2D.velocity.y);
    
        rb2D.velocity = Vector3.SmoothDamp(rb2D.velocity, velocidadObjetivo, ref velocidad, suavizadoDeMovimiento);

        if ((mover > 0 && !mirandoDerecha) || (mover < 0 && mirandoDerecha))
        {
            Girar();
        }

        if (enSuelo && saltar)
        {
            Salto();
        }
        else if (deslizando && saltar)
        {
            SaltoDePared();
        }
        else if ((enSuelo == false && saltar == true) || (deslizando == false && saltar == true))
        {
            if (tiempoEnElAire < coyoteTime)
            {
                Salto();
            }
        }

        if (deslizando)
        {
            rb2D.velocity = new Vector2(rb2D.velocity.x, Mathf.Clamp(rb2D.velocity.y, -velocidadDeslizar, float.MaxValue));
        }
    }

    private IEnumerator Dash()
    {
        particulas.Play();
        dasheando = true;
        animator.SetBool("Dash", dasheando);
        sePuedeMover = false;
        puedeHacerDash = false;
        rb2D.gravityScale = 0;
        rb2D.velocity = new Vector2(velocidadDash * transform.localScale.x, 0);
        yield return new WaitForSeconds(tiempoDash);
        sePuedeMover = true;
        puedeHacerDash = true;
        rb2D.gravityScale = gravedadInicial;
        tiempoDeDash = 0;
        dasheando = false;
        animator.SetBool("Dash", dasheando);
    }

    private IEnumerator Atacar()
    {
        if (!puedeAtacar) yield break; // Si no puede atacar, termina el método

        puedeAtacar = false; // Bloquear ataques
        atacando = true;
        animator.SetBool("estaAtacando", atacando);

        Collider2D[] objetos = Physics2D.OverlapCircleAll(controladorAtaque.position, radioAtaque);
        foreach (Collider2D collision in objetos)
        {
            if (collision.CompareTag("Alan"))
            {
                collision.GetComponent<AlanController>().TomarDaño(dañoAtaque);
            }
            if (collision.CompareTag("Enemigo"))
            {
                collision.GetComponent<SPUM_Prefabs>().TomarDaño(dañoAtaque);
            }
        }

        yield return new WaitForSeconds(0.15f); // Duración del ataque

        atacando = false;
        animator.SetBool("estaAtacando", atacando);

        yield return new WaitForSeconds(cooldownAtaque); // Cooldown del ataque

        puedeAtacar = true; // Permitir que el jugador ataque de nuevo
    }


    private void Salto()
    {
        particulas.Play();
        enSuelo = false;
        rb2D.velocity = new Vector2(rb2D.velocity.x, fuerzaDeSalto);
        tiempoEnElAire = 1f;
    }




    private void Girar()
    {
        mirandoDerecha = !mirandoDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

    private void SaltoDePared()
    {
        Salto();
        rb2D.AddForce(new Vector2(-fuerzaDeDespeguePared * inputX, 0f));
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
