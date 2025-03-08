using UnityEngine;

public class PlayerWallJump : MonoBehaviour
{
    public float wallJumpForce = 25f; // Duvardan zıplama kuvveti
    public LayerMask wallLayer; // Duvarlar için layer
    public LayerMask groundLayer; // Yer için layer
    public Transform wallCheck; // Duvara değme kontrolü için boş obje
    public float wallCheckDistance = 0.1f; // Duvara değme kontrol mesafesi
    public float maxWallHoldTime = 1f; // Duvara maksimum tutunma süresi (1 saniye)

    private Rigidbody2D _rigidbody2D;
    private bool _isTouchingWall; // Duvara değiyor mu?
    private bool _isGrounded; // Karakter yerde mi?
    private bool _isHolding; // Duvara tutunuyor mu?
    private float _wallJumpDirection; // Duvardan zıplama yönü
    private float _wallHoldTime; // Duvara tutunma süresi
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private BoxCollider2D _collider; // Karakterin collider'ı
    private Vector2 _originalOffset; // Collider'ın orijinal offset değeri

    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<BoxCollider2D>();
        _originalOffset = _collider.offset;
    }

    void Update()
    {
        CheckGrounded();
        CheckWall();
        HandleWallJump();
        HandleWallHoldTime();
        UpdateColliderPosition();
        if (!_isGrounded)
        {
            _animator.SetBool(AnimatorHashes.IsHolding, _isHolding);
        }
    }

    void CheckGrounded()
    {
        // Yerde olup olmadığını kontrol et
        _isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, groundLayer);
    }

    void CheckWall()
    {
        if (wallCheck == null || _spriteRenderer == null)
        {
            return; // Objeler atanmamışsa fonksiyonu durdur
        }

        // Karakterin baktığı yönü al
        float direction = _spriteRenderer.flipX ? -1 : 1;

        // WallCheck objesinin konumunu güncelle
        wallCheck.localPosition = new Vector3(Mathf.Abs(wallCheck.localPosition.x) * direction, wallCheck.localPosition.y, wallCheck.localPosition.z);

        // WallCheck objesinin yönünü güncelle
        Vector2 checkDirection = transform.right * direction;

        // Duvara değme kontrolü
        _isTouchingWall = Physics2D.Raycast(wallCheck.position, checkDirection, wallCheckDistance, wallLayer);

        // Duvara değiyorsa isHolding true, değmiyorsa false yap
        _isHolding = _isTouchingWall;

        // Debug çizgisi
        Debug.DrawRay(wallCheck.position, checkDirection * wallCheckDistance, _isTouchingWall ? Color.green : Color.red);
    }

    void HandleWallJump()
    {
        // Karakter yerde değilse ve duvara değiyorsa, duvardan zıpla
        if (!_isGrounded && _isTouchingWall && Input.GetKeyDown(KeyCode.Space))
        {
            // Duvara göre zıplama yönü
            _wallJumpDirection = _isTouchingWall ? -1 : 1;

            // 45 derecelik itme kuvveti (yatay ve dikey kuvvetler eşit)
            float horizontalForce = wallJumpForce * 0.7071f * Mathf.Sign(_rigidbody2D.linearVelocity.x);  // cos(45°) ≈ 0.7071
            float verticalForce = wallJumpForce * 0.7071f;  // sin(45°) ≈ 0.7071

            // Kuvveti uygula
            _rigidbody2D.linearVelocity = new Vector2(horizontalForce * _wallJumpDirection, verticalForce);
        }
    }

    void HandleWallHoldTime()
    {
        // Duvara değiyorsa tutunma süresini artır
        if (_isTouchingWall)
        {
            _wallHoldTime += Time.deltaTime;

            // Tutunma süresi dolduysa
            if (_wallHoldTime >= maxWallHoldTime)
            {
                _isTouchingWall = false; // Duvardan ayrıl
                _wallHoldTime = 0f; // Süreyi sıfırla
            }
        }
        else
        {
            _wallHoldTime = 0f; // Duvara değmiyorsa süreyi sıfırla
        }
    }
    void UpdateColliderPosition()
    {
        if (_isHolding)
        {
            if (!_spriteRenderer.flipX) // Sağa bakıyorsa
            {
                _collider.offset = new Vector2(_originalOffset.x - 0.05f, _collider.offset.y); // Sola kaydır
            }
            else // Sola bakıyorsa
            {
                _collider.offset = new Vector2(_originalOffset.x + 0.05f, _collider.offset.y); // Sağa kaydır
            }
        }
        else
        {
            _collider.offset = _originalOffset;
        }
    }
}