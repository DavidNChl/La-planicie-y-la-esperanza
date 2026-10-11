using UnityEngine;
using System.Collections;

public class GeneradorEnemigosAgua : MonoBehaviour
{
    [Header("Enemigos a Teletransportar")]
    public GameObject enemigoBase;
    public GameObject enemigoVeneno;
    public GameObject enemigoFuego;

    [Header("Configuración de Aparición")]
    public Vector2[] offsetsAparicion = new Vector2[] 
    { 
        new Vector2(-3f, 2f),  // Enemigo 1: Arriba a la izquierda
        new Vector2(3f, 2f),   // Enemigo 2: Arriba a la derecha
        new Vector2(0f, 4f)    // Enemigo 3: Directamente arriba
    };

    public bool reproducirEfectoParticulas = true;
    public GameObject prefabEfectoTeleport; // Opcional: VFX de aparición/humo

    private bool yaInvocados = false; // Evita invocar múltiples veces si el jugador entra/sale seguido

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (yaInvocados) return;

        if (collision.CompareTag("Player"))
        {
            InvocacionEnemigos(collision.transform);
            yaInvocados = true;
        }
    }

    public void InvocacionEnemigos(Transform jugador)
    {
        GameObject[] listaEnemigos = new GameObject[] { enemigoBase, enemigoVeneno, enemigoFuego };

        for (int i = 0; i < listaEnemigos.Length; i++)
        {
            if (listaEnemigos[i] == null) continue;

            // Calcular posición de teletransporte relativa al jugador
            Vector2 posicionDestino = (Vector2)jugador.position + offsetsAparicion[i % offsetsAparicion.Length];

            // 1. Teletransportar y activar el enemigo
            listaEnemigos[i].transform.position = posicionDestino;
            listaEnemigos[i].SetActive(true);

            // 2. Asignar el objetivo directamente en su IA
            AsignarObjetivoIA(listaEnemigos[i], jugador);

            // 3. Efecto visual opcional en la posición de llegada
            if (prefabEfectoTeleport != null)
            {
                GameObject vfx = Instantiate(prefabEfectoTeleport, posicionDestino, Quaternion.identity);
                Destroy(vfx, 1.5f);
            }
        }
    }

    // Opcional: Reiniciar la trampa si el jugador sale del área
    public void ResetearInvocacion()
    {
        yaInvocados = false;
    }
    private void AsignarObjetivoIA(GameObject enemigo, Transform jugador)
    {
        // Asignar objetivo al componente de teletransporte e inicie el TP al suelo
        TeleportEnemigo tp = enemigo.GetComponent<TeleportEnemigo>();
        if (tp != null)
        {
            tp.AsignarObjetivo(jugador);
            tp.EjecutarTeletransporte();
        }

        // Mantener la referencia del jugador en los scripts de IA
        EnemyAI aiBase = enemigo.GetComponent<EnemyAI>();
        if (aiBase != null) aiBase.playerTarget = jugador;

        EnemyVenomAI aiVeneno = enemigo.GetComponent<EnemyVenomAI>();
        if (aiVeneno != null) aiVeneno.playerTarget = jugador;

        EnemyFireAI aiFuego = enemigo.GetComponent<EnemyFireAI>();
        if (aiFuego != null) aiFuego.playerTarget = jugador;
    }
}