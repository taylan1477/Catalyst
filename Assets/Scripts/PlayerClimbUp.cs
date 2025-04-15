using UnityEngine;

public class PlayerClimbUp : MonoBehaviour
{
    public Transform climbCheck;               // Duvar tepe kontrolü için boş obje
    public float climbCheckDistance = 0.3f;    // Duvar kontrol mesafesi
    public LayerMask climbableLayer;           // Tırmanılabilir layer
    public float climbDuration = 0.8f;         // Tırmanma animasyon süresi
    public Vector2 climbOffset = new (0f, 1.2f); // Yukarı taşınma mesafesi

    public Transform climbCheckAnchor;         // Karakterin sabit referans noktası
    public Vector2 climbCheckLocalOffset = new (0.5f, 1.2f); // Yön bağımlı offset

    private Animator _animator;
    private Rigidbody2D _rb;
    private bool _isClimbing;
    private bool _canClimb;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (_isClimbing)
        {
            Debug.Log("Tırmanma hâlinde, giriş engellendi.");
            return;
        }

        CheckClimbableWall();

        if (_canClimb && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Climb());
        }
    }

    void LateUpdate()
    {
        if (climbCheck == null || climbCheckAnchor == null) return;

        float direction = GetFacingDirection();
        Vector3 offset = new Vector3(climbCheckLocalOffset.x * direction, climbCheckLocalOffset.y, 0);
        climbCheck.position = climbCheckAnchor.position + offset;
    }

    void CheckClimbableWall()
    {
        Vector2 direction = Vector2.right * GetFacingDirection();
        RaycastHit2D hit = Physics2D.Raycast(climbCheck.position, direction, climbCheckDistance, climbableLayer);
        _canClimb = hit.collider != null;

        if (_canClimb)
            Debug.Log("Tırmanılabilir duvar bulundu: " + hit.collider.name);

        Debug.DrawRay(climbCheck.position, direction * climbCheckDistance, _canClimb ? Color.green : Color.red);
    }

    int GetFacingDirection()
    {
        return GetComponent<SpriteRenderer>().flipX ? -1 : 1;
    }

    System.Collections.IEnumerator Climb()
    {
        _isClimbing = true;
        
        _animator.SetTrigger(AnimatorHashes.ClimbUp);

        _rb.linearVelocity = Vector2.zero;
        _rb.gravityScale = 0;

        yield return new WaitForSeconds(climbDuration);

        // Yukarı + yönüne göre offset
        Vector2 finalOffset = new Vector2(climbOffset.x * GetFacingDirection(), climbOffset.y);
        _rb.MovePosition(_rb.position + finalOffset);
        Debug.Log("Karakter yukarı taşındı.");

        _rb.gravityScale = 1;
        _isClimbing = false;
    }
}
