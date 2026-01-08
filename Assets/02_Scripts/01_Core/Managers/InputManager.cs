using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public event Action<Vector2> onMoveEvent;
    public event Action onUnDoEvent;

    private InputSystem_Actions _inputActions;

    private void Awake()
    {
        _inputActions = new InputSystem_Actions();
    }
    private void OnEnable()
    {
        _inputActions.Enable();

        _inputActions.Player.Move.performed += OnMovePerformed;
        _inputActions.Player.Move.canceled += OnMoveCanceled;
        _inputActions.Player.UnDo.performed += OnUnDoPerformed;
    }
    private void OnDisable()
    {
        _inputActions.Player.Move.performed -= OnMovePerformed;
        _inputActions.Player.Move.canceled -= OnMoveCanceled;
        _inputActions.Player.UnDo.performed -= OnUnDoPerformed;

        _inputActions.Disable();
    }
    private void OnDestroy()
    {
        _inputActions.Dispose();
    }
    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        onMoveEvent?.Invoke(context.ReadValue<Vector2>());
    }
    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        onMoveEvent?.Invoke(Vector2.zero);
    }
    private void OnUnDoPerformed(InputAction.CallbackContext context)
    {
        onUnDoEvent?.Invoke();
    }
}
