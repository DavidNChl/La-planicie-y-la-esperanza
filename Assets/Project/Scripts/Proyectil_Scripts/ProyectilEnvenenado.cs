using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class ProyectilVeneno : MonoBehaviour
{
    [Header("Parámetros del Proyectil")]
    public float dano = 1f;
    public float tiempoVida = 4f;
    public GameObject efectoImpactoPrefab;

    private void OnEnable()
    {
        // Cancelar invocaciones previas y programar autodestrucción
        CancelInvoke();
        Invoke(nameof(Desactivar), tiempoVida);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ignore la capa o colisión con los propios enemigos
        if (collision.CompareTag("Enemigo") || collision.gameObject.layer == LayerMask.NameToLayer("Enemigo"))
        {
            return;
        }

        // Si colisiona con el Jugador
        if (collision.CompareTag("Player"))
        {
            SaludJugador salud = collision.GetComponent<SaludJugador>();
            if (salud != null)
            {
                salud.RecibirDano(dano);
            }

            GenerarEfectoImpacto(collision);
            Desactivar();
            return;
        }

        // Si choca contra el suelo/escenario/paredes
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") || collision.gameObject.layer == LayerMask.NameToLayer("Pared"))
        {
            GenerarEfectoImpacto(collision);
            Desactivar();
        }
    }

    private void GenerarEfectoImpacto(Collider2D collision)
    {
        if (efectoImpactoPrefab != null)
        {
            Vector2 puntoContacto = collision.ClosestPoint(transform.position);
            Instantiate(efectoImpactoPrefab, puntoContacto, Quaternion.identity);
        }
    }

    private void Desactivar()
    {
        CancelInvoke();

        // Si usas reciclar/pooling o Instantiate tradicional
        gameObject.SetActive(false);
        Destroy(gameObject, 0.1f);
    }
}