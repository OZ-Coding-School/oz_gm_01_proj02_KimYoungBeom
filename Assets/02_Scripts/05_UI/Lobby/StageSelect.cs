using UnityEngine;
using UnityEngine.UI;

public class StageSelect : MonoBehaviour
{
    [SerializeField] private Button[] _stageSelectBtns;

    private void Awake()
    {
        int count = 0;
        foreach (var btn in _stageSelectBtns)
        {
            int stageIndex = count;
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnClickStageSelect(stageIndex));
            count++;
        }
    }

    private void OnClickStageSelect(int stageNum)
    {
        Managers.Game.LoadStageScene(stageNum);
    }
}
