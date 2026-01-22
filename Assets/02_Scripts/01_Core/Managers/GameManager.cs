using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("이벤트 발송")]
    [SerializeField] private VoidEventCHSO _onGamePause;
    [SerializeField] private VoidEventCHSO _onGameResume;
    [SerializeField] private VoidEventCHSO _onSceneChanged;
    [Header("이벤트 구독")]
    [SerializeField] private VoidEventCHSO _onStageClear;

    private bool _isPause = false;
    private bool _isGameOver = false;
    private bool _isProcessing = false;
    public bool CanPause { get; set; } = false;
    public int CurrentStageIndex { get; private set; } = -1;
    private void Start()
    {
        //LoadLobbyScene();
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleOnSceneLoad;
        _onStageClear.onEvent += HandleStageClear;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleOnSceneLoad;
        _onStageClear.onEvent -= HandleStageClear;
    }
    public void HandleOnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        _onSceneChanged.Raised();
    }
    public void LoadStageScene(int stageIndex)
    {
        DG.Tweening.DOTween.KillAll();

        CurrentStageIndex = stageIndex;
        Time.timeScale = 1.0f;
        _isPause = false;
        SceneManager.LoadScene(Defines.SCENE_STAGE);
    }
    public void LoadLobbyScene()
    {
        DG.Tweening.DOTween.KillAll();

        Managers.Pool.DespawnAll();
        CurrentStageIndex = -1;
        Time.timeScale = 1.0f;
        _isPause = false;

        Managers.Camera.ChangeView(EViewMode.Lobby);

        SceneManager.LoadScene(Defines.SCENE_LOBBY);
    }
    public void TogglePause()
    {
        if (!CanPause) return;

        if (_isGameOver)
        {
            LoadStageScene(CurrentStageIndex);
            return;
        }
        _isPause = !_isPause;
        if (_isPause)
        {
            Time.timeScale = 0.0f;
            _onGamePause.Raised();
        }
        else
        {
            Time.timeScale = 1.0f;
            _onGameResume.Raised();
        }
    }
    public void HandleStageClear()
    {
        if (_isProcessing) return;
        _isProcessing = true;
        ProcessStageClearAsync();
    }

    private async void ProcessStageClearAsync()
    {
        CurrentStageIndex++;
        Managers.Data.SetBestClearedStageNum(CurrentStageIndex);
        Managers.Stage.RequestGenerate(CurrentStageIndex);
        await Awaitable.NextFrameAsync();
        _isProcessing = false;
    }
}