using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyVenomAI : MonoBehaviour
{
    
    [Header("Ajustes Base")]
    public float speed = 3.5f;
    public float sizeRate = 1.0f;
    public Transform playerTarget;
    public SpriteRenderer spriteRenderer;
    public GameObject poisonProjectilePrefab;
    public Transform puntoDisparo;

    [Header("Reacción a Impacto")]
    public float fuerzaRetroceso = 4f;
    public float tiempoRetroceso = 0.4f;

    private Rigidbody2D rb;
    private Animator animator;
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
                    rb.linearVelocity = new Vector2(-direction * (speed * 0.7f) * sizeRate, rb.linearVelocity.y);
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

        if (distance >= 2.0f && distance <= 8.0f)
        {
            if (dice <= 70)
            {
                StartCoroutine(ExecuteAttack());
                yield break;
            }
        }

        if (dice <= 40) currentState = State.Walk;
        else if (dice <= 70) currentState = State.WalkBack;
        else currentState = State.Stand;

        yield return new WaitForSeconds(Random.Range(0.6f, 1.5f));
        isDecidingAction = false;
    }

    private IEnumerator ExecuteAttack()
    {
        currentState = State.Attack;
        if (animator != null) animator.SetTrigger("Ataque");
        yield return new WaitForSeconds(0.3f);

        if (poisonProjectilePrefab != null)
        {
            Vector3 pos = puntoDisparo != null ? puntoDisparo.position : transform.position;
            GameObject bullet = Instantiate(poisonProjectilePrefab, pos, Quaternion.identity);
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            if (bulletRb != null)
            {
                Vector2 targetDir = (playerTarget.position - transform.position).normalized;
                bulletRb.linearVelocity = targetDir * 9f;
            }
        }

        yield return new WaitForSeconds(0.5f);
        currentState = State.Stand;
        isDecidingAction = false;
    }
}