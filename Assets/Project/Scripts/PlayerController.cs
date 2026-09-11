using UnityEngine;
using UnityEngine.InputSystem;

public class ControladorJugador : MonoBehaviour
{
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

        // Inactividad y activacion de animacion
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

        // detecta suelo
        if (puntoSuelo != null)
        {
            enSuelo = Physics2D.OverlapCircle(puntoSuelo.position, radioDeteccion, capaSuelo);
        }

        // animacion de caminata canina
        if (animator != null)
        {
            animator.SetBool("camina", movimientoH != 0f);
        }

        // configuracion de salto
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && enSuelo)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
            contadorInactividad = 0f; 
        }

        // seccion del disparo
        bool presionoDisparo = (Keyboard.current != null && Keyboard.current.jKey.wasPressedThisFrame) ||
                               (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame);

        if (presionoDisparo && Time.time >= tiempoSiguienteDisparo)
        {
            Disparar();
            tiempoSiguienteDisparo = Time.time + cadenciaDisparo;
            contadorInactividad = 0f; 
        }

        // voltear a los lados
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




}