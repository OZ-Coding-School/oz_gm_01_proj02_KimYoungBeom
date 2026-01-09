using UnityEngine;

public static class Defines
{
    public const string SCENE_LOBBY = "Lobby";
    public const string SCENE_STAGE = "Stage";
    public static readonly Vector3 PLAYER_Y_OFFSET = new Vector3(0.0f, 0.5f, 0.0f);
    public static readonly int IDLE_HASH = Animator.StringToHash("Idle");
    public static readonly int SAD_IDLE_HASH = Animator.StringToHash("SadIdle");
    public static readonly int VICTORY_IDLE_HASH = Animator.StringToHash("VictoryIdle");
    public static readonly int JUMP_HASH = Animator.StringToHash("Jump");
    public static readonly int ANIM_SPEED_HASH = Animator.StringToHash("AnimSpeed");
}
public enum ENodeShape
{
    Cross, Horizontal, Vertical, UpRight, UpLeft, DownRight, DownLeft, TUp, TDown, TRight, TLeft
}
public enum ENodeState
{
    None, Start, Finish
}
public enum EViewMode
{
    Intro, Quarter, Top, FirstPerson, Lobby
}