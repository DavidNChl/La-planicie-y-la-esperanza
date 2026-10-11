using UnityEngine;

public class EnemigoMovimiento : MonoBehaviour
{
    [Header("Parámetros de Movimiento")]
    public float velocidad = 3.5f;
    public float fuerzaSalto = 9f;

    public bool MirandoDerecha { get; private set; } = false;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void MoverseHacia(Vector3 objetivoPosition)
    {
        if (rb == null) return;

        float direccionX = objetivoPosition.x - transform.position.x;
        float movH = direccionX > 0 ? 1f : -1f;

        rb.linearVelocity = new Vector2(movH * velocidad, rb.linearVelocity.y);

        if (movH > 0 && !MirandoDerecha) Girar();
        else if (movH < 0 && MirandoDerecha) Girar();
    }

    public void FrenarHorizontal()
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
    }

    public void Saltar()
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
        }
    }

    private void Girar()
    {
        MirandoDerecha = !MirandoDerecha;
        transform.Rotate(0f, 180f, 0f);
    }
}