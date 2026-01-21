using UnityEngine;

public static class Defines
{
    //씬
    public const string SCENE_LOBBY = "Lobby";
    public const string SCENE_STAGE = "Stage";

    //플레이어
    public static readonly Vector3 PLAYER_Y_OFFSET = new Vector3(0.0f, 0.5f, 0.0f);

    //노드
    public const int MAX_NODE_COUNT = 50;

    //애니메이션
    public static readonly int IDLE_HASH = Animator.StringToHash("Idle");
    public static readonly int SAD_IDLE_HASH = Animator.StringToHash("SadIdle");
    public static readonly int VICTORY_IDLE_HASH = Animator.StringToHash("VictoryIdle");
    public static readonly int JUMP_HASH = Animator.StringToHash("Jump");
    public static readonly int ATTACK_HASH = Animator.StringToHash("Attack");
    public static readonly int DEATH_HASH = Animator.StringToHash("Death");
    public static readonly int MOVE_HASH = Animator.StringToHash("Move");

    public static readonly int ANIM_SPEED_HASH = Animator.StringToHash("AnimSpeed");

    //데이터매니저
    public static readonly int CAM_SENS_MAX = 5;
    public static readonly int CAM_SENS_MIN = 1;
}
public enum ENodeShape
{
    Cross, Horizontal, Vertical, UpRight, UpLeft, DownRight, DownLeft, TUp, TDown, TRight, TLeft, None
}
public enum ENodeState
{
    None, Start, Finish, Moving, Key, OnEnemyUp, OnEnemyDown, OnEnemyLeft, OnEnemyRight, MovingEnemy
}
public enum ENodeTrail
{
    None, Horizontal, Vertical, UpRight, UpLeft, DownRight, DownLeft, Up, Left, Right, Down
}
public enum EViewMode
{
    Intro, Quarter, Top, FirstPerson, Lobby
}