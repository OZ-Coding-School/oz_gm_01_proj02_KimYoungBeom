using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BtnAndViewController : MonoBehaviour
{
    [Header("컴포넌트")]
    [SerializeField] private Button _restartBtn;
    [SerializeField] private Button _undoBtn;
    [SerializeField] private Button _topViewBtn;
    [SerializeField] private Button _firstViewBtn;
    [SerializeField] private TextMeshProUGUI _topText;

    [Header("이벤트 구독")]
    [SerializeField] private BoolEventCHSO _onPlayerMoving;

    private const string TOP_TEXT = "Top View (T)";
    private const string QUARTER_TEXT = "Quarter View (Q)";

    private EViewMode _currentView;
    private bool _isPlayerMoving = false;
    private void Awake()
    {
        InitBtns();
    }
    private void OnEnable()
    {
        Managers.Input.onFirstViewEvent += OnChangeFirstPersonView;
        Managers.Input.onTopViewEvent += OnChangeTopView;
        Managers.Input.onQuarterViewEvent += OnChangeQuarterView;
        Managers.Input.onReloadStageEvent += OnReloadStage;

        Managers.Camera.onViewChanged += HandleViewChanged;

        _onPlayerMoving.onEvent += OnPlayerMoving;
    }
    private void OnDisable()
    {
        Managers.Input.onFirstViewEvent -= OnChangeFirstPersonView;
        Managers.Input.onTopViewEvent -= OnChangeTopView;
        Managers.Input.onQuarterViewEvent -= OnChangeQuarterView;
        Managers.Input.onReloadStageEvent -= OnReloadStage;

        Managers.Camera.onViewChanged -= HandleViewChanged;

        _onPlayerMoving.onEvent -= OnPlayerMoving;
    }
    private void InitBtns()
    {
        _restartBtn.onClick.RemoveAllListeners();
        _undoBtn.onClick.RemoveAllListeners();
        _topViewBtn.onClick.RemoveAllListeners();
        _firstViewBtn.onClick.RemoveAllListeners();

        _restartBtn.onClick.AddListener(OnClickRestartBtn);
        _undoBtn.onClick.AddListener(OnClickUndoBtn);
        _topViewBtn.onClick.AddListener(OnTopViewBtn);
        _firstViewBtn.onClick.AddListener(OnFirstViewBtn);
    }
    private void OnPlayerMoving(bool val)
    {
        _isPlayerMoving = val;
    }
    private void OnReloadStage()
    {
        _topText.SetText(TOP_TEXT);
        _firstViewBtn.gameObject.SetActive(true);
    }

    private void HandleViewChanged(EViewMode mode)
    {
        _currentView = mode;
    }
    private void OnClickRestartBtn()
    {
        Managers.Input.ExecuteReloadStage();
    }
    private void OnClickUndoBtn()
    {
        Managers.Input.ExecuteUnDo();
    }
    private void OnTopViewBtn()
    {
        if (_currentView == EViewMode.Quarter)
        {
            Managers.Input.ExecuteTopView();
        }
        else
        {
            Managers.Input.ExecuteQuarterView();
        }
    }
    private void OnFirstViewBtn()
    {
        Managers.Input.ExecuteFirstView();
    }

    #region Input Action
    private void OnChangeTopView()
    {
        if (Managers.Camera.IsBlending) return;
        if (_isPlayerMoving) return;

        if (_currentView == EViewMode.Quarter)
        {
            _topText.SetText(QUARTER_TEXT);
            _firstViewBtn.gameObject.SetActive(false);
            _ = ChangeViewAfterEndOfFrame(EViewMode.Top);
        }
    }
    private void OnChangeFirstPersonView()
    {
        if (Managers.Camera.IsBlending) return;
        if (_isPlayerMoving) return;

        if (_currentView == EViewMode.Quarter)
        {
            _topText.SetText(QUARTER_TEXT);
            _firstViewBtn.gameObject.SetActive(false);
            _ = ChangeViewAfterEndOfFrame(EViewMode.FirstPerson);
        }
    }
    private void OnChangeQuarterView()
    {
        if (Managers.Camera.IsBlending) return;
        if (_isPlayerMoving) return;

        if (_currentView != EViewMode.Quarter)
        {
            _topText.SetText(TOP_TEXT);
            _firstViewBtn.gameObject.SetActive(true);

            _ = ChangeViewAfterEndOfFrame(EViewMode.Quarter);
        }
    }
    #endregion

    #region Helper
    private async Awaitable ChangeViewAfterEndOfFrame(EViewMode mode)
    {
        await Awaitable.WaitForSecondsAsync(0.1f, destroyCancellationToken);
        Managers.Camera.ChangeView(mode);
    }
    #endregion
}
