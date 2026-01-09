using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private string SavePath => Path.Combine(Application.persistentDataPath, "savegame.json");

    public int clearedStageNum = 0;
    #region LifeCycle
    private void Awake()
    {
    }
    private void OnEnable()
    {
    }
    private void OnDisable()
    {
    }
    #endregion

    #region 외부호출

    #endregion

    #region Save & Load
    [ContextMenu("Save Game")]
    public void SaveGame()
    {
        SaveData data = new SaveData();
        data.clearedStageNum = clearedStageNum;

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

        clearedStageNum = data.clearedStageNum;
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
    public int clearedStageNum;
}