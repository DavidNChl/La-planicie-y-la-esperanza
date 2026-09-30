using UnityEngine;

public class EnemigoBrainIA : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform transformJugador;

    private Rigidbody2D rb;
    private Animator animator;
    private EnemigoDeteccion deteccion;
    private EnemigoMovimiento movimiento;
    private EnemigoAtaque ataque;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        deteccion = GetComponent<EnemigoDeteccion>();
        movimiento = GetComponent<EnemigoMovimiento>();
        ataque = GetComponent<EnemigoAtaque>();

        if (transformJugador == null)
        {
            GameObject jugador = GameObject.FindGameObjectWithTag("Player");
            if (jugador != null) transformJugador = jugador.transform;
        }
    }

    void Update()
    {
        if (transformJugador == null || rb == null) return;

        // 1. Actualizar sensores del entorno
        if (deteccion != null)
        {
            deteccion.ActualizarDetecciones();
        }

        // 2. Evaluar decisiones de salto
        if (deteccion != null && movimiento != null && deteccion.EnSuelo)
        {
            if (deteccion.HayAcantilado || deteccion.VieneProyectil || deteccion.HayPared)
            {
                movimiento.Saltar();
                deteccion.ForzarEnSueloFalso();
            }
        }

        // 3. Evaluar persecución vs. ataque
        float distanciaAlJugador = Vector2.Distance(transform.position, transformJugador.position);
        float rangoAtaque = ataque != null ? ataque.distanciaAtaque : 1.2f;

        if (distanciaAlJugador <= rangoAtaque)
        {
            if (movimiento != null) movimiento.FrenarHorizontal();

            if (ataque != null && ataque.PuedeAtacar())
            {
                ataque.EjecutarAtaque();
            }
        }
        else
        {
            if (movimiento != null) movimiento.MoverseHacia(transformJugador.position);
        }

        // 4. Sincronizar animaciones
        if (animator != null)
        {
            animator.SetBool("camina", Mathf.Abs(rb.linearVelocity.x) > 0.1f);
        }
    }

    // Método expuesto para el bridge script (EventosAnimacionEnemigo.cs)
    public void AplicarDanoAtaque()
    {
        if (ataque != null)
        {
            ataque.AplicarDanoAtaque(transformJugador);
        }
    }
}