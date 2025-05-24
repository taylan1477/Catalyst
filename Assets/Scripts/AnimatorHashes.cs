using UnityEngine;
public static class AnimatorHashes
{
    public static readonly int Speed = Animator.StringToHash("speed");
    public static readonly int IsGrounded = Animator.StringToHash("isGrounded");
    public static readonly int IsCharging = Animator.StringToHash("isCharging");
    public static readonly int IsStoping = Animator.StringToHash("isStoping");
    public static readonly int IsPushing = Animator.StringToHash("isPushing");
    public static readonly int IsPulling = Animator.StringToHash("isPulling");
    public static readonly int IsHolding = Animator.StringToHash("isHolding");
    public static readonly int AttackTrigger = Animator.StringToHash("AttackTrigger");
    public static readonly int DeadTrigger = Animator.StringToHash("DeadTrigger");
    public static readonly int SpawnTrigger = Animator.StringToHash("Spawn");
    public static readonly int HurtTrigger = Animator.StringToHash("HurtTrigger");
    public static readonly int ClimbUp = Animator.StringToHash("ClimbUp");
    
    public static readonly int Walk = Animator.StringToHash("isWalking");
    public static readonly int Dead = Animator.StringToHash("isDead");
    public static readonly int Hurt = Animator.StringToHash("isHurt");
    
    public static readonly int JumpBite = Animator.StringToHash("MonsterJump");
    public static readonly int MonsterGrounded = Animator.StringToHash("MonsterGrounded");
    public static readonly int MonsterDead = Animator.StringToHash("MonsterDead");
    public static readonly int MonsterHurt = Animator.StringToHash("MonsterHurt");
    
}