using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{

    [Header("Stage Repository")]
    [SerializeField] private List<NodeGraphSO> _stageRepository = new List<NodeGraphSO>();

    private LevelGenerator _generator;

    public void RegisterGenerator(LevelGenerator generator)
    {
        _generator = generator;
        int targetIndex = Managers.Game.CurrentStageIndex;
        if (targetIndex != -1)
        {
            RequestGenerate(targetIndex);
        }
    }

    public void RequestGenerate(int index)
    {
        if (_generator == null) return;
        if (index < 0 || index >= _stageRepository.Count) return;
        if (index >= _stageRepository.Count)
        {
            //모든 스테이지를 클리어 했을 때 (일단 로비로 돌아감)
            Managers.Game.LoadLobbyScene();
            return;
        }

        _generator.GenerateLevel(_stageRepository[index]);
    }
}