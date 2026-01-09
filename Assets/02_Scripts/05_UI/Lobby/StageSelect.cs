using System.Collections.Generic;
using UnityEngine;

public class StageSelect : MonoBehaviour
{
    [SerializeField] private PoolableObjSO _stageSelectBtn;
    [SerializeField] private Transform _btnRoot;
    [SerializeField] private int _totalStageNum = 10;

    private float _notClearedBtnAlpha = 100.0f / 255.0f;
    private readonly List<StageSelectBtn> _btnList = new List<StageSelectBtn>();

    private void OnEnable()
    {
        GenerateBtns();
    }
    private void OnDisable()
    {
        if (Managers.Pool != null && _btnList.Count > 0)
        {
            foreach (StageSelectBtn btn in _btnList)
            {
                btn.transform.SetParent(Managers.Pool.transform, false);
                btn.ReturnPool();
            }
        }
        _btnList.Clear();
    }
    private void GenerateBtns()
    {
        for (int i = 0; i < _totalStageNum; i++)
        {
            StageSelectBtn btn = Managers.Pool.Spawn<StageSelectBtn>(_stageSelectBtn, Vector3.zero);
            _btnList.Add(btn);
            btn.transform.SetParent(_btnRoot, false);
            btn.BtnText.SetText((i + 1).ToString());

            btn.stageNum = i;

            var color = btn.BtnImage.color;
            if (i > Managers.Data.clearedStageNum) color.a = _notClearedBtnAlpha;
            else color.a = 1.0f;

            btn.BtnImage.color = color;
        }
    }
}
