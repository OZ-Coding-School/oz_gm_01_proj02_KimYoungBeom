using DG.Tweening;
using UnityEngine;

public class Piece_Button : Piece_Base
{
    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private SpatialNodeEventCHSO _onNotifyFromNode;    //PlayerController น฿วเ

    private static readonly int ColorProperty = Shader.PropertyToID("_BaseColor");
    private static MaterialPropertyBlock _defaultBlock;
    private static MaterialPropertyBlock _activeBlock;

    private SpatialNode _prevPlayerNode;

    private void Awake()
    {
        if (_defaultBlock == null)
        {
            _defaultBlock = new MaterialPropertyBlock();
            _renderer.GetPropertyBlock(_defaultBlock);
            _defaultBlock.SetColor(ColorProperty, Color.white);
        }
        if (_activeBlock == null)
        {
            _activeBlock = new MaterialPropertyBlock();
            _renderer.GetPropertyBlock(_activeBlock);
            _activeBlock.SetColor(ColorProperty, Color.green);
        }
    }
    public override void OnSpawn()
    {
        base.OnSpawn();
        _prevPlayerNode = null;
        _renderer.SetPropertyBlock(_defaultBlock);
        var localPos = _renderer.transform.localPosition;
        localPos.y = 0;
        _renderer.transform.localPosition = localPos;
        _onNotifyFromNode.onEvent += RegisterFromNode;
    }
    public override void OnDespawn()
    {
        base.OnDespawn();
        _onNotifyFromNode.onEvent -= RegisterFromNode;
    }
    public override void ReturnPool()
    {
        _renderer.SetPropertyBlock(_defaultBlock);
        Managers.Pool.Despawn(poolData, this);
    }

    protected override void HandleIntroEnd()
    {
    }

    protected override void HandleNotify(SpatialNode node)
    {
        if (node != GroundNode)
        {
            if (_prevPlayerNode != GroundNode) return;
            _renderer.transform.DOLocalMoveY(0.0f, 0.2f)
                .SetEase(Ease.OutQuad);
            _renderer.SetPropertyBlock(_defaultBlock);
            Managers.Stage.DeactiveButton();
        }
        else
        {
            _renderer.transform.DOLocalMoveY(-0.1f, 0.1f)
                .SetEase(Ease.InOutSine)
                .OnComplete(() =>
                {
                    _renderer.SetPropertyBlock(_activeBlock);
                    Managers.Stage.GetKeyRequest();
                });
        }
    }

    private void RegisterFromNode(SpatialNode node)
    {
        _prevPlayerNode = node;
    }
}
