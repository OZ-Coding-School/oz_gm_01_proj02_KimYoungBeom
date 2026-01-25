using System;
using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private string SavePath => Path.Combine(Application.persistentDataPath, "savegame.json");


    public event Action onCamSensitivityChange;

    public int ClearedStageNum { get; private set; } = 0;
    public float CamSensitivity { get; private set; } = 3.0f; //1~5 사이 Defines -> CAM_SEN

    #region LifeCycle
    private void Awake()
    {
    }
    private void Start()
    {
        onCamSensitivityChange?.Invoke();
    }
    private void OnEnable()
    {
    }
    private void OnDisable()
    {
    }
    #endregion

    #region 외부호출
    public void SetBestClearedStageNum(int clearedStageNum)
    {
        if (clearedStageNum > ClearedStageNum)
        {
            ClearedStageNum = clearedStageNum;
        }
    }
    public void SetSensitivity(float value)
    {
        CamSensitivity = Mathf.Clamp(value, Defines.CAM_SENS_MIN, Defines.CAM_SENS_MAX);
        onCamSensitivityChange?.Invoke();
    }
    #endregion

    #region Save & Load
    [ContextMenu("Save Game")]
    public void SaveGame()
    {
        SaveData data = new SaveData();
        data.clearedStageNum = ClearedStageNum;
        data.camSensitivity = CamSensitivity;

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
    }

    public void LoadGame()
    {

        if (!File.Exists(SavePath))
        {
            return;
        }

        string json = File.ReadAllText(SavePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        ClearedStageNum = data.clearedStageNum;
        CamSensitivity = data.camSensitivity;
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
    #endregion

}

[System.Serializable]
public class SaveData
{
    public int clearedStageNum = 1;
    public float camSensitivity = 3.0f;
}