using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // --- PATRÓN SINGLETON ---
    public static GameManager Instancia { get; private set; }

    [Header("Progreso y Puntuación")]
    public int puntosTotales = 0;
    public int monedasRecolectadas = 0;

    [Header("Estados del Juego")]
    public bool juegoPausado = false;
    public bool juegoTerminado = false;

    [Header("Interfaz de Usuario (UI)")]
    public GameObject panelPausa;
    public GameObject panelGameOver;
    public GameObject panelVictoria;

    private void Awake()
    {
        // Garantiza que solo exista una instancia activa
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        Instancia = this;

        // Opcional: Descomenta esta línea si quieres que el GameManager no se destruya al cambiar de nivel
        // DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        // Detectar tecla de Pausa (Escape o P)
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (!juegoTerminado)
            {
                TogglePausa();
            }
        }
    }

    // --- GESTIÓN DE PUNTOS Y MONEDAS ---

    public void AgregarPuntos(int cantidad)
    {
        puntosTotales += cantidad;
        // Aquí puedes invocar a tu script de UI para actualizar el texto en pantalla
        // UIManager.Instancia.ActualizarPuntos(puntosTotales);
    }

    public void AgregarMonedas(int cantidad)
    {
        monedasRecolectadas += cantidad;
        // UIManager.Instancia.ActualizarMonedas(monedasRecolectadas);
    }

    // --- FLUJO Y ESTADOS DEL JUEGO ---

    public void TogglePausa()
    {
        juegoPausado = !juegoPausado;

        // Congela o reanuda el tiempo del motor de física
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
        Time.timeScale = 0f; // Pausa el juego tras morir

        if (panelGameOver != null)
        {
            panelGameOver.SetActive(true);
        }
    }

    public void ActivarVictoria()
    {
        juegoTerminado = true;
        Time.timeScale = 0f;

        if (panelVictoria != null)
        {
            panelVictoria.SetActive(true);
        }
    }

    // --- CONTROL DE ESCENAS ---

    public void ReiniciarNivel()
    {
        Time.timeScale = 1f; // Restablece el tiempo antes de recargar
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
            // Si no hay más niveles, vuelve al Menú Principal (Escena 0)
            SceneManager.LoadScene(0);
        }
    }

    public void IrAlMenuPrincipal()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}