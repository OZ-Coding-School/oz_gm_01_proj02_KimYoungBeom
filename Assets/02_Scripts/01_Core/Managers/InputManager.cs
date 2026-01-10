using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
        _inputActions.Player.ReLoadStage.performed += OnReloadStage;
        _inputActions.Player.Escape.performed += OnEscape;
    }
    private void OnDisable()
    {
        _inputActions.Player.Move.performed -= OnMovePerformed;
        _inputActions.Player.Move.canceled -= OnMoveCanceled;
        _inputActions.Player.UnDo.performed -= OnUnDoPerformed;
        _inputActions.Player.ReLoadStage.performed -= OnReloadStage;
        _inputActions.Player.Escape.performed -= OnEscape;

        _inputActions.Disable();
    }
    private void OnDestroy()
    {
        _inputActions.Dispose();
    }
    public void ExecuteReloadStage()
    {
        Managers.Stage.RequestGenerate(Managers.Game.CurrentStageIndex, false);
    }
    public void ExecuteEscape()
    {
        Managers.Game.LoadLobbyScene();
    }
    public void ExecuteUnDo()
    {
        onUnDoEvent?.Invoke();
    }
    private void OnReloadStage(InputAction.CallbackContext context)
    {
        //재시작 키 'R'
        ExecuteReloadStage();
    }
    private void OnEscape(InputAction.CallbackContext context)
    {
        //편의상 로비로
        ExecuteEscape();
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
        ExecuteUnDo();
    }

}
