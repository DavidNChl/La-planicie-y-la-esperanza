using UnityEngine;

public class AutoDestruir : MonoBehaviour
{
    public float tiempoVida = 0.4f; 

    void Start()
    {
        Destroy(gameObject, tiempoVida);
    }
}