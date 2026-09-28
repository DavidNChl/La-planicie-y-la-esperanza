using UnityEngine;

public class EventosAnimacionEnemigo : MonoBehaviour
{
    private ControladorEnemigoIA controladorIA;

    void Start()
    {
        // Busca el script ControladorEnemigoIA en el objeto Padre
        controladorIA = GetComponentInParent<ControladorEnemigoIA>();
    }

    // Este es el método que seleccionaremos en el Animation Event
    public void EventoGolpeAtaque()
    {
        if (controladorIA != null)
        {
            controladorIA.AplicarDanoAtaque();
        }
    }
}