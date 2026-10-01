using System.Collections;
using UnityEngine;

public class Proyectil : MonoBehaviour
{
    public float velocidad = 12f;
    public float dano = 1f;
    public float tiempoVida = 3f;

    [Header("Efecto de Impacto")]
    public GameObject efectoImpactoPrefab;

    private Rigidbody2D rb;
    private Coroutine rutinaDesactivacion;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        // Aplicar velocidad cada vez que se saca de la pool
        if (rb != null)
        {
            rb.linearVelocity = transform.right * velocidad;
        }

        // Iniciar temporizador para desactivar si no choca con nada
        rutinaDesactivacion = StartCoroutine(DesactivarPorTiempo());
    }

    private void OnDisable()
    {
        if (rutinaDesactivacion != null)
        {
            StopCoroutine(rutinaDesactivacion);
        }
    }

    private IEnumerator DesactivarPorTiempo()
    {
        yield return new WaitForSeconds(tiempoVida);
        Desactivar();
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
        Desactivar();
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

    private void Desactivar()
    {
        gameObject.SetActive(false);
    }
}