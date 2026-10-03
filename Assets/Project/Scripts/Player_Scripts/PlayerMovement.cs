using UnityEngine;

public class JugadorMovimiento : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 8f;
    public float fuerzaSalto = 12f;

    [Header("Detección de Suelo")]
    public Transform puntoSuelo;
    public float radioDeteccion = 0.35f;
    public LayerMask capaSuelo;

    public bool EnSuelo { get; private set; }
    public bool MirandoDerecha { get; private set; } = true;

    private Rigidbody2D rb;
    private JugadorInput input;
    private Animator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<JugadorInput>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        ComprobarSuelo();
        ProcesarSalto();
        ProcesarGiro();
    }

    private void FixedUpdate()
    {
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
            if (animator != null)
            {
                AnimatorStateInfo estadoActual = animator.GetCurrentAnimatorStateInfo(0);
                if (estadoActual.IsTag("SinSalto"))
                {
                    return; 
                }
            }

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
           
            AudioJugador audioJugador = GetComponent<AudioJugador>();
            if (audioJugador != null)
            {
                audioJugador.ReproducirSonidoSalto();
            }
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

    private void OnDrawGizmosSelected()
    {
        if (puntoSuelo != null)
        {
            Gizmos.color = EnSuelo ? Color.green : Color.red;
            Gizmos.DrawWireSphere(puntoSuelo.position, radioDeteccion);
        }
    }
}