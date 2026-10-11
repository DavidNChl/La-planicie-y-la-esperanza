using UnityEngine;

public class Autodestruir : MonoBehaviour
{
    public float tiempoVida = 1f;

    void Start()
    {
        Destroy(gameObject, tiempoVida);
    }
}