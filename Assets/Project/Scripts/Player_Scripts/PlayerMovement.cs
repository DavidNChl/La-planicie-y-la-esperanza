using UnityEngine;
using Unity.Cinemachine;

public class JugadorMovimiento : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 8f;
    public float fuerzaSalto = 12f;

    [Header("Detección de Suelo")]
    public Transform puntoSuelo;
    public float radioDeteccion = 0.35f;
    public LayerMask capaSuelo;

    [Header("Detección de Caída / Shake")]
    public float velocidadMinimaCaidaShake = -8f;
    private float velocidadCaidaPrevia;

    // Propiedades públicas para que otros scripts sepan el estado físico
    public bool EnSuelo { get; private set; }
    public bool MirandoDerecha { get; private set; } = true;

    private Rigidbody2D rb;
    private JugadorInput input;
    private CinemachineImpulseSource impulseSource;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<JugadorInput>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    void Update()
    {
        ComprobarSuelo();
        ProcesarSalto();
        ProcesarGiro();
    }

    void FixedUpdate()
    {
        velocidadCaidaPrevia = rb.linearVelocity.y;
        
        // Movimiento horizontal en física
        rb.linearVelocity = new Vector2(input.MovimientoH * velocidad, rb.linearVelocity.y);
    }

    private void ComprobarSuelo()
    {
        if (puntoSuelo != null)
        {
            Physics2D.queriesStartInColliders = false;
            EnSuelo = Physics2D.OverlapCircle(puntoSuelo.position, radioDeteccion, capaSuelo);
        }
    }

    private void ProcesarSalto()
    {
        if (input.SaltoPresionado && EnSuelo)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
        }
    }

    private void ProcesarGiro()
    {
        if ((input.MovimientoH > 0 && !MirandoDerecha) || (input.MovimientoH < 0 && MirandoDerecha))
        {
            MirandoDerecha = !MirandoDerecha;
            transform.Rotate(0f, 180f, 0f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (velocidadCaidaPrevia <= velocidadMinimaCaidaShake && impulseSource != null)
        {
            impulseSource.GenerateImpulse();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (puntoSuelo != null)
        {
            Gizmos.color = EnSuelo ? Color.green : Color.red;
            Gizmos.DrawWireSphere(puntoSuelo.position, radioDeteccion);
        }
    }
}