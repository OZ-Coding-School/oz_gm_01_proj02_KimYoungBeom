using UnityEngine;
using UnityEngine.UI;

public class StageBtnController : MonoBehaviour
{
    [SerializeField] private Button _restartBtn;
    [SerializeField] private Button _undoBtn;

    private void Awake()
    {
        InitBtns();
    }
    private void InitBtns()
    {
        _restartBtn.onClick.RemoveAllListeners();
        _undoBtn.onClick.RemoveAllListeners();

        _restartBtn.onClick.AddListener(OnClickRestartBtn);
        _undoBtn.onClick.AddListener(OnClickUndoBtn);
    }
    private void OnClickRestartBtn()
    {
        Managers.Input.ExecuteReloadStage();
    }
    private void OnClickUndoBtn()
    {
        Managers.Input.ExecuteUnDo();
    }

}
