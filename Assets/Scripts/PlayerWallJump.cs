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
    private float _wallJumpDirection; // Duvardan zıplama yönü
    private float _wallHoldTime; // Duvara tutunma süresi
    private SpriteRenderer _spriteRenderer;

    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        CheckGrounded();
        CheckWall();
        HandleWallJump();
        HandleWallHoldTime();
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
            float horizontalForce = wallJumpForce * 0.7071f; // cos(45°) ≈ 0.7071
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
}