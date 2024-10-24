using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuracionController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<MovimientoJugador>().Curar(30);
            Destroy(gameObject);
        }
    }
}
