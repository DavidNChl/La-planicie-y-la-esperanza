using UnityEngine;
using UnityEngine.EventSystems;

public class BotonOpcionUI : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
        NotificarFlecha();
    }

    public void OnSelect(BaseEventData eventData)
    {
        NotificarFlecha();
    }

    private void NotificarFlecha()
    {
        if (ControladorFlechaUI.Instancia != null)
        {
            ControladorFlechaUI.Instancia.MoverA(rectTransform);
        }
    }
    private void OnDisable()
    {
        if (ControladorFlechaUI.Instancia != null)
        {
            
        }
    }
}