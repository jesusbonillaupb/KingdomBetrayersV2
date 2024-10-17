using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    [SerializeField] Transform projectile;
    [SerializeField] Transform spawnPoint;
    [SerializeField] LineRenderer lineRenderer;

    [SerializeField] float trajectoryTimeStep = 0.05f;
    [SerializeField] int trajectoryStepCount = 15;

    [SerializeField] Transform player; // Referencia al jugador para apuntar hacia su posición


    /*private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            DrawTrajectory();
        }
        if (Input.GetMouseButtonUp(0))
        {
            FireProjectile();
            ClearTrajectory();
        }
    }*/

    void DrawTrajectory()
    {
        Vector3[] positions = new Vector3[trajectoryStepCount];
        Vector2 initialVelocity = CalculateLaunchVelocity();

        for (int i = 0; i < trajectoryStepCount; i++)
        {
            float t = i * trajectoryTimeStep;
            Vector3 pos = (Vector2)spawnPoint.position + initialVelocity * t + 0.5f * Physics2D.gravity * t * t;
            positions[i] = pos;
        }
        lineRenderer.positionCount = trajectoryStepCount;
        lineRenderer.SetPositions(positions);
    }

    public void FireProjectile()
    {
        Transform pr = Instantiate(projectile, spawnPoint.position, Quaternion.identity);
        Rigidbody2D rb = pr.GetComponent<Rigidbody2D>();
        rb.velocity = CalculateLaunchVelocity(); // Aplicar la velocidad inicial parabólica
    }

    Vector2 CalculateLaunchVelocity()
    {
        // Dirección hacia el jugador
        Vector2 direction = player.position - spawnPoint.position;
        float distanceX = direction.x; // Distancia horizontal entre el punto de disparo y el jugador
        float distanceY = direction.y; // Diferencia de altura entre el punto de disparo y el jugador

        // Calcular la gravedad (en magnitud positiva)
        float gravity = Mathf.Abs(Physics2D.gravity.y);

        // Usar la fórmula para calcular la velocidad inicial en función de la distancia y la gravedad
        float launchVelocitySquared = (gravity * distanceX * distanceX) /
            (2 * (distanceY - Mathf.Tan(Mathf.Deg2Rad * 45f) * distanceX) * Mathf.Cos(Mathf.Deg2Rad * 45f) * Mathf.Cos(Mathf.Deg2Rad * 45f));

        // Si la velocidad calculada es negativa (por una diferencia de altura imposible), usar un valor positivo
        if (launchVelocitySquared < 0) launchVelocitySquared = Mathf.Abs(launchVelocitySquared);

        // Calcular la velocidad inicial a partir de la velocidad al cuadrado
        float launchVelocity = Mathf.Sqrt(launchVelocitySquared);

        // Separar la velocidad en componentes X y Y usando el ángulo de 45 grados
        float launchVelocityX = launchVelocity * Mathf.Cos(Mathf.Deg2Rad * 45f);
        float launchVelocityY = launchVelocity * Mathf.Sin(Mathf.Deg2Rad * 45f);

        // Ajustar el signo de la componente X según la posición del jugador (derecha o izquierda)
        launchVelocityX *= Mathf.Sign(distanceX);

        // Retornar la velocidad inicial
        return new Vector2(launchVelocityX, launchVelocityY);
    }

    void ClearTrajectory()
    {
        lineRenderer.positionCount = 0;
    }
}
