using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 0.0f; // Mevcut hız
    public float acceleration = 0.02f; // Hızlanma
    public float deceleration = 0.03f; // Yavaşlama
    public float maxSpeed = 3.3f; // Maks hız
    public float normalJumpForce = 20f; // Normal zıplama kuvveti
    public float chargedJumpForce = 28f; // Charged zıplama kuvveti
    public float chargeThreshold = 0.6f; // Charged Jump için gereken süre
    public float groundCheckDistance = 0.1f; // Yerde olup olmadığını kontrol etmek için mesafe
    public LayerMask groundLayer; // Yer katmanı

    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private bool _isGrounded; // Karakter yerde mi?
    private bool _isCharging = false;
    private float _chargeStartTime = 0f;

    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        HandleMovement();
        ApplyMovement();
        UpdateAnimator();
        CheckGrounded();
        HandleJump();
    }

    void HandleMovement()
    {
        if (Input.GetKey(KeyCode.D)) // Sağa hareket
        {
            if (speed < 0) speed *= -1; // Eğer sola gidiyorsa, hızı tersine çevir
            speed += acceleration;
            _spriteRenderer.flipX = false;
        }
        else if (Input.GetKey(KeyCode.A)) // Sola hareket
        {
            if (speed > 0) speed *= -1;
            speed -= acceleration;
            _spriteRenderer.flipX = true;
        }
        else // Tuşa basılmıyorsa yavaşla
        {
            speed = Mathf.MoveTowards(speed, 0, deceleration);
        }

        speed = Mathf.Clamp(speed, -maxSpeed, maxSpeed); // Hızı sınırla
    }

    void ApplyMovement()
    {
        _rigidbody2D.linearVelocity = new Vector2(speed * 4, _rigidbody2D.linearVelocity.y);
    }

    void UpdateAnimator()
    {
        _animator.SetFloat("speed", Mathf.Abs(speed));
        _animator.SetBool("isGrounded", _isGrounded);
        _animator.SetBool("isCharging", _isCharging);
    }

    void CheckGrounded()
    {
        Vector2 origin = (Vector2)transform.position + Vector2.down * 0.5f;
        float radius = 0.2f;
        float distance = groundCheckDistance;
        Vector2 direction = Vector2.down;

        RaycastHit2D hit = Physics2D.CircleCast(origin, radius, direction, distance, groundLayer);
        _isGrounded = hit.collider != null;

        Debug.DrawRay(origin, direction * distance, _isGrounded ? Color.green : Color.red);
    }

    void HandleJump()
    {
        if (_isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _isCharging = true;
                _chargeStartTime = Time.time; // Basıldığı an kaydedilir
            }

            if (Input.GetKeyUp(KeyCode.Space) && _isCharging)
            {
                float heldTime = Time.time - _chargeStartTime;
                float jumpPower = (heldTime >= chargeThreshold) ? chargedJumpForce : normalJumpForce;

                _rigidbody2D.linearVelocity = new Vector2(_rigidbody2D.linearVelocity.x, jumpPower);
                
                _isCharging = false;
            }
        }
    }
}