using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectBtn : PoolableComponent
{
    [SerializeField] private Image _btnImage;
    [SerializeField] private TextMeshProUGUI _btnText;

    public int stageNum = 1;

    public Image BtnImage => _btnImage;
    public TextMeshProUGUI BtnText => _btnText;
    public Button Button { get; private set; }
    private void Awake()
    {
        Button = GetComponent<Button>();
        Button.onClick.RemoveAllListeners();
        Button.onClick.AddListener(OnClickStageSelect);
    }
    public override void OnDespawn()
    {

    }
    public override void OnSpawn()
    {

    }
    private void OnClickStageSelect()
    {
        Managers.Game.LoadStageScene(stageNum);
    }
    public override void ReturnPool()
    {
        Managers.Pool.Despawn(poolData, this);
    }
}
