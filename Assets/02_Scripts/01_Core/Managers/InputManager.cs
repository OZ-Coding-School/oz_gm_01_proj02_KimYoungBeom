using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public event Action<Vector2> onMoveEvent;
    public event Action onReloadStageEvent;
    public event Action onUnDoEvent;
    public event Action onTopViewEvent;
    public event Action onFirstViewEvent;
    public event Action onQuarterViewEvent;
    public event Action<Vector2> onLookEvent;

    private InputSystem_Actions _inputActions;

    public bool IsPlayerDeath { get; set; }

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
        _inputActions.Player.QuarterView.performed += OnQuarterView;
        _inputActions.Player.Attack.performed += OnClickAtFPView;
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
        _inputActions.Player.QuarterView.performed -= OnQuarterView;
        _inputActions.Player.Attack.performed -= OnClickAtFPView;

        _inputActions.Disable();
    }
    private void OnDestroy()
    {
        _inputActions.Dispose();
    }
    public void ExecuteReloadStage()
    {
        Managers.Stage.RequestGenerate(Managers.Game.CurrentStageIndex, false);
        onReloadStageEvent?.Invoke();
    }
    public void ExecuteEscape()
    {
        Managers.Game.LoadLobbyScene();
    }
    public void ExecuteUnDo()
    {
        if (IsPlayerDeath) return;
        onUnDoEvent?.Invoke();
    }
    public void ExecuteTopView()
    {
        if (IsPlayerDeath || Managers.Stage.IsStageTurn) return;
        onTopViewEvent?.Invoke();
    }
    public void ExecuteFirstView()
    {
        if (IsPlayerDeath || Managers.Stage.IsStageTurn) return;
        onFirstViewEvent?.Invoke();
    }
    public void ExecuteQuarterView()
    {
        if (IsPlayerDeath || Managers.Stage.IsStageTurn) return;
        onQuarterViewEvent?.Invoke();
    }
    private void ExecuteClickAtFPView()
    {
        if (Managers.Camera.CurrentViewMode != EViewMode.FirstPerson) return;
        Managers.Camera.ToggleMouseActive();
    }
    private void OnClickAtFPView(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        Vector2 pointerPosition = Pointer.current.position.ReadValue();
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = pointerPosition
        };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        if (results.Count > 0) return;

        ExecuteClickAtFPView();
    }
    private void OnQuarterView(InputAction.CallbackContext context)
    {
        // Q 
        ExecuteQuarterView();
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
        //esc 편의상 로비로
        ExecuteEscape();
    }
    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        if (IsPlayerDeath) return;
        onMoveEvent?.Invoke(context.ReadValue<Vector2>());
    }
    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        if (IsPlayerDeath) return;
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
