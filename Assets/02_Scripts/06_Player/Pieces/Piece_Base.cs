using DG.Tweening;
using UnityEngine;

public abstract class Piece_Base : PoolableComponent
{
    [SerializeField] protected SpatialNodeEventCHSO _onNotifySpecialNode;   //PlayerController ¹ß¼Û

    protected float _rotateHalfDuration = 1.0f;
    protected float _bounceAmplitude = 0.15f;
    protected float _rotateSpeed = 8.0f;
    protected Tween _upDownTween;

    public SpatialNode GroundNode { get; private set; }

    public override void OnSpawn()
    {
        _onNotifySpecialNode.onEvent += HandleNotify;
        Managers.Stage.onIntroEnd += HandleIntroEnd;
    }
    public override void OnDespawn()
    {
        _onNotifySpecialNode.onEvent -= HandleNotify;
        Managers.Stage.onIntroEnd -= HandleIntroEnd;
    }
    public virtual void InjectNode(SpatialNode groundNode)
    {
        GroundNode = groundNode;
    }
    protected abstract void HandleNotify(SpatialNode node);
    protected abstract void HandleIntroEnd();

    protected virtual void RotateToTarget(Vector3 targetPos)
    {
        Vector3 targetDir = targetPos - transform.position;
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
    protected virtual void RotateToCamera()
    {
        Vector3 camPos = Managers.Camera.CameraTrans.position;
        RotateToTarget(camPos);
    }


}
