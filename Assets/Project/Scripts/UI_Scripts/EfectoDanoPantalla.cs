using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EfectoDanoPantalla : MonoBehaviour
{
    public static EfectoDanoPantalla Instancia { get; private set; }

    [Header("Configuración del Efecto")]
    public Image imagenDano;
    public float duracionFlash = 0.3f;
    [Range(0f, 1f)] public float alfaMaximo = 0.4f; // Intensidad del rojo (0.4 = 40% de opacidad)

    private Coroutine corrutinaEfecto;

    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;
    }

    public void MostrarEfectoDano()
    {
        if (imagenDano == null) return;

        if (corrutinaEfecto != null)
        {
            StopCoroutine(corrutinaEfecto);
        }

        corrutinaEfecto = StartCoroutine(AnimarFlashRojo());
    }

    private IEnumerator AnimarFlashRojo()
    {
        float tiempo = 0f;
        Color colorOriginal = imagenDano.color;

        while (tiempo < duracionFlash / 2f)
        {
            tiempo += Time.deltaTime;
            float alfa = Mathf.Lerp(0f, alfaMaximo, tiempo / (duracionFlash / 2f));
            imagenDano.color = new Color(colorOriginal.r, colorOriginal.g, colorOriginal.b, alfa);
            yield return null;
        }

        tiempo = 0f;

        while (tiempo < duracionFlash / 2f)
        {
            tiempo += Time.deltaTime;
            float alfa = Mathf.Lerp(alfaMaximo, 0f, tiempo / (duracionFlash / 2f));
            imagenDano.color = new Color(colorOriginal.r, colorOriginal.g, colorOriginal.b, alfa);
            yield return null;
        }

        imagenDano.color = new Color(colorOriginal.r, colorOriginal.g, colorOriginal.b, 0f);
    }
}