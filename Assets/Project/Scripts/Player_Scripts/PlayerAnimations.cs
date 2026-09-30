using UnityEngine;

public class JugadorAnimaciones : MonoBehaviour
{
    [Header("Tiempo de Inactividad / Descanso")]
    public float tiempoParaDescansar = 3f;
    private float contadorInactividad = 0f;
    private bool estaDescansando = false;

    private Animator animator;
    private JugadorInput input;
    private JugadorMovimiento movimiento;

    void Start()
    {
        animator = GetComponent<Animator>();
        input = GetComponent<JugadorInput>();
        movimiento = GetComponent<JugadorMovimiento>();
    }

    void Update()
    {
        if (animator == null) return;

        // Actualizar caminata
        animator.SetBool("camina", input.MovimientoH != 0f);

        // Control de descanso e inactividad
        bool estaHaciendoAlgo = input.MovimientoH != 0f || input.SaltoPresionado || input.DisparoPresionado || !movimiento.EnSuelo;

        if (!estaHaciendoAlgo)
        {
            contadorInactividad += Time.deltaTime;

            if (contadorInactividad >= tiempoParaDescansar && !estaDescansando)
            {
                estaDescansando = true;
                animator.SetTrigger("descansar");
            }
        }
        else
        {
            contadorInactividad = 0f;
            estaDescansando = false;
        }
    }
}