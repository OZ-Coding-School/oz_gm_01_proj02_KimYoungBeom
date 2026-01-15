using DG.Tweening;
using UnityEngine;

public class Piece_Goal : Piece_Base
{
    [SerializeField] private float _rotateHalfDuration = 1.0f;
    [SerializeField] private float _bounceAmplitude = 0.2f;

    private MeshRenderer[] _piecesRend;    //본인 포함 7개 0 ~ 6
    private float _rotateSpeed = 15.0f;
    private Tween _tween;

    private void Awake()
    {
        _piecesRend = GetComponentsInChildren<MeshRenderer>();
    }
    public override void OnSpawn()
    {
        base.OnSpawn();
        foreach (var piece in _piecesRend)
        {
            Utils.Log($"{piece.bounds.center}");
        }
        UpDownIdle();
    }
    public override void OnDespawn()
    {
        base.OnDespawn();
        if (_tween != null)
        {
            _tween.Kill();
        }
    }
    private void FixedUpdate()
    {
        RotateToCamera();
    }
    public void ReturnActionBomb()
    {
        Vector3[] dir = new Vector3[_piecesRend.Length];
        int index = 0;
        foreach (var rend in _piecesRend)
        {
            dir[index] = rend.GetWorldCenter();
            rend.gameObject.transform.DOMove(dir[index], 1.0f);
            index++;
        }
    }
    public override void ReturnPool()
    {
        Managers.Pool.Despawn(poolData, this);
    }

    protected override void HandleNotify(SpatialNode node)
    {
        if (node != GroundNode) return;

        Managers.Stage.StageClearRequest();

        ReturnPool();
    }


    private void RotateToCamera()
    {
        Vector3 camPos = Managers.Camera.CameraTrans.position;
        Vector3 targetDir = camPos - transform.position;
        targetDir.y = 0.0f;

        if ((targetDir - transform.position).sqrMagnitude < 0.01f) return;

        Quaternion lookQtrn = Quaternion.LookRotation(targetDir, Vector3.up);

        float angleDiff = Quaternion.Angle(transform.rotation, lookQtrn);

        if (angleDiff < 0.5f)
        {
            transform.rotation = lookQtrn;
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookQtrn, Time.fixedDeltaTime * _rotateSpeed);
        }
    }
    private void UpDownIdle()
    {
        _tween?.Kill();
        _tween = transform.DOMoveY(_bounceAmplitude, _rotateHalfDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetRelative();
    }

}
