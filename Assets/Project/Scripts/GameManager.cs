using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instancia { get; private set; }

    [Header("Paneles de UI")]
    public GameObject panelPausa;
    public GameObject panelGameOver;
    public GameObject panelVictoria;

    [Header("Estado del Juego")]
    public bool juegoPausado = false;
    public bool juegoTerminado = false;

    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;
    }

    private void OnDestroy()
    {
        //Limpia la referencia al destruirse la escena
        if (Instancia == this)
        {
            Instancia = null;
        }
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (!juegoTerminado)
            {
                TogglePausa();
            }
        }
    }

    public void TogglePausa()
    {
        juegoPausado = !juegoPausado;
        Time.timeScale = juegoPausado ? 0f : 1f;

        if (panelPausa != null)
        {
            panelPausa.SetActive(juegoPausado);
        }
    }

    public void ActivarGameOver()
    {
        if (juegoTerminado) return;
        juegoTerminado = true;
        Time.timeScale = 0f;

        if (panelGameOver != null)
        {
            panelGameOver.SetActive(true);
        }
    }

    public void ActivarVictoria()
    {
        if (juegoTerminado) return;
        juegoTerminado = true;
        Time.timeScale = 0f;

        if (panelVictoria != null)
        {
            panelVictoria.SetActive(true);
        }
    }

    public void ContinuarJuego()
    {
        if (juegoPausado)
        {
            TogglePausa();
        }
    }

    public void ReiniciarNivel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void IrAlMenuPrincipal()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(0);
    }
}