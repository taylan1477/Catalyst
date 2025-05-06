using UnityEngine;

public class PlayerClimbUp : MonoBehaviour
{
    public float checkRadius = 0.2f;
    public Vector2 climbOffset = new(0f, 1.2f);
    public Transform ledgeCheck;

    [Header("Refs")]
    public PlayerMovement playerMovement;

    private Animator _animator;
    private Rigidbody2D _rb;
    private bool _isClimbing;
    private bool _wasAtLedge;  // Önceki frame durumu

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // 1) Henüz tırmanmıyorsak ve havadaysak
        if (!_isClimbing && !playerMovement.IsGrounded() && _rb.linearVelocity.y <= 0)
        {
            // 2) Şu an köşede miyiz?
            bool isAtLedge = IsAtLedge();

            // 3) Kenara yeni mi geldik?
            if (isAtLedge && !_wasAtLedge)
            {
                StartClimb();
            }

            // 4) Eski durumu güncelle
            _wasAtLedge = isAtLedge;
        }
        else if (playerMovement.IsGrounded())
        {
            // Yere indiğinde resetle ki yeniden tetikleyebilelim
            _wasAtLedge = false;
        }
    }

    private bool IsAtLedge()
    {
        Collider2D lower = Physics2D.OverlapCircle(ledgeCheck.position, checkRadius);
        Collider2D upper = Physics2D.OverlapCircle(
            ledgeCheck.position + Vector3.up * checkRadius * 2, 
            checkRadius
        );

        Debug.Log($"[CLIMB] lower: {lower?.name}, upper: {upper?.name}");
        return lower != null && upper == null;
    }

    private void StartClimb()
    {
        Debug.Log("[CLIMB] Başlıyor");
        _isClimbing = true;
        _animator.SetTrigger(AnimatorHashes.ClimbUp);
        _rb.linearVelocity = Vector2.zero;
        _rb.gravityScale = 0;
    }

    // Animation Event: tam yukarı çekildiği anda
    public void ClimbMove()
    {
        Debug.Log("[CLIMB] Event: ClimbMove");
        Vector2 ofs = new Vector2(
            climbOffset.x * GetFacingDirection(), 
            climbOffset.y
        );
        _rb.position += ofs;
    }

    // Animation Event: animasyon sonu
    public void OnClimbComplete()
    {
        Debug.Log("[CLIMB] Event: OnClimbComplete");
        _rb.gravityScale = 1;
        _isClimbing = false;
        // İsteğe bağlı: yere inmeden yeniden tırmanmayı önlemek için
        // _wasAtLedge = true;
    }

    private int GetFacingDirection()
        => GetComponent<SpriteRenderer>().flipX ? -1 : 1;

    private void OnDrawGizmosSelected()
    {
        if (!ledgeCheck) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(ledgeCheck.position, checkRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            ledgeCheck.position + Vector3.up * checkRadius * 2, 
            checkRadius
        );
    }
}
