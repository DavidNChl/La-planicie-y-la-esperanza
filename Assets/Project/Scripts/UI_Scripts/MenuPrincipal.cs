using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
   
    public void Jugar()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(1); 
    }
  
    public void Salir()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit(); 
    }
}