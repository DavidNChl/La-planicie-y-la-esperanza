using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BossAI : MonoBehaviour
{
    [Header("Ajustes Base")]
    public float speed = 3.5f;
    public float fuerzaRetroceso = 5f;
    public float tiempoRetroceso = 0.3f;
    public Transform playerTarget;
    public SpriteRenderer spriteRenderer;

    [Header("Ataque Cuerpo a Cuerpo")]
    public float rangoCuerpoACuerpo = 2f;
    public float danoCuerpoACuerpo = 1f;

    [Header("Ataque Espada Larga")]
    public GameObject objetoEspadaLarga; 
    public float rangoEspada = 5f;
    public float duracionAtaqueEspada = 0.6f;

    [Header("Ataque Volar y Aplastar")]
    public float alturaVuelo = 6f;
    public float velocidadVueloHorizontal = 8f;
    public float fuerzaCaidaAplastado = 25f;
    public float danoAplastamiento = 2f;

    private Rigidbody2D rb;
    private Animator animator;
    private float direction = 1.0f;
    private bool isDecidingAction = false;
    private bool recibiendoDano = false;
    private bool ejecutandoAtaque = false;

    private enum State { Stand, Walk, WalkBack, Attack }
    private State currentState = State.Stand;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        if (objetoEspadaLarga != null) objetoEspadaLarga.SetActive(false);
    }

    private void Start()
    {
       BuscarJugador();

        // Ignora la colisión física entre el Collider de este enemigo y el del Jugador
        if (playerTarget != null)
        {
            Collider2D colJugador = playerTarget.GetComponent<Collider2D>();
            Collider2D colEnemigo = GetComponent<Collider2D>();

            if (colJugador != null && colEnemigo != null)
            {
                Physics2D.IgnoreCollision(colEnemigo, colJugador, true);
            }
        }
    }

    private void BuscarJugador()
    {
        if (playerTarget == null)
            playerTarget = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    public void AsignarObjetivo(Transform objetivo)
    {
        playerTarget = objetivo;
    }

    private void Update()
    {
        if (recibiendoDano || ejecutandoAtaque) return;

        if (!isDecidingAction)
        {
            StartCoroutine(DecideActionLoop());
        }
    }

    private void FixedUpdate()
    {
        if (recibiendoDano || ejecutandoAtaque) return;

        // Orientar sprite hacia el jugador
        if (playerTarget != null)
        {
            direction = playerTarget.position.x > transform.position.x ? 1.0f : -1.0f;
            if (spriteRenderer != null) spriteRenderer.flipX = (direction == -1.0f);
        }

        if (animator != null)
        {
            animator.SetFloat("Velocidad", Mathf.Abs(rb.linearVelocity.x));
        }

        // Control de movimiento base
        switch (currentState)
        {
            case State.Stand:
            case State.Attack:
                rb.linearVelocity = new Vector2(Mathf.MoveTowards(rb.linearVelocity.x, 0, 15f * Time.fixedDeltaTime), rb.linearVelocity.y);
                break;
            case State.Walk:
                rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
                break;
            case State.WalkBack:
                rb.linearVelocity = new Vector2(-direction * speed * 0.8f, rb.linearVelocity.y);
                break;
        }
    }

    // --- MÁQUINA DE DECISIONES DE LA IA ---

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

        float distancia = Vector2.Distance(transform.position, playerTarget.position);
        int dado = Random.Range(0, 100);

        // 1. Si está MUY CERCA del jugador -> Prioridad Ataque Normal Cuerpo a Cuerpo (80% probabilidad)
        if (distancia <= rangoCuerpoACuerpo)
        {
            if (dado < 80)
            {
                StartCoroutine(AtaqueCuerpoACuerpo());
                yield break;
            }
        }
        // 2. Si está a MEDIA DISTANCIA -> Prioridad Ataque de Tijeras Extendidas
        else if (distancia <= rangoEspada)
        {
            if (dado < 70)
            {
                StartCoroutine(AtaqueEspadaLarga());
                yield break;
            }
        }

        // 3. Si está alejado o falla el dado -> Ataque de Vuelo/Aplastamiento o Movimiento
        if (dado < 40)
        {
            StartCoroutine(AtaqueVolarYAplastar());
            yield break;
        }

        if (dado <= 60) currentState = State.Walk;
        else if (dado <= 85) currentState = State.WalkBack;
        else currentState = State.Stand;

        yield return new WaitForSeconds(Random.Range(0.4f, 0.9f));
        isDecidingAction = false;
    }

    // --- ATAQUES ---

    private IEnumerator AtaqueCuerpoACuerpo()
    {
        ejecutandoAtaque = true;
        currentState = State.Attack;
        rb.linearVelocity = Vector2.zero;

        if (animator != null) animator.SetTrigger("Ataque");

        yield return new WaitForSeconds(0.25f); // Tiempo del golpe

        // Aplica daño si el jugador está en frente (Rango tolerante de 3.5f)
        if (playerTarget != null && Vector2.Distance(transform.position, playerTarget.position) <= 3.5f)
        {
            SaludJugador salud = playerTarget.GetComponent<SaludJugador>();
            if (salud != null)
            {
                salud.RecibirDano(danoCuerpoACuerpo);
            }
        }

        yield return new WaitForSeconds(0.4f);
        TerminarAtaque();
    }

    private IEnumerator AtaqueEspadaLarga()
    {
        ejecutandoAtaque = true;
        currentState = State.Attack;
        rb.linearVelocity = Vector2.zero;

        if (animator != null) animator.SetTrigger("AtaqueEspada");

        yield return new WaitForSeconds(0.2f); // Tiempo de preparación

        if (objetoEspadaLarga != null)
        {
            objetoEspadaLarga.SetActive(true);

            // Disparar la animación de sprites en el objeto hijo de las tijeras
            Animator animTijera = objetoEspadaLarga.GetComponent<Animator>();
            if (animTijera != null)
            {
                animTijera.Play(0, -1, 0f); // Inicia la animación desde el frame 0
            }

            // Definir escala considerando la orientación actual del personaje
            float dirX = spriteRenderer != null && spriteRenderer.flipX ? -1f : 1f;
            Vector3 escalaInicial = new Vector3(0.1f * dirX, 1f, 1f);
            Vector3 escalaFinal = new Vector3(1f * dirX, 1f, 1f);

            // Extensión progresiva (Estiramiento fluido)
            float tiempo = 0f;
            float duracionEstiramiento = 0.15f;

            while (tiempo < duracionEstiramiento)
            {
                tiempo += Time.deltaTime;
                objetoEspadaLarga.transform.localScale = Vector3.Lerp(escalaInicial, escalaFinal, tiempo / duracionEstiramiento);
                yield return null;
            }

            // Mantener las tijeras extendidas y abiertas durante el ataque
            yield return new WaitForSeconds(duracionAtaqueEspada);

            // Retracción progresiva
            tiempo = 0f;
            while (tiempo < duracionEstiramiento)
            {
                tiempo += Time.deltaTime;
                objetoEspadaLarga.transform.localScale = Vector3.Lerp(escalaFinal, escalaInicial, tiempo / duracionEstiramiento);
                yield return null;
            }

            objetoEspadaLarga.transform.localScale = escalaFinal;
            objetoEspadaLarga.SetActive(false);
        }

        yield return new WaitForSeconds(0.3f);
        TerminarAtaque();
    }   

   private IEnumerator AtaqueVolarYAplastar()
    {
        ejecutandoAtaque = true;
        currentState = State.Attack;

        if (animator != null) animator.SetTrigger("Volar");

        
        float tiempoSubida = 0f;
        while (tiempoSubida < 0.35f)
        {
            tiempoSubida += Time.deltaTime;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 16f);
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;

       
        float tiempoSeguimiento = 0f;
        while (tiempoSeguimiento < 0.4f && playerTarget != null)
        {
            tiempoSeguimiento += Time.deltaTime;
            float dirX = Mathf.Sign(playerTarget.position.x - transform.position.x);
            rb.linearVelocity = new Vector2(dirX * velocidadVueloHorizontal, 0f);

            // Si ya se alineó sobre el jugador, cae de inmediato
            if (Mathf.Abs(transform.position.x - playerTarget.position.x) < 0.2f) break;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
        if (animator != null) animator.SetTrigger("Aplastamiento");
    
        float tiempoCaida = 0f;
        bool tocoSuelo = false;

        while (!tocoSuelo && tiempoCaida < 2.0f)
        {
            tiempoCaida += Time.deltaTime;
            rb.linearVelocity = new Vector2(0f, -fuerzaCaidaAplastado);

            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.down, 1.3f);

            foreach (var hit in hits)
            {
                // Si el objeto tocado NO es el Boss ni el Player, significa que es el suelo/escenario
                if (hit.collider != null && hit.collider.gameObject != gameObject && !hit.collider.CompareTag("Player"))
                {
                    tocoSuelo = true;
                    break;
                }
            }

            yield return null;
        }

        // Detener la velocidad de golpe al tocar el suelo
        rb.linearVelocity = Vector2.zero;
        rb.linearVelocity = Vector2.zero;

        // 4. DAÑO DE ÁREA AJUSTADO (Radio de 2.5f)
        Vector2 puntoImpacto = (Vector2)transform.position + Vector2.down * 0.5f;
        float radioImpacto = 4.5f; 

        Collider2D[] colisiones = Physics2D.OverlapCircleAll(puntoImpacto, radioImpacto);

        foreach (Collider2D col in colisiones)
        {
            if (col.CompareTag("Player"))
            {
                SaludJugador salud = col.GetComponentInParent<SaludJugador>();
                if (salud == null) salud = col.GetComponentInChildren<SaludJugador>();

                if (salud != null)
                {
                    salud.RecibirDano(danoAplastamiento);
                }
            }
        }

        //Pequeña pausa de recuperación tras el impacto antes de volver a moverse
        yield return new WaitForSeconds(0.4f);
        TerminarAtaque();
    }

    // Visualizador en la ventana Scene
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 puntoImpacto = (Vector2)transform.position + Vector2.down * 0.5f;
        Gizmos.DrawWireSphere(puntoImpacto, 2.5f);
    }

    private void TerminarAtaque()
    {
        ejecutandoAtaque = false;
        isDecidingAction = false;
        currentState = State.Stand;
    }


    public void RecibirImpacto()
    {
        if (recibiendoDano) return;

        StopAllCoroutines();
        if (objetoEspadaLarga != null) objetoEspadaLarga.SetActive(false);

        StartCoroutine(RutinaRetroceso());
    }

    private IEnumerator RutinaRetroceso()
    {
        recibiendoDano = true;
        ejecutandoAtaque = false;
        currentState = State.WalkBack;

        if (animator != null) animator.SetTrigger("WalkBack");
        rb.linearVelocity = new Vector2(-direction * fuerzaRetroceso, rb.linearVelocity.y);

        yield return new WaitForSeconds(tiempoRetroceso);

        recibiendoDano = false;
        isDecidingAction = false;
        currentState = State.Stand;
    }
}