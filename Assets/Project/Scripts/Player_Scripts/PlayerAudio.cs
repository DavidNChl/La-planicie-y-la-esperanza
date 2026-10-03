using UnityEngine;

public class AudioJugador : MonoBehaviour
{
    private SaludJugador salud;
    private JugadorMovimiento movimiento;

    private void Awake()
    {
        salud = GetComponent<SaludJugador>();
        movimiento = GetComponent<JugadorMovimiento>();
    }

    private void OnEnable()
    {
        // Suscribirse al evento de muerte 
        if (salud != null)
        {
            salud.OnMuerte.AddListener(ReproducirSonidoMuerte);
        }
    }

    private void OnDisable()
    {
        if (salud != null)
        {
            salud.OnMuerte.RemoveListener(ReproducirSonidoMuerte);
        }
    }
    //Metodos de mencion

    public void ReproducirSonidoSalto()
    {
        if (AudioManager.Instancia != null)
        {
            AudioManager.Instancia.ReproducirSFX(AudioManager.Instancia.sfxSalto);
        }
    }

    public void ReproducirSonidoDano()
    {
        if (AudioManager.Instancia != null)
        {
            AudioManager.Instancia.ReproducirSFX(AudioManager.Instancia.sfxDano);
        }
    }

    public void ReproducirSonidoMuerte()
    {
        if (AudioManager.Instancia != null)
        {
            AudioManager.Instancia.ReproducirSFX(AudioManager.Instancia.sfxMuerte);
        }
    }
}