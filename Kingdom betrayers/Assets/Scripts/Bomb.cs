using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<MovimientoJugador>().TomarDaño(30);
            Destroy(gameObject);
        }
        if (other.CompareTag("terrain"))
        {
            
            Destroy(gameObject);
        }

    }
}
