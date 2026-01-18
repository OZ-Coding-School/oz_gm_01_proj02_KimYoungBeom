using DG.Tweening;
using UnityEngine;

public class Piece_Goal : Piece_Base
{
    private MeshRenderer[] _piecesRends;
    private Vector3[] _piecesOriginLocalPos;

    private float _bombMoveRange = 0.1f;
    private bool _isSpread = false;

    private void Awake()
    {
        _piecesRends = GetComponentsInChildren<MeshRenderer>();
        _piecesOriginLocalPos = new Vector3[_piecesRends.Length];
        for (int i = 0; i < _piecesRends.Length; i++)
        {
            _piecesOriginLocalPos[i] = _piecesRends[i].transform.localPosition;
        }
    }
    private void OnEnable()
    {
        Managers.Stage.onGetAllKeys += HandleGetAllKeys;

    }
    private void OnDisable()
    {
        Managers.Stage.onGetAllKeys -= HandleGetAllKeys;
    }
    public override void OnSpawn()
    {
        base.OnSpawn();
        ResetPieces();
        _isSpread = false;
        UpDownIdle();
    }
    public override void OnDespawn()
    {
        base.OnDespawn();

        KillTweensAndReset();
    }
    private void FixedUpdate()
    {
        if (!_isSpread) RotateToCamera();
    }

    //이벤트 핸들러
    protected override void HandleNotify(SpatialNode node)
    {
        if (node != GroundNode) return;
        if (_isSpread) return;

        Managers.Stage.StageClearRequest();

        ReturnPool();
    }
    protected override void HandleIntroEnd()
    {
        if (Managers.Stage.RemainingKeyCount > 0)
        {
            SpreadAllDirection();
        }
    }
    private void HandleGetAllKeys()
    {
        DeSpreadAllDirection();
    }
    private void SpreadAllDirection()
    {
        _isSpread = true;

        Vector3[] meshCenter = new Vector3[_piecesRends.Length];
        for (int i = 0; i < _piecesRends.Length; i++)
        {
            Transform t = _piecesRends[i].transform;
            Vector3 moveDir = (_piecesRends[i].GetWorldCenter() - t.position).normalized;

            Vector3 targetLocalPos = _piecesOriginLocalPos[i] + t.InverseTransformDirection(moveDir * _bombMoveRange + Vector3.up);
            t.DOLocalMove(targetLocalPos, 1.0f).SetEase(Ease.OutBack);
        }
    }
    private void DeSpreadAllDirection()
    {
        _isSpread = false;

        for (int i = 0; i < _piecesRends.Length; ++i)
        {
            _piecesRends[i].transform.DOLocalMove(_piecesOriginLocalPos[i], 1.0f)
                .SetEase(Ease.InBack);
        }
    }
    private void KillTweensAndReset()
    {
        foreach (var rend in _piecesRends)
        {
            rend.transform.DOKill();
        }
        _upDownTween?.Kill();

        ResetPieces();
    }
    private void ResetPieces()
    {
        for (int i = 0; i < _piecesRends.Length; i++)
        {
            _piecesRends[i].transform.localPosition = _piecesOriginLocalPos[i];
        }
    }

    private void UpDownIdle()
    {
        _upDownTween?.Kill();
        _upDownTween = transform.DOMoveY(_bounceAmplitude, _rotateHalfDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetRelative();
    }

    public override void ReturnPool()
    {
        Managers.Pool.Despawn(poolData, this);
    }
}
