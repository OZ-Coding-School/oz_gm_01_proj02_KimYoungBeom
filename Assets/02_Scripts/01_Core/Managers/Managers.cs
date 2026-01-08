using UnityEngine;

[DefaultExecutionOrder(-200)]
public class Managers : MonoBehaviour
{
    public static Managers Instance { get; private set; }
    //[SerializeField] private GameObject dataManagerPrefab;
    [SerializeField] private GameObject _poolManagerPrefab;
    [SerializeField] private GameObject _gameManagerPrefab;
    [SerializeField] private GameObject _stageManagerPrefab;
    [SerializeField] private GameObject _inputManagerPrefab;


    //public static DataManager Data { get; private set; }
    public static PoolManager Pool { get; private set; }
    public static GameManager Game { get; private set; }
    public static StageManager Stage { get; private set; }
    public static InputManager Input { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        //if (dataManagerPrefab != null)
        //{
        //    GameObject dataGo = Instantiate(dataManagerPrefab, transform);
        //    Data = dataGo.GetComponent<DataManager>();
        //}

        if (_inputManagerPrefab != null)
        {
            GameObject inputGo = Instantiate(_inputManagerPrefab, transform);
            Input = inputGo.GetComponent<InputManager>();
        }
        if (_poolManagerPrefab != null)
        {
            GameObject poolGo = Instantiate(_poolManagerPrefab, transform);
            Pool = poolGo.GetComponent<PoolManager>();
        }
        if (_gameManagerPrefab != null)
        {
            GameObject gameGo = Instantiate(_gameManagerPrefab, transform);
            Game = gameGo.GetComponent<GameManager>();
        }
        if (_stageManagerPrefab != null)
        {
            GameObject stageGo = Instantiate(_stageManagerPrefab, transform);
            Stage = stageGo.GetComponent<StageManager>();
        }
        //if (Data != null)
        //{
        //    Data.LoadGame();
        //}

    }

}