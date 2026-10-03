using UnityEngine;
using UnityEngine.UI;

public class ControladorFlechaUI : MonoBehaviour
{
    public static ControladorFlechaUI Instancia { get; private set; }

    [Header("Configuración")]
    public RectTransform rectFlecha; 
    public float offsetHorizontal = -30f; 
    public float velocidadMovimiento = 15f; 

    private Vector3 posicionObjetivo;
    private bool flechaVisible = false;

    private void Awake()
    {
        Instancia = this;
    }

    private void Start()
    {
        if (rectFlecha != null)
        {
            rectFlecha.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (flechaVisible && rectFlecha != null)
        {
            rectFlecha.position = Vector3.Lerp(rectFlecha.position, posicionObjetivo, Time.unscaledDeltaTime * velocidadMovimiento);
        }
    }

    public void MoverA(RectTransform objetivo)
    {
        if (rectFlecha == null || objetivo == null) return;

        if (!flechaVisible)
        {
            flechaVisible = true;
            rectFlecha.gameObject.SetActive(true);
            Vector3 posInicial = objetivo.position;
            posInicial.x += offsetHorizontal;
            rectFlecha.position = posInicial;
        }

        Vector3 nuevaPosicion = objetivo.position;
        nuevaPosicion.x += offsetHorizontal;
        posicionObjetivo = nuevaPosicion;
    }

    public void Ocultar()
    {
        flechaVisible = false;
        if (rectFlecha != null)
        {
            rectFlecha.gameObject.SetActive(false);
        }
    }
    
}