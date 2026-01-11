using TMPro;
using UnityEngine;

public class StageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _turnTxt;

    private void OnEnable()
    {
        Managers.Stage.onTurnCountChange += HandleTurnCountChange;
        _turnTxt.SetText(Managers.Stage.CurrentTurnCount.ToString());
    }
    private void OnDisable()
    {
        Managers.Stage.onTurnCountChange -= HandleTurnCountChange;
    }
    private void HandleTurnCountChange()
    {
        //DOTween으로 텍스트 애니메이션

        _turnTxt.SetText(Managers.Stage.CurrentTurnCount.ToString());
    }
}
