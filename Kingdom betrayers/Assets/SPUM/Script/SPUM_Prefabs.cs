using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPUM_Prefabs : MonoBehaviour
{
    public float _version;
    public SPUM_SpriteList _spriteOBj;
    public bool EditChk;
    public string _code;
    public Animator _anim;

    public bool _horse;
    public string _horseString;

    // Variables nuevas
    public Transform player; // Referencia al jugador
    public float detectionRange = 10f; // Rango en el que el enemigo puede detectar al jugador
    public float attackRange = 2f; // Rango para atacar
    public float speed = 3f; // Velocidad de movimiento del enemigo
    public float attackCooldown = 2f; // Tiempo de espera entre ataques
    public float damage = 10f; // Daño que hace el enemigo al jugador

    private Vector3 startPosition; // Posición inicial del enemigo
    private bool isChasingPlayer = false;
    private bool isReturningToStart = false;
    private Vector3 lastPosition; // Para almacenar la última posición
    private MovimientoJugador playerHealth; // Referencia al script del jugador
    private float lastAttackTime; // Para controlar el tiempo entre ataques
    public float vida;

    void Start()
    {
        // Guardar la posición inicial del enemigo y la posición anterior
        startPosition = transform.position;
        lastPosition = transform.position; // Iniciar con la posición actual
        lastAttackTime = -attackCooldown; // Permitir que ataque inmediatamente al iniciar
        playerHealth = player.GetComponent<MovimientoJugador>(); // Obtener el componente del jugador
    }

    void Update()
    {
        // Calcular la distancia entre el enemigo y el jugador
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange && vida > 0)
        {
            // Si el jugador está en rango de ataque, atacar
            AttackPlayer();
        }
        else if (distanceToPlayer <= detectionRange)
        {
            // Si el jugador está dentro del rango de detección pero fuera del rango de ataque, perseguir
            ChasePlayer(distanceToPlayer);
        }
        else
        {
            // Si el jugador está fuera de ambos rangos, detener cualquier acción
            StopChasing();
        }

        // Comportamiento al volver a la posición inicial
        if (isReturningToStart && Vector3.Distance(transform.position, startPosition) < 0.1f)
        {
            StopReturning();
        }

        // Actualizar la última posición
        lastPosition = transform.position;
    }

    void StopChasing()
    {
        isChasingPlayer = false;
        PlayAnimation(0); // Cambiar a Idle cuando no está persiguiendo
    }



    void ChasePlayer(float distanceToPlayer)
    {
        if (vida > 0) { 
        isChasingPlayer = true;
        isReturningToStart = false;

        // Reproducir animación de correr
        PlayAnimation(1);

        // Calcular la dirección hacia el jugador
        Vector3 direction = player.position - transform.position;

        // Invertir el sprite para que siempre mire hacia el jugador
        if (direction.x < 0)
        {
            // Si el jugador está a la izquierda, voltear el enemigo hacia la izquierda
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (direction.x > 0)
        {
            // Si el jugador está a la derecha, voltear el enemigo hacia la derecha
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        // Mover al enemigo hacia el jugador solo si está dentro del rango de ataque
        if (distanceToPlayer > attackRange)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }
    }
    }

    public void TomarDaño(float daño)
    {
        vida -= daño;

        // Si la vida cae a 0 o menos, inicia la muerte
        if (vida <= 0)
        {
            StartCoroutine(Muerte());
        }
    }

    private IEnumerator Muerte()
    {
        // Reproducir animación de muerte
        PlayAnimation(2); // Asegúrate de que tu Animator tenga un trigger llamado "Die"

        // Desactivar el collider
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false; // Desactiva el collider
        }

        // Desactivar el rigidbody
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = false; // Desactiva la simulación del rigidbody
        }

        // Esperar a que la animación de muerte termine (ajusta el tiempo según tu animación)
        yield return new WaitForSeconds(10f); // Cambia 10f por la duración de tu animación

        // Destruir el objeto después de la espera
        Destroy(gameObject);
    }



    void AttackPlayer()
    {
        // Si ha pasado suficiente tiempo desde el último ataque
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            // Reproducir animación de ataque
            PlayAnimation(4); // Animación de ataque

            // Hacer daño al jugador
            playerHealth.TomarDaño(damage);

            // Actualizar el último tiempo de ataque
            lastAttackTime = Time.time;
        }
    }

    void ReturnToStart()
    {
        isChasingPlayer = false;
        isReturningToStart = true;

        // Reproducir animación de correr
        PlayAnimation(1);

        // Moverse de regreso a la posición inicial
        transform.position = Vector3.MoveTowards(transform.position, startPosition, speed * Time.deltaTime);
    }

    void StopReturning()
    {
        isReturningToStart = false;

        // Reproducir animación de idle
        PlayAnimation(0);
    }

    public void PlayAnimation(int num)
    {
        switch (num)
        {
            case 0: // Idle
                _anim.SetFloat("RunState", 0f);
                break;

            case 1: // Run
                _anim.SetFloat("RunState", 0.5f);
                break;

            case 2: // Death
                _anim.SetTrigger("Die");
                _anim.SetBool("EditChk", EditChk);
                break;

            case 3: // Stun
                _anim.SetFloat("RunState", 1.0f);
                break;

            case 4: // Attack Sword
                _anim.SetTrigger("Attack");
                _anim.SetFloat("AttackState", 0.0f);
                _anim.SetFloat("NormalState", 0.0f);
                break;
        }
    }
}
