using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MuerteJugador : MonoBehaviour
{
    [Header("Configuración de Muerte")]
    [Tooltip("Tiempo en segundos antes de activar la pantalla de Game Over")]
    public float tiempoEsperaGameOver = 1.5f;

    private Animator animator;
    private Rigidbody2D rb;
    private SaludJugador salud;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        salud = GetComponent<SaludJugador>();
    }

    private void OnEnable()
    {
        if (salud != null)
        {
            salud.OnMuerte.AddListener(EjecutarMuerte);
        }
    }

    private void OnDisable()
    {
        if (salud != null)
        {
            salud.OnMuerte.RemoveListener(EjecutarMuerte);
        }
    }

    private void EjecutarMuerte()
    {
        StartCoroutine(SecuenciaMuerte());
    }

    private IEnumerator SecuenciaMuerte()
    {
       
        if (salud != null && salud.barraVida != null)
        {
            salud.barraVida.gameObject.SetActive(false);
        }

       
        JugadorMovimiento movimiento = GetComponent<JugadorMovimiento >();
        if (movimiento != null)
        {
            movimiento.enabled = false; 
        }
       
        if (animator != null)
        {
            animator.SetTrigger("muerte");
        }

       
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false; 
        }

     
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        yield return new WaitForSeconds(tiempoEsperaGameOver);

        if (GameManager.Instancia != null)
        {
            GameManager.Instancia.ActivarGameOver();
        }
    }
}