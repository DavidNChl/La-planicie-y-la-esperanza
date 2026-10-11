using UnityEngine;

public class EnemigoDeteccion : MonoBehaviour
{
    [Header("Detección de Suelo")]
    public Transform puntoSuelo;
    public float radioSuelo = 0.2f;
    public LayerMask capaSuelo;

    [Header("Detección de Acantilados")]
    public Transform detectorSuelo;
    public float distanciaRaycastSuelo = 1.5f;

    [Header("Detección de Proyectiles")]
    public Transform detectorProyectil;
    public float distanciaDeteccionProyectil = 3f;
    public LayerMask capaProyectil;

    [Header("Detección de Paredes")]
    public Transform detectorPared;
    public float distanciaRaycastPared = 0.8f;
    public LayerMask capaPared;

    // Propiedades públicas consultadas por el cerebro de la IA
    public bool EnSuelo { get; private set; }
    public bool HayAcantilado { get; private set; }
    public bool VieneProyectil { get; private set; }
    public bool HayPared { get; private set; }

    private Rigidbody2D rb;
    private EnemigoMovimiento movimiento;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        movimiento = GetComponent<EnemigoMovimiento>();
    }

    public void ActualizarDetecciones()
    {
        Physics2D.queriesStartInColliders = false;
        bool mirandoDerecha = movimiento != null && movimiento.MirandoDerecha;

        // 1. Suelo real
        if (rb != null && rb.linearVelocity.y <= 0.1f && puntoSuelo != null)
        {
            EnSuelo = Physics2D.OverlapCircle(puntoSuelo.position, radioSuelo, capaSuelo);
        }
        else
        {
            EnSuelo = false;
        }

        // 2. Acantilado
        HayAcantilado = false;
        if (detectorSuelo != null)
        {
            HayAcantilado = !Physics2D.Raycast(detectorSuelo.position, Vector2.down, distanciaRaycastSuelo, capaSuelo);
        }

        // 3. Proyectil frontal
        VieneProyectil = false;
        if (detectorProyectil != null)
        {
            Vector2 direccion = mirandoDerecha ? Vector2.right : Vector2.left;
            VieneProyectil = Physics2D.Raycast(detectorProyectil.position, direccion, distanciaDeteccionProyectil, capaProyectil);
        }

        // 4. Pared frontal
        HayPared = false;
        if (detectorPared != null)
        {
            Vector2 direccion = mirandoDerecha ? Vector2.right : Vector2.left;
            HayPared = Physics2D.Raycast(detectorPared.position, direccion, distanciaRaycastPared, capaPared);
        }
    }

    public void ForzarEnSueloFalso()
    {
        EnSuelo = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (puntoSuelo != null)
        {
            Gizmos.color = EnSuelo ? Color.green : Color.red;
            Gizmos.DrawWireSphere(puntoSuelo.position, radioSuelo);
        }

        if (detectorSuelo != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(detectorSuelo.position, detectorSuelo.position + Vector3.down * distanciaRaycastSuelo);
        }

        if (detectorProyectil != null)
        {
            Gizmos.color = Color.cyan;
            Vector3 dir = transform.eulerAngles.y == 0 ? Vector3.right : Vector3.left;
            Gizmos.DrawLine(detectorProyectil.position, detectorProyectil.position + dir * distanciaDeteccionProyectil);
        }

        if (detectorPared != null)
        {
            Gizmos.color = Color.magenta;
            Vector3 dir = transform.eulerAngles.y == 0 ? Vector3.right : Vector3.left;
            Gizmos.DrawLine(detectorPared.position, detectorPared.position + dir * distanciaRaycastPared);
        }
    }
}