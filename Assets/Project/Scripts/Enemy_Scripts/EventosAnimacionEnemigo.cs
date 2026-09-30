using UnityEngine;

public class EventosAnimacionEnemigo : MonoBehaviour
{
    private EnemigoBrainIA cerebroIA;

    void Start()
    {
        cerebroIA = GetComponentInParent<EnemigoBrainIA>();
    }

    public void EventoGolpeAtaque()
    {
        if (cerebroIA != null)
        {
            cerebroIA.AplicarDanoAtaque();
        }
    }
}