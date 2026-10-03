using UnityEngine;
using Unity.Cinemachine;

public class EfectoImpactoCaida : MonoBehaviour
{
    public float velocidadMinimaCaidaShake = -8f;
    private CinemachineImpulseSource impulseSource;
    private Rigidbody2D rb;
    private float velocidadCaidaPrevia;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void FixedUpdate()
    {
        velocidadCaidaPrevia = rb.linearVelocity.y;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (velocidadCaidaPrevia <= velocidadMinimaCaidaShake && impulseSource != null)
        {
            impulseSource.GenerateImpulse();
        }
    }
}