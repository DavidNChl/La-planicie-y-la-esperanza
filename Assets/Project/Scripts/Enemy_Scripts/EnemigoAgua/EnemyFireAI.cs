using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyFireAI : MonoBehaviour
{
    [Header("Ajustes Base")]
    public float speed = 4.0f;
    public float sizeRate = 1.0f;
    public Transform playerTarget;
    public SpriteRenderer spriteRenderer;

    [Header("Reacción a Impacto")]
    public float fuerzaRetroceso = 4f;
    public float tiempoRetroceso = 0.4f;

    private Rigidbody2D rb;
    private Animator animator;
    private EnemigoAtaque ataque;
    private float direction = 1.0f;
    private bool isDecidingAction = false;
    private bool isChargingForward = false;
    private bool recibiendoDano = false;

    private enum State { Stand, Walk, WalkBack, Attack }
    private State currentState = State.Stand;
    private TeleportEnemigo teleportScript;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        ataque = GetComponent<EnemigoAtaque>();
        teleportScript = GetComponent<TeleportEnemigo>();
    }

    private void Start()
    {
        BuscarJugador();
    }

    private void OnEnable()
    {
        isDecidingAction = false;
        isChargingForward = false;
        recibiendoDano = false;
        currentState = State.Stand;
        BuscarJugador();
    }

    private void BuscarJugador()
    {
        if (playerTarget == null)
            playerTarget = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        // Si el script de teletransporte está activo o ejecutándose, pausar la IA de combate
        if (teleportScript != null && teleportScript.EstaTeletransportando) return;

        if (!isDecidingAction && !recibiendoDano && currentState != State.Attack)
        {
            StartCoroutine(DecideActionLoop()); // O DecideNextActionLoop()
        }
    }

    private void FixedUpdate()
    {
        if (playerTarget != null && !recibiendoDano && currentState != State.Attack)
        {
            direction = playerTarget.position.x > transform.position.x ? 1.0f : -1.0f;
            if (spriteRenderer != null) spriteRenderer.flipX = (direction == -1.0f);
        }

        if (animator != null)
        {
            animator.SetFloat("Velocidad", Mathf.Abs(rb.linearVelocity.x));
        }

        if (isChargingForward && !recibiendoDano)
        {
            rb.linearVelocity = new Vector2(direction * 10f * sizeRate, rb.linearVelocity.y);
        }
        else
        {
            switch (currentState)
            {
                case State.Stand:
                case State.Attack:
                    rb.linearVelocity = new Vector2(Mathf.MoveTowards(rb.linearVelocity.x, 0, 12f * Time.fixedDeltaTime), rb.linearVelocity.y);
                    break;
                case State.Walk:
                    rb.linearVelocity = new Vector2(direction * speed * sizeRate, rb.linearVelocity.y);
                    break;
                case State.WalkBack:
                    if (!recibiendoDano)
                    {
                        rb.linearVelocity = new Vector2(-direction * speed * 0.6f * sizeRate, rb.linearVelocity.y);
                    }
                    break;
            }
        }
    }

    public void RecibirImpacto()
    {
        if (recibiendoDano) return;

        StopAllCoroutines();
        StartCoroutine(RutinaRetroceso());
    }

    private IEnumerator RutinaRetroceso()
    {
        recibiendoDano = true;
        isChargingForward = false;
        currentState = State.WalkBack;

        if (animator != null) animator.SetTrigger("WalkBack");
        rb.linearVelocity = new Vector2(-direction * fuerzaRetroceso, rb.linearVelocity.y);

        yield return new WaitForSeconds(tiempoRetroceso);

        recibiendoDano = false;
        isDecidingAction = false;
        currentState = State.Stand;
    }

    private IEnumerator DecideActionLoop()
    {
        isDecidingAction = true;

        if (playerTarget == null)
        {
            currentState = State.Stand;
            yield return new WaitForSeconds(1f);
            isDecidingAction = false;
            yield break;
        }

        float distance = Vector2.Distance(transform.position, playerTarget.position);
        int dice = Random.Range(0, 100);

        if (distance >= 1.0f && distance <= 5.0f)
        {
            if (dice <= 70 && (ataque == null || ataque.PuedeAtacar()))
            {
                StartCoroutine(ExecuteAttackA());
                yield break;
            }
        }

        if (dice <= 50) currentState = State.Walk;
        else currentState = State.WalkBack;

        yield return new WaitForSeconds(Random.Range(0.5f, 1.2f));
        isDecidingAction = false;
    }

    private IEnumerator ExecuteAttackA()
    {
        currentState = State.Attack;
        if (animator != null) animator.SetTrigger("Ataque");

        yield return new WaitForSeconds(0.2f);
        isChargingForward = true;

        float tiempo = 0f;
        bool yaHizoDano = false;

        while (tiempo < 0.4f)
        {
            tiempo += Time.deltaTime;

            if (!yaHizoDano && playerTarget != null)
            {
                float distancia = Vector2.Distance(transform.position, playerTarget.position);
                if (distancia <= 1.5f) // Si embiste y toca al jugador
                {
                    SaludJugador salud = playerTarget.GetComponent<SaludJugador>();
                    if (salud != null)
                    {
                        salud.RecibirDano(1f);
                        yaHizoDano = true; // Evita quitarle toda la vida en un solo frame
                    }
                }
            }
            yield return null;
        }

        isChargingForward = false;
        yield return new WaitForSeconds(0.4f);

        currentState = State.Stand;
        isDecidingAction = false;
    }
}