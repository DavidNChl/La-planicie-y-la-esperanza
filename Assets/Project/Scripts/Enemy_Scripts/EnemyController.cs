using UnityEngine;

public class ControladorEnemigoIA : MonoBehaviour
{
    [Header("Persecución y Movimiento")]
    public float velocidad = 3.5f;
    public float fuerzaSalto = 9f;
    public Transform transformJugador;

    [Header("Ataque")]
    public float distanciaAtaque = 1.2f;
    public float danoAtaque = 1f;
    public float cadenciaAtaque = 1.5f;
    private float tiempoSiguienteAtaque = 0f;

    [Header("Detección de Suelo y Acantilados")]
    public Transform puntoSuelo;          // Asigna aquí el objeto en los pies del enemigo
    public float radioSuelo = 0.2f;       // Usaremos una pequeña esfera/círculo igual que en el jugador
    public Transform detectorSuelo;      // Punto al frente-abajo para detectar bordes
    public float distanciaRaycastSuelo = 1.5f;
    public LayerMask capaSuelo;

    [Header("Detección de Proyectiles (Huevos)")]
    public Transform detectorProyectil;  // Punto al frente del enemigo
    public float distanciaDeteccionProyectil = 3f;
    public LayerMask capaProyectil;

    [Header("Detección de Paredes")]
public Transform detectorPared;        // Punto al frente del enemigo (altura media)
public float distanciaRaycastPared = 0.8f;
public LayerMask capaPared;            // Normalmente la misma que 'capaSuelo'

    // Componentes y estado
    private Rigidbody2D rb;
    private Animator animator;
    private SaludEnemigo saludEnemigo;
    private bool mirandoDerecha = false;
    private bool enSuelo = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        saludEnemigo = GetComponent<SaludEnemigo>();
        animator = GetComponentInChildren<Animator>();

        // Si no asignaste al jugador en el Inspector, lo busca por su etiqueta
        if (transformJugador == null)
        {
            GameObject jugador = GameObject.FindGameObjectWithTag("Player");
            if (jugador != null) transformJugador = jugador.transform;
        }
    }

    void Update()
    {
        // Si el enemigo no tiene jugador a la vista o no tiene componentes, no hace nada
        if (transformJugador == null || rb == null) return;

        // Comprobaciones de entorno
        ComprobarEntorno();

        // Calcular distancia al jugador
        float distanciaAlJugador = Vector2.Distance(transform.position, transformJugador.position);

        if (distanciaAlJugador <= distanciaAtaque)
        {
            // Freno horizontal al atacar
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            
            if (Time.time >= tiempoSiguienteAtaque)
            {
                Atacar();
                tiempoSiguienteAtaque = Time.time + cadenciaAtaque;
            }
        }
        else
        {
            MoverseHaciaJugador();
        }

        // Actualizar animaciones
        if (animator != null)
        {
            animator.SetBool("camina", Mathf.Abs(rb.linearVelocity.x) > 0.1f);
        }
    }

    private void ComprobarEntorno()
    {
        // Desactivar que el raycast detecte el collider propio
        Physics2D.queriesStartInColliders = false;

        // 1. Detección de suelo real usando un OverlapCircle en los pies (solo si no estamos subiendo en un salto)
        if (rb.linearVelocity.y <= 0.1f && puntoSuelo != null)
        {
            enSuelo = Physics2D.OverlapCircle(puntoSuelo.position, radioSuelo, capaSuelo);
        }
        else
        {
            enSuelo = false; // Si va subiendo (linearVelocity.y > 0.1f), FORZAMOS a que no esté en el suelo
        }

        // 2. Detección de acantilado / fin de plataforma
        bool haySueloAdelante = true;
        if (detectorSuelo != null)
        {
            haySueloAdelante = Physics2D.Raycast(detectorSuelo.position, Vector2.down, distanciaRaycastSuelo, capaSuelo);
        }

        // 3. Detección de proyectiles
        bool vieneProyectil = false;
        if (detectorProyectil != null)
        {
            Vector2 direccionFrente = mirandoDerecha ? Vector2.right : Vector2.left;
            vieneProyectil = Physics2D.Raycast(detectorProyectil.position, direccionFrente, distanciaDeteccionProyectil, capaProyectil);
        }

        bool hayParedAdelante = false;
        if (detectorPared != null)
        {
            Vector2 direccionFrente = mirandoDerecha ? Vector2.right : Vector2.left;
            hayParedAdelante = Physics2D.Raycast(detectorPared.position, direccionFrente, distanciaRaycastPared, capaPared);
        }

        // Solo salta si ESTÁ EN EL SUELO y detecta un hueco o un proyectil
        if ((!haySueloAdelante || vieneProyectil || hayParedAdelante) && enSuelo)
        {
            Saltar();
        }
    }

    private void MoverseHaciaJugador()
    {
        float direccionX = transformJugador.position.x - transform.position.x;
        
        // Determinar dirección de movimiento (-1 o 1)
        float movH = direccionX > 0 ? 1f : -1f;

        rb.linearVelocity = new Vector2(movH * velocidad, rb.linearVelocity.y);

        // Voltear sprite según la posición del jugador
        if (movH > 0 && !mirandoDerecha) Girar();
        else if (movH < 0 && mirandoDerecha) Girar();
    }

    private void Saltar()
    {
        enSuelo = false; // Apagamos la variable inmediatamente al iniciar el impulso
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
    }

    private void Atacar()
    {
        if (animator != null)
        {
            animator.SetTrigger("atacar");
        }

    }
    
    public void AplicarDanoAtaque()
    {
        if (transformJugador == null) return;

        // Verifica que el jugador siga dentro del rango de ataque al momento del golpe
        float distanciaActual = Vector2.Distance(transform.position, transformJugador.position);
        if (distanciaActual <= distanciaAtaque + 0.5f)
        {
            SaludJugador saludJugador = transformJugador.GetComponent<SaludJugador>();
            if (saludJugador != null)
            {
                saludJugador.RecibirDano(danoAtaque);
            }
        }
    }
    
    private void Girar()
    {
        mirandoDerecha = !mirandoDerecha;
        transform.Rotate(0f, 180f, 0f);
    }
    
    private void OnDrawGizmosSelected()
    {
        // Visualización de Raycasts en la ventana Scene
        if (detectorSuelo != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(detectorSuelo.position, detectorSuelo.position + Vector3.down * distanciaRaycastSuelo);
        }

        if (detectorProyectil != null)
        {
            Gizmos.color = Color.cyan;
            Vector3 dir = mirandoDerecha ? Vector3.right : Vector3.left;
            Gizmos.DrawLine(detectorProyectil.position, detectorProyectil.position + dir * distanciaDeteccionProyectil);
        }

        if (detectorPared != null)
        {
            Gizmos.color = Color.magenta;
            Vector3 dir = mirandoDerecha ? Vector3.right : Vector3.left;
            Gizmos.DrawLine(detectorPared.position, detectorPared.position + dir * distanciaRaycastPared);
        }
    }
}