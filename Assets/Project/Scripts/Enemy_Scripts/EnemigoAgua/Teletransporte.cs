using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TeleportEnemigo : MonoBehaviour
{
    [Header("Configuración de Teletransporte")]
    public float distanciaMaximaTeleport = 8f;
    public float distanciaBusquedaSuelo = 15f;
    public LayerMask capaSuelo;
    public Vector2 offsetJugador = new Vector2(2.5f, 0f);

    [Header("Tiempos de Animación")]
    public float tiempoDesaparicion = 0.4f;
    public float tiempoReaparicion = 0.4f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Transform playerTarget;

    public bool EstaTeletransportando { get; private set; } = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        BuscarJugador();
    }

    private void BuscarJugador()
    {
        if (playerTarget == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTarget = playerObj.transform;
        }
    }

    private void Update()
    {
        if (playerTarget == null || EstaTeletransportando) return;

        float distancia = Vector2.Distance(transform.position, playerTarget.position);
        if (distancia >= distanciaMaximaTeleport)
        {
            EjecutarTeletransporte();
        }
    }

    public void AsignarObjetivo(Transform objetivo)
    {
        playerTarget = objetivo;
    }

    public void EjecutarTeletransporte()
    {
        if (EstaTeletransportando) return;
        StartCoroutine(RutinaTeletransporteSuelo());
    }

    private IEnumerator RutinaTeletransporteSuelo()
    {
        EstaTeletransportando = true;

        // 1. Detener físicas durante el TP
        if (rb != null) rb.linearVelocity = Vector2.zero;

        // 2. Disparar animación de inicio
        if (animator != null)
        {
            animator.SetTrigger("teleport");
        }

        yield return new WaitForSeconds(tiempoDesaparicion);

        // 3. Reubicar pegado al suelo
        Vector3 puntoSuelo = EncontrarPuntoSueloCercano();
        transform.position = puntoSuelo;

        if (rb != null) rb.linearVelocity = Vector2.zero;

        // 4. Girar hacia el jugador
        if (playerTarget != null && spriteRenderer != null)
        {
            float dirX = playerTarget.position.x > transform.position.x ? 1.0f : -1.0f;
            spriteRenderer.flipX = (dirX == -1.0f);
        }

        yield return new WaitForSeconds(tiempoReaparicion);

        EstaTeletransportando = false;
    }

    private Vector3 EncontrarPuntoSueloCercano()
    {
        if (playerTarget == null) return transform.position;

        // Determinar lado aleatorio respecto al jugador (izquierda o derecha)
        float lado = Random.value > 0.5f ? 1f : -1f;
        Vector2 puntoOrigenRay = new Vector2(playerTarget.position.x + (lado * offsetJugador.x), playerTarget.position.y + 3f);

        // Raycast hacia abajo buscando la capa asignada como Suelo
        RaycastHit2D hit = Physics2D.Raycast(puntoOrigenRay, Vector2.down, distanciaBusquedaSuelo, capaSuelo);

        if (hit.collider != null)
        {
            return new Vector3(hit.point.x, hit.point.y + 0.1f, transform.position.z);
        }

        // Raycast de respaldo desde la posición directa del jugador
        RaycastHit2D hitCentro = Physics2D.Raycast(playerTarget.position, Vector2.down, distanciaBusquedaSuelo, capaSuelo);
        if (hitCentro.collider != null)
        {
            return new Vector3(hitCentro.point.x, hitCentro.point.y + 0.1f, transform.position.z);
        }

        return playerTarget.position;
    }
}