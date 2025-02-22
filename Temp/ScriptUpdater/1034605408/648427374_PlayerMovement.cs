using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 0.0f; // Mevcut hız
    public float acceleration = 0.015f; // Hızlanma
    public float deceleration = 0.04f; // Yavaşlama
    public float maxSpeed = 2.5f; // Maks hız
    public float jumpForce = 10f; // Zıplama kuvveti
    public float groundCheckDistance = 0.1f; // Yerde olup olmadığını kontrol etmek için mesafe
    public LayerMask groundLayer; // Yer katmanı

    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private bool _isGrounded; // Karakter yerde mi?

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
        CheckGrounded(); // Yerde olup olmadığını kontrol et
        HandleJump(); // Zıplamayı kontrol et
    }

    void HandleMovement()
    {
        if (Input.GetKey(KeyCode.D)) // Sağa hareket
        {
            if (speed < 0) // Eğer sola gidiyorsa, hızı tersine çevir
            {
                speed *= -1;
            }
            speed += acceleration;
            _spriteRenderer.flipX = false; // Sprite'ı sağa dönük yap
        }
        else if (Input.GetKey(KeyCode.A)) // Sola hareket
        {
            if (speed > 0) // Eğer sağa gidiyorsa, hızı tersine çevir
            {
                speed *= -1;
            }
            speed -= acceleration;
            _spriteRenderer.flipX = true; // Sprite'ı sola dönük yap
        }
        else // Tuşa basılmıyorsa yavaşla
        {
            speed = Mathf.MoveTowards(speed, 0, deceleration);
        }

        // Hızı sınırla
        speed = Mathf.Clamp(speed, -maxSpeed, maxSpeed);
    }

    void ApplyMovement()
    {
        // Hızı yön ile çarp ve karaktere uygula
        _rigidbody2D.linearVelocity = new Vector2(speed * 4, _rigidbody2D.linearVelocity.y);
    }

    void UpdateAnimator()
    {
        // Animator'a hızın mutlak değerini ilet
        _animator.SetFloat("speed", Mathf.Abs(speed));

        // Animator'a yerde olup olmadığını ilet
        _animator.SetBool("isGrounded", _isGrounded);
    }

    void CheckGrounded()
    {
        // Karakterin altında bir ışın (ray) çiz ve yerde olup olmadığını kontrol et
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        _isGrounded = hit.collider != null; // Eğer bir çarpışma varsa, karakter yerde demektir
    }

    void HandleJump()
    {
        if (_isGrounded && Input.GetKeyDown(KeyCode.Space)) // Yerdeyse ve Space tuşuna basıldıysa
        {
            _rigidbody2D.linearVelocity = new Vector2(_rigidbody2D.linearVelocity.x, jumpForce); // Zıplama kuvveti uygula
        }
    }
}