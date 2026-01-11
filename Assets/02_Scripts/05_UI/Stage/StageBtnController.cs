using UnityEngine;
using UnityEngine.UI;

public class StageBtnController : MonoBehaviour
{
    [SerializeField] private Button _restartBtn;
    [SerializeField] private Button _undoBtn;
    [SerializeField] private Button _topViewBtn;
    [SerializeField] private Button _firstViewBtn;

    private void Awake()
    {
        InitBtns();
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
        Managers.Input.ExecuteTopView();
    }
    private void OnFirstViewBtn()
    {
        Managers.Input.ExecuteFirstView();
    }

}
