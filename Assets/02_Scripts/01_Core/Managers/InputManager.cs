using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    public event Action<Vector2> onMoveEvent;
    public event Action onUnDoEvent;
    public event Action onTopViewEvent;
    public event Action onFirstViewEvent;
    public event Action<Vector2> onLookEvent;

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
        _inputActions.Player.TopView.performed += OnTopView;
        _inputActions.Player.FirstView.performed += OnFirstView;
        _inputActions.Player.Look.performed += OnLook;
    }
    private void OnDisable()
    {
        _inputActions.Player.Move.performed -= OnMovePerformed;
        _inputActions.Player.Move.canceled -= OnMoveCanceled;
        _inputActions.Player.UnDo.performed -= OnUnDoPerformed;
        _inputActions.Player.ReLoadStage.performed -= OnReloadStage;
        _inputActions.Player.Escape.performed -= OnEscape;
        _inputActions.Player.TopView.performed -= OnTopView;
        _inputActions.Player.FirstView.performed -= OnFirstView;
        _inputActions.Player.Look.performed -= OnLook;

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
    public void ExecuteTopView()
    {
        onTopViewEvent?.Invoke();
    }
    public void ExecuteFirstView()
    {
        onFirstViewEvent?.Invoke();
    }
    private void OnLook(InputAction.CallbackContext context)
    {
        onLookEvent?.Invoke(context.ReadValue<Vector2>());
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
        //Z 키
        ExecuteUnDo();
    }
    private void OnTopView(InputAction.CallbackContext context)
    {
        //T 키
        ExecuteTopView();
    }
    private void OnFirstView(InputAction.CallbackContext context)
    {
        //F 키
        ExecuteFirstView();
    }

}
