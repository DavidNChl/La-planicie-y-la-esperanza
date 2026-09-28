using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class ControladorJugador : MonoBehaviour
{
    private CinemachineImpulseSource impulseSource;

    [Header("Detección de Caída / Shake")]
    public float velocidadMinimaCaidaShake = -8f; // Caídas más rápidas que esto activan el shake
    private float velocidadCaidaPrevia;           // Para registrar la velocidad ANTES de la colisión

    [Header("Movimiento")]
    public float velocidad = 8f;
    public float fuerzaSalto = 12f;

    [Header("Ataque y Cadencia")]
    public GameObject prefabProyectil;
    public Transform puntoDisparo;
    public float cadenciaDisparo = 0.4f;
    private float tiempoSiguienteDisparo = 0f;

    [Header("Detección de Suelo")]
    public Transform puntoSuelo;
    public float radioDeteccion = 0.35f;
    public LayerMask capaSuelo;

    [Header("Tiempo de Inactividad / Descanso")]
    public float tiempoParaDescansar = 3f; 
    private float contadorInactividad = 0f;
    private bool estaDescansando = false;

    private Rigidbody2D rb;
    private Animator animator;
    private float movimientoH;
    private bool enSuelo;
    private bool mirandoDerecha = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    void Update()
    {
        // Movimiento del jugador
        movimientoH = 0f;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            {
                movimientoH = -1f;
            }
            else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            {
                movimientoH = 1f;
            }
        }

        // Inactividad y activación de animación
        if (movimientoH == 0f && enSuelo)
        {
            contadorInactividad += Time.deltaTime;

            if (contadorInactividad >= tiempoParaDescansar && !estaDescansando)
            {
                estaDescansando = true;
                if (animator != null)
                {
                    animator.SetTrigger("descansar"); 
                }
            }
        }
        else
        {
            contadorInactividad = 0f;
            estaDescansando = false;
        }

        // Detecta suelo
        if (puntoSuelo != null)
        {
            enSuelo = Physics2D.OverlapCircle(puntoSuelo.position, radioDeteccion, capaSuelo);
        }

        // Animación de caminata
        if (animator != null)
        {
            animator.SetBool("camina", movimientoH != 0f);
        }

        // Configuración de salto
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && enSuelo)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
            contadorInactividad = 0f; 
        }

        // Sección del disparo
        bool presionoDisparo = (Keyboard.current != null && Keyboard.current.jKey.wasPressedThisFrame) ||
                               (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame);

        if (presionoDisparo && Time.time >= tiempoSiguienteDisparo)
        {
            Disparar();
            tiempoSiguienteDisparo = Time.time + cadenciaDisparo;
            contadorInactividad = 0f; 
        }

        // Voltear a los lados
        if (movimientoH > 0 && !mirandoDerecha)
        {
            Girar();
        }
        else if (movimientoH < 0 && mirandoDerecha)
        {
            Girar();
        }
    }

    void FixedUpdate()
    {
        // Guardamos la velocidad vertical justo antes de que la física procese una colisión
        velocidadCaidaPrevia = rb.linearVelocity.y;

        rb.linearVelocity = new Vector2(movimientoH * velocidad, rb.linearVelocity.y);
    }

    private void Disparar()
    {
        if (prefabProyectil != null && puntoDisparo != null)
        {
            Instantiate(prefabProyectil, puntoDisparo.position, puntoDisparo.rotation);
        }
    }

    private void Girar()
    {
        mirandoDerecha = !mirandoDerecha;
        transform.Rotate(0f, 180f, 0f);
    }

    private void OnDrawGizmosSelected()
    {
        if (puntoSuelo != null)
        {
            Gizmos.color = enSuelo ? Color.green : Color.red;
            Gizmos.DrawWireSphere(puntoSuelo.position, radioDeteccion);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Evaluamos la velocidad que traía JUSTO ANTES del golpe
        if (velocidadCaidaPrevia <= velocidadMinimaCaidaShake)
        {
            if (impulseSource != null)
            {
                // Dispara el impulso básico con los parámetros del Inspector
                impulseSource.GenerateImpulse();
            }
        }
    }
}