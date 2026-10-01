using System.Collections.Generic;
using UnityEngine;

public class PoolProyectiles : MonoBehaviour
{
    public static PoolProyectiles Instancia { get; private set; }

    [Header("Configuración de la Pool")]
    public GameObject prefabProyectil;
    public int tamanoInicialPool = 15;

    private List<GameObject> listaPool = new List<GameObject>();

    private void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;

        InicializarPool();
    }

    private void InicializarPool()
    {
        for (int i = 0; i < tamanoInicialPool; i++)
        {
            GameObject obj = Instantiate(prefabProyectil, transform);
            obj.SetActive(false);
            listaPool.Add(obj);
        }
    }

    public GameObject ObtenerProyectil(Vector3 posicion, Quaternion rotacion)
    {
        // 1. Buscar uno desactivado en la lista
        for (int i = 0; i < listaPool.Count; i++)
        {
            if (!listaPool[i].activeInHierarchy)
            {
                listaPool[i].transform.position = posicion;
                listaPool[i].transform.rotation = rotacion;
                listaPool[i].SetActive(true);
                return listaPool[i];
            }
        }

        // 2. Si la pool se queda corta, instanciar uno nuevo de apoyo
        GameObject nuevoObj = Instantiate(prefabProyectil, posicion, rotacion, transform);
        listaPool.Add(nuevoObj);
        return nuevoObj;
    }
}