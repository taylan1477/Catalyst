using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float acceleration = 0.012f; // Hızlanma oranı
    public float deceleration = 0.02f;  // Yavaşlama oranı
    public float maxSpeed = 2.0f;       // Maksimum hız
    private float speed = 0.0f;         // Mevcut hız
    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer; // Sprite Renderer bileşeni

    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>(); // Sprite Renderer'ı al
    }

    void Update()
    {
        HandleMovement();
        ApplyMovement();
        UpdateAnimator();
    }

    void HandleMovement()
    {
        if (Input.GetKey(KeyCode.D)) // Sağa hareket
        {
            speed += acceleration;
            _spriteRenderer.flipX = false; // Sprite'ı sağa dönük yap
        }
        else if (Input.GetKey(KeyCode.A)) // Sola hareket
        {
            speed += acceleration;
            _spriteRenderer.flipX = true; // Sprite'ı sola dönük yap
        }
        else // Tuşa basılmıyorsa yavaşla
        {
            speed -= deceleration;
        }

        // Hızı sınırla
        speed = Mathf.Clamp(speed, 0, maxSpeed);
    }

    void ApplyMovement()
    {
        // Hızı yön ile çarp ve karaktere uygula
        _rigidbody2D.linearVelocity = new Vector2(speed, _rigidbody2D.linearVelocity.y);
    }

    void UpdateAnimator()
    {
        // Animator'a hızı ilet
        _animator.SetFloat("speed", Mathf.Abs(speed)); // Hızın mutlak değerini kullan
    }
}