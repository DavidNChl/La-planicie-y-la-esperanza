using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    
    [Header("Parámetros de Movimiento")]
    public float speed = 3.0f;
    public float sizeRate = 1.0f;
    public float attackRangeMin = 1.0f;
    public float attackRangeMax = 2.0f;

    [Header("Reacción a Impacto")]
    public float fuerzaRetroceso = 4f;
    public float tiempoRetroceso = 0.4f;

    [Header("Referencias")]
    public Transform playerTarget;
    public SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    private Animator animator;
    private EnemigoAtaque ataque;
    private float direction = 1.0f;
    private bool isDecidingAction = false;
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
        recibiendoDano = false;
        currentState = State.Stand;
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
        // Si el script de teletransporte está activo o ejecutándose, pausar la IA de combate
        if (teleportScript != null && teleportScript.EstaTeletransportando) return;

        if (!isDecidingAction && !recibiendoDano && currentState != State.Attack)
        {
            StartCoroutine(DecideNextActionLoop()); // O DecideNextActionLoop()
        }
    }   

    private void FixedUpdate()
    {
        if (!recibiendoDano) TurnTowardsPlayer();

        if (animator != null)
        {
            animator.SetFloat("Velocidad", Mathf.Abs(rb.linearVelocity.x));
        }

        switch (currentState)
        {
            case State.Stand:
            case State.Attack:
                rb.linearVelocity = new Vector2(Mathf.MoveTowards(rb.linearVelocity.x, 0, 10f * Time.fixedDeltaTime), rb.linearVelocity.y);
                break;
            case State.Walk:
                rb.linearVelocity = new Vector2(direction * speed * sizeRate, rb.linearVelocity.y);
                break;
            case State.WalkBack:
                if (!recibiendoDano)
                {
                    rb.linearVelocity = new Vector2(-direction * (speed * 0.5f) * sizeRate, rb.linearVelocity.y);
                }
                break;
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
        currentState = State.WalkBack;
        if (animator != null) animator.SetTrigger("WalkBack");

        rb.linearVelocity = new Vector2(-direction * fuerzaRetroceso, rb.linearVelocity.y);
        yield return new WaitForSeconds(tiempoRetroceso);

        recibiendoDano = false;
        isDecidingAction = false;
        currentState = State.Stand;
    }

    private IEnumerator DecideNextActionLoop()
    {
        isDecidingAction = true;

        if (playerTarget == null)
        {
            currentState = State.Stand;
            yield return new WaitForSeconds(1.0f);
            isDecidingAction = false;
            yield break;
        }

        float distance = Vector2.Distance(transform.position, playerTarget.position);
        int dice = Random.Range(0, 100);

        if (distance >= attackRangeMin && distance <= attackRangeMax)
        {
            if (dice <= 80 && (ataque == null || ataque.PuedeAtacar()))
            {
                StartCoroutine(ExecuteAttackA());
                yield break;
            }
        }

        if (dice <= 40) currentState = State.Walk;
        else if (dice <= 70) currentState = State.WalkBack;
        else currentState = State.Stand;

        yield return new WaitForSeconds(Random.Range(0.5f, 1.2f));
        isDecidingAction = false;
    }

    private void TurnTowardsPlayer()
    {
        if (playerTarget == null || currentState == State.Attack) return;

        float desiredDir = playerTarget.position.x > transform.position.x ? 1.0f : -1.0f;
        if (direction != desiredDir)
        {
            direction = desiredDir;
            if (spriteRenderer != null) spriteRenderer.flipX = (direction == -1.0f);
        }
    }

   private IEnumerator ExecuteAttackA()
    {
        currentState = State.Attack;
        
        if (animator != null) animator.SetTrigger("Ataque");

        // Esperar el momento del impacto en el sprite (ejemplo: 0.3 segundos)
        yield return new WaitForSeconds(0.3f);

        if (playerTarget != null)
        {
            float distancia = Vector2.Distance(transform.position, playerTarget.position);
            if (distancia <= attackRangeMax)
            {
                SaludJugador salud = playerTarget.GetComponent<SaludJugador>();
                if (salud != null)
                {
                    salud.RecibirDano(1f); // Resta 1 de vida al jugador
                }
            }
        }

        yield return new WaitForSeconds(0.5f);
        currentState = State.Stand;
        isDecidingAction = false;
    }
}