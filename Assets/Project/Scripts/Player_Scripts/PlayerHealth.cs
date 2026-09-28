using UnityEngine;
using UnityEngine.UI;

public class SaludJugador : MonoBehaviour
{
    [Header("Salud")]
    public float vidaMaxima = 5f;
    private float vidaActual;

    [Header("Invulnerabilidad (i-Frames)")]
    public float tiempoInvulnerabilidad = 1.5f;
    private bool esInvulnerable = false;

    [Header("Interfaz UI")]
    public Slider barraVida; // O imágenes de corazones

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool estaMuerto = false;

    void Start()
    {
        vidaActual = vidaMaxima;
        
        // Busca en los hijos por si el SpriteRenderer/Animator está en el objeto hijo
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        ActualizarUI();
    }

    public void RecibirDano(float cantidadDano)
    {
        if (estaMuerto || esInvulnerable) return;

        vidaActual -= cantidadDano;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);

        ActualizarUI();

        if (vidaActual <= 0f)
        {
            Morir();
        }
        else
        {
            // Trigger opcional para animación de herida / daño
            if (animator != null)
            {
                animator.SetTrigger("dano");
            }

            // Inicia tiempo de gracia donde no puede recibir daño continuo
            StartCoroutine(RutinaInvulnerabilidad());
        }
    }

    private System.Collections.IEnumerator RutinaInvulnerabilidad()
    {
        esInvulnerable = true;

        // Efecto visual de parpadeo opcional
        if (spriteRenderer != null)
        {
            float tiempoPasado = 0f;
            while (tiempoPasado < tiempoInvulnerabilidad)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled; // Parpadea el sprite
                yield return new WaitForSeconds(0.1f);
                tiempoPasado += 0.1f;
            }
            spriteRenderer.enabled = true; // Asegura que quede visible
        }
        else
        {
            yield return new WaitForSeconds(tiempoInvulnerabilidad);
        }

        esInvulnerable = false;
    }

    private void Morir()
    {
        estaMuerto = true;

        if (animator != null)
        {
            animator.SetTrigger("muerte");
        }

        if (barraVida != null)
        {
            barraVida.gameObject.SetActive(false);
        }
        // Desactivar el control del jugador al morir
        ControladorJugador controlador = GetComponent<ControladorJugador>();
        if (controlador != null)
        {
            controlador.enabled = false;
        }

        // Frenar físicas residuales
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        //Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        //foreach (Collider2D c in colliders)
        //{
        //    c.enabled = false;
        //}
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
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