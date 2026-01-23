using DG.Tweening;
using UnityEngine;

public class DeathState : PlayerState
{
    public DeathState(PlayerController player, IState parent = null) : base(player, parent) { }

    private readonly float _downEndTimeRate = 51.0f / 181.0f;
    public override void Enter()
    {
        _elapsedTimeBase = 0.0f;
        Vector3 enemyBackward = new Vector3(-_player.EnemyForwardDir.x, 0.0f, -_player.EnemyForwardDir.y);
        Vector3 lookDir = Quaternion.Euler(0.0f, 45.0f, 0.0f) * enemyBackward;
        _player.transform.rotation = Quaternion.LookRotation(lookDir, Vector3.up);
        _player.Anim.applyRootMotion = true;
        _player.Anim.CrossFadeInFixedTime(Defines.DEATH_HASH, 0.1f);
    }
    public override void Update() { }
    public override void FixedUpdate()
    {
        _elapsedTimeBase += Time.fixedDeltaTime;
        if (_elapsedTimeBase > _player.GetClipLength(Defines.DEATH_HASH) * _downEndTimeRate)
        {
            _elapsedTimeBase = 0.0f;
            if (_player.IsAvatar)
            {
                Managers.Input.IsPlayerDeath = false;
                _player.OnNotifyDeath?.Raised(_player.IsAvatar);
                _player.ReturnPool();
            }
            else Managers.Input.ExecuteReloadStage();
        }
    }
    public override void Exit()
    {
        _elapsedTimeBase = 0.0f;
    }
}
