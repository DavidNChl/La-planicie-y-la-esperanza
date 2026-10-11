using UnityEngine;

public class EfectosAgua : MonoBehaviour
{
    [Header("Efecto de Partículas Reutilizable")]
    public GameObject prefabSalpicadura; 

    private BoxCollider2D boxCollider;
    private bool seEstaCerrando = false;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void OnApplicationQuit()
    {
        seEstaCerrando = true;
    }

    private void OnDestroy()
    {
        seEstaCerrando = true;
    }

    private void GenerarSalpicadura(Collider2D col)
    {
        if (prefabSalpicadura == null || seEstaCerrando) return;

        float alturaSuperficie = transform.position.y + (boxCollider != null ? boxCollider.size.y / 2f : 0f);
        Vector3 puntoContacto = new Vector3(col.transform.position.x, alturaSuperficie, col.transform.position.z);

        GameObject vfx = Instantiate(prefabSalpicadura, puntoContacto, Quaternion.identity);

        ParticleSystem ps = vfx.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            float tiempoDeVidaTotal = ps.main.duration + ps.main.startLifetime.constantMax;
            Destroy(vfx, tiempoDeVidaTotal);
        }
        else
        {
            Destroy(vfx, 2f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (seEstaCerrando) return;

        if (collision.CompareTag("Player"))
        {
            // Notificar al componente de agua del jugador
            JugadorAgua aguaJugador = collision.GetComponent<JugadorAgua>();
            if (aguaJugador != null)
            {
                aguaJugador.EntrarAlAgua();
            }

            GenerarSalpicadura(collision);

            if (AudioManager.Instancia != null && AudioManager.Instancia.sfxEntrarAgua != null)
            {
                AudioManager.Instancia.ReproducirSFX(AudioManager.Instancia.sfxEntrarAgua);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (seEstaCerrando || !gameObject.activeInHierarchy) return;

        if (collision.CompareTag("Player"))
        {
            // Notificar salida
            JugadorAgua aguaJugador = collision.GetComponent<JugadorAgua>();
            if (aguaJugador != null)
            {
                aguaJugador.SalirDelAgua();
            }

            GenerarSalpicadura(collision);

            if (AudioManager.Instancia != null && AudioManager.Instancia.sfxSalirAgua != null)
            {
                AudioManager.Instancia.ReproducirSFX(AudioManager.Instancia.sfxSalirAgua);
            }
        }
    }
}