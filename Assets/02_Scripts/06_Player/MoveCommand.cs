using UnityEngine;
using DG.Tweening;

public class MoveCommand : ICommand
{
    private PlayerController _player;
    private SpatialNode _fromNode;
    private SpatialNode _toNode;
    private float _duration;

    public Vector2Int MoveDir { get; private set; }
    public MoveCommand(PlayerController player, SpatialNode from, SpatialNode to, float duration)
    {
        _player = player;
        _fromNode = from;
        _toNode = to;
        _duration = duration;
        MoveDir = to.GridCoordinate - from.GridCoordinate;
    }

    public void Execute()
    {
        Vector3 targetPos = _toNode.WorldCoordinate + Defines.PLAYER_Y_OFFSET;
        _player.transform.DOMove(targetPos, _duration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                _player.SetCurrentNode(_toNode);
                _player.IsGoTo = false;
                _player.IsMoving = false;
                _player.OnPlayerMoving.Raised(_player.IsMoving);
                _player.OnPlayerTurnEnd.Raised();
            });
    }

    public void UnDo()
    {
        Vector3 undoPos = _fromNode.WorldCoordinate + Defines.PLAYER_Y_OFFSET;
        _player.NotifySpecialNode(_fromNode, _player.CurrentNode);
        _player.transform.DOMove(undoPos, _duration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                _player.SetCurrentNode(_fromNode);
                _player.IsGoFrom = false;
                _player.IsMoving = false;
                _player.OnPlayerMoving.Raised(_player.IsMoving);
            });
    }
    public bool CheckUnDo()
    {
        if (_player.CurrentView != EViewMode.Top && _fromNode.WorldCoordinate.y != _toNode.WorldCoordinate.y)
        {
            return false;
        }
        return true;
    }
}