using UnityEngine;
using System.Collections;

public class PlayerClimbUp : MonoBehaviour
{
    [Header("Climb Settings")]
    public float checkRadius = 0.2f;           // Çember yarıçapı
    public Vector2 climbOffset = new(0f, 1.2f); // Yukarı taşınma mesafesi
    public float climbDuration = 0.8f;         // Animasyon süresi

    [Header("References")]
    public Transform ledgeCheck;               // Köşe kontrol noktası

    private Animator _animator;
    private Rigidbody2D _rb;
    private bool _isClimbing;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (_isClimbing) return;

        if (IsAtLedge())
        {
            StartClimb();
        }
    }

    private bool IsAtLedge()
    {
        Collider2D lowerHit = Physics2D.OverlapCircle(ledgeCheck.position, checkRadius);
        Vector2 topCheckPos = ledgeCheck.position + Vector3.up * (checkRadius * 2f);
        Collider2D upperHit = Physics2D.OverlapCircle(topCheckPos, checkRadius);

        Debug.Log($"[CLIMB] lowerHit: {(lowerHit != null ? lowerHit.name : "none")}, upperHit: {(upperHit != null ? upperHit.name : "none")}");

        if (lowerHit != null && upperHit == null)
        {
            Debug.Log("[CLIMB] Geçerli köşe tespit edildi.");
            return true;
        }

        return false;
    }


    private void StartClimb()
    {
        Debug.Log("[CLIMB] Tırmanma başlatılıyor...");
        _isClimbing = true;
        _animator.SetTrigger(AnimatorHashes.ClimbUp);

        _rb.linearVelocity = Vector2.zero;
        _rb.gravityScale = 0;

        StartCoroutine(ClimbRoutine());
    }


    private IEnumerator ClimbRoutine()
    {
        yield return new WaitForSeconds(climbDuration);

        // Karakteri yukarı taşı
        Vector2 ofs = new Vector2(climbOffset.x * GetFacingDirection(), climbOffset.y);
        _rb.position += ofs;

        // Reset
        _rb.gravityScale = 1;
        _isClimbing = false;
    }

    private int GetFacingDirection()
    {
        return GetComponent<SpriteRenderer>().flipX ? -1 : 1;
    }

    private void OnDrawGizmosSelected()
    {
        if (ledgeCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(ledgeCheck.position, checkRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(ledgeCheck.position + Vector3.up * (checkRadius * 2f), checkRadius);
    }
}
