using UnityEngine;
public static class AnimatorHashes
{
    public static readonly int Speed = Animator.StringToHash("speed");
    public static readonly int IsGrounded = Animator.StringToHash("isGrounded");
    public static readonly int IsCharging = Animator.StringToHash("isCharging");
    public static readonly int IsStoping = Animator.StringToHash("isStoping");
    public static readonly int IsPushing = Animator.StringToHash("isPushing");
    
    public static readonly int AttackTrigger = Animator.StringToHash("AttackTrigger");
    
}