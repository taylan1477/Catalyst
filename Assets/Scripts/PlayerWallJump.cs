using UnityEngine;

public class PlayerWallJump : MonoBehaviour
{
    public float wallJumpForce = 16f;
    public float wallCheckDistance = 0.1f;
    public float maxWallHoldTime = 1f;
    public LayerMask wallLayer;
    public LayerMask groundLayer;
    public Transform wallCheck;

    private Rigidbody2D _rb;
    private SpriteRenderer _sr;
    private Animator _anim;
    private BoxCollider2D _collider;
    private Vector2 _originalOffset;

    private bool _isTouchingWall;
    private bool _isGrounded;
    private bool _isHolding;
    private float _wallHoldTimer;
    private float _wallHoldTime; // Duvara tutunma süresi
    private float _lastWallTouchTime;
    public float wallJumpCoyoteTime = 0.2f; // Zıplama toleransı süresi

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _anim = GetComponent<Animator>();
        _collider = GetComponent<BoxCollider2D>();
        _originalOffset = _collider.offset;
    }

    void Update()
    {
        CheckGrounded();
        CheckWall();
        HandleWallHoldTime();
        HandleWallJump();
        UpdateColliderPosition();

        if (!_isGrounded)
        {
            _anim.SetBool(AnimatorHashes.IsHolding, _isHolding);
        }
    }

    void CheckGrounded()
    {
        _isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, groundLayer);
    }
    public bool IsTouchingWall()
    {
        return _isTouchingWall;
    }

    void CheckWall()
    {
        float direction = _sr.flipX ? -1 : 1;

        // Wall check pozisyonunu güncelle
        wallCheck.localPosition = new Vector3(Mathf.Abs(wallCheck.localPosition.x) * direction, wallCheck.localPosition.y, wallCheck.localPosition.z);

        // Raycast ile duvar kontrolü
        Vector2 checkDirection = Vector2.right * direction;
        bool isCurrentlyTouchingWall = Physics2D.Raycast(wallCheck.position, checkDirection, wallCheckDistance, wallLayer);
    
        // Yerçekimi ve tutunma kontrolü
        if (isCurrentlyTouchingWall && !_isGrounded)
        {
            _lastWallTouchTime = Time.time;
            _wallHoldTimer += Time.deltaTime;
            _isHolding = true;
            _rb.gravityScale = 5f;

            if (_wallHoldTimer >= maxWallHoldTime)
            {
                _isHolding = false; // Artık tutunamıyor
            }
        }
        else
        {
            _wallHoldTimer = 0f;
            _isHolding = false;
            _rb.gravityScale = 6f;
        }

        // En sonda _isTouchingWall'ı güncelle
        _isTouchingWall = isCurrentlyTouchingWall && _isHolding;

        Debug.DrawRay(wallCheck.position, checkDirection * wallCheckDistance, _isTouchingWall ? Color.green : Color.red);
    }


    void HandleWallHoldTime()
    {
        if (_isTouchingWall && !_isGrounded)
        {
            _wallHoldTime += Time.deltaTime;

            if (_wallHoldTime >= maxWallHoldTime)
            {
                _isTouchingWall = false;
                _isHolding = false;
                _wallHoldTime = 0f;

                // Duvardan kaymasın diye velocity'yi hafifçe azalt
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, Mathf.Min(_rb.linearVelocity.y, 0f));
            }
        }
        else
        {
            _wallHoldTime = 0f;
        }
    }


    void HandleWallJump()
    {
        bool canWallJump = (Time.time - _lastWallTouchTime <= wallJumpCoyoteTime);

        if (!_isGrounded && canWallJump && Input.GetKeyDown(KeyCode.Space))
        {
            float direction = _sr.flipX ? 1 : -1; // Zıplama yönü ters olacak

            float horizontalForce = wallJumpForce * 0.7071f * direction;
            float verticalForce = wallJumpForce * 0.7071f;

            _rb.linearVelocity = new Vector2(horizontalForce, verticalForce);
            _isTouchingWall = false; // Hemen sonra tekrar zıplamasın
            _wallHoldTime = 0f;
        }
    }


    void UpdateColliderPosition()
    {
        if (_isHolding)
        {
            if (!_sr.flipX) // Sağa bakıyorsa
                _collider.offset = new Vector2(_originalOffset.x - 0.05f, _originalOffset.y);
            else
                _collider.offset = new Vector2(_originalOffset.x + 0.05f, _originalOffset.y);
        }
        else
        {
            _collider.offset = _originalOffset;
        }
    }
}