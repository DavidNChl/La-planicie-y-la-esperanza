using UnityEngine;

public class EnemigoAtaque : MonoBehaviour
{
    [Header("Parámetros de Ataque")]
    public float distanciaAtaque = 1.2f;
    public float danoAtaque = 1f;
    public float cadenciaAtaque = 1.5f;

    private float tiempoSiguienteAtaque = 0f;
    private Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public bool PuedeAtacar()
    {
        return Time.time >= tiempoSiguienteAtaque;
    }

    public void EjecutarAtaque()
    {
        if (animator != null)
        {
            animator.SetTrigger("atacar");
        }
        tiempoSiguienteAtaque = Time.time + cadenciaAtaque;
    }

    public void AplicarDanoAtaque(Transform transformJugador)
    {
        if (transformJugador == null) return;

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
}