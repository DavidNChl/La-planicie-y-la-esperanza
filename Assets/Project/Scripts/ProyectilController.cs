using UnityEngine;

public class Proyectil : MonoBehaviour
{
    public float velocidad = 12f;
    public float dano = 1f;
    public float tiempoVida = 3f;

    [Header("Efecto de Impacto")]
    public GameObject efectoImpactoPrefab; 

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.right * velocidad;

        Destroy(gameObject, tiempoVida);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player") || collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            return;
        }

        SaludEnemigo enemigo = collision.GetComponent<SaludEnemigo>();
        if (enemigo != null)
        {
            enemigo.RecibirDano(dano);
        }

        GenerarEfecto(collision);

        Destroy(gameObject);
    }

    private void GenerarEfecto(Collider2D collision)
    {
        if (efectoImpactoPrefab != null)
        {

            Vector2 puntoContacto = collision.ClosestPoint(transform.position);

            Vector3 posicionConZ = new Vector3(puntoContacto.x, puntoContacto.y, -1f);

            Instantiate(efectoImpactoPrefab, posicionConZ, Quaternion.identity);
        }
    }
}