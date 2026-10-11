using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instancia { get; private set; } 

    [Header("Paneles de UI")]
    public GameObject panelPausa;       // UI de Pausa
    public GameObject panelGameOver;    // UI de Perder
    public GameObject panelVictoria;    // UI de Ganar

    [Header("Ajustes de Transición UI")]
    public float velocidadFade = 2.5f;   // Velocidad de desvanecimiento

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

        if (panelGameOver != null)
        {
            StartCoroutine(AparecerPanelSuave(panelGameOver)); 
        }
        else
        {
            Time.timeScale = 0f;
        }
    }

    public void ActivarVictoria()
    {
        if (juegoTerminado) return; 
        juegoTerminado = true;

        if (panelVictoria != null)
        {
            StartCoroutine(AparecerPanelSuave(panelVictoria)); 
        }
        else
        {
            Time.timeScale = 0f; 
        }
    }

    private IEnumerator AparecerPanelSuave(GameObject panel)
    {
        panel.SetActive(true); 
        CanvasGroup cg = panel.GetComponent<CanvasGroup>();

        if (cg != null)
        {
            cg.alpha = 0f;
            cg.interactable = false;
            cg.blocksRaycasts = false;

            float tiempo = 0f;
            while (tiempo < 1f)
            {
                tiempo += Time.unscaledDeltaTime * velocidadFade;
                cg.alpha = Mathf.Clamp01(tiempo);
                yield return null;
            }

            cg.alpha = 1f;
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }

        Time.timeScale = 0f;
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

    public void CargarSiguienteNivel()
    {
        Time.timeScale = 1f; 
        int siguienteEscena = SceneManager.GetActiveScene().buildIndex + 1;
        
        if (siguienteEscena < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(siguienteEscena);
        }
        else
        {
            IrAlMenuPrincipal(); 
        }
    }

    public void IrAlMenuPrincipal()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(0); 
    }
}