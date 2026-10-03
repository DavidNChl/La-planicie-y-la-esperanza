using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SaludJugador : MonoBehaviour
{
    [Header("Salud")]
    public float vidaMaxima = 5f;
    private float vidaActual;

    [Header("Interfaz UI")]
    public Slider barraVida;

    [Header("Eventos")]
    public UnityEvent OnMuerte; 

    private Animator animator;
    private InvulnerabilidadJugador invulnerabilidad;

    public bool EstaMuerto { get; private set; } = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        invulnerabilidad = GetComponent<InvulnerabilidadJugador>();
    }

    private void Start()
    {
        vidaActual = vidaMaxima;
        ActualizarUI();
    }

    public void RecibirDano(float cantidadDano)
    {
        if (EstaMuerto) return;

        if (invulnerabilidad != null && invulnerabilidad.EsInvulnerable) return;

        vidaActual -= cantidadDano;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);

        ActualizarUI();
        if (EfectoDanoPantalla.Instancia != null)
        {
            EfectoDanoPantalla.Instancia.MostrarEfectoDano();
        }
        
        if (vidaActual <= 0f)
        {
            EstaMuerto = true;
            OnMuerte?.Invoke(); 
        }
        else
        {
            if (animator != null)
            {
                animator.SetTrigger("dano");
            }

            if (invulnerabilidad != null)
            {
                invulnerabilidad.ActivarInvulnerabilidad();
            }
        }
        
        AudioJugador audioJugador = GetComponent<AudioJugador>();
        if (audioJugador != null)
        {
            audioJugador.ReproducirSonidoDano();
        }   
    }

    private void ActualizarUI()
    {
        if (barraVida != null)
        {
            barraVida.maxValue = vidaMaxima;
            barraVida.value = vidaActual;
        }
    }
}