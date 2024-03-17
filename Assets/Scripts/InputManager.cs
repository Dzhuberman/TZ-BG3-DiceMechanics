using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputSystem InputSystem = null;

    public UnityEvent Event;

    private void Awake()
    {
        InputSystem = new InputSystem();

        InputSystem.gameplay.startDiceRoll.performed += OpenIntr;
    }

    private void OnEnable()
    {
        InputSystem.Enable();
    }

    private void OnDisable()
    {
        InputSystem.Enable();
    }

    private void OpenIntr(InputAction.CallbackContext v)
    {
        Event?.Invoke();
    }
}
