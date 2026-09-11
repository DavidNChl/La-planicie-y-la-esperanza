using UnityEngine;
using UnityEngine.UI;

public class SaludEnemigo : MonoBehaviour
{
    [Header("Salud")]
    public float vidaMaxima = 3f;
    private float vidaActual;

    [Header("Interfaz UI")]
    public Slider barraVida;

    [Header("Efectos")]
    public GameObject efectoDestruccion;
    public float tiempoDestruccionTrasMuerte = 0.8f; 

    private Animator animator;
    private Collider2D col;
    private Rigidbody2D rb;
    private bool estaMuerto = false;

    void Start()
    {
        vidaActual = vidaMaxima;
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        if (barraVida != null)
        {
            barraVida.maxValue = vidaMaxima;
            barraVida.value = vidaActual;
        }
    }

    public void RecibirDano(float cantidadDano)
    {
        if (estaMuerto) return;

        vidaActual -= cantidadDano;

        if (barraVida != null)
        {
            barraVida.value = vidaActual;
        }

        if (vidaActual <= 0f)
        {
            Morir();
        }
    }

    private void Morir()
    {
        estaMuerto = true;

        if (barraVida != null)
        {
            barraVida.gameObject.SetActive(false);
        }

        if (animator != null)
        {
            animator.SetTrigger("muerte");
        }

        // desactivar colisiones 
        if (col != null) col.enabled = false;
        if (rb != null) rb.simulated = false;

        
        if (efectoDestruccion != null)
        {
            Instantiate(efectoDestruccion, transform.position, Quaternion.identity);
        }

        Destroy(gameObject, tiempoDestruccionTrasMuerte);
    }
}