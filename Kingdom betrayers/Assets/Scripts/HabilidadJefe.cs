using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HabilidadJefe : MonoBehaviour
{
    [SerializeField] private float daño;
    [SerializeField] private Vector2 dimensionesCaja;
    [SerializeField] private Transform posicionCaja;
    [SerializeField] private float tiempoDeVide;
    void Start()
    {
        Destroy(gameObject, tiempoDeVide);
    }


    void Update()
    {
        
    }

    public void Golpe()
    {
        Collider2D[] objetos = Physics2D.OverlapBoxAll(posicionCaja.position,dimensionesCaja, 0f);
        /* foreach (Collider2D collisiones in objetos)
        {
            if (collisiones.compareTag("Player"))
            {
                colisioiones.Getcomponent<CombateJugador>().TomarDaño(daño);


            }
                
            

        }   */
    }

    private void OnDrawGizmos()
    {
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(posicionCaja.position,dimensionesCaja);
    }
            
}




