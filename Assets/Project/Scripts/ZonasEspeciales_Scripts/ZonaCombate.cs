using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ZonaCombate : MonoBehaviour
{
    [System.Serializable]
    public struct ConfigEnemigo
    {
        public GameObject prefabEnemigo;
        public int cantidad;             
    }

    [Header("Configuración de Enemigos")]
    public List<ConfigEnemigo> listaEnemigos; 
    public Transform[] puntosSpawn;         
    public float tiempoEntreSpawns = 0.5f;

    [Header("Bloqueo de Zona")]
    public GameObject[] barrerasBloqueo;     

    [Header("Condición de Victoria")]
    [Tooltip("Marca esta casilla únicamente en la Zona de Combate de la batalla final con el Jefe")]
    public bool esZonaBoss = false;

    private int enemigosRestantes = 0;
    private bool zonaActivada = false;
    private Transform playerTarget;

    private void Start()
    {
        ActivarBarreras(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!zonaActivada && collision.CompareTag("Player"))
        {
            playerTarget = collision.transform;
            zonaActivada = true;
            
            GetComponent<Collider2D>().enabled = false;

            ActivarBarreras(true);

            StartCoroutine(GenerarEnemigos());
        }
    }

    private IEnumerator GenerarEnemigos()
    {
        enemigosRestantes = 0;
        foreach (var config in listaEnemigos)
        {
            enemigosRestantes += config.cantidad;
        }

        foreach (var config in listaEnemigos)
        {
            for (int i = 0; i < config.cantidad; i++)
            {
                if (puntosSpawn.Length == 0) yield break;

                Transform puntoElegido = puntosSpawn[Random.Range(0, puntosSpawn.Length)];

                GameObject enemigoInstanciado = Instantiate(config.prefabEnemigo, puntoElegido.position, Quaternion.identity);

                AsignarObjetivo(enemigoInstanciado);

                ControladorMuerteEnemigo monitor = enemigoInstanciado.AddComponent<ControladorMuerteEnemigo>();
                monitor.Inicializar(this);

                yield return new WaitForSeconds(tiempoEntreSpawns);
            }
        }
    }

    private void AsignarObjetivo(GameObject enemigo)
    {
        EnemyAI aiBase = enemigo.GetComponent<EnemyAI>();
        if (aiBase != null) aiBase.playerTarget = playerTarget;

        EnemyVenomAI aiVeneno = enemigo.GetComponent<EnemyVenomAI>();
        if (aiVeneno != null) aiVeneno.playerTarget = playerTarget;

        EnemyFireAI aiFuego = enemigo.GetComponent<EnemyFireAI>();
        if (aiFuego != null) aiFuego.playerTarget = playerTarget;

        BossAI aiBoss = enemigo.GetComponent<BossAI>();
        if (aiBoss != null) aiBoss.AsignarObjetivo(playerTarget);

        enemigo.SendMessage("AsignarObjetivo", playerTarget, SendMessageOptions.DontRequireReceiver);

        if (playerTarget != null)
        {
            Collider2D colJugador = playerTarget.GetComponent<Collider2D>();
            Collider2D colEnemigo = enemigo.GetComponent<Collider2D>();

            if (colJugador != null && colEnemigo != null)
            {
                Physics2D.IgnoreCollision(colEnemigo, colJugador, true);
            }
        }

        TeleportEnemigo tp = enemigo.GetComponent<TeleportEnemigo>();
        if (tp != null)
        {
            tp.AsignarObjetivo(playerTarget);
            tp.EjecutarTeletransporte();
        }
    }

    public void EnemigoDerrotado()
    {
        enemigosRestantes--;

        if (enemigosRestantes <= 0)
        {
            DespejarZona();
        }
    }

    private void DespejarZona()
    {
        ActivarBarreras(false);

        // Si esta zona es la del Boss final, activa la UI de Victoria
        if (esZonaBoss && GameManager.Instancia != null)
        {
            GameManager.Instancia.ActivarVictoria();
        }
    }

    private void ActivarBarreras(bool activar)
    {
        foreach (GameObject barrera in barrerasBloqueo)
        {
            if (barrera != null) barrera.SetActive(activar);
        }
    }
}

// Script auxiliar de detección[cite: 1]
public class ControladorMuerteEnemigo : MonoBehaviour
{
    private ZonaCombate zonaPadre;

    public void Inicializar(ZonaCombate zona)
    {
        zonaPadre = zona;
    }

    private void OnDestroy()
    {
        if (zonaPadre != null)
        {
            zonaPadre.EnemigoDerrotado();
        }
    }
}