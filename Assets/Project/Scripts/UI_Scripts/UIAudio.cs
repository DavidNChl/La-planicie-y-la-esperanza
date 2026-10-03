using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instancia { get; private set; }

    [Header("Fuentes de Audio")]
    public AudioSource fuenteMusica;
    public AudioSource fuenteSFX;

    [Header("Clips de Sonido")]
    public AudioClip musicaFondo;
    public AudioClip sfxSalto;
    public AudioClip sfxDano;
    public AudioClip sfxMuerte;

    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (musicaFondo != null)
        {
            ReproducirMusica(musicaFondo);
        }
    }

    public void ReproducirMusica(AudioClip clip)
    {
        if (fuenteMusica == null || clip == null) return;
        fuenteMusica.clip = clip;
        fuenteMusica.loop = true;
        fuenteMusica.Play();
    }

    public void ReproducirSFX(AudioClip clip)
    {
        if (fuenteSFX == null || clip == null) return;
        // PlayOneShot permite reproducir el efecto sin cortar si saltas o recibes daño varias veces seguidas
        fuenteSFX.PlayOneShot(clip);
    }
}