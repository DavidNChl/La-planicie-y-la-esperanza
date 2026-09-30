using UnityEngine;
using UnityEngine.InputSystem;

public class JugadorInput : MonoBehaviour
{
    public float MovimientoH { get; private set; }
    public bool SaltoPresionado { get; private set; }
    public bool DisparoPresionado { get; private set; }

    void Update()
    {
        // Movimiento Horizontal
        MovimientoH = 0f;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                MovimientoH = -1f;
            else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                MovimientoH = 1f;
        }

        // Salto
        SaltoPresionado = Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;

        // Disparo
        bool teclaDisparo = Keyboard.current != null && Keyboard.current.jKey.wasPressedThisFrame;
        bool clickDisparo = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        DisparoPresionado = teclaDisparo || clickDisparo;
    }
}