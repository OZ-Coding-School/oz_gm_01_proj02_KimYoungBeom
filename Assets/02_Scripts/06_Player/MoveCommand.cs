using UnityEngine;
using DG.Tweening;

public class MoveCommand : ICommand
{
    private PlayerController _player;
    private SpatialNode _fromNode;
    private SpatialNode _toNode;
    private float _duration;

    public MoveCommand(PlayerController player, SpatialNode from, SpatialNode to, float duration)
    {
        _player = player;
        _fromNode = from;
        _toNode = to;
        _duration = duration;
    }

    public void Execute()
    {
        Vector3 targetPos = _toNode.WorldPosition + Defines.PLAYER_Y_OFFSET;
        _player.transform.DOMove(targetPos, _duration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                _player.SetCurrentNode(_toNode);
                _player.IsGoTo = false;
                _player.IsMoving = false;
            });
    }

    public void UnDo()
    {
        Vector3 undoPos = _fromNode.WorldPosition + Defines.PLAYER_Y_OFFSET;

        _player.transform.DOMove(undoPos, _duration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                _player.SetCurrentNode(_fromNode);
                _player.IsGoFrom = false;
                _player.IsMoving = false;
            });
    }
}