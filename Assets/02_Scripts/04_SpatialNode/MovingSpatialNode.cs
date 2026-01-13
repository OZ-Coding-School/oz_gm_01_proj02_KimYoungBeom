using DG.Tweening;
using UnityEngine;

public class MovingSpatialNode : SpatialNode, IStageMovable
{
    private float _moveDuration = 0.5f;
    private Vector3Int dir = Vector3Int.forward;
    public async Awaitable ExecuteStageTurn()
    {
        //이동로직 BT로?

        //테스트로 위 아래 번갈아 이동
        var node = Managers.Stage.GetNodeAt(WorldCoordinate + dir);
        if (node != null)
        {
            dir = -dir;
        }
        Vector3Int targetPos = WorldCoordinate + dir;
        await transform.DOMove(targetPos, _moveDuration)
            .SetEase(Ease.OutQuad)
            .Awaiting();

        Managers.Stage.UpdateNode(this, WorldCoordinate, targetPos);
    }
}
