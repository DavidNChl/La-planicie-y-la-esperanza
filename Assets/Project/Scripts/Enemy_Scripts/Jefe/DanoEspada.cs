using UnityEngine;

public class DanoEspadaBoss : MonoBehaviour
{
    public float danoEspada = 2f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SaludJugador salud = collision.GetComponent<SaludJugador>();
            if (salud != null)
            {
                salud.RecibirDano(danoEspada);
            }
        }
    }
}