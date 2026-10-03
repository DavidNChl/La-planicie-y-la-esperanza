using UnityEngine;

public class ControladorAccionesJugador : MonoBehaviour
{
    public bool PuedeSaltar { get; private set; } = true;
    public bool PuedeMoverse { get; private set; } = true;
    public bool PuedeAtacar { get; private set; } = true;

    public void ActualizarPermisos(bool saltar, bool moverse, bool atacar)
    {
        PuedeSaltar = saltar;
        PuedeMoverse = moverse;
        PuedeAtacar = atacar;
    }
}