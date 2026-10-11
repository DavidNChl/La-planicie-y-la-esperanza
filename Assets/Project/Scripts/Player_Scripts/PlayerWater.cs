using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class JugadorAgua : MonoBehaviour
{
    [Header("Configuración de Natación")]
    public float fuerzaImpulsoAgua = 7f;      // Fuerza para nadar/impulsarse en el agua
    public float multiplicadorVelocidad = 0.6f; // Reduce la velocidad de movimiento horizontal en el agua

    [Header("Referencias Opcionales")]
    private Rigidbody2D rb;
    private JugadorInput input;
    private AudioJugador audioJugador;

    public bool EnAgua { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<JugadorInput>();
        audioJugador = GetComponent<AudioJugador>();
    }

    private void Update()
    {
        if (!EnAgua) return;

        // Si presiona el botón de salto dentro del agua, aplica un impulso hacia arriba
        if (input != null && input.SaltoPresionado)
        {
            ImpulsarEnAgua();
        }
    }

    public void EntrarAlAgua()
    {
        EnAgua = true;
    }

    public void SalirDelAgua()
    {
        EnAgua = false;
    }

    private void ImpulsarEnAgua()
    {
        // Resetea la velocidad vertical momentáneamente para un impulso constante
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaImpulsoAgua);

        if (audioJugador != null)
        {
            audioJugador.ReproducirSonidoSalto();
        }
    }
}
    
