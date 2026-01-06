using UnityEngine;

[DefaultExecutionOrder(-200)]
public class Managers : MonoBehaviour
{
    public static Managers Instance { get; private set; }
    //[SerializeField] private GameObject dataManagerPrefab;
    [SerializeField] private GameObject poolManagerPrefab;
    //[SerializeField] private GameObject gameManagerPrefab;
    //[SerializeField] private GameObject stageManagerPrefab;

    //public static DataManager Data { get; private set; }
    public static PoolManager Pool { get; private set; }
    //public static GameManager Game { get; private set; }
    //public static StageManager Stage { get; private set; }
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

        if (poolManagerPrefab != null)
        {
            GameObject poolGo = Instantiate(poolManagerPrefab, transform);
            Pool = poolGo.GetComponent<PoolManager>();
        }

        //if (gameManagerPrefab != null)
        //{
        //    GameObject gameGo = Instantiate(gameManagerPrefab, transform);
        //    Game = gameGo.GetComponent<GameManager>();
        //}
        //if (stageManagerPrefab != null)
        //{
        //    GameObject stageGo = Instantiate(stageManagerPrefab, transform);
        //    Stage = stageGo.GetComponent<StageManager>();
        //}
        //if (playerStatsManagerPrefab != null)
        //{
        //    GameObject statsGo = Instantiate(playerStatsManagerPrefab, transform);
        //}
        //if (Data != null)
        //{
        //    Data.LoadGame();
        //}

    }

}