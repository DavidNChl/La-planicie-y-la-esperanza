using UnityEngine;

public class JugadorAtaque : MonoBehaviour
{
    [Header("Ataque y Cadencia")]
    public GameObject prefabProyectil;
    public Transform puntoDisparo;
    public float cadenciaDisparo = 0.4f;
    
    private float tiempoSiguienteDisparo = 0f;
    private JugadorInput input;

    void Start()
    {
        input = GetComponent<JugadorInput>();
    }

    void Update()
    {
        if (input.DisparoPresionado && Time.time >= tiempoSiguienteDisparo)
        {
            Disparar();
            tiempoSiguienteDisparo = Time.time + cadenciaDisparo;
        }
    }

    private void Disparar()
    {
        if (puntoDisparo == null) return;

        if (prefabProyectil != null && puntoDisparo != null)
        {
            PoolProyectiles.Instancia.ObtenerProyectil(puntoDisparo.position, puntoDisparo.rotation);
        }
        else if (prefabProyectil != null)
        {
            Instantiate(prefabProyectil, puntoDisparo.position, puntoDisparo.rotation);
        }
    }
}