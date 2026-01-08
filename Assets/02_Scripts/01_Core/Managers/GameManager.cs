using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private VoidEventCHSO _onGamePause;
    [SerializeField] private VoidEventCHSO _onGameResume;
    [SerializeField] private VoidEventCHSO _onSceneChanged;

    private bool bIsPause = false;
    private bool bIsGameOver = false;
    public bool CanPause { get; set; } = false;
    public int CurrentStageIndex { get; private set; } = -1;
    private void Start()
    {
        //LoadLobbyScene();
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleOnSceneLoad;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleOnSceneLoad;
    }
    public void HandleOnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        _onSceneChanged.Raised();
    }
    public void LoadStageScene(int stageIndex)
    {
        CurrentStageIndex = stageIndex;
        Time.timeScale = 1.0f;
        bIsPause = false;
        SceneManager.LoadScene(Defines.SCENE_STAGE);
    }
    public void LoadLobbyScene()
    {
        Managers.Pool.DespawnAll();
        CurrentStageIndex = -1;
        Time.timeScale = 1.0f;
        bIsPause = false;
        SceneManager.LoadScene(Defines.SCENE_LOBBY);
    }
    public void TogglePause()
    {
        if (!CanPause) return;

        if (bIsGameOver)
        {
            LoadStageScene(CurrentStageIndex);
            return;
        }
        bIsPause = !bIsPause;
        if (bIsPause)
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
        CurrentStageIndex++;

        //하나 커진 인덱스를 DataManager에 저장하여 완료한 스테이지 기록

        Managers.Stage.RequestGenerate(CurrentStageIndex);
    }
}