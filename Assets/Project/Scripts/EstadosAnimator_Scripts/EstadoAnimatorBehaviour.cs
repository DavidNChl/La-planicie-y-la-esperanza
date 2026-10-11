using UnityEngine;

public class EstadoAnimatorBehaviour : StateMachineBehaviour
{
    [Header("Permisos del Estado")]
    public bool puedeSaltar = true;
    public bool puedeMoverse = true;
    public bool puedeAtacar = true;

    // Se ejecuta en el primer fotograma al entrar al estado (ej. Idle)
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ControladorAccionesJugador acciones = animator.GetComponentInParent<ControladorAccionesJugador>();
        if (acciones != null)
        {
            acciones.ActualizarPermisos(puedeSaltar, puedeMoverse, puedeAtacar);
        }
    }
}